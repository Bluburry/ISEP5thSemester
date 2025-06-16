import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SpecializationControlComponent } from './specialization-control.component'; // Ensure this import
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { BehaviorSubject, of } from 'rxjs';
import { SpecializationData } from '../interfaces/specialization-data';

describe('SpecializationControlComponent', () => {
	let component: SpecializationControlComponent;
	let fixture: ComponentFixture<SpecializationControlComponent>;
	let adminServiceSpy: jasmine.SpyObj<AdminService>;
	let routerSpy: jasmine.SpyObj<Router>;

	beforeEach(async () => {
		const adminServiceMock = jasmine.createSpyObj('AdminService', [
			'validate',
			'getSpecializations',
			'deleteSpecialization',
			'editSpecialization',
		]);
		const routerMock = jasmine.createSpyObj('Router', ['navigate']);

		// Mock BehaviorSubject for specializationResults$
		const specializationResultsSubject = new BehaviorSubject<SpecializationData[]>([]);

		adminServiceMock.specializationResults$ = specializationResultsSubject.asObservable();

		await TestBed.configureTestingModule({
			imports: [SpecializationControlComponent], // Import the standalone component here
			providers: [
				{ provide: AdminService, useValue: adminServiceMock },
				{ provide: Router, useValue: routerMock },
			]
		}).compileComponents();

		fixture = TestBed.createComponent(SpecializationControlComponent);
		component = fixture.componentInstance;
		adminServiceSpy = TestBed.inject(AdminService) as jasmine.SpyObj<AdminService>;
		routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;

		// Simulating a stored token
		localStorage.setItem('authToken', 'mock-token');
		component.storedToken = 'mock-token';
	});

	it('should create the component', () => {
		expect(component).toBeTruthy();
	});

	it('should call validate and navigate if the user is not an admin', () => {
		const response = { role: 'USER' }; // Mock response for non-admin
		adminServiceSpy.validate.and.returnValue(of(response));

		component.ngOnInit();

		expect(routerSpy.navigate).toHaveBeenCalledWith(['']);
	});

	it('should filter specializations by name using partialSearch', () => {
		component.specializations = [
			{ SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart related' },
			{ SpecializationCode: '002', SpecializationName: 'Neurology', SpecializationDescription: 'Brain related' }
		];

		component.searchName = 'Cardiology';
		component.partialSearch('name', {} as Event);

		expect(component.spDisplay.length).toBe(1);
		expect(component.spDisplay[0].SpecializationName).toBe('Cardiology');
	});

	it('should filter specializations by code using findSpecialization', () => {
		component.specializations = [
			{ SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart related' },
			{ SpecializationCode: '002', SpecializationName: 'Neurology', SpecializationDescription: 'Brain related' }
		];

		component.searchCode = '001';
		component.findSpecialization();

		expect(component.spDisplay.length).toBe(1);
		expect(component.spDisplay[0].SpecializationCode).toBe('001');
	});

	it('should call deleteSpecialization and update specialization lists', () => {
		const mockSpecializations: SpecializationData[] = [
			{ SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart related' },
			{ SpecializationCode: '002', SpecializationName: 'Neurology', SpecializationDescription: 'Brain related' }
		];
		component.specializations = [...mockSpecializations];
		component.spPicked = { SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart related' };

		adminServiceSpy.deleteSpecialization.and.callThrough(); // Mock service method call

		component.deleteSpecialization();

		expect(adminServiceSpy.deleteSpecialization).toHaveBeenCalledWith('mock-token', '001');
		expect(component.specializations.length).toBe(2);
		expect(component.spPicked).toBeNull();
		expect(component.spDisplay.length).toBe(2);
	});

	it('should call editSpecialization and update the specialization', () => {
		const mockSpecializations: SpecializationData[] = [
			{ SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart related' },
			{ SpecializationCode: '002', SpecializationName: 'Neurology', SpecializationDescription: 'Brain related' }
		];
		component.specializations = [...mockSpecializations];
		component.spPicked = { SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart and blood vessels' };
		let spPicked = { SpecializationCode: '001', SpecializationName: 'Cardiology', SpecializationDescription: 'Heart and blood vessels' };

		adminServiceSpy.editSpecialization.and.callThrough(); // Mock service method call

		component.editSpecialization();

		expect(adminServiceSpy.editSpecialization).toHaveBeenCalledWith('mock-token', spPicked);
		expect(component.specializations.length).toBe(2);
		expect(component.spPicked).toBeNull();
		expect(component.spDisplay.length).toBe(2);
		expect(component.specializations[1].SpecializationDescription).toBe('Heart and blood vessels');
	});
});
