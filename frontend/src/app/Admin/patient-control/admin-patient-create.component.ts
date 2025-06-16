import { Component, OnInit } from '@angular/core';
import { LoginServiceService } from '../../login-service.service';
import { AdminService } from '../admin.service';
import { LoginResponse } from '../../login-result';
import { PatientRegistrationDto, PatientData } from '../interfaces/patient-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-patient-panel',
  standalone: true,
  templateUrl: './admin-patient-create.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class AdminCreatePatientPanelComponent implements OnInit {
  response: LoginResponse | null = null; 
  patient: PatientRegistrationDto | null = null; //The patient object to be edited
  sucessMessage: string = ''; 
  errorMessage: string = ''; 

  constructor(
    private loginService: LoginServiceService,
    private adminService: AdminService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    // Check if the token is in localStorage
    const initPatient = this.initiatePatientDate();
    this.patient = initPatient;
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      // Assign the stored token to response and trigger patient data loading
      this.response = { Token: storedToken } as LoginResponse;
    } else {
      // Subscribe to the loginService response$ if no token is in localStorage
      this.loginService.response$.subscribe((response: LoginResponse | null) => {
        if (response) {
          this.response = response;
          // Store the token in localStorage
          localStorage.setItem('authToken', response.Token);
        }
      });
    }
  }

  initiatePatientDate(): PatientRegistrationDto {
      return{
        firstName: '',
        lastName: '',
        fullName: '',
        email: '',
        phone: '',
        dateOfBirth: '',
        gender: '',
        emergencyContact: '',
      }
  }

  createPatient(): void {
    if (!this.patient || !this.response) {
      this.errorMessage = 'Missing patient or response data.';
      this.sucessMessage = '';
      return;
    }
  
    const missingField = Object.keys(this.patient).find(
      (key) => this.patient![key as keyof PatientRegistrationDto] === ''
    );
  
    if (missingField) {
      this.errorMessage = `Missing information for: ${missingField}`;
      this.sucessMessage = '';
      return;
    }

    if (this.patient && this.response) {
      this.adminService.createPatientAdmin(this.patient, this.response.Token).subscribe(
        (result: {mrn: string}) => {
            if (this.response){
            console.log('Patient Created Sucessfully!:', result.mrn);
            this.adminService.createBlankClinicalDetailsPatient(result.mrn, this.response.Token).subscribe(
              (result) => {
                this.sucessMessage = 'Patient Created Sucessfully!';
                this.errorMessage = '';
              },
              (error) => {
                console.error('Error creating clinicalDetails:', error);
                this.errorMessage = 'Error Creating Patient.';
                this.sucessMessage = '';
              }
            );
            }
        }, 
        (error) => {
          console.error('Error creating patient:', error);
          this.errorMessage = 'Error Creating Patient.';
          this.sucessMessage = '';
        }
      );
    }
  }
}
