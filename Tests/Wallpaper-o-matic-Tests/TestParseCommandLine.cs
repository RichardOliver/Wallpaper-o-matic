using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wallpaperomatic;

namespace Wallpaper_o_matic_Tests
{
    [TestClass]
    public class TestParseCommandLine
    {
        #region ParseCommandLine
        [TestMethod]
        public void ParseCommandLine_OneHypenStyleCommandLineOptionWithNoArguments_ReturnsMatchingKey()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe -Help";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            Assert.IsTrue(commandLineArgs.ContainsKey("Help"));
        }

        [TestMethod]
        public void ParseCommandLine_OneSlashStyleCommandLineOptionWithNoArguments_ReturnsMatchingKey()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /Help";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            Assert.IsTrue(commandLineArgs.ContainsKey("Help"));
        }

        [TestMethod]
        public void ParseCommandLine_OneSlashStyleCommandLineOptionWithEqualsStyleArgument_ReturnsMatchingKeyAndValue()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key=value";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actual = commandLineArgs["key"].ToList();
            var expected = new List<string> { "value" };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ParseCommandLine_OneHypenStyleCommandLineOptionWithColonStyleArgument_ReturnsMatchingKeyAndValue()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe -key:value";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actual = commandLineArgs["key"].ToList();
            var expected = new List<string> {"value"};
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ParseCommandLine_TwoCommandLineOptionWithSameKeys_ReturnsMatchingKeyAndValues()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key=value1 /key=value2";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actual = commandLineArgs["key"].ToList();
            var expected = new List<string> { "value1", "value2" };
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void ParseCommandLine_TwoCommandLineOptionWithDifferentKeys_ReturnsMatchingKeysAndValues()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key1=value1 /key2=value2";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actualFirstKey = commandLineArgs["key1"].ToList();
            var expectedFirstKey = new List<string> { "value1" };
            CollectionAssert.AreEquivalent(expectedFirstKey, actualFirstKey);

            var actualSecondKey = commandLineArgs["key2"].ToList();
            var expectedSecondKey = new List<string> { "value2" };
            CollectionAssert.AreEquivalent(expectedSecondKey, actualSecondKey);
        }
        #endregion

        #region GetFirstOrSubstitute
        [TestMethod]
        public void GetFirstOrSubstitute_KeyPresent_ReturnsArgument()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key=value";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actual = commandLineArgs.GetFirstOrSubstitute("Key", "FOO");
            Assert.AreEqual("value", actual);
        }

        [TestMethod]
        public void GetFirstOrSubstitute_KeyNotPresent_ReturnsSubstituteValue()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key=value";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            var actual = commandLineArgs.GetFirstOrSubstitute("NotAKey", "FOO");
            Assert.AreEqual("FOO", actual);
        }

        [TestMethod]
        public void GetFirstOrSubstitute_IlegalCastArgument_ReturnsSubstituteValue()
        {
            const string testCommandLine = @"c:\foo\ProgramName.exe /key=NotANumber";
            var commandLineArgs = new ParseCommandLine(testCommandLine);

            int actual = commandLineArgs.GetFirstOrSubstitute("key", 2);
            Assert.AreEqual(2, actual);
        }


        #endregion
    }
}