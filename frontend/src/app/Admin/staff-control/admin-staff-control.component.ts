import { Component, OnInit } from '@angular/core';
import { LoginServiceService } from '../../login-service.service';
import { AdminService } from '../admin.service';
import { LoginResponse } from '../../login-result';
import { EditPatientDtoAdmin, PatientData } from '../interfaces/patient-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientQueryData } from '../interfaces/patient-query-data';

import { EditStaffDtoAdmin, StaffData } from '../interfaces/staff-data';
import { StaffQueryData } from '../interfaces/staff-query-data';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  templateUrl: './admin-staff-control.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class AdminStaffControlPanelComponent implements OnInit {
  response: LoginResponse | null = null; 
  staffRoster: StaffData[] = [];
  staff: StaffData | null = null;

  queryStaffData: StaffQueryData = {
    license: '',
    name: '',
    email: '',
    specialization: '',
    status: ''
  }
  
  constructor(
    private loginService: LoginServiceService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    // Check if the token is in localStorage
    const storedToken = localStorage.getItem('authToken');
    if (storedToken) {
      // Assign the stored token to response and trigger patient data loading
      this.response = { Token: storedToken } as LoginResponse;
      this.initializeData(storedToken);
    } else {
      // Subscribe to the loginService response$ if no token is in localStorage
      this.loginService.response$.subscribe((response: LoginResponse | null) => {
        if (response) {
          this.response = response;
          // Store the token in localStorage
          localStorage.setItem('authToken', response.Token);
          this.initializeData(response.Token);
        }
      });
    }
  }

  // Method to initialize or refresh data
  initializeData(token: string): void {
  
    this.adminService.getAllStaff(this.queryStaffData, token);
    this.adminService.staff$.subscribe((staff: StaffData[]) => {
      this.staffRoster = staff;
      console.log('Staff data:', this.staffRoster);
    });

    this.adminService.selectedStaff$.subscribe((staff: StaffData | null) => {
      this.staff = staff;
      console.log('Fetched Staff by ID:', this.staff);
    });
  
  }

  // Refresh staff data
  refreshStaff(): void {
    const storedToken = localStorage.getItem('authToken');
    
    if(storedToken){
      this.adminService.getAllStaff(this.queryStaffData, storedToken);
    }
  }

  // Fetch staff member by License Number
  fetchStaffById(license: string): void {
    console.log(license);
    if (license) {
      this.adminService.getStaffById(license);
      this.adminService.selectedStaff$.subscribe((staff: StaffData | null) => {
        this.staff = staff;
        console.log('Fetched staff by License:', this.staff);
      });
    } else {
      console.error('Staff license input is empty.');
    }
  }

  convertToEditStaffDto(staff: StaffData): EditStaffDtoAdmin {
    return {
      LicenseNumber: staff.LicenseNumber,
      firstName: staff.FirstName,
      lastName: staff.LastName,
      fullName: staff.Fullname,
      email: staff.Email,
      phone: staff.Phone,
      specialization: staff.Specialization,
      status: staff.Status,
      availabilitySlots: staff.AvailabilitySlots,
    }
  }

  editStaff(): void {
    if (this.staff && this.response) {
      const editStaffDto = this.convertToEditStaffDto(this.staff);
      this.adminService.editStaffAdmin(editStaffDto, this.response.Token).subscribe(
        (result) => console.log('Staff updated successfully:', result),
        (error) => console.error('Error updating staff:', error)
      );
    }
  }

  deactivateSelectedStaff(): void {
    if (this.staff && this.response) {
      this.adminService.deactivateStaffProfile(this.staff.LicenseNumber, this.response.Token).subscribe({
        next: () => {
          console.log('Staff deactivated successfully');
          this.refreshStaff();
        },
        error: (err) => console.error('Error deleting staff:', err)
      });
    } else {
      console.error('No staff selected or missing authorization token');
    }
  }
}
