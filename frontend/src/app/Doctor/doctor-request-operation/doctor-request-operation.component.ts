import { Component, OnInit } from '@angular/core';
import { DoctorService } from '../doctor.service';
import { LoginResponse } from '../../login-result';
import { PatientData } from '../interfaces/patient-data';
import { OperationTypeData } from '../interfaces/operation-type-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  templateUrl: './doctor-request-operation.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class DoctorRequestOperationComponent implements OnInit {
  response: LoginResponse | null = null; 
  patients: PatientData[] = [];
  selectedPatient: PatientData | null = null;

  operationTypes: OperationTypeData[] = [];
  selectedOperationType: string | null = null;
  
  operationDeadline: string | null = null;
  operationPriority: string | null = null;

  storedToken = localStorage.getItem('authToken');

  constructor(
    private doctorService: DoctorService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    if (this.storedToken) {
      this.doctorService.validate(this.storedToken).subscribe(response => {
        if(response.role != "STAFF"){
          console.log("whar");
          this.router.navigate(['']);
        }
      })
      this.response = { Token: this.storedToken } as LoginResponse;
      this.initializeData(this.storedToken);
    } else {
      this.router.navigate(['']);
    }
  }

  initializeData(token: string): void {
  
    this.doctorService.getPatients();
    this.doctorService.patients$.subscribe((patient: PatientData[]) => {
      this.patients = patient;
      console.log('Patient data:', this.patients);
    });
  
    this.doctorService.getOperationTypes(token);
    this.doctorService.opTypes$.subscribe((opType: OperationTypeData[]) => {
      this.operationTypes = opType;
      console.log('Operation type data:', this.operationTypes);
    });

  }
  
  createRequest(): void {
    console.log(this.storedToken);
    if (this.selectedPatient && this.selectedOperationType && this.operationDeadline && this.operationPriority && this.storedToken) {
      this.doctorService.createOperation(this.selectedPatient.mrn, this.selectedOperationType, this.operationDeadline, this.operationPriority, this.storedToken)
    }
  }

  fetchPatientById(mrn: string): void {
    console.log(mrn);
    if (mrn){ 
      this.doctorService.getPatientById(mrn);
      this.doctorService.selectedPatient$.subscribe((patient: PatientData | null) => {
        this.selectedPatient = patient;
        console.log('Fetched patient by License:', this.selectedPatient);
      });
    } else {
      console.error('Patient license input is empty.');
    }
  }
}
