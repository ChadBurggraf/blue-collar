//-----------------------------------------------------------------------
// <copyright file="Tail.cs" company="Tasty Codes">
//     Copyright (c) 2012 Chad Burggraf.
// </copyright>
//-----------------------------------------------------------------------

namespace BlueCollar.Examples
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides file tail-reading services.
    /// </summary>
    public static class Tail
    {
        private static readonly Regex NewlineExpression = new Regex(@"[\n\r]+$", RegexOptions.Compiled);

        /// <summary>
        /// Reads the given file, tailing the given number of lines, using UTF-8 encoding.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="count">The maximum number of lines to tail.</param>
        /// <returns>The tail lines that were read.</returns>
        public static IEnumerable<string> Read(string path, int count)
        {
            return Read(path, count, Encoding.UTF8);
        }

        /// <summary>
        /// Reads the given file, tailing the given number of lines.
        /// </summary>
        /// <param name="path">The path of the file to read.</param>
        /// <param name="count">The maximum number of lines to tail.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The tail lines that were read.</returns>
        public static IEnumerable<string> Read(string path, int count, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path", "path must contain a value.");
            }

            using (FileStream stream = File.OpenRead(path))
            {
                return Read(stream, count, encoding);
            }
        }

        /// <summary>
        /// Reads the given stream, tailing the given number of lines, sing UTF-8 encoding.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="count">The maximum number of lines to tail.</param>
        /// <returns>The tail lines that were read.</returns>
        public static IEnumerable<string> Read(Stream stream, int count)
        {
            return Read(stream, count, Encoding.UTF8);
        }
        
        /// <summary>
        /// Reads the given stream, tailing the given number of lines.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="count">The maximum number of lines to tail.</param>
        /// <param name="encoding">The encoding to use.</param>
        /// <returns>The tail lines that were read.</returns>
        public static IEnumerable<string> Read(Stream stream, int count, Encoding encoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream", "stream cannot be null.");
            }

            if (!stream.CanRead)
            {
                throw new ArgumentException("stream must be readable.", "stream");
            }

            if (!stream.CanSeek)
            {
                throw new ArgumentException("stream must be seek-able.", "stream");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "count must be greater than or equal to zero.");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding", "encoding cannot be null.");
            }

            List<string> result = new List<string>();

            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                reader.BaseStream.Seek(0, SeekOrigin.End);

                string currentLine = string.Empty, previousLine = string.Empty;
                StringBuilder sb = new StringBuilder();
                char[] buffer = new char[1024];
                bool finished = false;

                while (!finished)
                {
                    long position = reader.BaseStream.Position - buffer.Length;
                    int max = buffer.Length, charCount, lineCount;

                    if (position < 0)
                    {
                        max = (int)(max + position);
                        position = 0;
                    }

                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    finished = position == 0;
                    charCount = reader.Read(buffer, 0, max);
                    lineCount = result.Count;
                    previousLine = ReadBuffer(result, sb, buffer, charCount);

                    if (result.Count > lineCount)
                    {
                        result[lineCount] += currentLine;
                        currentLine = string.Empty;
                    }

                    if (count == 0 || result.Count < count)
                    {
                        currentLine = currentLine + previousLine;

                        if (!finished)
                        {
                            reader.BaseStream.Seek(-encoding.GetByteCount(sb.ToString()), SeekOrigin.Current);

                            if (sb.Length > 0)
                            {
                                sb.Remove(0, sb.Length);
                            }
                        }
                    }
                    else
                    {
                        finished = true;
                    }
                }

                if (currentLine.Length > 0)
                {
                    result.Add(currentLine);
                }

                if (count > 0)
                {
                    while (result.Count > count)
                    {
                        result.RemoveAt(result.Count - 1);
                    }
                }
            }

            return result
                .Select(s => NewlineExpression.Replace(s, string.Empty))
                .ToArray();
        }

        /// <summary>
        /// Reads a buffer of characters in reverse, appending each line to the given output list and
        /// writing the un-split output to the given output string.
        /// </summary>
        /// <param name="output">The output list to append lines to.</param>
        /// <param name="outputValue">The output string builder to write raw output to.</param>
        /// <param name="buffer">The buffer to read from.</param>
        /// <param name="count">The number of characters to read.</param>
        /// <returns>The string representing the remainder of the buffer after all lines have been read.</returns>
        private static string ReadBuffer(IList<string> output, StringBuilder outputValue, char[] buffer, int count)
        {
            int remaining = count;
            
            for (int i = count - 1; i >= 0; --i)
            {
                if (buffer[i] == '\n')
                {
                    string str = new string(buffer, i + 1, remaining - i - 1);
                    output.Add(str);
                    outputValue.Append(str);
                    remaining -= str.Length;
                }
            }

            if (remaining > 0)
            {
                string str = new string(buffer, 0, remaining);
                outputValue.Append(str);
                return str;
            }

            return string.Empty;
        }
    }
}
