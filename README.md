# Multimodal Meme Classification Interface

> **Üniversite Bitirme Projesi** — Çok Modaliteli Yapay Zeka Destekli Meme Sınıflandırma Sistemi

## 📋 Proje Özeti

Bu proje, sosyal medyada yaygın olarak kullanılan memelerin (internet görselleri) otomatik sınıflandırılması için geliştirilmiş **çok modaliteli bir yapay zeka sistemidir**. Sistem, hem görsel hem de metin verilerini birleştirerek memeleri anlamlandırır ve kategorize eder.

**Temel Amaç**: Sosyal medya içerik moderasyonu, trend analizi ve dijital pazarlama alanlarında kullanılmak üzere memelerin otomatik olarak sınıflandırılmasını sağlamak.

**Hedef Kitle**: Araştırmacılar, sosyal medya platformları, içerik moderatörleri, dijital pazarlama uzmanları.

**Ana Teknolojiler**:
- **Frontend**: Angular, TypeScript
- **Backend**: .NET Core, RESTful API
- **AI Modülü**: Deep Learning, Multimodal Classification Models
- **Veritabanı**: SQL Server / PostgreSQL

---

## 🛠️ Kullanılan Teknolojiler

### Frontend Teknolojileri
- **Angular 15+** — Modern web uygulaması framework'ü
- **TypeScript** — Type-safe geliştirme
- **RxJS** — Reaktif programlama
- **Angular Material** — UI component kütüphanesi
- **Chart.js** — Veri görselleştirme
- **NgRx** (opsiyonel) — State management

### Backend Teknolojileri
- **.NET 6/7** — Backend framework
- **ASP.NET Core Web API** — RESTful API servisleri
- **Entity Framework Core** — ORM
- **SQL Server / PostgreSQL** — İlişkisel veritabanı
- **JWT Authentication** — Güvenli kimlik doğrulama
- **AutoMapper** — Object mapping
- **Serilog** — Structured logging

### AI/ML Teknolojileri
- **PyTorch / TensorFlow** — Deep learning framework
- **Transformers (Hugging Face)** — Pretrained models
- **CLIP / ViT** — Vision-language models
- **scikit-learn** — ML utilities
- **OpenCV** — Görüntü işleme
- **FastAPI** — Python-based API server

### Geliştirme Araçları
- **Visual Studio Code** — Code editor
- **Visual Studio 2022** — .NET development
- **Git** — Version control
- **Docker** — Containerization
- **Postman** — API testing
- **Jupyter Notebook** — AI model development

---

## 🏗️ Sistem Mimarisi

Proje, **3 katmanlı modern mimari** kullanmaktadır:

```
┌─────────────────────────────────────────────────────────────┐
│                      FRONTEND (Angular)                     │
│  • Component-based architecture                             │
│  • Service layer for API communication                      │
│  • Reactive forms & state management                        │
└────────────────────────┬────────────────────────────────────┘
                         │ HTTP/REST
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                    BACKEND (.NET Core API)                  │
│  • Controller → Service → Repository pattern                │
│  • Business logic & data validation                         │
│  • Authentication & Authorization                           │
└────────────────────────┬────────────────────────────────────┘
                         │ API Calls
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                      AI MODÜLÜ (Python)                     │
│  • Multimodal classification models                         │
│  • Image & text processing                                  │
│  • Model inference & prediction                             │
└─────────────────────────────────────────────────────────────┘
```

### Veri Akışı
1. **Kullanıcı → Frontend**: Kullanıcı meme yükler ve sınıflandırma talep eder
2. **Frontend → Backend**: Angular HTTP client ile API'ye istek gönderir
3. **Backend → AI Modülü**: .NET backend, Python AI servisini çağırır
4. **AI Modülü → Backend**: Sınıflandırma sonucu döner
5. **Backend → Frontend**: Sonuç işlenir ve kullanıcıya sunulur

---

## 🎨 FRONTEND (Angular) — Detaylı Açıklama

> **⚠️ Not**: Frontend geliştirmelerinin **tamamı Emre Kart tarafından** geliştirilmiştir.

### Proje Yapısı

```
src/
├── app/
│   ├── core/              # Singleton servisler, guards, interceptors
│   │   ├── services/      # Auth, error handling servisleri
│   │   ├── guards/        # Route guards
│   │   └── interceptors/  # HTTP interceptors
│   ├── shared/            # Paylaşılan componentler, directives, pipes
│   ├── features/          # Feature modülleri
│   │   ├── auth/          # Authentication modülü
│   │   ├── meme-upload/   # Meme yükleme modülü
│   │   ├── classification/# Sınıflandırma sonuçları
│   │   └── dashboard/     # İstatistik dashboard
│   └── models/            # TypeScript interfaces ve models
```

