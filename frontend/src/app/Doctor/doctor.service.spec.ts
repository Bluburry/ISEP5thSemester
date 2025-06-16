import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { DoctorService } from './doctor.service';
import { HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/local.environments.prod';
import { ClinicalDetails, PatientData } from './interfaces/patient-data';
import { OperationRequestData } from './interfaces/operation-request-data';
import { MedicalConditionData } from './interfaces/medical-condition-data';
import { AllergyData } from '../Admin/interfaces/allergy-data';

describe('DoctorService', () => {
	let service: DoctorService;
	let httpMock: HttpTestingController;
  
	beforeEach(() => {
	  TestBed.configureTestingModule({
		imports: [HttpClientTestingModule],
		providers: [DoctorService],
	  });
  
	  service = TestBed.inject(DoctorService);
	  httpMock = TestBed.inject(HttpTestingController);
	});
  
	afterEach(() => {
	  httpMock.verify();
	});

	it('should send a POST request to create an operation', () => {
		const token = 'testToken';
		const mrn = '123';
		const operationType = 'TestType';
		const deadline = '2024-12-01';
		const priority = 'HIGH';

		service.createOperation(mrn, operationType, deadline, priority, token);

		const req = httpMock.expectOne(`${environment.apiUrl}OperationRequest?patient=${mrn}&type=${operationType}&deadline=${deadline}&priority=${priority}`);
		expect(req.request.method).toBe('POST');
		expect(req.request.headers.get('auth')).toBe(token);

		req.flush({}); // Mock response
	});


	it('should send a GET request to fetch patients and update the subject', () => {
		const mockPatients = [{
			mrn: '123',
			firstName: '',
			lastName: '',
			fullName: '',
			gender: '',
			dateOfBirth: '',
			email: '',
			phone: '',
			medicalConditions: '',
			emergencyContact: '',
			appointmentHistory: [],
			userId: '',
		}];

		service.getPatients();

		const req = httpMock.expectOne(`${environment.apiUrl}Patient`);
		expect(req.request.method).toBe('GET');

		req.flush(mockPatients); // Mock response
		service.patients$.subscribe((patients) => {
			expect(patients).toEqual(mockPatients);
		});
	});


	it('should send a GET request to fetch operation types and update the subject', () => {
		const token = 'testToken';
		const mockOpTypes = [{
			ID: '',
			OperationName: '',
			EstimatedDuration: '',
			OperationStartDate: '',
			OperationEndDate: '',
			VersionNumber: '',
			ActivationStatus: '',
			OperationPhases: [],
			PhaseNames: [],
			PhasesDuration: [],
			RequiredSpecialists: [],
			SpecialistNames: [],
			SpecialistsCount: [],
		}];

		service.getOperationTypes(token);

		const req = httpMock.expectOne(`${environment.apiUrl}OperationType/ListForOperation`);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);

		req.flush(mockOpTypes); // Mock response
		service.opTypes$.subscribe((opTypes) => {
			expect(opTypes).toEqual(mockOpTypes);
		});
	});

	/*describe('deleteOperationRequest', () => {
		it('should send a DELETE request with the correct URL and headers', () => {
		const mockId = 'test-id'; 
		const mockAuth = 'test-auth-token'; 

		service.deleteOperationRequest(mockId, mockAuth);

		const expectedUrl = `https://localhost:5001/api/OperationRequest/${mockId}`;

		const req = httpMock.expectOne(expectedUrl);

		expect(req.request.method).toBe('DELETE');
		expect(req.request.headers.get('auth')).toBe(mockAuth);

		req.flush({ success: true });
		});
	});*/

	it('should send a POST request to validate a token', () => {
		const token = 'testToken';
		const mockResponse = { role: 'Doctor' };

		service.validate(token).subscribe((response) => {
			expect(response).toEqual(mockResponse);
		});

		const req = httpMock.expectOne(`${environment.apiUrl}Tokens`);
		expect(req.request.method).toBe('POST');
		expect(req.request.headers.get('token')).toBe(token);

		req.flush(mockResponse); // Mock response
	});

	describe('editOperationRequest', () => {
	it('should send a PATCH request with the correct URL, headers, and data', () => {
		const mockId = '3666b44c-3481-424a-b6f3-e324fe21d5df';
		const mockDeadline = '2024-12-27';
		const mockPriority = 'HIGH';
		const mockAuth = 'test-auth-token';

		const mockResponse: OperationRequestData = {
		ID: '3666b44c-3481-424a-b6f3-e324fe21d5df',
		Doctor: 'Dr. Smith',
		Patient: 'John Doe',
		OperationType: 'Appendectomy',
		OperationDeadline: '2024-12-27',
		OperationPriority: 'HIGH',
		RequiredSpecialists: ['Surgeon', 'Anesthesiologist'],
		EstimatedTime: '3 hours'
		};

		service.editOperationRequest(mockId, mockDeadline, mockPriority, mockAuth);

		const expectedUrl = `https://localhost:5001/api/OperationRequest/${mockId}?deadline=${mockDeadline}&priority=${mockPriority}`;

		const req = httpMock.expectOne(expectedUrl);

		expect(req.request.method).toBe('PATCH');
		expect(req.request.headers.get('auth')).toBe(mockAuth);

		req.flush(mockResponse);
	});
	});
	
	it('should fetch medical conditions', () => {
        const token = 'testToken';
        const dto: MedicalConditionData = {
            code: '123',
            designation: 'Hypertension',
            description: 'High blood pressure',
            symptoms: 'Headache, dizziness',
        };

        service.getMedicalConditions(token, dto).subscribe((conditions) => {
            expect(conditions.length).toBe(1);
            expect(conditions[0].code).toBe('123');
        });

        const req = httpMock.expectOne(
            `${environment.pmdUrl}medical-conditions?code=${encodeURIComponent(dto.code)}&designation=${encodeURIComponent(dto.designation || '')}&description=${encodeURIComponent(dto.description || '')}&symptoms=${encodeURIComponent(dto.symptoms || '')}`
        );
        expect(req.request.method).toBe('GET');
        expect(req.request.headers.get('token')).toBe(token);
        req.flush([dto]);
    });

    it('should handle error when fetching medical conditions', () => {
        const token = 'testToken';
        const dto: MedicalConditionData = {
            code: '123',
            designation: 'Hypertension',
            description: 'High blood pressure',
            symptoms: 'Headache, dizziness',
        };

        service.getMedicalConditions(token, dto).subscribe(
            () => fail('should have failed with the 500 error'),
            (error) => {
                expect(error.status).toBe(500);
            }
        );

        const req = httpMock.expectOne(
            `${environment.pmdUrl}medical-conditions?code=${encodeURIComponent(dto.code)}&designation=${encodeURIComponent(dto.designation || '')}&description=${encodeURIComponent(dto.description || '')}&symptoms=${encodeURIComponent(dto.symptoms || '')}`
        );
        expect(req.request.method).toBe('GET');
        expect(req.request.headers.get('token')).toBe(token);
        req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
    });

	it('should fetch medical conditions', () => {
		const token = 'testToken';
		const dto: MedicalConditionData = {
		  code: '123',
		  designation: 'Hypertension',
		  description: 'High blood pressure',
		  symptoms: 'Headache, dizziness',
		};
	
		service.getMedicalConditions(token, dto).subscribe((conditions) => {
		  expect(conditions.length).toBe(1);
		  expect(conditions[0].code).toBe('123');
		});
	
		const req = httpMock.expectOne(
		  `${environment.pmdUrl}medical-conditions?code=${encodeURIComponent(dto.code)}&designation=${encodeURIComponent(dto.designation || '')}&description=${encodeURIComponent(dto.description || '')}&symptoms=${encodeURIComponent(dto.symptoms || '')}`
		);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);
		req.flush([dto]);
	  });
	
	  it('should handle error when fetching medical conditions', () => {
		const token = 'testToken';
		const dto: MedicalConditionData = {
		  code: '123',
		  designation: 'Hypertension',
		  description: 'High blood pressure',
		  symptoms: 'Headache, dizziness',
		};
	
		service.getMedicalConditions(token, dto).subscribe(
		  () => fail('should have failed with the 500 error'),
		  (error) => {
			expect(error.status).toBe(500);
		  }
		);
	
		const req = httpMock.expectOne(
		  `${environment.pmdUrl}medical-conditions?code=${encodeURIComponent(dto.code)}&designation=${encodeURIComponent(dto.designation || '')}&description=${encodeURIComponent(dto.description || '')}&symptoms=${encodeURIComponent(dto.symptoms || '')}`
		);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);
		req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
	  });
	
	  it('should fetch filtered clinical details', () => {
		const token = 'testToken';
		const allergy = 'allergy123';
		const condition = 'condition123';
		const mockDetails: ClinicalDetails[] = [
		  { patientMRN: '123456', allergies: [], medicalConditions: [] }
		];
	
		service.getFilteredDetails(token, allergy, condition).subscribe((details) => {
		  expect(details.length).toBe(1);
		  expect(details[0].patientMRN).toBe('123456');
		});
	
		const req = httpMock.expectOne(`${environment.pmdUrl}clinicalDetails/filter`);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);
		expect(req.request.headers.get('allergyID')).toBe(allergy);
		expect(req.request.headers.get('medicalConditionID')).toBe(condition);
		req.flush(mockDetails);
	  });
	
	  it('should handle error when fetching filtered clinical details', () => {
		const token = 'testToken';
		const allergy = 'allergy123';
		const condition = 'condition123';
	
		service.getFilteredDetails(token, allergy, condition).subscribe(
		  () => fail('should have failed with the 500 error'),
		  (error) => {
			expect(error.status).toBe(500);
		  }
		);
	
		const req = httpMock.expectOne(`${environment.pmdUrl}clinicalDetails/filter`);
		expect(req.request.method).toBe('GET');
		req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
	  });
	
	  it('should update clinical details', () => {
		const token = 'testToken';
		const updatedClinicalDetails: ClinicalDetails = {
		  patientMRN: '123456',
		  allergies: [],
		  medicalConditions: []
		};
	
		service.updateClinicalDetails(token, updatedClinicalDetails).subscribe((details) => {
		  expect(details.patientMRN).toBe('123456');
		});
	
		const req = httpMock.expectOne(`${environment.pmdUrl}clinicalDetails/save`);
		expect(req.request.method).toBe('POST');
		expect(req.request.headers.get('Content-Type')).toBe('application/json');
		expect(req.request.headers.get('token')).toBe(token);
		req.flush(updatedClinicalDetails);
	  });
	
	  it('should handle error when updating clinical details', () => {
		const token = 'testToken';
		const updatedClinicalDetails: ClinicalDetails = {
		  patientMRN: '123456',
		  allergies: [],
		  medicalConditions: []
		};
	
		service.updateClinicalDetails(token, updatedClinicalDetails).subscribe(
		  () => fail('should have failed with the 500 error'),
		  (error) => {
			expect(error.status).toBe(500);
		  }
		);
	
		const req = httpMock.expectOne(`${environment.pmdUrl}clinicalDetails/save`);
		expect(req.request.method).toBe('POST');
		req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
	  });

	  it('should send a GET request to fetch allergies with the correct parameters and headers', () => {
		const token = 'testToken';
		const dto: AllergyData = { id: '1', name: 'Peanut', description: 'Allergy to peanuts' };
		const mockAllergies: AllergyData[] = [
		  { id: '1', name: 'Peanut', description: 'Allergy to peanuts' },
		  { id: '2', name: 'Dust', description: 'Allergy to dust' },
		];
	  
		service.getAllergies(token, dto).subscribe((allergies) => {
		  expect(allergies).toEqual(mockAllergies);
		});
	  
		const req = httpMock.expectOne(
		  `${environment.pmdUrl}allergies?code=${dto.id}&name=${dto.name}&description=${dto.description}`
		);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);
	  
		req.flush(mockAllergies); // Mock response
	  });	  
	
	  it('should handle error when fetching allergies', () => {
		const token = 'testToken';
		const dto: AllergyData = { id: '1', name: 'Peanut', description: 'Allergy to peanuts' };
	  
		service.getAllergies(token, dto).subscribe(
		  () => fail('should have failed with the 500 error'),
		  (error) => {
			expect(error.status).toBe(500);
		  }
		);
	  
		const req = httpMock.expectOne(
		  `${environment.pmdUrl}allergies?code=${dto.id}&name=${dto.name}&description=${dto.description}`
		);
		expect(req.request.method).toBe('GET');
		expect(req.request.headers.get('token')).toBe(token);
		
		// Simulate an error response
		req.flush('Something went wrong', { status: 500, statusText: 'Server Error' });
	  });	  
});
