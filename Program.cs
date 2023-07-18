using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System.Text;
using TesteMQTT.Interfaces;
using TesteMQTT.Handlers;
using Microsoft.Extensions.Configuration;

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



// Set up handlers
_mqttClient.ConnectedAsync += _mqttClient_ConnectedAsync;


_mqttClient.DisconnectedAsync += _mqttClient_DisconnectedAsync;


_mqttClient.ConnectingFailedAsync += _mqttClient_ConnectingFailedAsync;


// Connect to the broker
await _mqttClient.StartAsync(options);
await _mqttClient.SubscribeAsync(TestTopic);
await _mqttClient.SubscribeAsync(QuestionTopic);

IMQTTHandler testingHandler= new msgHandler(TestTopic,TestingTopic);

IMQTTHandler responseHandler= new questionMsgHandler(QuestionTopic,ResponseTopic);

List<IMQTTHandler> Handlers=new List<IMQTTHandler>();
Handlers.Add(testingHandler);
Handlers.Add(responseHandler);

_mqttClient.ApplicationMessageReceivedAsync += async e =>
            {   
                try{

                    string topic=e.ApplicationMessage.Topic;
                    String data=Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                
                    IMQTTHandler ? handleToCall=Handlers.Find(e=>e.receivedTopic==topic);

                    if(handleToCall is null){
                        Console.WriteLine($"Nenhum handle para {topic} encontrado");
                    } else {
                        handleToCall.handlerFunction(data,_mqttClient);
                    }

                }catch(Exception error){
                    Console.WriteLine($"ERRO:{error}");
                }
                
                
            };


Task _mqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
{
    Console.WriteLine("Connected");
    return Task.CompletedTask;
};
Task _mqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
{
    Console.WriteLine("Disconnected");
    return Task.CompletedTask;
};
Task _mqttClient_ConnectingFailedAsync(ConnectingFailedEventArgs arg)
{
    Console.WriteLine("Connection failed check network or broker!");
    return Task.CompletedTask;
}





while (true)
{


    await Task.Delay(TimeSpan.FromSeconds(1));
}