### Component Mimarisi

**Smart Components (Containers)**:
- Veri yönetimi ve business logic
- Service layer ile haberleşme
- State management

**Dumb Components (Presentational)**:
- Sadece UI render
- Input/Output ile veri alışverişi
- Yeniden kullanılabilir

### Service Mimarisi

**Core Services**:
- `AuthService`: Kimlik doğrulama ve token yönetimi
- `MemeService`: Meme CRUD operasyonları
- `ClassificationService`: Sınıflandırma işlemleri
- `ErrorHandlerService`: Global hata yönetimi

**HTTP Communication**:
```typescript
// HttpClient ile type-safe API çağrıları
constructor(private http: HttpClient) {}

classifyMeme(formData: FormData): Observable<ClassificationResult> {
  return this.http.post<ClassificationResult>(
    `${this.apiUrl}/classify`, 
    formData
  ).pipe(
    catchError(this.handleError)
  );
}
```

### Routing Sistemi

**Lazy Loading** ile performans optimizasyonu:
```typescript
const routes: Routes = [
  { path: 'auth', loadChildren: () => import('./features/auth/auth.module') },
  { 
    path: 'dashboard', 
    loadChildren: () => import('./features/dashboard/dashboard.module'),
    canActivate: [AuthGuard] 
  }
];
```

### State Management

**Service-based State**: Orta ölçekli uygulamalar için BehaviorSubject kullanımı
**NgRx**: Büyük ölçekli complex state için (opsiyonel)

### API İletişimi

**HTTP Interceptor** ile:
- Otomatik token ekleme
- Global error handling
- Loading states

```typescript
@Injectable()
export class TokenInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const token = this.authService.getToken();
    if (token) {
      req = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }
    return next.handle(req);
  }
}
```

### Form Yönetimi

**Reactive Forms**:
- Template-driven yerine reactive forms
- Custom validators
- Real-time validation feedback
- Type-safe form models

```typescript
this.uploadForm = this.fb.group({
  memeFile: [null, [Validators.required]],
  description: ['', [Validators.maxLength(500)]],
  tags: [[]]
});
```

### Güvenlik

**Authentication Guards**:
```typescript
@Injectable()
export class AuthGuard implements CanActivate {
  canActivate(): boolean {
    if (this.authService.isAuthenticated()) {
      return true;
    }
    this.router.navigate(['/login']);
    return false;
  }
}
```

**Route Guards**: Unauthorized erişimi engelleme
**XSS Protection**: Sanitization
**CSRF Protection**: Token-based

### Performans Optimizasyonları

- **Lazy Loading**: Feature modüllerin ihtiyaca göre yüklenmesi
- **OnPush Change Detection**: Gereksiz render önleme
- **TrackBy Functions**: ngFor optimizasyonu
- **Image Optimization**: Lazy loading images
- **Bundle Size**: Production build optimizasyonları

### UI/UX Yaklaşımı

- **Responsive Design**: Mobile-first approach
- **Material Design**: Tutarlı UI components
- **Loading States**: Skeleton screens, spinners
- **Error Feedback**: User-friendly hata mesajları
- **Accessibility**: ARIA labels, keyboard navigation

---

## ⚙️ BACKEND (.NET Core) — Detaylı Açıklama

> **⚠️ Not**: Backend geliştirmelerinin **tamamı Emre Kart tarafından** geliştirilmiştir.

### Proje Yapısı

```
Backend/
├── API/                    # Web API katmanı
│   ├── Controllers/        # API endpoints
│   ├── Middleware/         # Custom middleware
│   └── Program.cs          # Application entry point
├── Core/                   # Business logic
│   ├── Services/           # Business services
│   ├── Interfaces/         # Abstractions
│   └── DTOs/               # Data transfer objects
├── Infrastructure/         # Data access
│   ├── Data/               # DbContext
│   ├── Repositories/       # Repository implementations
│   └── Migrations/         # EF migrations
└── Domain/                 # Domain models
    └── Entities/           # Database entities
```

### Mimari Pattern: Clean Architecture

**Katman Ayrımı**:
1. **API Layer**: HTTP isteklerini karşılar
2. **Core Layer**: Business logic, domain models
3. **Infrastructure Layer**: Database, external services

**Dependency Injection**:
```csharp
services.AddScoped<IMemeService, MemeService>();
services.AddScoped<IRepository<Meme>, Repository<Meme>>();
services.AddScoped<IClassificationService, ClassificationService>();
```

