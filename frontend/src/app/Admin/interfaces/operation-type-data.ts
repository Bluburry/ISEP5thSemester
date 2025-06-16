export interface OperationPhase {
	phaseName: string,
	duration: number
}

export interface NeededSpecialist{
	specialization: string,
	totalCount: number,
	assignedCount: number,
	assignedStaff: string[],
	chosenTypes: string[],
	phase: string
}

export interface RequiredSpecialist {
	specialization: string,
	count: number,
	phase: string
}

export interface OperationTypeData {
	name: string,
	phases: OperationPhase[],
	specialists: RequiredSpecialist[],
	estimatedDuration: number,
	// endDate: Date,
	// startDate: Date,
	// activationStatus: string,
	// versionNumber: string
}

export interface ScheduleAppointmentsDto {
	operationRooms: string[],
	mrnList: string[],
	opCodes: string[],
	day: string
}

export interface AppointmentDto {
	OperationRequestId: string;
	appointmentStatus: string;
	dateAndTime: string;
	id: string;
	operationRoom: string;
	patientNumber: string;
	staffId: string;
}

export interface SchedulingResult {
	StaffAgenda: Record<number, Array<{ Item1: number; Item2: number }>>;
	RoomAgenda: Record<string, Array<{ Item1: number; Item2: number }>>;
}