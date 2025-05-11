export interface LoginResponse {
    token: string;  // Backend büyük harfle döndürdüğü için burada da büyük harfle tanımladık
    expiry: string;
    role: string;
  }