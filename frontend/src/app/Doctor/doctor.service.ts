import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, take, tap, throwError } from 'rxjs';
import { ClinicalDetails, PatientData } from './interfaces/patient-data';
import { OperationTypeData } from './interfaces/operation-type-data';
import { OperationRequestData } from './interfaces/operation-request-data';
import { environment } from '../../environments/local.environments.prod';
import { MedicalConditionData } from './interfaces/medical-condition-data';
import { AllergyData } from '../Admin/interfaces/allergy-data';
import { PatientQueryData } from './interfaces/patient-query-data';
import { AppointmentDto } from './interfaces/appointment-data';
import { StaffData } from '../Admin/interfaces/staff-data';

@Injectable({
	providedIn: 'root'
})
export class DoctorService {

	private readonly apiUrl = environment.apiUrl;
	// WHY IS THERE NO BETTER WAY TO DO THIS

	private patientsSubject: BehaviorSubject<PatientData[]> = new BehaviorSubject<PatientData[]>([]); // TL;DR: Stores data
	public patients$: Observable<PatientData[]> = this.patientsSubject.asObservable(); // TL;DR: Allows outsiders to access the data fetched, through an observable

	private selectedPatientSubject: BehaviorSubject<PatientData | null> = new BehaviorSubject<PatientData | null>(null);
	public selectedPatient$: Observable<PatientData | null> = this.selectedPatientSubject.asObservable();

	private opTypeSubject: BehaviorSubject<OperationTypeData[]> = new BehaviorSubject<OperationTypeData[]>([]);
	public opTypes$: Observable<OperationTypeData[]> = this.opTypeSubject.asObservable();

	private opReqSubject: BehaviorSubject<OperationRequestData[]> = new BehaviorSubject<OperationRequestData[]>([]);
	public opRequests$: Observable<OperationRequestData[]> = this.opReqSubject.asObservable();

	private selectedRequestSubject: BehaviorSubject<OperationRequestData | null> = new BehaviorSubject<OperationRequestData | null>(null);
	public selectedRequest$: Observable<OperationRequestData | null> = this.selectedRequestSubject.asObservable();

	private appointmentSubject: BehaviorSubject<AppointmentDto[]> = new BehaviorSubject<AppointmentDto[]>([]);
	public appointment$: Observable<AppointmentDto[]> = this.appointmentSubject.asObservable();

	private staffSubject: BehaviorSubject<StaffData[]> = new BehaviorSubject<StaffData[]>([]);
	public staff: Observable<StaffData[]> = this.staffSubject.asObservable();

	constructor(private http: HttpClient) { }

	createOperation(mrn: string, operationType: string, deadline: string, priority: string, token: string): void {
		const url = this.apiUrl + `OperationRequest?patient=${mrn}&type=${operationType}&deadline=${deadline}&priority=${priority}`

		let headers = new HttpHeaders();
		headers = headers.set('auth', token);

		this.http.post<any>(url, null, { headers }).subscribe();
	}

	getPatients() {
		const url = this.apiUrl + `Patient`

		this.http.get<PatientData[]>(url).subscribe({
			next: (patient: PatientData[]) => {
				console.log('Fetched patient data from API:', patient);
				this.patientsSubject.next(patient);
			},
			error: (err) => {
				console.error('Error fetching patient', err);
			}
		});
	}

	getPatientById(mrn: string) {
		const url = this.apiUrl + `Patient/GetPatientById`;
		
		let headers = new HttpHeaders();
		headers = headers.set('id', mrn);

		console.log(headers);
		this.http.get<PatientData>(url, {headers: headers}).subscribe({
			next: (patient: PatientData) => {
				this.selectedPatientSubject.next(patient); // Update selected patient
			},
			error: (err) => {
				console.error(`Error fetching patient with MRN ${mrn}`, err);
			}
		});
	}
	getOperationTypes(token: string) {
		const url = this.apiUrl + `OperationType/ListForOperation`

		const headers = new HttpHeaders({
			'token': `${token}`,
		});

		this.http.get<OperationTypeData[]>(url, { headers }).subscribe({
			next: (opType: OperationTypeData[]) => {
				console.log('Fetched operation type from API:', opType);
				this.opTypeSubject.next(opType);
			},
			error: (err) => {
				console.error('Error fetching operation type', err);
			}
		});
	}

