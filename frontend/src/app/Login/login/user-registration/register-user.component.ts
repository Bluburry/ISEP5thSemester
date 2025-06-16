import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { LoginServiceService } from '../../../login-service.service';
import { LoginCredentialsDto } from '../../../login-result';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule, CommonModule],
  templateUrl: './register-user.component.html',
})
export class RegisterUserComponent {
  title = 'register-user';
  username: string = '';
  password: string = '';
  errorMessage: string ='';
  errorMessage2: string ='';
  showConfirmationCode= false;
  showRegister= true;
  confirmationCode: string = '';
  read: boolean = false;
  acceptPolicy: boolean = false;
  constructor(private loginService: LoginServiceService, private router: Router) {}

  navigateToHome() {
    this.router.navigate(['']); // Navigates to the HomeComponent
  }

  navigateToPanel() {
    this.read = true;
    window.open('privacy-policy', '_blank')  
  }

  ngOnInit(): void {
    const params = new URLSearchParams(window.location.search);
    if (params.get('error')){
      this.errorMessage= "Error happened: " + params.get('error');}
  }

  onRegister(): void {
    if (this.acceptPolicy) {
      const credentials: LoginCredentialsDto = { username: this.username, password: this.password };
      console.log(credentials);
      this.loginService.register(credentials).subscribe(
        (response) => {
          this.loginService.setResponse(response);
          console.log('Register Request Successful:', response);
          this.errorMessage = '';
          this.showRegister = false;
          this.showConfirmationCode = true;
        },
        (error) => {
          console.error('Register Request Failed:', error);
          this.errorMessage = 'Error requesting registration.';
        }
      );
    }else{
      this.errorMessage = 'Privacy Policy has not been accepted.';
    }
  }

  confirmRegistration(): void {
    if(this.confirmationCode){
      console.log(this.confirmationCode);
      this.loginService.confirmRegistration(this.confirmationCode).subscribe(
        (result) => {
          console.log('User Registration successfull:', result);
          this.errorMessage2 = '';
          this.showConfirmationCode = false;
          localStorage.setItem('action',"successfull-registration");
          this.router.navigate(['']); // Navigates to the HomeComponent
          },
        (error) => {
          console.error('Error confirming registration:', error);
          this.errorMessage2 = 'Error confirming registration.';
        }
        )
    }
  
  }

  onRegisterExternal(): void {
    this.loginService.registerExternal();
  }
}
