using System;
using System.Linq.Expressions;
using TJ.FileFormatter.Exceptions;

namespace TJ.FileFormatter
{
    internal class FormatStep<TInstance> : IFormatStep<TInstance>
    {
        private readonly Func<TInstance, dynamic> _dynamicMemberFunc;
        private readonly int _length;
        private readonly Func<dynamic, string> _dynamicFormatFunc;
        private readonly Expression _memberExpression;

        public int Length { get { return _length; } }

        internal FormatStep(Func<TInstance, dynamic> dynamicMemberFunc, int length, Func<dynamic, string> dynamicFormatFunc, Expression memberExpression)
        {
            _dynamicMemberFunc = dynamicMemberFunc;
            _length = length;
            _dynamicFormatFunc = dynamicFormatFunc;
            _memberExpression = memberExpression;
        }

        public string Execute(TInstance instanceToFormat, bool fixedLength = false, bool hasDelimiter = false, string delimiter = null)
        {
            string formattedValue = _dynamicFormatFunc(_dynamicMemberFunc(instanceToFormat));
            var valueLength = formattedValue.Length;
            if ((fixedLength && valueLength != _length) || formattedValue.Length > _length)
            {
                throw new FormattingException(_memberExpression, formattedValue, _length, fixedLength);
            }
            else if (hasDelimiter && formattedValue.Contains(delimiter))
            {
                throw new InvalidFormatException(_memberExpression, formattedValue, delimiter);
            }
            return formattedValue;
        }
    }

    internal interface IFormatStep<TInstance>
    {
        string Execute(TInstance instanceToFormat, bool fixedLength = false, bool hasDelimiter = false, string delimiter = null);
        int Length { get; }
    }

    internal class StaticFormatStep<TInstance> : IFormatStep<TInstance>
    {
        private string _staticText;
        private readonly int _length;

        public StaticFormatStep(string staticText, int length)
        {
            _staticText = staticText;
            _length = length;
        }

        public string Execute(TInstance instanceToFormat, bool fixedLength = false, bool hasDelimiter = false, string delimiter = null)
        {
            return _staticText;
        }

        public int Length
        {
            get { return _length; }
        }
    }
}