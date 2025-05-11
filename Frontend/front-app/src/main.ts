import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
console.log("Angular uygulaması başlatılıyor...");

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
