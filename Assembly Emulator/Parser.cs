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

        static List<Command> Commands = new List<Command>();
        static Dictionary<string, int> JumpPositions = new Dictionary<string, int>();
        static Dictionary<string, int> Constants = new Dictionary<string, int>(Settings.Constants);

        char[] delim = new char[] { ' ', '\t', ',', ';' };
        int commandCount = 0;

        public List<Command> Create(string path)
        {
            var tokens = new List<string[]>();
            var content = GetContent(path).Select(c => c.ToUpper());

            foreach (var c in content)
            {
                var parsed = ParseTokens(c.Split(delim, StringSplitOptions.RemoveEmptyEntries));
                if (parsed.Length > 0)
                    tokens.Add(parsed);
            }

            foreach (var t in tokens)
                if (CommandDict.ContainsKey(t[0]))
                     CommandDict[t[0]](t);

            return Commands;
        }

        string[] ParseTokens(string[] tokens)
        {
            var cleanTokens = new List<string>();

            foreach (var t in tokens)
            {
                string toAdd = t;
                if (t.Contains(';'))
                    toAdd = t.Substring(0, t.IndexOf(';'));

                if (toAdd.EndsWith(':'))
                    JumpPositions.Add(toAdd.TrimEnd(':'), commandCount);

                else if (toAdd[0] == '@' && Constants.ContainsKey(toAdd.Substring(1)))
                    toAdd = "@" + Constants[toAdd.Substring(1)].ToString();

                else if (Constants.ContainsKey(toAdd))
                    toAdd = Constants[toAdd].ToString();

                cleanTokens.Add(toAdd);

                if (t.Contains(';'))
                    break;
            }

            if (tokens.Length > 0 && CommandDict.Keys.Contains(tokens[0]))
                commandCount++;

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

        string[] GetContent(string path)
        {
            if (!File.Exists(path)) File.Create(path).Close();
            return File.ReadAllLines(path);
        }

        Dictionary<string, Action<string[]>> CommandDict = new Dictionary<string, Action<string[]>>
        {
            { "MOV", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Commands.Add(new MOV_AT_TO { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('@'))
                        Commands.Add(new MOV_AT_FROM { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new MOV_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[1] == C || tokens[2] == C)
                        Commands.Add(new MOV_B { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
                    else
                        Commands.Add(new MOV { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "MOVX", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Commands.Add(new MOV_AT_TO { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new MOV_AT_FROM { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
            } },
            { "XCH", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new XCH_AT { from = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new XCH { from = ParseNumber(tokens[2]) });
            } },
            { "XCHD", tokens =>
            {
                    Commands.Add(new XCHD_AT { from = ParseNumber(tokens[2].Substring(1)) });
            } },
            { "SWAP", tokens =>
            {
                    Commands.Add(new SWAP());
            } },
            { "PUSH", tokens =>
            {
                    Commands.Add(new PUSH  { from = ParseNumber(tokens[1]) });
            } },
            { "POP", tokens =>
            {
                    Commands.Add(new POP  { to = ParseNumber(tokens[1]) });
            } },
            { "MOVC", tokens =>
            {
                    if (tokens[2] == "@A+DPTR")
                        Commands.Add(new MOV_DPTR());
                    if (tokens[2] == "@A+PC")
                        Commands.Add(new MOV_PC());
            } },
            { "ANL", tokens =>
            {
                    if (tokens[1] == C)
                        Commands.Add(new ANL_B { address = ParseNumber(tokens[2]) });
                    else if (tokens[2].StartsWith('@'))
                        Commands.Add(new ANL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new ANL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new ANL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "ORL", tokens =>
            {
                    if (tokens[1] == C)
                        Commands.Add(new ORL_B { address = ParseNumber(tokens[2]) });
                    else if (tokens[2].StartsWith('@'))
                        Commands.Add(new ORL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new ORL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new ORL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "XRL", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new XRL_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new XRL_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new XRL { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "CPL", tokens =>
            {
                    if (tokens[1] == A)
                        Commands.Add(new CPL_A());
                    else
                        Commands.Add(new CPL_B { address = ParseNumber(tokens[1])});
            } },
            { "CLR", tokens =>
            {
                    if (tokens[1] == A)
                        Commands.Add(new CLR_A());
                    else
                        Commands.Add(new CLR_B { address = ParseNumber(tokens[1])});
            } },
            { "SETB", tokens =>
            {
                    Commands.Add(new SETB { address = ParseNumber(tokens[1])});
            } },
            { "ADD", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new ADD_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new ADD_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new ADD { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "ADDC", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new ADDC_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new ADDC_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new ADDC { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "INC", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Commands.Add(new INC_AT { to = ParseNumber(tokens[1]) });
                    else
                        Commands.Add(new INC { to = ParseNumber(tokens[1]) });
            } },
            { "SUBB", tokens =>
            {
                    if (tokens[2].StartsWith('@'))
                        Commands.Add(new SUBB_AT { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2].Substring(1)) });
                    else if (tokens[2].StartsWith('#'))
                        Commands.Add(new SUBB_NUM { to = ParseNumber(tokens[1]), num = ParseNumber(tokens[2].Substring(1)) });
                    else
                        Commands.Add(new SUBB { to = ParseNumber(tokens[1]), from = ParseNumber(tokens[2]) });
            } },
            { "DEC", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Commands.Add(new DEC_AT { to = ParseNumber(tokens[1]) });
                    else
                        Commands.Add(new DEC { to = ParseNumber(tokens[1]) });
            } },
            { "MUL", tokens =>
            {
                    Commands.Add(new MUL());
            } },
            { "DIV", tokens =>
            {
                    Commands.Add(new DIV());
            } },
            { "RL", tokens =>
            {
                    Commands.Add(new RL());
            } },
            { "RR", tokens =>
            {
                    Commands.Add(new RR());
            } },
            { "RLC", tokens =>
            {
                    Commands.Add(new RLC());
            } },
            { "RRC", tokens =>
            {
                    Commands.Add(new RRC());
            } },
            { "LJMP", tokens =>
            {
                    Commands.Add(new JMP { to = JumpPositions[tokens[1]] });
            } },
            { "SJMP", tokens =>
            {
                    Commands.Add(new JMP { to = Commands.Count + ParseNumber(tokens[1]) });
            } },
            { "AJMP", tokens =>
            {
                    Commands.Add(new JMP { to = JumpPositions[tokens[1]] });
            } },
            { "JMP", tokens =>
            {
                    if (tokens[1] == "@A+DPTR")
                        Commands.Add(new JMP_A_DPTR());
                    else
                        Commands.Add(new JMP { to = JumpPositions[tokens[1]] });
            } },
            { "JC", tokens =>
            {
                    Commands.Add(new JB { to = JumpPositions[tokens[1]], address = Constants["C"] });
            } },
            { "JNC", tokens =>
            {
                    Commands.Add(new JNB { to = JumpPositions[tokens[1]], address = Constants["C"] });
            } },
            { "JB", tokens =>
            {
                    Commands.Add(new JB { to = JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JNB", tokens =>
            {
                    Commands.Add(new JNB { to = JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JBC", tokens =>
            {
                    Commands.Add(new JBC { to = JumpPositions[tokens[1]], address = ParseNumber(tokens[2]) });
            } },
            { "JZ", tokens =>
            {
                    Commands.Add(new JComp_NUM { to = ParseNumber(tokens[2]), compareTo = Constants["A"], num = 0 });
            } },
            { "JNZ", tokens =>
            {
                    Commands.Add(new JNComp_NUM { to = ParseNumber(tokens[2]), compareTo = Constants["A"], num = 1 });
            } },
            { "CJNE", tokens =>
            {
                    if (tokens[1].StartsWith('@'))
                        Commands.Add(new JNComp_AT { to = ParseNumber(tokens[3]), compareTo = ParseNumber(tokens[1]), num = ParseNumber(tokens[2]) });
                    else
                        Commands.Add(new JNComp { to = ParseNumber(tokens[3]), compareTo = ParseNumber(tokens[2]), toCompare = ParseNumber(tokens[1]) });
            } },
            { "DJNZ", tokens =>
            {
                    Commands.Add(new DJNZ { to = ParseNumber(tokens[2]), compareTo = ParseNumber(tokens[1]) });
            } },
            { "LCALL", tokens =>
            {
                    Commands.Add(new CALL { to = JumpPositions[tokens[1]] });
            } },
            { "ACALL", tokens =>
            {
                    Commands.Add(new CALL { to = JumpPositions[tokens[1]] });
            } },
            { "CALL", tokens =>
            {
                    Commands.Add(new CALL { to = JumpPositions[tokens[1]] });
            } },
            { "RET", tokens =>
            {
                    Commands.Add(new RET());
            } },


            { "PRINT", tokens =>
            {
                    Commands.Add(new PRINT { address = ParseNumber(tokens[1]) });
            } }
        };
    }
}