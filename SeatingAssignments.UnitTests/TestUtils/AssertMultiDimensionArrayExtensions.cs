using SeatingAssignments.Models;
using Xunit.Sdk;

namespace SeatingAssignments.UnitTests.TestUtils
{
  public class AssertExtensions : Assert
    {
        public static bool IsAllSeatsAvailable(SeatModel[,] seats)
        {
            for (var row = 0; row < seats.GetLength(0); row++)
            {
                for (var col = 0; col < seats.GetLength(1); col++)
                {
                    if (!seats[row, col].Available) throw new TrueException(null, null);
                }
            }

            return true;
        }
    }
}
