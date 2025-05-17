import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm: FormGroup;
  token: string = '';
  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
    this.resetPasswordForm = this.fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validator: this.passwordMatchValidator });
  }

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'];
    
    if (!this.token) {
      this.snackBar.open('Geçersiz veya eksik token.', 'Tamam', { duration: 5000 });
      this.router.navigate(['/forgot-password']);
      return;
    }

    this.validateToken();
  }

  private validateToken(): void {
    this.authService.validateResetToken(this.token).subscribe({
      error: () => {
        this.snackBar.open('Geçersiz veya süresi dolmuş token.', 'Tamam', { duration: 5000 });
        this.router.navigate(['/forgot-password']);
      }
    });
  }

  private passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null
      : { mismatch: true };
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid) {
      this.isLoading = true;
      const { newPassword, confirmPassword } = this.resetPasswordForm.value;

      this.authService.resetPassword(this.token, newPassword, confirmPassword).subscribe({
        next: () => {
          this.snackBar.open('Şifreniz başarıyla değiştirildi.', 'Tamam', { duration: 5000 });
          this.router.navigate(['/login']);
        },
        error: (error) => {
          this.snackBar.open(
            error.error?.message || 'Şifre değiştirme işlemi başarısız oldu.',
            'Tamam',
            { duration: 5000 }
          );
          this.isLoading = false;
        }
      });
    }
  }
} 