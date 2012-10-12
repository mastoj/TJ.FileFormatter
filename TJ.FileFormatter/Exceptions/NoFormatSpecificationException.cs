using System;

namespace TJ.FileFormatter.Exceptions
{
    public class NoFormatSpecificationException : Exception
    {
        public NoFormatSpecificationException()
            : base("No format specification is defined, use the With method to add format specifications.")
        {
            
        }
    }
}