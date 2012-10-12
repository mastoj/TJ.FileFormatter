using System;
using System.Linq.Expressions;

namespace TJ.FileFormatter.Exceptions
{
    public class InvalidFormatException : FormattingBaseException
    {
        public InvalidFormatException(Expression memberExpression, string value, string delimiter) : base(GetMessage(memberExpression, value, delimiter))
        {
            
        }

        private static string GetMessage(Expression memberExpression, string value, string delimiter)
        {
            var lambdaExpresion = GetMemberName(memberExpression);
            var message = string.Format("The expression {0} has invalid format. ", lambdaExpresion);
            message += string.Format("The value {0} contains the delimiter {1}.", value, delimiter);
            return message;
        }
    }
}