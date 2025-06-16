import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { AdminService } from '../admin.service';
import { MedicalConditionData } from '../interfaces/medical-condition-data';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-medical-condition-controller',
  templateUrl: './medical-condition-controller.component.html',
  styleUrls: ['./medical-condition-controller.component.css'],
  imports: [CommonModule, FormsModule]
})
export class MedicalConditionControllerComponent implements OnInit {
  allergyForm: FormGroup;
  token: string = '';
  conditions: MedicalConditionData[] = [];
  selectedCondition: MedicalConditionData | null = null;
  createData : MedicalConditionData = { code: '', designation: '', description: '', symptoms: '' };
  queryData: MedicalConditionData = { code: '', designation: '', description: '', symptoms: ''};
  sucessMessage: string | null = null;
  errorMessage: string | null = null;
  statusMessage: string | null = null;

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService
  ) {
    this.allergyForm = this.fb.group({
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
    this.adminService.getMedicalConditions(this.token, dto).subscribe({
      next: (conditions: MedicalConditionData[]) => {
        this.conditions = conditions;
        this.statusMessage = "";
        //this.statusMessage = "Medical Conditions fetched successfully";
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error fetching medical conditions", err);
        this.statusMessage = `Error fetching medical conditions: ${err.message}`;
      }
    });
  }

  createMedicalCondition() {
    if (this.createData.description == ''){
      this.createData.description = "[NO-DESCRIPTION-PROVIDED]"
    }
    this.adminService.createMedicalCondition(this.token, this.createData).subscribe({
      next: (condition: any) => {
        this.sucessMessage = "Medical Condition created successfully";
        this.errorMessage="";
        this.resetData();
        this.refreshConditions();
      },
      error: (err: HttpErrorResponse) => {
        console.log("Error creating Medical Condition", err);
        let errorMessage : string;
        if (err.statusText=="Internal Server Error") {
          errorMessage = "All the spaces which aren't optional need to be filled."
        } else {
          errorMessage = err.error.message;
        }
        
        this.errorMessage = "Error creating Medical Condition: " + errorMessage;
        this.sucessMessage = "";
      }
    });
  }
  resetData() {
    this.queryData = { code: '', designation: '', description: '', symptoms:'' };
    this.createData = { code: '', designation: '', description: '', symptoms:'' };
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
