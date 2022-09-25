// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using SeatingAssignments;
using SeatingAssignments.Service;

var startup = new Startup();
var classroomService = startup.ServiceProvider.GetRequiredService<IClassroomService>();
do
{
  try
  {
    Console.WriteLine("Hello Teacher for some Classroom! Let's create a seating chart");
    Console.WriteLine("Type 'exit' to exit program");
    Console.WriteLine("What Period do you want to create a seating chart for:");

    var period = Console.ReadLine();
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);
    Console.WriteLine("How big is your classroom? ex. 8x9");
    var sizeInput = Console.ReadLine();
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);
    var size = sizeInput.Split('x', StringSplitOptions.RemoveEmptyEntries);
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);

    var result = await classroomService.CreateSeatingChartAsync(int.Parse(period), int.Parse(size[0]), int.Parse(size[1]));
    Console.WriteLine(classroomService.CreateSeatingChartDisplay(result));
  }
  catch (Exception ex)
  {
    Console.WriteLine(ex.Message);
  }
} while (true);