	getOperationRequests(patient: string, operationType: string, deadline: string, priority: string, token: string) {
		const url = this.apiUrl + `OperationRequest?patient=${patient}&type=${operationType}&deadline=${deadline}&priority=${priority}` // À PATA

		const headers = new HttpHeaders({
			'auth': `${token}`, // Really should start thinking about having everything either be named 'auth' or 'token'.
		});

		this.http.get<OperationRequestData[]>(url, { headers }).subscribe({
			next: (opRequest: OperationRequestData[]) => {
				console.log('Fetched operation type from API:', opRequest);
				this.opReqSubject.next(opRequest);
			},
			error: (err) => {
				console.error('Error fetching operation type', err);
			}
		});
	}

	getOperationById(id: string, token: string) {
		const url = this.apiUrl + `OperationRequest/Operation?id=${id}`

		const headers = new HttpHeaders({
			'auth': `${token}`,
		});

		this.http.get<OperationRequestData>(url, { headers }).subscribe({
			next: (opRequest: OperationRequestData) => {
				console.log('Fetched operation type from API:', opRequest);
				this.selectedRequestSubject.next(opRequest);
			},
			error: (err) => {
				console.error('Error fetching operation type', err);
			}
		});
	}

	editOperationRequest(id: string, deadline: string, priority: string, auth: string): void {
		// https://localhost:5001/api/OperationRequest/3666b44c-3481-424a-b6f3-e324fe21d5df?deadline=2024-12-27&priority=HIGH
		// https://localhost:5001/api/OperationRequest/*id*?deadline=*deadline*&priority=*priority*
		let url = this.apiUrl + `OperationRequest/${id}?deadline=${deadline}&priority=${priority}`;

		let headers = new HttpHeaders({
			'auth': `${auth}`
		});

		console.log(id, deadline, priority);
		console.log(auth);
		console.log(url);

		this.http.patch<OperationRequestData>(url, null, { headers }).subscribe({
			next: (opRequest: OperationRequestData) => {
				console.log("Patched operation request: ", opRequest);
				this.selectedRequestSubject.next(opRequest);
			},
			error: (err) => {
				console.error('Error patching operation request', err);
			}
		});
	}

	deleteOperationRequest(id: string, auth: string): void {
		// https://localhost:5001/api/OperationRequest/5777b33c-1843-535b-b6f3-e324fe21dadf
		// https://localhost:5001/api/OperationRequest/*id*

		let url = this.apiUrl + `OperationRequest/${id}`

		/* let headers = new HttpHeaders({
			'auth': `${auth}`
		});

		console.log(url);

		this.http.delete<any>(url, { headers }); */
		let headers = new HttpHeaders().set('auth', auth);
		/* console.log(auth);
		console.log(headers);

		console.log("Hello???", url); */

		this.http.delete<any>(url, {
			headers,
			//body: JSON.stringify(name)  // Explicitly send mrn as a JSON string
		}).subscribe({
			next: (response) => console.log("Delete successful:", response),
			error: (err) => console.error("Delete failed:", err),
		});
		/* .subscribe({
			next: (opRequest: OperationRequestData) => {
				console.log("Deleted operation request: ", opRequest);
				this.selectedRequestSubject.next(opRequest);
			},
			error: (err) => {
				console.error('Error deleting operation request', err);
			}
		}); */
		/* this.http.delete<OperationRequestData>(url, { headers }).pipe(
			take(1), // Ensures the subscription completes after one emission
			tap(opRequest => {
				console.log("Deleted operation request: ", opRequest);
				this.selectedRequestSubject.next(opRequest); // Notify about the deletion
			}),
			catchError((err: HttpErrorResponse) => {
				console.error('Error deleting operation request', err.status);
				if (err.status === 404) {
					console.error('Operation request not found');
				} else if (err.status >= 500) {
					console.error('Server error occurred', err);
				} else {
					console.error('Error deleting operation request', err);
				} 
				return throwError(err); // Pass error to the caller
			})
		); */
	}

	validate(token: string) {
		const url = this.apiUrl + `Tokens`

		const headers = new HttpHeaders({
			'token': `${token}`,  // Sending the token in the header
		});

		return this.http.post<{ role: string }>(url, null, { headers });
	}

	getMedicalConditions(token: string, dto: MedicalConditionData): Observable<MedicalConditionData[]> {
		console.log('Sending query DTO:', dto);
		const url = environment.pmdUrl + 
			'medical-conditions?code=' + encodeURIComponent(dto.code) + 
			'&designation=' + encodeURIComponent(dto.designation || '') + 
			'&description=' + encodeURIComponent(dto.description || '') + 
			'&symptoms=' + encodeURIComponent(dto.symptoms || '');
		
		const headers = new HttpHeaders({
			'token': `${token}`,
		});
		
		console.log('Request URL:', url);
		
		return this.http.get<MedicalConditionData[]>(url, { headers });
	}

