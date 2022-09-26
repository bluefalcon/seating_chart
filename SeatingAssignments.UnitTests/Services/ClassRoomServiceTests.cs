using Moq;
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
      AssertExtensions.IsAllSeatsAvailable(result.Seats);
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_Valid_3x3()
    {
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var result = await _sut.CreateSeatingChartAsync(1, 3, 3);

      Assert.Equal("Billy Zinger", result.Seats[0,2].FullName);
      Assert.True(result.Seats[0, 1].Available);
      Assert.Equal("Johnny Rocket", result.Seats[0, 0].FullName);
      Assert.True(result.Seats[1, 2].Available);
      Assert.Equal("John Doe", result.Seats[1, 1].FullName);
      Assert.True(result.Seats[1, 0].Available);
      Assert.Equal("Foo Bar", result.Seats[2, 2].FullName);
      Assert.True(result.Seats[2, 1].Available);
      Assert.Equal("Red Alpha", result.Seats[2, 0].FullName);
      Assert.Equal(1, result.Period);
      Assert.Equal(3, result.TotalColumns);
      Assert.Equal(3, result.TotalRows);
    }

    [Fact]
    public async Task CreateSeatingChartAsync_When_Valid_3x2()
    {
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var result = await _sut.CreateSeatingChartAsync(1, 3, 2);

      Assert.Equal("Billy Zinger", result.Seats[0, 1].FullName);
      Assert.Equal("Johnny Rocket", result.Seats[0, 0].FullName);
      Assert.Equal("John Doe", result.Seats[1, 1].FullName);
      Assert.Equal("Foo Bar", result.Seats[1, 0].FullName);
      Assert.Equal("Red Alpha", result.Seats[2, 1].FullName);
      Assert.True(result.Seats[2, 0].Available);
      Assert.Equal(1, result.Period);
      Assert.Equal(2, result.TotalColumns);
      Assert.Equal(3, result.TotalRows);
    }

    [Fact]
    public void GenerateSeatingChartDisplayText_Valid_3x2()
    {
      var expected =
        "Seating Chart for Period: 0\r\nTotal Students: 5\r\n\r\n********** Front Of Classroom **********\r\n\r\n-                        Billy Zinger             \r\nJohn Doe                 Johnny Rocket            \r\nRed Alpha                Foo Bar                  \r\n";
      _classroomRepository.Setup(m => m.GetStudentsForPeriodAsync(1))
        .Returns(Task.FromResult(_students));

      var seatingChart = new SeatingChartModel(3, 2);

      seatingChart.Seats[0, 1] = new Seat { FirstName = "Billy", LastName = "Zinger", Column = 2, Row = 1 };
      seatingChart.Seats[0, 0] = new Seat { Column = 1, Row = 1 };
      seatingChart.Seats[1, 1] = new Seat { FirstName = "Johnny", LastName = "Rocket", Column = 2, Row = 2 };
      seatingChart.Seats[1, 0] = new Seat { FirstName = "John", LastName = "Doe", Column = 1, Row = 2 };
      seatingChart.Seats[2, 1] = new Seat { FirstName = "Foo", LastName = "Bar", Column = 2, Row = 3 };
      seatingChart.Seats[2, 0] = new Seat { FirstName = "Red", LastName = "Alpha", Column = 1, Row = 3 };

      var result = _sut.GenerateSeatingChartDisplayText(seatingChart);

      Assert.Equal(expected, result);
    }
  }
}