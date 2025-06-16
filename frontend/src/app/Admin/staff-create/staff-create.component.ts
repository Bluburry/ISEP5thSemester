import { Component, OnInit } from '@angular/core';
import { LoginResponse } from '../../login-result';
import { PatientData } from '../interfaces/patient-data';
import { OperationTypeData } from '../interfaces/operation-type-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EditStaffDtoAdmin, StaffData } from '../interfaces/staff-data';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-admin-panel',
  standalone: true,
  templateUrl: './staff-create.component.html',
  imports: [CommonModule, FormsModule],
  styles: []
})
export class StaffCreateComponent implements OnInit {
  response: LoginResponse | null = null; 
  storedToken = localStorage.getItem('authToken');

  license: string | null = null;
  firstname: string | null = null;
  lastname: string | null = null;
  email: string | null = null; 
  phone: string | null = null;
  specialization: string | null = null;

  correct = false;
  wrong = false;

  constructor(
    private _service: AdminService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    if (this.storedToken) {
      this._service.validate(this.storedToken).subscribe(response => {
        if(response.role != "ADMIN"){
          console.log("whar");
          this.router.navigate(['']);
        }
      })
      this.response = { Token: this.storedToken } as LoginResponse;
    } else {
      this.router.navigate(['']);
    }
  }
  
  convertToEditStaffDto(license: string, firstname: string, lastname: string, email: string, phone: string, specialization: string): EditStaffDtoAdmin {
    return {
      LicenseNumber: license,
      firstName: firstname,
      lastName: lastname,
      fullName: firstname + " " + lastname,
      email: email,
      phone: phone,
      specialization: specialization,
      status: "1",
      availabilitySlots: []
    }
  }

  createStaff(): void {
    console.log(this.storedToken);
    if (this.storedToken && this.license && this.firstname && this.lastname && this.email && this.phone && this.specialization) {
      try{
        this._service.createStaff(this.convertToEditStaffDto(this.license, this.firstname, this.lastname, this.email, this.phone, this.specialization), this.storedToken)
        this.correct = true;
      }catch{
        this.wrong = true;
      }
    }
  }
}
