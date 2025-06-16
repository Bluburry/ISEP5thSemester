import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { DoctorService } from '../doctor.service';
import { MedicalConditionData } from '../interfaces/medical-condition-data';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-medical-condition-search',
  templateUrl: './medical-condition-search.component.html',
  styleUrls: ['./medical-condition-search.component.css'],
  imports: [CommonModule, FormsModule]
})
export class MedicalConditionSearchComponent implements OnInit {
  medicalConditionForm: FormGroup;
  token: string = '';
  conditions: MedicalConditionData[] = [];
  selectedCondition: MedicalConditionData | null = null;
  queryData: MedicalConditionData = { code: '', designation: '', description: '', symptoms: ''};
  errorMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private doctorService: DoctorService
  ) {
    this.medicalConditionForm = this.fb.group({
      Code: [''],
      Designation: [''],
      Description: ['']
    });
  }

  ngOnInit() {
    this.token = localStorage.getItem('authToken') || this.token;
    this.refreshConditions();
  }

  getMedicalConditions() {
    const dto: MedicalConditionData = this.queryData;
    if (/[^0-9]/.test(dto.code)) {
      this.errorMessage = "Error fetching medical conditions: Code can only be numeric.";
    } else{
    this.doctorService.getMedicalConditions(this.token, dto).subscribe({
      next: (conditions: MedicalConditionData[]) => {
        this.conditions = conditions;
        this.errorMessage = "";
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching medical conditions", err);
        this.errorMessage = `Error fetching medical conditions: ${err.error.message}`;
      }
    });
    }
  }

  resetData() {
    this.queryData = { code: '', designation: '', description: '', symptoms:'' };
    this.selectedCondition = null;
  }

  refreshConditions() {
    this.getMedicalConditions();
  }

  fetchConditionById(code: string) {
    console.log("Showing Medical Condition with code: " + code);
    this.selectedCondition = this.conditions.find(condition => condition.code === code) || null;
  }
}
