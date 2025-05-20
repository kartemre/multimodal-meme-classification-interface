import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = ['id', 'username', 'email', 'role', 'status', 'actions'];
  dataSource: MatTableDataSource<any>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    // Initialize with sample data
    const users = [
      {
        id: 1,
        username: 'admin',
        email: 'admin@example.com',
        role: 'admin',
        status: 'active'
      },
      {
        id: 2,
        username: 'user1',
        email: 'user1@example.com',
        role: 'user',
        status: 'active'
      }
    ];

    this.dataSource = new MatTableDataSource(users);
  }

  ngOnInit(): void {}

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  addUser() {
    // Implement add user logic
    console.log('Add new user');
  }

  editUser(row: any) {
    // Implement edit user logic
    console.log('Edit user:', row);
  }

  deleteUser(row: any) {
    // Implement delete user logic
    console.log('Delete user:', row);
  }

  toggleStatus(row: any) {
    // Implement toggle status logic
    console.log('Toggle user status:', row);
  }
} 