using System;
using System.Collections.Generic;
using System.IO;

namespace Assembly_Emulator
{
    class Program
    {
        /// <summary>
        /// TODOs
        /// 
        /// Interrupts
        /// 
        /// org
        /// Includes
        /// defines
        /// databyte
        /// 
        /// </summary>

        public static int ProgramCounter = 0;
        public static Ram RAM = new Ram(4 * 1024);
        public static Queue<int> Stack = new Queue<int>();
        public static List<Command> Commands;

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

            Commands = new Parser().Create(GetContent(path));

            while (true)
            {
                while (ProgramCounter < Commands.Count)
                {
                    Input();
                    Commands[ProgramCounter++].Run();
                    Output();

                    System.Threading.Thread.Sleep(100);
                }

                Parser.Commands.Clear();

                string command = Console.ReadLine();
                if (command.ToUpper() == "Q")
                    break;

                Commands.AddRange(new Parser().Create(new string[] { command }));
            }
        }

        static void Input()
        {
            if (Console.KeyAvailable)
            {
                var key = char.ToUpper(Console.ReadKey(true).KeyChar);

                RAM.Bit(Settings.Constants["P1.0"], key == '0'); // Keyboard pressed Key 0
                RAM.Bit(Settings.Constants["P1.1"], key == '1'); // Keyboard pressed Key 1
                RAM.Bit(Settings.Constants["P1.2"], key == '2'); // Keyboard pressed Key 2
                RAM.Bit(Settings.Constants["P1.3"], key == '3'); // Keyboard pressed Key 3
                RAM.Bit(Settings.Constants["P1.4"], key == '4'); // Keyboard pressed Key 4
                RAM.Bit(Settings.Constants["P1.5"], key == '5'); // Keyboard pressed Key 5
                RAM.Bit(Settings.Constants["P1.6"], key == '6'); // Keyboard pressed Key 6
                RAM.Bit(Settings.Constants["P1.7"], key == '7'); // Keyboard pressed Key 7

                RAM.Bit(Settings.Constants["P3.2"], key == 'P'); // Keyboard pressed Key P
                RAM.Bit(Settings.Constants["P3.3"], key == 'O'); // Keyboard pressed Key O

                if (key == 'P') ; // Interrupt 0
                if (key == 'O') ; // Interrupt 1
            }
        }

        static void Output()
        {
            Action<string> print = (string o) => {
                Console.WriteLine(o + " : " + Convert.ToString(RAM.Byte(Settings.Constants[o]), 2).PadLeft(8, '0'));
            };

            Action<string, int, int> segment = (string s, int x, int y) => {

                var data = Convert.ToString(RAM.Byte(Settings.Constants[s]), 2).PadLeft(8, '0');

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
            Console.WriteLine(Commands[ProgramCounter].GetType().Name);

            Console.SetCursorPosition(0, 0);
        }

        static string[] GetContent(string path)
        {
            if (!File.Exists(path)) File.Create(path).Close();
            return File.ReadAllLines(path);
        }

    }
}
