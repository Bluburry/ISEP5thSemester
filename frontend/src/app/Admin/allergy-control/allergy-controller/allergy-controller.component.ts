import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AdminService } from '../../admin.service';
import { AllergyData } from '../../interfaces/allergy-data';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-allergy-controller',
  templateUrl: './allergy-controller.component.html',
  styleUrls: ['./allergy-controller.component.css'],
  imports: [CommonModule, FormsModule]
})
export class AllergyControllerComponent implements OnInit {
  allergyForm: FormGroup;
  token: string = '';
  allergies: AllergyData[] = [];
  selectedAllergy: AllergyData | null = null;
  createData : AllergyData = { id: '', name: '', description: '' };
  queryData: AllergyData = { id: '', name: '', description: '' };
  statusMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
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
    this.adminService.getAllergies(this.token, dto).subscribe({
      next: (allergies: AllergyData[]) => {
        this.allergies = allergies;
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching allergies", err);
        this.statusMessage = `Error fetching allergies: ${err.message}`;
      }
    });
  }

  patchAllergies() {
    if (!this.selectedAllergy) return;
    const id: string = this.selectedAllergy.id;
    const dto: AllergyData = this.selectedAllergy;
    this.adminService.patchAllergies(this.token, id, dto).subscribe({
      next: (allergy: AllergyData) => {
        this.statusMessage = "Allergy updated successfully";
        this.refreshAllergies();
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error updating allergy", err);
        this.statusMessage = `Error updating allergy: ${err.message}`;
      }
    });
    this.selectedAllergy = null;
  }

  createAllergy() {
    const idPattern = /^[A-Z0-9]{4}\.[A-Z0-9]{1}$/;
    let idMessage = '';
  
    if (!this.createData.id || !idPattern.test(this.createData.id)) {
      idMessage = ' The code was generated automatically.';
    }
  
    this.adminService.createAllergy(this.token, this.createData).subscribe({
      next: (allergy: any) => {
        this.statusMessage = `Allergy created successfully.${idMessage}`;
        this.resetQueryData();
        this.refreshAllergies();
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error creating allergy", err);
        this.statusMessage = `Error creating allergy: ${err.message}`;
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
