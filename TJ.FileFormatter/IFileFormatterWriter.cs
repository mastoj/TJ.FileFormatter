using System.Collections.Generic;

namespace TJ.FileFormatter
{
    public interface IFileFormatterWriter
    {
        void Format<T>(IEnumerable<T> objectsToFormat, string filePath, IFormatter<T> formatter);
    }
}
