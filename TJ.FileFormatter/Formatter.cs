using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TJ.FileFormatter.Exceptions;

namespace TJ.FileFormatter
{
    public class Formatter<TInstance> : IFormatter<TInstance>
    {
        private SortedDictionary<int, IFormatStep<TInstance>> _formattingSteps;
        private string _delimiter = "";
        private bool _fixedLength = true;
        private string _header;

        public Formatter()
        {
            _formattingSteps = new SortedDictionary<int, IFormatStep<TInstance>>();
        }

        public IEnumerable<string> Format(IEnumerable<TInstance> objectsToFormat)
        {
            CheckFormattingSteps();
            var formattedValues = new List<string>();
            if (_header != null)
            {
                formattedValues.Add(_header);
            }
            foreach (var item in objectsToFormat)
            {
                formattedValues.Add(Format(item));
            }
            return formattedValues;
        }

        private void CheckFormattingSteps()
        {
            if (_formattingSteps.Count == 0)
            {
                throw new NoFormatSpecificationException();
            }
            if (_fixedLength)
            {
                FixedLengthCheck();
            }
            else
            {
                WithDelimiterCheck();
            }
        }

        private void FixedLengthCheck()
        {
            var previousItem = _formattingSteps.ElementAt(0);
            for (int i = 1; i < _formattingSteps.Count; i++)
            {
                var currentItem = _formattingSteps.ElementAt(i);
                var expectedStart = previousItem.Key + previousItem.Value.Length;
                if (expectedStart > currentItem.Key)
                {
                    throw new FormatOverLapException(expectedStart, currentItem.Key);
                }
                else if (expectedStart < currentItem.Key)
                {
                    throw new FormatGapException(expectedStart, currentItem.Key);
                }
                previousItem = currentItem;
            }
        }

        private void WithDelimiterCheck()
        {
            var previousItem = _formattingSteps.Keys.First();
            for (int i = 1; i < _formattingSteps.Count; i++)
            {
                var currentKey = _formattingSteps.Keys.ElementAt(i);
                if (currentKey - previousItem != 1)
                {
                    throw new MissingFormatSpecificationException(previousItem + 1);
                }
                previousItem = currentKey;
            }
        }

        private string Format(TInstance instance)
        {
            var isFixedLength = string.IsNullOrEmpty(_delimiter);
            var hasDelimiter = !string.IsNullOrEmpty(_delimiter);
            return string.Join(_delimiter, _formattingSteps.Select(y => y.Value.Execute(instance, isFixedLength, hasDelimiter, _delimiter)));
        }

        public IFormatter<TInstance> With<TMember>(Expression<Func<TInstance, TMember>> memberFunc, int position, int length, Func<TMember, string> formatFunc)
        {
            if (_formattingSteps.ContainsKey(position))
            {
                throw new DuplicateFormattingSpecificationException(position);
            }

            Func<TInstance, dynamic> dynamicMemberFunc = y => memberFunc.Compile()(y);
            Func<dynamic, string> dynamicFormatFunc = y => formatFunc(y);
            _formattingSteps.Add(position, new FormatStep<TInstance>(dynamicMemberFunc, length, dynamicFormatFunc, memberFunc));
            return this;
        }

        public IFormatter<TInstance> With(string text, int position)
        {
            if (_formattingSteps.ContainsKey(position))
            {
                throw new DuplicateFormattingSpecificationException(position);
            }
            var length = text.Length;
            _formattingSteps.Add(position, new StaticFormatStep<TInstance>(text, length));
            return this;
        }

        public IFormatter<TInstance> WithDelimiter(string delimiter)
        {
            _delimiter = delimiter;
            _fixedLength = false;
            return this;
        }

        public IFormatter<TInstance> WithLinePrefix(string linePrefix)
        {
            return With(linePrefix, 0);
        }

        public IFormatter<TInstance> WithHeader(string header)
        {
            _header = header;
            return this;
        }
    }
}
