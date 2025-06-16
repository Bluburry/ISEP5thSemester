import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { EditPatientDtoPatient, PatientData } from './interfaces/patient-data';
import { environment } from '../../environments/local.environments.prod';

@Injectable({
  providedIn: 'root'
})
export class PatientService {

    private apiUrl = environment.apiUrl + 'Patient';  // Adjusted URL for fetching all patients
  
    private selectedPatientSubject: BehaviorSubject<PatientData | null> = new BehaviorSubject<PatientData | null>(null);
    public selectedPatient$: Observable<PatientData | null> = this.selectedPatientSubject.asObservable();
  
    constructor(private http: HttpClient) { }
    
    getPatientById(token: string): void {
      const url = `${this.apiUrl}/GetPatientByToken`;  // Endpoint for editing patient data
      const headers = new HttpHeaders({
        'token': `${token}`,  // Sending the token in the header
      });
      this.http.get<PatientData>(url, {headers}).subscribe({
        next: (patient: PatientData) => {
          this.selectedPatientSubject.next(patient); // Update selected patient
        },
        error: (err) => {
          console.error(`Error fetching the patient profile with the token: ${token}`, err);
        }
      });
    }
  
    // HTTP POST request to edit patient details (Patient)
    editPatientProfile(editData: EditPatientDtoPatient, token: string): Observable<any> {
      console.log('Editing patient profile in service');
      const url = `${this.apiUrl}/editPatient_Patient`;  // Endpoint for editing patient data
      const headers = new HttpHeaders({
        'token': `${token}`,  // Sending the token in the header
      });
  
      return this.http.post<any>(url, editData, { headers });
    }
  
    // DELETE request to delete self patient profile
    deletePatientProfile(token: string): Observable<any> {
      const url = `${this.apiUrl}/DeleteSelfPatient`;
      const headers = new HttpHeaders({
        'token': token,
      });
    
      return this.http.delete<any>(url, {headers});
    }
  
    confirmPatientProfileDelete(token:string):Observable<any> {
      console.log('Confirming patient profile delete in service');
      const url = `${this.apiUrl}/ConfirmPatientDeletion`;
      const headers = new HttpHeaders({
        'token': `${token}`,  // Sending the token in the header
      });
      return this.http.delete<any>(url, { headers });
    }

    confirmPatientProfileUpdate(token:string):Observable<any> {
      console.log('Confirming patient profile update in service');
      const url = `${this.apiUrl}/ConfirmUpdate`;
      const headers = new HttpHeaders({
        'token': `${token}`,  // Sending the token in the header
      });
      console.log(token);
      console.log(headers);
      // Fix: Pass headers as part of the options object
      return this.http.post<any>(url, null, { headers });
    }
  
    validate(token: string){
      const url = environment.apiUrl + `Tokens`
      
      const headers = new HttpHeaders({
        'token': `${token}`,  // Sending the token in the header
      });
  
      return this.http.post<{role: string}>(url, null, {headers});
    }

	sendClinicalDetailsPresentation(token: string, password: string) {
		const url = `${environment.pmdUrl}clinicalDetails/sendClinicalDetailsPresentation`;
		const headers = new HttpHeaders({
			'token': token,
			'password': password,
		});
		
		console.log("sending import request", url, headers);
		/* let body : ClinicalDetails = {
			patientMRN : dto.patientMRN,
			allergies : dto.allergies,
			medicalConditions : dto.medicalConditions
		} */
		return this.http.get(url, /* body,  */{ headers, responseType: 'blob' });
	}

	sendClinicalDetails(token: string, code: string, password: string) {

		const url = `${environment.pmdUrl}clinicalDetails/sendClinicalDetails`;
		const headers = new HttpHeaders({
			'token': token,
			'code': code,
			'password': password,
		});

		console.log("sending import request", url, headers);
		/* let body : ClinicalDetails = {
			patientMRN : dto.patientMRN,
			allergies : dto.allergies,
			medicalConditions : dto.medicalConditions
		} */
		return this.http.get(url, /* body,  */{ headers, responseType: 'blob' });
	}

	generatePassword(length: number) : string {
		const charset = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?';
		let password = '';

		for (let i = 0; i < length; i++) {
			const randomIndex = Math.floor(Math.random() * charset.length);
			password += charset[randomIndex];
		}

		return password;
	}
  }