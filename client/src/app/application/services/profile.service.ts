import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface UserProfile {
  id: string;
  fullName: string;
  email: string;
  phone?: string;
  address?: string;
  photoUrl?: string;
}

@Injectable({
  providedIn: 'root',
})
export class ProfileService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/customers`;
  private filesUrl = `${environment.apiUrl}/files`;

  getProfile(): Observable<UserProfile> {
    return this.http
      .get<{ isSuccess: boolean; data: UserProfile }>(`${this.apiUrl}/me`)
      .pipe(map((response) => response.data));
  }

  updateProfile(
    id: string,
    data: Partial<UserProfile>
  ): Observable<UserProfile> {
    return this.http
      .put<{ isSuccess: boolean; data: UserProfile }>(
        `${this.apiUrl}/${id}`,
        data
      )
      .pipe(map((response) => response.data));
  }

  uploadAvatar(file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(
      `${this.filesUrl}/upload/profile`,
      formData
    );
  }
}
