import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService, User } from '../../services/admin.service';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = ['id', 'username', 'email', 'isActive', 'createdAt', 'actions'];
  dataSource: MatTableDataSource<User>;
  isLoading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private adminService: AdminService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<User>([]);
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadUsers() {
    this.isLoading = true;
    this.adminService.getAllUsers().subscribe({
      next: (users) => {
        this.dataSource.data = users;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.snackBar.open('Kullanıcılar yüklenirken bir hata oluştu', 'Kapat', {
          duration: 3000
        });
        this.isLoading = false;
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  deleteUser(user: User) {
    if (confirm(`${user.username} kullanıcısını silmek istediğinizden emin misiniz?`)) {
      this.adminService.deleteUser(user.id).subscribe({
        next: () => {
          this.snackBar.open('Kullanıcı başarıyla silindi', 'Kapat', {
            duration: 3000
          });
          this.loadUsers();
        },
        error: (error) => {
          console.error('Error deleting user:', error);
          this.snackBar.open('Kullanıcı silinirken bir hata oluştu', 'Kapat', {
            duration: 3000
          });
        }
      });
    }
  }

  toggleUserStatus(user: User) {
    this.adminService.toggleUserStatus(user.id).subscribe({
      next: () => {
        this.snackBar.open('Kullanıcı durumu güncellendi', 'Kapat', {
          duration: 3000
        });
        this.loadUsers();
      },
      error: (error) => {
        console.error('Error toggling user status:', error);
        this.snackBar.open('Kullanıcı durumu güncellenirken bir hata oluştu', 'Kapat', {
          duration: 3000
        });
      }
    });
  }
} 