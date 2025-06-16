import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminPatientControlPanelComponent } from './admin-patient-control.component';
import { AdminService } from '../admin.service';
import { LoginServiceService } from '../../login-service.service';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientData } from '../interfaces/patient-data';

// Mock services
class MockAdminService {
  patients$ = of([{ mrn: '123', firstName: 'John', lastName: 'Doe' } as PatientData]);
  selectedPatient$ = of(null);
  getAllPatients = jasmine.createSpy().and.returnValue(of({ status: 'success' }));
  getPatientById = jasmine.createSpy();
  editPatientAdmin = jasmine.createSpy().and.returnValue(of({ status: 'success' }));
  deletePatientProfile = jasmine.createSpy().and.returnValue(of({ status: 'success' }));
}

class MockLoginService {
  response$ = of({ Token: 'mock-token' });
}

describe('AdminPatientControlPanelComponent', () => {
  let component: AdminPatientControlPanelComponent;
  let fixture: ComponentFixture<AdminPatientControlPanelComponent>;
  let adminService: MockAdminService;
  let loginService: MockLoginService;

  beforeEach(() => {
    adminService = new MockAdminService();
    loginService = new MockLoginService();
  
    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, AdminPatientControlPanelComponent], // Move the component here
      providers: [
        { provide: AdminService, useValue: adminService },
        { provide: LoginServiceService, useValue: loginService },
      ],
    });
  
    fixture = TestBed.createComponent(AdminPatientControlPanelComponent);
    component = fixture.componentInstance;
  
    fixture.detectChanges();
  });
  

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize data and load patients on init when token exists', () => {
    spyOn(localStorage, 'getItem').and.returnValue('mock-token');
    spyOn(component, 'initializeData');

    component.ngOnInit();

    expect(component.response?.Token).toBe('mock-token');
    expect(component.initializeData).toHaveBeenCalled();
    expect(component.statusMessage).toBe('Patient Data Initialized');
  });

  it('should handle missing token by subscribing to loginService response$', () => {
    spyOn(localStorage, 'getItem').and.returnValue(null);

    component.ngOnInit();

    expect(component.response?.Token).toBe('mock-token');
    expect(component.statusMessage).toBe('Patient Data Initialized');
  });

  it('should refresh patients and update patient list', () => {
    component.refreshPatients();

    expect(adminService.getAllPatients).toHaveBeenCalledWith(component.queryData);
    expect(component.statusMessage).toBe('Patient List Refreshed.');
  });

  it('should fetch a patient by ID and update the selected patient', () => {
    component.fetchPatientById('123');

    expect(adminService.getPatientById).toHaveBeenCalledWith('123');
    adminService.selectedPatient$.subscribe((patient) => {
      expect(component.patient).toEqual(patient);
    });
  });

  it('should display an error when fetching a patient fails', () => {
    adminService.selectedPatient$ = throwError(() => new Error('Error fetching patient'));

    component.fetchPatientById('123');

    expect(component.statusMessage).toBe('Error fetching patient by ID.');
  });

  it('should edit a patient and update the status message', () => {
    component.response = { 
      Token: 'mock-token',
      Result: 'success', 
      Type: 'ADMIN_AUTH_TOKEN'
    };
    component.patient = { mrn: '123', firstName: 'John', lastName: 'Doe' } as PatientData;

    component.editPatient();

    expect(adminService.editPatientAdmin).toHaveBeenCalled();
    expect(component.statusMessage).toBe('Patient Edited Successfully.');
  });

  it('should delete a patient and refresh the patient list', () => {
    component.response = { 
      Token: 'mock-token',
      Result: 'success', 
      Type: 'ADMIN_AUTH_TOKEN'
    };
    component.patient = { mrn: '123', firstName: 'John', lastName: 'Doe' } as PatientData;

    component.deleteSelectedPatient();

    expect(adminService.deletePatientProfile).toHaveBeenCalledWith('123', 'mock-token');
    expect(component.statusMessage).toBe('Patient has been successfully deleted.');
    expect(component.patient).toBeNull();
  });

  it('should display an error when delete operation fails', () => {
    adminService.deletePatientProfile = jasmine.createSpy().and.returnValue(
      throwError(() => new Error('Error deleting patient'))
    );

    component.response = { 
      Token: 'mock-token',
      Result: 'success', 
      Type: 'ADMIN_AUTH_TOKEN'
    };
    component.patient = { mrn: '123', firstName: 'John', lastName: 'Doe' } as PatientData;

    component.deleteSelectedPatient();

    expect(component.statusMessage).toBe('Error deleting patient.');
  });
});
