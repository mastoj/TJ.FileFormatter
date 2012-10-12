using System;

namespace TJ.FileFormatter.Exceptions
{
    public class FormatGapException : Exception
    {
        public FormatGapException(int expectedStart, int actualStart)
            : base("Formatting gap, expected to start at " + expectedStart + ", but started at " + actualStart)
        {
            
        }
    }
}