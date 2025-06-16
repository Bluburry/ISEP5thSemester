import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SpecializationData } from '../interfaces/specialization-data';
import { LoginResponse } from '../../login-result';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';

@Component({
	selector: 'app-specialization-control',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './specialization-control.component.html',
})
export class SpecializationControlComponent implements OnInit {
	response: LoginResponse | null = null;
	storedToken = localStorage.getItem('authToken');

	specializations: SpecializationData[] = [];
	spDisplay: SpecializationData[] = [];
	spPicked: SpecializationData | null = null; // = { name: "", code: "", description: "" };

	searchCode: string = "";
	searchName: string = "";
	searchDescription: string = "";

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
			this.specializations = specialization;
			this.specializations.forEach(sp => {
				this.spDisplay.push(sp);
			});
		});
	}

	gotSpecialization(code: string) {
		this.spPicked = null;
		for(let sp of this.spDisplay) {
			if (sp.SpecializationCode == code) {
				this.spPicked = sp;
				break;
			}
		}
		console.log(this.spPicked);
	}

	partialSearch(place: string, event: Event) {
		this.spPicked = null;
		if (place == 'name') {
			this.spPicked = null;
			this.spDisplay = [];
			for (let sp of this.specializations) {
				if (sp.SpecializationName.includes(this.searchName))
					this.spDisplay.push(sp);
			}
		}
		else if (place == 'description') {
			this.spPicked = null;
			this.spDisplay = [];
			for (let sp of this.specializations) {
				if (sp.SpecializationDescription.includes(this.searchDescription))
					this.spDisplay.push(sp);
			}
		}
	}

	findSpecialization() {
		this.spPicked = null;
		if (this.searchCode == "" && this.searchDescription == "" && this.searchName == "") {
			this.spDisplay = [];
			this.specializations.forEach(sp => {
				this.spDisplay.push(sp);
			});
		}
		else if (this.searchCode != "") {
			this.spDisplay = [];
			for (let sp of this.specializations) {
				if (sp.SpecializationCode == this.searchCode) {
					this.spDisplay.push(sp);
					break;
				}
			}
		}
	}

	deleteSpecialization() {
		if (this.storedToken && this.spPicked) {
			this._service.deleteSpecialization(this.storedToken, this.spPicked.SpecializationCode);
			this.spPicked = null;
			this.spDisplay = [];
			this.specializations = this.specializations.filter(
				sp => sp.SpecializationCode != this.spPicked?.SpecializationCode
			);
			this.specializations.forEach(sp => {
				this.spDisplay.push(sp);
			});
		}
	}

	editSpecialization() {
		if (this.storedToken && this.spPicked) {
			this._service.editSpecialization(this.storedToken, this.spPicked);
			this.specializations = this.specializations.filter(
				sp => sp.SpecializationCode != this.spPicked?.SpecializationCode
			);
			this.specializations.push(this.spPicked);
			this.spPicked = null;
			this.spDisplay = [];
			this.specializations.forEach(sp => {
				this.spDisplay.push(sp);
			});
		}
	}
}
