import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { RegisterRequest } from '../../models/register-request.model';
import { faEye, faEyeSlash } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerData: RegisterRequest = {
    firstName: '',
    lastName: '',
    username: '',
    password: '',
    passwordControl: '',
    email: '',
    phone: ''
  };

  errorMessage: string = '';
  successMessage: string = '';
  isLoading: boolean = false;

  // Şifre göstermek için ikonlar
  showPassword: boolean = false;
  showPasswordControl: boolean = false;
  faEye = faEye;
  faEyeSlash = faEyeSlash;

  constructor(private authService: AuthService, private router: Router) {}

  togglePasswordVisibility() {
    this.showPassword = !this.showPassword;
  }

  togglePasswordControlVisibility() {
    this.showPasswordControl = !this.showPasswordControl;
  }

  register() {
    // 🔥 Boş alan kontrolü
    if (!this.registerData.firstName.trim() || !this.registerData.lastName.trim() ||
        !this.registerData.username.trim() || !this.registerData.email.trim() ||
        !this.registerData.phone.trim() || !this.registerData.password.trim() ||
        !this.registerData.passwordControl.trim()) {
      this.errorMessage = "⚠ Lütfen tüm alanları doldurun!";
      return;
    }

    // 🔥 Şifreler eşleşiyor mu?
    if (this.registerData.password !== this.registerData.passwordControl) {
      this.errorMessage = "⚠ Şifreler eşleşmiyor!";
      return;
    }

    this.isLoading = true; // Butonu disable et

    this.authService.register(this.registerData).subscribe({
      next: () => {
        this.successMessage = "✅ Kayıt başarılı! Yönlendiriliyorsunuz...";
        this.errorMessage = '';
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || "❌ Bir hata oluştu, tekrar deneyin.";
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
}
