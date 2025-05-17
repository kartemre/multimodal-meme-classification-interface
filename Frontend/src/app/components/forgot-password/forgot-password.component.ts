import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  forgotPasswordForm: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.valid) {
      this.isLoading = true;
      const email = this.forgotPasswordForm.get('email')?.value;

      this.authService.forgotPassword(email).subscribe({
        next: (response) => {
          this.snackBar.open(
            'Şifre sıfırlama bağlantısı email adresinize gönderildi.',
            'Kapat',
            { duration: 5000 }
          );
          this.forgotPasswordForm.reset();
        },
        error: (error) => {
          this.snackBar.open(
            error.error?.error || 'Bir hata oluştu. Lütfen daha sonra tekrar deneyin.',
            'Kapat',
            { duration: 5000 }
          );
        },
        complete: () => {
          this.isLoading = false;
        }
      });
    }
  }
} 