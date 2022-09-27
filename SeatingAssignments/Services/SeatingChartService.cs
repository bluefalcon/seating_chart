using System.Text;
using SeatingAssignments.Data;
using SeatingAssignments.Extensions;
using SeatingAssignments.Models;

namespace SeatingAssignments.Services
{
  public interface ISeatingChartService
  {

    /// <summary>
    /// Generates a Classroom Seating chart
    /// </summary>
    /// <param name="period"></param>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    /// <returns></returns>
    Task<SeatingChartModel> GenerateSeatingChartAsync(int period, int rows, int columns);

    /// <summary>
    /// Generates a String of text to display on the console.  Should probably move to program.cs
    /// </summary>
    /// <param name="seatingChart"></param>
    /// <returns></returns>
    string GenerateSeatingChartDisplayText(SeatingChartModel seatingChart);
  }

  public class SeatingChartService : ISeatingChartService
  {
    private readonly IClassroomRepository _classroomRepository;

    public SeatingChartService(IClassroomRepository classroomRepository)
    {
      _classroomRepository = classroomRepository;
    }

    public async Task<SeatingChartModel> GenerateSeatingChartAsync(int period, int rows, int columns)
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

      var spacesAvailableCount = totalSeats - totalStudents;
      var studentsNoSpaceCount = Math.Max(0, totalStudents - spacesAvailableCount);
      var currentSortedStudentPos = 0;

      Func<int, bool> outOfStudentsFunc = (currentPosition) => currentPosition >= sortedStudents.Count;
      Func<int, bool> outOfStudentsNoSpaceFunc = (currentStudentsNoSpaceCount) =>  currentStudentsNoSpaceCount < totalStudents && currentStudentsNoSpaceCount > 0;

      for (var row = 1; row <= rows; row++)
      {
        var isEvenRow = row % 2 == 0;
        var workingColumn = 1;
        if (outOfStudentsFunc(currentSortedStudentPos)) break;
        for (var col = columns; col > 0; col--)
        {
          if (outOfStudentsFunc(currentSortedStudentPos)) break;
          var isEvenCol = workingColumn % 2 == 0;

          var addEvenRowStudent = false;
          var addOddRowStudent = false;
          if (outOfStudentsNoSpaceFunc(studentsNoSpaceCount))
          {
            addEvenRowStudent = true;
            addOddRowStudent = true;
          }
          else
          {
            addEvenRowStudent = isEvenRow && isEvenCol && spacesAvailableCount > 0 || spacesAvailableCount == 0;
            addOddRowStudent = !isEvenRow && !isEvenCol && spacesAvailableCount > 0 || spacesAvailableCount == 0;
          }

          if (addEvenRowStudent || addOddRowStudent)
          {
            seatingChartResult.Seats[row - 1, col - 1].FirstName = sortedStudents[currentSortedStudentPos].FirstName;
            seatingChartResult.Seats[row - 1, col - 1].LastName = sortedStudents[currentSortedStudentPos].LastName;

            currentSortedStudentPos++;
            if (outOfStudentsNoSpaceFunc(studentsNoSpaceCount)) studentsNoSpaceCount--;
          }
          else
          {
            if (spacesAvailableCount > 0) spacesAvailableCount--;
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
