import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminService } from '../admin.service';
import { LoginServiceService } from '../../login-service.service';
import { of, Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { StaffCreateComponent } from './staff-create.component';

class MockLoginServiceService {
	response$ = of({ Token: 'mock-token' });
}

describe('StaffCreateComponent', () => {
	let component: StaffCreateComponent;
	let fixture: ComponentFixture<StaffCreateComponent>;
	let mockAdminService: jasmine.SpyObj<AdminService>;
	let mockRouter: jasmine.SpyObj<Router>;

	beforeEach(async () => {
		mockAdminService = jasmine.createSpyObj('AdminService', ['createStaff', 'validate']);
		mockRouter = jasmine.createSpyObj('Router', ['navigate']);

		await TestBed.configureTestingModule({
			imports: [FormsModule, CommonModule, HttpClientModule, StaffCreateComponent],
			providers: [
				{ provide: AdminService, useValue: mockAdminService },
				{ provide: LoginServiceService, useClass: MockLoginServiceService },
				{ provide: Router, useValue: mockRouter },
			],
		}).compileComponents();

		fixture = TestBed.createComponent(StaffCreateComponent);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	it('should call createStaff with correct data and set correct to true on success', () => {
		const token = 'validToken';
		spyOn(localStorage, 'getItem').and.returnValue(token); // Mock token

		component.storedToken = token;
		component.license = '123';
		component.firstname = 'John';
		component.lastname = 'Doe';
		component.email = 'john.doe@example.com';
		component.phone = '555-5555';
		component.specialization = 'Cardiology';

		const expectedDto = {
			LicenseNumber: '123',
			firstName: 'John',
			lastName: 'Doe',
			fullName: 'John Doe',
			email: 'john.doe@example.com',
			phone: '555-5555',
			specialization: 'Cardiology',
			status: '1',
			availabilitySlots: [],
		};

		component.createStaff();

		expect(mockAdminService.createStaff).toHaveBeenCalledWith(expectedDto, token);
		expect(component.correct).toBeTrue();
		expect(component.wrong).toBeFalse();
	});
});
