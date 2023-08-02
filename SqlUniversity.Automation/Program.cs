// See https://aka.ms/new-console-template for more information
using SqlUniversity.Automation.Scenario;

var baseUrl = "https://localhost:7277/api";

var courseUrl = $"{baseUrl}/Courses";

var staticDataScenario = new StaticDataScenario(baseUrl);
await staticDataScenario.StartRunScenario();


var enrollmentUrl = $"{baseUrl}/Enrollment";
var enrollmentScenario = new EnrollmentScenario(enrollmentUrl);

await enrollmentScenario.StartRunScenario();



Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");
Console.WriteLine("All test passed successfully!!!!!");

