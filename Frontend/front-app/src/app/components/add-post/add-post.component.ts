import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { faImage, faTimes } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrls: ['./add-post.component.css']
})
export class AddPostComponent implements OnInit {
  postForm: FormGroup;
  selectedImage: File | null = null;
  imagePreview: SafeUrl | null = null;
  isSubmitting = false;
  errorMessage = '';

  // FontAwesome ikonları
  faImage = faImage;
  faTimes = faTimes;

  constructor(
    private fb: FormBuilder,
    private sanitizer: DomSanitizer
  ) {
    this.postForm = this.fb.group({
      content: ['', [Validators.required, Validators.maxLength(500)]],
      image: [null]
    });
  }

  ngOnInit(): void {
  }

  onImageSelected(event: Event): void {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) {
      this.selectedImage = file;
      // Resim önizleme oluştur
      const reader = new FileReader();
      reader.onload = () => {
        this.imagePreview = this.sanitizer.bypassSecurityTrustUrl(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(): void {
    this.selectedImage = null;
    this.imagePreview = null;
    this.postForm.patchValue({ image: null });
  }

  onSubmit(): void {
    if (this.postForm.valid) {
      this.isSubmitting = true;
      const formData = new FormData();
      
      // Form verilerini FormData'ya ekle
      formData.append('content', this.postForm.get('content')?.value);
      if (this.selectedImage) {
        formData.append('image', this.selectedImage);
      }

      // TODO: API'ye gönderme işlemi burada yapılacak
      console.log('Form data:', formData);
      
      // Örnek olarak 1 saniye bekletip başarılı gibi davranalım
      setTimeout(() => {
        this.isSubmitting = false;
        this.postForm.reset();
        this.selectedImage = null;
        this.imagePreview = null;
        // Başarılı mesajı göster
        alert('Gönderi başarıyla oluşturuldu!');
      }, 1000);
    }
  }
} 