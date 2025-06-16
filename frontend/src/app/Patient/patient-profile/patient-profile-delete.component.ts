import { Component, OnInit } from '@angular/core';
import { LoginServiceService } from '../../login-service.service';
import { PatientService } from '../patient.service';
import { LoginResponse } from '../../login-result';
import { EditPatientDtoPatient, PatientData } from '../interfaces/patient-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-panel',
  standalone: true,
  templateUrl: './patient-profile-delete.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class PatientProfileDeletePanelComponent implements OnInit {
  response: LoginResponse | null = null; 
  patient: PatientData | null = null; 
  message: string = 'Are you sure you want to delete your patient profile?'; 
  errorMessage: string = ''; 
  showConfirmationCode: boolean = false; 
  showDeleteButton: boolean = true; 
  confirmationCode: string = ''; // Variable to hold the confirmation code

  constructor(
    private loginService: LoginServiceService,
    private patientService: PatientService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    //localStorage.setItem("authToken", "aa9f2600-35a2-49ed-bba6-dcb51fbcff1e")
    //localStorage.setItem("authToken", "ce9ace42-1e77-45b4-a210-18fc03edea07")
    // Check if the token is in localStorage
    const storedToken = localStorage.getItem('authToken');
    console.log(storedToken);
    if (storedToken) {
      this.patientService.validate(storedToken).subscribe(response => {
        console.log(response.role);
        if(response.role != "PATIENT"){
          console.log(response.role);
          this.router.navigate(['']);
        }
      })
      // Assign the stored token to response and trigger patient data loading
      this.response = { Token: storedToken } as LoginResponse;
      this.initializeData(storedToken);
    } else {
      // Subscribe to the loginService response$ if no token is in localStorage
      this.loginService.response$.subscribe((response: LoginResponse | null) => {
        if (response) {
          this.response = response;
          // Store the token in localStorage
          localStorage.setItem('authToken', response.Token);
          this.initializeData(response.Token);
        }
      });
    }
  }

  // Method to initialize or refresh data
  initializeData(storedToken:string): void {
    this.patientService.getPatientById(storedToken)
    this.patientService.selectedPatient$.subscribe((patient: PatientData | null) => {
      this.patient = patient;
      console.log('Fetched patient by ID:', this.patient);
    });
  }

  convertToEditPatientDto(patient: PatientData): EditPatientDtoPatient {
    return {
      firstName: patient.firstName,
      lastName: patient.lastName,
      fullName: patient.fullName,
      email: patient.email,
      phone: patient.phone,
      dateOfBirth: patient.dateOfBirth,
      gender: patient.gender,
      emergencyContact: patient.emergencyContact
    };
  }

  deleteSelfProfile(): void {
    if (this.patient && this.response) {
      this.patientService.deletePatientProfile(this.response.Token).subscribe(
        (result) => {
            console.log('Delete request successfull:', result);
            this.showConfirmationCode = true;
            this.showDeleteButton = false;
            this.message = 'Email Requesting Deletion Sent. Please input the sent code.'
            this.errorMessage = '';
        }, 
        (error) => {
          console.error('Error requesting deletion:', error);
          this.errorMessage = 'Error Requesting the Delete.';
        }
      );
    }
  }

  confirmDelete(): void{
    if(this.confirmationCode){
      console.log(this.confirmationCode);
      this.patientService.confirmPatientProfileDelete(this.confirmationCode).subscribe(
        (result) => {
          console.log('Patient updated successfully:', result);
          this.showConfirmationCode = false;
          this.showDeleteButton = false;
          this.message = 'Deletion Request Accepted. The profile will be deleted within the GRPD parameters (within 30 days).'
          this.errorMessage = '';
          },
        (error) => {
          console.error('Error confirming deletion:', error);
        }
        )
    }
  }
}
