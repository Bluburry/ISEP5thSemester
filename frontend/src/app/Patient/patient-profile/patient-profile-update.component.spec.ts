import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PatientProfileUpdatePanelComponent } from './patient-profile-update.component';
import { PatientService } from '../patient.service';
import { Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginServiceService } from '../../login-service.service';
import { PatientData } from '../interfaces/patient-data';

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

  editPatientProfile = jasmine.createSpy().and.returnValue(of({}));
  confirmPatientProfileUpdate = jasmine.createSpy().and.returnValue(of({}));
  validate = jasmine.createSpy().and.returnValue(of({ role: 'PATIENT' }));
  getPatientById = jasmine.createSpy();
}

class MockRouter {
  navigate = jasmine.createSpy();
}

describe('PatientProfileUpdatePanelComponent', () => {
  let component: PatientProfileUpdatePanelComponent;
  let fixture: ComponentFixture<PatientProfileUpdatePanelComponent>;
  let patientService: MockPatientService;
  let loginService: jasmine.SpyObj<LoginServiceService>;
  let router: MockRouter;

  const mockPatient: PatientData = {
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
  };

  beforeEach(() => {
    // Mock the services
    patientService = new MockPatientService();
    loginService = jasmine.createSpyObj('LoginServiceService', ['response$']);
    router = new MockRouter();
    loginService.response$ = of({ Result: "AUTH", Token: "fjkdsnndsfajk", Type: 1});

    // Configure testing module
    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, PatientProfileUpdatePanelComponent],
      providers: [
        { provide: PatientService, useValue: patientService },
        { provide: LoginServiceService, useValue: loginService },
        { provide: Router, useValue: router },
      ],
    });

    // Create component
    fixture = TestBed.createComponent(PatientProfileUpdatePanelComponent);
    component = fixture.componentInstance;

    // Trigger change detection
    fixture.detectChanges();
  });

  it('should initialize patient data on component initialization', () => {
    component.response = { Result: "AUTH", Token: "fjkdsnndsfajk", Type: '  '};
    localStorage.setItem('authToken', 'fjkdsnndsfajk');

    component.ngOnInit();

    expect(patientService.validate).toHaveBeenCalledWith('fjkdsnndsfajk');
    expect(patientService.getPatientById).toHaveBeenCalled();
    
  });

  it('should call editPatientProfile on editPatient', () => {
    component.patient = mockPatient;
    component.response = { Result: "AUTH", Token: "fjkdsnndsfajk", Type: ''};

    component.editPatient();

    expect(patientService.editPatientProfile).toHaveBeenCalled();
  });

  it('should handle confirmPatientProfileUpdate correctly', () => {
    component.response = { Result: "AUTH", Token: "fjkdsnndsfajk", Type: ''};
    const mockToken = 'mock-confirmation-code';
    component.confirmationCode = mockToken;

    component.confirmUpdate();

    expect(patientService.confirmPatientProfileUpdate).toHaveBeenCalledWith(mockToken);
    expect(component.sucessMessage).toBe('Patient Updated Successfully!');
    expect(component.errorMessage).toBe('');
  });

  it('should navigate to login if user role is not "PATIENT"', () => {
    component.response = { Result: "AUTH", Token: "fjkdsnndsfajk", Type: ''};
    patientService.validate.and.returnValue(of({ role: 'ADMIN' }));

    component.ngOnInit();

    expect(router.navigate).toHaveBeenCalledWith(['']);
  });
});
