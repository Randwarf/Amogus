using Amogus.Language.Content;
using Amogus.Language.Resources;
using Amogus.Language.Types;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Amogus.Language
{
    public class AmogusVisitor : AmogusBaseVisitor<object?>
    {
        public readonly List<object?> SystemOut = new List<object?>();

        public Stack<Dictionary<string, object?>> scope;

        public AmogusVisitor()
        {
            scope = new Stack<Dictionary<string, object?>>();
            Dictionary<string, object?> obj = new Dictionary<string, object?>();
            scope.Push(obj);

            SharedResources.Variables["PI"] = Math.PI;
            SharedResources.Variables["E"] = Math.E;
            SharedResources.Variables["Print"] = new Func<object?[], object?>(Print);
            SharedResources.Variables["Write"] = new Func<object?[], string, object?>(Write);
            SharedResources.Variables["Read"] = new Func<string, object?>(Read);
        }


        public override object? VisitProgram(AmogusParser.ProgramContext context)
        {
            base.VisitProgram(context);

            return SystemOut;
        }

        public override object? VisitFunctionCall(AmogusParser.FunctionCallContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var args = context.expression().Select(Visit).ToArray();

            //if global
            if(SharedResources.Variables.ContainsKey(name))
            {
                if(SharedResources.Variables[name] is functionObject funcObj)
                {
                    return callFunction(funcObj, args);
                }
                if(SharedResources.Variables[name] is Func<object?[], object?> func)
                {
                    return func(args);
                }
            }

            //if current scope
            var currentScope = scope.Peek();

            if(!currentScope.ContainsKey(name))
            {
                throw new Exception($"Function {name} is not defined");
            }

            if(currentScope[name] is functionObject funcO)
            {
                return callFunction(funcO, args);
            }

            return null;
        }

        public object? callFunction(functionObject funcObj, object?[]? args)
        {
            if(args is not null)
            {
                if (args.Length != funcObj.Names.Length)
                {
                    throw new ArgumentException("Incorrect number of arguments, expected "+ funcObj.Names.Length);
                }
            }
            else if (funcObj.Names.Length != 0)
            {
                throw new ArgumentException("Incorrect number of arguments, expected "+ funcObj.Names.Length);
            }

            scope.Push(new Dictionary<string, object?>());
            scope.Peek()[funcObj.name] = funcObj;
            if (args != null)
            {
                for(int i = 0; i < args.Length; i++)
                {
                    scope.Peek()[funcObj.Names[i]] = args[i];
                }
            }
            try
            {
                var result = Visit(funcObj.body);
            }
            catch(ReturnException e)
            {
                scope.Pop();   
                return e.returnVar;
            }
            scope.Pop();   
            return null;
        }

        public override object? VisitReturn(AmogusParser.ReturnContext context)
        {
            var value = Visit(context.expression());
            throw new ReturnException(value);
        }

        public override object? VisitAssignment(AmogusParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            var value = Visit(context.expression());

            scope.Peek()[varName] = value;

            return null;
        }

        public override object? VisitIdentifierExpression(AmogusParser.IdentifierExpressionContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            if(SharedResources.Variables.ContainsKey(varName))
            {
                return SharedResources.Variables[varName];
            }
            if(scope.Peek().ContainsKey(varName))
            {
                return scope.Peek()[varName];
            }

            throw new Exception($"Variable {varName} is not defined.");
        }

        public override object? VisitAdditiveExpression(AmogusParser.AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.addOp().GetText();

            return op switch
            {
                "+" => HandleSimpleExpression(left, right, (l, r) => l + r),
                "-" => HandleSimpleExpression(left, right, (l, r) => l - r),
                _ => throw new NotImplementedException()
            };
        }

        public override object? VisitParenthesizedExpression(AmogusParser.ParenthesizedExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override object? VisitMultiplicationExpression(AmogusParser.MultiplicationExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.multOp().GetText();

            return op switch
            {
                "*" => HandleSimpleExpression(left, right, (l, r) => l * r),
                "/" => HandleSimpleExpression(left, right, (l, r) => l / r),
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

        public override object? VisitFunctionBlock(AmogusParser.FunctionBlockContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var args = context.variables().GetText().Split(',');
            SharedResources.Variables[name] = new functionObject(args, context.block(), name);

            return null;
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

        public override object? VisitIfBlock(AmogusParser.IfBlockContext context)
        {
            Func<object?, bool> condition = context.IF().GetText() == "if" ? IsTrue : IsFalse;

            if(condition(Visit(context.expression())))
            {
                Visit(context.block());
            }
            else
            {
                if(context.elseIfBlock() != null)
                {
                    Visit(context.elseIfBlock());
                }
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
                "<" => HandleSimpleExpression(left, right, (l, r) => l < r),
                ">" => HandleSimpleExpression(left, right, (l, r) => l > r),
                "==" => HandleSimpleExpression(left, right, (l, r) => l == r),
                ">=" => HandleSimpleExpression(left, right, (l, r) => l >= r),
                "<=" => HandleSimpleExpression(left, right, (l, r) => l <= r),
                _ => throw new NotImplementedException()
            };
        }

        private static bool IsTrue(object? value)
        {
            if(value is bool b)
            {
                return b;
            }

            throw new Exception("Value is not a boolean");
        }

        private static bool IsFalse(object? value) => !IsTrue(value);

        private static object? HandleSimpleExpression(object? left, object? right, Func<dynamic?, dynamic?, dynamic?> func)
        {
            if (left is int l && right is int r)
            {
                return func(l, r);
            }

            if (left is float lf && right is float rf)
            {
                return func(lf, rf);
            }

            if (left is string ls && right is string rs)
            {
                return func((AmogusString)ls, (AmogusString)rs);
            }

            throw new Exception($"Cannot manipulate values of types {left?.GetType()} and {right?.GetType()}");
        }

        private object? Print(object?[] args)
        {
            foreach (var arg in args)
            {
                SystemOut.Add(arg);
                Console.Write(arg);
            }

            SystemOut.Add("\n");
            Console.Write("\n");

            return null;
        }

        private object? Read(string path)
        {
            return File.ReadAllText(path);
        }

        private object? Write(object?[] args, string path)
        {
            var s = string.Empty;

            foreach (var arg in args)
            {
                s += arg?.ToString();
            }

            File.WriteAllText(path, s);

            return null;
        }
    }

    public class functionObject
    {
        public string name;
        public string[] Names;
        public AmogusParser.BlockContext body;

        public functionObject(string[] names, AmogusParser.BlockContext body, string name){
            Names = new string[names.Length];
            names.CopyTo(Names, 0);
            if(Names.Length == 1 && String.IsNullOrWhiteSpace(Names[0]))
            {
                Names=new string[0];
            }

            this.body = body;

            this.name = name;
        }
    }

    public class ReturnException : Exception
    {
        public object? returnVar;
        public ReturnException(object? var){
            returnVar = var;
        }
    }
}