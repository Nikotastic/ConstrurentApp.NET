import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { LoadingSpinnerComponent, AlertComponent } from '@presentation/shared';
import { API_CONFIG } from '@core/config/app.config';

interface FileUploadResponse {
  url: string;
  fileName: string;
  size: number;
  contentType: string;
}

/**
 * Image Upload Component
 * Reusable component for uploading images to S3
 */
@Component({
  selector: 'app-image-upload',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent, AlertComponent],
  template: `
    <div class="image-upload-container">
      @if (errorMessage) {
      <app-alert
        type="error"
        [message]="errorMessage"
        (dismissed)="errorMessage = ''"
      >
      </app-alert>
      }

      <div class="upload-area" [class.has-image]="imageUrl">
        @if (imageUrl) {
        <div class="image-preview">
          <img [src]="imageUrl" [alt]="fileName" />
          <button
            type="button"
            class="btn-remove"
            (click)="removeImage()"
            [disabled]="isUploading"
          >
            ✕
          </button>
        </div>
        } @else {
        <div class="upload-placeholder">
          <input
            type="file"
            #fileInput
            accept="image/*"
            (change)="onFileSelected($event)"
            [disabled]="isUploading"
            class="file-input"
          />

          @if (isUploading) {
          <app-loading-spinner [size]="40"></app-loading-spinner>
          } @else {
          <div class="upload-icon">📷</div>
          <p class="upload-text">Click to upload or drag and drop</p>
          <p class="upload-hint">PNG, JPG, GIF up to 5MB</p>
          }
        </div>
        }
      </div>
    </div>
  `,
  styles: [
    `
      .image-upload-container {
        width: 100%;
      }

      .upload-area {
        border: 2px dashed #cbd5e0;
        border-radius: 8px;
        padding: 2rem;
        text-align: center;
        transition: all 0.3s;
        cursor: pointer;
        background: #f7fafc;
      }

      .upload-area:hover {
        border-color: #4299e1;
        background: #ebf8ff;
      }

      .upload-area.has-image {
        padding: 0;
        border: none;
        background: transparent;
      }

      .file-input {
        position: absolute;
        width: 100%;
        height: 100%;
        top: 0;
        left: 0;
        opacity: 0;
        cursor: pointer;
      }

      .upload-placeholder {
        position: relative;
      }

      .upload-icon {
        font-size: 3rem;
        margin-bottom: 1rem;
      }

      .upload-text {
        font-size: 1rem;
        color: #2d3748;
        margin-bottom: 0.5rem;
      }

      .upload-hint {
        font-size: 0.875rem;
        color: #718096;
      }

      .image-preview {
        position: relative;
        display: inline-block;
      }

      .image-preview img {
        max-width: 100%;
        max-height: 300px;
        border-radius: 8px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
      }

      .btn-remove {
        position: absolute;
        top: 8px;
        right: 8px;
        background: rgba(0, 0, 0, 0.7);
        color: white;
        border: none;
        border-radius: 50%;
        width: 32px;
        height: 32px;
        cursor: pointer;
        font-size: 1.2rem;
        line-height: 1;
        transition: background 0.2s;
      }

      .btn-remove:hover {
        background: rgba(220, 38, 38, 0.9);
      }

      .btn-remove:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    `,
  ],
})
export class ImageUploadComponent {
  @Input() uploadType: 'product' | 'vehicle' | 'profile' = 'product';
  @Input() imageUrl: string = '';
  @Output() imageUrlChange = new EventEmitter<string>();
  @Output() uploadComplete = new EventEmitter<FileUploadResponse>();

  isUploading = false;
  errorMessage = '';
  fileName = '';

  constructor(private http: HttpClient) {}

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    const file = input.files[0];
    this.uploadFile(file);
  }

  private uploadFile(file: File): void {
    // Validate file
    const error = this.validateFile(file);
    if (error) {
      this.errorMessage = error;
      return;
    }

    this.isUploading = true;
    this.errorMessage = '';

    const formData = new FormData();
    formData.append('file', file);

    const endpoint = `${API_CONFIG.BASE_URL}/files/upload/${this.uploadType}`;

    this.http.post<FileUploadResponse>(endpoint, formData).subscribe({
      next: (response) => {
        this.imageUrl = response.url;
        this.fileName = response.fileName;
        this.imageUrlChange.emit(response.url);
        this.uploadComplete.emit(response);
        this.isUploading = false;
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Failed to upload image. Please try again.';
        this.isUploading = false;
      },
    });
  }

  removeImage(): void {
    this.imageUrl = '';
    this.fileName = '';
    this.imageUrlChange.emit('');
  }

  private validateFile(file: File): string | null {
    const maxSize = 5 * 1024 * 1024; // 5 MB
    const allowedTypes = [
      'image/jpeg',
      'image/jpg',
      'image/png',
      'image/gif',
      'image/webp',
    ];

    if (file.size > maxSize) {
      return 'File size exceeds 5 MB';
    }

    if (!allowedTypes.includes(file.type)) {
      return 'Invalid file type. Please upload an image (JPG, PNG, GIF, WEBP)';
    }

    return null;
  }
}
