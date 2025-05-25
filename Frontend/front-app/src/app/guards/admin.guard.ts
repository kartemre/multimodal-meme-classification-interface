import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AdminService } from '../admin/services/admin.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(private adminService: AdminService, private router: Router) {}

  canActivate(): boolean {
    const token = this.adminService.getAdminToken();
    if (!token) {
      this.router.navigate(['/admin/login']);
      return false;
    }
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      if (payload.role === 'Admin' || payload.role === 'admin') {
        return true;
      } else {
        this.router.navigate(['/']);
        return false;
      }
    } catch (e) {
      this.adminService.adminLogout();
      this.router.navigate(['/admin/login']);
      return false;
    }
  }
} 