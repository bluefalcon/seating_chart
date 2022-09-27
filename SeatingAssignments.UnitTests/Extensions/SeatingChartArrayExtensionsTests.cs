using SeatingAssignments.Extensions;
using SeatingAssignments.Models;
using SeatingAssignments.UnitTests.TestUtils;

namespace SeatingAssignments.UnitTests.Extensions
{
    public class SeatingChartArrayExtensionsTests
  {
    [Fact]
    public void InitWithEmptySeats_AllAvailable()
    {
      var seats = new SeatModel[5, 5];
      seats.InitWithEmptySeats();
      AssertExtensions.IsAllSeatsAvailable(seats);
    }

    [Fact]
    public void TotalStudents_Zero()
    {
      var seats = new SeatModel[2, 2];
      Assert.Equal(0, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_FirstAndLastName()
    {
      var seats = new SeatModel[2, 2];
      seats[0, 0] = new SeatModel { FirstName = "Bob", LastName = "Boo" };
      seats[0, 1] = new SeatModel { FirstName = "Bill", LastName = "Anderson" };
      Assert.Equal(2, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_FirstName()
    {
      var seats = new SeatModel[2, 2];
      seats[0, 0] = new SeatModel { FirstName = "Bob" };
      seats[0, 1] = new SeatModel { FirstName = "Bill" };
      Assert.Equal(2, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_LastName()
    {
      var seats = new SeatModel[2, 2];
      seats[0, 0] = new SeatModel { LastName = "Boo" };
      seats[0, 1] = new SeatModel { LastName = "Anderson" };
      Assert.Equal(2, seats.TotalStudents());
    }

  }
}
