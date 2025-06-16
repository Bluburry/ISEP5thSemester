import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, catchError, Observable, switchMap, throwError } from 'rxjs';
import { EditPatientDtoAdmin, PatientData, PatientRegistrationDto } from './interfaces/patient-data';
import { PatientQueryData } from './interfaces/patient-query-data';
import { EditStaffDtoAdmin, StaffData } from './interfaces/staff-data';
import { StaffQueryData } from './interfaces/staff-query-data';
import { AppointmentDto, OperationTypeData, ScheduleAppointmentsDto, SchedulingResult } from './interfaces/operation-type-data';
import { OperationTypeResultData } from './interfaces/operation-type-query-data';
import { environment } from '../../environments/local.environments.prod';
import { OperationRoomData } from './interfaces/operation-room-data';
import { OperationRequestData } from '../Doctor/interfaces/operation-request-data';
import { AllergyData } from './interfaces/allergy-data';
import { OperationRoomTypeData } from './interfaces/room-type-data';
import { MedicalConditionData } from './interfaces/medical-condition-data';
import { SpecializationData } from './interfaces/specialization-data';

@Injectable({
	providedIn: 'root'
})

export class AdminService {

	private apiUrl = environment.apiUrl + 'Patient/';  // Adjusted URL for fetching all patients
	private patientsSubject: BehaviorSubject<PatientData[]> = new BehaviorSubject<PatientData[]>([]); // Store patient data
	public patients$: Observable<PatientData[]> = this.patientsSubject.asObservable(); // Observable for external subscribers

	private selectedPatientSubject: BehaviorSubject<PatientData | null> = new BehaviorSubject<PatientData | null>(null);
	public selectedPatient$: Observable<PatientData | null> = this.selectedPatientSubject.asObservable();

	private staffSubject: BehaviorSubject<StaffData[]> = new BehaviorSubject<StaffData[]>([]); // Store patient data
	public staff$: Observable<StaffData[]> = this.staffSubject.asObservable(); // Observable for external subscribers
	private selectedStaffSubject: BehaviorSubject<StaffData | null> = new BehaviorSubject<StaffData | null>(null);
	public selectedStaff$: Observable<StaffData | null> = this.selectedStaffSubject.asObservable();

	private specializationSubject: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
	public specialization$: Observable<string[]> = this.specializationSubject.asObservable();

	private operationTypeSubject: BehaviorSubject<OperationTypeData | null> = new BehaviorSubject<OperationTypeData | null>(null);
	public operationType$: Observable<OperationTypeData | null> = this.operationTypeSubject.asObservable();

	private operationTypeResultsSubject: BehaviorSubject<OperationTypeResultData[]> = new BehaviorSubject<OperationTypeResultData[]>([]);
	public operationTypeResults$: Observable<OperationTypeResultData[]> = this.operationTypeResultsSubject.asObservable();

	private operationTypeResultSubject: BehaviorSubject<OperationTypeResultData | null> = new BehaviorSubject<OperationTypeResultData | null>(null);
	public operationTypeResult$: Observable<OperationTypeResultData | null> = this.operationTypeResultSubject.asObservable();

	private opReqSubject: BehaviorSubject<OperationRequestData[]> = new BehaviorSubject<OperationRequestData[]>([]);
	public opRequests$: Observable<OperationRequestData[]> = this.opReqSubject.asObservable();

	private selectedRequestSubject: BehaviorSubject<OperationRequestData | null> = new BehaviorSubject<OperationRequestData | null>(null);
	public selectedRequest$: Observable<OperationRequestData | null> = this.selectedRequestSubject.asObservable();

	private opRoomsSubject: BehaviorSubject<OperationRoomData[]> = new BehaviorSubject<OperationRoomData[]>([]);
	public opRooms$: Observable<OperationRoomData[]> = this.opRoomsSubject.asObservable();

	private allergies: BehaviorSubject<AllergyData[]> = new BehaviorSubject<AllergyData[]>([]);
	public allergies$: Observable<OperationRoomData[]> = this.opRoomsSubject.asObservable();

	private appointmentSubject: BehaviorSubject<AppointmentDto | null> = new BehaviorSubject<AppointmentDto | null>(null);
	public appointment$: Observable<AppointmentDto | null> = this.appointmentSubject.asObservable();

	private schedulingResultSubject: BehaviorSubject<SchedulingResult | null> = new BehaviorSubject<SchedulingResult | null>(null);
	public schedulingResult$: Observable<SchedulingResult | null> = this.schedulingResultSubject.asObservable();

