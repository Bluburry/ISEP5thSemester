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
	templateUrl: './policy-privacy.component.html',
	imports: [CommonModule, RouterModule, RouterOutlet, RouterLinkActive, RouterLink],
	styles: []
})
export class PrivacyPolicyComponent {
	
	constructor(
		private loginService: LoginServiceService,
		private patientService: PatientService,
		private router: Router,
		private doctorService: DoctorService
	) { }

	navigateToPanel(): void {
		this.router.navigate(['']);
	}


}




