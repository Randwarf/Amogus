using Amogus.Language.Content;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amogus.Language
{
    public class AmogusVisitor : AmogusBaseVisitor<object?>
    {
        public Dictionary<string, object?> Variables { get; } = new();

        public override object? VisitAssignment(AmogusParser.AssignmentContext context)
        {
            var varName = context.INDENTIFIER().GetText();
            var value = Visit(context.expression());

            Variables[varName] = value;

            return null;
        }
        
        public override object? VisitConstant(AmogusParser.ConstantContext context)
        {
            if (context.INTEGER() is { } intConstant)
            {
                return int.Parse(intConstant.GetText());
            }

            if (context.FLOAT() is { } floatConstant)
            {
                return float.Parse(floatConstant.GetText());
            }

            if (context.STRING() is { } stringConstant)
            {
                return stringConstant.GetText()[1..^1];
            }

            if (context.BOOL() is { } boolConstant)
            {
                return boolConstant.GetText() == "true";
            }

            if (context.NULL() is { })
            {
                return null;
            }

            throw new NotImplementedException();
        }
    }
}
