export interface AppointmentHistory {
    id: string;
    dateAndTime: string;
    appointmentStatus: string;
    reason: string;
    diagnosis: string;
    notes: string;
    staffId: string;
    patientNumber: string;
  }

  export interface EditPatientDtoAdmin {
    patientId: string;
    firstName: string;
    lastName: string;
    fullName: string;
    email: string;
    phone: string;
    medicalHistory?: AppointmentHistory;
    dateOfBirth: string;
  }

  export interface PatientRegistrationDto{
    firstName: string;    
    lastName: string;
    fullName: string;
    gender: string;
    dateOfBirth: string;
    email: string;
    phone: string;
    emergencyContact: string;
  }
  
  export interface PatientData {
    mrn: string;
    firstName: string;
    lastName: string;
    fullName: string;
    gender: string;
    dateOfBirth: string;
    email: string;
    phone: string;
    emergencyContact: string;
    appointmentHistory: AppointmentHistory[];
    userId: string;
  }
  