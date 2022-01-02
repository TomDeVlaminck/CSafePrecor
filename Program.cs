using System;
using CSafe;
using CSafe.Commands;
using CSafe.Commands.CSafe;
using CSafe.Commands.CsafeLogic;

namespace PrecorTest02
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("starting PrecorTest01");
            IConnection connection = new Connection();
            
            Command powerCommand = new PowerCommand();
            CommandSet commandSet = new CommandSet();
            ResponseReader responseReader = new ResponseReader(1024);

            connection.Open();

            commandSet.Reset();
            commandSet.Add(powerCommand);
            commandSet.Prepare();

            byte[] commands = commandSet.getCommands();
            byte[] csafeframe = CSafeUtil.generateCSAFEFrame(commands);
            byte[] csaferesponse = connection.SendCSAFECommand(csafeframe);

            byte[] response = CSafeUtil.extractCSAFEResponse(csaferesponse);

            response.CopyTo(responseReader.Buffer, 0);
            responseReader.Reset(response.Length);

            commandSet.Read(responseReader);

            Console.WriteLine(powerCommand);

            Console.ReadLine();
        }
    }
}
