import { booleanAttribute, Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginServiceService } from '../../login-service.service';
import { AdminService } from '../admin.service';
import { LoginResponse } from '../../login-result';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  templateUrl: './admin-panel.component.html',
  imports: [CommonModule, RouterModule, RouterOutlet, RouterLinkActive, RouterLink],
  styles: [`
    .container {
      display: flex;
      justify-content: center;
      align-items: center;
      height: 100vh;
    }
    .button-group {
      display: flex;
      flex-direction: column;
      gap: 20px;
    }
    button {
      padding: 10px 20px;
      font-size: 16px;
    }
    .nav {
      width: 90%;
    }
    .logOut {
      width: 100px;
      position: relative;
      margin-left: 100%;
      border-radius: 20%;
      border: 1px solid grey;
      transition: .5s;
    }
    .logOut:hover{
      background-color: #cccccc;
      transition: .5s;
    } 
  `]
})
export class AdminPanelComponent {
  user: string = ''; 
  role: string = ''; 

  constructor(
    private loginService: LoginServiceService,
    private adminService: AdminService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const auth = localStorage.getItem('authToken');
        console.log(auth)
        if(auth){
          this.loginService.getLogin(auth).subscribe(
            userDto => {
                console.log('UserDto retrieved:', userDto);
                localStorage.setItem("user", userDto.EmailAddress);
                localStorage.setItem("role", userDto.Role);
                this.user = userDto.EmailAddress;
                this.role = userDto.Role;
                if(userDto.Role != "ADMIN"){
                  console.log(userDto.Role);
                  this.router.navigate(['']);
                }
            },
            error => {
                console.error('Error retrieving UserDto:', error);
            }
        );      
        } else {
          // Subscribe to the loginService response$ if no token is in localStorage
          this.loginService.response$.subscribe((response: LoginResponse | null) => {
            if (response) {
              localStorage.setItem('authToken', response.Token);
            }
          });
        }

  }

  navigateToPanel(link: String): void {
    this.router.navigate([link]);
  }

  logout(): void {
    localStorage.clear();
    this.router.navigate(['']);
  }

}
