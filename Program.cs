// Implicit using statements are included
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Azure;

// Add Azure OpenAI package
using Azure.AI.OpenAI;

// Build a config object and retrieve user settings.
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();
string? oaiEndpoint = config["AzureOAIEndpoint"];
string? oaiKey = config["AzureOAIKey"];
string? oaiDeploymentName = config["AzureOAIDeploymentName"];

if(string.IsNullOrEmpty(oaiEndpoint) || string.IsNullOrEmpty(oaiKey) || string.IsNullOrEmpty(oaiDeploymentName) )
{
    Console.WriteLine("Please check your appsettings.json file for missing or incorrect values.");
    return;
}

 // Initialize the Azure OpenAI client
OpenAIClient client = new OpenAIClient(new Uri(oaiEndpoint), new AzureKeyCredential(oaiKey));
    
 // System message to provide context to the model
string systemMessage = "You are a friendly chatbot helping users to book appointment in Super Clinic. You can provide information about the clinic, book an appointment, or cancel an appointment.";

ChatChoice responseChoice;

// Build completion options object
ChatCompletionsOptions chatCompletionsOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatRequestSystemMessage(systemMessage),
    },
    MaxTokens = 400,
    Temperature = 0.7f,
    DeploymentName = oaiDeploymentName,

};

Console.WriteLine("Welcome to Super Clinic! How can I help you today? (or type 'quit' to exit): ");
do {
   
    string? inputText = Console.ReadLine();
    if (inputText == "quit") break;

    // Generate summary from Azure OpenAI
    if (inputText == null) {
        Console.WriteLine("Please enter a prompt.");
        continue;
    }
    chatCompletionsOptions.Messages.Add(new ChatRequestUserMessage(inputText));

    // Add code to send request...

    FunctionDefinition bookAppointmentFunction = BookAppointment.GetFunctionDefinition();
    chatCompletionsOptions.Functions.Add(bookAppointmentFunction);

    // Send request to Azure OpenAI model
    Console.WriteLine("\nSending request for summary to Azure OpenAI endpoint...\n\n");
    ChatCompletions response = client.GetChatCompletions(chatCompletionsOptions);

    responseChoice = response.Choices[0];


    if (responseChoice.FinishReason == CompletionsFinishReason.FunctionCall &&
        responseChoice.Message.FunctionCall != null &&
        responseChoice.Message.FunctionCall.Name == "GetAppointmentDate")
    {
        string unvalidatedArguments = responseChoice.Message.FunctionCall.Arguments;
        Appointment? appointment = JsonSerializer.Deserialize<Appointment>(unvalidatedArguments,
                                    new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    

        Console.WriteLine($"Appointment for {appointment?.Name} on {appointment?.Date} at {appointment?.Time} for {appointment?.Duration} minutes.");
        chatCompletionsOptions.Messages.Clear();
    }
    else
    {
        Console.WriteLine("Response: " + responseChoice.Message.Content + "\n. Type 'quit' to exit.");
    }
    
} while (true);
