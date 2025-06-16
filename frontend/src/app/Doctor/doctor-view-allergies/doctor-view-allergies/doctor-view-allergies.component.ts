import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';


import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AllergyData } from '../../../Admin/interfaces/allergy-data';
import { DoctorService } from '../../../Doctor/doctor.service';

@Component({
  selector: 'app-doctor-view-allergies',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './doctor-view-allergies.component.html',
  styleUrl: './doctor-view-allergies.component.css'
})
export class DoctorViewAllergiesComponent {
  allergyForm: FormGroup;
  token: string = '';
  allergies: AllergyData[] = [];
  selectedAllergy: AllergyData | null = null;
  queryData: AllergyData = { id: '', name: '', description: '' };
  statusMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService
  ) {
    this.allergyForm = this.fb.group({
      Id: [''],
      Name: [''],
      Description: ['']
    });
  }

  ngOnInit() {
    this.token = localStorage.getItem('authToken') || this.token;
    this.refreshAllergies();
  }

  getAllergies() {
    const dto: AllergyData = this.queryData;
    this.doctorService.getAllergies(this.token, dto).subscribe({
      next: (allergies: AllergyData[]) => {
        this.allergies = allergies;
        this.statusMessage = "Allergies fetched successfully";
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching allergies", err);
        this.statusMessage = `Error fetching allergies: ${err.message}`;
      }
    });
  }


  resetQueryData() {
    this.queryData = { id: '', name: '', description: '' };
  }

  refreshAllergies() {
    this.getAllergies();
  }

  fetchAllergyById(id: string) {
    this.selectedAllergy = this.allergies.find(allergy => allergy.id === id) || null;
  }
}
