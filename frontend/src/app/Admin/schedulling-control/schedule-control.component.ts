import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Router } from '@angular/router';
import { LoginResponse } from '../../login-result';
import { AppointmentDto, NeededSpecialist, OperationPhase, OperationTypeData, RequiredSpecialist, ScheduleAppointmentsDto, SchedulingResult } from '../interfaces/operation-type-data';
import { OperationTypeQueryData, OperationTypeResultData } from '../interfaces/operation-type-query-data';
import { StaffData } from '../interfaces/staff-data';
import { StaffQueryData } from '../interfaces/staff-query-data';
import { OperationRequestData } from '../../Doctor/interfaces/operation-request-data';
import { OperationRoomData } from '../interfaces/operation-room-data';

@Component({
	selector: 'app-operation-type-control',
	standalone: true,
	imports: [CommonModule, FormsModule],
	templateUrl: './schedule-control.component.html',
	styleUrl: './schedule-control.component.css'
})

export class ScheduleControlComponent implements OnInit {
	response: LoginResponse | null = null;
	storedToken = localStorage.getItem('authToken');

	staffRoster: StaffData[] = [];
	staff: StaffData | null = null;
	
	operationRequests: OperationRequestData[] = [];

	rooms: OperationRoomData[] = [];
	selectedRooms: OperationRoomData[] = [];
	day: string | null = null;

	queryStaffData: StaffQueryData = {
		license: '',
		name: '',
		email: '',
		specialization: '',
		status: ''
	}

	requiredSpecialists: RequiredSpecialist[] = [];
	neededSpecialists: NeededSpecialist[] = [];

	appointmentDto: SchedulingResult | null = null;
	
	roomSchedule: String[] = [];
	staffSchedule: String[] = [];

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
		this._service.getAllStaff(this.queryStaffData, token);
		this._service.staff$.subscribe((staff: StaffData[]) => {
		  this.staffRoster = staff;
		  console.log('Staff data:', this.staffRoster);
		});

		this._service.getOperationRequests(token);
		this._service.opRequests$.subscribe((opRequest: OperationRequestData[]) => {
			this.operationRequests = opRequest;
			console.log("Operation Request data: ", this.operationRequests);
		});

		this._service.getOperationRooms(token);
		this._service.opRooms$.subscribe((opRoom: OperationRoomData[]) => {
			this.rooms = opRoom;
			console.log("Operation Room data: ", this.rooms);
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

	addRoom(room: OperationRoomData) {
		if (room) {
			if (!this.selectedRooms.includes(room)) {
				this.selectedRooms.push(room);
				console.log(`Room ID ${room.Name} added to rooms`);
			} else {
				console.log(`Room ID ${room.Name} exists to rooms`);
			}
		}
	}

	generateScheduleAppointments(day: string): ScheduleAppointmentsDto {
		// Extract all unique operation types (phases) from neededSpecialists
		var rooms: string[] = [];
		this.selectedRooms.forEach(element => {
			rooms.push(element.Id);
		});
		const opCodes = Array.from(
			new Set(this.neededSpecialists.flatMap((specialist) => specialist.chosenTypes))
		);
		// Extract all license numbers from assignedStaff arrays in neededSpecialists
		const mrnList = this.neededSpecialists
			.flatMap((specialist) => specialist.assignedStaff)
			.filter((license, index, self) => self.indexOf(license) === index); // Remove duplicates
	
		return {
			operationRooms: rooms,
			mrnList: mrnList,
			opCodes: opCodes,
			day: day.replace(/-/g, ''),
		};
	}
	  
	minutesToTimeString(minutes: number): string {
		const hours = Math.floor(minutes / 60);
		const mins = minutes % 60;
		return `${hours.toString().padStart(2, '0')}:${mins.toString().padStart(2, '0')}`;
	}

	logRoomAgenda(roomAgenda: Record<string, Array<{ Item1: number; Item2: number }>>): void {
		console.log('Room Agenda:');
		Object.entries(roomAgenda).forEach(([room, schedules]) => {
			this.roomSchedule.push(`Room: ${room}`);
			schedules.forEach((schedule, index) => {
				const startTime = this.minutesToTimeString(schedule.Item1);
				const endTime = this.minutesToTimeString(schedule.Item2);
				this.roomSchedule.push(`Scheduled from: ${startTime} until ${endTime}`)
			});
			this.roomSchedule.push("---------------------------------")
		});
		console.log(this.roomSchedule)
	};
	
	
	logStaffAgenda(staffAgenda: Record<number, Array<{ Item1: number; Item2: number }>>): void {
		console.log('Staff Agenda:');
		Object.entries(staffAgenda).forEach(([staffId, schedules]) => {
			this.staffSchedule.push(`Staff ID: ${staffId}`);
			schedules.forEach((schedule, index) => {
				const startTime = this.minutesToTimeString(schedule.Item1);
				const endTime = this.minutesToTimeString(schedule.Item2);
				this.staffSchedule.push(`Scheduled from: ${startTime} until ${endTime}`)
			});
			this.staffSchedule.push("---------------------------------")
		});
		console.log(this.staffSchedule)
	}



	generateSchedule(){
		if(this.storedToken && this.selectedRooms.length != 0 && this.day){
			this._service.getSchedule(this.generateScheduleAppointments(this.day), this.storedToken); 
			this._service.schedulingResult$.subscribe(value => {
				this.appointmentDto = value;

				if(this.appointmentDto){
					this.logRoomAgenda(this.appointmentDto.RoomAgenda);
					this.logStaffAgenda(this.appointmentDto.StaffAgenda);
				}
			})
		}
	}

}