	private specializationResultsSubject: BehaviorSubject<SpecializationData[]> = new BehaviorSubject<SpecializationData[]>([]);
	public specializationResults$: Observable<SpecializationData[]> = this.specializationResultsSubject.asObservable();

	private specializationResultSubject: BehaviorSubject<SpecializationData | null> = new BehaviorSubject<SpecializationData | null>(null);
	public specializationResult$: Observable<SpecializationData | null> = this.specializationResultSubject.asObservable();

	constructor(private http: HttpClient) { }

	// GET all patients filtered by query data
	getAllPatients(queryData: PatientQueryData): Observable<string> {
		console.log(queryData);
		const body = {
			Name: queryData.name,
			Email: queryData.email,
			PhoneNumber: queryData.phoneNumber,
			MedicalRecordNumber: queryData.medicalRecordNumber,
			DateOfBirth: queryData.dateOfBirth,
			Gender: queryData.gender,
			LicenseNumber: "",
			Specialization: "",
			OperationType: "",
			Priority: "",
			Status: "",
		};
		const params = new HttpParams({ fromObject: { ...queryData } });

		return new Observable<string>((observer) => {
			this.http.post<PatientData[]>(this.apiUrl + 'filter', body).subscribe({
				next: (patients: PatientData[]) => {
					this.patientsSubject.next(patients); // Update the BehaviorSubject with the fetched data
					observer.next('success'); // Emit success message
					observer.complete(); // Complete the observable
				},
				error: (err) => {
					console.error('Error fetching filtered patients', err); // Handle error appropriately
					observer.error('failure'); // Emit failure message
				}
			});
		});
	}




	getPatientById(mrn: string): void {
		const url = `${this.apiUrl}GetPatientById`;  // Endpoint for editing patient data
		const headers = new HttpHeaders({
			'id': `${mrn}`,  // Sending the token in the header
		});
		this.http.get<PatientData>(url, { headers }).subscribe({
			next: (patient: PatientData) => {
				this.selectedPatientSubject.next(patient); // Update selected patient
			},
			error: (err) => {
				console.error(`Error fetching patient with MRN ${mrn}`, err);
			}
		});
	}

	createPatientAdmin(createData: PatientRegistrationDto, token: string): Observable<any> {
		console.log('Create patient in service');
		const patientUrl = `${this.apiUrl}CreatePatient`;
		const headers = new HttpHeaders({
			'token': token,
		});

		return this.http.post<any>(patientUrl, createData, { headers });
	}

	createBlankClinicalDetailsPatient(mrn: string, token: string): Observable<any> {
		console.log('Create ClinicalDetails in service');

		const clinicalDetailsUrl = `${environment.pmdUrl}clinicalDetails/blank`;
		const headers = new HttpHeaders({
			'token': token,
			'mrn': mrn,
		});

		return this.http.post<{ code: string }>(clinicalDetailsUrl, null, { headers });
	}


	// HTTP POST request to edit patient details (Admin)
	editPatientAdmin(editData: EditPatientDtoAdmin, token: string): Observable<any> {
		console.log('Editing patient in service');
		const url = `${this.apiUrl}editPatient_Admin`;  // Endpoint for editing patient data
		const headers = new HttpHeaders({
			'token': `${token}`,  // Sending the token in the header
		});

		return this.http.post<any>(url, editData, { headers });
	}

	// DELETE request to delete patient by MRN (Medical Record Number)
	deletePatientProfile(mrn: string, token: string): Observable<any> {
		const url = `${this.apiUrl}DeletePatient`;
		const headers = new HttpHeaders({
			'token': token,
			'Content-Type': 'application/json'  // Ensure the body is sent as JSON
		});

		return this.http.delete<any>(url, {
			headers,
			body: JSON.stringify(mrn)  // Explicitly send mrn as a JSON string
		});
	}

	// -------------- STAFF --------------

	createStaff(bodyData: EditStaffDtoAdmin, Token: string) {
		const url = environment.apiUrl + `Staff/Create`;  // Endpoint for editing patient data

		let headers = new HttpHeaders().set('auth', Token);

		return this.http.put<any>(url, bodyData, { headers }).subscribe();
	}

	getAllStaff(queryData: StaffQueryData, Token: string): void {
		const params = new HttpParams({ fromObject: { ...queryData } });

		// The headers will include the 'auth' token
		const headers = new HttpHeaders({
			'auth': Token,  // Replace with the actual auth token or dynamic value
		});

		// Send POST request with query parameters and headers
		this.http.post<StaffData[]>(
			environment.apiUrl + 'Staff/Filter',
			null, // For POST, if no body content, you can pass `null` here
			{ params: params, headers: headers }
		).subscribe({
			next: (staff: StaffData[]) => {
				console.log('Fetched staff data from API:', staff);
				this.staffSubject.next(staff);  // Update the BehaviorSubject with the fetched data
			},
			error: (err) => {
				console.error('Error fetching staff', err); // Handle error appropriately
			}
		});
	}

