using System.Reflection;
using System.Text.Json;

namespace SeatingAssignments.Data
{
  public interface IClassroomRepository
  {
    Task<IEnumerable<ClassroomEntity>> GetStudentsForPeriodAsync(int period);
  }
  public  class ClassroomRepository: IClassroomRepository
  {
    public async Task<IEnumerable<ClassroomEntity>> GetStudentsForPeriodAsync(int period)
    {
      var exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var templateLocation = Path.Combine(exeLocation, "Data");
        var path = Path.Combine(templateLocation, "classroom.json");
        var json = await File.ReadAllTextAsync(path);
        var entity = JsonSerializer.Deserialize<List<ClassroomEntity>>(json);
        return entity != null ? entity.Where(x => x.Period == period) : Array.Empty<ClassroomEntity>();
    }
  }
}
