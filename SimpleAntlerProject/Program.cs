using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AntlerCPlusPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "(10+2)*2"; // 10+2*2
            AntlrInputStream inputStream = new AntlrInputStream(input);

            BGrammerLexer spreadsheetLexer = new BGrammerLexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(spreadsheetLexer);
            BGrammerParser bParser = new BGrammerParser(commonTokenStream);
            BGrammerParser.ExprContext expressionContext = bParser.expr();
            BVisitor visitor = new BVisitor();

            Console.WriteLine(visitor.Visit(expressionContext));
        }
    }
}
