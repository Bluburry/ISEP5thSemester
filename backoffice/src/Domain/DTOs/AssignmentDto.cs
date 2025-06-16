using System.Collections.Generic;

namespace DDDSample1.DTO{
    public class AssignmentsDto
{
    // Property for RequestId
    public string RequestId { get; set; }

    public string OperationRoom { get; set; }   

    // Property for Assignees (array of strings)
    public List<string> Assignees { get; set; }

    // Property for TimeInterval
    public string TimeInterval { get; set; }

    // Optional constructor to initialize the properties
    public AssignmentsDto(string requestId, List<string> assignees, string timeInterval, string opRoom)
    {
        RequestId = requestId;
        Assignees = assignees;
        TimeInterval = timeInterval;
        OperationRoom = opRoom;
    }

    public AssignmentsDto()
    {
        Assignees = new List<string>();
    }

    public void AddAssignee(string ass){
        Assignees.Add(ass);
    }
}
}
