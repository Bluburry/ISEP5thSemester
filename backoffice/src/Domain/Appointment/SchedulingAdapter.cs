using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DDDSample1.Domain.AvailabilitySlots;
using DDDSample1.Domain.HospitalStaff;
using DDDSample1.Domain.OperationRequests;
using DDDSample1.Domain.OperationRooms;
using DDDSample1.DTO;

public class SchedulingAdapter
{
    private static readonly string prologFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, 
        "../../../../../scheduling/GeneticPlanningBase.pl"
    );
    public SchedulingAdapter()
    {
        // Constructor logic if needed
    }

    /// <summary>
    /// Schedules operations for the given staff, operation requests, and operation room on a specific day.
    /// </summary>
    /// <param name="staffList">List of staff members available for scheduling.</param>
    /// <param name="opList">List of operation requests to be scheduled.</param>
    /// <param name="opRoom">The operation room where the operations will occur.</param>
    /// <param name="day">The day for which to schedule the operations (in string format).</param>
    /// <returns>List of scheduled operations or some result object.</returns>
    public SchedulingResult ScheduleOperations(
        List<StaffDto> staffList,
        List<OperationRequestDTO> opList,
        List<OperationRoom> opRooms,
        string day
        )
    {
        
        // Read the content of the Prolog file
        string fileContentRaw = File.ReadAllText(prologFilePath);

        // Split content by lines
        var lines = fileContentRaw.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        // Find the index where "%DATA" is located
        int dataIndex = lines.FindIndex(line => line.Contains("%DATA"));

        if (dataIndex == -1)
        {
            throw new InvalidOperationException("No '%DATA' marker found in the Prolog file.");
        }

        // Prepare the Prolog data to be inserted
        List<string> prologData = new List<string>();
        List<string> staffProfileData = new List<string>();
        List<string> staffTimetableData = new List<string>();
        List<string> scheduleData = new List<string>();
        List<string> assignmentData = new List<string>();

        // Add Prolog facts for surgery, staff, timetable, and assignments
        foreach (var staff in staffList)
        {
            string scheduleString = string.Join(", ", staff.AvailabilitySlots ?? new List<string>());
            string staffProfile, staffTimetable;

            if (staff.Specialization.ToUpper().Contains("CLEANER"))
            {
                staffProfile = $"staff({staff.LicenseNumber}, cleaner, cleaner, []).";
                staffTimetable = $"timetable({staff.LicenseNumber}, {day}, (400, 1400)).";
            }
            else if (staff.Specialization.ToUpper().Contains("ANAESTHESIST"))
            {
                staffProfile = $"staff({staff.LicenseNumber}, anaesthesist, anaesthesist, []).";
                staffTimetable = $"timetable({staff.LicenseNumber}, {day}, (200, 1200)).";
            }
            else
            {
                staffProfile = $"staff({staff.LicenseNumber}, doctor, doctor, []).";
                staffTimetable = $"timetable({staff.LicenseNumber}, {day}, (300, 1300)).";
            }

            staffProfileData.Add(staffProfile);
            staffTimetableData.Add(staffTimetable);

            var schedule = $"agenda_staff({staff.LicenseNumber}, {day}, [{scheduleString}]).";
            scheduleData.Add(schedule);

            
            //assignmentData.Add($"assignment_surgery(s0, {staff.LicenseNumber}).");
        }

        string surgeryId;
        List<AssignmentsDto> assignments = new List<AssignmentsDto>();
        foreach(var opDto in opList){
            AssignmentsDto ass = new AssignmentsDto();
            ass.RequestId = opDto.ID;
            foreach(var requiredSpec in opDto.RequiredSpecialists){
                string specialization = requiredSpec.Split(':')[1].Split(',')[0].Trim();
                foreach(var staff in staffList){
                    if(staff.Specialization == specialization){
                        ass.AddAssignee(staff.LicenseNumber);
                        surgeryId = opDto.ID.Split('-')[0];
                        assignmentData.Add($"assignment_surgery({"a" + surgeryId}, {staff.LicenseNumber}).");
                    }
                }
            }
            assignments.Add(ass);
        }

        var uniqueEntries = new HashSet<string>();

        foreach (var opDto in opList)
        {
            var entry = $"surgery({opDto.OperationType}, 20, 30, 20).";
            if (uniqueEntries.Add(entry)) // Add returns false if the entry already exists
            {
                prologData.Add(entry);
            }
        }

            
            int count=0;
        foreach (var opDto in opList)
            {
                surgeryId = opDto.ID.Split('-')[0];
                prologData.Add($"surgery_id({"a"+surgeryId}, {opDto.OperationType}).");
            }  
        
        prologData.AddRange(staffProfileData);
        prologData.AddRange(staffTimetableData);
        prologData.AddRange(scheduleData);
        prologData.AddRange(assignmentData);

        foreach (OperationRoom opRoom in opRooms){
        string scheduleString = string.Join(", ", opRoom.AvailabilitySlots != null ? opRoom.AvailabilitySlots : new List<AvailabilitySlot>());

        prologData.Add($"agenda_operation_room({opRoom.Name.ToLower()}, {day}, [{scheduleString}]).");
        }
        // Add Prolog code for operation room and heuristic query

        //prologData.Add($":-busiest_doctor_heuristic({opRoom.Id}, {day})");

        // Insert the Prolog code after the "%DATA" line
        lines.InsertRange(dataIndex + 1, prologData);

        // Write the modified content back to the Prolog file
        File.WriteAllLines(prologFilePath, lines);

        int countRoom = opRooms.Count;

        string output = RunPrologProcess(day, countRoom);

        var staffAgenda = new Dictionary<int, List<(int, int)>>();
        
    var roomAgenda = new Dictionary<string, List<(int, int)>>();

    // Split the string into lines
    var outLines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

    // Parse each line based on its context
    bool isStaffSection = false;
    bool isRoomSection = false;

    foreach (var line in outLines)
    {
        if (line.Contains("Agenda Staff Information:"))
        {
            isStaffSection = true;
            isRoomSection = false;
            continue;
        }
        if (line.Contains("Agenda Operation Room Information:"))
        {
            isStaffSection = false;
            isRoomSection = true;
            continue;
        }

        if (isStaffSection && !string.IsNullOrWhiteSpace(line))
        {
            // Parse staff agenda line
            var match = Regex.Match(line, @"(\d+),\d+,(\[.*\])");
            if (match.Success)
            {
                int staffId = int.Parse(match.Groups[1].Value);
                var slots = ParseIntervals(match.Groups[2].Value);
                staffAgenda[staffId] = slots;
            }
        }
        else if (isRoomSection && !string.IsNullOrWhiteSpace(line))
        {
            // Parse operation room agenda line
            var match = Regex.Match(line, @"([a-zA-Z]+),\d+,(\[.*\])");
            if (match.Success)
            {
                string roomId = match.Groups[1].Value;
                var slots = ParseIntervals(match.Groups[2].Value);
                roomAgenda[roomId] = slots;
            }
        }
    }


        string pattern = @"\(([^)]+)\)";  // Match content inside parentheses
        var matches = Regex.Matches(output, pattern);

        string[] result = matches.Cast<Match>()
                                 .Select(m => m.Groups[1].Value)
                                 .ToArray();

        foreach (var opDto in opList)
            {
                surgeryId = opDto.ID.Split('-')[0];
                foreach (var interval in result){
                    if(interval.Contains(surgeryId)){
                        foreach(var assignment in assignments){
                            if(assignment.RequestId == opDto.ID){
                                int secondCommaIndex = interval.IndexOf(',', interval.IndexOf(',') + 1);
                                string formattedInterval = interval.Substring(0, secondCommaIndex); 

                                assignment.TimeInterval = formattedInterval;
                            }
                        }
                    }
                }
            }  

        // Read the file again
        string updatedContent = File.ReadAllText(prologFilePath);
        

        // Find the positions of %DATA and %EOD
        dataIndex = updatedContent.IndexOf("%DATA");
        int eodIndex = updatedContent.IndexOf("%EOD");

        // Check if both markers exist
        if (dataIndex != -1 && eodIndex != -1 && eodIndex > dataIndex)
        {
            // Keep everything before %DATA and after %EOD
            string beforeData = updatedContent.Substring(0, dataIndex + "% DATA".Length);
            string afterEod = updatedContent.Substring(eodIndex);

            // Create the cleaned-up content
            string cleanedContent = beforeData + "\n\n" + afterEod; // Add two newlines between % DATA and % EOD

            // Write the cleaned content back to the file
            File.WriteAllText(prologFilePath, cleanedContent);
        }

        return new SchedulingResult
        {
            StaffAgenda = staffAgenda,
            RoomAgenda = roomAgenda
        };
    }

    private string RunPrologProcess(string day, int count)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = "swipl",
            Arguments = $"-q -s {prologFilePath} -g \"schedule_all_rooms({day}), print_all_agendas, halt.\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true, // Enable input redirection
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
            // Write input to the Prolog process

            // Capture the output and error streams
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new InvalidOperationException($"Prolog process error: {error}");
            }

            return output;
        }
    }
    private List<(int, int)> ParseIntervals(string intervalString)
    {
        var intervals = new List<(int, int)>();
        var matches = Regex.Matches(intervalString, @"\((\d+),(\d+)\)");

        foreach (Match match in matches)
        {
            int start = int.Parse(match.Groups[1].Value);
            int end = int.Parse(match.Groups[2].Value);
            intervals.Add((start, end));
        }

        return intervals;
    }
}

public class SchedulingResult
{
    public Dictionary<int, List<(int, int)>> StaffAgenda { get; set; }
    public Dictionary<string, List<(int, int)>> RoomAgenda { get; set; }
}
