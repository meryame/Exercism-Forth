using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Exercism_Exo_Forth
{
    public static class Forth
    {
        private static Stack<int>? Stack;
        private static Dictionary<string, Func<int, int, int>>? Operators;
        private static Dictionary<string, string>? CustomInstructions;
        public static string Evaluate(string[] instructions)
        {
            Initialize();
            foreach (string instruction in instructions) EvaluateInstruction(instruction);
            return OutputStack();
        }
        private static void Initialize()
        {
            Stack = new();
            CustomInstructions = new();
            Operators = new()
            {
                { "*", (a, b) => a * b },
                { "+", (a, b) => a + b },
                { "/", (a, b) => a / b },
                { "-", (a, b) => a - b },
            };
        }
        private static void EvaluateInstruction(string instruction)
        {
            instruction = instruction.ToLower();
            if (instruction[0] == ':')
            {
                AddCustomInstruction(instruction);
                return;
            }
            foreach (string piece in instruction.Split(' '))
            {
                if (CustomInstructions.ContainsKey(piece))
                {
                    EvaluateInstruction(CustomInstructions[piece]);
                    continue;
                }
                if (int.TryParse(piece, out int value)) Stack.Push(value);
                if (Operators.ContainsKey(piece)) Compute(piece);
                if (Regex.IsMatch(piece, @"[^\d\W]+")) FollowInstruction(piece.ToLower());
            }
        }
        private static void AddCustomInstruction(string instruction)
        {
            Match instr = Regex.Match(instruction, @"^: (?<name>[^\s\d]+) (?<value>.+);$");
            if (!instr.Success) throw new InvalidOperationException();
            string name = instr.Groups["name"].Value.Trim();
            string value = instr.Groups["value"].Value.GetCustomValue();
            if (CustomInstructions.ContainsKey(name)) CustomInstructions[name] = value;
            else CustomInstructions.Add(name, value);
        }
        private static string GetCustomValue(this string value)
        {
            return value.Split(' ').Aggregate("", (a, b) => a + (CustomInstructions.ContainsKey(b) ? CustomInstructions[b] : b) + " ").Trim();
        }
        private static void Compute(string operation)
        {
            int b = Stack.Pop();
            int a = Stack.Pop();
            Stack.Push(Operators[operation](a, b));
        }
        private static void FollowInstruction(string instruction)
        {
            if (instruction != "dup" && instruction != "drop" && instruction != "swap" && instruction != "over") throw new InvalidOperationException();
            if (instruction == "dup") Duplicate();
            if (instruction == "drop") Drop();
            if (instruction == "swap") Swap();
            if (instruction == "over") Over();
        }
        private static void Duplicate() => Stack.Push(Stack.Peek());
        private static void Drop() => Stack.Pop();
        private static void Swap()
        {
            int first = Stack.Pop();
            int last = Stack.Pop();
            Stack.Push(first);
            Stack.Push(last);
        }
        private static void Over()
        {
            int lastValue = Stack.Pop();
            int valueToCopy = Stack.Peek();
            Stack.Push(lastValue);
            Stack.Push(valueToCopy);
        }
        private static string OutputStack() => Stack.Reverse().Aggregate("", (a, b) => a + b + " ").Trim();
    }
}

