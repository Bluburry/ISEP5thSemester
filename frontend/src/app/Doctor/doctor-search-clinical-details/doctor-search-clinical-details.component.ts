import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { DoctorService } from '../doctor.service';
import { MedicalConditionData } from '../interfaces/medical-condition-data';
import { AllergyData } from '../../Admin/interfaces/allergy-data';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ClinicalDetails, PatientData } from '../interfaces/patient-data';
import { PatientQueryData } from '../interfaces/patient-query-data';

@Component({
  standalone: true,
  selector: 'app-update-clinical-details',
  templateUrl: './doctor-search-clinical-details.component.html',
  styleUrls: ['./doctor-search-clinical-details.component.css'],
  imports: [CommonModule, FormsModule]
})
export class ClinicalDetailsSearchComponent implements OnInit {
  medicalConditionForm: FormGroup;
  token: string = '';
  //==================================
  patients: PatientData[] = [];
  conditions: MedicalConditionData[] = [];
  allergies: AllergyData[] = [];
  //==================================
  selectedAppointmentAllergies: AllergyData[] = [];
  selectedAppointmentConditions: MedicalConditionData[] = [];

  selectedPatient: PatientData | null = null;
  selectedCondition: MedicalConditionData | null = null;
  selectedAllergy: AllergyData | null = null;
  selectedClinicalDetails: ClinicalDetails | null = null;
  originalPatientConditions: MedicalConditionData[] | null = null;     //Indicator to track ClinicalDetails changes
  originalPatientAllergies: AllergyData[] | null = null;      //Indicator to track ClinicalDetails changes
  //==================================
  queryDataPatient: PatientQueryData = {name: '', email: '', phoneNumber: '', medicalRecordNumber: '', dateOfBirth: '', gender: ''}
  queryDataCondition: MedicalConditionData = { code: '', designation: '', description: '', symptoms: ''};
  queryDataAllergy: AllergyData = { id: '', name: '', description: ''};

  queryAllergy: string = "";
  queryCondition: string = "";
  //==================================
  errorMessageAllergy: string | null = null;
  errorMessageCondition: string | null = null;
  errorMessagePatient: string | null = null;
  errorMessageClinical: string | null = null;
  //==================================

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService
  ) {
    this.medicalConditionForm = this.fb.group({
      Code: [''],
      Designation: [''],
      Description: ['']
    });
  }

  ngOnInit() {
    this.token = localStorage.getItem('authToken') || this.token;
    this.refreshConditions();
    this.getAllergies();
    this.getMedicalConditions();
    this.getPatients();
  }

// ================= GETS =================

  getMedicalConditions() {
    const dto: MedicalConditionData = this.queryDataCondition;
    if (/[^0-9]/.test(dto.code)) {
      this.errorMessageCondition = "Error fetching medical conditions: Code can only be numeric.";
    } else{
    this.doctorService.getMedicalConditions(this.token, dto).subscribe({
      next: (conditions: MedicalConditionData[]) => {
        this.conditions = conditions;
        this.errorMessageCondition = "";
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching medical conditions", err);
        this.errorMessageCondition = `Error fetching medical conditions: ${err.error.message}`;
      }
    });
    }
  }

  getAllergies() {
      const dto: AllergyData = this.queryDataAllergy;
      this.doctorService.getAllergies(this.token, dto).subscribe({
        next: (allergies: AllergyData[]) => {
          this.allergies = allergies;
          this.errorMessageAllergy = "";
        },
        error: (err: HttpErrorResponse) => {
          console.error("Error fetching allergies", err);
          this.errorMessageAllergy = `Error fetching allergies: ${err.message}`;
        }
      });
    }

  getPatients(): void {
    this.doctorService.getQueryPatients(this.token, this.queryDataPatient).subscribe({
      next: (patients: PatientData[]) => {
        this.patients = patients;
        this.errorMessagePatient = "";
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching patients", err);
        this.errorMessagePatient = `Error fetching patients: ${err.error.message}`;
      }
    });
    }

