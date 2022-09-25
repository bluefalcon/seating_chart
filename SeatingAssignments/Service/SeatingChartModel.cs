
namespace SeatingAssignments.Service
{
  public class Seat
  {
    public int Row { get; set; }
    public int Number { get; set; }
    public string SeatNumber => $"{Row}, {Number}";
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public bool Available => String.IsNullOrEmpty(FirstName) && String.IsNullOrEmpty(LastName);
  }
  public class SeatingChartModel
  {
    public SeatingChartModel(int rows, int columns)
    {
      TotalRows = rows;
      TotalColumns = columns;
      Seats = new List<Seat>();
    }
    public int Period { get; set; }
    public int TotalRows { get; }
    public int TotalColumns { get; }
    public List<Seat> Seats { get; set; }
  }
}
