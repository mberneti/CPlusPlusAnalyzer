using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;

namespace AntlerCPlusPlus
{
    public class BVisitor : BGrammerBaseVisitor<int>
    {
        public override int VisitInt(BGrammerParser.IntContext context)
        {
            return int.Parse(context.INT().GetText());
        }

        public override int VisitAddSub(BGrammerParser.AddSubContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == BGrammerParser.ADD)
            {
                return left + right;
            }
            else
            {
                return left - right;
            }
        }

        public override int VisitMulDiv(BGrammerParser.MulDivContext context)
        {
            int left = Visit(context.expr(0));
            int right = Visit(context.expr(1));
            if (context.op.Type == BGrammerParser.MUL)
            {
                return left * right;
            }
            else
            {
                return left / right;
            }
        }

        public override int VisitParens(BGrammerParser.ParensContext context)
        {
            return Visit(context.expr());
        }
    }
}
