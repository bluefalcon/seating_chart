using SeatingAssignments.Service;

namespace SeatingAssignments.UnitTests
{
  public class AssertExtensions : Xunit.Assert
  {
    public static bool IsAllSeatsNull(Seat[,] seats)
    {
      for (var row = 0; row < seats.GetLength(0); row++)
      {
        for (var col = 0; col < seats.GetLength(1); col++)
        {
          if (seats[row, col] != null) return false;
        }
      }

      return true;
    }
  }
}