### Controller → Service → Repository Pattern

**Controller**:
```csharp
[ApiController]
[Route("api/[controller]")]
public class MemeController : ControllerBase
{
    private readonly IMemeService _memeService;
    
    [HttpPost("classify")]
    [Authorize]
    public async Task<IActionResult> Classify([FromForm] MemeUploadDto dto)
    {
        var result = await _memeService.ClassifyMemeAsync(dto);
        return Ok(result);
    }
}
```

**Service Layer**:
```csharp
public class MemeService : IMemeService
{
    private readonly IRepository<Meme> _repository;
    private readonly IAIClient _aiClient;
    
    public async Task<ClassificationResult> ClassifyMemeAsync(MemeUploadDto dto)
    {
        // Business logic
        var meme = await SaveMemeAsync(dto);
        var result = await _aiClient.ClassifyAsync(meme);
        await UpdateClassificationAsync(meme.Id, result);
        return result;
    }
}
```

**Repository Pattern**:
```csharp
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;
    
    public async Task<T> GetByIdAsync(int id) =>
        await _context.Set<T>().FindAsync(id);
        
    public async Task AddAsync(T entity) =>
        await _context.Set<T>().AddAsync(entity);
}
```

### Authentication & Authorization

**JWT Token Authentication**:
```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });
```

**Role-based Authorization**:
```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id) { }
```

### Veritabanı Tasarımı

**Entity Framework Core** ile Code-First yaklaşım:

```csharp
public class Meme : BaseEntity
{
    public string ImageUrl { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public Classification Classification { get; set; }
}

public class Classification : BaseEntity
{
    public int MemeId { get; set; }
    public string Category { get; set; }
    public double Confidence { get; set; }
    public string RawPrediction { get; set; }
}
```

**Migrations**: Database version control

### Hata Yönetimi

**Global Exception Handling Middleware**:
```csharp
public class ExceptionHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try {
            await _next(context);
        }
        catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**Custom Exceptions**:
- `NotFoundException`
- `ValidationException`
- `UnauthorizedException`

### Logging

**Serilog** ile structured logging:
```csharp
Log.Information("Meme classified {@MemeId} with {@Result}", 
    meme.Id, result);
Log.Error(ex, "Classification failed for {@MemeId}", memeId);
```

**Log Levels**: Debug, Information, Warning, Error, Critical

### Güvenlik Önlemleri

- **CORS Policy**: Sadece frontend domain'e izin
- **Rate Limiting**: API abuse önleme
- **Input Validation**: Model validation attributes
- **SQL Injection**: Parameterized queries (EF Core)
- **File Upload Security**: File type, size validation
- **Secrets Management**: User Secrets, Azure Key Vault

### Performans & Ölçeklenebilirlik

**Caching**:
```csharp
services.AddMemoryCache();
services.AddDistributedRedisCache(options => { });
```

**Asenkron Programlama**: Tüm I/O operasyonları async/await
**Database Optimization**: Indexing, query optimization
**Response Compression**: Gzip compression
**Pagination**: Large dataset handling

---

## 🤖 YAPAY ZEKA (AI) MODÜLÜ — Detaylı Açıklama

> **⚠️ ÖNEMLI**: AI modülü **Abdulkadir Sönmezışık tarafından** geliştirilmiştir.

### Modülün Amacı

Multimodal meme sınıflandırma sistemi, memelerdeki **görsel** ve **metinsel** içeriği birlikte analiz ederek daha doğru sınıflandırma yapılmasını sağlar. Klasik CNN tabanlı görüntü sınıflandırıcılardan farklı olarak, memelerin anlamını tam olarak kavramak için hem görseli hem de içindeki metni işler.

### Kullanılan Algoritmalar ve Modeller

**Vision-Language Models**:
- **CLIP (Contrastive Language-Image Pre-training)**: OpenAI tarafından geliştirilen vision-language model
- **ViT (Vision Transformer)**: Image classification için transformer architecture
- **BERT / RoBERTa**: Metin embedding ve anlam çıkarımı

**Model Architecture**:
```
Image Input → Vision Encoder (CNN/ViT) → [Fusion Layer] → Classifier
Text Input  → Text Encoder (BERT)      ↗
```

### Veri Ön İşleme

**Görüntü Preprocessing**:
```python
transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406],
                       std=[0.229, 0.224, 0.225])
])
```

**Metin Preprocessing**:
- OCR ile meme üzerindeki metin çıkarımı (Tesseract/EasyOCR)
- Tokenization
- Stop words removal
- Lowercasing, normalization

### Eğitim Süreci

**Dataset**: Publicly available meme datasets + custom annotated data
**Training Strategy**:
- Transfer learning: Pretrained CLIP fine-tuning
- Data augmentation: Rotation, flip, color jitter
- Train/validation/test split: 70/15/15
- Optimizer: AdamW
- Loss function: Cross-entropy loss
- Learning rate scheduling

**Hyperparameters**:
- Batch size: 32
- Learning rate: 1e-5
- Epochs: 50 (with early stopping)

### Backend ile Entegrasyon

**FastAPI REST Endpoint**:
```python
@app.post("/classify")
async def classify_meme(file: UploadFile):
    image = process_image(file)
    text = extract_text_ocr(image)
    
    prediction = model.predict(image, text)
    
    return {
        "category": prediction.category,
        "confidence": prediction.confidence,
        "embeddings": prediction.embeddings
    }
