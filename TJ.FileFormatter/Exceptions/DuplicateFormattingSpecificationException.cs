using System;

namespace TJ.FileFormatter.Exceptions
{
    public class DuplicateFormattingSpecificationException : Exception
    {
        public DuplicateFormattingSpecificationException(int position)
            : base("A format specification is alread defined for position " + position)
        {
            
        }        
    }
}