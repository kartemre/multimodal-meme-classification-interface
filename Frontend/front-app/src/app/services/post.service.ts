import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

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
}

@Injectable({
  providedIn: 'root'
})
export class PostService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createPost(post: CreatePostDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/post/create`, post);
  }

  getAllPosts(): Observable<PostDto[]> {
    return this.http.get<PostDto[]>(`${this.apiUrl}/post/all`);
  }

  // Resmi base64'e çeviren yardımcı metod
  async convertImageToBase64(file: File): Promise<string> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.onload = () => resolve(reader.result as string);
      reader.onerror = error => reject(error);
      reader.readAsDataURL(file);
    });
  }
} 