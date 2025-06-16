using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DDDSample1.Application.AvailabilitySlots;
using DDDSample1.Domain.AssignedStaffs;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.HospitalPatient;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;
using DDDSample1.DTO;
using DDDSample1.Infrastructure.AssignedStaffs;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Engines;

namespace DDDSample1.Domain.HospitalAppointment
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;
        private readonly IPatientRepository _patientRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly IOperationRequestRepository _opReqRepo;
        private readonly IOperationRoomRepository _opRoomRepo;
        private readonly IAvailabilitySlotsRepository _availRepo;
        private readonly IAssignedStaffRepository _assignedRepo;

        public AppointmentService(IUnitOfWork unitOfWork, IAppointmentRepository repo, IPatientRepository patientRepo, IStaffRepository staffRepo, IOperationRoomRepository opRoomRepo, IOperationRequestRepository opReqRepo, IAvailabilitySlotsRepository availRepo, IAssignedStaffRepository assignedRepo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._patientRepo = patientRepo;
            this._staffRepo = staffRepo;
            this._opRoomRepo = opRoomRepo;
            this._opReqRepo = opReqRepo;
            this._availRepo = availRepo;
            this._assignedRepo = assignedRepo;
        }

        public AppointmentService(){}
        
        public virtual async Task<List<Appointment>> GetAllAsync()
        {
            var list = this._repo.GetAppointmentList();
            
            return list.ToList();
        }

        public virtual async Task<List<RoomAvailabilityDto>> GetAllRoomAsync()
        {
            var list = await this._availRepo.GetAllRoomsAsync();

            var dtoList = new List<RoomAvailabilityDto>();

            foreach (var slot in list)
            {
                // Get the OperationRoom asynchronously by ID
                var operationRoom = await this._opRoomRepo.GetByIdAsync(new OperationRoomId(slot.roomId.Value));

                // Create RoomAvailabilityDto and add to the list
                var dto = new RoomAvailabilityDto
                {
                    Value = slot.Value,
                    OperationRoomName = operationRoom?.Name, // Ensure null check in case operationRoom is null
                    OperationRoomID = operationRoom?.Id.Value
                };

                dtoList.Add(dto);
            }

            return dtoList;
        }

        public virtual async Task<AppointmentDto> GetByIdAsync(AppointmentID id)
        {
            var appointment = await this._repo.GetByIdAsync(id);

            return appointment == null ? null : appointment.toDto();
        }

        //Check if we require to delete the appointment from the Patient History list !!
        public async Task<bool> DeleteAsync(AppointmentID id)
        {
            var appointment = await this._repo.GetByIdAsync(id);

            this._repo.Remove(appointment);
            await this._unitOfWork.CommitAsync();

            return true;
        }

        public virtual async Task<OperationRoom> GetOperationRoom(string opRoomId){

            return await _opRoomRepo.GetRoomById(new OperationRoomId(opRoomId));
        }

        public async Task<List<OperationRoomDto>> GetOperationRooms(){
            List<OperationRoom> list = await _opRoomRepo.GetAll();
            List<OperationRoomDto> listDto =  list.ConvertAll(s => s.ToDto());
            return listDto;
        }

        public virtual async Task<AppointmentDto> registerSurgeryAppointment(string dateAndTime, string staffId, string patientNumber, string operationRoom, string requestID)
        {
            var patient = await this._patientRepo.GetByIdAsync(new MedicalRecordNumber(patientNumber));
            var request = await this._opReqRepo.GetByIdAsync(new OperationRequestId(requestID));

            var room = await this._opRoomRepo.GetByIdAsync(new OperationRoomId(operationRoom));

            AppointmentBuilder builder = new AppointmentBuilder();

            Appointment appointment =
            builder.WithDateAndTime(dateAndTime)
                .WithStatus("SCHEDULED")
                .WithPatient(patient)
                //.WithStaff(staff)
                .WithRequest(request)
                .WithRoom(room)
                .Build();

            string[] ids = staffId.Split(",");
            List<AssignedStaff> staff = new List<AssignedStaff>();
            foreach (string id in ids)
                staff.Add(await this._assignedRepo.AddAsync(new AssignedStaff(await this._staffRepo.GetStaffByLicense(new LicenseNumber(id)), appointment)));
            

            patient.AddAppointement(appointment);
            appointment.ChangeTeam(staff);
            await this._repo.AddAsync(appointment);
            await this._unitOfWork.CommitAsync();

            return appointment.toDto();
        }

        public virtual async Task<AppointmentDto> updateSurgeryAppointment(string ID, string date, string staffIDs, string roomID)
        {
            Appointment previousAppointment = await this._repo.GetAppointmentByID(new AppointmentID(ID));
            
            foreach(AssignedStaff s in previousAppointment.designedStaff){
                AssignedStaff a = await this._assignedRepo.GetByIdAsync(s.Id);
                this._assignedRepo.Remove(a);
            }

            string[] ids = staffIDs.Split(",");

            List<AssignedStaff> staff = new List<AssignedStaff>();            
            foreach (string id in ids)
                staff.Add(await this._assignedRepo.AddAsync(new AssignedStaff(await this._staffRepo.GetStaffByLicense(new LicenseNumber(id)), previousAppointment)));
            
            var room = await this._opRoomRepo.GetByIdAsync(new OperationRoomId(roomID));

            previousAppointment.ChangeTeam(staff);
            previousAppointment.ChangeDate(new DateAndTime(DateTime.ParseExact(date, "yyyy-MM-dd HH:mm", null)));
            previousAppointment.ChangeRoom(room);

            this._repo.Update(previousAppointment);
            await this._unitOfWork.CommitAsync();

            return previousAppointment.toDto();
        }

        public virtual async Task<List<StaffDto>> GetStaffByTimeSlot(string request, string dateAndTime){

            var opReq = await this._opReqRepo.GetRequestById(new OperationRequestId(request));
            var opLength = opReq.OperationType.EstimatedDuration.Duration;
            string[] parts = dateAndTime.Split(' '); 
            string[] timeParts = parts[1].Split(':');

            int hours = int.Parse(timeParts[0]);
            int minutes = int.Parse(timeParts[1]);
            int total = (hours * 60) + minutes;

            var staff = this._staffRepo.GetStaffList().ToList();
            List<StaffDto> availableStaff = new List<StaffDto>();
            
            foreach (Staff s in staff)
            {
                if(await StaffIsAvailable(s.Id, total, opLength)){
                    availableStaff.Add(s.toDto());
                }
            }
            return availableStaff;
        }

        public virtual async Task<bool> StaffIsAvailable(LicenseNumber id, int total, int opLength){
            var staffSlots = await this._availRepo.GetAllStaffSlots(id);
            if(staffSlots.Count() != 0){
                foreach (var slot in staffSlots)
                {
                    string[] time2 = slot.Value.Split(",");
                    int lowerBound = int.Parse(time2[0].Replace("(", ""));
                    int upperBound = int.Parse(time2[1].Replace(")", ""));

                    if(lowerBound <= total && upperBound >= total && ((lowerBound + opLength) <= upperBound))
                        return false;
                }
            }

            return true;
        }

        public virtual async Task<AppointmentDto> registerAppointment(AppointmentDto dto)
        {
            var patient = await this._patientRepo.GetByIdAsync(new MedicalRecordNumber(dto.patientNumber));
            var staff = await this._staffRepo.GetByIdAsync(new LicenseNumber(dto.staffId));
            var request = await this._opReqRepo.GetByIdAsync(new OperationRequestId(dto.OperationRequestId));
            var room = await this._opRoomRepo.GetByIdAsync(new OperationRoomId(dto.operationRoom));

            Appointment appointment = appointmentBuild(dto, request, patient,null, room);

            
            patient.AddAppointement(appointment);

            await this._repo.AddAsync(appointment);
            await this._unitOfWork.CommitAsync();

            return appointment.toDto();
        }

        public virtual async Task<AppointmentDto> updateAppointment(AppointmentDto dto, Appointment prev)
        {
            var patient = await this._patientRepo.GetByIdAsync(new MedicalRecordNumber(dto.patientNumber));
            var staff = await this._staffRepo.GetByIdAsync(new LicenseNumber(dto.staffId));
            var request = await this._opReqRepo.GetByIdAsync(new OperationRequestId(dto.OperationRequestId));
            var room = await this._opRoomRepo.GetByIdAsync(new OperationRoomId(dto.operationRoom));

            Appointment appointment = appointmentBuild(dto, request, patient,null, room);

            

            patient.UpdateAppointment(prev,appointment);

            this._repo.Update(appointment);
            await this._unitOfWork.CommitAsync();

            return appointment.toDto();
        }
        
        public virtual async Task<AppointmentDto> GetByRoomID(string id)
        {
            var appointments = _repo.GetByRoomID(new OperationRoomId(id));
            var roomSlots = _availRepo.GetByRoomID(new OperationRoomId(id)).ToList();
            // Get current system time
            // Find the current appointment
            var currentAppointment = appointments.FirstOrDefault(app =>
            {
                DateTime startTime = app.dateAndTime.DateTime;
                DateTime endTime = startTime.AddMinutes(app.request.OperationType.EstimatedDuration.Duration);

                foreach (var slot in roomSlots)
                {
                    string[] time2 = slot.Value.Split(",");
                    int lowerBound = int.Parse(time2[0].Replace("(", ""));
                    int upperBound = int.Parse(time2[1].Replace(")", ""));

                    if((startTime.Hour*60 + startTime.Minute) >= lowerBound && (endTime.Hour*60 + endTime.Minute) <= upperBound){
                        return true;
                    }
                }

                return false;
            });

            return currentAppointment == null ? null : currentAppointment.toDto();
        }

        private Appointment appointmentBuild(AppointmentDto dto,OperationRequest request, Patient patient, List<Staff> staff, OperationRoom room){
            AppointmentBuilder builder = new AppointmentBuilder();

            Appointment appointment =
            builder.WithDateAndTime(dto.dateAndTime)
                .WithStatus(dto.appoitmentStatus)
                .WithPatient(patient)
                //.WithStaff(staff)
                .WithRequest(request)
                .WithRoom(room)
                .Build();

            return appointment;
        }

        public async Task<ActionResult<SchedulingResult>> ScheduleAppointmentForRoomDay(List<StaffDto> staffList, List<OperationRequestDTO> opList, List<OperationRoom> opRooms, string day)
        {
            SchedulingAdapter adapter = new SchedulingAdapter();
        
            SchedulingResult results = adapter.ScheduleOperations(staffList, opList, opRooms, day);
            AvailabilitySlot slot = new AvailabilitySlot();

            Appointment appointment;
            foreach (var roomSlotKey in results.RoomAgenda.Keys)
            {
                OperationRoom room = opRooms.Find(r => r.Name == roomSlotKey);
                if (room != null)
                {
                    var slots = results.RoomAgenda[roomSlotKey];
                    foreach (var innerSlot in slots)
                    {
                        slot.roomId = room.Id;
                        slot.opRoom = room;

                        slot.Value = innerSlot.ToString();
                        slot = new AvailabilitySlot(Guid.NewGuid().ToString());
                        await _availRepo.AddAsync(slot);
                        await this._unitOfWork.CommitAsync();
                    }
                }
            }


            foreach (var staffSlotKey in results.StaffAgenda.Keys)
            {
                StaffDto staff = staffList.Find(r => r.LicenseNumber == staffSlotKey.ToString());
                Staff theStaff = await _staffRepo.GetByIdAsync(new LicenseNumber(staff.LicenseNumber));
                if (staff != null)
                {
                    var slots = results.StaffAgenda[staffSlotKey];
                    foreach (var innerSlot in slots)
                    {
                        slot.StaffId = theStaff.Id;
                        slot.theStaff = theStaff;

                        slot.Value = innerSlot.ToString();
                        slot = new AvailabilitySlot(Guid.NewGuid().ToString());
                        await _availRepo.AddAsync(slot);
                        await this._unitOfWork.CommitAsync();
                    }
                }
            }

            return results;

            }

            
        }
    }

