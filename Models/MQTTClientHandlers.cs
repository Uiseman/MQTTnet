using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using TesteMQTT.Interfaces;

namespace TesteMQTT.Models
{
    public class MQTTClientHandlers
    {   
        public List<IMQTTHandler> Handlers {get; set;}

        public IManagedMqttClient mqttClient{get;set;}

        public MQTTClientHandlers(IManagedMqttClient _mqttClient)
        {
            mqttClient=_mqttClient;
        }

        public void AddClients(List<IMQTTHandler> _handlers ){
            _handlers.ForEach(element=>{
                Handlers.Add(element);
            });
        }


        public async Task ClientReceivedMessage(MqttApplicationMessageReceivedEventArgs e){
            try{

                    string topic=e.ApplicationMessage.Topic;
                    String data=Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                
                    IMQTTHandler ? handleToCall=Handlers.Find(e=>e.receivedTopic==topic);

                    if(handleToCall is null){
                        Console.WriteLine($"Nenhum handle para {topic} encontrado");
                    } else {
                        await handleToCall.handlerFunction(data,mqttClient);
                    }

            }catch(Exception error){
                    Console.WriteLine($"ERRO:{error}");
            }
        }
        public Task ClientConnectedAsync(MqttClientConnectedEventArgs arg)
        {
            Console.WriteLine("Connected");
            return Task.CompletedTask;
        }
        public Task ClientDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
            Console.WriteLine("Disconnected");
            return Task.CompletedTask;
        }
        Task ClientConnectingFailedAsync(ConnectingFailedEventArgs arg)
        {
            Console.WriteLine("Connection failed check network or broker!");
            return Task.CompletedTask;
        }

    }
}