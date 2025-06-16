import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SpecializationData } from '../interfaces/specialization-data';
import { LoginResponse } from '../../login-result';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';

@Component({
	selector: 'app-specialization-creation',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './specialization-creation.component.html',
})

export class SpecializationCreationComponent implements OnInit {
	response: LoginResponse | null = null;
	storedToken = localStorage.getItem('authToken');

	spCode: string = "";
	spName: string = "";
	spDesc: string = "";

	specialization: SpecializationData = { SpecializationName: "", SpecializationCode: "", SpecializationDescription: "" };

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
		} else {
			this._router.navigate(['']);
		}
	}

	createSpecialization(): void {
		if (this.storedToken) {
			if (this.specialization.SpecializationName == "")
				return;

			let dto: SpecializationData = {
				SpecializationCode: this.specialization.SpecializationCode,
				SpecializationName: this.specialization.SpecializationName,
				SpecializationDescription: this.specialization.SpecializationDescription
			};

			this._service.createSpecialization(this.storedToken, dto);
		}

		this.specialization.SpecializationCode = "";
		this.specialization.SpecializationName = "";
		this.specialization.SpecializationDescription = "";
	}

}
