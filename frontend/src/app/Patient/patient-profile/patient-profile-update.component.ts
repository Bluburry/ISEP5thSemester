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
  templateUrl: './patient-profile-update.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class PatientProfileUpdatePanelComponent implements OnInit {
  response: LoginResponse | null = null; 
  patient: PatientData | null = null; // The patient object to be edited
  sucessMessage: string = ''; 
  errorMessage: string = ''; 
  showConfirmationCode: boolean = false; 
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

  editPatient(): void {
    if (this.patient && this.response) {
      const editPatientDto = this.convertToEditPatientDto(this.patient);
      this.patientService.editPatientProfile(editPatientDto, this.response.Token).subscribe(
        (result) => {
          if(this.isMessageObject(result)){
            console.log('Sensitive information is trying to be altered. Patient update request successfull:', result);
            this.sucessMessage = 'Sensitive information is trying to be altered. An e-mail has been sent to confirm changes.';
            this.showConfirmationCode = true;
          } else {
            console.log('Patient updated successfully:', result);
            this.sucessMessage = 'Patient Updated Successfully!';
          }
          this.errorMessage = '';
        }, 
        (error) => {
          console.error('Error updating patient:', error);
          this.errorMessage = 'Error Updating Patient.';
          this.sucessMessage = '';
        }
      );
    }
  }

  // Helper function to check if the result is a message object
  private isMessageObject(result: any): result is { message: string } {
    return result && typeof result === 'object' && 'message' in result;
  }

  confirmUpdate(): void{
    if(this.confirmationCode){
      console.log(this.confirmationCode);
      this.patientService.confirmPatientProfileUpdate(this.confirmationCode).subscribe(
        (result) => {
          console.log('Patient updated successfully:', result);
          this.sucessMessage = 'Patient Updated Successfully!';
          this.errorMessage = '';
          this.showConfirmationCode = false;
          },
        (error) => {
          console.error('Error confirming update:', error);
          this.errorMessage = 'Error confirming update.';
          this.sucessMessage = '';
        }
        )
    }
  }
}
