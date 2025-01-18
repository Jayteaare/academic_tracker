using System;
using C868_WGU_Tallis_Jordan.Models;
using SQLite;

namespace C868_WGU_Tallis_Jordan.Models
{
    public class Term : BaseEntity
    {
        public string TermName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}