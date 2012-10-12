using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TJ.FileFormatter
{
    public interface IFormatter<TInstance>
    {
        /// <summary>
        /// Takes a list of items and returns a list of formatted strings in UTF-8 (standard .NET-formatting).
        /// </summary>
        /// <param name="objectsToFormat">Items to format</param>
        /// <returns>List of formatted strings</returns>
        IEnumerable<string> Format(IEnumerable<TInstance> objectsToFormat);
        /// <summary>
        /// Defines a formatting step. This has two "modes" depending on the format
        /// is using delimiter or not. If a delimiter is specified with the "WithDelimiter"
        /// method the position parameter defines position based on delimiter and length
        /// defines the max length of the string. If no delimiter is specifed (default)
        /// the position is the character position in the string and length is the 
        /// required length.
        /// </summary>
        /// <example>
        ///     int position = 2;
        ///     int length = 5;
        ///     formatter.With(y => y.MemberToFormat, position, length, memberToFormat => memberToFormat.ToString("00000"));
        /// </example>
        /// <typeparam name="TMember">Type of member/expression to format.</typeparam>
        /// <param name="memberFunc">Selector function for member/expression to format.</param>
        /// <param name="position">Fixed position if no delimiter, delimiter based if delimiter is specified.</param>
        /// <param name="length">Max length if delimiter is used, required length if no delimiter.</param>
        /// <param name="formatFunc">The format to apply to the result from the memberFunc</param>
        /// <returns>The instance with the format added (enables chaining)</returns>
        IFormatter<TInstance> With<TMember>(Expression<Func<TInstance, TMember>> memberFunc, int position, int length, Func<TMember, string> formatFunc);
        /// <summary>
        /// Defines a formatting step for a string that is the same for each line. The length
        /// is calculated automatically from the length of the text string.
        /// </summary>
        /// <param name="text">The text to put in the position.</param>
        /// <param name="position">Fixed position if no delimiter, delimiter based if delimiter is specified.</param>
        /// <returns>The instance with the format added (enables chaining)</returns>
        IFormatter<TInstance> With(string text, int position);
        /// <summary>
        /// If delimiter is defined the formatter switches over to delimiter mode, 
        /// that is, no fixed length and position is based on delimiter instead
        /// of character position. The delimiter specified is used as delimiter.
        /// </summary>
        /// <param name="delimiter">The delimiter to use.</param>
        /// <returns>The instance with the format added (enables chaining)</returns>
        IFormatter<TInstance> WithDelimiter(string delimiter);
        /// <summary>
        /// Short cut for "formatter.With("someString", 0);"
        /// </summary>
        /// <lt see cref="IFormatter{T}.With(string, int)"> for more information
        /// <param name="linePrefix"></param>
        /// <returns>The instance with the format added (enables chaining)</returns>
        IFormatter<TInstance> WithLinePrefix(string linePrefix);
        /// <summary>
        /// Header row for, if specified the first element when running "Format"
        /// will be this row.
        /// </summary>
        /// <param name="header">The header to use.</param>
        /// <returns>The instance with the format added (enables chaining)</returns>
        IFormatter<TInstance> WithHeader(string header);
    }
}