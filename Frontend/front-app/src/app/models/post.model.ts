export interface Post {
  id: number;
  text: string;
  imageBase64: string;
  createdAt: string;
  userId: number;
  userName: string;
  likes: number;
  comments: number;
  isLiked: boolean;
} 