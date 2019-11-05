using System;
using System.Collections.Generic;
using System.IO;

namespace Assembly_Emulator
{
    class MainClass
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Braun\Desktop\Assembly-Emulator-master\program.ae"; // EDIT PATH !
            if (args.Length == 1)
                path = args[0];
            //else // TODO UNComment
            //{
            //    Console.WriteLine("Please Enter your Program file Path : ");
            //    path = Console.ReadLine();
            //}

            new Program(path).Run();
        }
    }

    class ProgramData
    {
        public int ProgramCounter = 0;
        
        public Memory RAM = new Memory(4 * 1024);
        public Memory ROM = new Memory(4 * 1024);
        public Queue<int> Stack = new Queue<int>();
        public List<Command> Commands = new List<Command>();
        public Dictionary<string, int> Orgs = new Dictionary<string, int>();
        public Dictionary<string, int> JumpPositions = new Dictionary<string, int>();
    }

    class Program
    {
        /// <summary>
        /// TODOs
        /// 
        /// Correct locations for P & R
        /// 
        /// </summary>

        static public ProgramData Data { get; set; }

        public Program(string path)
        {
            Data = new ProgramData();
            new Parser().Create(GetContent(path));
        }

        public void Run()
        {
            while (true)
            {
                while (Data.ProgramCounter < Data.Commands.Count)
                {
                    Input();
                    Data.Commands[Data.ProgramCounter++].Run();
                    Output();

                    System.Threading.Thread.Sleep(2000);
                }

                string command = Console.ReadLine();
                if (command.ToUpper() == "Q")
                    break;

                new Parser().Create(new string[] { command });
            }
        }

        void Input()
        {
            if (Console.KeyAvailable)
            {
                var key = char.ToUpper(Console.ReadKey(true).KeyChar);

                Data.RAM.Bit(Settings.Constants["P1.0"], key == '0');
                Data.RAM.Bit(Settings.Constants["P1.1"], key == '1');
                Data.RAM.Bit(Settings.Constants["P1.2"], key == '2');
                Data.RAM.Bit(Settings.Constants["P1.3"], key == '3');
                Data.RAM.Bit(Settings.Constants["P1.4"], key == '4');
                Data.RAM.Bit(Settings.Constants["P1.5"], key == '5');
                Data.RAM.Bit(Settings.Constants["P1.6"], key == '6');
                Data.RAM.Bit(Settings.Constants["P1.7"], key == '7');

                Data.RAM.Bit(Settings.Constants["P3.2"], key == 'P');
                Data.RAM.Bit(Settings.Constants["P3.3"], key == 'O');

                if (Data.RAM.Bit(Settings.Constants["EA"]) != 0)
                {
                    Action<string> inter = (string s) =>
                    {
                        if (Data.Orgs.ContainsKey(s))
                        {
                            Data.Stack.Enqueue(Data.ProgramCounter);
                            Data.ProgramCounter = Data.Orgs[s];
                        }
                    };
                    Func<string, string, bool> set = (string f, string s) => {
                        return  Data.RAM.Bit(Settings.Constants[f]) != 0 &&
                                Data.RAM.Bit(Settings.Constants[s]) != 0;
                    };
                    Action<string, string, string> timer = (string f, string s, string org) => {
                        if (Data.RAM.Byte(Settings.Constants[f]) == 255)
                        {
                            if (Data.RAM.Byte(Settings.Constants[s]) == 255)
                                inter(org);
                            new INC { to = Settings.Constants[s] }.Run();
                        }
                        new INC { to = Settings.Constants[f] }.Run();
                    };

                    if (key == 'P' && set("EX0", "IT0"))
                        inter("0003H"); // Interrupt 0
                    if (key == 'O' && set("EX1", "IT1"))
                        inter("0013H"); // Interrupt 1

                    if (set("ET0", "TR0"))
                        timer("TL0", "TH0", "000BH"); // Interrupt T0

                    if (set("ET1", "TR1"))
                        timer("TL1", "TH1", "001BH"); // Interrupt T1
                }
            }
        }

        void Output()
        {
            Action<string> print = (string o) => {
                Console.WriteLine(o + " : " + Convert.ToString(Data.RAM.Byte(Settings.Constants[o]), 2).PadLeft(8, '0'));
            };

            Action<string, int, int> segment = (string s, int x, int y) => {

                var data = Convert.ToString(Data.RAM.Byte(Settings.Constants[s]), 2).PadLeft(8, '0');

                Action<char, int, int> p = (char o, int x1, int y1) => {
                    Console.SetCursorPosition(x + x1, y + y1);
                    Console.Write(o);
                };

                p('+', 0, 0);
                p('+', 2, 0);
                p('+', 0, 2);
                p('+', 2, 2);
                p('+', 0, 4);
                p('+', 2, 4);

                if (data[0] == '1') p('.', 3, 4); else p(' ', 3, 4);
                if (data[1] == '1') p('-', 1, 2); else p(' ', 1, 2);
                if (data[2] == '1') p('|', 0, 1); else p(' ', 0, 1);
                if (data[3] == '1') p('|', 0, 3); else p(' ', 0, 3);
                if (data[4] == '1') p('-', 1, 4); else p(' ', 1, 4);
                if (data[5] == '1') p('|', 2, 3); else p(' ', 2, 3);
                if (data[6] == '1') p('|', 2, 1); else p(' ', 2, 1);
                if (data[7] == '1') p('-', 1, 0); else p(' ', 1, 0);
            };

            Console.SetCursorPosition(5, 5);
            print("P0");
            Console.SetCursorPosition(5, 7);
            print("P2");

            segment("P0", 5, 10);
            segment("P2", 15, 10);

            Console.SetCursorPosition(5, 3);
            Console.WriteLine("                                     ");
            Console.SetCursorPosition(5, 3);
            Console.WriteLine(Data.Commands[Data.ProgramCounter].Desc());

            Console.SetCursorPosition(0, 0);
        }

        static string[] GetContent(string path)
        {
            if (!File.Exists(path)) File.Create(path).Close();
            return File.ReadAllLines(path);
        }

    }
}
