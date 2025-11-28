import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormsModule,
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import {
  ProfileService,
  UserProfile,
} from '@application/services/profile.service';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  template: `
    <div class="container mx-auto px-4 py-8">
      <div class="max-w-4xl mx-auto">
        <!-- Header -->
        <div class="mb-8">
          <h1 class="text-3xl font-bold text-gray-800">👤 Profile</h1>
          <p class="text-gray-600">
            Manage your personal information and preferences
          </p>
        </div>

        <div class="bg-white rounded-2xl shadow-xl overflow-hidden">
          <!-- Banner / Cover (Optional aesthetic touch) -->
          <div class="h-32 bg-gradient-to-r from-blue-600 to-indigo-700"></div>

          <div class="px-8 pb-8">
            <!-- Profile Header with Avatar -->
            <div class="relative flex justify-between items-end -mt-12 mb-8">
              <div class="flex items-end">
                <div class="relative group">
                  <div
                    class="w-32 h-32 rounded-full border-4 border-white shadow-lg overflow-hidden bg-gray-200"
                  >
                    <img
                      [src]="
                        profile?.photoUrl ||
                        'https://ui-avatars.com/api/?name=' +
                          (profile?.fullName || 'User') +
                          '&background=random'
                      "
                      alt="Profile"
                      class="w-full h-full object-cover"
                    />

                    <!-- Upload Overlay -->
                    <label
                      class="absolute inset-0 bg-black/50 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity cursor-pointer text-white font-semibold text-sm"
                    >
                      <span>📷 Change</span>
                      <input
                        type="file"
                        class="hidden"
                        (change)="onFileSelected($event)"
                        accept="image/*"
                      />
                    </label>
                  </div>

                  <!-- Uploading Spinner -->
                  <div
                    *ngIf="uploadingImage"
                    class="absolute inset-0 flex items-center justify-center bg-white/80 rounded-full z-20"
                  >
                    <div
                      class="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"
                    ></div>
                  </div>
                </div>

                <div class="ml-6 mb-2">
                  <h2 class="text-2xl font-bold text-gray-800">
                    {{ profile?.fullName }}
                  </h2>
                  <p class="text-gray-500">{{ profile?.email }}</p>
                </div>
              </div>
            </div>

            <!-- Form -->
            <form
              [formGroup]="profileForm"
              (ngSubmit)="onSubmit()"
              class="space-y-6"
            >
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <!-- Full Name -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2"
                    >Full Name</label
                  >
                  <input
                    type="text"
                    formControlName="fullName"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  />
                  <p
                    *ngIf="
                      profileForm.get('fullName')?.touched &&
                      profileForm.get('fullName')?.invalid
                    "
                    class="text-red-500 text-xs mt-1"
                  >
                    El nombre es requerido
                  </p>
                </div>

                <!-- Email (Read Only) -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2"
                    >Email</label
                  >
                  <input
                    type="email"
                    formControlName="email"
                    class="w-full px-4 py-2 border border-gray-200 rounded-lg bg-gray-50 text-gray-500 cursor-not-allowed"
                    readonly
                  />
                </div>

                <!-- Phone -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2"
                    >Phone</label
                  >
                  <input
                    type="tel"
                    formControlName="phone"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  />
                </div>

                <!-- Address -->
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-gray-700 mb-2"
                    >Address</label
                  >
                  <input
                    type="text"
                    formControlName="address"
                    class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                  />
                </div>
              </div>

              <!-- Actions -->
              <div
                class="flex items-center justify-end pt-6 border-t border-gray-100"
              >
                <p
                  *ngIf="successMessage"
                  class="text-green-600 mr-4 font-medium animate-fade-in"
                >
                  {{ successMessage }}
                </p>
                <p
                  *ngIf="errorMessage"
                  class="text-red-600 mr-4 font-medium animate-fade-in"
                >
                  {{ errorMessage }}
                </p>

                <button
                  type="submit"
                  [disabled]="profileForm.invalid || saving"
                  class="bg-blue-600 hover:bg-blue-700 text-white font-bold py-2.5 px-6 rounded-lg shadow-md hover:shadow-lg transition-all transform hover:-translate-y-0.5 disabled:opacity-50 disabled:cursor-not-allowed flex items-center"
                >
                  <span *ngIf="saving" class="mr-2">
                    <div
                      class="animate-spin rounded-full h-4 w-4 border-b-2 border-white"
                    ></div>
                  </span>
                  {{ saving ? 'Saving...' : 'Save Changes' }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class ProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileService = inject(ProfileService);

  profile: UserProfile | null = null;
  profileForm: FormGroup;
  loading = true;
  saving = false;
  uploadingImage = false;
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor() {
    this.profileForm = this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: [''], // Readonly
      phone: [''],
      address: [''],
    });
  }

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading = true;
    this.profileService
      .getProfile()
      .pipe(
        catchError((error) => {
          console.error('Error loading profile', error);
          this.errorMessage = 'Error loading profile.';
          return of(null);
        }),
        finalize(() => (this.loading = false))
      )
      .subscribe((profile) => {
        if (profile) {
          this.profile = profile;
          this.profileForm.patchValue({
            fullName: profile.fullName,
            email: profile.email,
            phone: profile.phone,
            address: profile.address,
          });
        }
      });
  }

  onSubmit() {
    if (this.profileForm.invalid || !this.profile) return;

    this.saving = true;
    this.successMessage = null;
    this.errorMessage = null;

    const updateData = {
      ...this.profileForm.value,
      // Ensure we don't send email if it's readonly in backend logic, but here we just send what form has
    };

    this.profileService
      .updateProfile(this.profile.id, updateData)
      .pipe(
        catchError((error) => {
          console.error('Error updating profile', error);
          this.errorMessage = 'Error al guardar los cambios.';
          return of(null);
        }),
        finalize(() => (this.saving = false))
      )
      .subscribe((updatedProfile) => {
        if (updatedProfile) {
          this.profile = updatedProfile;
          this.successMessage = 'Perfil actualizado correctamente.';
          setTimeout(() => (this.successMessage = null), 3000);
        }
      });
  }

  onFileSelected(event: any) {
    const file: File = event.target.files[0];
    if (file) {
      this.uploadingImage = true;
      this.profileService
        .uploadAvatar(file)
        .pipe(
          catchError((error) => {
            console.error('Error uploading image', error);
            this.errorMessage = 'Error al subir la imagen.';
            return of(null);
          }),
          finalize(() => (this.uploadingImage = false))
        )
        .subscribe((response) => {
          if (response && this.profile) {
            // Update profile with new image URL locally
            this.profile.photoUrl = response.url;

            // Also update in backend (if upload endpoint doesn't auto-update customer entity)
            // Usually upload just returns URL, we need to save it to customer profile
            this.updateProfilePicture(response.url);
          }
        });
    }
  }

  updateProfilePicture(url: string) {
    if (!this.profile) return;

    this.profileService
      .updateProfile(this.profile.id, { photoUrl: url })
      .subscribe({
        next: () => {
          this.successMessage = 'Foto de perfil actualizada.';
          setTimeout(() => (this.successMessage = null), 3000);
        },
        error: () =>
          (this.errorMessage = 'Error al guardar la foto en el perfil.'),
      });
  }
}
