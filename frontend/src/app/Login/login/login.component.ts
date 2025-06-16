import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LoginServiceService } from '../../login-service.service';
import { Router } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  title = 'login-app';
  username: string = '';
  password: string = '';
  successMessage: string = '';
  errorMessage: string ='';

  constructor(private loginService: LoginServiceService, private router: Router) {}

  navigateToHome() {
    this.router.navigate(['']); // Navigates to the HomeComponent
  }

  ngOnInit(): void {
    var storedToken;
    const params = new URLSearchParams(window.location.search);
    let token = params.get('token');
    const error = params.get('error');
    const action = params.get('action');

    if (token){
      token = token.replace(/ /g, '+'); // Replace spaces with '+'
      localStorage.setItem('authToken', token);
    }

    if (error){
      this.errorMessage = "Error happened: " + error;
    } 

    if (action){
      localStorage.setItem('action', action);
    }
    
    if (localStorage.getItem('action')){
      switch(localStorage.getItem('action')){
        case "successfull-registration":
          this.successMessage = 'Registration was a sucess! Please login with the new informations.'
        break;
      }
      //localStorage.removeItem('action');
    }
    
    storedToken = localStorage.getItem('authToken');

    if(storedToken){
      this.loginService.validate(storedToken).subscribe(response => {
      switch(response.role){
        case "ADMIN":
          this.router.navigate(['adminPanel']);
        break;
        case "STAFF":
          this.router.navigate(['doctor-panel']);
        break;
        case "PATIENT":
          this.router.navigate(['patient-panel']);
        break;
      }
    },
    (error) => {
      console.error('Validation Failed:', error);
    }) 
  }
  }

  onLogin(): void {
    this.loginService.login(this.username, this.password).subscribe(
      (response) => {
        this.loginService.setResponse(response);
        console.log('Login Successful:', response);
      
        localStorage.setItem('authToken', response.Token);

        console.log(response.Type);
        switch(response.Type){
          case "ADMIN_AUTH_TOKEN":
            this.router.navigate(['adminPanel']);
          break;
          case "STAFF_AUTH_TOKEN":
            this.router.navigate(['doctor-panel']);
          break;
          case "PATIENT_AUTH_TOKEN":
            this.router.navigate(['patient-panel']);
          break;
        }
      },
      (error) => {
        console.error('Login Failed:', error);
      }
    );
  }

  onLoginExternal(): void {
    this.loginService.loginExternal();
  }

  navigateRegister():void{
    this.router.navigate(['register-user']);
  }
}
