import { Component, OnInit } from '@angular/core';
import { DoctorService } from '../doctor.service';
import { LoginResponse } from '../../login-result';
import { PatientData } from '../interfaces/patient-data';
import { OperationTypeData } from '../interfaces/operation-type-data';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { OperationRequestData } from '../interfaces/operation-request-data';


@Component({
	selector: 'app-admin-panel',
	standalone: true,
	templateUrl: './doctor-operation-control.component.html',
	imports: [CommonModule, FormsModule],
	styles: []
})
export class DoctorOperationControlPanelComponent implements OnInit {
	response: LoginResponse | null = null;
	operationRequests: OperationRequestData[] = [];
	selectedRequest: OperationRequestData | null = null;

	// Variables that will save what was inserted by our utmost excellency (doctor), to filter operation requests.
	patientID: string = '';

	operationTypes: OperationTypeData[] = [];
	selectedOperationType: string = '';

	operationPriority: string = '';
	operationStatus: string = '';


	storedToken = localStorage.getItem('authToken');

	constructor(
		private doctorService: DoctorService,
		private router: Router,
	) { }

	ngOnInit(): void {
		if (this.storedToken) {
			this.doctorService.validate(this.storedToken).subscribe(response => {
				if (response.role != "STAFF") {
					console.log("whar");
					this.router.navigate(['']);
				}
			})
			this.response = { Token: this.storedToken } as LoginResponse;
			this.initializeData(this.storedToken);
		} else {
			this.router.navigate(['']);
		}
	}

	initializeData(token: string): void {
		// We are INITIALIZING data, so by querying with no filters we'll get all of the requests :bleh:
		this.doctorService.getOperationRequests('', '', '', '', token);
		this.doctorService.opRequests$.subscribe((opRequest: OperationRequestData[]) => {
			this.operationRequests = opRequest;
			console.log("Operation Request data: ", this.operationRequests);
		});

		this.doctorService.getOperationTypes(token);
		this.doctorService.opTypes$.subscribe((opType: OperationTypeData[]) => {
			this.operationTypes = opType;
			console.log('Operation type data:', this.operationTypes);
		});
	}

	fetchOperationById(id: string) {
		if (this.storedToken && id) {
			this.doctorService.getOperationById(id, this.storedToken);
			this.doctorService.selectedRequest$.subscribe((opRequest: OperationRequestData | null) => {
				this.selectedRequest = opRequest;
				if (this.selectedRequest == null) {
					console.log("couldn't find operation request data");
					return;
				}
				this.selectedRequest.OperationDeadline = new Date(this.selectedRequest.OperationDeadline).toISOString().split('T')[0];
				console.log("Operation Request data: ", this.operationRequests);
			});
		}
	}

	searchForOperation() {
		console.log(this.patientID);
		console.log(this.selectedOperationType);
		console.log(this.operationPriority);
		console.log(this.operationStatus);
		if (this.storedToken) {
			console.log("bleh");
			this.doctorService.getOperationRequests(this.patientID, this.selectedOperationType, this.operationPriority, this.operationStatus, this.storedToken);
			this.doctorService.opRequests$.subscribe((opRequest: OperationRequestData[]) => {
				this.operationRequests = opRequest;
				console.log("Operation Request data: ", this.operationRequests);
			});
		}
	}

	editRequest(): void {
		if (this.storedToken) {
			console.log("doing thing");
			if (this.selectedRequest == null) {
				console.log("selected request null");
				return;
			}
			console.log(this.selectedRequest.OperationPriority);
			console.log(this.selectedRequest);
			this.doctorService.editOperationRequest(this.selectedRequest.ID, this.selectedRequest.OperationDeadline, this.selectedRequest.OperationPriority, this.storedToken);
		}
		// this.searchForOperation();
	}

	deleteRequest(): void {
		if (this.storedToken) {
			console.log("doing thing");
			if (this.selectedRequest == null) {
				console.log("selected request null");
				return;
			}
			console.log(this.selectedRequest);
			this.doctorService.deleteOperationRequest(this.selectedRequest.ID, this.storedToken);
		}
		// this.searchForOperation();
	}
}
