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

  export interface EditPatientDtoPatient {
    firstName: string;
    lastName: string;
    fullName: string;
    email: string;
    phone: string;
    medicalHistory?: AppointmentHistory;
    dateOfBirth: string;
    emergencyContact: string;
    gender: string;
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
  