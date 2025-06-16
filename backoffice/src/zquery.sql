-- Active: 1735408519094@@10.9.22.65@3306@HospitalDb
INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('admin@hospital.com', 'admin@hospital.com', '!AdminPassword1', 2, 1);


INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('patient1@hospital.com', 'patient1@hospital.com', '!PatientPassword1', 1, 1);

INSERT INTO ContactInformations
	(Id, Email_Value, Phone_Value)
VALUES
	('b3b8c97f-2a32-4c4e-b234-334dc03d59e4', 'johndoe@example.com', '+1234567890');

INSERT INTO Patients
	(Id, firstName_firstName, lastName_lastName,fullName_fullname, gender, ContactInformationId,  emergencyContact_Value, userId, dateOfBirth_dateOfBirth)
VALUES
	('MRN12345', 'John', 'Doe', 'John Doe' , '1', 'b3b8c97f-2a32-4c4e-b234-334dc03d59e4',  '1234567890', 'patient1@hospital.com', '2011-05-05');


/* DELETE FROM Patients
WHERE Id = 'MRN1031'; */

-- INSERT INTO Patients
-- 	(Id, firstName_firstName, lastName_lastName,fullName_fullname, gender, ContactInformationId,  emergencyContact_Value, dateOfBirth_dateOfBirth)
-- VALUES
-- 	('MRN1031', 'Richie', 'Days', 'Richie Days' , 'Male', 'b3b8c97f-2a32-4c4e-b234-334dc03db245',  '931113222', '2004-03-11');


/* DELETE FROM Patients
WHERE Id = '20241 0000001'; */


 
INSERT INTO Patients
	(Id, firstName_firstName, lastName_lastName,fullName_fullname, gender, ContactInformationId,  emergencyContact_Value, dateOfBirth_dateOfBirth)
VALUES
	('MRN12346', 'John', 'Doe', 'John Doe' , '1', 'b3b8c97f-2a32-4c4e-b234-334dc03db234',  '1234567890', '2001-04-12');



INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('patient3@hospital.com', 'patient3@hospital.com', '!PatientPassword3', 1, 1);
INSERT INTO ContactInformations
	(Id, Email_Value, Phone_Value)
