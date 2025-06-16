import { Component, OnInit } from '@angular/core';
import { LoginResponse } from '../../login-result';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { OperationPhase, OperationTypeData, RequiredSpecialist } from '../interfaces/operation-type-data';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpecializationData } from '../interfaces/specialization-data';

@Component({
	selector: 'app-operation-type-creation',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './admin-operation-type-creation.component.html',
	styleUrl: './admin-operation-type-creation.component.css'
})

export class OperationTypeCreationComponent implements OnInit {
	response: LoginResponse | null = null;

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

	phasePlaceholder: string[] = [
		'Preparation', '0', 'Required Specialists:',
		'Surgery', '0', 'Required Specialists:',
		'Cleaning', '0', 'Required Specialists:'
	];

	storedToken = localStorage.getItem('authToken');

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
	}

	addRequiredSpecialist(phase: number) {
		if (this.requiredSpecialistHelper[phase].specialization == null ||
			this.requiredSpecialistHelper[phase].specialization == '' ||
			this.requiredSpecialistHelper[phase].count <= 0) {
			if (this.operationPhases[phase].duration.toString() != this.phasePlaceholder[phase + 1]) {
				let phaseCheck = phase * 3;
				this.phasePlaceholder[phaseCheck + 1] = this.operationPhases[phase].duration.toString();
				let fillPhasePlaceholder = document.getElementById("phase-info-" + phase) as HTMLInputElement;
				fillPhasePlaceholder.value = this.phasePlaceholder[phaseCheck] + ', duration: ' + this.phasePlaceholder[phaseCheck + 1] + '\n' + this.phasePlaceholder[phaseCheck + 2];
			}

			return;
		}

		let specialist: RequiredSpecialist = {
			specialization: this.requiredSpecialistHelper[phase].specialization,
			count: this.requiredSpecialistHelper[phase].count,
			phase: this.requiredSpecialistHelper[phase].phase
		}
		this.requiredSpecialists.push(specialist);

		// console.log(this.requiredSpecialists);

		let phaseCheck = phase * 3;

		this.phasePlaceholder[phaseCheck + 1] = this.operationPhases[phase].duration.toString();
		this.phasePlaceholder[phaseCheck + 2] += '\n\t' + this.requiredSpecialistHelper[phase].specialization + ', ' + this.requiredSpecialistHelper[phase].count.toString();

		let fillPhasePlaceholder = document.getElementById("phase-info-" + phase) as HTMLInputElement;
		fillPhasePlaceholder.value = this.phasePlaceholder[phaseCheck] + ', duration: ' + this.phasePlaceholder[phaseCheck + 1] + '\n' + this.phasePlaceholder[phaseCheck + 2];

		// console.log(this.phasePlaceholder[phaseCheck] + ', duration: ' + this.phasePlaceholder[phaseCheck + 1] + '\n' + this.phasePlaceholder[phaseCheck + 2]);

		this.requiredSpecialistHelper[phase].specialization = '';
		this.requiredSpecialistHelper[phase].count = 0;
	}

	createOperationType(): void {
		this.operationType.phases = this.operationPhases;
		this.operationType.specialists = this.requiredSpecialists;


		if (this.storedToken) {
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
			this._service.createOperationType(this.storedToken, this.operationType.name,
				this.operationType.estimatedDuration, phaseNames, phaseDurations,
				specialistNames, specialistsCount, specialistPhases
			);
		}

		this.operationPhases = [
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
		this.operationType = { name: '', phases: [], specialists: [], estimatedDuration: 0 };

		for (let i = 0; i < 3; i++) {
			this.requiredSpecialistHelper[i].specialization = '';
			this.requiredSpecialistHelper[i].count = 0;
		}
	}

}