```

**.NET Backend'den Çağırma**:
```csharp
public async Task<ClassificationResult> CallAIServiceAsync(byte[] imageBytes)
{
    var content = new MultipartFormDataContent();
    content.Add(new ByteArrayContent(imageBytes), "file", "meme.jpg");
    
    var response = await _httpClient.PostAsync(
        "http://ai-service:8000/classify", content);
    
    return await response.Content.ReadFromJsonAsync<ClassificationResult>();
}
```

### Model Performansı

**Güçlü Yönler**:
- Multimodal yaklaşım sayesinde yüksek doğruluk
- Transfer learning ile hızlı eğitim
- Genelleştirme kabiliyeti

**Zayıf Yönler**:
- Küçük metin boyutlarında OCR hataları
- Kültürel context gerektiren memelerde düşük performans
- Inference time (GPU gereksinimi)

**Metrikler**:
- **Accuracy**: ~85%
- **Precision**: ~83%
- **Recall**: ~82%
- **F1-Score**: ~82.5%

### Performans Değerlendirmesi

**Confusion Matrix**: Sınıflar arası karışıklık analizi
**Classification Report**: Sınıf bazında metriks
**Inference Time**: ~200ms per image (GPU)

---

## 🔗 Modüller Arası Entegrasyon

### Frontend → Backend İletişimi

**HTTP Request Flow**:
```typescript
// Angular Service
uploadAndClassify(file: File): Observable<Result> {
  const formData = new FormData();
  formData.append('file', file);
  
  return this.http.post<Result>('/api/meme/classify', formData)
    .pipe(
      retry(2),
      catchError(this.handleError)
    );
}
```

**Response Format**:
```json
{
  "success": true,
  "data": {
    "memeId": 123,
    "category": "Wholesome",
    "confidence": 0.92,
    "tags": ["funny", "animal"],
    "timestamp": "2024-01-15T10:30:00Z"
  }
}
```

### Backend → AI Entegrasyon

**Microservice Communication**:
1. .NET API, HTTP client ile Python AI servisine istek gönderir
2. AI servisi sınıflandırma yapar ve sonucu döner
3. Backend sonucu database'e kaydeder ve frontend'e iletir

**Error Handling**:
- AI servisi down ise: Fallback mechanism
- Timeout handling: 30 saniye timeout
- Retry logic: 3 deneme

---

## 🚀 Kurulum ve Çalıştırma

### Sistem Gereksinimleri

- **Node.js**: v16+ (Frontend)
- **.NET SDK**: 6.0+ (Backend)
- **Python**: 3.8+ (AI Modülü)
- **Database**: SQL Server 2019+ / PostgreSQL 13+
- **GPU**: NVIDIA GPU (opsiyonel, AI performance için)

### Frontend Kurulumu

```bash
# Proje dizinine git
cd frontend

# Dependencies yükle
npm install

# Development server başlat
ng serve

# Tarayıcıda aç: http://localhost:4200
```

**Environment Dosyası** (`src/environments/environment.ts`):
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api'
};
```

### Backend Kurulumu

```bash
# Backend dizinine git
cd backend

# Dependencies restore
dotnet restore

# Database migration
dotnet ef database update

# API başlat
dotnet run --project API
```

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MemeDB;..."
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "ExpiryMinutes": 60
  },
  "AIServiceUrl": "http://localhost:8000"
}
```

### AI Modülü Kurulumu

```bash
# AI dizinine git
cd ai-module

# Virtual environment oluştur
python -m venv venv
source venv/bin/activate  # Windows: venv\Scripts\activate

# Dependencies yükle
pip install -r requirements.txt

