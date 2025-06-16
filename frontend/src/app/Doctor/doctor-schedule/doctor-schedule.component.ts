import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../Admin/admin.service';
import { Router } from '@angular/router';
import { LoginResponse } from '../../login-result';
import { AppointmentDto, NeededSpecialist, OperationPhase, RequiredSpecialist, ScheduleAppointmentsDto } from '../../Admin/interfaces/operation-type-data';
import { OperationTypeQueryData, OperationTypeResultData } from '../../Admin/interfaces/operation-type-query-data';
import { StaffData } from '../../Admin/interfaces/staff-data';
import { StaffQueryData } from '../interfaces/staff-query-data';
import { OperationRequestData } from '../interfaces/operation-request-data';
import { OperationRoomData } from '../../Admin/interfaces/operation-room-data';
import { DoctorService } from '../doctor.service';

@Component({
	selector: 'app-operation-type-control',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './doctor-schedule.component.html',
	styleUrl: './doctor-schedule.component.css'
})

export class DoctorScheduleComponent implements OnInit {
	response: LoginResponse | null = null;
	storedToken = localStorage.getItem('authToken');

	staffRoster: StaffData[] = [];
	staff: StaffData | null = null;
	
	operationRequests: OperationRequestData[] = [];
	operationRequest: OperationRequestData | null = null;

	selectedRequest: any | null = null;
	rooms: OperationRoomData[] = [];
	selectedRoom: string | null = null;
	day: string | null = null;

	neededSpecialists: NeededSpecialist[] = [];
	worked: string | null = null;
	constructor(
		private _service: DoctorService,
		private _admService: AdminService,
		private _router: Router
	) { }

	ngOnInit(): void {
		if (this.storedToken) {
			this._service.validate(this.storedToken).subscribe(response => {
				if (response.role != "STAFF") {
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
		this._service.getOperationRequests("", "", "", "", token);
		this._service.opRequests$.subscribe((opRequest: OperationRequestData[]) => {
			this.operationRequests = opRequest;
			console.log("Operation Request data: ", this.operationRequests);
		});

		this._admService.getOperationRooms(token);
		this._admService.opRooms$.subscribe((opRoom: OperationRoomData[]) => {
			this.rooms = opRoom;
			console.log("Operation Room data: ", this.operationRequests);
		});
	}

	onRequestChange(selectedOpReq: any): void {
		console.log(selectedOpReq);

		this.neededSpecialists = [];
		this.totalSpecialists(selectedOpReq.ID, selectedOpReq.RequiredSpecialists);
	}

	totalSpecialists(opId: string, specialistStrings: string[]) {
		console.log("hi");
		specialistStrings.forEach((specialistString: string) => {
			const regex = /Specialization:\s*(\w+),\s*count:\s*(\d+),\s*phase:\s*(\w+)/;
			const match = specialistString.match(regex);
	
			if (match) {
				const specialization = match[1]; 
				const count = parseInt(match[2]); 
				const phase = match[3];

				console.log(specialization, count, phase)
				let exist = false;
	
				this.neededSpecialists.forEach((spec2: NeededSpecialist, index: number) => {
					if (spec2.specialization === specialization) {
						exist = true;
	
						if (spec2.totalCount < count) this.neededSpecialists[index].totalCount = count;
					}
					if (!spec2.chosenTypes.includes(opId)) {
                        this.neededSpecialists[index].chosenTypes.push(opId);
                    }
				});
	
				if (!exist) {
					this.neededSpecialists.push({
						specialization,
						totalCount: count,
						assignedCount: 0,
						assignedStaff: [],
						chosenTypes: [opId],
						phase
					})
				}
			} else {
				console.error("Invalid string format:", specialistString);
			}
		});
		console.log(this.neededSpecialists);
	}

	addSpecialist(license: string, spec: string) {
		const specialist = this.neededSpecialists.find(
			(needed) => needed.specialization === spec
		);
	
		if (specialist) {
			if (!specialist.assignedStaff.includes(license)) {
				specialist.assignedCount++;
				specialist.assignedStaff.push(license);
				console.log(`License ${license} added to specialization ${spec}`);
			} else {
				console.log(`License ${license} is already assigned to specialization ${spec}`);
			}
		} else {
			console.error(`Specialization ${spec} not found in neededSpecialists`);
		}
	}

	fetchAvailableSpecialists(){
		if(this.storedToken && this.selectedRequest && this.day){
			console.log(this.selectedRequest.ID)
			this._service.getStaffList(this.selectedRequest.ID, this.day, this.storedToken)

			this._service.staff.subscribe((staff: StaffData[]) => {
				this.staffRoster = staff;
				console.log('Staff data:', this.staffRoster);
			});
		}
	}

	generateSchedule(){
		if(this.storedToken && this.selectedRequest){
			console.log(this.selectedRequest);
			var staffIds = "";
			var notEnoughSpecialists = false;
			this.neededSpecialists.forEach(specialist => {
				if(specialist.totalCount == specialist.assignedCount){
					specialist.assignedStaff.forEach(ids => {
						staffIds = staffIds + "," + ids;
					});
				}else{
					notEnoughSpecialists = true;
				}
			});
			staffIds = staffIds.substring(1)
			try{
				if(!notEnoughSpecialists && this.day && this.selectedRoom){
					this._service.scheduleAppointment(this.day, staffIds, this.selectedRequest.Patient, this.selectedRoom, this.selectedRequest.ID, this.storedToken)
					this.worked = "a";
				}
			} catch (e) {
				console.log(e);
			}
		}
	}
}
