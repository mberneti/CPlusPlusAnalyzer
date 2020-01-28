
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
        int labelCounter=1;
        private CommonTokenStream commonTokenStream;
        private TokenStreamRewriter rewriter;
        public ReplaceExpression(CommonTokenStream tokens)
        {
            commonTokenStream = tokens;
            rewriter = new TokenStreamRewriter(tokens);
        }
        public override void EnterSelectionstatement([NotNull] CPP14Parser.SelectionstatementContext context) {

            var isAnIfCondition = context.GetChild(0).GetText().Contains("if");

            if (isAnIfCondition)
            {
                // add a not operator to the condition ---------------------------------------
                var condition = context.GetChild(2);
                var conditionStartToken = commonTokenStream.Get(condition.SourceInterval.a);
                var conditionEndToken = commonTokenStream.Get(condition.SourceInterval.b);

                rewriter.Replace(conditionStartToken, conditionEndToken, $"!({condition.GetText()})");
                // ---------------------------------------------------------------------------

                var conditionLabel = GetNewLabel();

                // add goto statement to the if condition ------------------------------------
                var rightParen = context.GetChild(3);
                var rightParenToken = commonTokenStream.Get(rightParen.SourceInterval.a);
                rewriter.InsertAfter(rightParenToken, $" goto {conditionLabel}");
                // ---------------------------------------------------------------------------

                // add label to the end of condition -----------------------------------------
                var stopSpaces = GetTokenSpaces(context.Start.Column);
                rewriter.InsertAfter(context.Stop, $"\r\n{stopSpaces + conditionLabel}:\r\n");
                // ---------------------------------------------------------------------------

                var childrenCount = context.children.Count;
                var statement = context.children[childrenCount - 1];
                if (statement.GetText().EndsWith("}"))
                {
                    var leftBraceToken = commonTokenStream.Get(statement.SourceInterval.a);
                    var rightBraceToken = commonTokenStream.Get(statement.SourceInterval.b);
                    rewriter.Delete(leftBraceToken);
                    rewriter.Delete(rightBraceToken);
                }

            }

            //Console.WriteLine(context.GetText());
        }
        public override void ExitStatement([NotNull]CPP14Parser.StatementContext context)
        {
        }

        public override string ToString()
        {
            return rewriter.GetText();
        }

        #region Helpers

        string GetNewLabel()
        {
            return $"L{labelCounter++}";
        }

        string GetTokenSpaces(int size)
        {
            var totalSpaces = "";

            for (int i = 0; i < size; i++) totalSpaces += " ";

            return totalSpaces;
        }
        #endregion
    }
}