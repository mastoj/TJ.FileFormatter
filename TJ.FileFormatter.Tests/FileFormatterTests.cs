using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TJ.FileFormatter.Tests
{
    [TestClass]
    public class When_Writing_Formatted_Value_To_File
    {
        private List<DummyObject> _input;
        private string _outputPath;
        private IFormatter<DummyObject> _formatter;
        private int _numberOfObjects;
        private DateTime _baseDate;

        [TestInitialize]
        public void Setup()
        {
            // Given/arrange
            var fileFormatterWriter = new FileFormatterWriter();
            _outputPath = "./formattingText.txt";
            _numberOfObjects = 5;
            _baseDate = DateTime.Now;
            _input = new List<DummyObject>();
            for (int i = 0; i < _numberOfObjects; i++)
            {
                _input.Add(new DummyObject()
                               {SomeDateTime = _baseDate.AddDays(i), SomeInt = i, SomeString = i.ToString()});
            }
            _formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 5, y => y.ToString().PadLeft(5,'0'));

            // When/act
            fileFormatterWriter.Format(_input, _outputPath, _formatter);
        }
        
        [TestMethod]
        public void Then_The_File_Should_Exist()
        {
            Assert.IsTrue(File.Exists(_outputPath));
        }

        [TestMethod]
        public void Then_It_Should_Have_The_Expected_Number_Of_Lines()
        {
            List<string> actualLines = ReadFile();
            Assert.AreEqual(_numberOfObjects, actualLines.Count, "Expected {0} rows in the file but it had {1}", _numberOfObjects, actualLines.Count);            
        }

        [TestMethod]
        public void Then_It_Should_Have_The_Rows_Given_From_The_Formatter()
        {
            var expectedLines = _formatter.Format(_input);
            List<string> actualLines = ReadFile();
            for (int i = 0; i < _numberOfObjects; i++)
            {
                Assert.AreEqual(expectedLines.ElementAt(i), actualLines.ElementAt(i));
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (File.Exists(_outputPath))
            {
                File.Delete(_outputPath);
            }
        }

        private List<string> _actualLines; 
        private List<string> ReadFile()
        {
            if (_actualLines == null)
            {
                _actualLines = new List<string>();
                using (var fileStream = File.OpenRead(_outputPath))
                {
                    using (var streamReader = new StreamReader(fileStream))
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            _actualLines.Add(line);
                        }
                    }
                }
            }
            return _actualLines;
        }
    }
}
