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
  templateUrl: './update-clinical-details.component.html',
  styleUrls: ['./update-clinical-details.component.css'],
  imports: [CommonModule, FormsModule]
})
export class ClinicalDetailsUpdateComponent implements OnInit {
  medicalConditionForm: FormGroup;
  token: string = '';
  //==================================
  patients: PatientData[] = [];
  conditions: MedicalConditionData[] = [];
  allergies: AllergyData[] = [];
  //==================================
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
  //==================================
  errorMessageAllergy: string | null = null;
  errorMessageCondition: string | null = null;
  errorMessagePatient: string | null = null;
  errorMessageClinical: string | null = null;
  //==================================
  addAllergyButton: boolean = false;
  removeAllergyButton: boolean = false;
  addMedicalConditionButton: boolean = false;
  removeMedicalConditionButton: boolean = false;
  updateClinicalDetailsButton: boolean = false;

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

  resetButtons(){
    this.addAllergyButton = false;
    this.removeAllergyButton = false;
    this.addMedicalConditionButton = false;
    this.removeMedicalConditionButton = false;
    this.updateClinicalDetailsButton = false;
  }

  resetAll(){
    this.resetCondition();
    this.resetAllergy();
    this.resetPatient();
    this.resetButtons();
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
      if (index !== -1) {
        this.removeMedicalConditionButton = true;
        this.addMedicalConditionButton = false;
      } else{
        this.removeMedicalConditionButton = false;
        this.addMedicalConditionButton = true;
      }
    }
  }

  fetchAllergyById(id: string) {
    console.log("Showing Allergy with ID: " + id);
    this.selectedAllergy = this.allergies.find(allergy => allergy.id === id) || null;
    if (this.selectedClinicalDetails){
      const index = this.selectedClinicalDetails.allergies.findIndex(
        allergy => allergy.id === this.selectedAllergy?.id
      );
      if (index !== -1) {
        this.removeAllergyButton = true;
        this.addAllergyButton = false;
      } else{
        this.removeAllergyButton = false;
        this.addAllergyButton = true;
      }
    }
  }

  fetchPatientById(id: string): void {
    const patient = this.patients.find(patient => patient.mrn === id) || null;
      this.errorMessagePatient = ``;
      this.selectedPatient = patient;
      if (this.selectedPatient){
        if (this.selectedPatient.mrn){
          this.doctorService.getClinicalDetails(this.token, this.selectedPatient.mrn).subscribe({
            next: (clinicalDetails) => {
                console.log('Clinical Details retrieved:', clinicalDetails);
                this.selectedClinicalDetails = clinicalDetails;
                this.originalPatientAllergies = [...this.selectedClinicalDetails.allergies];
                this.originalPatientConditions = [...this.selectedClinicalDetails.medicalConditions];
                
                this.updateClinicalDetailsButton = true;
                         
                if (this.selectedClinicalDetails){
                  let index = this.selectedClinicalDetails.allergies.findIndex(
                    allergy => allergy.id === this.selectedAllergy?.id
                  );
                  if (index !== -1) {
                    this.removeAllergyButton = true;
                    this.addAllergyButton = false;
                  } else{
                    this.removeAllergyButton = false;
                    this.addAllergyButton = true;
                  }
        
                  index = this.selectedClinicalDetails.medicalConditions.findIndex(
                    condition => condition.code === this.selectedCondition?.code
                  );
                  if (index !== -1) {
                    this.removeMedicalConditionButton = true;
                    this.addMedicalConditionButton = false;
                  } else{
                    this.removeMedicalConditionButton = false;
                    this.addMedicalConditionButton = true;
                  }
                }
            },
            error: (err) => {
                console.error('Error fetching Clinical Details:', err);
                this.errorMessagePatient = "Legacy Patient - This patient doesn't have ClinicalDetails Associated to him."
            }
          });
        }
      }
    //}
  }

  // ================= UPDATE =================

  addAllergy(){
    console.log("Adding Allergy:" + this.selectedAllergy);
    if (this.selectedClinicalDetails){
      if (this.selectedAllergy) {
        this.errorMessageAllergy = '';
        this.selectedClinicalDetails.allergies.push(this.selectedAllergy);
        console.log(this.selectedClinicalDetails.allergies);
        this.addAllergyButton = false;
        this.removeAllergyButton = true;
    } else {
        this.errorMessageAllergy = `Please select an Allergy to add.`; //This will never be activated, but it is required to bypass " || null" exceptions.
    }
    } else {
      this.errorMessageAllergy = `Please select a patient's Clinical Details.`;
    }
  }

  removeAllergy() {
    console.log("Removing Allergy:", this.selectedAllergy);
    if (this.selectedClinicalDetails) {
        if (this.selectedAllergy) {
            this.errorMessageAllergy = '';

            // Check if the allergy exists in the array
            const index = this.selectedClinicalDetails.allergies.findIndex(
                allergy => allergy.id === this.selectedAllergy?.id
            );

            if (index !== -1) {
                // Remove the allergy from the array
                this.selectedClinicalDetails.allergies.splice(index, 1);
                console.log('Allergy removed successfully.');
                this.addAllergyButton = true;
                this.removeAllergyButton = false;
            } else {
                console.error('The selected allergy is not in the list.');
            }
        } else {
            this.errorMessageAllergy = `Please select an Allergy to remove.`; // This will never activate, but ensures null safety.
        }
    } else {
        this.errorMessageAllergy = `Please select a patient's Clinical Details.`;
    }
  }

  addMedicalCondition(){
    console.log("Adding Medical Condition:" + this.selectedCondition);
    if (this.selectedClinicalDetails){
      if (this.selectedCondition) {
        this.errorMessageCondition = '';
        this.selectedClinicalDetails.medicalConditions.push(this.selectedCondition);
        this.addMedicalConditionButton = false;
        this.removeMedicalConditionButton = true;
    } else {
        this.errorMessageCondition = `Please select an Allergy to add.`; //This will never be activated, but it is required to bypass " || null" exceptions.
    }
    } else {
      this.errorMessageCondition = `Please select a patient's Clinical Details.`;
    }
  }

  removeMedicalCondition() {
    console.log("Removing Medical Condition:", this.selectedCondition);
    if (this.selectedClinicalDetails) {
        if (this.selectedCondition) {
            this.errorMessageCondition = '';

            // Check if the condition exists in the array
            const index = this.selectedClinicalDetails.medicalConditions.findIndex(
              condition => condition.code === this.selectedCondition?.code
            );

            if (index !== -1) {
                // Remove the condition from the array
                this.selectedClinicalDetails.medicalConditions.splice(index, 1);
                console.log('Medical Condition removed successfully.');
                this.addMedicalConditionButton = true;
                this.removeMedicalConditionButton = false;
            } else {
                console.error('The selected Medical Condition is not in the list.');
            }
        } else {
            this.errorMessageCondition = `Please select an Medical Condition to remove.`; // This will never activate, but ensures null safety.
        }
    } else {
        this.errorMessageCondition = `Please select a patient's Clinical Details.`;
    }
  }

  updateClinicalDetails(){
    if(this.selectedClinicalDetails){
      if (
        !this.doctorService.areArraysEqual(this.selectedClinicalDetails.allergies, this.originalPatientAllergies) || 
        !this.doctorService.areArraysEqual(this.selectedClinicalDetails.medicalConditions, this.originalPatientConditions)
      ){
        console.log("Updating Clinical Details:" + this.selectedClinicalDetails.medicalConditions);
        this.doctorService.updateClinicalDetails(this.token, this.selectedClinicalDetails).subscribe(
          (response) => {
            console.log('Clinical details updated:', response);
            this.selectedClinicalDetails = response; 
          },
          (error) => {
            console.error('Error updating clinical details:', error);
          }
        );
      } else {
        this.errorMessageClinical = "No changes were made to the Clinical Details.";
      }
    } else{
      this.errorMessageClinical = "Please select a Clinical Detail to update.";
    }

  }

}
