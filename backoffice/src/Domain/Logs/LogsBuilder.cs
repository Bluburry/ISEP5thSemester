using System;
using System.Collections.Generic;
using DDDSample1.Domain.ContactInformations;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Specializations;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.ValueObjects;

namespace DDDSample1.Domain.Logs

{
    public class LogsBuilder
    {
        private ObjectLoggedType _loggedType;
        private string _loggedInformation;
        private DateAndTime _loggedDate;
        private string _id;

        public LogsBuilder WithObjectType(string objectType)
        {
            _loggedType = (ObjectLoggedType)Enum.Parse(typeof(ObjectLoggedType), objectType);
            return this;
        }

        public LogsBuilder WithInformation(string information)
        {
            _loggedInformation = information;
            return this;
        }

        public LogsBuilder WithDateAndTime(DateAndTime dateTime)
        {
            _loggedDate = dateTime;
            return this;
        }

        public LogsBuilder WithDateAndTime()
        {
            _loggedDate = new DateAndTime(DateTime.Now.AddSeconds(30));
            return this;
        }


        public LogsBuilder WithID(string id)
        {
            _id = id;
            return this;
        }

        public Log Build()
        {
            if (_loggedType.Equals(null))
                throw new ArgumentException("Object Type is required.");
            if (_loggedInformation == null)
                throw new ArgumentException("Information is required to log.");
            if (_loggedDate == null)
                throw new ArgumentException("Date and Time are required.");
            if (_id == null)
                throw new ArgumentException("ID of the object logged is needed");
            
            var shadowTable = new Log(_loggedDate, _loggedInformation, _loggedType, _id);


            return shadowTable;
        }
    }
}