	getStaffById(license: string): void {
		const url = environment.apiUrl + `Staff/GetStaffById`;  // Endpoint for editing patient data
		const headers = new HttpHeaders({
			'id': `${license}`,  // Sending the token in the header
		});
		this.http.get<StaffData>(url, { headers }).subscribe({
			next: (staff: StaffData) => {
				this.selectedStaffSubject.next(staff);  // Emit the fetched staff data
			},
			error: (err) => {
				console.error(`Error fetching staff with license ${license}`, err);
			}
		});
	}

	editStaffAdmin(editData: EditStaffDtoAdmin, token: string): Observable<any> {
		console.log('Editing staff in service');
		const url = environment.apiUrl + `Staff/Update`;  // Endpoint for editing patient data
		const headers = new HttpHeaders({
			'auth': `${token}`,  // Sending the token in the header
		});

		return this.http.patch<any>(url, editData, { headers });
	}

	deactivateStaffProfile(license: string, token: string): Observable<any> {
		const url = environment.apiUrl + `Staff/Disable?license=${license}`;
		const headers = new HttpHeaders({
			'auth': token,
			'Content-Type': 'application/json'  // Ensure the body is sent as JSON
		});

		return this.http.delete<any>(url, {
			headers,
			body: JSON.stringify(license)  // Explicitly send mrn as a JSON string
		});
	}

	validate(token: string) {
		const url = environment.apiUrl + `Tokens`

		const headers = new HttpHeaders({
			'token': `${token}`,  // Sending the token in the header
		});

		return this.http.post<{ role: string }>(url, null, { headers });
	}

	// ------- OPERATION TYPE ------- 

	getAllSpecializations(token: string): void {
		// https://localhost:5001/api/OperationType/getSpecializations

		let url = environment.apiUrl + `OperationType/getSpecializations`

		let headers = new HttpHeaders({
			'token': `${token}`
		});

		this.http.get<string[]>(url, { headers }).subscribe({
			next: (specialization: string[]) => {
				console.log("Fetched specialization: ", specialization);
				this.specializationSubject.next(specialization);
			},
			error: (err) => {
				console.log("Error fetching specializations", err);
			}
		});
	}

	createOperationType(token: string, operationName: string, estimatedDuration: number, phaseNames: string[],
		phasesDuration: string[], specialistNames: string[], specialistsCount: string[], specialistPhases: string[]
	): void {
		let url = environment.apiUrl + `OperationType/createOperation`;

		let headers = new HttpHeaders({
			'token': `${token}`
		});

		let body = {
			operationName: operationName,
			estimatedDuration: estimatedDuration,
			phaseNames: phaseNames,
			phasesDuration: phasesDuration,
			specialistNames: specialistNames,
			specialistsCount: specialistsCount,
			specialistPhases: specialistPhases
		};

		this.http.post<OperationTypeData>(url, body, { headers }).subscribe({
			next: (opType: OperationTypeData) => {
				console.log("Created Operation: ", opType);
				this.operationTypeSubject.next(opType);
			},
			error: (err) => {
				console.log('Error creating operation type', err);
			}

		});

		// https://localhost:5001/api/OperationType
		/* 
		{
			"operationName": "testString",
			"estimatedDuration": "90",
			"phaseNames": [
				"preparation",
				"surgery",
				"cleaning"
			],
			"phasesDuration": [
				"15", "60", "15"
			],
			"specialistNames": [
				"Anaesthesist",
				"Doctor",
				"assistant",
				"Cleaner"
			],
			"specialistsCount": [
				"2",
				"1",
				"3",
				"5"
			],
			"specialistPhases": [
				"preparation",
				"surgery",
				"surgery",
				"cleaning"
			]
		} 
		*/

	}

