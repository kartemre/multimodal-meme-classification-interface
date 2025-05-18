import { Component, OnInit } from '@angular/core';
import { PostService } from '../../services/post.service';
import { Post } from '../../models/post.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  posts: Post[] = [];
  isLoading = true;
  error: string | null = null;

  constructor(private postService: PostService) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.isLoading = true;
    this.error = null;

    this.postService.getPosts().subscribe({
      next: (posts: Post[]) => {
        this.posts = posts;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error loading posts:', error);
        this.error = 'Gönderiler yüklenirken bir hata oluştu.';
        this.isLoading = false;
      }
    });
  }

  toggleLike(post: Post): void {
    this.postService.toggleLike(post.id).subscribe({
      next: () => {
        post.isLiked = !post.isLiked;
        post.likes += post.isLiked ? 1 : -1;
      },
      error: (error: any) => {
        console.error('Error toggling like:', error);
        this.error = 'Beğeni işlemi sırasında bir hata oluştu.';
      }
    });
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
