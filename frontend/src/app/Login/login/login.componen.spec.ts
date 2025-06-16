import { TestBed, ComponentFixture, fakeAsync, tick } from '@angular/core/testing';
import { Router } from '@angular/router';
import { LoginComponent } from './login.component';
import { LoginServiceService } from '../../login-service.service';
import { of, throwError } from 'rxjs';
import { LoginResponse } from '../../login-result';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockLoginService: jasmine.SpyObj<LoginServiceService>;
  let mockRouter: jasmine.SpyObj<Router>;

  beforeEach(async () => {
    mockLoginService = jasmine.createSpyObj('LoginServiceService', [
      'login',
      'validate',
      'loginExternal',
      'setResponse',
    ]);
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [FormsModule, HttpClientTestingModule, LoginComponent],
      declarations: [],
      providers: [
        { provide: LoginServiceService, useValue: mockLoginService },
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to register-user page', () => {
    component.navigateRegister();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['register-user']);
  });
  

  describe('ngOnInit', () => {
    it('should validate token from URL query parameters and navigate based on role', fakeAsync(() => {
      const mockResponse = { role: 'ADMIN' };
      mockLoginService.validate.and.returnValue(of(mockResponse));
  
      component.ngOnInit();
      tick();
  
      expect(mockLoginService.validate).toHaveBeenCalledWith('mock-token');
      expect(mockRouter.navigate).toHaveBeenCalledWith(['adminPanel']);

    }));
    
    it('should set successMessage for successful-registration action', fakeAsync(() => {
      spyOn(localStorage, 'getItem').and.callFake((key) => key === 'action' ? 'successfull-registration' : null);
    
      component.ngOnInit();
      tick();
    
      expect(component.successMessage).toBe('Registration was a sucess! Please login with the new informations.');
    }));
    
  });

  describe('onLogin', () => {
    it('should log in and navigate based on response type', fakeAsync(() => {
      component.username = 'testuser';
      component.password = 'testpassword';

      const mockLoginResponse: LoginResponse = { Token: 'mock-token', Result: 'Success', Type: '' };
      mockLoginService.login.and.returnValue(of(mockLoginResponse));

      component.onLogin();
      tick();

      expect(mockLoginService.login).toHaveBeenCalledWith('testuser', 'testpassword');
      expect(mockLoginService.setResponse).toHaveBeenCalledWith(mockLoginResponse);
      expect(localStorage.getItem('authToken')).toBe('mock-token');
    }));

    it('should handle login failure and log the error', fakeAsync(() => {
      component.username = 'testuser';
      component.password = 'wrongpassword';

      const mockError = { error: 'Invalid credentials' };
      mockLoginService.login.and.returnValue(throwError(mockError));
      spyOn(console, 'error');

      component.onLogin();
      tick();

      expect(mockLoginService.login).toHaveBeenCalledWith('testuser', 'wrongpassword');
      expect(console.error).toHaveBeenCalledWith('Login Failed:', mockError);
    }));
  });

  describe('onLoginExternal', () => {
    it('should redirect to external login URL', () => {
      component.onLoginExternal();
      expect(mockLoginService.loginExternal).toHaveBeenCalled();
    });
  });
});
