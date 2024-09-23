using System;
using System.Text.Json;
using Azure.AI.OpenAI;

public class BookAppointment
{
    public static string Name = "GetAppointmentDate";
    // Return function metadata
    static public FunctionDefinition GetFunctionDefinition()
    {
        return new FunctionDefinition()
        {
            Name = "GetAppointmentDate",
            Description = "Get the appointment date for a booking a doctor's appointment.",
            Parameters = BinaryData.FromObjectAsJson(new
            {
                Type = "object",
                Properties = new {
                    Name = new
                    {
                        type = "string",
                        description = "The name of the doctor."
                    },
                    Date = new
                    {
                        type = "string",
                        description = "The date for the appointment in the format YYYY-MM-DD."
                    },
                    Time = new
                    {
                        type = "string",
                        description = "The time for the appointment in the format HH:MM in 24 hour format."
                    },
                    Duration = new
                    {
                        type = "integer",
                        description = "The duration of the appointment in minutes."
                    }
                },
                Required = new string[] { "Name", "Date", "Time", "Duration" }
            },
            new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
        };
    }

    static public void GetAppointmentDate(string name, string date, string time, int duration)
    {
        // Code to get the appointment date
        Console.WriteLine($"Appointment for {name} on {date} at {time} for {duration} minutes.");
    }
}

public class Appointment
{
    public string? Name { get; set; }
    public string? Date { get; set; }
    public string? Time { get; set; }
    public int Duration { get; set; }
}