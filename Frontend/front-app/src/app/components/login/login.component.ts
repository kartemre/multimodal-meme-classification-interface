import { Component } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { LoginRequest } from '../../models/login-request.model';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons'; // FontAwesome ikonları eklendi

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username: string = '';
  password: string = '';
  errorMessage: string = '';
  isLoading: boolean = false;
  showPassword: boolean = false;
  faEye = faEye;
  faEyeSlash = faEyeSlash;

  constructor(private authService: AuthService, private router: Router) {}

  login() {
    if (!this.username.trim() || !this.password.trim()) {
      this.errorMessage = "⚠ Kullanıcı adı ve şifre zorunludur!";
      return;
    }

    const loginData = new LoginRequest(this.username, this.password);
    this.isLoading = true;

    this.authService.login(loginData).subscribe({
      next: (response) => {
        console.log('✅ Login başarılı:', response);
        this.authService.saveToken(response.token);
        this.router.navigate(['/home']);
      },
      error: (err) => {
        console.error('⛔ Login hatası:', err);
        this.isLoading = false;
        if (err.status === 401) {
          this.errorMessage = "Yetkilendirme hatası! Kullanıcı adı veya şifre yanlış.";
          this.isLoading = false;
        } else if (err.status === 400) {
          this.isLoading = false;
          this.errorMessage = "Geçersiz istek! Lütfen bilgilerinizi kontrol edin.";
        } else {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Bilinmeyen bir hata oluştu. Lütfen tekrar deneyin.';
        }
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
  
  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }
}
