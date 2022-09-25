using SeatingAssignments.Service;

namespace SeatingAssignments.Extensions
{
  public static class SeatingChartArrayExtensions
  {
    public static int TotalStudents(this Seat[,] seatingChart)
    {
      var totalStudents = 0;
      for (var row = 0; row < seatingChart.GetLength(0); row++)
      {
        for (var col = 0; col < seatingChart.GetLength(1); col++)
        {
          if (seatingChart[row, col] != null && !seatingChart[row, col].Available) totalStudents++;
        }
      }

      return totalStudents;
    }
  }
}
