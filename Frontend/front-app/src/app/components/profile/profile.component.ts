import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/user-profile.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  user: UserProfile | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.loadUserProfile();
  }

  loadUserProfile() {
    this.authService.getProfile().subscribe(
      (profile) => {
        this.user = profile;
      },
      (error) => {
        console.error('Profil yüklenirken hata oluştu:', error);
        this.router.navigate(['/login']); // Kullanıcı giriş yapmamışsa login sayfasına yönlendir
      }
    );
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}