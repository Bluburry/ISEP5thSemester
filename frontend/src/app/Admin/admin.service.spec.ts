import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AdminService } from './admin.service';
import { PatientData, EditPatientDtoAdmin, PatientRegistrationDto } from './interfaces/patient-data';
import { StaffData, EditStaffDtoAdmin } from './interfaces/staff-data';
import { PatientQueryData } from './interfaces/patient-query-data'; import { HttpErrorResponse } from '@angular/common/http';
import { StaffQueryData } from './interfaces/staff-query-data';
import { OperationTypeData } from './interfaces/operation-type-data';
import { MedicalConditionData } from '../Doctor/interfaces/medical-condition-data';
import { SpecializationData } from './interfaces/specialization-data';

describe('AdminService', () => {
	let service: AdminService;
	let httpMock: HttpTestingController;

	beforeEach(() => {
		TestBed.configureTestingModule({
			imports: [HttpClientTestingModule], // Add this line
			providers: [AdminService],
		});
		service = TestBed.inject(AdminService);
		httpMock = TestBed.inject(HttpTestingController);
	});

	afterEach(() => {
		httpMock.verify(); // Ensure no unmatched HTTP calls
	});

	it('should be created', () => {
		expect(service).toBeTruthy();
	});

	describe('getAllPatients', () => {
		/* it('should fetch all patients and update', () => {
			const patientQuery: PatientQueryData = {
				name: 'John',
				email: '',
				phoneNumber: '',
				medicalRecordNumber: '',
				dateOfBirth: '',
				gender: ''
			};

			const mockPatients: PatientData[] = [
				{
					mrn: '12345',
					firstName: 'John',
					lastName: 'Doe',
					fullName: 'John Doe',
					gender: 'Male',
					dateOfBirth: '1990-01-01',
					email: 'john.doe@example.com',
					phone: '123-456-7890',
					emergencyContact: 'Jane Doe - 123-456-7891',
					appointmentHistory: [],
					userId: 'user-12345'
				}
			];

			// Call the service method
			service.getAllPatients(patientQuery).subscribe();

			// Intercept the HTTP request and provide a mock response
			const req = httpMock.expectOne((req) => {
				const urlMatch = req.url === 'https://localhost:5001/api/Patient/filter';
				const paramsMatch = req.params.get('name') === 'John' &&
					req.params.get('email') === '' &&
					req.params.get('phoneNumber') === '' &&
					req.params.get('medicalRecordNumber') === '' &&
					req.params.get('dateOfBirth') === '' &&
					req.params.get('gender') === '';
				return urlMatch && paramsMatch;
			});

			expect(req.request.method).toBe('GET');
			req.flush(mockPatients); // Provide the mock response data

			// Subscribe to the observable to check the data emitted by the BehaviorSubject
			service.patients$.subscribe((patients) => {
				expect(patients).toEqual(mockPatients); // Assert the response
			});

			// Verify that there are no outstanding requests
			httpMock.verify();
		}); */

		/* it('should log an error if fetching patients fails', () => {
			const patientQuery: PatientQueryData = {
				name: 'trash',
				email: 'trash',
				phoneNumber: 'trash',
				medicalRecordNumber: 'trash',
				dateOfBirth: 'trash',
				gender: 'trash'
			};

			// Spy on console.error to ensure it's called on error
			spyOn(console, 'error');

			// Call the method
			service.getAllPatients(patientQuery).subscribe({
				next: () => fail('Expected an error, but got a success response'),
				error: (error) => {
					expect(error).toBe('failure');
					expect(console.error).toHaveBeenCalledWith(
						'Error fetching filtered patients',
						jasmine.anything() // Match the actual error object
					);
				}
			});

			// Match the exact request
			const req = httpMock.expectOne((req) =>
				req.url === 'https://localhost:5001/api/Patient/filter' &&
				req.params.get('name') === 'trash' &&
				req.params.get('email') === 'trash' &&
				req.params.get('phoneNumber') === 'trash' &&
				req.params.get('medicalRecordNumber') === 'trash' &&
				req.params.get('dateOfBirth') === 'trash' &&
				req.params.get('gender') === 'trash'
			);

			// Simulate a server error
			req.flush('Error', { status: 500, statusText: 'Server Error' });
		}); */

	});

	describe('getPatientByID', () => {
		it('should fetch patient by ID and update the selected patient subject', () => {
			const mockPatient = {
				mrn: '12345',
				firstName: 'John',
				lastName: 'Doe',
				fullName: 'John Doe',
				gender: 'Male',
				dateOfBirth: '1990-01-01',
				email: 'john.doe@example.com',
				phone: '123-456-7890',
				medicalConditions: 'None',
				emergencyContact: 'Jane Doe - 123-456-7891',
				appointmentHistory: [],
				userId: 'user-12345'
			};

			service.getPatientById('12345');

			const req = httpMock.expectOne('https://localhost:5001/api/Patient/GetPatientById');
			expect(req.request.method).toBe('GET');
			req.flush(mockPatient);

			service.selectedPatient$.subscribe((patient) => {
				expect(patient).toEqual(mockPatient);
			});
		});

		it('should log an error if fetching patient by ID fails', () => {
			spyOn(console, 'error');

			service.getPatientById('invalid-id');

			const req = httpMock.expectOne('https://localhost:5001/api/Patient/id?id=invalid-id');
			expect(req.request.method).toBe('GET');
			req.flush('Error', { status: 404, statusText: 'Not Found' });

			expect(console.error).toHaveBeenCalledWith(
				'Error fetching patient with MRN invalid-id',
				jasmine.anything()
			);
		});
	})


	describe('editPatientAdmin', () => {
		it('should send a POST request with the correct payload and headers', () => {
			const mockEditData: EditPatientDtoAdmin = {
				patientId: '123',                // Assuming 'patientId' corresponds to 'id'
				firstName: 'John',
				lastName: 'Doe',
				fullName: 'John Doe',
				email: 'john.doe@example.com',
				phone: '123-456-7890',
				medicalHistory: {                // Optional field
					// You can add mock data for AppointmentHistory if you want to test this field
					id: "",
					dateAndTime: "24-01-21",
					appointmentStatus: "SCHEDULED",
					reason: "patient has cancer",
					diagnosis: "cancer",
					notes: "they have cancer",
					staffId: "192",
					patientNumber: "MRN712"
				},
				dateOfBirth: '1990-01-01'        // Required field
			};
			const mockResponse = { success: true };

			service.editPatientAdmin(mockEditData, 'mock-token').subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			const req = httpMock.expectOne('https://localhost:5001/api/Patient/editPatient_Admin');
			expect(req.request.method).toBe('POST');
			expect(req.request.body).toEqual(mockEditData);
			expect(req.request.headers.get('token')).toBe('mock-token');
			req.flush(mockResponse);
		});
	});

	describe('createPatientAdmin', () => {
		it('should send a POST request with the correct payload and headers', () => {
			const mockToken = 'mock-token';
			const mockRequestBody: PatientRegistrationDto = {
				firstName: 'John',
				lastName: 'Doe',
				fullName: 'John Doe',
				gender: 'Male',
				dateOfBirth: '1990-01-01',
				email: 'john.doe@example.com',
				phone: '+123456789',
				emergencyContact: JSON.stringify({
					name: 'Jane Doe',
					phoneNumber: '+987654321',
					relation: 'Spouse',
				}), // Convert to a string
			};
			const mockResponse = { success: true };

			// Perform the request
			service.createPatientAdmin(mockRequestBody, mockToken).subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			// Mock the HTTP request and check its structure
			const req = httpMock.expectOne('https://localhost:5001/api/Patient/CreatePatient');
			expect(req.request.method).toBe('POST');
			expect(req.request.body).toEqual(mockRequestBody);
			req.flush(mockResponse);  // Simulate a successful response
		});

		it('should log an error when the POST request fails', () => {
			const mockToken = 'mock-token';
			const mockRequestBody: PatientRegistrationDto = {
				firstName: 'John',
				lastName: 'Doe',
				fullName: 'John Doe',
				gender: 'Male',
				dateOfBirth: '1990-01-01',
				email: 'john.doe@example.com',
				phone: '+123456789',
				emergencyContact: JSON.stringify({
					name: 'Jane Doe',
					phoneNumber: '+123456789',
					relation: 'Spouse',
				}),
			};
			const mockError = { message: 'Error creating patient' };

			// Simulate the HttpErrorResponse
			const errorResponse = new HttpErrorResponse({
				error: mockError,
				status: 500,
				statusText: 'Server Error',
				url: 'https://localhost:5001/api/Patient/CreatePatient',
			});



			service.createPatientAdmin(mockRequestBody, mockToken).subscribe(
				() => fail('Expected an error, but none was thrown'),
				(error) => {
					expect(error).toEqual(errorResponse);  // Check against the full HttpErrorResponse object
				}
			);

			// Mock the HTTP service to throw an error
			const req = httpMock.expectOne('https://localhost:5001/api/Patient/CreatePatient');
			expect(req.request.method).toBe('POST');
			expect(req.request.body).toEqual(mockRequestBody);

			req.flush(mockError, { status: 500, statusText: 'Server Error' });
		});
	});



	describe('deletePatientProfile', () => {
		it('should send a DELETE request with the correct payload and headers', () => {
			const mrn = '12345';
			const mockResponse = { success: true };

			service.deletePatientProfile(mrn, 'mock-token').subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			const req = httpMock.expectOne('https://localhost:5001/api/Patient/DeletePatient');
			expect(req.request.method).toBe('DELETE');
			expect(req.request.body).toBe(JSON.stringify(mrn));
			expect(req.request.headers.get('token')).toBe('mock-token');
			req.flush(mockResponse);
		});
	});

	describe('createStaff', () => {
		it('should send a PUT request to create a staff member', () => {
			const mockBody: EditStaffDtoAdmin = {
				LicenseNumber: '',
				firstName: '',
				lastName: '',
				fullName: '',
				email: '',
				phone: '',
				specialization: '',
				status: '',
				availabilitySlots: []
			};
			const mockToken = 'testToken';

			service.createStaff(mockBody, mockToken);

			const req = httpMock.expectOne('https://localhost:5001/api/Staff/Create');
			expect(req.request.method).toBe('PUT');
			expect(req.request.body).toEqual(mockBody);
			expect(req.request.headers.get('auth')).toBe(mockToken);

			req.flush({}); // Mock response
		});

	});

	describe('getAllStaff', () => {
		it('should send a POST request to fetch staff with query parameters', () => {
			const mockQuery: StaffQueryData = {
				license: '',
				name: '',
				email: '',
				specialization: '',
				status: '',
			};
			const mockToken = 'testToken';

			service.getAllStaff(mockQuery, mockToken);
			const req = httpMock.expectOne((request) => {
				const urlMatches = request.url === 'https://localhost:5001/api/Staff/Filter';
				const methodMatches = request.method === 'POST';

				// Ensure all query parameters match
				const paramsMatch =
					request.params.get('license') === mockQuery.license &&
					request.params.get('name') === mockQuery.name &&
					request.params.get('email') === mockQuery.email &&
					request.params.get('specialization') === mockQuery.specialization &&
					request.params.get('status') === mockQuery.status;

				return urlMatches && methodMatches && paramsMatch;
			});

			// Additional assertions
			expect(req.request.headers.get('auth')).toBe(mockToken);
			expect(req.request.body).toBeNull();

			/* const req = httpMock.expectOne((request) =>
				request.url === 'https://localhost:5001/api/Staff/Filter' &&
				request.method === 'POST'
			);

			expect(req.request.headers.get('auth')).toBe(mockToken);
			expect(req.request.params.keys()).toEqual(Object.keys(mockQuery)); */

			req.flush([]); // Mock response
		});

	});

	describe('getStaffByID', () => {
		it('should send a GET request to fetch a staff member by ID', () => {
			const license = 'testLicense';
			const mockResponse: StaffData = {
				AvailabilitySlots: [],
				Email: '',
				FirstName: '',
				Fullname: '',
				LastName: '',
				LicenseNumber: '',
				Phone: '',
				Specialization: '',
				Status: ''
			};

			service.getStaffById(license);

			const req = httpMock.expectOne(`https://localhost:5001/api/Staff/id?id=${license}`);
			expect(req.request.method).toBe('GET');

			req.flush(mockResponse); // Mock response
		});

	});

	describe('deactivateStaffProfile', () => {
		it('should send a DELETE request to deactivate a staff profile', () => {
			const license = 'testLicense';
			const mockToken = 'testToken';
			const mockResponse = { success: true };

			service.deactivateStaffProfile(license, mockToken).subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			const req = httpMock.expectOne(`https://localhost:5001/api/Staff/Disable?license=${license}`);
			expect(req.request.method).toBe('DELETE');
			expect(req.request.headers.get('auth')).toBe(mockToken);
			expect(req.request.headers.get('Content-Type')).toBe('application/json');
			expect(req.request.body).toBe(JSON.stringify(license));

			req.flush(mockResponse); // Mock response
		});

	});

	describe('createOperationType', () => {
		it('should send a POST request to create an operation type', () => {
			const mockToken = 'test-token';
			const mockOperationName = 'Test Operation';
			const mockEstimatedDuration = 120;
			const mockPhases = [
				{ phaseName: 'Preparation', duration: 60 },
				{ phaseName: 'Execution', duration: 60 },
			];
			const mockSpecialists = [
				{ specialization: 'Surgeon', count: 2, phase: 'Execution' },
				{ specialization: 'Nurse', count: 1, phase: 'Preparation' },
			];

			const mockResponse: OperationTypeData = {
				name: mockOperationName,
				phases: mockPhases,
				specialists: mockSpecialists,
				estimatedDuration: mockEstimatedDuration,
			};

			service.createOperationType(
				mockToken,
				mockOperationName,
				mockEstimatedDuration,
				mockPhases.map(p => p.phaseName),
				mockPhases.map(p => p.duration.toString()),
				mockSpecialists.map(s => s.specialization),
				mockSpecialists.map(s => s.count.toString()),
				mockSpecialists.map(s => s.phase)
			);

			const req = httpMock.expectOne(
				`https://localhost:5001/api/OperationType/createOperation`
			);

			expect(req.request.method).toBe('POST');
			expect(req.request.headers.get('token')).toBe(mockToken);
			expect(req.request.body).toEqual({
				operationName: mockOperationName,
				estimatedDuration: mockEstimatedDuration,
				phaseNames: mockPhases.map(p => p.phaseName),
				phasesDuration: mockPhases.map(p => p.duration.toString()),
				specialistNames: mockSpecialists.map(s => s.specialization),
				specialistsCount: mockSpecialists.map(s => s.count.toString()),
				specialistPhases: mockSpecialists.map(s => s.phase),
			});

			req.flush(mockResponse);
		});
	});


	describe('patchOperationType', () => {
		it('should send a PATCH request with the correct payload and headers', () => {
			const mockToken = 'test-token';
			const operationName = 'Test';
			const estimatedDuration = '120';
			const phaseNames = ['Preparation', 'Surgery'];
			const phasesDuration = ['30', '90'];
			const specialistNames = ['Spec1', 'Spec2'];
			const specialistsCount = ['2', '1'];
			const specialistPhases = ['Preparation', 'Surgery'];

			const mockResponse = { success: true };

			service.patchOperationType(
				mockToken, operationName, estimatedDuration, phaseNames, phasesDuration,
				specialistNames, specialistsCount, specialistPhases
			);

			const req = httpMock.expectOne(`https://localhost:5001/api/OperationType/updateOperation/${operationName}`);
			expect(req.request.method).toBe('PATCH');
			expect(req.request.headers.get('token')).toBe(mockToken);

			expect(req.request.body).toEqual({
				operationName: operationName,
				estimatedDuration: estimatedDuration,
				phaseNames: phaseNames,
				phasesDuration: phasesDuration,
				specialistNames: specialistNames,
				specialistsCount: specialistsCount,
				specialistPhases: specialistPhases
			});

			req.flush(mockResponse);
		});
	});

	describe('deleteOperationType', () => {
		it('should send a DELETE request with the correct URL and headers', () => {
			const mockToken = 'test-token';
			const operationName = 'Test Operation';
			const mockResponse = { success: true }; // Mock response from the server

			// Call the deleteOperationType method
			service.deleteOperationType(mockToken, operationName);

			// Expect the DELETE request to be made to the correct URL
			const req = httpMock.expectOne(`https://localhost:5001/api/OperationType/deactivateOperation/${operationName}`);

			// Assert that the request method is DELETE
			expect(req.request.method).toBe('DELETE');

			// Assert that the correct token header is being sent
			expect(req.request.headers.get('token')).toBe(mockToken);

			// Simulate a successful response
			req.flush(mockResponse);
		});
	});

	describe('getOperationTypeFiltered', () => {
		it('should send a GET request with the correct URL and headers based on the parameters', () => {
			const mockToken = 'test-token';
			const operationName = 'Test Operation';
			const specialization = 'Specialist1';
			const status = 'Active';
			const mockResponse = [{ id: '1', operationName: 'Test Operation', specialization: 'Specialist1', status: 'Active' }]; // Mock response

			service.getOperationTypeFiltered(mockToken, operationName, specialization, status);

			const expectedUrl = `https://localhost:5001/api/OperationType/filterOperationType?operationName=${operationName}&specialization=${specialization}&activeStatus=${status}`;
			const req = httpMock.expectOne(expectedUrl);

			expect(req.request.method).toBe('GET');
			expect(req.request.headers.get('token')).toBe(mockToken);

			req.flush(mockResponse);
		});
	});
	describe('Medical Conditions', () => {
		it('should fetch medical conditions with the correct query parameters and headers', () => {
			const token = 'mock-token';
			const mockQuery: MedicalConditionData = {
				code: 'M123',
				designation: 'Flu',
				description: 'A viral infection',
				symptoms: 'Fever, cough, fatigue',
			};
			const mockResponse: MedicalConditionData[] = [
				{
					code: 'M123',
					designation: 'Flu',
					description: 'A viral infection',
					symptoms: 'Fever, cough, fatigue',
				},
			];

			service.getMedicalConditions(token, mockQuery).subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			const req = httpMock.expectOne((req) => {
				const urlMatch = req.url === 'http://localhost:4000/api/medical-conditions?code=M123&designation=Flu&description=A%20viral%20infection&symptoms=Fever%2C%20cough%2C%20fatigue';

				return urlMatch;
			});

			expect(req.request.method).toBe('GET');
			expect(req.request.headers.get('token')).toBe(token);
			req.flush(mockResponse);
		});


		it('should create a medical condition with the correct payload and headers', () => {
			const token = 'mock-token';
			const mockDto: MedicalConditionData = {
				code: 'M124',
				designation: 'Cold',
				description: 'Common cold virus',
				symptoms: 'Sneezing, runny nose',
			};
			const mockResponse: MedicalConditionData = {
				...mockDto,
			};

			service.createMedicalCondition(token, mockDto).subscribe((response) => {
				expect(response).toEqual(mockResponse);
			});

			const req = httpMock.expectOne('http://localhost:4000/api/medical-conditions');
			expect(req.request.body).toEqual(mockDto);
			expect(req.request.headers.get('token')).toBe(token);
			expect(req.request.headers.get('Content-Type')).toBe('application/json');
			req.flush(mockResponse);
		});

		describe('createSpecialization', () => {
			it('should send a POST request to create a specialization', () => {
				const mockToken = 'test-token';
				const spec: SpecializationData = {
					SpecializationCode: "1234",
					SpecializationName: "testName",
					SpecializationDescription: ""
				};

				service.createSpecialization(
					mockToken,
					spec
				);

				const req = httpMock.expectOne(
					`https://localhost:5001/api/Specialization/CreateSpecialization`
				);

				expect(req.request.method).toBe('POST');
				expect(req.request.headers.get('token')).toBe(mockToken);
				expect(req.request.body).toEqual({
					SpecializationCode: spec.SpecializationCode,
					SpecializationName: spec.SpecializationName,
					SpecializationDescription: spec.SpecializationDescription
				});

				req.flush(spec);
			});
		});


		describe('editSpecialization', () => {
			it('should send a PATCH request with the correct payload and headers', () => {
				const mockToken = 'test-token';
				const spec: SpecializationData = {
					SpecializationCode: "1234",
					SpecializationName: "testName",
					SpecializationDescription: ""
				};

				const mockResponse = { success: true };

				service.editSpecialization(
					mockToken, spec
				);

				const req = httpMock.expectOne(
					`https://localhost:5001/api/Specialization/EditSpecialization/${spec.SpecializationCode}`
				);

				expect(req.request.method).toBe('PATCH');
				expect(req.request.headers.get('token')).toBe(mockToken);
				expect(req.request.body).toEqual({
					SpecializationCode: spec.SpecializationCode,
					SpecializationName: spec.SpecializationName,
					SpecializationDescription: spec.SpecializationDescription
				});

				req.flush(mockResponse);
			});
		});

		describe('deleteSpecialization', () => {
			it('should send a DELETE request with the correct URL and headers', () => {
				const mockToken = 'test-token';
				const spec: SpecializationData = {
					SpecializationCode: "1234",
					SpecializationName: "testName",
					SpecializationDescription: ""
				};

				const mockResponse = { success: true }; // Mock response from the server

				// Call the deleteOperationType method
				service.deleteSpecialization(mockToken, spec.SpecializationCode);

				// Expect the DELETE request to be made to the correct URL
				const req = httpMock.expectOne(
					`https://localhost:5001/api/Specialization/DeleteSpecialization/${spec.SpecializationCode}`
				);

				// Assert that the request method is DELETE
				expect(req.request.method).toBe('DELETE');

				// Assert that the correct token header is being sent
				expect(req.request.headers.get('token')).toBe(mockToken);

				// Simulate a successful response
				req.flush(mockResponse);
			});
		});

		describe('getSpecializations', () => {
			it('should send a GET request with the correct URL and headers based on the parameters', () => {
				const mockToken = 'test-token';
				const spec: SpecializationData[] = [{
					SpecializationCode: "1234",
					SpecializationName: "testName",
					SpecializationDescription: ""
				}];

				service.getSpecializations(mockToken);


				// Expect the DELETE request to be made to the correct URL
				const req = httpMock.expectOne(
					`https://localhost:5001/api/Specialization/GetSpecializationList`
				);

				expect(req.request.method).toBe('GET');
				expect(req.request.headers.get('token')).toBe(mockToken);

				req.flush(spec);
			});
		});

	});



});
