import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../Admin/admin.service';
import { Router } from '@angular/router';
import { LoginResponse } from '../../login-result';
import { AppointmentDto } from '../interfaces/appointment-data'
import { NeededSpecialist, OperationPhase, RequiredSpecialist, ScheduleAppointmentsDto } from '../../Admin/interfaces/operation-type-data';
import { OperationTypeQueryData, OperationTypeResultData } from '../../Admin/interfaces/operation-type-query-data';
import { StaffData } from '../../Admin/interfaces/staff-data';
import { StaffQueryData } from '../interfaces/staff-query-data';
import { OperationRequestData } from '../interfaces/operation-request-data';
import { OperationRoomData } from '../../Admin/interfaces/operation-room-data';
import { DoctorService } from '../doctor.service';

@Component({
	selector: 'app-doctor-schedule-update',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './doctor-schedule-update.component.html',
	styleUrl: './doctor-schedule-update.component.css'
})

export class DoctorScheduleUpdateComponent implements OnInit {
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

	appointments: AppointmentDto[] = [];
	selectedAppointment: AppointmentDto | null = null;

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
		this._service.getAppointments(token);
		this._service.appointment$.subscribe((appt: AppointmentDto[]) => {
			this.appointments = appt;
			console.log("Appointment data: ", this.appointments);
		})

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
		this.neededSpecialists = [];
		this.selectedRequest = selectedOpReq;
		this.totalSpecialists(selectedOpReq.ID, selectedOpReq.RequiredSpecialists);
		
	}

	trackByFn(index: number, item: any): any {
		return item.ID;
	}

	changeAppointment(selectedAppt: any): void{
		this.selectedAppointment = selectedAppt;

		this.rooms.forEach(element => {
			if(element.Name === selectedAppt.operationRoom)
				this.selectedRoom = element.Id;
			
		});

		this.operationRequests.forEach(element => {
			if(element.ID === selectedAppt.OperationRequestId){
				this.selectedRequest = element;
				this.totalSpecialists(this.selectedRequest, element.RequiredSpecialists)
			}
				
		});
	}

	totalSpecialists(opId: string, specialistStrings: string[]) {
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
			this._service.getStaffList(this.selectedRequest.ID, this.day, this.storedToken)

			this._service.staff.subscribe((staff: StaffData[]) => {
				this.staffRoster = staff;
				console.log('Staff data:', this.staffRoster);
			});
		}
	}

	changeSchedule(){
		if(this.storedToken && this.selectedAppointment){
			let requestID = this.selectedAppointment.operationRequestId;
			let staffIds = this.selectedAppointment.staffId;
			var notEnoughSpecialists = false;
			if(this.selectedRequest.ID != requestID){
				requestID = this.selectedRequest.ID;
				staffIds = "";
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
			}

			var room = this.selectedAppointment.operationRoom;
			if(room != this.selectedRoom && this.selectedRoom)
				room = this.selectedRoom;

			var day = this.selectedAppointment.dateAndTime;
			if(this.day != day && this.day)
				day = this.day;

			if(!notEnoughSpecialists) 
				this._service.updateAppointment(this.selectedAppointment.id, day, staffIds, room, this.storedToken)		
		}
	}
}
