import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PostService } from '../../services/post.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MessageDialogComponent } from './message-dialog.component';

@Component({
  selector: 'app-create-post',
  templateUrl: './create-post.component.html',
  styleUrls: ['./create-post.component.scss']
})
export class CreatePostComponent {
  postForm: FormGroup;
  selectedImage: File | null = null;
  imagePreview: string | null = null;
  isLoading = false;
  errorMessage = '';
  showValidationMessage = false;

  constructor(
    private fb: FormBuilder,
    private postService: PostService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.postForm = this.fb.group({
      text: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  async onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) { // 5MB limit
        this.showMessage('Hata', 'Resim boyutu 5MB\'dan küçük olmalıdır.');
        return;
      }
      this.selectedImage = file;
      try {
        this.imagePreview = await this.postService.convertImageToBase64(file);
      } catch (error) {
        console.error('Error converting image:', error);
        this.showMessage('Hata', 'Resim yüklenirken bir hata oluştu.');
      }
    } else {
      this.selectedImage = null;
      this.imagePreview = null;
    }
  }

  removeImage() {
    this.selectedImage = null;
    this.imagePreview = null;
  }

  isFormValid(): boolean {
    return this.postForm.valid && this.imagePreview !== null;
  }

  private showMessage(title: string, message: string): void {
    const dialogRef = this.dialog.open(MessageDialogComponent, {
      width: '400px',
      data: { title, message }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.router.navigate(['/home']);
      }
    });
  }

  async onSubmit() {
    this.showValidationMessage = true;

    if (!this.postForm.valid) {
      this.showMessage('Hata', 'Lütfen bir metin girin.');
      return;
    }

    if (!this.imagePreview) {
      this.showMessage('Hata', 'Lütfen bir resim seçin.');
      return;
    }

    if (this.isFormValid()) {
      this.isLoading = true;
      this.errorMessage = '';

      try {
        const text = this.postForm.get('text')?.value;
        const postData = {
          text: text.trim(),
          imageBase64: this.imagePreview
        };

        await this.postService.createPost(postData).toPromise();
        this.showMessage('Başarılı', 'Gönderiniz başarıyla paylaşıldı!');
      } catch (error: any) {
        console.error('Error creating post:', error);
        const errorMessage = error.message === 'Gönderiniz uygunsuz içerik içermesi sebebiyle paylaşılamıyor.' 
          ? 'Gönderiniz uygunsuz içerik içermesi sebebiyle paylaşılamıyor. Uygunsuz olduğunu düşünmüyorsanız SAFEBOOK destek ekibine ulaşabilirsiniz.'
          : error.message || 'Gönderi paylaşılırken bir hata oluştu.';

        this.showMessage('Hata', errorMessage);
      } finally {
        this.isLoading = false;
      }
    }
  }
} 