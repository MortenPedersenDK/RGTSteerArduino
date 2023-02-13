using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;

namespace RGTKeyboard
{
    internal class RGTKeyboard
    {
        bool run = true;
        SerialPort serialPort;
        Thread worker;
        Commands commands;

        public RGTKeyboard(string comPort, int baudRate = 9600)
        {
            try
            {
                commands = new Commands();

                serialPort = new SerialPort(comPort, baudRate);
                serialPort.ReadTimeout = 1000;
                serialPort.Open();

                worker = new Thread(SerialReader);
                worker.Priority = ThreadPriority.BelowNormal;
                worker.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Quit()
        {
            run = false;
            worker.Join(2000);
            serialPort.Close();
        }

        public void Wait()
        {
            Console.WriteLine("Waiting for action. Press enter if you want to quit");
            Console.ReadLine();
            Quit();
        }

        public void SerialReader()
        {
            while (run)
            {
                try
                {
                    string msg = serialPort.ReadLine().Trim();
                    if (msg.StartsWith("{") && msg.EndsWith("}"))
                    {
                        var arduinoMessage = JsonConvert.DeserializeObject<ArduinoMessage>(msg);
                        Console.WriteLine($"Port {arduinoMessage.Port} activated {arduinoMessage.Activations} time(s).");
                        var cmd = commands.GetKeyboardCommand(arduinoMessage.Port);
                        if (cmd != null)
                        {
                            WinApi.SimulateKeyPress(cmd.wK, cmd.wScan);
                        }
                        else
                        {
                            Console.WriteLine($"No command configured for port {arduinoMessage.Port}. Edit file Commands.txt to setup command for this port.");
                        }
                    }
                }
                catch (TimeoutException) { }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        static void Main(string[] args)
        {
            if(SerialPort.GetPortNames().Length == 0)
            {
                Console.WriteLine("No COM ports available! Press enter to quit.");
                Console.ReadLine();
                return;
            }

            var port = string.Empty;
            if (args.Length > 0)
            {
                var portFound = false;
                foreach (string comport in SerialPort.GetPortNames())
                {
                    if (comport == args[0])
                    {
                        portFound = true;
                        break;
                    }
                }
                if(portFound)
                {
                    port = args[0];
                }
            }
            
            while (string.IsNullOrWhiteSpace(port))
            {
                Console.WriteLine("Available serial ports: ");
                var ports = SerialPort.GetPortNames();
                for (int i = 0; i < ports.Length; i++)
                {
                    Console.WriteLine($"{i + 1}: {ports[i]}");
                }
                Console.Write($"Select serial port(option 1 to {ports.Length}): ");
                var choice = Console.ReadLine();
                if (int.TryParse(choice, out int idx) && idx <= ports.Length && idx > 0)
                {
                    port = ports[idx - 1];
                }
            }
            new RGTKeyboard(port).Wait();
        }
    }

}
