using System;
using CoAP.EndPoint;
using CoAP.Examples.Resources;

namespace CoAP.Examples
{
    class CoAPServer : LocalEndPoint
    {

        public CoAPServer()
        {
            AddResource(new HelloWorldResource());
            AddResource(new CarelessResource());
            AddResource(new ImageResource());
            AddResource(new SeparateResource());
            AddResource(new TimeResource());
        }

        static void Main(String[] args)
        {
            try
            {
                CoAPServer server = new CoAPServer();
                Console.WriteLine("CoAP server [{1}] is listening on port {0}.",
                    server.Communicator.Port);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
