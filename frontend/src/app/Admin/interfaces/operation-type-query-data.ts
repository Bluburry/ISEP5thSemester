export interface OperationTypeQueryData {
	name: string,
	specialization: string,
	status: string
}

export interface OperationTypeResultData {
	ID: string;
	OperationName: string;
	EstimatedDuration: string;
	OperationStartDate: string;
	OperationEndDate: string;
	VersionNumber: string;
	ActivationStatus: string;
	OperationPhases: string[];
	PhaseNames: string[];
	PhasesDuration: string[];
	RequiredSpecialists: string[];
	SpecialistNames: string[];
	SpecialistsCount: string[];
	SpecialistsPhases: string[];
}
