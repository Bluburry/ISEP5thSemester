import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { SpecializationCreationComponent } from './specialization-creation.component';
import { AdminService } from '../admin.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('SpecializationCreationComponent', () => {
	let component: SpecializationCreationComponent;
	let adminServiceSpy: jasmine.SpyObj<AdminService>;
	let routerSpy: jasmine.SpyObj<Router>;
	let fixture: ComponentFixture<SpecializationCreationComponent>;

	beforeEach(async () => {
		const adminServiceMock = jasmine.createSpyObj('AdminService', ['validate', 'createSpecialization']);
		const routerMock = jasmine.createSpyObj('Router', ['navigate']);

		await TestBed.configureTestingModule({
			imports: [HttpClientTestingModule, SpecializationCreationComponent], // Include standalone component in imports
			providers: [
				{ provide: AdminService, useValue: adminServiceMock },
				{ provide: Router, useValue: routerMock }
			]
		}).compileComponents();

		fixture = TestBed.createComponent(SpecializationCreationComponent);
		component = fixture.componentInstance;
		adminServiceSpy = TestBed.inject(AdminService) as jasmine.SpyObj<AdminService>;
		routerSpy = TestBed.inject(Router) as jasmine.SpyObj<Router>;
	});

	it('should create the component', () => {
		expect(component).toBeTruthy();
	});

	it('should create a specialization when inputs are valid', () => {
		component.storedToken = 'mockToken';
		component.specialization.SpecializationCode = '001';
		component.specialization.SpecializationName = 'Cardiology';
		component.specialization.SpecializationDescription = 'Heart specialization';

		component.createSpecialization();

		expect(adminServiceSpy.createSpecialization).toHaveBeenCalledWith('mockToken', {
			SpecializationCode: '001',
			SpecializationName: 'Cardiology',
			SpecializationDescription: 'Heart specialization'
		});

		expect(component.specialization.SpecializationCode).toBe('');
		expect(component.specialization.SpecializationName).toBe('');
		expect(component.specialization.SpecializationDescription).toBe('');
	});

	it('should not create a specialization if the name is empty', () => {
		component.storedToken = 'mockToken';
		component.specialization.SpecializationCode = '001';
		component.specialization.SpecializationName = ''; // Name is empty
		component.specialization.SpecializationDescription = 'Heart specialization';

		component.createSpecialization();

		expect(adminServiceSpy.createSpecialization).not.toHaveBeenCalled();
	});
});