// ================= RESETS =================

  resetPatient() {
    this.queryDataPatient = {name: '', email: '', phoneNumber: '', medicalRecordNumber: '', dateOfBirth: '', gender: ''}
    this.selectedPatient = null;
    this.selectedClinicalDetails = null;
    this.getPatients();
  }

  resetCondition() {
    this.queryDataCondition = { code: '', designation: '', description: '', symptoms:'' };
    this.selectedCondition = null;
    this.getMedicalConditions();
  }

  resetAllergy() {
    this.queryDataAllergy = { id: '', name: '', description: '' };
    this.selectedAllergy = null;
    this.getAllergies();
  }

  resetAll(){
    this.resetCondition();
    this.resetAllergy();
    this.resetPatient();
  }

  refreshConditions() {
    this.getMedicalConditions();
    this.getAllergies();
  }

// ================= FETCH =================

  fetchConditionById(code: string) {
    console.log("Showing Medical Condition with code: " + code);
    this.selectedCondition = this.conditions.find(condition => condition.code === code) || null;
    if (this.selectedClinicalDetails){
      const index = this.selectedClinicalDetails.medicalConditions.findIndex(
        condition => condition.code === this.selectedCondition?.code
      );
    }
  }

  fetchAllergyById(id: string) {
    console.log("Showing Allergy with ID: " + id);
    this.selectedAllergy = this.allergies.find(allergy => allergy.id === id) || null;
    if (this.selectedClinicalDetails){
      const index = this.selectedClinicalDetails.allergies.findIndex(
        allergy => allergy.id === this.selectedAllergy?.id
      );
    }
  }

  fetchPatientById(id: string): void {
    const patient = this.patients.find(patient => patient.mrn === id) || null;
      this.errorMessagePatient = ``;
      this.selectedPatient = patient;
      if (this.selectedPatient){
          this.doctorService.getClinicalDetails(this.token, this.selectedPatient.mrn).subscribe({
            next: (clinicalDetails) => {
                console.log('Clinical Details retrieved:', clinicalDetails);
                this.selectedClinicalDetails = clinicalDetails;
                this.originalPatientAllergies = [...this.selectedClinicalDetails.allergies];
                this.originalPatientConditions = [...this.selectedClinicalDetails.medicalConditions];
                                         
                if (this.selectedClinicalDetails){
                  let index = this.selectedClinicalDetails.allergies.findIndex(
                    allergy => allergy.id === this.selectedAllergy?.id
                  );
        
                  index = this.selectedClinicalDetails.medicalConditions.findIndex(
                    condition => condition.code === this.selectedCondition?.code
                  );
                }

                this.selectedAppointmentAllergies = [];
                this.selectedAppointmentConditions = [];

                clinicalDetails.allergies.forEach(element => {
                  const allergy = this.allergies.find(allergy => allergy.id === element.id) || null;
                  if (allergy){
                    this.selectedAppointmentAllergies.push(allergy);
                    console.log("hi");
                  }
                });

                clinicalDetails.medicalConditions.forEach(element => {
                  const condition = this.conditions.find(condition => condition.code === element.code) || null;
                  if (condition){
                    this.selectedAppointmentConditions.push(condition);
                  }
                });
            },
            error: (err) => {
                console.error('Error fetching Clinical Details:', err);
            }
          });
      }
    }

  queryPatientsByClinical(): void {
    this.errorMessagePatient = ``;
    let a = this.patients;
    this.patients = [];
    this.selectedAppointmentAllergies = [];
    this.selectedAppointmentConditions = [];
    if (this.queryAllergy || this.queryCondition || true){
      this.doctorService.getFilteredDetails(this.token, this.queryAllergy, this.queryCondition).subscribe({
          next: (clinicalDetails) => {
            clinicalDetails.forEach(details => {
              a.forEach(element => {  
                if(details.patientMRN === element.mrn){
                  this.patients.push(element);
                  console.log("I WORK")
                }
              })
            });

          },
          error: (err) => {
              console.error('Error fetching Clinical Details:', err);
          }
        });
      }
    }
  }

