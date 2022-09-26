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
      var studentList = students.ToList();
      if (!studentList.Any()) return seatingChartResult;

      var sortedStudents = studentList.ToList().OrderByDescending(x => x.LastName).ToList();
      var totalStudents = sortedStudents.Count();
      var totalSeats = (rows * columns);

      if (totalStudents > totalSeats)
        throw new Exception($"Total Students: {totalStudents} exceeds Total Seats: {totalSeats}");

      var spacesAvailable = totalSeats - totalStudents;
      var studentsNoSpace = totalStudents - spacesAvailable;
      var currentStudent = 0;

      Func<int, bool> outOfStudentsFunc = (currentPosition) => currentPosition >= sortedStudents.Count;
      Func<int, bool> checkOutOfStudentNoSpace = (studentsNoSpaceCount) => studentsNoSpaceCount < totalStudents && studentsNoSpaceCount > 0;

    for (var row = 1; row <= rows; row++)
      {
        var isEvenRow = row % 2 == 0;
        var workingColumn = 1;
        for (var col = columns; col > 0; col--)
        {
          var isEvenCol = workingColumn % 2 == 0;

          var addEvenRowStudent = false;
          var addOddRowStudent = false;
          if (checkOutOfStudentNoSpace(studentsNoSpace))
          {
            addEvenRowStudent = true;
            addOddRowStudent = true;
          }
          else
          {
            addEvenRowStudent = isEvenRow && isEvenCol &&  spacesAvailable > 0 ||  spacesAvailable == 0;
            addOddRowStudent = !isEvenRow && !isEvenCol &&  spacesAvailable > 0 ||  spacesAvailable == 0;
          }


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
            if (checkOutOfStudentNoSpace(studentsNoSpace)) studentsNoSpace--;
          }
          else
          {
            seatingChartResult.Seats[row - 1, col - 1] = new Seat { Column = col, Row = row };
            if ( spacesAvailable > 0)  spacesAvailable--;
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
      result.AppendLine("********** Front Of Classroom **********");
      result.AppendLine();
      for (var row = 0; row < seatingChart.Seats.GetLength(0); row++)
      {
        for (var col = 0; col < seatingChart.Seats.GetLength(1); col++)
        {
          var seat = seatingChart.Seats[row, col].Available ? "-" : seatingChart.Seats[row, col].FullName;
          result.Append(seat.PadRight(25));
        }

        result.AppendLine();
      }

      return result.ToString();
    }
  }
}
