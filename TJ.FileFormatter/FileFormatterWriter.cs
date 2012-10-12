using System.Collections.Generic;
using System.IO;

namespace TJ.FileFormatter
{
    public class FileFormatterWriter : IFileFormatterWriter
    {
        /// <summary>
        /// Writes the list of objects given a formatter to the given file path. The file
        /// is written using UTF-8 (.net-standard)
        /// </summary>
        /// <typeparam name="TInstance">Type of instances to format.</typeparam>
        /// <param name="objectsToFormat">List of items to format.</param>
        /// <param name="filePath">File path.</param>
        /// <param name="formatter">The formatter to use for formatting the items.</param>
        public void Format<TInstance>(IEnumerable<TInstance> objectsToFormat, string filePath, IFormatter<TInstance> formatter)
        {
            using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    foreach (var formattedValue in formatter.Format(objectsToFormat))
                    {
                        streamWriter.WriteLine(formattedValue);
                    }
                }
            }
        }
    }
}