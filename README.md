Azure Service Bus Demo Project
Overview
This project demonstrates the implementation of Azure Service Bus messaging in a .NET solution. It consists of two main components:

A Blazor Server application (Sender) that sends messages to a Service Bus queue
A Console application (Receiver) that processes messages from the queue

Project Structure
CopyServiceBusDemo/
├── SBSender/             # Blazor Server app for sending messages
├── SBReceiver/           # Console app for receiving messages
└── SBSharedLib/         # Shared library for common models
Prerequisites

.NET 8.0 SDK
Azure Service Bus namespace
Visual Studio 2022 or VS Code
Azure subscription

Setup Instructions
1. Clone the Repository
bashCopygit clone https://github.com/makikaka/azure-service-bus.git
cd azure-service-bus
2. Configure User Secrets
The project uses .NET User Secrets to securely store the Azure Service Bus connection string. Follow these steps for both projects:
For the Receiver (Console App):
bashCopycd SBReceiver
dotnet user-secrets init --project "SBReceiver.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
For the Sender (Blazor App):
bashCopycd SBSender
dotnet user-secrets init --project "SBSender.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
3. Running the Applications
Start the Receiver:
bashCopycd SBReceiver
dotnet run
Start the Sender:
bashCopycd SBSender
dotnet run
Features

Message sending through a clean service interface
Message receiving with error handling
Dead letter queue support
JSON serialization of messages
Graceful shutdown handling
Environment-based configuration

Application Flow

The Blazor application allows users to input person details
Messages are sent to an Azure Service Bus queue
The console application receives and processes these messages
Failed messages are moved to a dead letter queue
Successful processing is logged to the console

Error Handling
The receiver implements comprehensive error handling:

Deserialization errors
Processing errors
Dead letter queue management
Message abandonment when dead letter queue is unavailable

Security

Connection strings are stored in user secrets
No sensitive information in configuration files
All configuration files are properly gitignored

Contributing

Fork the repository
Create a feature branch
Commit your changes
Push to the branch
Create a new Pull Request

License
This project is licensed under the MIT License - see the LICENSE file for details.
Contact
For any questions or concerns, please open an issue in the GitHub repository.
