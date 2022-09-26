using SeatingAssignments.Service;

namespace SeatingAssignments.Extensions
{
  public static class SeatingChartArrayExtensions
  {
    public static int TotalStudents(this Seat[,] seats)
    {
      var totalStudents = 0;
      for (var row = 0; row < seats.GetLength(0); row++)
      {
        for (var col = 0; col < seats.GetLength(1); col++)
        {
          if (seats[row, col] != null && !seats[row, col].Available) totalStudents++;
        }
      }

      return totalStudents;
    }

    public static void InitWithEmptySeats(this Seat[,] seats)
    {
      for (var row = 0; row < seats.GetLength(0); row++)
      {
        for (var col = 0; col < seats.GetLength(1); col++)
        {
          seats[row, col] = new Seat { Row = row + 1, Column = col + 1 };
        }
      }
    }
  }
}
