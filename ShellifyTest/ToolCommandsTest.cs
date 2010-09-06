﻿/* Shellify Copyright (c) 2010 Sebastien LEBRETON

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
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shellify.Tool.CommandLine;
using Shellify.Tool.Commands;
using Shellify.Tool.Options;

namespace Shellify.Test
{
    [TestClass]
    public class ToolCommandsTest
    {
        public ToolCommandsTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestToolCommands()
        {
            CreateRelativeCommand crc = new CreateRelativeCommand(string.Empty, string.Empty);
            CreateAbsoluteCommand cac = new CreateAbsoluteCommand(string.Empty, string.Empty);
            UpdateCommand uc = new UpdateCommand(string.Empty, string.Empty);
            DisplayInfosCommand dic = new DisplayInfosCommand(string.Empty, string.Empty);

            string tmpFile = Path.GetTempFileName();

            crc.Arguments.Add(tmpFile);
            crc.Arguments.Add("ShellifyTool.exe");
            crc.Execute();
            crc.Arguments[1] = @"c:\foo";
            try
            {
                crc.Execute();
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ArgumentException));
            }
            crc.Arguments[1] = @"foo";
            crc.Execute();

            cac.Arguments.Add(tmpFile);
            cac.Arguments.Add("ShellifyTool.exe");
            cac.Execute();
            cac.Arguments[1] = @"foo";
            cac.Execute();
            cac.Arguments[1] = ".";
            cac.Execute();

            Option on = Enumerable.ToList(ProgramContext.Options).Where(o => o.Tag == "name").First();
            on.Arguments.Add(string.Empty);

            uc.Arguments.Add(tmpFile);
            uc.Options.Add(on);
            uc.Execute();
        }
    }
}
