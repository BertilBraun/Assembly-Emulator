using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assembly_Emulator
{
    abstract class Command
    {
        public abstract void Run();
    }

    class MOV : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(from));
        }
    }

    class MOV_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, num);
        }
    }

    class MOV_AT_FROM : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(Program.RAM.Byte(from)));
        }
    }

    class MOV_AT_TO : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(Program.RAM.Byte(to), Program.RAM.Byte(from));
        }
    }

    class MOV_DPTR : Command
    {
        public override void Run()
        {
            var address = Program.RAM.Byte(Settings.Constants["A"]) + Program.RAM.Byte(Settings.Constants["DPTR"]);

            Program.RAM.Byte(Settings.Constants["A"], Program.ROM.Byte(address));
        }
    }
    class MOV_PC : Command
    {
        public override void Run()
        {
            var address = Program.RAM.Byte(Settings.Constants["A"]) + Program.RAM.Byte(Program.ProgramCounter);

            Program.RAM.Byte(Settings.Constants["A"], Program.ROM.Byte(address));
        }
    }
    class MOV_B : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Bit(to, Program.RAM.Bit(from));
        }
    }

    class ANL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) & Program.RAM.Byte(from));
        }
    }
    class ANL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) & Program.RAM.Byte(Program.RAM.Byte(from)));
        }
    }
    class ANL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) & num);
        }
    }

    class ORL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) | Program.RAM.Byte(from));
        }
    }
    class ORL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) | Program.RAM.Byte(Program.RAM.Byte(from)));
        }
    }
    class ORL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) | num);
        }
    }

    class XRL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) ^ Program.RAM.Byte(from));
        }
    }
    class XRL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) ^ Program.RAM.Byte(Program.RAM.Byte(from)));
        }
    }
    class XRL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) ^ num);
        }
    }

    class CPL_A : Command
    {
        public override void Run()
        {
            Program.RAM.Byte(Settings.Constants["A"], ~Program.RAM.Byte(Settings.Constants["A"]));
        }
    }
    class CLR_A : Command
    {
        public override void Run()
        {
            Program.RAM.Byte(Settings.Constants["A"], 0);
        }
    }

    class CPL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.RAM.Bit(address, Program.RAM.Bit(address) == 0 ? 1 : 0);
        }
    }
    class CLR_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.RAM.Bit(address, 0);
        }
    }

    class SETB : Command
    {
        public int address;

        public override void Run()
        {
            Program.RAM.Bit(address, 1);
        }
    }

    class ORL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.RAM.Bit(Settings.Constants["C"], Program.RAM.Bit(Settings.Constants["C"]) | Program.RAM.Bit(address));
        }
    }

    class ANL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.RAM.Bit(Settings.Constants["C"], Program.RAM.Bit(Settings.Constants["C"]) & Program.RAM.Bit(address));
        }
    }

    class SWAP : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            Program.RAM.Byte(Settings.Constants["A"], ((A & 0x0F) << 4 | (A & 0xF0) >> 4));
        }
    }

    class PUSH : Command
    {
        public int from;

        public override void Run()
        {
            Program.Stack.Enqueue(Program.RAM.Byte(from));
        }
    }

    class POP : Command
    {
        public int to;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.Stack.Dequeue());
        }
    }

    class JMP : Command
    {
        public int to;

        public override void Run()
        {
            Program.ProgramCounter = to;
        }
    }

    class JMP_A_DPTR : Command
    {
        public override void Run()
        {
            var adptr = Program.RAM.Byte(Settings.Constants["A"]) + Program.RAM.Byte(Settings.Constants["DPTR"]);
            var address = Program.RAM.Byte(adptr);

            Program.ProgramCounter = Program.RAM.Byte(address);
        }
    }

    class CALL : Command
    {
        public int to;

        public override void Run()
        {
            Program.Stack.Enqueue(Program.ProgramCounter);
            Program.ProgramCounter = to;
        }
    }

    class RET : Command
    {
        public override void Run()
        {
            Program.ProgramCounter = Program.Stack.Dequeue();
        }
    }

    class RETI : Command
    {
        public override void Run()
        {
            Program.ProgramCounter = Program.Stack.Dequeue();
        }
    }

    class ADD : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + Program.RAM.Byte(from));
        }
    }
    class ADD_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + Program.RAM.Byte(Program.RAM.Byte(from)));
        }
    }
    class ADD_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + num);
        }
    }

    class ADDC : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + Program.RAM.Byte(from) + Program.RAM.Bit(Settings.Constants["C"]));
        }
    }
    class ADDC_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + Program.RAM.Byte(Program.RAM.Byte(from)) + Program.RAM.Bit(Settings.Constants["C"]));
        }
    }
    class ADDC_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + num + Program.RAM.Bit(Settings.Constants["C"]));
        }
    }

    class INC : Command
    {
        public int to;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + 1);
        }
    }
    class INC_AT : Command
    {
        public int to;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) + 1);
        }
    }

    class SUBB : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) - (Program.RAM.Byte(from) + Program.RAM.Bit(Settings.Constants["C"])));
        }
    }
    class SUBB_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) - (Program.RAM.Byte(Program.RAM.Byte(from)) + Program.RAM.Bit(Settings.Constants["C"])));
        }
    }
    class SUBB_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) - (num + Program.RAM.Bit(Settings.Constants["C"])));
        }
    }

    class DEC : Command
    {
        public int to;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) - 1);
        }
    }
    class DEC_AT : Command
    {
        public int to;

        public override void Run()
        {
            Program.RAM.Byte(to, Program.RAM.Byte(to) - 1);
        }
    }

    class MUL : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            var B = Program.RAM.Byte(Settings.Constants["B"]);

            var AB = A * B;

            Program.RAM.Byte(Settings.Constants["A"], AB % 256);
            Program.RAM.Byte(Settings.Constants["B"], AB / 256);
            Program.RAM.Bit(Settings.Constants["C"], 0);
        }
    }

    class DIV : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            var B = Program.RAM.Byte(Settings.Constants["B"]);

            Program.RAM.Byte(Settings.Constants["A"], A / B);
            Program.RAM.Byte(Settings.Constants["B"], A % B);
            Program.RAM.Bit(Settings.Constants["C"], 0);
        }
    }

    class JB : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.RAM.Bit(address) != 0)
                Program.ProgramCounter = to;
        }
    }
    class JNB : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.RAM.Bit(address) != 0)
                Program.ProgramCounter = to;
        }
    }
    class JBC : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.RAM.Bit(address) != 0)
            {
                Program.ProgramCounter = to;
                Program.RAM.Bit(address, 0);
            }
        }
    }

    class JComp : Command
    {
        public int to, compareTo, toCompare;

        public override void Run()
        {
            if (Program.RAM.Byte(compareTo) == Program.RAM.Byte(toCompare))
                Program.ProgramCounter = to;
        }
    }
    class JNComp : Command
    {
        public int to, compareTo, toCompare;

        public override void Run()
        {
            if (Program.RAM.Byte(compareTo) != Program.RAM.Byte(toCompare))
                Program.ProgramCounter = to;
        }
    }
    class JComp_NUM : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.RAM.Byte(compareTo) == num)
                Program.ProgramCounter = to;
        }
    }
    class JNComp_NUM : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.RAM.Byte(compareTo) != num)
                Program.ProgramCounter = to;
        }
    }
    class JComp_AT : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.RAM.Byte(Program.RAM.Byte(compareTo)) == num)
                Program.ProgramCounter = to;
        }
    }
    class JNComp_AT : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.RAM.Byte(Program.RAM.Byte(compareTo)) != num)
                Program.ProgramCounter = to;
        }
    }
    class DJNZ : Command
    {
        public int to, compareTo;

        public override void Run()
        {
            if (Program.RAM.Byte(compareTo) != 0)
                Program.ProgramCounter = to;
            Program.RAM.Byte(compareTo, Program.RAM.Byte(compareTo) - 1);
        }
    }

    class XCH : Command
    {
        public int from;

        public override void Run()
        {
            var T = Program.RAM.Byte(from);
            Program.RAM.Byte(from, Program.RAM.Byte(Settings.Constants["A"]));
            Program.RAM.Byte(Settings.Constants["A"], T);
        }
    }
    class XCH_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            var FA = Program.RAM.Byte(from);
            var T = Program.RAM.Byte(FA);
            Program.RAM.Byte(FA, Program.RAM.Byte(Settings.Constants["A"]));
            Program.RAM.Byte(Settings.Constants["A"], T);
        }
    }
    class XCHD_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            var FA = Program.RAM.Byte(from);
            var F = Program.RAM.Byte(FA);
            var S = Program.RAM.Byte(Settings.Constants["A"]);
            Program.RAM.Byte(FA, (S & 0x0F) | (F & 0xF0));
            Program.RAM.Byte(Settings.Constants["A"], (F & 0x0F) | (S & 0xF0));
        }
    }
    class RL : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            Program.RAM.Byte(Settings.Constants["A"], (A << 1) | (A >> 7));
        }
    }
    class RLC : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            var C = Program.RAM.Bit(Settings.Constants["C"]);

            Program.RAM.Bit(Settings.Constants["C"], ((A & 0x80) != 0) ? 1 : 0);
            Program.RAM.Byte(Settings.Constants["A"], (A << 1) | C);
        }
    }
    class RR : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            Program.RAM.Byte(Settings.Constants["A"], (A >> 1) | (A << 7));
        }
    }
    class RRC : Command
    {
        public override void Run()
        {
            var A = Program.RAM.Byte(Settings.Constants["A"]);
            var C = Program.RAM.Bit(Settings.Constants["C"]);

            Program.RAM.Bit(Settings.Constants["C"], ((A & 0x01) != 0) ? 1 : 0);
            Program.RAM.Byte(Settings.Constants["A"], (A >> 1) | (C << 7));
        }
    }


    class PRINT : Command
    {
        public int address;

        public override void Run()
        {
            var D = Program.RAM.Byte(address);
            
            if (Settings.Constants.ContainsValue(address))
                Console.Write(Settings.Constants.FirstOrDefault(x => x.Value == address).Key);
            else
                Console.Write(address);
            Console.WriteLine(" : " + Convert.ToString(D, 2).PadLeft(8, '0'));
        }
    }
}
