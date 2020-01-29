
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var lineSpaces = GetTokenSpaces(context.Start.Column);// for better code formatting

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

                // check else statement ------------------------------------------------------
                var elseLabelText = "";
                var elseStatementText = "";
                var elseContext = context.children.FirstOrDefault(x => x.GetText() == "else");

                if (elseContext !=null)
                {
                    var elseToken = commonTokenStream.Get(elseContext.SourceInterval.a);

                    var elseLabel = GetNewLabel();

                    // replace else with goto keyword -------------------------------
                    var elseGotoText = $"goto {elseLabel}";
                    rewriter.Replace(elseToken, elseGotoText);
                    // --------------------------------------------------------------

                    // get else statement -------------------------------------------
                    var elseIndex = context.children.IndexOf(elseContext);
                    var elseStatementContext = context.children[elseIndex + 1];

                    // removing braces from statement ----------------------------------------------------------
                    elseStatementText = Regex.Replace(elseStatementContext.GetText(), @"^{+|}+$", string.Empty);
                    // -----------------------------------------------------------------------------------------

                    elseStatementText = $"\r\n{lineSpaces + elseStatementText}\r\n";
                    // --------------------------------------------------------------

                    elseLabelText = $"\r\n{lineSpaces+elseLabel}:\r\n";
                }
                // ---------------------------------------------------------------------------

                // add label to the end of condition -----------------------------------------
                var conditionLabelText = $"\r\n{lineSpaces + conditionLabel}:\r\n";
                rewriter.InsertAfter(context.Stop, conditionLabelText + elseStatementText + elseLabelText);
                // ---------------------------------------------------------------------------

                foreach (var statement in context.children)
                {
                    var sText = statement.GetText();
                    if (sText.EndsWith("}"))
                    {
                        var leftBraceToken = commonTokenStream.Get(statement.SourceInterval.a);
                        var rightBraceToken = commonTokenStream.Get(statement.SourceInterval.b);
                        rewriter.Delete(leftBraceToken);
                        rewriter.Delete(rightBraceToken);
                        //rewriter.Replace(leftBraceToken, $"\r\n");
                        //rewriter.Replace(rightBraceToken, $"\r\n");
                    }

                }

            }

            //Console.WriteLine(context.GetText());
        }
        public override void ExitStatement([NotNull]StatementContext context)
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