using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using static CPP14Parser;

namespace AntlerCPlusPlus
{
    public class BVisitor : CPP14BaseVisitor<StatementContext>
    {
        public List<StatementContext> Lines = new List<StatementContext>();
        private TokenStreamRewriter rewriter;

        //public override StatementContext VisitStatement([NotNull] CPP14Parser.StatementContext context)
        //{
        //    Lines.Add(context);
        //    return new StatementContext((ParserRuleContext)context.Parent, context.invokingState);
        //}

        // public override DeclarationstatementContext VisitDeclarationstatement([NotNull]CPP14Parser.DeclarationstatementContext context)
        // {
        //     return new DeclarationstatementContext((ParserRuleContext)context.Parent, context.invokingState);
        // }
    }
    // public class ClassVisitor : CPP14BaseVisitor<Class>
    // {
    //     public Class visitClassDeclaration(CPP14Parser.ClassDeclarationContext ctx)
    //     {
    //         String className = ctx.className().getText();
    //         MethodVisitor methodVisitor = new MethodVisitor();
    //         List<Method> methods = ctx.method()
    //                 .stream()
    //                 .map(method->method.accept(methodVisitor))
    //                 .collect(toList());
    //         return new Class(className, methods);
    //     }
    // }

    // public class MethodVisitor : CPP14BaseVisitor<Method>
    // {
    //     public Method visitMethod(CPP14Parser.decl ctx)
    //     {
    //         String methodName = ctx.methodName().getText();
    //         InstructionVisitor instructionVisitor = new InstructionVisitor();
    //         List<Instruction> instructions = ctx.instruction()
    //                 .stream()
    //                 .map(instruction->instruction.accept(instructionVisitor))
    //                 .collect(toList());
    //         return new Method(methodName, instructions);
    //     }
    // }

    // public class InstructionVisitor : CPP14BaseVisitor<IdexpressionContext>
    // {
    //     public IdexpressionContext visitInstruction(CPP14Parser.IdexpressionContext ctx)
    //     {
    //         String instructionName = ctx.getText();
    //         return new IdexpressionContext(instructionName);
    //     }
    // }
}
