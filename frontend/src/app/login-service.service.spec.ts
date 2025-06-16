import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { LoginServiceService } from './login-service.service';
import { LoginResponse, LoginCredentialsDto } from './login-result';
import { HttpHeaders } from '@angular/common/http';

describe('LoginServiceService', () => {
  let service: LoginServiceService;
  let httpMock: HttpTestingController;

  const mockLoginResponse: LoginResponse = {
    Token: 'mock-token',
    Result: 'Success',
    Type: ''
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [LoginServiceService],
    });

    service = TestBed.inject(LoginServiceService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding HTTP requests
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should login and return a token', () => {
    const username = 'testuser';
    const password = 'testpassword';

    service.login(username, password).subscribe((response) => {
      expect(response).toEqual(mockLoginResponse);
    });

    const req = httpMock.expectOne('https://localhost:5001/api/Login/Login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ username, password });
    req.flush(mockLoginResponse);
  });

  it('should handle login failure', () => {
    const username = 'testuser';
    const password = 'wrongpassword';
    const errorResponse = { error: 'Invalid credentials' };
  
    service.login(username, password).subscribe(
      () => fail('should have failed'),
      (error) => {
        expect(error.status).toBe(400); // Verify status code
        expect(error.error).toEqual(errorResponse); // Verify the error payload
      }
    );
  
    const req = httpMock.expectOne('https://localhost:5001/api/Login/Login');
    expect(req.request.method).toBe('POST');
    req.flush(errorResponse, { status: 400, statusText: 'Bad Request' });
  });
  
  

  it('should register a user and return a response', () => {
    const credentials: LoginCredentialsDto = {
      username: 'newuser',
      password: 'newpassword',
    };

    service.register(credentials).subscribe((response) => {
      expect(response).toEqual(mockLoginResponse);
    });

    const req = httpMock.expectOne('https://localhost:5001/api/Users/RegisterUserPatient');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(credentials);
    req.flush(mockLoginResponse);
  });

  it('should confirm registration with a token', () => {
    const token = 'mock-confirmation-token';

    service.confirmRegistration(token).subscribe((response) => {
      expect(response).toEqual({ message: 'Account activated' });
    });

    const req = httpMock.expectOne('https://localhost:5001/api/Users/ActivatePatientAccount');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('token')).toBe(token);
    req.flush({ message: 'Account activated' });
  });

  it('should redirect to external registration URL', () => {
    spyOn(service, 'redirect');
    service.registerExternal();
    expect(service.redirect).toHaveBeenCalledWith('https://localhost:5001/api/Users/RegisterIAM');
  });

  it('should redirect to external login URL', () => {
    spyOn(service, 'redirect');
    service.loginExternal();
    expect(service.redirect).toHaveBeenCalledWith('https://localhost:5001/api/Login/LoginIAM');
  });

  it('should validate a token', () => {
    const token = 'mock-token';
    const mockValidationResponse = { role: 'Admin' };

    service.validate(token).subscribe((response) => {
      expect(response).toEqual(mockValidationResponse);
    });

    const req = httpMock.expectOne('https://localhost:5001/api/Tokens');
    expect(req.request.method).toBe('POST');
    expect(req.request.headers.get('token')).toBe(token);
    req.flush(mockValidationResponse);
  });

  it('should set and get the response using BehaviorSubject', () => {
    const mockResponse = { Token: 'mock-token' };
    service.setResponse(mockResponse);

    service.response$.subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    expect(service.getResponse()).toEqual(mockResponse);
  });

  it('should handle validation failure', () => {
    const token = 'invalid-token';
    const errorResponse = { error: 'Invalid token' };
  
    service.validate(token).subscribe(
      () => fail('should have failed'),
      (error) => {
        expect(error.status).toBe(400); // Verify the status code is 400
        expect(error.error).toEqual(errorResponse); // Verify the error payload
      }
    );
  
    const req = httpMock.expectOne('https://localhost:5001/api/Tokens');
    expect(req.request.method).toBe('POST');
    req.flush(errorResponse, { status: 400, statusText: 'Bad Request' });
  });  
  
  
});
