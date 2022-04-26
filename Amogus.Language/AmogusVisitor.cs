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

        public AmogusVisitor()
        {
            Variables["PI"] = Math.PI;
            Variables["E"] = Math.E;

            Variables["Print"] = new Func<object?[], object?>(Print);
        }

        private object? Print(object?[] args)
        {
            foreach(var arg in args)
            {
                Console.WriteLine(arg);
            }

            return null;
        }

        public override object? VisitFunctionCall(AmogusParser.FunctionCallContext context)
        {
            var name = context.INDENTIFIER().GetText();
            var args = context.expression().Select(Visit).ToArray();

            if(!Variables.ContainsKey(name))
            {
                throw new Exception($"Function {name} is not defined");
            }

            if(Variables[name] is not Func<object?[], object?> func)
            {
                throw new Exception($"Variable {name} is not a function");
            }

            return func(args);
        }

        public override object? VisitAssignment(AmogusParser.AssignmentContext context)
        {
            var varName = context.INDENTIFIER().GetText();
            var value = Visit(context.expression());

            Variables[varName] = value;

            return null;
        }

        public override object? VisitIndentifierExpression(AmogusParser.IndentifierExpressionContext context)
        {
            var varName = context.INDENTIFIER().GetText();

            if(!Variables.ContainsKey(varName))
            {
                throw new Exception($"Variable {varName} is not defined.");
            }

            return Variables[varName];
        }

        public override object? VisitAdditiveExpression(AmogusParser.AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.addOp().GetText();

            return op switch
            {
                "+" => Add(left, right),
                //"-" => Subtract(left, right),
                _ => throw new NotImplementedException()
            };
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

        public override object? VisitWhileBlock(AmogusParser.WhileBlockContext context)
        {
            Func<object?, bool> condition = context.WHILE().GetText() == "while" ? IsTrue : IsFalse;

            if(condition(Visit(context.expression())))
            {
                do
                {
                    Visit(context.block());
                } while (condition(Visit(context.expression())));
            }
            else
            {
                Visit(context.elseIfBlock());
            }

            return null;
        }

        public override object? VisitComparisonExpression(AmogusParser.ComparisonExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.compareOp().GetText();

            return op switch
            {
                "<" => LessThan(left, right),
                _ => throw new NotImplementedException()
            };
        }

        private bool LessThan(object? left, object? right)
        {
            if(left is int l && right is int r)
            {
                return l < r;
            }

            if(left is float fl && right is float fr)
            {
                return fl < fr;
            }

            throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
        }

        private bool IsTrue(object? value)
        {
            if(value is bool b)
            {
                return b;
            }

            throw new Exception("Value is not a boolean");
        }

        private bool IsFalse(object? value) => !IsTrue(value);

        private object? Add(object? left, object? right)
        {
            if(left is int l && right is int r)
            {
                return l + r;
            }

            if (left is float lf && right is float rf)
            {
                return lf + rf;
            }

            throw new Exception($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
        }
    }
}
