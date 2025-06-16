import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { LoginResponse, LoginCredentialsDto, UserDto } from './login-result';
import { environment } from '../environments/local.environments.prod';

@Injectable({
  providedIn: 'root'
})
export class LoginServiceService {

  private loginApiUrl = environment.apiUrl + 'Login';
  private userApiUrl = environment.apiUrl + 'Users';

  constructor(private http: HttpClient) { }

  private responseSubject = new BehaviorSubject<any>(null);
  
  // Observable that other components can subscribe to
  public response$ = this.responseSubject.asObservable();

  // Method to update the response
  setResponse(response: any): void {
    this.responseSubject.next(response);
  }

  // Optionally, get the current response value directly
  getResponse(): any {
    return this.responseSubject.getValue();
  }

  login(username: string, password: string): Observable<LoginResponse> {
    const body = { username, password };
    const url = `${this.loginApiUrl}/Login`;
    return this.http.post<LoginResponse>(url, body);
  }

  register(credentials: LoginCredentialsDto): Observable<LoginResponse> {
    const url = `${this.userApiUrl}/RegisterUserPatient`;
    return this.http.post<LoginResponse>(url, credentials);
  }

  confirmRegistration(token: string){
    const url = `${this.userApiUrl}/ActivatePatientAccount`;
    const headers = new HttpHeaders({
        'token': `${token}`,
    });
    return this.http.post<any>(url, null, { headers });
  }

  registerExternal(): void {
    this.redirect(`${this.userApiUrl}/RegisterIAM`);
  }

  loginExternal(): void {
    this.redirect(`${this.loginApiUrl}/LoginIAM`);
  }
  
  redirect(url: string): void {
    window.location.href = url;
  }

  validate(token: string){
    const url = environment.apiUrl + `Tokens`

    const headers = new HttpHeaders({
      'token': `${token}`,  // Sending the token in the header
    });

    return this.http.post<{role: string}>(url, null, {headers});
  }

  getLogin(tokenId: string): Observable<UserDto> {
    const url = environment.apiUrl + `Tokens/GetTokenUserById`;
    
    const headers = new HttpHeaders({
        'tokenId': `${tokenId}` // Include token in headers if required
    });

    return this.http.get<UserDto>(url, { headers })}
}
