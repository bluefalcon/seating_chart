using SeatingAssignments.Extensions;

namespace SeatingAssignments.Models
{
    public class SeatingChartModel
    {
        public SeatingChartModel(int rows, int columns)
        {
            TotalRows = rows;
            TotalColumns = columns;
            Seats = new SeatModel[rows, columns];
            Seats.InitWithEmptySeats();
        }
        public int Period { get; set; }
        public int TotalRows { get; }
        public int TotalColumns { get; }
        public SeatModel[,] Seats { get; set; }
    }
}
