using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assembly_Emulator
{
    class Parser
    {
        static string C = Settings.Constants["C"].ToString();
        static string A = Settings.Constants["A"].ToString();
        static char[] delim = new char[] { ' ', '\t', ',', ';' };
        
        Dictionary<string, int> Constants = new Dictionary<string, int>(Settings.Constants);
        int commandCount = 0;

        public void Create(string[] lines)
        {
            var tokens = new List<string[]>();
            var content = lines.Select(c => c.ToUpper());

            foreach (var c in content)
            {
                var parsed = ParseTokens(c.Split(delim, StringSplitOptions.RemoveEmptyEntries));
                if (parsed.Length > 0)
                    tokens.Add(parsed);
            }

            foreach (var t in tokens)
                if (CommandDict.ContainsKey(t[0]))
                     CommandDict[t[0]](t);
        }

        string[] ParseTokens(string[] tokens)
        {
            var cleanTokens = new List<string>();
            
            if (tokens.Length == 0)
                return cleanTokens.ToArray();

            foreach (var t in tokens)
            {
                string toAdd = t;
                if (t.Contains(';'))
                    toAdd = t.Substring(0, t.IndexOf(';'));

                if (toAdd.EndsWith(':'))
                    Program.Data.JumpPositions.Add(toAdd.TrimEnd(':'), commandCount);

                else if (toAdd[0] == '@' && Constants.ContainsKey(toAdd.Substring(1)))
                    toAdd = "@" + Constants[toAdd.Substring(1)].ToString();

                else if (Constants.ContainsKey(toAdd))
                    toAdd = Constants[toAdd].ToString();

                cleanTokens.Add(toAdd);

                if (t.Contains(';'))
                    break;
            }

            if (tokens.Length > 1 && tokens[1] == "EQE")
                Constants.Add(tokens[0], ParseNumber(tokens[2]));

            if (tokens[0].Substring(1) == "INCLUDE")
                new Parser().Create(File.ReadAllLines(tokens[1].Replace('\"', ' ')));

            if (tokens.Length > 0 && CommandDict.Keys.Contains(tokens[0]))
                commandCount++;

            if (tokens[0] == "ORG")
                Program.Data.Orgs[tokens[1]] = commandCount;

            if (tokens[0] == "DB")
                Program.Data.ROM.Byte(commandCount * 8, ParseNumber(tokens[1]));

            return cleanTokens.ToArray();
        }

        static int ParseNumber(string t)
        {
            var number = t.ToUpper();

            if (number.EndsWith('B'))
                return Convert.ToInt32(number.TrimEnd('B'), 2);

            if (number.EndsWith('H'))
                return Convert.ToInt32(number.TrimEnd('H'), 16);

            return Convert.ToInt32(number, 10);
        }

        Dictionary<string, Action<string[]>> CommandDict = new Dictionary<string, Action<string[]>>
        {
            { "MOV", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Program.Data.Commands.Add(new MOV_AT_TO { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new MOV_AT_FROM { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new MOV_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[1] == C || tokens[2] == C)
                        Program.Data.Commands.Add(new MOV_B { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
                    else
                        Program.Data.Commands.Add(new MOV { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "MOVX", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Program.Data.Commands.Add(new MOV_AT_TO { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new MOV_AT_FROM { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
            } },
            { "MOVC", tokens =>
            {
                    if (tokens[2] == "@A+DPTR")
                        Program.Data.Commands.Add(new MOV_DPTR());
                    if (tokens[2] == "@A+PC")
                        Program.Data.Commands.Add(new MOV_PC());
            } },
            { "XCH", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new XCH_AT { from = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new XCH { from = ParseNumber(tokens[2]) });
            } },
            { "XCHD", tokens =>
            {
                    Program.Data.Commands.Add(new XCHD_AT { from = ParseNumber(tokens[2].Substring(1)) });
            } },
            { "SWAP", tokens =>
            {
                    Program.Data.Commands.Add(new SWAP());
            } },
            { "PUSH", tokens =>
            {
                    Program.Data.Commands.Add(new PUSH  { from = ParseNumber(tokens[1]) });
            } },
            { "POP", tokens =>
            {
                    Program.Data.Commands.Add(new POP  { to = ParseNumber(tokens[1]) });
            } },
            { "ANL", tokens =>
            {
                    if (tokens[1] == C)
                        Program.Data.Commands.Add(new ANL_B { address = ParseNumber(tokens[2]) });
                    else if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new ANL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new ANL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new ANL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "ORL", tokens =>
            {
                    if (tokens[1] == C)
                        Program.Data.Commands.Add(new ORL_B { address = ParseNumber(tokens[2]) });
                    else if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new ORL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new ORL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new ORL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "XRL", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new XRL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new XRL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new XRL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "CPL", tokens =>
            {
                    if (tokens[1] == A)
                        Program.Data.Commands.Add(new CPL_A());
                    else
                        Program.Data.Commands.Add(new CPL_B { address = ParseNumber(tokens[1])});
            } },
            { "CLR", tokens =>
            {
                    if (tokens[1] == A)
                        Program.Data.Commands.Add(new CLR_A());
                    else
                        Program.Data.Commands.Add(new CLR_B { address = ParseNumber(tokens[1])});
            } },
            { "SETB", tokens =>
            {
                    Program.Data.Commands.Add(new SETB { address = ParseNumber(tokens[1])});
            } },
            { "ADD", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new ADD_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new ADD_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new ADD { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "ADDC", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new ADDC_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new ADDC_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new ADDC { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "INC", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Program.Data.Commands.Add(new INC_AT { to = ParseNumber(tokens[1]) });
                    else
                        Program.Data.Commands.Add(new INC { to = ParseNumber(tokens[1]) });
            } },
            { "SUBB", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Program.Data.Commands.Add(new SUBB_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Program.Data.Commands.Add(new SUBB_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Program.Data.Commands.Add(new SUBB { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "DEC", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Program.Data.Commands.Add(new DEC_AT { to = ParseNumber(tokens[1]) });
                    else
                        Program.Data.Commands.Add(new DEC { to = ParseNumber(tokens[1]) });
            } },
            { "MUL", tokens =>
            {
                    Program.Data.Commands.Add(new MUL());
            } },
            { "DIV", tokens =>
            {
                    Program.Data.Commands.Add(new DIV());
            } },
            { "RL", tokens =>
            {
                    Program.Data.Commands.Add(new RL());
            } },
            { "RR", tokens =>
            {
                    Program.Data.Commands.Add(new RR());
            } },
            { "RLC", tokens =>
            {
                    Program.Data.Commands.Add(new RLC());
            } },
            { "RRC", tokens =>
            {
                    Program.Data.Commands.Add(new RRC());
            } },
            { "LJMP", tokens =>
            {
                    Program.Data.Commands.Add(new JMP { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "SJMP", tokens =>
            {
                    Program.Data.Commands.Add(new JMP { to = Program.Data.Commands.Count + ParseNumber(tokens[1]) });
            } },
            { "AJMP", tokens =>
            {
                    Program.Data.Commands.Add(new JMP { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "JMP", tokens =>
            {
                    if (tokens[1] == "@A+DPTR")
                        Program.Data.Commands.Add(new JMP_A_DPTR());
                    else
                        Program.Data.Commands.Add(new JMP { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "JC", tokens =>
            {
                    Program.Data.Commands.Add(new JB { to = Program.Data.JumpPositions[tokens[1]], address = int.Parse(C) });
            } },
            { "JNC", tokens =>
            {
                    Program.Data.Commands.Add(new JNB { to = Program.Data.JumpPositions[tokens[1]], address = int.Parse(C) });
            } },
            { "JB", tokens =>
            {
                    Program.Data.Commands.Add(new JB { to = Program.Data.JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JNB", tokens =>
            {
                    Program.Data.Commands.Add(new JNB { to = Program.Data.JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JBC", tokens =>
            {
                    Program.Data.Commands.Add(new JBC { to = Program.Data.JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JZ", tokens =>
            {
                    Program.Data.Commands.Add(new JComp_NUM { to = ParseNumber(tokens[2]), compareTo = int.Parse(A), num = 0 });
            } },
            { "JNZ", tokens =>
            {
                    Program.Data.Commands.Add(new JNComp_NUM { to = ParseNumber(tokens[2]), compareTo = int.Parse(A), num = 1 });
            } },
            { "CJNE", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Program.Data.Commands.Add(new JNComp_AT { to = ParseNumber(tokens[3]), compareTo = ParseNumber(tokens[1]), num = ParseNumber(tokens[2]) });
                    else
                        Program.Data.Commands.Add(new JNComp { to = ParseNumber(tokens[3]), compareTo = ParseNumber(tokens[2]), toCompare = ParseNumber(tokens[1]) });
            } },
            { "DJNZ", tokens =>
            {
                    Program.Data.Commands.Add(new DJNZ { to = ParseNumber(tokens[2]), compareTo = ParseNumber(tokens[1]) });
            } },
            { "LCALL", tokens =>
            {
                    Program.Data.Commands.Add(new CALL { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "ACALL", tokens =>
            {
                    Program.Data.Commands.Add(new CALL { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "CALL", tokens =>
            {
                    Program.Data.Commands.Add(new CALL { to = Program.Data.JumpPositions[tokens[1]] });
            } },
            { "RET", tokens =>
            {
                    Program.Data.Commands.Add(new RET());
            } },
            { "RETI", tokens =>
            {
                    Program.Data.Commands.Add(new RETI());
            } },


            { "PRINT", tokens =>
            {
					if (tokens[1].Contains('\"'))
						Program.Data.Commands.Add(new PRINT_STR { str = tokens[1].Substring(1, tokens[1].Length - 1) });
					else
						Program.Data.Commands.Add(new PRINT { address = ParseNumber(tokens[1]) });
            } }
        };
    }
}