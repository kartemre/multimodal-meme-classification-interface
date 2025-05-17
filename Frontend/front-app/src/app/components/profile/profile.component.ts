import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/user-profile.model';
import { Router } from '@angular/router';

interface Post {
  id: string;
  content: string;
  imageUrl?: string;
  createdAt: Date;
  userId: string;
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: UserProfile | null = null;
  posts: Post[] = [];
  activeTab: 'posts' | 'info' = 'posts';
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadUserProfile();
    this.loadUserPosts();
  }

  loadUserProfile() {
    this.authService.getProfile().subscribe(
      (profile) => {
        this.user = profile;
      },
      (error) => {
        console.error('Profil yüklenirken hata oluştu:', error);
        this.router.navigate(['/login']);
      }
    );
  }

  loadUserPosts() {
    // TODO: Implement post service and call it here
    // this.postService.getUserPosts().subscribe(
    //   (posts) => {
    //     this.posts = posts;
    //   },
    //   (error) => {
    //     console.error('Gönderiler yüklenirken hata oluştu:', error);
    //   }
    // );
  }

  sendPasswordResetLink() {
    if (!this.user?.email) {
      this.errorMessage = 'E-posta adresi bulunamadı.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    this.authService.forgotPassword(this.user.email).subscribe({
      next: () => {
        this.successMessage = 'Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.';
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Şifre sıfırlama hatası:', error);
        this.errorMessage = error.error?.message || 'Bir hata oluştu. Lütfen tekrar deneyin.';
        this.isLoading = false;
      }
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}