import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { AdminPatientControlPanelComponent } from './Admin/patient-control/admin-patient-control.component';
import { AdminStaffControlPanelComponent } from './Admin/staff-control/admin-staff-control.component';
import { LoginComponent } from './Login/login/login.component';
import { AdminPanelComponent } from './Admin/admin-panel/admin-panel.component';
import { DoctorPanelComponent } from './Doctor/doctor-panel/doctor-panel.component';
import { DoctorRequestOperationComponent } from './Doctor/doctor-request-operation/doctor-request-operation.component';
import { DoctorOperationControlPanelComponent } from './Doctor/operation-control/doctor-operation-control.component';
import { PatientPanelComponent } from './Patient/patient-panel/patient-panel.component';
import { PatientProfileUpdatePanelComponent } from './Patient/patient-profile/patient-profile-update.component';
import { PatientProfileDeletePanelComponent } from './Patient/patient-profile/patient-profile-delete.component';
import { AdminCreatePatientPanelComponent } from './Admin/patient-control/admin-patient-create.component';
import { OperationTypeCreationComponent } from './Admin/operation-type-creation/admin-operation-type-creation.component';
import { OperationTypeControlComponent } from './Admin/operation-type-control/admin-operation-type-control.component';
import { RegisterUserComponent } from './Login/login/user-registration/register-user.component';
import { HospitalComponent } from './hospital/hospital.component';
import { ScheduleControlComponent } from './Admin/schedulling-control/schedule-control.component';
import { StaffCreateComponent } from './Admin/staff-create/staff-create.component';
import { AllergyControllerComponent } from './Admin/allergy-control/allergy-controller/allergy-controller.component';
import { OperationRoomTypeCreateComponent } from './Admin/op-room-type-create/operation-room-type-create/operation-room-type-create.component';
import { DoctorViewAllergiesComponent } from './Doctor/doctor-view-allergies/doctor-view-allergies/doctor-view-allergies.component';
import { MedicalConditionControllerComponent } from './Admin/medical-condition-creation/medical-condition-controller.component';
import { MedicalConditionSearchComponent } from './Doctor/doctor-view-medical-condition/medical-condition-search.component';
import { ClinicalDetailsUpdateComponent } from './Doctor/doctor-update-clinical-details/update-clinical-details.component';
import { SpecializationCreationComponent } from './Admin/specialization-creation/specialization-creation.component';
import { SpecializationControlComponent } from './Admin/specialization-control/specialization-control.component';
import { ClinicalDetailsSearchComponent } from './Doctor/doctor-search-clinical-details/doctor-search-clinical-details.component';
import { DoctorScheduleUpdateComponent } from './Doctor/doctor-schedule-update/doctor-schedule-update.component';
import { DoctorScheduleComponent } from './Doctor/doctor-schedule/doctor-schedule.component';
import { PrivacyPolicyComponent } from './Patient/policy-privacy/policy-privacy.component';

const routeConfig: Routes = [
	{
		path: '',
		component: LoginComponent,
		title: 'Landing page'
	},
	{
		path: 'adminPanel',
		component: AdminPanelComponent,
		title: 'Administrator Panel'
	},
	{
		path: 'admin-patient-create',
		component: AdminCreatePatientPanelComponent,
		title: 'Administrator Patient Creation'
	},
	{
		path: 'admin-patient-control',
		component: AdminPatientControlPanelComponent,
		title: 'Administrator Patient Management'
	},
	{
		path: 'admin-staff-control',
		component: AdminStaffControlPanelComponent,
		title: 'Administrator Staff Management'
	},
	{
		path: 'admin-operation-type-creation',
		component: OperationTypeCreationComponent,
		title: 'Administrator Operation Type Creation'
	},
	{
		path: 'admin-operation-type-control',
		component: OperationTypeControlComponent,
		title: 'Administrator Operation Type Creation'
	},
	{
		path: 'admin-staff-create',
		component: StaffCreateComponent,
		title: 'Staff Creation'
	},
	{
		path: 'schedule-control',
		component: ScheduleControlComponent,
		title: 'Schedulling'
	},
	{
		path: 'doctor-panel',
		component: DoctorPanelComponent,
		title: 'Doctor Panel'
	},
	{
		path: 'doctor-operation-request',
		component: DoctorRequestOperationComponent,
		title: 'Request Operation'
	},
	{
		path: 'doctor-operation-control',
		component: DoctorOperationControlPanelComponent,
		title: 'Operation Management'
	},
	{
		path: 'doctor-schedule',
		component: DoctorScheduleComponent,
		title: 'Schedule appointment'
	},
	{
		path: 'doctor-schedule-update',
		component: DoctorScheduleUpdateComponent,
		title: 'Update appointment'
	},
	{
		path: 'doctor-search-clinical-details',
		component: ClinicalDetailsSearchComponent,
		title: 'Search patient medical records'
	},
	{
		path: 'patient-panel',
		component: PatientPanelComponent,
		title: 'Patient Panel'
	},
	{
		path: 'patient-profile-update',
		component: PatientProfileUpdatePanelComponent,
		title: 'Profile Update'
	},
	{
		path: 'patient-profile-delete',
		component: PatientProfileDeletePanelComponent,
		title: 'Profile Delete'
	},
	{
		path: 'privacy-policy',
		component: PrivacyPolicyComponent,
		title: 'Privacy Policy'
	},
	{
		path: 'register-user',
		component: RegisterUserComponent,
		title: 'User Register'
	},
	{
		path: 'hospital-3d',
		component: HospitalComponent,
		title: "Hospital 3D Visualization"
	},
	{
		path: 'admin-allergy-control',
		component: AllergyControllerComponent,
		title: "Allergy Data Management"
	},
	{
		path: 'admin-condition-control',
		component: MedicalConditionControllerComponent,
		title: "Medical Conditions Creation"
	},
	{
		path: 'app-operation-room-type-create',
		component: OperationRoomTypeCreateComponent,
		title: "Creation of Room Types"
	},
	{
		path: 'app-doctor-view-allergies',
		component: DoctorViewAllergiesComponent,
		title: "Viewing of Allergy Data"
	},
	{
		path: 'app-medical-condition-search',
		component: MedicalConditionSearchComponent,
		title: "Viewing of Medical Condition Data"
	},
	{
		path: 'app-clinical-details-update',
		component: ClinicalDetailsUpdateComponent,
		title: "Update Patient's ClinicalDetails"
	},
	{
		path: 'specialization-creation',
		component: SpecializationCreationComponent,
		title: "Create a new Specialization"
	},
	{
		path: 'specialization-control',
		component: SpecializationControlComponent,
		title: "List, edit, or remove Specializations"
	},
];

export default routeConfig;
