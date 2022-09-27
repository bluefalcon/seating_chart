namespace SeatingAssignments.Models
{
    public class SeatModel
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public bool Available => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName);
    }
}
