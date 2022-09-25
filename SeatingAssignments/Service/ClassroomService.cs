using System.Text;
using SeatingAssignments.Data;

namespace SeatingAssignments.Service
{
  public interface IClassroomService
  {
    Task<SeatingChartModel> CreateSeatingChartAsync(int period, int rows, int columns);
    string GetSeatingChart(SeatingChartModel model);
    string CreateSeatingChartDisplay(SeatingChartModel seatingChart);
  }
  public class ClassroomService : IClassroomService
  {
    private readonly ClassroomRepository _classroomRepository;

    public ClassroomService(ClassroomRepository classroomRepository)
    {
      _classroomRepository = classroomRepository ?? new ClassroomRepository();
    }

    public async Task<SeatingChartModel> CreateSeatingChartAsync(int period, int rows, int columns)
    {
      if(period <= 0) throw new ArgumentException("Period less than equal To 0", nameof(period));
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
      if (totalStudents > totalSeats) throw new Exception($"Total Students: {totalStudents} exceeds Total Seats: {totalSeats}");

      var totalSpacesAvailable = totalSeats - totalStudents;
      var currentStudent = 0;

      Func<int, bool> outOfStudentsFunc = (currentPosition) => currentPosition >= sortedStudents.Count;

      for (var row = 1; row <= rows; row++)
      {
        var isEvenRow = row % 2 == 0;
        var workingColumn = 1;
        for (var col= columns; col > 0; col--)
        {
          var isEvenCol = workingColumn % 2 == 0;
          var addEvenRowStudent =isEvenRow && isEvenCol && totalSpacesAvailable > 0 || totalSpacesAvailable == 0;
          var addOddRowStudent =!isEvenRow && !isEvenCol && totalSpacesAvailable > 0 || totalSpacesAvailable == 0;
          if (addEvenRowStudent && !outOfStudentsFunc(currentStudent) || addOddRowStudent && !outOfStudentsFunc(currentStudent))
          {
            seatingChartResult.Seats.Add(new Seat
            {
              FirstName = sortedStudents[currentStudent].FirstName,
              LastName = sortedStudents[currentStudent].LastName,
              Number = col,
              Row = row
            });
              seatingChartResult.SeatingAssignments[row - 1, col - 1] = $"{sortedStudents[currentStudent].FirstName} {sortedStudents[currentStudent].LastName}";
              currentStudent++;
          }
          else
          {
            seatingChartResult.Seats.Add(new Seat { Number = col, Row = row });
            if (totalSpacesAvailable > 0) totalSpacesAvailable--;
          }

          workingColumn++;
        }
      }
      return seatingChartResult;
    }

    public string CreateSeatingChartDisplay(SeatingChartModel seatingChart)
    {
      var result = new StringBuilder();
      result.AppendLine($"Seating Chart for Period: {seatingChart.Period}");
      var totalStudents = seatingChart.Seats.Count(x => x.Available == false);
      result.AppendLine($"Total Students: {totalStudents}");
      if (totalStudents <= 0) return result.ToString();
      result.AppendLine();

      var pad = seatingChart.Seats.OrderByDescending(x => x.FullName.Length)
        .First()
        .FullName.Length + 3;

      for (var row = 1; row <= seatingChart.TotalRows; row++)
      {
        var seats = seatingChart.Seats.Where(x => x.Row == row);
        foreach (var seat in seats.OrderBy(x => x.SeatNumber))
        {
          var str = seat.Available ? "X" : seat.FullName;
          result.Append(str.PadRight(pad));
        }

        result.AppendLine();
      }

      return result.ToString();
    }

    public string GetSeatingChart(SeatingChartModel model)
    {
      var result = new StringBuilder();
      for (int row = 0; row < model.SeatingAssignments.GetLength(0); row++)
      {
        for (int col = 0; col < model.SeatingAssignments.GetLength(1); col++)
        {
          var str = model.SeatingAssignments[row, col] == null ? "X" : model.SeatingAssignments[row, col];
          result.Append(str.PadRight(25));
        }

        result.AppendLine();
      }

      return result.ToString();
    }
  }
}
