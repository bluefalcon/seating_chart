using Moq;
using System.Text.Json;
using SeatingAssignments.Data;
using SeatingAssignments.Service;

namespace SeatingAssignments.UnitTests.Services
{
  public class ClassRoomServiceTests
  {
    private readonly IClassroomService _sut;
    private readonly Mock<IClassroomRepository> _classroomRepository;
    private readonly IEnumerable<ClassroomEntity> _students;

    public ClassRoomServiceTests()
    {
      _classroomRepository = new Mock<IClassroomRepository>();
      _sut = new ClassroomService(_classroomRepository.Object);
      _students = new List<ClassroomEntity>()
      {
        new ClassroomEntity { FirstName = "Billy", LastName = "Zinger", Period = 1 },
        new ClassroomEntity { FirstName = "Johnny", LastName = "Rocket", Period = 1 },
        new ClassroomEntity { FirstName = "Foo", LastName = "Bar", Period = 1 },
        new ClassroomEntity { FirstName = "Red", LastName = "Alpha", Period = 1 },
        new ClassroomEntity { FirstName = "John", LastName = "Doe", Period = 1 }
      };
    }

    [Theory]
    [InlineData(-1, 1, 1)]
    [InlineData(1, -1, 1)]
    [InlineData(1, 1, -1)]
    public async Task CreateSeatingChartAsync_When_Invalid_Argument_ThrowArgumentException(int period, int rows,
      int columns)
    {
      await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateSeatingChartAsync(period, rows, columns));
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_NotEnoughSeats_ThrowException()
    {
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      await Assert.ThrowsAsync<Exception>(() => _sut.CreateSeatingChartAsync(1, 2, 2));
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_NoStudentsForPeriod_Return_Empty_SeatingChartModel()
    {
      IEnumerable<ClassroomEntity> students = new List<ClassroomEntity>();

      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(students));

      var result = await _sut.CreateSeatingChartAsync(1, 2, 3);

      Assert.Equal(1, result.Period);
      Assert.Equal(2, result.TotalRows);
      Assert.Equal(3, result.TotalColumns);
      Assert.Empty(result.Seats);
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_Valid_3x3()
    {
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var seats = new List<Seat>()
      {
        new Seat { FirstName = "Billy", LastName = "Zinger", Number = 3, Row = 1 },
        new Seat { Number = 2, Row = 1 },
        new Seat { FirstName = "Johnny", LastName = "Rocket", Number = 1, Row = 1 },
        new Seat { Number = 3, Row = 2 },
        new Seat { FirstName = "John", LastName = "Doe", Number = 2, Row = 2 },
        new Seat { Number = 1, Row = 2 },
        new Seat { FirstName = "Foo", LastName = "Bar", Number = 3, Row = 3 },
        new Seat { Number = 2, Row = 3 },
        new Seat { FirstName = "Red", LastName = "Alpha", Number = 1, Row = 3 },
      };

      var result = await _sut.CreateSeatingChartAsync(1, 3, 3);

      var strResult = JsonSerializer.Serialize(result.Seats);
      var strExpected = JsonSerializer.Serialize(seats);
      Assert.Equal(strExpected, strResult);
      Assert.Equal(1, result.Period);
      Assert.Equal(3, result.TotalColumns);
      Assert.Equal(3, result.TotalRows);
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_Valid_3x2()
    {
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var seats = new List<Seat>()
      {
        new Seat { FirstName = "Billy", LastName = "Zinger", Number = 2, Row = 1 },
        new Seat { Number = 1, Row = 1 },
        new Seat { FirstName = "Johnny", LastName = "Rocket", Number = 2, Row = 2 },
        new Seat { FirstName = "John", LastName = "Doe", Number = 1, Row = 2 },
        new Seat { FirstName = "Foo", LastName = "Bar", Number = 2, Row = 3 },
        new Seat { FirstName = "Red", LastName = "Alpha", Number = 1, Row = 3 },
      };

      var result = await _sut.CreateSeatingChartAsync(1, 3, 2);

      var strResult = JsonSerializer.Serialize(result.Seats);
      var strExpected = JsonSerializer.Serialize(seats);
      Assert.Equal(strExpected, strResult);
      Assert.Equal(1, result.Period);
      Assert.Equal(2, result.TotalColumns);
      Assert.Equal(3, result.TotalRows);
    }

    [Fact]
    public void CreateSeatingChartDisplay_Valid_3x2()
    {
      var expected = "Seating Chart for Period: 0\r\nTotal Students: 5\r\n\r\nX               Billy Zinger    \r\nJohn Doe        Johnny Rocket   \r\nRed Alpha       Foo Bar         \r\n";
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var seatingChart = new SeatingChartModel(3, 2)
      {
        Seats = new List<Seat>()
        {
          new Seat { FirstName = "Billy", LastName = "Zinger", Number = 2, Row = 1 },
          new Seat { Number = 1, Row = 1 },
          new Seat { FirstName = "Johnny", LastName = "Rocket", Number = 2, Row = 2 },
          new Seat { FirstName = "John", LastName = "Doe", Number = 1, Row = 2 },
          new Seat { FirstName = "Foo", LastName = "Bar", Number = 2, Row = 3 },
          new Seat { FirstName = "Red", LastName = "Alpha", Number = 1, Row = 3 },
        }
      };

      var result = _sut.CreateSeatingChartDisplay(seatingChart);

      Assert.Equal(expected, result);
    }
  }
}