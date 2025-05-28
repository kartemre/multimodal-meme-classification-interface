from flask import Flask, request, jsonify, redirect
from flasgger import Swagger
import torch
import torch.nn as nn
from torchvision import models, transforms
from transformers import BertTokenizer
from PIL import Image
import base64
import io

# Flask ayarları
app = Flask(__name__)
swagger = Swagger(app)

# ------------------ ANASAYFA: Swagger'a Yönlendir ------------------
@app.route('/')
def home():
    return redirect("/apidocs")

# ------------------ MODEL TANIMLARI (EĞİTİMLE BİREBİR) ------------------

class TextFeatureExtractor(nn.Module):
    def __init__(self):
        super(TextFeatureExtractor, self).__init__()
        self.embedding = nn.Embedding(4410, 100)  # 🔴 Eğitimdeki hali
        self.lstm = nn.LSTM(100, 128, num_layers=2, bidirectional=True, batch_first=True)

    def forward(self, x):
        x = self.embedding(x)
        _, (h, _) = self.lstm(x)
        return torch.cat((h[-2], h[-1]), dim=1)  # 256 boyutlu

class ImageFeatureExtractor(nn.Module):
    def __init__(self):
        super(ImageFeatureExtractor, self).__init__()
        vgg = models.vgg16(weights="IMAGENET1K_V1")
        self.features = vgg.features
        for param in self.features.parameters():
            param.requires_grad = False

    def forward(self, x):
        x = self.features(x)
        return torch.flatten(x, 1)  # 25088 boyutlu

class MultimodalModel(nn.Module):
    def __init__(self):
        super(MultimodalModel, self).__init__()
        self.text_branch = TextFeatureExtractor()
        self.image_branch = ImageFeatureExtractor()
        self.fc = nn.Sequential(
            nn.Linear(256 + 25088, 64),
            nn.ReLU(),
            nn.Linear(64, 1),
            nn.Sigmoid()
        )

    def forward(self, text_input, image_input):
        text_feat = self.text_branch(text_input)
        image_feat = self.image_branch(image_input)
        combined = torch.cat((text_feat, image_feat), dim=1)
        return self.fc(combined).squeeze()

# ------------------ MODEL YÜKLEME ------------------

model = MultimodalModel()
model.load_state_dict(torch.load("Stack_LSTM_VGG_mul_model.pth", map_location=torch.device("cpu")))
model.eval()

# ------------------ DÖNÜŞÜM ARAÇLARI ------------------

transform = transforms.Compose([
    transforms.Resize((224, 224)),
    transforms.ToTensor(),
    transforms.Normalize(mean=[0.485, 0.456, 0.406],
                         std=[0.229, 0.224, 0.225])
])

tokenizer = BertTokenizer.from_pretrained('bert-base-uncased')

def base64_to_image(base64_str):
    if "," in base64_str:
        base64_str = base64_str.split(",")[1]
    image_data = base64.b64decode(base64_str)
    return Image.open(io.BytesIO(image_data)).convert("RGB")

def encode_text(text):
    tokens = tokenizer(text, padding='max_length', truncation=True, max_length=100, return_tensors="pt")
    input_ids = tokens['input_ids']
    input_ids = torch.clamp(input_ids, max=4409)  # Eğitimdeki vocab boyutu ile uyumlu
    return input_ids

# ------------------ TAHMİN ENDPOINT ------------------

@app.route('/predict', methods=['POST'])
def predict():
    """
    Görsel ve metinden ofansif içerik sınıflandırması
    ---
    parameters:
      - name: image
        in: formData
        type: string
        required: true
      - name: text
        in: formData
        type: string
        required: true
    responses:
      200:
        description: Prediction
        examples:
          application/json: {"prediction": "offensive"}
    """
    image_b64 = request.form.get("image")
    text = request.form.get("text")

    if not image_b64 or not text:
        return jsonify({"error": "Metin ve görsel verisi gereklidir"}), 400

    try:
        image = base64_to_image(image_b64)
        image = transform(image).unsqueeze(0)

        input_ids = encode_text(text)

        with torch.no_grad():
            output = model(input_ids, image)
            prediction = output.item()

        label = "offensive" if prediction >= 0.5 else "non-offensive"
        return jsonify({"prediction": label})

    except Exception as e:
        return jsonify({"error": str(e)}), 500

# ------------------ SUNUCUYU BAŞLAT ------------------

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