VALUES
	('b3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'johndoe@example.com', '+1234567890');
INSERT INTO Patients
	(Id, firstName_firstName, lastName_lastName,fullName_fullname, gender, ContactInformationId,  emergencyContact_Value, userId, dateOfBirth_dateOfBirth)
VALUES
	('MRN12347', 'John', 'Doe', 'John Doe' , '1', 'b3b8c97f-2a32-4c4e-b234-334dc03d59d1',  '1234567890', 'patient3@hospital.com', '1999-07-11');

insert into Specializations
	(Id, SpecializationName)
values
	('1', 'testSpecialization'),
	('2', 'specializationTest'),
	('3', 'specialTestization');

-- delete from OperationTypes where OperationTypeName_OperationName='test';

insert into OperationTypes
	(Id, OperationTypeName_OperationName, EstimatedDuration_Duration, OperationTypeStartDate_StartDate, ActivationStatus, VersionNumber)
values
	('1fb2a6bf-e9b0-4635-b045-959a669973a7', 'test', '50', '2024-10-25 21:57:28.3953453', '1', '1');
insert into OperationTypes
	(Id, OperationTypeName_OperationName, EstimatedDuration_Duration, OperationTypeStartDate_StartDate, ActivationStatus, VersionNumber)
values
	('c14bf1a7-cd5d-4fd8-b996-1c366d618a5d', 'test2', '60', '2024-10-25 21:58:28.3953453', '1', '1');
insert into OperationTypes
	(Id, OperationTypeName_OperationName, EstimatedDuration_Duration, OperationTypeStartDate_StartDate, ActivationStatus, VersionNumber)
values
	('2996b44c-3481-424a-b6f3-e324be21c2df', 'test3', '55', '2024-10-25 21:59:28.3953453', '1', '1');

insert into RequiredSpecialists
	(Id, SpecializationId, SpecialistCount_Count, OperationTypeId, PhaseName)
values
	('481082df-ca2d-4a1d-a90c-7888f4353c68', '1', '5', '1fb2a6bf-e9b0-4635-b045-959a669973a7', '0'),
	('d767341c-3dbc-47fc-864c-671b478e57aa', '2', '3', '1fb2a6bf-e9b0-4635-b045-959a669973a7', '1'),
	('aa59bfcb-b3b0-4a45-9630-abcca57d9e80', '1', '4', 'c14bf1a7-cd5d-4fd8-b996-1c366d618a5d', '1'),
	('20b7a277-96f7-43e1-aa32-b8c5baaba8bb', '2', '6', '2996b44c-3481-424a-b6f3-e324be21c2df', '2'),
	('51f7c277-a6f7-4ee1-bb52-a3c5b231a8bb', '3', '6', '2996b44c-3481-424a-b6f3-e324be21c2df', '2');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('026f0195-0bb0-48a9-bf11-16a18e5cbb8a', '0', '15', '1fb2a6bf-e9b0-4635-b045-959a669973a7'),
	('327db1e5-6922-43a1-b171-164d6e2fcd47', '1', '15', '1fb2a6bf-e9b0-4635-b045-959a669973a7'),
	('b246ce67-961b-4c4f-af63-545535e790d8', '2', '15', '1fb2a6bf-e9b0-4635-b045-959a669973a7'),
	('77cc446d-6c3a-4cf7-800f-6feeac113a22', '0', '20', 'c14bf1a7-cd5d-4fd8-b996-1c366d618a5d'),
	('de67e7dc-1b0e-40a5-b226-ad240994e54e', '1', '20', 'c14bf1a7-cd5d-4fd8-b996-1c366d618a5d'),
	('e1ae7159-27f1-4e3b-b13d-1a9311e917f4', '2', '20', 'c14bf1a7-cd5d-4fd8-b996-1c366d618a5d'),
	('9ff0888b-5296-40a1-97fc-77c34c40e3fb', '0', '15', '2996b44c-3481-424a-b6f3-e324be21c2df'),
	('a6656ebb-94f5-4b33-a6f1-d7ef1120f66d', '1', '15', '2996b44c-3481-424a-b6f3-e324be21c2df'),
	('e4519b70-dd50-40e0-ba2d-61a340faad97', '2', '15', '2996b44c-3481-424a-b6f3-e324be21c2df');

-- STAFF


INSERT INTO Specializations
	(Id, SpecializationName)
VALUES
	('6', 'Doctor'),
	('7', 'Cleaner'),
	('8', 'Anaesthesist');

INSERT INTO Doctors
	(Id, StaffId)
VALUES
	('z3b8c97f-2a32-4c4e-b234-334dc03d59d1', '103');


INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('staff@hospital.com', 'staff@hospital.com', '!StaffPassword1', 0, 1);

INSERT INTO ContactInformations
	(Id, Email_Value, Phone_Value)
VALUES
	('d3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'staff@hospital.com', '555555666');

INSERT INTO Staff
	(Id, specializationId, userId, ContactInformationId, Fullname_fullname, FirstName_firstName, LastName_lastName, Status)
VALUES
	('2', '1', 'staff@hospital.com', 'd3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'Staff Hospital', 'Staff', 'Hospital', 0);

INSERT INTO Doctors
	(Id, StaffId)
VALUES
	('a54dc97f-2a32-4c4e-b234-334dc03d59d1', '2');


INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('anaesthesist@hospital.com', 'anaesthesist@hospital.com', '!AnasthPassword1', 0, 1);

INSERT INTO ContactInformations
	(Id, Email_Value, Phone_Value)
VALUES
	('d3b8c97f-2a32-4c4e-b234-334dc03d59d2', 'ana@hospital.com', '555555777');

INSERT INTO Staff
	(Id, specializationId, userId, ContactInformationId, Fullname_fullname, FirstName_firstName, LastName_lastName, Status)
VALUES
	('101', '8', 'anaesthesist@hospital.com', 'd3b8c97f-2a32-4c4e-b234-334dc03d59d2', 'Anaesthesist Hospital', 'Anaesthesist', 'Hospital', 0);

INSERT INTO operationRoomTypes
	(Id, Name_Value, description_description)
VALUES
	('d3b8c97f-2a42-4c4e-b234-334dc03d59d2', 'SurgeryRoom', '');

INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('d3b8c97f-2a42-4c4e-b234-334dc03d59d2', 'A', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef0-6f66-4c05-994f-3a107786421f', NULL, NULL, '(300,415)', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef2-6f66-4c05-994f-3a107786421f', NULL, NULL, '(430,600)', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');


INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('d3b8c97f-2a42-4c4e-b234-334dc03b12d2', 'B', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef1-6f66-4c05-994f-3a107786421a', NULL, NULL, '(1000,1215)', 'd3b8c97f-2a42-4c4e-b234-334dc03b12d2', 'd3b8c97f-2a42-4c4e-b234-334dc03b12d2');


INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('d3b8c97f-2b12-4c4e-b234-334dc03b12d2', 'C', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef1-6f66-1d05-994f-3a107786421f', NULL, NULL, '(500,1015)', 'd3b8c97f-2b12-4c4e-b234-334dc03b12d2', 'd3b8c97f-2b12-4c4e-b234-334dc03b12d2');


INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('a1a8c97f-2b12-4c4e-b234-334dc03b12d2', 'D', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bea7-6f66-4c05-994f-3a107786421f', NULL, NULL, '(650,1015)', 'a1a8c97f-2b12-4c4e-b234-334dc03b12d2', 'a1a8c97f-2b12-4c4e-b234-334dc03b12d2');

INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('a1a8c97f-2b12-1a1a-b234-334dc03b12d2', 'E', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef1-6f66-4c05-114b-3a107786421f', NULL, NULL, '(600,1015)', 'a1a8c97f-2b12-1a1a-b234-334dc03b12d2', 'a1a8c97f-2b12-1a1a-b234-334dc03b12d2');

INSERT INTO operationRooms
	(Id, Name, OperationRoomTypeId)
Values
	('a1a4c97f-1b11-1a1a-b234-334dc03b12d2', 'F', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO availabilitySlots
	(Id, StaffId, StaffId1, Value, OperationRoomId, roomId)
VALUES
	('d4c2bef1-3a31-4c05-994f-3a107786421f', NULL, NULL, '(600,1315)', 'a1a4c97f-1b11-1a1a-b234-334dc03b12d2', 'a1a4c97f-1b11-1a1a-b234-334dc03b12d2');

INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('cleaner@hospital.com', 'cleaner@hospital.com', '!CleanerPassword1', 0, 1);

INSERT INTO ContactInformations
	(Id, Email_Value, Phone_Value)
VALUES
	('d3b8c97f-2a32-4c4e-b234-334dc03d59d3', 'cleaner@hospital.com', '555555777');

INSERT INTO Staff
	(Id, specializationId, userId, ContactInformationId, Fullname_fullname, FirstName_firstName, LastName_lastName, Status)
VALUES
	('102', '7', 'cleaner@hospital.com', 'd3b8c97f-2a32-4c4e-b234-334dc03d59d3', 'Cleaner Hospital', 'Cleaner', 'Hospital', 0);


insert into OperationTypes
	(Id, OperationTypeName_OperationName, EstimatedDuration_Duration, OperationTypeStartDate_StartDate, ActivationStatus, VersionNumber)
values
	('2996b44c-3481-424a-b6f3-e324be21d5df', 'heartSurgery', '180', '2024-10-25 21:59:28.3953453', '1', '1');

insert into RequiredSpecialists
	(Id, SpecializationId, SpecialistCount_Count, OperationTypeId, PhaseName)
values
	('20b7a277-96f7-43e1-aa32-b8c5baaba9cc', '6', '1', '2996b44c-3481-424a-b6f3-e324be21d5df', '1'),
	('20b7a277-96f7-43e1-aa32-b8c5baaba9cd', '7', '1', '2996b44c-3481-424a-b6f3-e324be21d5df', '2'),
	('20b7a277-96f7-43e1-aa32-b8c5baaba9cf', '8', '1', '2996b44c-3481-424a-b6f3-e324be21d5df', '0');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('e4519b70-dd50-40e0-ba2d-61a341fbbd97', '0', '20', '2996b44c-3481-424a-b6f3-e324be21d5df');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('e4219b70-dd50-40e0-ba2d-61a341fbbd97', '1', '20', '2996b44c-3481-424a-b6f3-e324be21d5df');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('e4419b70-dd50-40e0-ba2d-61a341fbbd97', '2', '20', '2996b44c-3481-424a-b6f3-e324be21d5df');


insert into OperationTypes
	(Id, OperationTypeName_OperationName, EstimatedDuration_Duration, OperationTypeStartDate_StartDate, ActivationStatus, VersionNumber)
values
	('7887b44c-3481-424a-b6f3-e324be21d5df', 'lungSurgery', '180', '2024-10-25 21:59:28.3953453', '1', '1');

insert into RequiredSpecialists
	(Id, SpecializationId, SpecialistCount_Count, OperationTypeId, PhaseName)
values
	('21b7a277-96f7-43e1-aa32-b8c5baaba9cc', '6', '3', '7887b44c-3481-424a-b6f3-e324be21d5df', '1'),
	('22b7a277-96f7-43e1-aa32-b8c5baaba9cd', '7', '1', '7887b44c-3481-424a-b6f3-e324be21d5df', '2'),
	('23b7a277-96f7-43e1-aa32-b8c5baaba9cf', '8', '2', '7887b44c-3481-424a-b6f3-e324be21d5df', '0');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('a4519b70-dd50-40e0-ba2d-61a341fbbd97', '0', '30', '7887b44c-3481-424a-b6f3-e324be21d5df');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('a4219b70-dd50-40e0-ba2d-61a341fbbd97', '1', '30', '7887b44c-3481-424a-b6f3-e324be21d5df');

insert into OperationPhases
	(Id, PhaseName, PhaseDuration, OperationTypeId)
values
	('a4419b70-dd50-40e0-ba2d-61a341fbbd97', '2', '30', '7887b44c-3481-424a-b6f3-e324be21d5df');

INSERT INTO OperationRequests
	(Id, DoctorId, PatientId, OperationTypeId, OperationPriority, OperationDeadline, OperationStatus)
VALUES('e2229b70-dd50-40e0-ba2d-61a341ffbd97', 'z3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'MRN12345', '2996b44c-3481-424a-b6f3-e324be21d5df', 1, '2024-12-15', 0);



-- Operation Requests

insert into OperationRequests
	(Id, DoctorId, PatientId, OperationTypeId, OperationPriority, OperationDeadline, OperationStatus)
values
	('3666b44c-3481-424a-b6f3-e324fe21d5df', 'z3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'MRN1031', '2996b44c-3481-424a-b6f3-e324be21d5df', '2', '2024/12/24', '0');
insert into OperationRequests
	(Id, DoctorId, PatientId, OperationTypeId, OperationPriority, OperationDeadline, OperationStatus)
values
	('5777b33c-1843-535b-b6f3-e324fe21dadf', 'z3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'MRN12345', '2996b44c-3481-424a-b6f3-e324be21d5df', '0', '2024/10/22', '0');

-- delete from OperationRequests where Id is '5777b33c-1843-535b-b6f3-e324fe21dpdf';


INSERT INTO OperationRequests
	(Id, DoctorId, PatientId, OperationTypeId, OperationPriority, OperationDeadline, OperationStatus)
VALUES('e4419b70-dd50-40e0-ba2d-61a341ffbd97', 'z3b8c97f-2a32-4c4e-b234-334dc03d59d1', 'MRN12345', '2996b44c-3481-424a-b6f3-e324be21d5df', 1, '2024-12-15', 0);


-- Misc

insert into Specializations
	(Id, SpecializationName)
values
	('4', 'anesthesiologist'),
	('5', 'assistant');
	/* ('cleaner'), */

	--- TESTING VALUES SETUP BE VERY CAREFUL WHEN CHANGING AND ASK THE TEAM


--INSERT INTO Tokens(Id, ExpirationDate_DateTime, UserId, TokenValue)
--VALUES ('d17a7e65-02c0-4189-affa-3101b4c0a4e9', '2026-11-22 10:27:42.3703345', 'admin@hospital.com', 8);

-- delete from OperationTypes where OperationTypeName_OperationName is 'eyeSurgery';
INSERT INTO Users
	(Id, EmailAddress_Value, Password_Value, Role, ActivationStatus)
VALUES
	('tobecleaner@hospital.com', 'tobecleaner@hospital.com', '!ToClean1', 0, 1);


insert into Specializations (Id, SpecializationName, SpecializationDescription)
VALUES ('666', 'testingDescription', 'According to all known laws of aviation, there is no way that a bee should be able to fly. Its wings are too small to get its fat little body off the ground. The bee, of course, flies anyway. Because bees don’t care what humans think is impossible. According to all known laws of aviation, there is no way that a bee should be able to fly. Its wings are too small to get its fat little body off the ground. The bee, of course, flies anyway. Because bees don’t care what humans think is impossible.');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('1ad622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 06:00', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('2bd622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 07:10', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('3cd622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 16:50', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'd3b8c97f-2a42-4c4e-b234-334dc03d59d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('4dd622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 09:00', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'd3b8c97f-2b12-4c4e-b234-334dc03b12d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('5ed622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 10:00', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'a1a8c97f-2b12-1a1a-b234-334dc03b12d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('6fd622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 12:00', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'a1a4c97f-1b11-1a1a-b234-334dc03b12d2');

INSERT INTO Appointments (Id, dateAndTime_DateTime, appoitmentStatus, patientId, patientID, requestId, OpRoomId)
VALUES ('7ad622e4-8e46-41db-8476-fc422d2ba341', '2025-01-05 11:36', '0', 'MRN1031', 'MRN1031', '3666b44c-3481-424a-b6f3-e324fe21d5df', 'a1a8c97f-2b12-4c4e-b234-334dc03b12d2');