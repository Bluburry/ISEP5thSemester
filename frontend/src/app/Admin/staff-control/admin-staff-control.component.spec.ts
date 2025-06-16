import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminStaffControlPanelComponent } from './admin-staff-control.component';
import { AdminService } from '../admin.service';
import { LoginServiceService } from '../../login-service.service';
import { of, BehaviorSubject, Observable, Subscription } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { EditStaffDtoAdmin, StaffData } from '../interfaces/staff-data';

class MockAdminService {
	private staffSubject = new BehaviorSubject<StaffData[]>([]);
	public staff$ = this.staffSubject.asObservable();

	getAllStaff(query: any, token: string): void {
		this.staffSubject.next([{
			LicenseNumber: '123',
			FirstName: 'John',
			LastName: 'Doe',
			Fullname: 'John Die',
			Email: 'john.doe@example.com',
			Phone: '1234567890',
			Specialization: 'Cardiology',
			Status: 'Active',
			AvailabilitySlots: []
		}]);
	}

	private selectedStaffSubject = new BehaviorSubject<StaffData | null>(null);
	public selectedStaff$ = this.selectedStaffSubject.asObservable();

	
	getStaffById(license: string): void {
		// Emit a mock staff member
		this.selectedStaffSubject.next({
			LicenseNumber: license,
			FirstName: 'John',
			LastName: 'Doe',
			Fullname: 'John Die',
			Email: 'john.doe@example.com',
			Phone: '1234567890',
			Specialization: 'Cardiology',
			Status: 'Active',
			AvailabilitySlots: []
		});
	}

	editStaffAdmin(editData: EditStaffDtoAdmin, token: string): Observable<any> {
		return of({ Observable });
	}
	deactivateStaffProfile(license: string, token: string): Observable<any>  {
		return of({ Observable });
	}
}

class MockLoginServiceService {
	response$ = of({ Token: 'mock-token' });
}

describe('AdminStaffControlPanelComponent', () => {
	let component: AdminStaffControlPanelComponent;
	let fixture: ComponentFixture<AdminStaffControlPanelComponent>;
	let adminService: MockAdminService;

	beforeEach(async () => {
		await TestBed.configureTestingModule({
			declarations: [],
			imports: [FormsModule, CommonModule, AdminStaffControlPanelComponent],
			providers: [
				{ provide: AdminService, useClass: MockAdminService },
				{ provide: LoginServiceService, useClass: MockLoginServiceService },
			],
		}).compileComponents();

		fixture = TestBed.createComponent(AdminStaffControlPanelComponent);
		component = fixture.componentInstance;
		adminService = TestBed.inject(AdminService) as any;
		fixture.detectChanges();
	});

	it('should create the component', () => {
		expect(component).toBeTruthy();
	});

	it('should initialize staff data on init', () => {
		spyOn(adminService, 'getAllStaff').and.callThrough();
		const storedToken = localStorage.getItem('authToken') || 'mock-token';
		component.ngOnInit();
		expect(adminService.getAllStaff).toHaveBeenCalledWith(component.queryStaffData, storedToken);
		adminService.staff$.subscribe((staff) => {
			expect(staff.length).toBeGreaterThan(0);
		});
	});

	it('should fetch staff by license', () => {
		spyOn(adminService, 'getStaffById').and.callThrough();
		component.fetchStaffById('123');
		expect(adminService.getStaffById).toHaveBeenCalledWith('123');
		adminService.selectedStaff$.subscribe((staff) => {
			expect(staff?.LicenseNumber).toBe('123');
		});
	});

	it('should refresh staff data', () => {
		spyOn(adminService, 'getAllStaff').and.callThrough();
		const storedToken = localStorage.getItem('authToken') || 'mock-token';
		component.refreshStaff();
		expect(adminService.getAllStaff).toHaveBeenCalledWith(component.queryStaffData, storedToken);
	});

	it('should edit selected staff', () => {
		component.response = { Token: 'mock-token', Result: '', Type: '' };
		component.staff = {
			LicenseNumber: '123',
			FirstName: 'John',
			LastName: 'Doe',
			Fullname: 'John Die',
			Email: 'john.doe@example.com',
			Phone: '1234567890',
			Specialization: 'Cardiology',
			Status: 'Active',
			AvailabilitySlots: []
		};

		spyOn(adminService, 'editStaffAdmin').and.returnValue(of({ success: true }));
		component.editStaff();
		expect(adminService.editStaffAdmin).toHaveBeenCalled();
	});

	it('should fetch staff data', () => {
		const mockToken = 'mock-token';
		const mockStaff = [{
			LicenseNumber: '123',
			Email: 'test@email.com',
			FirstName: 'John',
			Fullname: 'John Doe',
			LastName: 'Doe',
			Status: '',
			Phone: '',
			Specialization: 'Nurse',
			AvailabilitySlots: [],
		}];
		spyOn(adminService, 'getAllStaff');
		/* spyOn(adminService.staff$, 'subscribe').and.callFake((callback) => {
			callback(mockStaff);
		}); */
		spyOn(adminService.staff$, 'subscribe').and.callFake((callback: (value: StaffData[]) => void) => {
			callback(mockStaff);
			return {
				unsubscribe: () => { }
			} as Subscription;
		});

		component.initializeData(mockToken);

		expect(adminService.getAllStaff).toHaveBeenCalledWith(component.queryStaffData, mockToken);
		expect(component.staffRoster).toEqual(mockStaff);
	});

	it('should refresh staff data with token from localStorage', () => {
		const mockToken = 'mock-token';
		spyOn(localStorage, 'getItem').and.returnValue(mockToken);
		spyOn(adminService, 'getAllStaff');

		component.refreshStaff();

		expect(adminService.getAllStaff).toHaveBeenCalledWith(component.queryStaffData, mockToken);
	});

	it('should call editStaffAdmin and log success message on successful update', () => {
		component.staff = {
			LicenseNumber: '123',
			Email: 'test@email.com',
			FirstName: 'John',
			Fullname: 'John Doe',
			LastName: 'Doe',
			Status: '',
			Phone: '',
			Specialization: 'Nurse',
			AvailabilitySlots: [],
		};
		component.response = {
			Token: 'mock-token',
			Result: 'Success',
			Type: ''
		};
		spyOn(adminService, 'editStaffAdmin').and.returnValue(of({ success: true }));
		spyOn(console, 'log');

		component.editStaff();

		expect(adminService.editStaffAdmin).toHaveBeenCalled();
		expect(console.log).toHaveBeenCalledWith('Staff updated successfully:', { success: true });
	});

	it('should deactivate selected staff and refresh staff list', () => {
		const mockToken = 'mock-token';
		component.staff = {
			LicenseNumber: '123',
			Email: 'test@email.com',
			FirstName: 'John',
			Fullname: 'John Doe',
			LastName: 'Doe',
			Status: '',
			Phone: '',
			Specialization: 'Nurse',
			AvailabilitySlots: [],
		};
		component.response = {
			Token: 'mock-token',
			Result: 'Success',
			Type: ''
		};
		spyOn(adminService, 'deactivateStaffProfile').and.returnValue(of(true));
		spyOn(component, 'refreshStaff');

		component.deactivateSelectedStaff();

		expect(adminService.deactivateStaffProfile).toHaveBeenCalledWith('123', mockToken);
		expect(component.refreshStaff).toHaveBeenCalled();
	});

});
