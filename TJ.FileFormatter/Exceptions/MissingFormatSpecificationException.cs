using System;

namespace TJ.FileFormatter.Exceptions
{
    public class MissingFormatSpecificationException : Exception
    {
        public MissingFormatSpecificationException(int position) : base("Missing format specification for position " + position)
        {
            
        }
    }
}