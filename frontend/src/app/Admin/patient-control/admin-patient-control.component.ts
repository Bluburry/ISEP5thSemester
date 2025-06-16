import { Component, OnInit } from '@angular/core';
import { LoginServiceService } from '../../login-service.service';
import { AdminService } from '../admin.service';
import { LoginResponse } from '../../login-result';
import { EditPatientDtoAdmin, PatientData } from '../interfaces/patient-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientQueryData } from '../interfaces/patient-query-data';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  templateUrl: './admin-patient-control.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class AdminPatientControlPanelComponent implements OnInit {
  response: LoginResponse | null = null; 
  patients: PatientData[] = [];  // Store the patient data
  patient: PatientData | null = null; // The patient object to be edited
  queryData: PatientQueryData = {
    name: '',
    email: '',
    phoneNumber: '',
    medicalRecordNumber: '',
    dateOfBirth: '',
    gender: ''
  };
  statusMessage: string | null = null; // To hold the error message

  constructor(
    private loginService: LoginServiceService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    // Check if the token is in localStorage
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      // Assign the stored token to response and trigger patient data loading
      this.response = { Token: storedToken } as LoginResponse;
      this.initializeData();
      this.statusMessage = 'Patient Data Initialized';
    } else {
      // Subscribe to the loginService response$ if no token is in localStorage
      this.loginService.response$.subscribe((response: LoginResponse | null) => {
        if (response) {
          this.response = response;
          // Store the token in localStorage
          localStorage.setItem('authToken', response.Token);
          this.initializeData();
          this.statusMessage = 'Patient Data Initialized';
        }
      });
    }
  }

  // Method to initialize or refresh data
  initializeData(): void {
    this.refreshPatients();
    this.adminService.patients$.subscribe(
      (patients: PatientData[]) => {
        this.patients = patients;
        console.log('Patients data:', this.patients);
      },
      (error) => {
        this.statusMessage = 'Error loading patient data.';
        console.error('Error loading patients:', error);
      }
    );

    this.adminService.selectedPatient$.subscribe((patient: PatientData | null) => {
      this.patient = patient;
      console.log('Fetched patient by ID:', this.patient);
    });
  }

  // Refresh patient data
  refreshPatients(): void {
    this.adminService.getAllPatients(this.queryData).subscribe({
      next: (status) => console.log(`Operation status: ${status}`),
      error: (err) => this.statusMessage = "Invalid Data in Refresh Field."
    });
    this.statusMessage = 'Patient List Refreshed.';
  }

  // Fetch a patient by MRN (Medical Record Number)
  fetchPatientById(id: string): void {
    if (id) {
      this.adminService.getPatientById(id);
      this.adminService.selectedPatient$.subscribe(
        (patient: PatientData | null) => {
          this.patient = patient;
          console.log('Fetched patient by ID:', this.patient);
          this.statusMessage = null; // Clear any previous error messages
        },
        (error) => {
          this.statusMessage = 'Error fetching patient by ID.';
          console.error('Error fetching patient by ID:', error);
        }
      );
    } else {
      this.statusMessage = 'Patient ID input is empty.';
      console.error('Patient ID input is empty.');
    }
  }

  convertToEditPatientDto(patient: PatientData): EditPatientDtoAdmin {
    return {
      patientId: patient.mrn,
      firstName: patient.firstName,
      lastName: patient.lastName,
      fullName: patient.fullName,
      email: patient.email,
      phone: patient.phone,
      dateOfBirth: patient.dateOfBirth,
    };
  }

  editPatient(): void {
    if (this.patient && this.response) {
      const editPatientDto = this.convertToEditPatientDto(this.patient);
      this.adminService.editPatientAdmin(editPatientDto, this.response.Token).subscribe(
        (result) => {
          console.log('Patient updated successfully:', result);
          this.statusMessage = 'Patient Edited Successfully.';
        },
        (error) => {
          this.statusMessage = 'Error updating patient.';
          console.error('Error updating patient:', error);
        }
      );
    }
  }

  deleteSelectedPatient(): void {
    if (this.patient && this.response) {
      this.adminService.deletePatientProfile(this.patient.mrn, this.response.Token).subscribe({
        next: () => {
          console.log('Patient deleted successfully');
          this.refreshPatients();
          this.patient = null;
          this.statusMessage = 'Patient has been successfully deleted.';
        },
        error: (err) => {
          this.statusMessage = 'Error deleting patient.';
          console.error('Error deleting patient:', err);
        }
      });
    } else {
      this.statusMessage = 'No patient selected or missing authorization token';
      console.error('No patient selected or missing authorization token');
    }
  }
}
