using System;

namespace BluePrintAssembler.Steam
{
    internal class Errors {
        public int count = 0;                                    // number of errors detected
        public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
        public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

        public virtual void SynErr (int line, int col, int n) {
            string s;
            switch (n) {
                case 0: s = "EOF expected"; break;
                case 1: s = "stringValue expected"; break;
                case 2: s = "\"{\" expected"; break;
                case 3: s = "\"}\" expected"; break;
                case 4: s = "??? expected"; break;
                case 5: s = "invalid Value"; break;

                default: s = "error " + n; break;
            }
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }

        public virtual void SemErr (int line, int col, string s) {
            errorStream.WriteLine(errMsgFormat, line, col, s);
            count++;
        }
	
        public virtual void SemErr (string s) {
            errorStream.WriteLine(s);
            count++;
        }
	
        public virtual void Warning (int line, int col, string s) {
            errorStream.WriteLine(errMsgFormat, line, col, s);
        }
	
        public virtual void Warning(string s) {
            errorStream.WriteLine(s);
        }
    }
}