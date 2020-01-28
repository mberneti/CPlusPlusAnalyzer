
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using static CPP14Parser;

namespace AntlerCPlusPlus
{
    public class ReplaceExpression : CPP14BaseListener
    {
        private TokenStreamRewriter rewriter;
        public ReplaceExpression(CommonTokenStream tokens)
        {
            rewriter = new TokenStreamRewriter(tokens);
        }
        public override void ExitStatement([NotNull]CPP14Parser.StatementContext context)
        {
            LogDetails(context);

            var parentType = context.Parent.GetType();

            var types = new List<Type> {
                typeof(SelectionstatementContext),
                typeof(IterationstatementContext)
             };

            if (types.Contains(parentType))
            {
                AddLogToStatement(context);
            }
        }

        int functionStatementCounter = 1;
        int functionCounter = 1;
        public override void EnterFunctionbody([NotNull]CPP14Parser.FunctionbodyContext context)
        {
            var logText = context.Parent.GetText();
            functionStatementCounter = 1;
            AddLogAfter(context.Start, "MethodEntryLog");
            functionCounter++;
        }

        public override string ToString()
        {
            return rewriter.GetText();
        }

        #region Helpers
        void LogDetails(StatementContext context)
        {
            var child = context.children[0];
            Console.WriteLine(context.Depth() + $"{child.GetType() } " + context.GetText() + child.GetType());

        }
        void AddLogToStatement(StatementContext context)
        {
            var child = context.children[0];

            if (child.GetType() == typeof(CompoundstatementContext))
            {
                AddLogBefore(context.Stop);
            }
            else
            {
                AddBracesAndLog(context);
            }
        }
        void AddBracesAndLog(StatementContext context)
        {
            var column = context.Start.Column;
            var columnSpaces = GetTokenSpaces(column);

            rewriter.Replace(context.Start, context.Stop, $"{{\r\n{columnSpaces}\t" + context.GetText() + GetLog(column) + "}");


        }
        void AddLogAfter(IToken start, string text = null)
        {
            rewriter.InsertAfter(start, GetLog(start.Column, text));
        }
        void AddLogBefore(IToken stop, string text = null)
        {
            rewriter.InsertBefore(stop, GetLog(stop.Column));
        }

        string GetTokenSpaces(int size)
        {
            var totalSpaces = "";

            for (int i = 0; i < size; i++) totalSpaces += " ";

            return totalSpaces;
        }



        string GetLog(int column = 0, string text = null)
        {
            var columnSpaces = GetTokenSpaces(column);

            var logCode = $"\r\n{columnSpaces}\tcout << \"{functionCounter}.{functionStatementCounter}.{text}\" << endl;";

            if (string.IsNullOrWhiteSpace(text))
                logCode = $"\r\n{columnSpaces}\tcout << \"{functionCounter}.{functionStatementCounter}.log\" << endl;";

            ++functionStatementCounter;

            return GetCommentTemplate(columnSpaces, logCode);
        }

        string GetCommentTemplate(string columnSpaces, string logCode)
        {
            var emptyComment = $"\r\n{columnSpaces}\t// ---------------------";
            var header = $"\r\n{columnSpaces}\t// --- Documantation ---";

            var template = header + logCode + emptyComment + $"\r\n{columnSpaces}";
            return template;
        }
        #endregion
    }
}