import { Component } from '@angular/core';
import { faFacebookF, faTwitter, faLinkedinIn } from '@fortawesome/free-brands-svg-icons';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.css']
})
export class FooterComponent {
  faFacebook = faFacebookF;
  faTwitter = faTwitter;
  faLinkedin = faLinkedinIn;
}
