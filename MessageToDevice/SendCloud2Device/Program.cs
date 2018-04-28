using Microsoft.Azure.Devices;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendCloud2Device
{
    class Program
    {
        static ServiceClient serviceClient;
        static string connectionString = "HostName=electra.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=12AhxT0etW53dGg5l2O2fAmzSB08o6OCGZpAephLwC0=";

        private async static Task SendCloudToDeviceMessageAsync(string s)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes(s));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync("electra-iot-design.gr", commandMessage);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Send Cloud-to-Device message\n");
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            ReceiveFeedbackAsync();
            while (true)
            {
                Console.WriteLine("Press any key to send a C2D message.");
                var s = Console.ReadLine();
                SendCloudToDeviceMessageAsync(s).Wait();
            }
        }

        private async static void ReceiveFeedbackAsync()
        {
            var feedbackReceiver = serviceClient.GetFeedbackReceiver();

            Console.WriteLine("\nReceiving c2d feedback from service");
            while (true)
            {
                var feedbackBatch = await feedbackReceiver.ReceiveAsync();
                if (feedbackBatch == null) continue;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
                Console.ResetColor();

                await feedbackReceiver.CompleteAsync(feedbackBatch);
            }
        }
    }
}
