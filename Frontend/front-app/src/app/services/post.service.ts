import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Post } from '../models/post.model';

export interface CreatePostDto {
  text: string;
  imageBase64?: string;
}

export interface PostDto {
  id: number;
  text: string;
  imageBase64?: string;
  userId: number;
  userName: string;
  createdAt: Date;
  isActive: boolean;
}

interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getPosts(): Observable<Post[]> {
    return this.http.get<ApiResponse<Post[]>>(`${this.apiUrl}/post/all`)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          }
          throw new Error('Posts could not be fetched');
        })
      );
  }

  getUserPosts(): Observable<Post[]> {
    return this.http.get<ApiResponse<Post[]>>(`${this.apiUrl}/post/user`)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          }
          throw new Error('User posts could not be fetched');
        })
      );
  }

  createPost(postData: { text: string; imageBase64: string }): Observable<Post> {
    return this.http.post<ApiResponse<Post>>(`${this.apiUrl}/post/create`, postData)
      .pipe(
        map(response => {
          if (response.success) {
            return response.data;
          }
          throw new Error(response.message || 'Post could not be created');
        })
      );
  }

  toggleLike(postId: number): Observable<void> {
    return this.http.post<ApiResponse<void>>(`${this.apiUrl}/post/${postId}/like`, {})
      .pipe(
        map(response => {
          if (!response.success) {
            throw new Error('Like operation failed');
          }
        })
      );
  }

  // Resmi base64'e çeviren yardımcı metod
  async convertImageToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
    });
  }
} 