// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using SeatingAssignments;
using SeatingAssignments.Services;

var startup = new Startup();
var seatingChartService = startup.ServiceProvider.GetRequiredService<ISeatingChartService>();
do
{
  try
  {
    Console.WriteLine("Hello Teacher for some Classroom! Let's create a seating chart");
    Console.WriteLine("Type 'exit' to exit program");
    Console.WriteLine("What Period do you want to create a seating chart for:  ");

    var period = Console.ReadLine();
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);
    Console.WriteLine("Classroom size ex. 8x9. defaults to 8x9:  ");
    var sizeInput = Console.ReadLine();
    if (string.IsNullOrEmpty(sizeInput)) sizeInput = "8x9";
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);
    var size = sizeInput.Split('x', StringSplitOptions.RemoveEmptyEntries);
    if (period.Equals("exit", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);

    var result = await seatingChartService.GenerateSeatingChartAsync(int.Parse(period), int.Parse(size[0]), int.Parse(size[1]));

    Console.WriteLine(seatingChartService.GenerateSeatingChartDisplayText(result));
  }
  catch (Exception ex)
  {
    Console.WriteLine(ex.Message);
  }
} while (true);

