using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RGTKeyboard
{
    internal class Commands
    {
        #region Keyboard Scan Codes
        Dictionary<string, int> scanCodes = new (string, int)[] 
        { 
            ("q", 16),
            ("w", 17),
            ("e", 18),
            ("r", 19),
            ("t", 20),
            ("y", 21),
            ("u", 22),
            ("i", 23),
            ("o", 24),
            ("p", 25),
            ("a", 30),
            ("s", 31),
            ("d", 32),
            ("f", 33),
            ("g", 34),
            ("h", 35),
            ("j", 36),
            ("k", 37),
            ("l", 38),
            ("z", 44),
            ("x", 45),
            ("c", 46),
            ("v", 47),
            ("b", 48),
            ("n", 49),
            ("m", 50),
            ("F1", 59),
            ("F2", 60),
            ("F3", 61),
            ("F4", 62),
            ("F5", 63),
            ("F6", 64),
            ("F7", 65),
            ("F8", 66),
            ("F9", 67),
            ("F10", 68),
            ("F11", 69),
            ("F12", 70),
            ("1", 2),
            ("2", 3),
            ("3", 4),
            ("4", 5),
            ("5", 6),
            ("6", 7),
            ("7", 8),
            ("8", 9),
            ("9", 10),
            ("0", 11)
        }.ToDictionary(x => x.Item1, y => y.Item2);
        #endregion

        Dictionary<int, KeyboardCommand> commands = new Dictionary<int, KeyboardCommand>();

        internal Commands()
        {
            if(!File.Exists("Commands.txt"))
            {
                Console.WriteLine("No Commands.txt file found");
                return;
            }
            using (var sr = new StreamReader("Commands.txt"))
            {
                var line = sr.ReadLine();
                while(line != null)
                {
                    var arr = line.Split(';');
                    if (line.IndexOf('#') == 0 || arr.Length < 3)
                    {
                        line = sr.ReadLine();
                        continue;
                    }

                    if (int.TryParse(arr[0], out int port))
                    {
                        if (commands.ContainsKey(port))
                        {
                            Console.WriteLine($"Command for port {port} already exists. Skipping this one");
                        }
                        switch (arr[1])
                        {
                            case "K":
                                if (scanCodes.TryGetValue(arr[2], out int scanCode))
                                {
                                    commands.Add(port, new KeyboardCommand { wK = 0, wScan = (ushort)scanCode });
                                }
                                break;
                            default:
                                Console.WriteLine($"Unknown command type '{arr[1]}'");
                                break;
                        }
                    }
                    line = sr.ReadLine();
                }
            }
        }

        public KeyboardCommand GetKeyboardCommand(int port)
        {
            if(commands.TryGetValue(port, out KeyboardCommand command))
            {
                return command;
            }
            return null;
        }
    }
}
