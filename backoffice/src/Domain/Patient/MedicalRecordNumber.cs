using System;
using DDDSample1.Domain.Shared;
using Newtonsoft.Json;

namespace DDDSample1.Domain.HospitalPatient
{
    public class MedicalRecordNumber : EntityId
    {
        private static int _sequentialNumber = 0;  

        [JsonConstructor]
        public MedicalRecordNumber(string value) : base(value)
        {
        }

        // Generates a new Medical Record Number in the format "YYYYMMnnnnnn"
        public static MedicalRecordNumber GenerateMRN()
        {
            string yearMonth = DateTime.Now.ToString("yyyyMM");
            string sequential = GenerateSequentialNumber().ToString("D6");
            string newId = yearMonth + sequential;
            
            return new MedicalRecordNumber(newId);
        }

        private static int GenerateSequentialNumber()
        {
            // It still requires to retrieve and increment a value stored persistently
            return ++_sequentialNumber;
        }

        override
        protected Object createFromString(String text)
        {
            return text;
        }

        override
        public String AsString()
        {
            return base.ObjValue.ToString();
        }
    }
}