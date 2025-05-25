import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError, BehaviorSubject} from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LoginRequest } from '../models/login-request.model';
import { LoginResponse } from '../models/login-response.model';
import { UserProfile } from '../models/user-profile.model';
import { RegisterRequest } from '../models/register-request.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private authStatusSubject = new BehaviorSubject<boolean>(this.hasToken());
  authStatus = this.authStatusSubject.asObservable();
  private chatResetSubject = new BehaviorSubject<boolean>(false);
  chatReset$ = this.chatResetSubject.asObservable();

  constructor(private http: HttpClient) { }

  // Kullanıcı Kaydı
  register(registerRequest: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/user/register`, registerRequest)
      .pipe(
        tap(response => console.log('Kayıt başarılı:', response)),
        catchError(error => {
          console.error('Kayıt hatası:', error);
          return throwError(() => error);
        })
      );
  }

  login(loginRequest: LoginRequest): Observable<LoginResponse> {
    console.log("Gönderilen login isteği:", loginRequest);

    return this.http.post<LoginResponse>(`${this.apiUrl}/user/login`, loginRequest)
      .pipe(
        tap(response => {
          console.log('Gelen yanıt:', response);
          if (response.token) {
            this.saveToken(response.token);
            this.authStatusSubject.next(true);
            this.chatResetSubject.next(true);
          } else {
            console.error("Login yanıtında token bulunamadı!");
          }
        }),
        catchError(this.handleError)
      );
  }

  isAuthenticated(): boolean {
    return this.hasToken();
  }

  saveToken(token: string): void {
    localStorage.setItem('token', token);
    this.authStatusSubject.next(true);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUsernameFromToken(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.username || null;
    } catch (error) {
      console.error('Token çözümleme hatası:', error);
      // Token bozuksa otomatik logout
      this.logout();
      return null;
    }
  }

  private hasToken(): boolean {
    return !!this.getToken();
  }

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.apiUrl}/user/me`, this.getAuthHeaders())
      .pipe(
        tap(profile => console.log('Kullanıcı profili alındı:', profile)),
        catchError(this.handleError)
      );
  }

  updateProfile(profileData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/user/update-profile`, profileData, this.getAuthHeaders())
      .pipe(
        tap(response => console.log('Profil güncellendi:', response)),
        catchError(this.handleError)
      );
  }

  changePassword(passwordData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/user/change-password`, passwordData, this.getAuthHeaders())
      .pipe(
        tap(response => console.log('Şifre değiştirildi:', response)),
        catchError(this.handleError)
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.authStatusSubject.next(false);
    this.chatResetSubject.next(true);
    console.log("Kullanıcı çıkış yaptı ve chat mesajları temizlendi.");
  }

  private handleError(error: any) {
    console.error("Bir hata oluştu:", error);
    return throwError(() => error);
  }

  // Şifre sıfırlama için e-posta gönderme
  forgotPassword(email: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/user/forgot-password`, { email })
      .pipe(
        tap(response => console.log('Şifre sıfırlama e-postası gönderildi:', response)),
        catchError(this.handleError)
      );
  }

  // Şifre sıfırlama token'ının geçerliliğini kontrol etme
  validateResetToken(token: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/user/validate-reset-token?token=${token}`)
      .pipe(
        tap(response => console.log('Token geçerliliği:', response)),
        catchError(this.handleError)
      );
  }

  // Yeni şifre belirleme
  resetPassword(token: string, newPassword: string, confirmPassword: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/user/reset-password`, {
      token,
      newPassword,
      confirmPassword
    }).pipe(
      tap(response => console.log('Şifre başarıyla sıfırlandı:', response)),
      catchError(this.handleError)
    );
  }

  private getAuthHeaders() {
    const token = this.getToken();
    const headers = new HttpHeaders({
      'Authorization': token ? `Bearer ${token}` : ''
    });
    return { headers };
  }
}
