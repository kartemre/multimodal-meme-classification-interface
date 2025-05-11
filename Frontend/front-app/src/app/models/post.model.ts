export interface Post {
  id: number;
  content: string;
  imageUrl?: string;
  createdAt: string;
  author: {
    id: number;
    username: string;
    profileImageUrl?: string;
  };
  likes: number;
  comments: number;
  isLiked: boolean;
} 