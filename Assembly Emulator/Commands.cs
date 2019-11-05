using System;
using System.Linq;

namespace Assembly_Emulator
{
    abstract class Command
    {
        public abstract void Run();
		public abstract string Desc();
		
		protected string Addr(int addr) {
            if (Settings.Constants.ContainsValue(addr))
                return Settings.Constants.FirstOrDefault(x => x.Value == addr).Key;			
			return addr.ToString();
        }
        protected string Pos(int addr)
        {
            if (Program.Data.JumpPositions.ContainsValue(addr))
                return Program.Data.JumpPositions.FirstOrDefault(x => x.Value == addr).Key;
            return addr.ToString();
        }
    }

    class MOV : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "MOV " + Addr(to) + ", " + Addr(from);
        }
    }

    class MOV_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, num);
        }
        public override string Desc()
        {
			return "MOV " + Addr(to) + ", " + num.ToString();
        }
    }

    class MOV_AT_FROM : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)));
        }
        public override string Desc()
        {
			return "MOV " + Addr(to) + ", @" + Addr(from);
        }
    }

    class MOV_AT_TO : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(Program.Data.RAM.Byte(to), Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "MOV @" + Addr(to) + ", " + Addr(from);
        }
    }

    class MOV_DPTR : Command
    {
        public override void Run()
        {
            var address = Program.Data.RAM.Byte(Settings.Constants["A"]) + Program.Data.RAM.Byte(Settings.Constants["DPTR"]);

            Program.Data.RAM.Byte(Settings.Constants["A"], Program.Data.ROM.Byte(address));
        }
        public override string Desc()
        {
			return "MOV A, @A+DPTR";
        }
    }
    class MOV_PC : Command
    {
        public override void Run()
        {
            var address = Program.Data.RAM.Byte(Settings.Constants["A"]) + Program.Data.RAM.Byte(Program.Data.ProgramCounter);

            Program.Data.RAM.Byte(Settings.Constants["A"], Program.Data.ROM.Byte(address));
        }
        public override string Desc()
        {
			return "MOV A, @A+PC";
        }
    }
    class MOV_B : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Bit(to, Program.Data.RAM.Bit(from));
        }
        public override string Desc()
        {
			return "MOV " + Addr(to) + ", " + Addr(from);
        }
    }

    class ANL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) & Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "ANL " + Addr(to) + ", " + Addr(from);
        }
    }
    class ANL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) & Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)));
        }
        public override string Desc()
        {
			return "ANL " + Addr(to) + ", @" + Addr(from);
        }
    }
    class ANL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) & num);
        }
        public override string Desc()
        {
			return "ANL " + Addr(to) + ", " + num.ToString();
        }
    }

    class ORL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) | Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "ORL " + Addr(to) + ", " + Addr(from);
        }
    }
    class ORL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) | Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)));
        }
        public override string Desc()
        {
			return "ORL " + Addr(to) + ", @" + Addr(from);
        }
    }
    class ORL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) | num);
        }
        public override string Desc()
        {
			return "ORL " + Addr(to) + ", " + num.ToString();
        }
    }

    class XRL : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) ^ Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "XRL " + Addr(to) + ", " + Addr(from);
        }
    }
    class XRL_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) ^ Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)));
        }
        public override string Desc()
        {
			return "XRL " + Addr(to) + ", @" + Addr(from);
        }
    }
    class XRL_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) ^ num);
        }
        public override string Desc()
        {
			return "XRL " + Addr(to) + ", " + num.ToString();
        }
    }

    class CPL_A : Command
    {
        public override void Run()
        {
            Program.Data.RAM.Byte(Settings.Constants["A"], ~Program.Data.RAM.Byte(Settings.Constants["A"]));
        }
        public override string Desc()
        {
			return "CPL A";
        }
    }
    class CLR_A : Command
    {
        public override void Run()
        {
            Program.Data.RAM.Byte(Settings.Constants["A"], 0);
        }
        public override string Desc()
        {
			return "CLR A";
        }
    }

    class CPL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.Data.RAM.Bit(address, Program.Data.RAM.Bit(address) == 0 ? 1 : 0);
        }
        public override string Desc()
        {
			return "CPL B";
        }
    }
    class CLR_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.Data.RAM.Bit(address, 0);
        }
        public override string Desc()
        {
			return "CLR B";
        }
    }

    class SETB : Command
    {
        public int address;

        public override void Run()
        {
            Program.Data.RAM.Bit(address, 1);
        }
        public override string Desc()
        {
			return "SETB " + Addr(address);
        }
    }

    class ORL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.Data.RAM.Bit(Settings.Constants["C"], Program.Data.RAM.Bit(Settings.Constants["C"]) | Program.Data.RAM.Bit(address));
        }
        public override string Desc()
        {
			return "ORL C, " + Addr(address);
        }
    }

    class ANL_B : Command
    {
        public int address;

        public override void Run()
        {
            Program.Data.RAM.Bit(Settings.Constants["C"], Program.Data.RAM.Bit(Settings.Constants["C"]) & Program.Data.RAM.Bit(address));
        }
        public override string Desc()
        {
			return "ANL C, " + Addr(address);
        }
    }

    class SWAP : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            Program.Data.RAM.Byte(Settings.Constants["A"], ((A & 0x0F) << 4 | (A & 0xF0) >> 4));
        }
        public override string Desc()
        {
			return "SWAP";
        }
    }

    class PUSH : Command
    {
        public int from;

        public override void Run()
        {
            Program.Data.Stack.Enqueue(Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "PUSH " + Addr(from);
        }
    }

    class POP : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.Stack.Dequeue());
        }
        public override string Desc()
        {
			return "POP " + Addr(to);
        }
    }

    class JMP : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "JMP " + Pos(to);
        }
    }

    class JMP_A_DPTR : Command
    {
        public override void Run()
        {
            var adptr = Program.Data.RAM.Byte(Settings.Constants["A"]) + Program.Data.RAM.Byte(Settings.Constants["DPTR"]);
            var address = Program.Data.RAM.Byte(adptr);

            Program.Data.ProgramCounter = Program.Data.RAM.Byte(address);
        }
        public override string Desc()
        {
			return "JMP @A+DPTR";
        }
    }

    class CALL : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.Stack.Enqueue(Program.Data.ProgramCounter);
            Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CALL " + Pos(to);
        }
    }

    class RET : Command
    {
        public override void Run()
        {
            Program.Data.ProgramCounter = Program.Data.Stack.Dequeue();
        }
        public override string Desc()
        {
			return "RET";
        }
    }

    class RETI : Command
    {
        public override void Run()
        {
            Program.Data.ProgramCounter = Program.Data.Stack.Dequeue();
        }
        public override string Desc()
        {
			return "RETI";
        }
    }

    class ADD : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + Program.Data.RAM.Byte(from));
        }
        public override string Desc()
        {
			return "ADD " + Addr(to) + ", " + Addr(from);
        }
    }
    class ADD_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)));
        }
        public override string Desc()
        {
			return "ADD " + Addr(to) + ", @" + Addr(from);
        }
    }
    class ADD_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + num);
        }
        public override string Desc()
        {
			return "ADD " + Addr(to) + ", " + num.ToString();
        }
    }

    class ADDC : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + Program.Data.RAM.Byte(from) + Program.Data.RAM.Bit(Settings.Constants["C"]));
        }
        public override string Desc()
        {
			return "ADDC " + Addr(to) + ", " + Addr(from);
        }
    }
    class ADDC_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)) + Program.Data.RAM.Bit(Settings.Constants["C"]));
        }
        public override string Desc()
        {
			return "ADDC " + Addr(to) + ", @" + Addr(from);
        }
    }
    class ADDC_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + num + Program.Data.RAM.Bit(Settings.Constants["C"]));
        }
        public override string Desc()
        {
			return "ADDC " + Addr(to) + ", " + num.ToString();
        }
    }

    class INC : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) + 1);
        }
        public override string Desc()
        {
			return "INC " + Addr(to);
        }
    }
    class INC_AT : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(Program.Data.RAM.Byte(to)) + 1);
        }
        public override string Desc()
        {
			return "INC @" + Addr(to);
        }
    }

    class SUBB : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) - (Program.Data.RAM.Byte(from) + Program.Data.RAM.Bit(Settings.Constants["C"])));
        }
        public override string Desc()
        {
			return "SUBB " + Addr(to) + ", " + Addr(from);
        }
    }
    class SUBB_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) - (Program.Data.RAM.Byte(Program.Data.RAM.Byte(from)) + Program.Data.RAM.Bit(Settings.Constants["C"])));
        }
        public override string Desc()
        {
			return "SUBB " + Addr(to) + ", @" + Addr(from);
        }
    }
    class SUBB_NUM : Command
    {
        public int to, num;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) - (num + Program.Data.RAM.Bit(Settings.Constants["C"])));
        }
        public override string Desc()
        {
			return "SUBB " + Addr(to) + ", " + num.ToString();
        }
    }

    class DEC : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) - 1);
        }
        public override string Desc()
        {
			return "DEC " + Addr(to);
        }
    }
    class DEC_AT : Command
    {
        public int to;

        public override void Run()
        {
            Program.Data.RAM.Byte(to, Program.Data.RAM.Byte(to) - 1);
        }
        public override string Desc()
        {
			return "DEC @" + Addr(to);
        }
    }

    class MUL : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            var B = Program.Data.RAM.Byte(Settings.Constants["B"]);

            var AB = A * B;

            Program.Data.RAM.Byte(Settings.Constants["A"], AB % 256);
            Program.Data.RAM.Byte(Settings.Constants["B"], AB / 256);
            Program.Data.RAM.Bit(Settings.Constants["C"], 0);
        }
        public override string Desc()
        {
			return "MUL AB";
        }
    }

    class DIV : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            var B = Program.Data.RAM.Byte(Settings.Constants["B"]);

            Program.Data.RAM.Byte(Settings.Constants["A"], A / B);
            Program.Data.RAM.Byte(Settings.Constants["B"], A % B);
            Program.Data.RAM.Bit(Settings.Constants["C"], 0);
        }
        public override string Desc()
        {
			return "DIV AB";
        }
    }

    class JB : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.Data.RAM.Bit(address) != 0)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "JB " + Addr(to) + ", " + Addr(address);
        }
    }
    class JNB : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.Data.RAM.Bit(address) != 0)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "JNB " + Addr(to) + ", " + Addr(address);
        }
    }
    class JBC : Command
    {
        public int to, address;

        public override void Run()
        {
            if (Program.Data.RAM.Bit(address) != 0)
            {
                Program.Data.ProgramCounter = to;
                Program.Data.RAM.Bit(address, 0);
            }
        }
        public override string Desc()
        {
			return "JBC " + Addr(to) + ", " + Addr(address);
        }
    }

    class JComp : Command
    {
        public int to, compareTo, toCompare;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(compareTo) == Program.Data.RAM.Byte(toCompare))
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJE " + Addr(to) + ", " + Addr(compareTo) + ", " + Addr(toCompare);
        }
    }
    class JNComp : Command
    {
        public int to, compareTo, toCompare;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(compareTo) != Program.Data.RAM.Byte(toCompare))
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJNE " + Addr(compareTo) + ", " + Addr(toCompare) + ", " + Addr(to);
        }
    }
    class JComp_NUM : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(compareTo) == num)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJE " + Addr(compareTo) + ", " + num.ToString() + ", " + Addr(to);
        }
    }
    class JNComp_NUM : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(compareTo) != num)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJNE " + Addr(compareTo) + ", " + num.ToString() + ", " + Addr(to);
        }
    }
    class JComp_AT : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(Program.Data.RAM.Byte(compareTo)) == num)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJE @" + Addr(compareTo) + ", " + num.ToString() + ", " + Addr(to);
        }
    }
    class JNComp_AT : Command
    {
        public int to, compareTo, num;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(Program.Data.RAM.Byte(compareTo)) != num)
                Program.Data.ProgramCounter = to;
        }
        public override string Desc()
        {
			return "CJNE @" + Addr(compareTo) + ", " + num.ToString() + ", " + Addr(to);
        }
    }
    class DJNZ : Command
    {
        public int to, compareTo;

        public override void Run()
        {
            if (Program.Data.RAM.Byte(compareTo) != 0)
                Program.Data.ProgramCounter = to;
            Program.Data.RAM.Byte(compareTo, Program.Data.RAM.Byte(compareTo) - 1);
        }
        public override string Desc()
        {
			return "DJNZ " + Addr(compareTo) + ", " + Addr(to);
        }
    }

    class XCH : Command
    {
        public int from;

        public override void Run()
        {
            var T = Program.Data.RAM.Byte(from);
            Program.Data.RAM.Byte(from, Program.Data.RAM.Byte(Settings.Constants["A"]));
            Program.Data.RAM.Byte(Settings.Constants["A"], T);
        }
        public override string Desc()
        {
			return "XCH " + Addr(from);
        }
    }
    class XCH_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            var FA = Program.Data.RAM.Byte(from);
            var T = Program.Data.RAM.Byte(FA);
            Program.Data.RAM.Byte(FA, Program.Data.RAM.Byte(Settings.Constants["A"]));
            Program.Data.RAM.Byte(Settings.Constants["A"], T);
        }
        public override string Desc()
        {
			return "XCH " + Addr(to) + ", @" + Addr(from);
        }
    }
    class XCHD_AT : Command
    {
        public int to, from;

        public override void Run()
        {
            var FA = Program.Data.RAM.Byte(from);
            var F = Program.Data.RAM.Byte(FA);
            var S = Program.Data.RAM.Byte(Settings.Constants["A"]);
            Program.Data.RAM.Byte(FA, (S & 0x0F) | (F & 0xF0));
            Program.Data.RAM.Byte(Settings.Constants["A"], (F & 0x0F) | (S & 0xF0));
        }
        public override string Desc()
        {
			return "XCHD " + Addr(to) + ", @" + Addr(from);
        }
    }
    class RL : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            Program.Data.RAM.Byte(Settings.Constants["A"], (A << 1) | (A >> 7));
        }
        public override string Desc()
        {
			return "RL";
        }
    }
    class RLC : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            var C = Program.Data.RAM.Bit(Settings.Constants["C"]);

            Program.Data.RAM.Bit(Settings.Constants["C"], ((A & 0x80) != 0) ? 1 : 0);
            Program.Data.RAM.Byte(Settings.Constants["A"], (A << 1) | C);
        }
        public override string Desc()
        {
			return "RLC";
        }
    }
    class RR : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            Program.Data.RAM.Byte(Settings.Constants["A"], (A >> 1) | (A << 7));
        }
        public override string Desc()
        {
			return "RR";
        }
    }
    class RRC : Command
    {
        public override void Run()
        {
            var A = Program.Data.RAM.Byte(Settings.Constants["A"]);
            var C = Program.Data.RAM.Bit(Settings.Constants["C"]);

            Program.Data.RAM.Bit(Settings.Constants["C"], ((A & 0x01) != 0) ? 1 : 0);
            Program.Data.RAM.Byte(Settings.Constants["A"], (A >> 1) | (C << 7));
        }
        public override string Desc()
        {
			return "RRC";
        }
    }
	
    class PRINT : Command
    {
        public int address;

        public override void Run()
        {
            var D = Program.Data.RAM.Byte(address);
            
            if (Settings.Constants.ContainsValue(address))
                Console.Write(Settings.Constants.FirstOrDefault(x => x.Value == address).Key);
            else
                Console.Write(address);
            Console.WriteLine(" : " + Convert.ToString(D, 2).PadLeft(8, '0'));
        }
        public override string Desc()
        {
			return "PRINT " + Addr(address);
        }
    }
    class PRINT_STR : Command
    {
        public string str;

        public override void Run()
        {
            Console.WriteLine(str);
        }
        public override string Desc()
        {
			return "PRINT " + str;
        }
    }
}
