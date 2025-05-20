import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { tap, catchError } from 'rxjs/operators';

export interface User {
  id: number;
  username: string;
  email: string;
  isActive: boolean;
  isDeleted: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = `${environment.apiUrl}/admin`;
  private adminStatusSubject = new BehaviorSubject<boolean>(this.hasAdminToken());
  adminStatus = this.adminStatusSubject.asObservable();

  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { username, password })
      .pipe(
        tap((response: any) => {
          if (response.token) {
            this.saveAdminToken(response.token);
            this.adminStatusSubject.next(true);
          }
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  isAdminAuthenticated(): boolean {
    return this.hasAdminToken();
  }

  saveAdminToken(token: string): void {
    localStorage.setItem('adminToken', token);
    this.adminStatusSubject.next(true);
  }

  getAdminToken(): string | null {
    return localStorage.getItem('adminToken');
  }

  private hasAdminToken(): boolean {
    return !!this.getAdminToken();
  }

  adminLogout(): void {
    localStorage.removeItem('adminToken');
    this.adminStatusSubject.next(false);
  }

  private getAdminAuthHeaders() {
    const token = this.getAdminToken();
    return {
      headers: new HttpHeaders({
        'Authorization': token ? `Bearer ${token}` : '',
        'Content-Type': 'application/json'
      })
    };
  }

  getAllPosts(): Observable<any> {
    return this.http.get(`${this.apiUrl}/posts`, this.getAdminAuthHeaders())
      .pipe(catchError(this.handleError));
  }

  deletePost(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/posts/${id}`, this.getAdminAuthHeaders())
      .pipe(catchError(this.handleError));
  }

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(`${this.apiUrl}/users`, this.getAdminAuthHeaders())
      .pipe(catchError(this.handleError));
  }

  toggleUserStatus(id: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/users/${id}/toggle-status`, {}, this.getAdminAuthHeaders())
      .pipe(catchError(this.handleError));
  }

  deleteUser(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/users/${id}`, this.getAdminAuthHeaders())
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    console.error('API Error:', error);
    return throwError(() => error);
  }
}
