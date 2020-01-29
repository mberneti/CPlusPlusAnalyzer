using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using static CPP14Parser;
using System.Text.RegularExpressions;

namespace AntlerCPlusPlus
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = @"
                            #include <iostream>
            
                            using namespace std;
                                        
                            void main()
                            {
                                cout << ""Hello World!"";

                                int x = 2;

                                if(x == 2)
                                {
                                    int y = 2;
                                    x = 20 * y;
                                }

                                if(x<1)
                                {
                                    cout <<""bad"";
                                }else {
                                    cout <<""good"";
                                }

                                for (int i = 0; i < 10; i++)
                                {
                                    cout << i + ""-"";
                                    cout << 2i + ""XX"";
                                }
                            }
                            ";

            Console.WriteLine("Input:");
            Console.WriteLine(input);

            AntlrInputStream inputStream = new AntlrInputStream(input);

            CPP14Lexer lexer = new CPP14Lexer(inputStream);
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            CPP14Parser parser = new CPP14Parser(commonTokenStream);

            parser.BuildParseTree = true;
            IParseTree tree = parser.translationunit(); //first row of grammer in *.g4 file >> first rule
            ReplaceExpression replaceExpression = new ReplaceExpression(commonTokenStream);
            ParseTreeWalker.Default.Walk(replaceExpression, tree);

            // BVisitor visitor = new BVisitor();
            // Console.WriteLine(visitor.Visit(tree));

            Console.WriteLine("////////////////////////////////////////////////////////////////////////");
            Console.WriteLine("Output:");

            var resultString = Regex.Replace(replaceExpression.ToString(), @"^\s+$[\r\n]*", string.Empty,
                RegexOptions.Multiline);

            resultString = Regex.Replace(resultString, @"^\s+", string.Empty,RegexOptions.Multiline);

            Console.WriteLine(resultString);

        }
    }
}
