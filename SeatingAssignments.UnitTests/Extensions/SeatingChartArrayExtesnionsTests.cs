using SeatingAssignments.Extensions;
using SeatingAssignments.Service;

namespace SeatingAssignments.UnitTests.Extensions
{
  public class SeatingChartArrayExtesnionsTests
  {

    [Fact]
    public void TotalStudents_Zero()
    {
      var seats = new Seat[2, 2];
      Assert.Equal(0, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_FirstAndLastName()
    {
      var seats = new Seat[2, 2];
      seats[0, 0] = new Seat { FirstName = "Bob", LastName = "Boo" };
      seats[0, 1] = new Seat { FirstName = "Bill", LastName = "Anderson" };
      Assert.Equal(2, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_FirstName()
    {
      var seats = new Seat[2, 2];
      seats[0, 0] = new Seat { FirstName = "Bob" };
      seats[0, 1] = new Seat { FirstName = "Bill" };
      Assert.Equal(2, seats.TotalStudents());
    }

    [Fact]
    public void TotalStudents_LastName()
    {
      var seats = new Seat[2, 2];
      seats[0, 0] = new Seat { LastName = "Boo" };
      seats[0, 1] = new Seat { LastName = "Anderson" };
      Assert.Equal(2, seats.TotalStudents());
    }

  }
}
