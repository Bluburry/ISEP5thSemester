export interface AppointmentDto {
	id: string;
  	dateAndTime: string;
  	appointmentStatus: string | null;
  	staffId: string;
	patientNumber: string;
	operationRoom: string;
	operationRequestId: string;
}