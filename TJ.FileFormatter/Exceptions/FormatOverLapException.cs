using System;

namespace TJ.FileFormatter.Exceptions
{
    public class FormatOverLapException : Exception
    {
        public FormatOverLapException(int expectedStart, int actualStart)
            : base("Formatting overlap, expected to start at " + expectedStart + ", but started at " + actualStart)
        {
            
        }
    }
}