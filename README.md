# Azure Service Bus Demo Project

## Overview
This project demonstrates the implementation of Azure Service Bus messaging in a .NET solution. It consists of four main components:
- **Blazor Server application (Queue Publisher):** Sends messages to a Service Bus queue
- **Console application (Queue Subscriber):** Processes messages from the queue
- **Blazor Server application (Topic Publisher):** Sends messages to a Service Bus topic with message type filtering
- **Console application (Topic Subscriber):** Processes messages from topic subscriptions with filtering capabilities

---

## Project Structure
```
CopyServiceBusDemo/
├── SBQuePublisher/    # Blazor Server app for sending messages to queue
├── SBQueSubscriber/   # Console app for receiving messages from queue
├── SBTopicPublisher/  # Blazor Server app for sending messages to topic
├── SBTopicSubscriber/ # Console app for receiving messages from topic subscriptions
└── SBSharedLib/       # Shared library for common models
```

---

## Prerequisites
Ensure the following tools and resources are available before setting up the project:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/)
- Azure Service Bus namespace with:
  - Queue: "person-que"
  - Topic: "person-topic" with subscriptions:
    - "maki" (for VIP messages)
    - "paun" (for Regular messages)
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
The project uses .NET User Secrets to securely store the Azure Service Bus connection string. Configure the secrets for all projects as follows:

#### **For Queue Subscriber:**
```bash
cd SBQueSubscriber
dotnet user-secrets init --project "SBQueSubscriber.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```

#### **For Queue Publisher:**
```bash
cd SBQuePublisher
dotnet user-secrets init --project "SBQuePublisher.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```

#### **For Topic Subscriber:**
```bash
cd SBTopicSubscriber
dotnet user-secrets init --project "SBTopicSubscriber.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```

#### **For Topic Publisher:**
```bash
cd SBTopicPublisher
dotnet user-secrets init --project "SBTopicPublisher.csproj"
dotnet user-secrets set "ConnectionStrings:AzureServiceBus" "your_connection_string"
```

In Azure, you can find the Service Bus connection string (which includes the key) by following these steps:

1. Go to the Azure Portal (portal.azure.com)
2. Navigate to your Service Bus Namespace
3. In the left sidebar, click on "Shared access policies" under "Settings"
4. Click on "RootManageSharedAccessKey" (or create a new policy with appropriate permissions)
5. You'll see two connection strings:
   - Primary Connection String
   - Secondary Connection String

You can copy either one - they both work the same way. The connection string will look something like this:
`Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yourkey123...`

---

### 3. Running the Applications

#### **Start the Queue Subscriber:**
```bash
cd SBQueSubscriber
dotnet run
```

#### **Start the Queue Publisher:**
```bash
cd SBQuePublisher
dotnet run
```

#### **Start the Topic Subscriber:**
```bash
cd SBTopicSubscriber
dotnet run
# Choose subscription (maki or paun) when prompted
```

#### **Start multiple Topic Subscribers:**
Open multiple terminal windows and run the Topic Subscriber in each one, choosing different subscriptions to see how message filtering works.

#### **Start the Topic Publisher:**
```bash
cd SBTopicPublisher
dotnet run
```

---

## Features
- **Queue-based messaging:** Simple point-to-point communication
- **Topic-based messaging:** Publish-subscribe pattern with filtering
- **Message type filtering:** VIP and Regular message routing
- **Multiple subscription support:** Different subscribers for different message types
- **Message sending:** Through a clean service interface
- **Message receiving:** With robust error handling
- **Dead letter queue support:** For failed message processing
- **JSON serialization:** For efficient data transmission
- **Graceful shutdown handling:** Ensures proper cleanup
- **Environment-based configuration:** Seamlessly adapts to different environments

---

## Application Flow
1. The Blazor applications allow users to input person details
2. Messages can be sent to either:
   - A Service Bus queue (Queue Publisher)
   - A Service Bus topic with message type (Topic Publisher)
3. The console applications receive and process these messages:
   - Queue Subscriber processes all queue messages
   - Topic Subscribers process messages based on their subscription filters
4. Failed messages are moved to a dead letter queue
5. Successfully processed messages are logged to the console

---

## Error Handling
The Subscriber applications include comprehensive error handling:
- **Deserialization errors:** Handles issues with incoming message formats
- **Processing errors:** Catches and logs exceptions during processing
- **Dead letter queue management:** Ensures failed messages are redirected appropriately
- **Message abandonment:** If the dead letter queue is unavailable, messages are abandoned

---

## Security
- **Secure storage:** Connection strings are stored in .NET User Secrets
- **Configuration hygiene:** No sensitive information is present in configuration files
- **Gitignore:** Configuration files are properly excluded from version control

---

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

---

## License
This project is licensed under the [MIT License](LICENSE).

---

## Contact
For any questions or concerns, please open an issue in the [GitHub repository](https://github.com/makikaka/azure-service-bus).
