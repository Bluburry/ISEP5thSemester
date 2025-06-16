import { TestBed, ComponentFixture } from '@angular/core/testing';
import { DoctorOperationControlPanelComponent } from './doctor-operation-control.component';
import { DoctorService } from '../doctor.service';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OperationRequestData } from '../interfaces/operation-request-data';

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
			medicalConditions: 'None',
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


describe('DoctorOperationControlPanelComponent', () => {
	let component: DoctorOperationControlPanelComponent;
	let fixture: ComponentFixture<DoctorOperationControlPanelComponent>;
	let mockDoctorService: jasmine.SpyObj<DoctorService>;
	let mockRouter: jasmine.SpyObj<Router>;

	beforeEach(async () => {
		mockDoctorService = jasmine.createSpyObj('DoctorService', [
			'validate',
			'getOperationRequests',
			'getOperationById',
			'editOperationRequest',
			'deleteOperationRequest',
			'getOperationTypes'
		]);

		mockRouter = jasmine.createSpyObj('Router', ['navigate']);

		await TestBed.configureTestingModule({
			imports: [CommonModule, FormsModule, DoctorOperationControlPanelComponent],
			declarations: [],
			providers: [
				{ provide: DoctorService, useValue: mockDoctorService },
				{ provide: Router, useValue: mockRouter }
			]
		}).compileComponents();
	});

	beforeEach(() => {
		fixture = TestBed.createComponent(DoctorOperationControlPanelComponent);
		component = fixture.componentInstance;

		// Mock initial values
		mockDoctorService.validate.and.returnValue(of({ role: 'STAFF' }));
		mockDoctorService.opRequests$ = of([]);
		mockDoctorService.opTypes$ = of([]);

		fixture.detectChanges();
	});

	it('should create the component', () => {
		expect(component).toBeTruthy();
	});

	describe('ngOnInit', () => {
		it('should validate the user token and initialize data', () => {
			const token = 'mock-token';
			localStorage.setItem('authToken', token);

			mockDoctorService.validate.and.returnValue(of({ role: 'STAFF' }));
			spyOn(component, 'initializeData');

			component.ngOnInit();

			expect(mockDoctorService.validate).toHaveBeenCalledWith(token);
			expect(component.initializeData).toHaveBeenCalledWith(token);
		});

		/* it('should navigate to the home page if no token is stored', () => {
			localStorage.removeItem('authToken');

			component.ngOnInit();

			expect(mockRouter.navigate).toHaveBeenCalledWith(['']);
		}); */

		it('should navigate to the home page if the role is not "STAFF"', () => {
			localStorage.setItem('authToken', 'mock-token');
			mockDoctorService.validate.and.returnValue(of({ role: 'NON_STAFF' }));

			component.ngOnInit();

			expect(mockRouter.navigate).toHaveBeenCalledWith(['']);
		});
	});

	describe('initializeData', () => {
		it('should fetch operation requests and operation types', () => {
			const token = 'mock-token';
			const mockRequests : OperationRequestData[] = [{
				ID: '1',
				Doctor: 'Dr. A',
				Patient: 'P1',
				OperationType: 'Type1',
				OperationDeadline: '',
				OperationPriority: '',
				RequiredSpecialists: [],
				EstimatedTime: '3'
			}];
			const mockTypes = [{ ID: '1', OperationName: 'Type1', EstimatedDuration: '', OperationStartDate: '', OperationEndDate: '', VersionNumber: '', ActivationStatus: '', OperationPhases: [], PhaseNames: [], PhasesDuration: [], RequiredSpecialists: [], SpecialistNames: [], SpecialistsCount: [] }];

			mockDoctorService.opRequests$ = of(mockRequests);
			mockDoctorService.opTypes$ = of(mockTypes);

			component.initializeData(token);

			expect(mockDoctorService.getOperationRequests).toHaveBeenCalledWith('', '', '', '', token);
			expect(mockDoctorService.getOperationTypes).toHaveBeenCalledWith(token);

			mockDoctorService.opRequests$.subscribe(() => {
				expect(component.operationRequests).toEqual(mockRequests);
			});

			mockDoctorService.opTypes$.subscribe(() => {
				expect(component.operationTypes).toEqual(mockTypes);
			});
		});
	});

	describe('fetchOperationById', () => {
		it('should fetch operation by ID and update selectedRequest', () => {
			const token = 'mock-token';
			const requestId = '123';
			const mockRequest: OperationRequestData = {
				ID: '1',
				Doctor: 'Dr. A',
				Patient: 'P1',
				OperationType: 'Type1',
				OperationDeadline: '',
				OperationPriority: '',
				RequiredSpecialists: [],
				EstimatedTime: '3'
			};

			component.storedToken = token;
			mockDoctorService.selectedRequest$ = of(mockRequest);

			component.fetchOperationById(requestId);

			expect(mockDoctorService.getOperationById).toHaveBeenCalledWith(requestId, token);

			mockDoctorService.selectedRequest$.subscribe(() => {
				expect(component.selectedRequest).toEqual(mockRequest);
			});
		});
	});

	describe('editRequest', () => {
		it('should call the service to edit an operation request', () => {
			const token = 'mock-token';
			component.storedToken = token;
			component.selectedRequest = {
				ID: '123',
				Doctor: 'Dr. A',
				Patient: 'P1',
				OperationType: 'Type1',
				OperationDeadline: '2025-12-23',
				OperationPriority: 'High',
				RequiredSpecialists: [],
				EstimatedTime: '3'
			};

			component.editRequest();

			expect(mockDoctorService.editOperationRequest).toHaveBeenCalledWith('123', '2025-12-23', 'High', token);
		});

		it('should log and return if no selected request is available', () => {
			spyOn(console, 'log');
			component.storedToken = 'mock-token';
			component.selectedRequest = null;

			component.editRequest();

			expect(console.log).toHaveBeenCalledWith('selected request null');
			expect(mockDoctorService.editOperationRequest).not.toHaveBeenCalled();
		});
	});

	describe('deleteRequest', () => {
		it('should call the service to delete an operation request', () => {
			const token = 'mock-token';
			component.storedToken = token;
			component.selectedRequest = {
				ID: '123',
				Doctor: 'Dr. A',
				Patient: 'P1',
				OperationType: 'Type1',
				OperationDeadline: '2025-12-23',
				OperationPriority: '',
				RequiredSpecialists: [],
				EstimatedTime: '3'
			};

			component.deleteRequest();

			expect(mockDoctorService.deleteOperationRequest).toHaveBeenCalledWith('123', token);
		});

		it('should log and return if no selected request is available', () => {
			spyOn(console, 'log');
			component.storedToken = 'mock-token';
			component.selectedRequest = null;

			component.deleteRequest();

			expect(console.log).toHaveBeenCalledWith('selected request null');
			expect(mockDoctorService.deleteOperationRequest).not.toHaveBeenCalled();
		});
	});
});
