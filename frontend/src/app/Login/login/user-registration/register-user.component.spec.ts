import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { RegisterUserComponent } from './register-user.component';
import { LoginServiceService } from '../../../login-service.service';

describe('RegisterUserComponent', () => {
  let component: RegisterUserComponent;
  let fixture: ComponentFixture<RegisterUserComponent>;
  let mockRouter: jasmine.SpyObj<Router>;
  let httpTestingController: HttpTestingController;
  let loginService: LoginServiceService;

  beforeEach(async () => {
    // Mock Router
    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, RegisterUserComponent],
      providers: [
        LoginServiceService,
        { provide: Router, useValue: mockRouter },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(RegisterUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    httpTestingController = TestBed.inject(HttpTestingController);
    loginService = TestBed.inject(LoginServiceService);
  });

  afterEach(() => {
    httpTestingController.verify(); // Ensure no unexpected HTTP calls
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should navigate to home on navigateToHome', () => {
    component.navigateToHome();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['']);
  });

  it('should clear error message and show confirmation code input on successful registration', () => {
    component.username = 'testuser';
    component.password = 'testpassword';

    component.onRegister();

    const req = httpTestingController.expectOne('https://localhost:5001/api/Users/RegisterUserPatient');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ username: 'testuser', password: 'testpassword' });

    req.flush({ message: 'success' }); // Simulate successful response

    expect(component.errorMessage).toBe('');
    expect(component.showRegister).toBeFalse();
    expect(component.showConfirmationCode).toBeTrue();
  });

  

  it('should clear error message and navigate to home on successful confirmation', () => {
    component.confirmationCode = '12345';

    component.confirmRegistration();

    const req = httpTestingController.expectOne('https://localhost:5001/api/Users/ActivatePatientAccount');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('token')).toBe('12345');

    req.flush({ message: 'success' }); // Simulate successful response

    expect(component.errorMessage2).toBe('');
    expect(component.showConfirmationCode).toBeFalse();
    expect(mockRouter.navigate).toHaveBeenCalledWith(['']);
    expect(localStorage.getItem('action')).toBe('successfull-registration');
  });

  it('should set error message on failed confirmation', () => {
    component.confirmationCode = '12345';

    component.confirmRegistration();

    const req = httpTestingController.expectOne('https://localhost:5001/api/Users/ActivatePatientAccount');
    expect(req.request.method).toBe('POST');

    req.flush({ error: 'Confirmation failed' }, { status: 400, statusText: 'Bad Request' });

    expect(component.errorMessage2).toBe('Error confirming registration.');
    expect(component.showConfirmationCode).toBeFalse();
  });

  it('should call redirect method on external registration', () => {
    spyOn(loginService, 'redirect');
    component.onRegisterExternal();
    expect(loginService.redirect).toHaveBeenCalledWith('https://localhost:5001/api/Users/RegisterIAM');
  });

  it('should call redirect method on external login', () => {
    spyOn(loginService, 'redirect');
    loginService.loginExternal();
    expect(loginService.redirect).toHaveBeenCalledWith('https://localhost:5001/api/Login/LoginIAM');
  });
});
