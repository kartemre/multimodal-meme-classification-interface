import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AdminService } from '../admin/services/admin.service';
import { Observable, map, take } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {
  constructor(
    private adminService: AdminService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> | boolean {
    // Login sayfası için guard'ı bypass et
    if (route.routeConfig?.path === 'login') {
      return true;
    }

    return this.adminService.adminStatus.pipe(
      take(1),
      map(isAdmin => {
        if (isAdmin) {
          return true;
        }
        
        this.router.navigate(['/admin/login']);
        return false;
      })
    );
  }
} 