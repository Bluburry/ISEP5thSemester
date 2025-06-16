import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { LoginResponse } from '../../login-result';
import { OperationPhase, OperationTypeData, RequiredSpecialist } from '../interfaces/operation-type-data';
import { OperationTypeQueryData, OperationTypeResultData } from '../interfaces/operation-type-query-data';
import { SpecializationData } from '../interfaces/specialization-data';

@Component({
	selector: 'app-operation-type-control',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './admin-operation-type-control.component.html',
	styleUrl: './admin-operation-type-control.component.css'
})

export class OperationTypeControlComponent implements OnInit {
	response: LoginResponse | null = null;
	storedToken = localStorage.getItem('authToken');

	specializations: string[] = [];

	operationPhases: OperationPhase[] = [
		{ phaseName: 'Preparation', duration: 0 },
		{ phaseName: 'Surgery', duration: 0 },
		{ phaseName: 'Cleaning', duration: 0 },
	];

	requiredSpecialists: RequiredSpecialist[] = [];
	requiredSpecialistHelper: RequiredSpecialist[] = [
		{ specialization: '', count: 0, phase: 'Preparation' },
		{ specialization: '', count: 0, phase: 'Surgery' },
		{ specialization: '', count: 0, phase: 'Cleaning' },
	];

	operationType: OperationTypeData = { name: '', phases: [], specialists: [], estimatedDuration: 0 };

	// for getting filtered list of operation types
	operationTypeResults: OperationTypeResultData[] = [];
	// for saving any singular returner operation type (patch, delete)
	operationTypeResult: OperationTypeResultData | null = null;
	operationTypeQuery: OperationTypeQueryData = { name: '', specialization: '', status: '' };

	requireSpecialistChangeHelper: string = '';

	constructor(
		private _service: AdminService,
		private _router: Router
	) { }

	ngOnInit(): void {
		if (this.storedToken) {
			this._service.validate(this.storedToken).subscribe(response => {
				if (response.role != "ADMIN") {
					console.log("not an admin");
					this._router.navigate(['']);
				}
			})
			this.response = { Token: this.storedToken } as LoginResponse;
			this.initializeData(this.storedToken);
		} else {
			this._router.navigate(['']);
		}
	}

	initializeData(token: string) {
		this._service.getSpecializations(token);
		this._service.specializationResults$.subscribe((specialization: SpecializationData[]) => {
			for (let sp of specialization)
				this.specializations.push(sp.SpecializationName);
			// console.log("Specializations: ", this.specializations);
		});
		this._service.getOperationTypeFiltered(token, "", "", "");
		this._service.operationTypeResults$.subscribe((opTypes: OperationTypeResultData[]) => {
			this.operationTypeResults = opTypes;
			console.log("Operations returned: ", this.operationType);
		});

	}

	gotOperation(id: string): void {
		this.operationTypeResult = null;
		this.requireSpecialistChangeHelper = "";
		for (let ot of this.operationTypeResults) {
			if (ot.ID === id) {
				this.operationTypeResult = ot;
				break;
			}
		}
		if (this.operationTypeResult) {
			this.operationTypeResult.RequiredSpecialists.forEach((rs: string) => {
				this.requireSpecialistChangeHelper += rs + "\n";
			});
		}
		// console.log(this.requireSpecialistChangeHelper);
		/* this.operationTypeResults.forEach((ot: OperationTypeResultData) => {
			if (ot.ID == id)
			{
				this.operationTypeResult = ot;
			}
		}); */
	}

	findOperationType(): void {
		if (this.storedToken) {
			console.log(this.operationTypeQuery);
			this._service.getOperationTypeFiltered(this.storedToken,
				this.operationTypeQuery.name, this.operationTypeQuery.specialization,
				this.operationTypeQuery.status);
			this._service.operationTypeResults$.subscribe((opTypes: OperationTypeResultData[]) => {
				this.operationTypeResults = opTypes;
				//console.log("Operations returned: ", this.operationType);
			});
		}
		this.operationTypeResult = null;
		this.requireSpecialistChangeHelper = "";
	}

	deactivateOperation(): void {
		console.log(this.storedToken);
		if (this.storedToken && this.operationTypeResult) {
			this._service.deleteOperationType(this.storedToken, this.operationTypeResult.OperationName);
			/* this._service.operationTypeResult$.subscribe((ot: OperationTypeResultData | null) => {
				this.operationTypeResult = ot;
				if (this.operationTypeResult == null)
					console.log("error retrieving deactivated operation");
				else
					console.log("deactivated operation type", this.operationTypeResult);
			}); */
		}
		this.requireSpecialistChangeHelper = "";
		this.operationTypeResult = null;
		// this.findOperationType();
	}

	patchOperation(): void {
		if (!this.storedToken || this.operationTypeResult == null)
			return;

		let specialist: string[] = [];
		let count: string[] = [];
		let phases: string[] = [];

		try {
			let helper = this.requireSpecialistChangeHelper.split("\n");
			// console.log(helper);

			helper.forEach((h: string) => {
				if (h != "") {
					specialist.push(h.split(':')[1].split(',')[0].trim());
					count.push(h.split(':')[2].split(',')[0].trim());
					phases.push(h.split(':')[3].split('.')[0].trim());
					// console.log(h);
				}
			});
			/* console.log(specialist);
			console.log(count);
			console.log(phases); */
			let phaseNames: string[] = [], phaseDurations: string[] = [];
			this.operationPhases.forEach(op => {
				phaseNames.push(op.phaseName);
				phaseDurations.push(op.duration.toString());
			});
			this._service.patchOperationType(this.storedToken,
				this.operationTypeResult.OperationName, this.operationTypeResult.EstimatedDuration,
				this.operationTypeResult.PhaseNames, this.operationTypeResult.PhasesDuration,
				specialist, count, phases);
			specialist = [];
			count = [];
			phases = [];
			this.requireSpecialistChangeHelper = "";
			this.operationTypeResult = null;
		} catch (error) {
			console.log(error);
		}
		// this.findOperationType();

		// this.operationType.phases = this.operationPhases;
		// this.operationType.specialists = this.requiredSpecialists;

		// console.log(this.operationTypeResult);

		/* if (this.storedToken) {
			if (this.operationType == null)
				return;
			console.log("doing thing");
			console.log(this.operationType);
			let phaseNames: string[] = [], phaseDurations: string[] = [];
			this.operationPhases.forEach(op => {
				phaseNames.push(op.phaseName);
				phaseDurations.push(op.duration.toString());
			});
			let specialistNames: string[] = [], specialistsCount: string[] = [], specialistPhases: string[] = [];
			this.requiredSpecialists.forEach(rs => {
				specialistNames.push(rs.specialization);
				specialistsCount.push(rs.count.toString());
				specialistPhases.push(rs.phase);
			});
			this._service.patchOperationType(this.storedToken, this.operationType.name,
				this.operationType.estimatedDuration, phaseNames, phaseDurations,
				specialistNames, specialistsCount, specialistPhases
			);
		} */

		/* this.operationPhases = [
			{ phaseName: 'Preparation', duration: 0 },
			{ phaseName: 'Surgery', duration: 0 },
			{ phaseName: 'Cleaning', duration: 0 },
		];
		this.requiredSpecialists = [];
		this.requiredSpecialistHelper = [
			{ specialization: '', count: 0, phase: 'Preparation' },
			{ specialization: '', count: 0, phase: 'Surgery' },
			{ specialization: '', count: 0, phase: 'Cleaning' },
		];
		this.operationType = { name: '', phases: [], specialists: [], estimatedDuration: 0 }; */
	}

}
