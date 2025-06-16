import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PatientService } from './patient.service';
import { EditPatientDtoPatient, PatientData } from './interfaces/patient-data';

describe('PatientService', () => {
  let service: PatientService;
  let httpMock: HttpTestingController;

  const mockPatient: PatientData = {
    mrn: '12345',
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    gender: 'Male',
    dateOfBirth: '1990-01-01',
    email: 'john.doe@example.com',
    phone: '123-456-7890',
    // medicalConditions: 'None',
    emergencyContact: 'Jane Doe - 123-456-7891',
    appointmentHistory: [],
    userId: 'user-12345'
  };

  const mockEditData: EditPatientDtoPatient = {
    firstName: 'John',
    lastName: 'Doe',
    fullName: 'John Doe',
    email: 'john.doe@example.com',
    phone: '123-456-7890',
    dateOfBirth: '1990-01-01',
    medicalHistory: undefined, // if you don't have data for this field
    // medicalConditions: 'None',
    emergencyContact: 'Jane Doe - 987-654-3210',
    gender: 'Male'
};


  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PatientService]
    });
    service = TestBed.inject(PatientService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify(); // Ensure no outstanding HTTP requests
  });

  describe('getPatientById', () => {
    it('should fetch the patient by token and update the selectedPatientSubject', () => {
      const token = 'mock-token';
      
      service.getPatientById(token);

      const req = httpMock.expectOne(`https://localhost:5001/api/Patient/GetPatientByToken`);
      expect(req.request.method).toBe('GET');
      req.flush(mockPatient);

      service.selectedPatient$.subscribe(patient => {
        expect(patient).toEqual(mockPatient);
      });
    });

    it('should log an error when fetching a patient fails', () => {
      const token = 'mock-token';
      spyOn(console, 'error');  // Spy on console.error

      service.getPatientById(token);

      const req = httpMock.expectOne(`https://localhost:5001/api/Patient/GetPatientByToken`);
      req.flush('Error', { status: 500, statusText: 'Server Error' });

      expect(console.error).toHaveBeenCalledWith('Error fetching the patient profile with the token: mock-token', jasmine.anything());
    });
  });

  describe('editPatientProfile', () => {
    it('should send a POST request with the correct payload and headers', () => {
      const token = 'mock-token';

      service.editPatientProfile(mockEditData, token).subscribe(response => {
        expect(response).toEqual({ success: true });
      });

      const req = httpMock.expectOne('https://localhost:5001/api/Patient/editPatient_Patient');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual(mockEditData);
      expect(req.request.headers.get('token')).toBe(token);
      req.flush({ success: true });
    });
  });

  describe('deletePatientProfile', () => {
    it('should send a DELETE request with the correct token in headers', () => {
      const token = 'mock-token';

      service.deletePatientProfile(token).subscribe(response => {
        expect(response).toEqual({ success: true });
      });

      const req = httpMock.expectOne('https://localhost:5001/api/Patient/DeleteSelfPatient');
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.get('token')).toBe(token);
      req.flush({ success: true });
    });
  });

  describe('confirmPatientProfileUpdate', () => {
    it('should send a POST request to confirm the update with the correct token in headers', () => {
      const token = 'mock-token';

      service.confirmPatientProfileDelete(token).subscribe(response => {
        expect(response).toEqual({ success: true });
      });

      const req = httpMock.expectOne('https://localhost:5001/api/Patient/ConfirmPatientDeletion');
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.get('token')).toBe(token);
      req.flush({ success: true });
    });
  });
});
