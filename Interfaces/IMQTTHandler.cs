using MQTTnet.Extensions.ManagedClient;

namespace TesteMQTT.Interfaces
{
    public interface IMQTTHandler
    {
        public String receivedTopic {get; set;}

        public String sendingTopic {get; set;}

        public IManagedMqttClient mqttClient {get; set;}

         Task handlerFunction(String msg);
    }
}