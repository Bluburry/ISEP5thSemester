import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientProfileDeletePanelComponent } from './patient-profile-delete.component';
import { PatientService } from '../patient.service';
import { LoginServiceService } from '../../login-service.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

// Mock services
class MockPatientService {
  selectedPatient$ = of({
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'john.doe@example.com',
    phone: '123456789',
    dateOfBirth: '1990-01-01',
    gender: 'Male',
    emergencyContact: 'Jane Doe',
    mrn: '12345',
    appointmentHistory: [],
    userId: 'user-123',
  });

  validate = jasmine.createSpy().and.returnValue(of({ role: 'PATIENT' }));
  getPatientById = jasmine.createSpy();
  deletePatientProfile = jasmine.createSpy().and.returnValue(of({}));
  confirmPatientProfileDelete = jasmine.createSpy().and.returnValue(of({}));
}

class MockRouter {
  navigate = jasmine.createSpy();
}

describe('PatientProfileDeletePanelComponent', () => {
  let component: PatientProfileDeletePanelComponent;
  let fixture: ComponentFixture<PatientProfileDeletePanelComponent>;
  let patientService: MockPatientService;
  let loginService: jasmine.SpyObj<LoginServiceService>;
  let router: MockRouter;

  beforeEach(() => {
    // Mock the services
    patientService = new MockPatientService();
    loginService = jasmine.createSpyObj('LoginServiceService', ['response$']);
    loginService.response$ = of({ Token: 'mock-token', Type: 1 });
    router = new MockRouter();

    // Configure testing module
    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, PatientProfileDeletePanelComponent],
      providers: [
        { provide: PatientService, useValue: patientService },
        { provide: LoginServiceService, useValue: loginService },
        { provide: Router, useValue: router },
      ],
    });

    // Create component
    fixture = TestBed.createComponent(PatientProfileDeletePanelComponent);
    component = fixture.componentInstance;

    // Trigger change detection
    fixture.detectChanges();
  });

  it('should navigate to login if user role is not "PATIENT"', () => {
    patientService.validate.and.returnValue(of({ role: 'ADMIN' }));

    component.ngOnInit();

    expect(router.navigate).toHaveBeenCalledWith(['']);
  });

  it('should call deletePatientProfile and update UI on deleteSelfProfile', () => {
    const mockToken = 'mock-token';
    component.response = { Token: mockToken, Type: 1 } as any;

    component.deleteSelfProfile();

    expect(patientService.deletePatientProfile).toHaveBeenCalledWith(mockToken);
    expect(component.showConfirmationCode).toBeTrue();
    expect(component.showDeleteButton).toBeFalse();
    expect(component.message).toBe('Email Requesting Deletion Sent. Please input the sent code.');
  });


  it('should call confirmPatientProfileDelete and update UI on confirmDelete', () => {
    const mockCode = 'mock-code';
    component.confirmationCode = mockCode;

    component.confirmDelete();

    expect(patientService.confirmPatientProfileDelete).toHaveBeenCalledWith(mockCode);
    expect(component.message).toBe(
      'Deletion Request Accepted. The profile will be deleted within the GRPD parameters (within 30 days).'
    );
    expect(component.errorMessage).toBe('');
    expect(component.showConfirmationCode).toBeFalse();
    expect(component.showDeleteButton).toBeFalse();
  });

  
});
