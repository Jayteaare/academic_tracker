using SQLite;
using System;

namespace C868_WGU_Tallis_Jordan.Models
{
    public class Course : BaseEntity
    {
        public int TermId { get; set; }

        public string CourseName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string InstructorName { get; set; } = string.Empty;

        public string InstructorPhone { get; set; } = string.Empty;

        public string InstructorEmail { get; set; } = string.Empty;

        public string Notes { get; set; } = string.Empty;

        public bool Notifications { get; set; }

        public string NotificationsText => Notifications ? "Enabled" : "Disabled";
    }
}