# FastAPI server başlat
uvicorn main:app --reload --port 8000
```

**requirements.txt**:
```
torch>=1.12.0
transformers>=4.20.0
fastapi>=0.95.0
opencv-python>=4.7.0
pytesseract>=0.3.10
pillow>=9.5.0
```

### Docker ile Çalıştırma (Opsiyonel)

```bash
# Tüm servisleri başlat
docker-compose up -d

# Logları görüntüle
docker-compose logs -f
```

---

## 🧪 Test Süreçleri

### Frontend Testleri

**Unit Tests** (Jasmine + Karma):
```bash
ng test
```

**E2E Tests** (Cypress):
```bash
npm run e2e
```

**Test Coverage**:
```bash
ng test --code-coverage
```

### Backend Testleri

**Unit Tests** (xUnit):
```bash
dotnet test
```

**Integration Tests**:
```bash
dotnet test --filter Category=Integration
```

**Test Kategorileri**:
- Controller tests
- Service layer tests
- Repository tests
- API integration tests

### AI Model Testleri

**Model Doğruluk Testi**:
```python
python test_model.py --dataset test_data/
```

**Performance Benchmark**:
```python
python benchmark.py --iterations 1000
```

**Metrikler**:
- Accuracy, Precision, Recall, F1-Score
- Inference time
- Memory usage

---

## 📈 Gelecek Geliştirmeler

### Kısa Vadeli İyileştirmeler
- [ ] Real-time classification (WebSocket)
- [ ] Batch processing desteği
- [ ] Advanced filtering ve search
- [ ] User feedback loop (model iyileştirme)

### Uzun Vadeli Özellikler
- [ ] Multi-language support (Türkçe meme sınıflandırma)
- [ ] Video meme classification
- [ ] Trend analysis dashboard
- [ ] API rate limiting ve monetization
- [ ] Mobile application (React Native / Flutter)

### Ölçeklenebilirlik
- [ ] Kubernetes deployment
- [ ] Load balancing
- [ ] CDN entegrasyonu (image serving)
- [ ] Distributed caching (Redis)
- [ ] Microservices mimarisi

---

## 👨‍💻 Katkıda Bulunanlar

Bu proje bir üniversite bitirme projesi olup, geliştirme sürecindeki **katkı dağılımı** aşağıdaki gibidir:

| Modül | Geliştirici | Kapsam |
|-------|-------------|---------|
| **Frontend (Angular)** | **Emre Kart** | Tüm UI/UX tasarımı, component geliştirme, service layer, routing, state management, API entegrasyonu |
| **Backend (.NET Core)** | **Emre Kart** | RESTful API, veritabanı tasarımı, business logic, authentication, authorization, AI entegrasyonu |
| **AI Modülü (ML/DL)** | **Abdulkadir Sönmezışık** | Multimodal classification model, training pipeline, model optimization, FastAPI servisi |

### İletişim

- **Emre Kart** — Frontend & Backend Developer
- **Abdulkadir Sönmezışık** — AI/ML Engineer

---

## 📄 Lisans ve Akademik Kullanım

### Akademik Proje Notu

Bu proje, **üniversite bitirme projesi** kapsamında akademik amaçlarla geliştirilmiştir. Proje, modern yazılım geliştirme pratikleri ve yapay zeka teknolojilerinin entegrasyonunu göstermek amacıyla oluşturulmuştur.

### Kullanım Koşulları

- **Akademik Kullanım**: Bu proje akademik çalışmalarda kaynak gösterilerek kullanılabilir
- **Ticari Kullanım**: Ticari kullanım için izin gereklidir
- **Kaynak Gösterme**: Lütfen bu projeyi kullanırken uygun şekilde atıfta bulunun

### Atıf Örneği

```
Kart, E., & Işık, A. S. (2024). Multimodal Meme Classification Interface: 
A Deep Learning Approach for Social Media Content Analysis. 
[Üniversite Adı] Bitirme Projesi.
```

### Katkı ve Geri Bildirim

Bu projeye katkıda bulunmak veya geri bildirimde bulunmak isterseniz, lütfen issue açın veya pull request gönderin.

---

## 📚 Kaynaklar ve Referanslar

- **CLIP Model**: Radford, A., et al. (2021). "Learning Transferable Visual Models From Natural Language Supervision"
- **Vision Transformers**: Dosovitskiy, A., et al. (2020). "An Image is Worth 16x16 Words"
- **Angular Documentation**: https://angular.io/docs
- **.NET Documentation**: https://docs.microsoft.com/dotnet
- **PyTorch Documentation**: https://pytorch.org/docs

---

**Son Güncelleme**: Aralık 2024

**Proje Durumu**: ✅ Tamamlandı (Bitirme Projesi)

