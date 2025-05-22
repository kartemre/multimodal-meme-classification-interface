import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AdminService } from '../../services/admin.service';

interface Post {
  id: number;
  title: string;
  imageBase64: string;
  createdAt: string;
  userId?: number;
  userName?: string;
  author?: string;
  likes?: number;
  comments?: number;
  isLiked?: boolean;
}

@Component({
  selector: 'app-posts',
  templateUrl: './posts.component.html',
  styleUrls: ['./posts.component.scss']
})
export class PostsComponent implements OnInit {
  displayedColumns: string[] = ['id', 'title', 'author', 'createdAt', 'actions'];
  dataSource: MatTableDataSource<Post>;
  isLoading = false;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private adminService: AdminService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {
    this.dataSource = new MatTableDataSource<Post>([]);
  }

  ngOnInit(): void {
    this.loadPosts();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  loadPosts() {
    this.isLoading = true;
    this.adminService.getAllPosts().subscribe({
      next: (posts: any[]) => {
        // Map backend fields to Post interface
        const mappedPosts: Post[] = posts.map(post => ({
          id: post.id,
          title: post.title || '',
          imageBase64: post.imageBase64 || post.image || '',
          createdAt: post.createdAt,
          author: post.author || '',
          likes: post.likes ?? 0,
          comments: post.comments ?? 0,
          isLiked: post.isLiked ?? false
        }));
        this.dataSource.data = mappedPosts;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading posts:', error);
        this.snackBar.open('Gönderiler yüklenirken bir hata oluştu', 'Kapat', {
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

  viewPost(post: Post) {
    this.dialog.open(PostViewDialog, {
      width: '600px',
      data: post
    });
  }

  deletePost(post: Post) {
    if (confirm(`${post.title.substring(0, 50)}... gönderisini silmek istediğinizden emin misiniz?`)) {
      this.adminService.deletePost(post.id).subscribe({
        next: () => {
          this.snackBar.open('Gönderi başarıyla silindi', 'Kapat', {
            duration: 3000
          });
          this.loadPosts();
        },
        error: (error) => {
          console.error('Error deleting post:', error);
          this.snackBar.open('Gönderi silinirken bir hata oluştu', 'Kapat', {
            duration: 3000
          });
        }
      });
    }
  }

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}

// Post View Dialog Component
@Component({
  selector: 'app-post-view-dialog',
  template: `
    <h2 mat-dialog-title>Gönderi Detayı</h2>
    <mat-dialog-content>
      <div class="post-content">
        <img *ngIf="data.imageBase64" [src]="getImageSrc(data.imageBase64)" 
             alt="Post image" class="post-image">
        <h3 class="post-title">{{data.title}}</h3>
        <div class="post-meta">
          <p><strong>Yazar:</strong> {{data.author}}</p>
          <p><strong>Oluşturulma Tarihi:</strong> {{formatDate(data.createdAt)}}</p>
          <p><strong>Beğeni:</strong> {{data.likes}}</p>
          <p><strong>Yorumlar:</strong> {{data.comments}}</p>
        </div>
      </div>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Kapat</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .post-content {
      padding: 16px;
    }
    .post-image {
      max-width: 100%;
      max-height: 400px;
      object-fit: contain;
      border-radius: 8px;
      margin-bottom: 16px;
    }
    .post-title {
      font-size: 18px;
      font-weight: 600;
      margin-bottom: 12px;
    }
    .post-meta {
      color: #666;
      font-size: 14px;
    }
    .post-meta p {
      margin: 8px 0;
    }
  `]
})
export class PostViewDialog {
  constructor(@Inject(MAT_DIALOG_DATA) public data: Post) {}

  formatDate(date: string): string {
    return new Date(date).toLocaleDateString('tr-TR', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getImageSrc(imageBase64: string): string {
    if (!imageBase64) return '';
    if (imageBase64.startsWith('data:image')) {
      return imageBase64;
    }
    // Varsayılan olarak jpeg kabul edelim
    return 'data:image/jpeg;base64,' + imageBase64;
  }
} 