	getOperationTypeFiltered(token: string, operationName: string, specialization: string, status: string): void {
		// https://localhost:5001/api/OperationType/filterOperationType?operationName=test2&specialization=testSpecialization&activeStatus=active
		let url = environment.apiUrl + `OperationType/filterOperationType`;
		// let url = `https://localhost:5001/api/OperationType/filterOperationType?operationName=${operationName}&specialization=${specialization}&activeStatus=${status}`

		/* let headers = new HttpHeaders({
			'token': `${token}`
		}); */
		let headers = new HttpHeaders().set('token', token);

		let check = false;
		if (operationName != "") {
			url += `?operationName=${operationName}`;
			check = true;
		}
		if (specialization != "") {
			if (check)
				url += `&specialization=${specialization}`;
			else {
				check = true
				url += `?specialization=${specialization}`;
			}
		}
		if (status != "") {
			if (check)
				url += `&activeStatus=${status}`;
			else {
				check = true;
				url += `?activeStatus=${status}`;
			}
		}
		console.log(url);
		console.log(headers);

		this.http.get<OperationTypeResultData[]>(url, { headers }).subscribe({
			next: (opType: OperationTypeResultData[]) => {
				console.log("Got Operation: ", opType);
				this.operationTypeResultsSubject.next(opType);
			},
			error: (err) => {
				console.log('Error getting operation type', err);
			}
		});
	}

	deleteOperationType(token: string, name: string): void {
		// https://localhost:5001/api/OperationType/delete?name=test
		// let url = `https://localhost:5001/api/OperationType/delete?name=${name}`
		let url = environment.apiUrl + `OperationType/deactivateOperation/${name}`
		// let url = `https://localhost:5001/api/OperationType/delete` //?name=${name}`
		// https://localhost:5001/api/OperationType/delete
		/* let headers = new HttpHeaders({
			'token': `${token}`
		}); */
		/* let body = {
			name: name,
		} */
		let headers = new HttpHeaders().set('token', token);
		console.log(token);
		console.log(headers);

		console.log("Hello???", url);

		this.http.delete<any>(url, {
			headers,
			//body: JSON.stringify(name)  // Explicitly send mrn as a JSON string
		}).subscribe({
			next: (response) => console.log("Delete successful:", response),
			error: (err) => console.error("Delete failed:", err),
		});

		/* this.http.delete<any>(url, { headers }).subscribe({
			next: (response) => console.log("Delete successful:", response),
			error: (err) => console.error("Delete failed:", err),
		}); */
	}

	patchOperationType(token: string, operationName: string, estimatedDuration: string, phaseNames: string[],
		phasesDuration: string[], specialistNames: string[], specialistsCount: string[], specialistPhases: string[]
	): void {
		// https://localhost:5001/api/OperationType/test2
		let url = environment.apiUrl + `OperationType/updateOperation/${operationName}`;

		let headers = new HttpHeaders({
			'token': `${token}`
		});

		let body = {
			operationName: operationName,
			estimatedDuration: estimatedDuration,
			phaseNames: phaseNames,
			phasesDuration: phasesDuration,
			specialistNames: specialistNames,
			specialistsCount: specialistsCount,
			specialistPhases: specialistPhases
		};
		// console.log(body);

		this.http.patch<OperationTypeData>(url, body, { headers }).subscribe({
			next: (opType: OperationTypeData) => {
				console.log("Pached Operation: ", opType);
				this.operationTypeSubject.next(opType);
			},
			error: (err) => {
				console.log('Error patching operation type', err);
			}
		});
	}

	// -------------- SCHEDULLING --------------


	getOperationRequests(token: string) {
		const url = environment.apiUrl + `OperationRequest/ListForAdmin`

		const headers = new HttpHeaders({
			'auth': `${token}`, // Really should start thinking about having everything either be named 'auth' or 'token'.
		});
		console.log(headers);
		this.http.get<OperationRequestData[]>(url, { headers }).subscribe({
			next: (opRequest: OperationRequestData[]) => {
				console.log('Fetched operation request from API:', opRequest);
				this.opReqSubject.next(opRequest);
			},
			error: (err) => {
				console.error('Error fetching operation request', err);
			}
		});
	}

