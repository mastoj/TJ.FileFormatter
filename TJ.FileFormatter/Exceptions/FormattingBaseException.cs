using System;
using System.Linq.Expressions;

namespace TJ.FileFormatter.Exceptions
{
    public abstract class FormattingBaseException : Exception
    {
        public FormattingBaseException(string message) : base(message)
        {
            
        }

        protected static string GetMemberName(Expression expr)
        {
            var lambdaExpression = expr as LambdaExpression;
            if (lambdaExpression == null)
            {
                return "";
            }
            string lambdaText = "";
            MemberExpression mexpr = null;
            if (lambdaExpression.Body is MemberExpression)
            {
                mexpr = (MemberExpression)lambdaExpression.Body;
            }
            else if (lambdaExpression.Body is UnaryExpression)
            {
                mexpr = (MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand;
            }
            if (mexpr == null)
            {
                return null;
            }
            return mexpr.ToString();
        }
    }
}