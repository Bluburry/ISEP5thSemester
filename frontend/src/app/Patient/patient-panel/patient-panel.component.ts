import { booleanAttribute, Component, OnInit, Version } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginServiceService } from '../../login-service.service';
import { PatientService } from '../patient.service';
import { LoginResponse } from '../../login-result';
import { PatientData } from '../interfaces/patient-data';
import { ClinicalDetails } from '../../Doctor/interfaces/patient-data';
import { DoctorService } from '../../Doctor/doctor.service';
import { AllergyData } from '../../Admin/interfaces/allergy-data';
import { MedicalConditionData } from '../../Admin/interfaces/medical-condition-data';
import { PDFDocument, StandardFonts } from 'pdf-lib';

@Component({
	selector: 'app-patient-panel',
	standalone: true,
	templateUrl: './patient-panel.component.html',
	imports: [CommonModule, RouterModule, RouterOutlet, RouterLinkActive, RouterLink],
	styles: []
})
export class PatientPanelComponent {
	user: string = '';
	role: string = '';
	patient: PatientData | null = null; // The patient object to be edited
	clinicalDetails: ClinicalDetails | null = null;
	patientAllergies: AllergyData[] | null = null;
	patientConditions: MedicalConditionData[] | null = null;
	zipPass: string | null = null;
	isPopupOpen: boolean | null = null;

	constructor(
		private loginService: LoginServiceService,
		private patientService: PatientService,
		private router: Router,
		private doctorService: DoctorService
	) { }

	ngOnInit(): void {
		const auth = localStorage.getItem('authToken');
		if (auth) {
			this.loginService.getLogin(auth).subscribe(
				userDto => {
					console.log('UserDto retrieved:', userDto);
					localStorage.setItem("user", userDto.EmailAddress);
					localStorage.setItem("role", userDto.Role);
					this.user = userDto.EmailAddress;
					this.role = userDto.Role;
					if (userDto.Role != "PATIENT") {
						console.log(userDto.Role);
						this.router.navigate(['']);
					}
				},
				error => {
					console.error('Error retrieving UserDto:', error);
				}
			)
			this.initializeData(auth);
		} else {
			// Subscribe to the loginService response$ if no token is in localStorage
			this.loginService.response$.subscribe((response: LoginResponse | null) => {
				if (response) {
					localStorage.setItem('authToken', response.Token);
					this.initializeData(response.Token);
				}
			});
		}
	}

	// Method to initialize or refresh data
	initializeData(storedToken: string): void {
		this.patientService.getPatientById(storedToken)
		this.patientService.selectedPatient$.subscribe((patient: PatientData | null) => {
			if (patient) {
				this.patient = patient;
				this.fetchPatientById(patient.mrn, storedToken);
				console.log('Fetched patient by ID:', this.patient);
			}
		});
	}

	navigateToPanel(link: String): void {
		this.router.navigate([link]);
	}
	
	privacyPolicy() {
		window.open('privacy-policy', '_blank')  
	}

	logout(): void {
		localStorage.clear();
		this.router.navigate(['']);
	}

	fetchPatientById(id: string, token: string): void {
		this.doctorService.getClinicalDetails(token, id).subscribe({
			next: (clinicalDetails: ClinicalDetails) => {
				console.log('Clinical Details retrieved:', clinicalDetails);
				this.clinicalDetails = clinicalDetails;
				this.patientAllergies = clinicalDetails.allergies;
				this.patientConditions = clinicalDetails.medicalConditions
			},
			error: (err) => {
				console.error('Error fetching Clinical Details:', err);
			}
		});
	}

	async downloadInfo() {
		const auth = localStorage.getItem('authToken');
		if (auth && this.patient) {
			const pass = this.patientService.generatePassword(18);
			this.patientService.sendClinicalDetails(auth, this.patient.mrn, pass).subscribe({
				next: (response: Blob) => {
					console.log('File received:', response);
					
					// Check if the file is empty
					if (response.size === 0) {
						console.log('Received file is empty, not downloading.');
						return; // Exit if the file is empty
					}

					// Create a URL for the Blob and trigger a download
					const blob = new Blob([response], { type: 'application/zip' });
					const url = window.URL.createObjectURL(blob);
					const a = document.createElement('a');
					a.href = url;
					a.download = 'clinicalDetails.zip'; // File name for the download
					document.body.appendChild(a);
					a.click();
					window.URL.revokeObjectURL(url);
					document.body.removeChild(a);
					this.zipPass = pass;
				},
				error: (err) => {
					console.error('Error fetching Clinical Details:', err);
				},
			});
		}
	}

	async downloadPresentation() {
		const auth = localStorage.getItem('authToken');
		if (auth) {
			const pass = this.patientService.generatePassword(18);
			console.log(pass);
			this.patientService.sendClinicalDetailsPresentation(auth, pass).subscribe({
				next: (response: Blob) => {
					console.log('File received:', response);

					// Check if the file is empty
					if (response.size === 0) {
						console.log('Received file is empty, not downloading.');
						return; // Exit if the file is empty
					}

					// Create a URL for the Blob and trigger a download
					const blob = new Blob([response], { type: 'application/zip' });
					const url = window.URL.createObjectURL(blob);
					const a = document.createElement('a');
					a.href = url;
					a.download = 'clinicalDetails.zip'; // File name for the download
					document.body.appendChild(a);
					a.click();
					window.URL.revokeObjectURL(url);
					document.body.removeChild(a);
					this.zipPass = pass;
				},
				error: (err) => {
					console.error('Error fetching Clinical Details:', err);
				},
			});
		}
	}

	openPopup() {
		this.isPopupOpen = true;
	}
	
	closePopup() {
		this.isPopupOpen = false;
	}
}




