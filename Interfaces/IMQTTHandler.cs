using MQTTnet.Extensions.ManagedClient;

namespace TesteMQTT.Interfaces
{
    public interface IMQTTHandler
    {
        public String receivedTopic {get; set;}

        public String sendingTopic {get; set;}

         Task handlerFunction(String msg, IManagedMqttClient _mqttClient);
    }
}