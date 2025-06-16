using System;

namespace DDDSample1.Domain.HospitalAppointment
{
    public class SurgeryRoom
    {
        public string Name { get; private set; }

        public SurgeryRoom(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            
            Name = name;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Name cannot be null or empty.", nameof(newName));
            
            Name = newName;
        }
    }
}
