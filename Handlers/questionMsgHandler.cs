using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MQTTnet.Extensions.ManagedClient;
using TesteMQTT.Interfaces;
using TesteMQTT.Models;

namespace TesteMQTT.Handlers
{
    public class questionMsgHandler : IMQTTHandler
    {   
        public questionMsgHandler(String _receivedTopic,String _sendingTopic,IManagedMqttClient _mqttClient)
        {
            receivedTopic=_receivedTopic;
            sendingTopic=_sendingTopic;
            mqttClient=_mqttClient;
        }
       
        public String receivedTopic {get; set;}

        public String sendingTopic {get; set;}

        public IManagedMqttClient mqttClient {get; set;}

        public async Task handlerFunction(string msg)
        {    
            try{

                QuestionMsg ? obj=JsonSerializer.Deserialize<QuestionMsg>(msg);

                Double ? response=obj?.a+obj?.b;

                 string json = JsonSerializer.Serialize(
                 new { response = response, 
                      timestamp = DateTime.UtcNow });

                 await mqttClient.EnqueueAsync(sendingTopic, json);
            

            }catch(Exception error){
                Console.WriteLine($"ERRO: {error}");
            }
           
        }
    }
}
