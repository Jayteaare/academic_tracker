using SQLite;
using System;

namespace C868_WGU_Tallis_Jordan.Models
{
    public class Assessment : BaseEntity
    {

        public int CourseId { get; set; }

        public string AssessName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Type { get; set; } = string.Empty;

        public bool Notifications { get; set; }

        public string NotificationsText => Notifications ? "Enabled" : "Disabled";
    }
}