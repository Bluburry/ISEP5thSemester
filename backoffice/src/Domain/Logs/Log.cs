using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Tokens;
using DDDSample1.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;


namespace DDDSample1.Domain.Logs
{
    public class Log : Entity<LogId>, IAggregateRoot
    {
        public ObjectLoggedType loggedType {get; set;}
        public string LoggedInformation { get; set;}
        public DateAndTime LoggedDate { get; set;}
        public string LoggedId {get; set;}

        public Log(DateAndTime date, string log, ObjectLoggedType type, string Id)
        {
            loggedType = type;
            LoggedDate = date;
            LoggedInformation = log;
            LoggedId = Id;
            this.Id = new LogId(Guid.NewGuid());
        }

        public Log()
        {
            //for ORM
        }

        public LogDto toDto()
        {
            return new LogDto(
                LoggedId,
                LoggedInformation,
                loggedType.ToString(),
                LoggedDate.ToString() // Assuming DateAndTime has a ToString() implementation
            );
        }




    }
}
