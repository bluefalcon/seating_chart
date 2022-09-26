using SeatingAssignments.Service;
using Xunit.Sdk;

namespace SeatingAssignments.UnitTests
{
  public class AssertExtensions : Xunit.Assert
  {
    public static bool IsAllSeatsAvailable(Seat[,] seats)
    {
      var isAvailable = true;
      for (var row = 0; row < seats.GetLength(0); row++)
      {
        for (var col = 0; col < seats.GetLength(1); col++)
        {
          if (!seats[row, col].Available) throw new TrueException(null,null);
        }
      }

      return true;
    }
  }
}
