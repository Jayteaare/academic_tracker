using SQLite;

namespace C868_WGU_Tallis_Jordan.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string HashedPassword { get; set; }
    }
}