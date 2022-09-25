using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeatingAssignments.Data;
using SeatingAssignments.Service;

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

      serviceCollection.AddSingleton<IClassroomService, ClassroomService>();
      serviceCollection.AddSingleton<ClassroomRepository>();

      ServiceProvider = serviceCollection.BuildServiceProvider();
    }
  }
}