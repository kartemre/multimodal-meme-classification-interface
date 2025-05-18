import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isAuthenticated: boolean = false;
  username: string | null = null;
  dropdownOpen: boolean = false;
  private authSubscription!: Subscription;

  constructor(private authService: AuthService, private router: Router) {
    console.log('Header Component Constructor - Initial Auth Status:', this.authService.isAuthenticated());
  }

  ngOnInit() {
    console.log('Header Component OnInit - Before updateUserStatus');
    this.updateUserStatus();
    console.log('Header Component OnInit - After updateUserStatus:', this.isAuthenticated);

    this.authSubscription = this.authService.authStatus.subscribe(status => {
      console.log('Auth Status Changed:', status);
      this.isAuthenticated = status;
      this.username = this.authService.getUsernameFromToken();
      console.log('Updated Auth Status:', this.isAuthenticated, 'Username:', this.username);
    });
  }

  ngOnDestroy() {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  logout() {
    this.authService.logout();
    this.isAuthenticated = false;
    this.router.navigate(['/login']);
  }

  toggleDropdown() {
    this.dropdownOpen = !this.dropdownOpen;
  }

  private updateUserStatus() {
    const token = this.authService.getToken();
    console.log('Current Token:', token);
    this.isAuthenticated = this.authService.isAuthenticated();
    this.username = this.authService.getUsernameFromToken();
    console.log('UpdateUserStatus - isAuthenticated:', this.isAuthenticated, 'username:', this.username);
  }
}
