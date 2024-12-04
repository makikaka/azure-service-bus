# Azure Service Bus Demo Project

## Overview
This project demonstrates the implementation of Azure Service Bus messaging in a .NET solution. It consists of two main components:
- **Blazor Server application (Sender):** Sends messages to a Service Bus queue.
- **Console application (Receiver):** Processes messages from the queue.

---

## Project Structure
```
CopyServiceBusDemo/
├── SBSender/          # Blazor Server app for sending messages
├── SBReceiver/        # Console app for receiving messages
└── SBSharedLib/       # Shared library for common models
```

---

## Prerequisites
Ensure the following tools and resources are available before setting up the project:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/)
- Azure Service Bus namespace
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- Azure subscription

---

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/makikaka/azure-service-bus.git
cd azure-service-bus
```

### 2. Configure User Secrets
The project uses .NET User Secrets to securely store the Azure Service Bus connection string. Configure the secrets for both projects as follows:

#### **For the Receiver (Console App):**
```bash
cd SBReceiver
dotnet user-secrets init --project "SBReceiver.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```

#### **For the Sender (Blazor App):**
```bash
cd SBSender
dotnet user-secrets init --project "SBSender.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```
In Azure, you can find the Service Bus connection string (which includes the key) by following these steps:

Go to the Azure Portal (portal.azure.com)
Navigate to your Service Bus Namespace
In the left sidebar, click on "Shared access policies" under "Settings"
Click on "RootManageSharedAccessKey" (or create a new policy with appropriate permissions)
You'll see two connection strings:

Primary Connection String
Secondary Connection String



You can copy either one - they both work the same way. The connection string will look something like this:
Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yourkey123...

---

### 3. Running the Applications

#### **Start the Receiver:**
```bash
cd SBReceiver
dotnet run
```

#### **Start the Sender:**
```bash
cd SBSender
dotnet run
```

---

## Features
- **Message sending:** Through a clean service interface.
- **Message receiving:** With robust error handling.
- **Dead letter queue support:** For failed message processing.
- **JSON serialization:** For efficient data transmission.
- **Graceful shutdown handling:** Ensures proper cleanup.
- **Environment-based configuration:** Seamlessly adapts to different environments.

---

## Application Flow
1. The Blazor application allows users to input person details.
2. Messages are sent to an Azure Service Bus queue.
3. The console application receives and processes these messages.
4. Failed messages are moved to a dead letter queue.
5. Successfully processed messages are logged to the console.

---

## Error Handling
The Receiver application includes comprehensive error handling:
- **Deserialization errors:** Handles issues with incoming message formats.
- **Processing errors:** Catches and logs exceptions during processing.
- **Dead letter queue management:** Ensures failed messages are redirected appropriately.
- **Message abandonment:** If the dead letter queue is unavailable, messages are abandoned.

---

## Security
- **Secure storage:** Connection strings are stored in .NET User Secrets.
- **Configuration hygiene:** No sensitive information is present in configuration files.
- **Gitignore:** Configuration files are properly excluded from version control.

---

## Contributing
1. Fork the repository.
2. Create a feature branch.
3. Commit your changes.
4. Push to the branch.
5. Create a new Pull Request.

---

## License
This project is licensed under the [MIT License](LICENSE).

---

## Contact
For any questions or concerns, please open an issue in the [GitHub repository](https://github.com/makikaka/azure-service-bus).
