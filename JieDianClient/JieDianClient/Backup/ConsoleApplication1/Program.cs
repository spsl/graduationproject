using System;
using System.Collections.Generic;
using System.Text;
using CoAP;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            Request request = Request.NewGet();
            request.URI = new Uri("coap://[aaaa::cc]/camera");
            request.Send();
            Response response = request.WaitForResponse();
            Console.WriteLine(response.payload);
            Console.ReadLine();
        }
    }
}
