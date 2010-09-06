/* Shellify Copyright (c) 2010 Sebastien LEBRETON

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. */

using System;
using System.IO;
using System.Reflection;
using Shellify.Tool.CommandLine;
using Shellify.Tool.Commands;
using Shellify.Tool.Options;
using System.Text;

namespace Shellify.Tool
{
    public class Program
    {
        public const string DemoValue = "value";
        public const string OptionTagDescriptionSeparator = " - ";

        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(string.Format("ShellifyTool v{0} by Sebastien LEBRETON", typeof(Program).Assembly.GetName().Version.ToString(2)));
                Console.WriteLine();
                Command command = CommandLineParser.Parse(args);
                if (command != null)
                {
                    command.Execute();
                }
                else
                {
                    DisplayUsage();
                }
            }
            catch (CommandLineParseException e)
            {
                DisplayUsage();
                Console.WriteLine();
                Console.WriteLine(string.Concat("ERROR: ", e.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Concat("ERROR: ", e.Message));
            }
        }

        private static int ComputeOptionWidth()
        {
            int result = 0;
            int value = 0;

            foreach (Option option in ProgramContext.Options)
            {
                value = option.Tag.Length + CommandLineParser.OptionPrefix.Length;
                if (option.ExpectedArguments > 0)
                {
                    value += CommandLineParser.OptionArgumentSeparator.Length + DemoValue.Length;
                }
                result = Math.Max(result, value);
            }

            return result;
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("Usage: ShellifyTool <Command> [Options...] filename [target]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            foreach (Command command in ProgramContext.Commands)
            {
                Console.WriteLine(string.Format("{0}: {1}", command.Tag, command.Description));
            }

            int optionWidth = ComputeOptionWidth();
            Console.WriteLine();
            Console.WriteLine("Options:");
            foreach (Option option in ProgramContext.Options)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("{0}{1}", CommandLineParser.OptionPrefix, option.Tag);
                if (option.ExpectedArguments > 0)
                {
                    builder.Append(CommandLineParser.OptionArgumentSeparator);
                    builder.Append("value");
                }
                while (builder.Length < optionWidth)
                {
                    builder.Append(" ");
                }
                builder.Append(OptionTagDescriptionSeparator);
                builder.Append(option.Description);
                Console.WriteLine(NormalizeBuilder(builder, optionWidth));
            }
        }

        private static string NormalizeBuilder(StringBuilder builder, int optionWidth)
        {
            string result = builder.ToString();
            char separator = ',';
            int currentWidth = 0;
            const int windowWidth = 80; // Console.WindowWidth no implemented in mono...

            if (builder.Length > windowWidth)
            {
                string[] splits = result.Split(separator);
                builder.Length = 0;
                foreach (String split in splits)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(separator);
                        currentWidth++;
                    }
                    if (currentWidth + split.Length >= windowWidth)
                    {
                        builder.AppendLine();
                        currentWidth = 0;
                        while (currentWidth < optionWidth + OptionTagDescriptionSeparator.Length)
                        {
                            builder.Append(" ");
                            currentWidth++;
                        }
                    }
                    builder.Append(split);
                    currentWidth += split.Length;
                }
                result = builder.ToString();
            }

            return result;
        }

    }
	
}