	getAllergies(token: string, dto: AllergyData): Observable<AllergyData[]> {
			console.log('before sending DTO:', dto);
			let url = environment.pmdUrl + 'allergies?code=' + dto.id + '&name=' + dto.name + '&description=' + dto.description;
	
			
			const headers = new HttpHeaders({
				'token': `${token}`,
			});
			console.log(url);
			
			return this.http.get<AllergyData[]>(url, { headers });
	}

	getQueryPatients(token: string, dto: PatientQueryData): Observable<PatientData[]> {
		const url = `${this.apiUrl}Patient/filter`; // Complete URL to the backend endpoint
		const headers = {
			Authorization: `Bearer ${token}`, // Add token to headers
			'Content-Type': 'application/json'
		};
	
		return this.http.post<PatientData[]>(url, dto, { headers });
	}

	getClinicalDetails(token: string, id: string): Observable<ClinicalDetails> {
		console.log('Getting ClinicalDetail with the following code:', id);
		const url = `${environment.pmdUrl}clinicalDetails/${id}`; // Construct the endpoint with the ID
	
		const headers = new HttpHeaders({
			'token': token // Add the token to the headers
		});
	
		return this.http.get<ClinicalDetails>(url, { headers });
	}
	
	getStaffList(request: string, date: string, auth: string){
		const url = this.apiUrl + `Appointment/StaffSlot?request=${request}&dateAndTime=${date}`

		const headers = new HttpHeaders({
			'auth': `${auth}`,
		});

		console.log(headers);
		this.http.get<StaffData[]>(url, { headers }).subscribe({
			next: (staff: StaffData[]) => {
				console.log('Fetched staff from API:', staff);
				this.staffSubject.next(staff);
			},
			error: (err) => {
				console.error('Error fetching staff', err);
			}
		});
	}

	scheduleAppointment(dateAndTime: string, staffId: string, patientNumber: string, operationRoom: string, requestID: string, auth: string){
		const url = this.apiUrl + `Appointment?dateAndTime=${dateAndTime}&staffId=${staffId}&patientNumber=${patientNumber}&operationRoom=${operationRoom}&requestID=${requestID}`

		let header = new HttpHeaders().set('auth', auth);

		this.http.put<any>(url, {}, { headers: header }).subscribe({
			error: (err) => {
				console.error("Error creating appointment", err);
			}
		});
	}

	getAppointments(auth: string){
		const url = this.apiUrl + `Appointment`

		const headers = new HttpHeaders({
			'auth': `${auth}`,
		});

		this.http.get<AppointmentDto[]>(url, { headers }).subscribe({
			next: (appt: AppointmentDto[]) => {
				console.log('Fetched appointments from API:', appt);
				this.appointmentSubject.next(appt);
			},
			error: (err) => {
				console.error('Error fetching appointment', err);
			}
		});
	}

	updateAppointment(ID: string, dateAndTime: string, staffId: string, operationRoom: string, auth: string){
		const url = this.apiUrl + `Appointment?ID=${ID}&dateAndTime=${dateAndTime}&staffIDs=${staffId}&operationRoom=${operationRoom}`

		const headers = new HttpHeaders({
			'auth': `${auth}`,
		});

		this.http.patch<AppointmentDto[]>(url, {}, { headers }).subscribe({
			error: (err) => {
				console.error('Error updating appointment', err);
			}
		});
	}
	
	getFilteredDetails(token: string, allergy: string, condition: string): Observable<ClinicalDetails[]> {
		console.log('WHAT');
		const url = `${environment.pmdUrl}clinicalDetails/filter`;
	
		const headers = new HttpHeaders({
			'token': token,
			'allergyID': allergy,
			'medicalConditionID': condition
		});
	
		return this.http.get<ClinicalDetails[]>(url, { headers });
	}

	updateClinicalDetails(token: string, updatedClinicalDetails: ClinicalDetails): Observable<ClinicalDetails> {
		const url = `${environment.pmdUrl}clinicalDetails/save`;
		const headers = new HttpHeaders({
		  'Content-Type': 'application/json',
		  token: token,
		});
	  
		return this.http.post<ClinicalDetails>(url, updatedClinicalDetails, { headers });
	  }

	//Helper method
	areArraysEqual(arr1: any[] | null, arr2: any[] | null): boolean {
		if (arr1 === arr2) return true; // Same reference or both null
		if (!arr1 || !arr2 || arr1.length !== arr2.length) return false; // Null checks and length mismatch
		return arr1.every((item, index) => JSON.stringify(item) === JSON.stringify(arr2[index]));
	  }
	
}