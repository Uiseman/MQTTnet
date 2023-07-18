using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System.Text;
using TesteMQTT.Interfaces;
using TesteMQTT.Handlers;
using Microsoft.Extensions.Configuration;
using TesteMQTT.Models;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

String ? MqttHost=config.GetSection("MqttHost").Value?.ToString();
String ? MqttPort=config.GetSection("MqttPort").Value?.ToString();
String ? MqttClientId=config.GetSection("MqttClientId").Value?.ToString();
String ? QuestionTopic=config.GetSection("QuestionTopic").Value?.ToString();
String ? TestTopic=config.GetSection("TestTopic").Value?.ToString();
String ? ResponseTopic=config.GetSection("ResponseTopic").Value?.ToString();
String ? TestingTopic=config.GetSection("TestingTopic").Value?.ToString();

Console.WriteLine($"MQTT Host: {MqttHost} Port: {MqttPort}");

IManagedMqttClient _mqttClient = new MqttFactory().CreateManagedMqttClient();

// Create client options object
MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                                        .WithClientId(MqttClientId)
                                        .WithTcpServer(MqttHost,Int32.Parse(MqttPort));
                                        
ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                        .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                        .WithClientOptions(builder.Build())
                        .Build();

IMQTTHandler testingHandler= new msgHandler(TestTopic,TestingTopic,_mqttClient);

IMQTTHandler responseHandler= new questionMsgHandler(QuestionTopic,ResponseTopic,_mqttClient);

List<IMQTTHandler> Handlers=new List<IMQTTHandler>();
Handlers.Add(testingHandler);
Handlers.Add(responseHandler);

MQTTClientHandlers mqttFunctions=new MQTTClientHandlers(_mqttClient,Handlers);

// Set up handlers
_mqttClient.ConnectedAsync += async arg=>{
   await  mqttFunctions.ClientConnectedAsync(arg);
};


_mqttClient.DisconnectedAsync += async arg=>{
    await  mqttFunctions.ClientDisconnectedAsync(arg);
};


_mqttClient.ConnectingFailedAsync += async arg=>{
    await  mqttFunctions.ClientConnectingFailedAsync(arg);
};

_mqttClient.ApplicationMessageReceivedAsync += async arg =>{    
    await mqttFunctions.ClientReceivedMessage(arg);
};


// Connect to the broker
await _mqttClient.StartAsync(options);
await _mqttClient.SubscribeAsync(TestTopic);
await _mqttClient.SubscribeAsync(QuestionTopic);


while (true)
{


    await Task.Delay(TimeSpan.FromSeconds(1));
}