using System.Text;
using SeatingAssignments.Data;
using SeatingAssignments.Extensions;

namespace SeatingAssignments.Service
{
  public interface IClassroomService
  {
    Task<SeatingChartModel> CreateSeatingChartAsync(int period, int rows, int columns);
    string GenerateSeatingChartDisplayText(SeatingChartModel seatingChart);
  }

  public class ClassroomService : IClassroomService
  {
    private readonly IClassroomRepository _classroomRepository;

    public ClassroomService(IClassroomRepository classroomRepository)
    {
      _classroomRepository = classroomRepository;
    }

    public async Task<SeatingChartModel> CreateSeatingChartAsync(int period, int rows, int columns)
    {
      if (period <= 0) throw new ArgumentException("Period less than equal To 0", nameof(period));
      if (rows <= 0) throw new ArgumentException("Rows Less than Equal To 0", nameof(rows));
      if (columns <= 0) throw new ArgumentException("Columns Less Than Equal To 0", nameof(columns));

      var seatingChartResult = new SeatingChartModel(rows, columns)
      {
        Period = period
      };
      var students = await _classroomRepository.GetStudentsForPeriodAsync(period);
      var sortedStudents = students.ToList().OrderByDescending(x => x.LastName).ToList();

      var totalStudents = sortedStudents.Count();
      var totalSeats = (rows * columns);
      if (totalStudents <= 0) return seatingChartResult;
      if (totalStudents > totalSeats)
        throw new Exception($"Total Students: {totalStudents} exceeds Total Seats: {totalSeats}");

      var totalSpacesAvailable = totalSeats - totalStudents;
      var currentStudent = 0;

      Func<int, bool> outOfStudentsFunc = (currentPosition) => currentPosition >= sortedStudents.Count;

      for (var row = 1; row <= rows; row++)
      {
        var isEvenRow = row % 2 == 0;
        var workingColumn = 1;
        for (var col = columns; col > 0; col--)
        {
          var isEvenCol = workingColumn % 2 == 0;
          var addEvenRowStudent = isEvenRow && isEvenCol && totalSpacesAvailable > 0 || totalSpacesAvailable == 0;
          var addOddRowStudent = !isEvenRow && !isEvenCol && totalSpacesAvailable > 0 || totalSpacesAvailable == 0;
          if (addEvenRowStudent && !outOfStudentsFunc(currentStudent) ||
              addOddRowStudent && !outOfStudentsFunc(currentStudent))
          {
            var seat = new Seat
            {
              FirstName = sortedStudents[currentStudent].FirstName,
              LastName = sortedStudents[currentStudent].LastName,
              Column = col,
              Row = row
            };
            seatingChartResult.Seats[row - 1, col - 1] = seat;
            currentStudent++;
          }
          else
          {
            seatingChartResult.Seats[row - 1, col - 1] = new Seat { Column = col, Row = row };
            if (totalSpacesAvailable > 0) totalSpacesAvailable--;
          }

          workingColumn++;
        }
      }

      return seatingChartResult;
    }

    public string GenerateSeatingChartDisplayText(SeatingChartModel seatingChart)
    {
      var result = new StringBuilder();
      result.AppendLine($"Seating Chart for Period: {seatingChart.Period}");
      var totalStudents = seatingChart.Seats.TotalStudents();
      result.AppendLine($"Total Students: {totalStudents}");
      if (totalStudents <= 0) return result.ToString();
      result.AppendLine();
      result.AppendLine("********** Insert Teacher, Desk, Etc.. Here **********");
      result.AppendLine();
      for (var row = 0; row < seatingChart.Seats.GetLength(0); row++)
      {
        for (var col = 0; col < seatingChart.Seats.GetLength(1); col++)
        {
          var seat = seatingChart.Seats[row, col].Available ? "X" : seatingChart.Seats[row, col].FullName;
          result.Append(seat.PadRight(25));
        }

        result.AppendLine();
      }

      return result.ToString();
    }
  }
}