	getOperationRooms(token: string) {
		const url = environment.apiUrl + `OperationRequest/OperationRooms`

		const headers = new HttpHeaders({
			'auth': `${token}`, // Really should start thinking about having everything either be named 'auth' or 'token'.
		});

		this.http.get<OperationRoomData[]>(url, { headers }).subscribe({
			next: (opRoom: OperationRoomData[]) => {
				console.log('Fetched room from API:', opRoom);
				this.opRoomsSubject.next(opRoom);
			},
			error: (err) => {
				console.error('Error fetching rooms', err);
			}
		});
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

	patchAllergies(token: string, id: string, dto: AllergyData): Observable<AllergyData> {
		const url = environment.pmdUrl + 'allergies';
		let body = {
			name: dto.name,
			description: dto.description
		}
		const headers = new HttpHeaders({
			'token': `${token}`,
			'icd': `${id}`,
		});

		return this.http.patch<AllergyData>(url, body, { headers });
	}

	createAllergy(token: string, dto: AllergyData): Observable<AllergyData> {
		console.log('Sending DTO:', dto);
		const url = environment.pmdUrl + 'allergies';

		let body = {
			name: dto.name,
			description: dto.description
		}

		const headers = new HttpHeaders({
			'token': `${token}`,
			'Content-Type': 'application/json',  // Make sure the content type is set
		});

		return this.http.post<any>(url, body, { headers });
	}

	createOperationRoomType(token: string, data: OperationRoomTypeData): Observable<any> {
		console.log(data);
		const url = `${environment.apiUrl}OperationRoomType`; // Ensure the URL is correctly formatted
		const headers = new HttpHeaders({
			'Content-Type': 'application/json',
			'token': `${token}` // Ensure the header name matches what the backend expects
		});

		return this.http.post(url, data, { headers });
	}

	getSchedule(dto: ScheduleAppointmentsDto, token: string) {
		console.log(dto);
		const url = environment.apiUrl + `OperationRequest/Schedule`

		const headers = new HttpHeaders({
			'auth': `${token}`,
		});

		this.http.post<SchedulingResult>(url, dto, { headers }).subscribe({
			next: (appointment: SchedulingResult) => {
				this.schedulingResultSubject.next(appointment);
				console.log(appointment);
			},
			error: (err) => {
				console.error("Error fetching rooms", err);
			}
		});
	}

	// -------------- MEDICAL CONDITIONS --------------

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

	createMedicalCondition(token: string, dto: MedicalConditionData): Observable<MedicalConditionData> {
		console.log('Sending creation DTO:', dto);
		const url = environment.pmdUrl + 'medical-conditions';

		const body = {
			code: dto.code,
			designation: dto.designation,
			description: dto.description,
			symptoms: dto.symptoms,
		};

		const headers = new HttpHeaders({
			'token': `${token}`,
			'Content-Type': 'application/json',
		});

		return this.http.post<MedicalConditionData>(url, body, { headers });
	}


	// --------------- SPECIALIZATION -----------------------------

	createSpecialization(token: string, dto: SpecializationData): void {
		// https://localhost:5001/api/Specialization/
		let url = environment.apiUrl + `Specialization/CreateSpecialization`;

		let headers = new HttpHeaders({ 'token': `${token}` });

		let body = {
			SpecializationCode: dto.SpecializationCode,
			SpecializationName: dto.SpecializationName,
			SpecializationDescription: dto.SpecializationDescription
		};

		this.http.post<SpecializationData>(url, body, { headers }).subscribe({
			next: (sp: SpecializationData) => {
				console.log("Specialization Created: ", sp);
				this.specializationResultSubject.next(sp);
			},
			error: (err) => {
				console.log('Error creating specialization', err);
			}
		})
	}

	getSpecializations(token: string): void {
		let url = environment.apiUrl + `Specialization/GetSpecializationList`;
		let headers = new HttpHeaders({ 'token': `${token}` });
		this.http.get<SpecializationData[]>(url, { headers }).subscribe({
			next: (sp: SpecializationData[]) => {
				console.log('Fetched specializations: ', sp);
				this.specializationResultsSubject.next(sp);
			},
			error: (err) => {
				console.log('Error getting specializations', err);
			}
		})
	}

	editSpecialization(token: string, dto: SpecializationData): void {
		let url = environment.apiUrl + `Specialization/EditSpecialization/${dto.SpecializationCode}`;
		let headers = new HttpHeaders({ 'token': `${token}` });
		let body = {
			SpecializationCode: dto.SpecializationCode,
			SpecializationName: dto.SpecializationName,
			SpecializationDescription: dto.SpecializationDescription
		};

		this.http.patch<SpecializationData>(url, body, { headers }).subscribe({
			next: (sp: SpecializationData) => {
				console.log('Fetched specializations: ', sp);
				this.specializationResultSubject.next(sp);
			},
			error: (err) => {
				console.log('Error getting specializations', err);
			}
		})
	}

	deleteSpecialization(token: string, code: string): void {
		//https://localhost:5001/api/Specialization/DeleteSpecialization/123tr
		let url = environment.apiUrl + `Specialization/DeleteSpecialization/${code}`

		let headers = new HttpHeaders().set('token', token);
		/* console.log(token);
		console.log(headers);

		console.log("Hello???", url); */

		this.http.delete<any>(url, {
			headers,
			//body: JSON.stringify(name) 
		}).subscribe({
			next: (response) => console.log("Delete successful:", response),
			error: (err) => console.error("Delete failed:", err),
		});

		/* this.http.delete<any>(url, { headers }).subscribe({
			next: (response) => console.log("Delete successful:", response),
			error: (err) => console.error("Delete failed:", err),
		}); */
	}

}
