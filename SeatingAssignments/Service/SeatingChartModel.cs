
namespace SeatingAssignments.Service
{
  public class Seat
  {
    public int Row { get; set; }
    public int Column { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public bool Available => string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName);
  }
  public class SeatingChartModel
  {
    public SeatingChartModel(int rows, int columns)
    {
      TotalRows = rows;
      TotalColumns = columns;
      Seats = new Seat[rows, columns];
    }
    public int Period { get; set; }
    public int TotalRows { get; }
    public int TotalColumns { get; }
    public Seat[,] Seats { get; set; }
  }
}
