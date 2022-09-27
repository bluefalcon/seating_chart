using Microsoft.Extensions.DependencyInjection;
using SeatingAssignments.Data;
using SeatingAssignments.Services;

namespace SeatingAssignments
{
  public class Startup
  {

    public Startup()
    {
      ConfigureServices();
    }

    public IServiceProvider ServiceProvider { get; private set; }


    private void ConfigureServices()
    {
      var serviceCollection = new ServiceCollection();

      serviceCollection.AddSingleton<ISeatingChartService, SeatingChartService>();
      serviceCollection.AddSingleton<IClassroomRepository, ClassroomRepository>();

      ServiceProvider = serviceCollection.BuildServiceProvider();
    }
  }
}