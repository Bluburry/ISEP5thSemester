export interface RequiredSpecialist {
	specialization: string,
	count: string,
	phase: string
}


export interface OperationRequestData {
	ID: string;
	Doctor: string;
	Patient: string;
	OperationType: string;
	OperationDeadline: string;
	OperationPriority: string;
	RequiredSpecialists: string[];
	EstimatedTime: string;
}

