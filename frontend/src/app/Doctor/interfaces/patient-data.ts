import { AllergyData } from "../../Admin/interfaces/allergy-data";
import { MedicalConditionData } from "./medical-condition-data";

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

  export interface ClinicalDetails{
    patientMRN: string;
    allergies: AllergyData[];
    medicalConditions: MedicalConditionData[];
  }
  