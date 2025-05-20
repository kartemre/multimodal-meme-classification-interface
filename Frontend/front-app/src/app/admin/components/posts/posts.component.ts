import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {
  displayedColumns: string[] = ['id', 'title', 'author', 'category', 'status', 'createdAt', 'actions'];
  dataSource: MatTableDataSource<any>;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    // Initialize with sample data
    const posts = [
      {
        id: 1,
        title: 'First Post',
        author: 'user1',
        category: 'technology',
        status: 'published',
        createdAt: '2024-02-20'
      },
      {
        id: 2,
        title: 'Second Post',
        author: 'user2',
        category: 'science',
        status: 'draft',
        createdAt: '2024-02-19'
      }
    ];

    this.dataSource = new MatTableDataSource(posts);
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

  getCategoryColor(category: string): string {
    switch (category) {
      case 'technology':
        return 'primary';
      case 'science':
        return 'accent';
      case 'art':
        return 'warn';
      default:
        return 'primary';
    }
  }

  createPost() {
    // Implement create post logic
    console.log('Create new post');
  }

  viewPost(row: any) {
    // Implement view post logic
    console.log('View post:', row);
  }

  editPost(row: any) {
    // Implement edit post logic
    console.log('Edit post:', row);
  }

  deletePost(row: any) {
    // Implement delete post logic
    console.log('Delete post:', row);
  }

  toggleStatus(row: any) {
    // Implement toggle status logic
    console.log('Toggle post status:', row);
  }
} 