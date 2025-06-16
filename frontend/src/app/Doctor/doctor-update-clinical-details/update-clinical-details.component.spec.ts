import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ClinicalDetailsUpdateComponent } from './update-clinical-details.component';
import { DoctorService } from '../doctor.service';
import { Observable, of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MedicalConditionData } from '../interfaces/medical-condition-data';
import { AllergyData } from '../../Admin/interfaces/allergy-data';
import { ClinicalDetails, PatientData } from '../interfaces/patient-data';
import { HttpErrorResponse } from '@angular/common/http';

class MockDoctorService {
    getMedicalConditions(token: string, dto: MedicalConditionData): Observable<MedicalConditionData[]> {
        return of([
          { code: '001', designation: 'Condition 1', description: 'Description 1', symptoms: 'Symptom 1' } as MedicalConditionData,
          { code: '002', designation: 'Condition 2', description: 'Description 2', symptoms: 'Symptom 2' } as MedicalConditionData,
        ]);
      }

  getAllergies(token: string, query: AllergyData) {
    return of([{ id: '1', name: 'Allergy A', description: 'Mild' }]);
  }

  getQueryPatients(token: string, query: any) {
    return of([{ mrn: '123', firstName: 'John', lastName: 'Doe' }]);
  }

  getClinicalDetails(token: string, id: string): Observable<ClinicalDetails> {
    return of({
      patientMRN: '123456',
      allergies: [], 
      medicalConditions: [], 
    });
  }

  updateClinicalDetails(token: string, clinicalDetails: any) {
    return of(clinicalDetails);
  }

  areArraysEqual(arr1: any[], arr2: any[]) {
    return JSON.stringify(arr1) === JSON.stringify(arr2);
  }
}

describe('ClinicalDetailsUpdateComponent', () => {
  let component: ClinicalDetailsUpdateComponent;
  let fixture: ComponentFixture<ClinicalDetailsUpdateComponent>;
  let doctorService: MockDoctorService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [],
      imports: [FormsModule, CommonModule, ClinicalDetailsUpdateComponent],
      providers: [{ provide: DoctorService, useClass: MockDoctorService }],
    }).compileComponents();

    fixture = TestBed.createComponent(ClinicalDetailsUpdateComponent);
    component = fixture.componentInstance;
    doctorService = TestBed.inject(DoctorService);
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should load patients on initialization', () => {
    spyOn(doctorService, 'getQueryPatients').and.callThrough();
    component.ngOnInit();
    expect(doctorService.getQueryPatients).toHaveBeenCalled();
    expect(component.patients.length).toBeGreaterThan(0);
  });

  it('should select a patient and fetch clinical details', () => {
    spyOn(doctorService, 'getClinicalDetails').and.callThrough();
    component.fetchPatientById('123');
    expect(doctorService.getClinicalDetails).toHaveBeenCalledWith(component.token, '123');
    expect(component.selectedPatient).toBeDefined();
    expect(component.selectedClinicalDetails).toBeDefined();
  });

  it('should add a medical condition to clinical details', () => {
    component.selectedClinicalDetails = {patientMRN: '123456', allergies: [], medicalConditions: [] };
    component.selectedCondition = { code: 'A123', designation: 'Condition A', description: 'Description', symptoms: 'None' };
    component.addMedicalCondition();
    expect(component.selectedClinicalDetails?.medicalConditions.length).toBe(1);
  });

  it('should remove a medical condition from clinical details', () => {
    component.selectedClinicalDetails = {patientMRN: '123456', allergies: [], medicalConditions: [{ code: 'A123', designation: 'Condition A', description: 'Description', symptoms: 'None' }] };
    component.selectedCondition = { code: 'A123', designation: 'Condition A', description: 'Description', symptoms: 'None' };
    component.removeMedicalCondition();
    expect(component.selectedClinicalDetails?.medicalConditions.length).toBe(0);
  });


  it('should update clinical details if changes were made', () => {
    component.selectedClinicalDetails = {patientMRN: '123456',  allergies: [{ id: '1', name: 'Allergy A', description: 'Mild' }], medicalConditions: [{ code: 'A123', designation: 'Condition A', description: 'Description', symptoms: 'None' }] };
    component.originalPatientAllergies = [{ id: '4A8Z.3', name: 'Allergy A', description: 'Mild' }];
    component.originalPatientConditions = [{ code: '12356789', designation: 'Condition A', description: 'Description', symptoms: 'None' }];
    spyOn(doctorService, 'updateClinicalDetails').and.callThrough();
    component.updateClinicalDetails();
    expect(doctorService.updateClinicalDetails).toHaveBeenCalled();
  });

  it('should show error message if no changes were made while updating clinical details', () => {
    component.selectedClinicalDetails = {patientMRN: '123456',  allergies: [], medicalConditions: [] };
    component.originalPatientAllergies = [];
    component.originalPatientConditions = [];
    component.updateClinicalDetails();
    expect(component.errorMessageClinical).toBe('No changes were made to the Clinical Details.');
  });
});
