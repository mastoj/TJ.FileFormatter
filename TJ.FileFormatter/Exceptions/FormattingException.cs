using System.Linq.Expressions;

namespace TJ.FileFormatter.Exceptions
{
    public class FormattingException : FormattingBaseException
    {
        public FormattingException(Expression memberExpression, string formattedValue, int length, bool fixedLength) 
            : base(GetExceptionMessage(memberExpression, formattedValue, length, fixedLength))
        {
        }

        private static string GetExceptionMessage(Expression memberExpression, string formattedValue, int length, bool fixedLength)
        {
            var lambdaText = GetMemberName(memberExpression);
            string exceptionMessage = string.Format("Expression: {0} does not meet requirement, ", lambdaText);
            exceptionMessage += fixedLength
                                   ? string.Format("the value {0} does not meet the required length of the field which is {1}", formattedValue, length)
                                   : string.Format("the value {0} is longer than the max length {1} of the field.", formattedValue, length);
            return exceptionMessage;
        }
    }
}