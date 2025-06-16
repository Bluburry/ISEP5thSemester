import { TestBed, ComponentFixture } from '@angular/core/testing';
import { DoctorRequestOperationComponent } from './doctor-request-operation.component';
import { DoctorService } from '../doctor.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

// Mock services
class MockDoctorService {
	validate = jasmine.createSpy('validate').and.returnValue(of({ role: 'STAFF' }));
	getPatients = jasmine.createSpy('getPatients');
	patients$ = of([
		{
			mrn: '123',
			firstName: 'John',
			lastName: 'Doe',
			fullName: 'John Doe',
			gender: 'Male',
			dateOfBirth: '1990-01-01',
			email: 'john.doe@example.com',
			phone: '123-456-7890',
			emergencyContact: 'Jane Doe',
			appointmentHistory: [],
			userId: 'user123'
		}
	]);
	getOperationTypes = jasmine.createSpy('getOperationTypes');
	opTypes$ = of([
		{
			ID: 'op1',
			OperationName: 'Surgery',
			EstimatedDuration: '120',
			OperationStartDate: '2025-12-01',
			OperationEndDate: 'not set',
			VersionNumber: '1',
			ActivationStatus: 'ACTIVATED',
			OperationPhases: [
				"Operation phase: PREPARATION, duration: 15",
				"Operation phase: SURGERY, duration: 90",
				"Operation phase: CLEANING, duration: 15"
			],
			PhaseNames: ['PREPARATION', 'SURGERY', 'CLEANING'],
			PhasesDuration: ['15', '90', '15'],
			RequiredSpecialists: [
				"Specialization: Doctor, count: 1, phase: SURGERY.",
				"Specialization: Cleaner, count: 1, phase: CLEANING.",
				"Specialization: Anaesthesist, count: 1, phase: PREPARATION."
			],
			SpecialistNames: [
				"Doctor",
				"Cleaner",
				"Anaesthesist"
			],
			"SpecialistsCount": [
				"1",
				"1",
				"1"
			]
		}
	]);
	createOperation = jasmine.createSpy('createOperation');
	getPatientById = jasmine.createSpy('getPatientById');
	selectedPatient$ = of(null);
}

class MockRouter {
	navigate = jasmine.createSpy('navigate');
}

describe('DoctorRequestOperationComponent', () => {
	let component: DoctorRequestOperationComponent;
	let fixture: ComponentFixture<DoctorRequestOperationComponent>;
	let doctorService: MockDoctorService;
	let router: MockRouter;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			imports: [FormsModule, CommonModule], // Ensure these modules are imported
			providers: [
				{ provide: DoctorService, useClass: MockDoctorService },
				{ provide: Router, useClass: MockRouter },
			],
		})
			.compileComponents();

		// Import the standalone component into imports
		fixture = TestBed.createComponent(DoctorRequestOperationComponent);
		component = fixture.componentInstance;
		doctorService = TestBed.inject(DoctorService) as unknown as MockDoctorService;
		router = TestBed.inject(Router) as unknown as MockRouter;

		spyOn(localStorage, 'getItem').and.callFake((key: string) => {
			if (key === 'authToken') return 'mock-token';
			return null;
		});
	});

	it('should create the component', () => {
		expect(component).toBeTruthy();
	});

	/* it('should redirect to the homepage if no stored token is found', () => {
		spyOn(localStorage, 'getItem').and.returnValue(null);
		component.ngOnInit();
		expect(router.navigate).toHaveBeenCalledWith(['']);
	}); */

	it('should call doctorService.validate on init if token is present', () => {
		component.ngOnInit();
		expect(doctorService.validate).toHaveBeenCalledWith('mock-token');
	});

	it('should redirect to the homepage if validate response role is not STAFF', () => {
		doctorService.validate.and.returnValue(of({ role: 'ADMIN' }));
		component.ngOnInit();
		expect(router.navigate).toHaveBeenCalledWith(['']);
	});

	it('should initialize patients and operation types data', () => {
		component.ngOnInit();

		expect(doctorService.getPatients).toHaveBeenCalled();
		expect(doctorService.getOperationTypes).toHaveBeenCalledWith('mock-token');

		expect(component.patients).toEqual([
			{
				mrn: '123',
				firstName: 'John',
				lastName: 'Doe',
				fullName: 'John Doe',
				gender: 'Male',
				dateOfBirth: '1990-01-01',
				email: 'john.doe@example.com',
				phone: '123-456-7890',
				emergencyContact: 'Jane Doe',
				appointmentHistory: [],
				userId: 'user123'
			}
		]);
		expect(component.operationTypes).toEqual([
			{
				ID: 'op1',
				OperationName: 'Surgery',
				EstimatedDuration: '120',
				OperationStartDate: '2025-12-01',
				OperationEndDate: 'not set',
				VersionNumber: '1',
				ActivationStatus: 'ACTIVATED',
				OperationPhases: [
					"Operation phase: PREPARATION, duration: 15",
					"Operation phase: SURGERY, duration: 90",
					"Operation phase: CLEANING, duration: 15"
				],
				PhaseNames: ['PREPARATION', 'SURGERY', 'CLEANING'],
				PhasesDuration: ['15', '90', '15'],
				RequiredSpecialists: [
					"Specialization: Doctor, count: 1, phase: SURGERY.",
					"Specialization: Cleaner, count: 1, phase: CLEANING.",
					"Specialization: Anaesthesist, count: 1, phase: PREPARATION."
				],
				SpecialistNames: [
					"Doctor",
					"Cleaner",
					"Anaesthesist"
				],
				"SpecialistsCount": [
					"1",
					"1",
					"1"
				],
			}
		]);
	});

	it('should call createOperation with the correct parameters', () => {
		component.selectedPatient = {
			mrn: '123',
			firstName: 'John',
			lastName: 'Doe',
			fullName: 'John Doe',
			gender: 'Male',
			dateOfBirth: '1990-01-01',
			email: 'john.doe@example.com',
			phone: '123-456-7890',
			emergencyContact: 'Jane Doe',
			appointmentHistory: [],
			userId: 'user123'
		};

		component.selectedOperationType = 'Surgery';
		component.operationDeadline = '2023-12-31';
		component.operationPriority = 'HIGH';
		component.storedToken = 'mock-token';

		component.createRequest();

		expect(doctorService.createOperation).toHaveBeenCalledWith(
			'123', // Patient MRN
			'Surgery', // Operation Type
			'2023-12-31', // Deadline
			'HIGH', // Priority
			'mock-token' // Token
		);
	});

	it('should not call createOperation if required fields are missing', () => {
		component.selectedPatient = null; // Simulate missing patient
		component.createRequest();
		expect(doctorService.createOperation).not.toHaveBeenCalled();
	});

	it('should fetch patient by ID', () => {
		component.fetchPatientById('123');
		expect(doctorService.getPatientById).toHaveBeenCalledWith('123');
		expect(component.selectedPatient).toBeNull();
	});

	it('should log an error if fetchPatientById is called with an empty ID', () => {
		spyOn(console, 'error');
		component.fetchPatientById('');
		expect(console.error).toHaveBeenCalledWith('Patient license input is empty.');
	});
});
