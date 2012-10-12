using System;

namespace TJ.FileFormatter.Tests
{
    public class DummyObject
    {
        public int SomeInt { get; set; }
        public string SomeString { get; set; }
        public DateTime SomeDateTime { get; set; }
        public DummyObject CompositeObj { get; set; }
    }
}