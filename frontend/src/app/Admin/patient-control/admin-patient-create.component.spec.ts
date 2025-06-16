import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminCreatePatientPanelComponent } from './admin-patient-create.component';
import { AdminService } from '../admin.service';
import { LoginServiceService } from '../../login-service.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientRegistrationDto } from '../interfaces/patient-data';

// Mock services
class MockAdminService {
  createPatientAdmin = jasmine.createSpy().and.returnValue(of({}));
}

class MockLoginService {
  response$ = of({ Token: 'mock-token' });
}

class MockRouter {
  navigate = jasmine.createSpy();
}

describe('AdminCreatePatientPanelComponent', () => {
  let component: AdminCreatePatientPanelComponent;
  let fixture: ComponentFixture<AdminCreatePatientPanelComponent>;
  let adminService: MockAdminService;
  let loginService: MockLoginService;
  let router: MockRouter;

  const mockPatient: PatientRegistrationDto = {
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'john.doe@example.com',
    phone: '123456789',
    dateOfBirth: '1990-01-01',
    gender: 'Male',
    emergencyContact: 'Jane Doe',
  };

  beforeEach(() => {
    adminService = new MockAdminService();
    loginService = new MockLoginService();
    router = new MockRouter();

    TestBed.configureTestingModule({
      imports: [CommonModule, FormsModule, AdminCreatePatientPanelComponent],
      providers: [
        { provide: AdminService, useValue: adminService },
        { provide: LoginServiceService, useValue: loginService },
        { provide: Router, useValue: router },
      ],
    });

    fixture = TestBed.createComponent(AdminCreatePatientPanelComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should initialize patient data on component initialization', () => {
    component.ngOnInit();

    component.response = { Token: 'mock-token', Result: 'SUCCESS', Type : '' };
    expect(component.patient).toEqual({
      firstName: '',
      lastName: '',
      fullName: '',
      email: '',
      phone: '',
      dateOfBirth: '',
      gender: '',
      emergencyContact: '',
    });
  });

  it('should set error message when patient or response is missing on createPatient', () => {
    component.patient = null;
    component.response = null;

    component.createPatient();

    expect(component.errorMessage).toBe('Missing patient or response data.');
    expect(component.sucessMessage).toBe('');
  });

  it('should set error message when a required field is missing on createPatient', () => {
    component.patient = { ...mockPatient, firstName: '' }; // firstName is missing
    component.response = { Token: 'mock-token', Result: 'SUCCESS', Type : '' };

    component.createPatient();

    expect(component.errorMessage).toBe('Missing information for: firstName');
    expect(component.sucessMessage).toBe('');
  });

  /* it('should call createPatientAdmin and show success message on successful creation', () => {
    component.patient = mockPatient;
    component.response = { Token: 'mock-token', Result: 'SUCCESS', Type : '' };

    component.createPatient();

    expect(adminService.createPatientAdmin).toHaveBeenCalledWith(mockPatient, 'mock-token');
    expect(component.sucessMessage).toBe('Patient Created Sucessfully!');
    expect(component.errorMessage).toBe('');
  }); */


});
