import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Post } from '../../models/post.model';
import { faHeart, faComment, faShare, faEllipsisH } from '@fortawesome/free-solid-svg-icons';
import { faHeart as farHeart } from '@fortawesome/free-regular-svg-icons';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  posts: Post[] = [];
  isLoading = true;
  error: string | null = null;

  // FontAwesome ikonları
  faHeart = faHeart;
  farHeart = farHeart;
  faComment = faComment;
  faShare = faShare;
  faEllipsisH = faEllipsisH;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadPosts();
  }

  loadPosts(): void {
    this.isLoading = true;
    this.error = null;

    // Örnek veri - API hazır olduğunda bu kısmı güncelleyin
    setTimeout(() => {
      this.posts = [
        {
          id: 1,
          content: 'Harika bir gün! İstanbul\'un güzel manzarası.',
          imageUrl: 'https://picsum.photos/800/600',
          createdAt: new Date().toISOString(),
          author: {
            id: 1,
            username: 'emrekart',
            profileImageUrl: 'https://picsum.photos/50/50'
          },
          likes: 42,
          comments: 5,
          isLiked: false
        },
        {
          id: 2,
          content: 'Yeni projemiz üzerinde çalışıyoruz. Çok heyecanlıyım!',
          imageUrl: 'https://picsum.photos/800/601',
          createdAt: new Date(Date.now() - 3600000).toISOString(),
          author: {
            id: 2,
            username: 'johndoe',
            profileImageUrl: 'https://picsum.photos/51/51'
          },
          likes: 28,
          comments: 3,
          isLiked: true
        }
      ];
      this.isLoading = false;
    }, 1000);

    // API entegrasyonu için:
    /*
    this.http.get<Post[]>(`${environment.apiUrl}/posts`).subscribe({
      next: (data) => {
        this.posts = data;
        this.isLoading = false;
      },
      error: (error) => {
        this.error = 'Gönderiler yüklenirken bir hata oluştu.';
        this.isLoading = false;
        console.error('Gönderi yükleme hatası:', error);
      }
    });
    */
  }

  toggleLike(post: Post): void {
    // API entegrasyonu için:
    /*
    this.http.post(`${environment.apiUrl}/posts/${post.id}/like`, {}).subscribe({
      next: () => {
        post.isLiked = !post.isLiked;
        post.likes += post.isLiked ? 1 : -1;
      },
      error: (error) => {
        console.error('Beğeni işlemi hatası:', error);
      }
    });
    */
    
    // Örnek işlem
    post.isLiked = !post.isLiked;
    post.likes += post.isLiked ? 1 : -1;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (diffInSeconds < 60) {
      return 'Az önce';
    } else if (diffInSeconds < 3600) {
      const minutes = Math.floor(diffInSeconds / 60);
      return `${minutes} dakika önce`;
    } else if (diffInSeconds < 86400) {
      const hours = Math.floor(diffInSeconds / 3600);
      return `${hours} saat önce`;
    } else {
      return date.toLocaleDateString('tr-TR', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    }
  }
}
