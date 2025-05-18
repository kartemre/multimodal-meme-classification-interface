import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PostService } from '../../services/post.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';

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
    private snackBar: MatSnackBar
  ) {
    this.postForm = this.fb.group({
      text: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  async onImageSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      if (file.size > 5 * 1024 * 1024) { // 5MB limit
        this.snackBar.open('Resim boyutu 5MB\'dan küçük olmalıdır.', 'Tamam', {
          duration: 3000
        });
        return;
      }
      this.selectedImage = file;
      try {
        this.imagePreview = await this.postService.convertImageToBase64(file);
      } catch (error) {
        console.error('Error converting image:', error);
        this.snackBar.open('Resim yüklenirken bir hata oluştu.', 'Tamam', {
          duration: 3000
        });
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

  async onSubmit() {
    this.showValidationMessage = true;

    if (!this.postForm.valid) {
      this.snackBar.open('Lütfen bir metin girin.', 'Tamam', {
        duration: 3000
      });
      return;
    }

    if (!this.imagePreview) {
      this.snackBar.open('Lütfen bir resim seçin.', 'Tamam', {
        duration: 3000
      });
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
        this.snackBar.open('Gönderi başarıyla paylaşıldı!', 'Tamam', {
          duration: 3000
        });
        this.router.navigate(['/home']);
      } catch (error) {
        console.error('Error creating post:', error);
        this.snackBar.open('Gönderi paylaşılırken bir hata oluştu.', 'Tamam', {
          duration: 3000
        });
      } finally {
        this.isLoading = false;
      }
    }
  }
} 