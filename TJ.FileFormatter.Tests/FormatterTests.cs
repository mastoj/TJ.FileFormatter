using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TJ.FileFormatter.Exceptions;

namespace TJ.FileFormatter.Tests
{
    [TestClass]
    public class When_No_Delimiter_Is_Specified
    {
        [TestMethod]
        public void Then_Fixed_Length_Should_Be_Applied_To_The_Fields()
        {
            var dummyObjects = new List<DummyObject>();
            for (int i = 0; i < 10; i++)
            {
                dummyObjects.Add(new DummyObject() { SomeInt = i, SomeString = "Str" + i});
            }
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 5, y => y.ToString().PadLeft(5, '0'))
                .With(y => y.SomeString, 5, 5, y => y.PadRight(5, ' '));
            var result = formatter.Format(dummyObjects);
            for (int i = 0; i < dummyObjects.Count; i++)
            {
                string expected = i.ToString().PadLeft(5, '0') + ("Str" + i).PadRight(5, ' ');
                string actual = result.ElementAt(i);
                Assert.AreEqual(expected, actual);
            }
        }
    }

    [TestClass]
    public class When_Formatting_A_List_Of_10_DummyObjects_Without_Formatting_Specified
    {
        private List<DummyObject> _objectsToFormat;
        private int _numberOfObjects = 10;
        private Formatter<DummyObject> _formatter;
        
        [TestMethod]
        public void It_Should_Not_Be_null()
        {
            _objectsToFormat = new List<DummyObject>();
            for (int i = 0; i < _numberOfObjects; i++)
            {
                _objectsToFormat.Add(new DummyObject());
            }
            _formatter = new Formatter<DummyObject>();
            try
            {
                _formatter.Format(_objectsToFormat);
                Assert.Fail("Expected NoFormatSpecificationException to be thrown");
            }
            catch (NoFormatSpecificationException)
            {
                // Expect NoFormatSpecificationException to be thrown.
            }
        }
    }

    [TestClass]
    public class When_Formatting_A_List_Of_10_DummyObjects_With_Formatting_Specified_And_Delimiter
    {
        private List<DummyObject> _objectsToFormat;
        private int _numberOfObjects;
        private IFormatter<DummyObject> _formatter;
        private IEnumerable<string> _result;
        private DateTime _baseDate;
        private string _dateFormat = "yyyyMMdd";

        [TestInitialize]
        public void Setup()
        {
            _baseDate = DateTime.Now;
            _numberOfObjects = 10;
            _objectsToFormat = new List<DummyObject>();
            for (int i = 0; i < _numberOfObjects; i++)
            {
                _objectsToFormat.Add(new DummyObject()
                                         {
                                             SomeInt = i,
                                             SomeString = i.ToString(),
                                             SomeDateTime = _baseDate.AddDays(i),
                                             CompositeObj = new DummyObject() {SomeInt = 10 + i}
                                         });
            }
            _formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 5, y => y.ToString("00000"))
                .With(y => y.SomeString, 2, 6, y => y.ToString().PadLeft(6, '_'))
                .With(y => y.SomeDateTime, 1, 8, y => y.ToString(_dateFormat))
                .WithDelimiter(";");
            _result = _formatter.Format(_objectsToFormat);
        }

        [TestMethod]
        public void It_Should_Apply_The_Formatting()
        {
            string expectedResult = "";
            for (int i = 0; i < _numberOfObjects; i++)
            {
                var date = _baseDate.AddDays(i);
                expectedResult = "0000" + i + ";" + date.ToString(_dateFormat) + ";" + "_____" + i ;
                Assert.AreEqual(expectedResult, _result.ElementAt(i));
            }
        }
    }
    
    [TestClass]
    public class When_Formatting_A_List_Of_10_DummyObjects_With_Formatting_Specified_But_Formatting_Does_Not_Match_Specified_Length
    {
        private List<DummyObject> _objectsToFormat;
        private int _numberOfObjects;
        private IFormatter<DummyObject> _formatter;
        private DateTime _baseDate;

        [TestInitialize]
        public void Setup()
        {
            _baseDate = DateTime.Now;
            _numberOfObjects = 10;
            _objectsToFormat = new List<DummyObject>();
            for (int i = 0; i < _numberOfObjects; i++)
            {
                _objectsToFormat.Add(new DummyObject()
                {
                    SomeInt = i,
                    SomeString = i.ToString(),
                    SomeDateTime = _baseDate.AddDays(i)
                });
            }
            string formatLongerThan5 = "000000";
            int maxLength = 5;
            _formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, maxLength, y => y.ToString(formatLongerThan5));
        }

        [TestMethod]
        public void It_Should_Throw_Formatting_Exception()
        {
            try
            {
                _formatter.Format(_objectsToFormat);
                Assert.Fail("Expected FormattingException to be thrown");
            }
            catch (FormattingException exception)
            {
                // no throw since expected exception was thrown.
            }
        }
    }

    [TestClass]
    public class When_Missing_A_Specification_For_Position_When_Delimiter_Is_Used
    {
        [TestMethod]
        public void It_Should_Throw_Missing_Format_Specification_Exception()
        {
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 4, y => y.ToString())
                .With(y => y.SomeString, 2, 3, y => y.ToString())
                .WithDelimiter(";");
            try
            {
                IEnumerable<DummyObject> items = new List<DummyObject>();
                formatter.Format(items);
                Assert.Fail("Should throw MissingFormatSpecificationException");
            }
            catch (MissingFormatSpecificationException)
            {
            }
        }
    }

    [TestClass]
    public class When_Positions_Overlap_And_Not_Using_Delimiter
    {
        [TestMethod]
        public void It_Should_Throw_Format_Overlap_Exception()
        {
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 4, y => y.ToString())
                .With(y => y.SomeString, 2, 3, y => y.ToString());
            try
            {
                IEnumerable<DummyObject> items = new List<DummyObject>();
                formatter.Format(items);
                Assert.Fail("Should throw MissingFormatSpecificationException");
            }
            catch (FormatOverLapException)
            {
            }
        }
    }

    [TestClass]
    public class When_There_Is_A_Position_Gap_And_Not_Using_Delimiter
    {
        [TestMethod]
        public void It_Should_Throw_Format_Gap_Exception()
        {
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 4, y => y.ToString())
                .With(y => y.SomeString, 5, 3, y => y.ToString());
            try
            {
                IEnumerable<DummyObject> items = new List<DummyObject>();
                formatter.Format(items);
                Assert.Fail("Should throw MissingFormatSpecificationException");
            }
            catch (FormatGapException)
            {
            }
        }
    }

    [TestClass]
    public class When_Adding_Two_Formatting_Specifications_For_The_Same_Position
    {
        [TestMethod]
        public void It_Should_Throw_Duplicate_Formatting_Specification_Exception()
        {
            try
            {
                var formatter = new Formatter<DummyObject>()
                    .With(y => y.SomeInt, 0, 0, y => y.ToString())
                    .With(y => y.SomeInt, 0, 0, y => y.ToString());
                Assert.Fail("Expectend DuplicateFormattingSpecificationException to be thrown");
            }
            catch (DuplicateFormattingSpecificationException)
            {
            }
        }
    }

    [TestClass]
    public abstract class FormatterTestsBase
    {
        protected List<DummyObject> _objectsToFormat = new List<DummyObject>();
        protected virtual void ChildSetup()
        {
        }

        [TestInitialize]
        public void Setup()
        {
            for (int i = 0; i < 10; i++)
            {
                _objectsToFormat.Add(new DummyObject()
                {
                    SomeInt = i,
                    SomeString = i.ToString(),
                    CompositeObj = new DummyObject() { SomeInt = 10 + i }
                });
            }
            ChildSetup();
        }
    }

    [TestClass]
    public class When_A_LinePrefix_Is_Used : FormatterTestsBase
    {
        [TestMethod]
        public void Then_All_Rows_Should_Have_The_Line_Prefix_At_Start_Of_Row()
        {
            var prefix = "ThisIsAPrefix";
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, prefix.Length, 5, y => y.ToString("00000"))
                .With(y => y.SomeString, prefix.Length + 5, 6, y => y.ToString().PadLeft(6, '_'))
                .WithLinePrefix(prefix);
            var result = formatter.Format(_objectsToFormat);
            foreach (var item in result)
            {
                Assert.IsTrue(item.StartsWith(prefix));
            }
        }

        [TestMethod]
        public void And_Prefix_Is_Used_It_Should_Be_Applied_At_Start_Of_Row()
        {
            var prefix = "ThisIsAPrefix";
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 1, 5, y => y.ToString("00000"))
                .With(y => y.SomeString, 2, 6, y => y.ToString().PadLeft(6, '_'))
                .WithLinePrefix(prefix)
                .WithDelimiter(";");
            var result = formatter.Format(_objectsToFormat);
            foreach (var item in result)
            {
                Assert.IsTrue(item.StartsWith(prefix + ";"));
            }
        }
    }

    [TestClass]
    public class When_Using_A_Header : FormatterTestsBase
    {
        [TestMethod]
        public void Then_The_Header_Should_Be_The_First_Row()
        {
            string header = "This is the header";
            var formatter = new Formatter<DummyObject>()
                .With(y => y.SomeInt, 0, 5, y => y.ToString("00000"))
                .With(y => y.SomeString, 5, 6, y => y.ToString().PadLeft(6, '_'))
                .WithHeader(header);
            var result = formatter.Format(_objectsToFormat);
            Assert.AreEqual(header, result.First());
        }
    }

    [TestClass]
    public class When_Formatted_Value_Contains_Delimiter 
    {
        private IFormatter<DummyObject> _formatter;
        private List<DummyObject> _objects;
        [TestInitialize]
        public void Setup()
        {
            _formatter = new Formatter<DummyObject>()
                .WithDelimiter(";")
                .With(y => y.SomeString, 0, 100, y => y);
            _objects = new List<DummyObject> {new DummyObject() {SomeString = "ThisContains;Delimiter"}};
        }

        [TestMethod]
        public void Then_An_Invalid_Format_Exception_Should_Be_Thrown()
        {
            try
            {
                _formatter.Format(_objects);
                Assert.Fail("When resulting value contains delimiter an InvalidFormatException should be thrown.");
            }
            catch (InvalidFormatException)
            {
            }
        }
    }

    [TestClass]
    public class When_A_Static_Format_For_Field_Is_Used : FormatterTestsBase
    {
        [TestMethod]
        public void Then_The_Static_Field_Should_Be_Printed()
        {
            var formatter = new Formatter<DummyObject>()
                .With("Hello", 0)
                .With(y => y.SomeInt, 1, 5, y => y.ToString("00000"))
                .With("World!", 2)
                .WithDelimiter(";");
            var result = formatter.Format(_objectsToFormat);
            for (int i = 0; i < _objectsToFormat.Count; i++)
            {
                var expected = "Hello;" + _objectsToFormat[i].SomeInt.ToString("00000") + ";World!";
                Assert.AreEqual(expected, result.ElementAt(i));
            }
        }
    }
}
