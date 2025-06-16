import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LoginServiceService } from './login-service.service';
import { Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'frontend-app';
  username: string = '';
  password: string = '';

  constructor(private loginService: LoginServiceService, private router: Router) {}

  navigateToHome() {
    this.router.navigate(['']); // Navigates to the HomeComponent
  }

  
}
