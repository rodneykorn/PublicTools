//-----------------------------------------------------------------------
// <copyright file="ConsoleHelper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------- 

namespace Microsoft.Interflow.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// This class just contains some helper methods for output to the console.
    /// </summary>
    public static class ConsoleHelper
    {
        #region Public Constants
        /// <summary>
        /// The number of characters to use for indentation.
        /// </summary>
        public const int IndentSize = 4;
        #endregion

        #region Public Properties
        /// <summary>
        /// The level of the indent.  Initialized by the runtime to 0.
        /// </summary>
        private static int indentLevel;

        /// <summary>
        /// Buffer width is not legal in a build window so supply it if necessary.
        /// </summary>
        private static int bufferWidth = -1;

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the number of characters of the current indent.
        /// </summary>
        public static int IndentCharacterCount
        {
            get
            {
                return indentLevel * 4;
            }
        }

        /// <summary>
        /// Gets the buffer width of the console.  Under the build environment this will produce an
        /// exception so set a default to 80 in that case.
        /// </summary>
        public static int BufferWidth
        {
            get
            {
                if (bufferWidth == -1)
                {
                    try
                    {
                        bufferWidth = Console.BufferWidth;
                    }
                    catch (Exception)
                    {
                        bufferWidth = 80;
                    }
                }

                return bufferWidth;    
            }
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Writes a string to the console as a line.
        /// </summary>
        /// <param name="text">Text to write to the console.</param>
        public static void Write(string text)
        {
            string output = text.PadLeft(text.Length + (indentLevel * 4));

            Console.WriteLine(output);
        }

        /// <summary>
        /// Writes lines of text to the console.
        /// </summary>
        /// <param name="lines">Text to write to the console.</param>
        /// <param name="indentAfterFirst">Increase the indent level after first line.</param>
        public static void Write(List<string> lines, bool indentAfterFirst)
        {
            bool indented = false;
            foreach (string line in lines)
            {
                Write(line);
                if (indentAfterFirst && !indented)
                {
                    Indent(true);
                    indented = true;
                }
            }

            if (indented)
            {
                Indent(false);
            }
        }

        /// <summary>
        /// Increase or decrease indentLevel.
        /// </summary>
        /// <param name="increase">Indicates whether the indent level is being increased (TRUE) or decreased.</param>
        public static void Indent(bool increase)
        {
            indentLevel += increase ? 1 : -1;
        }

        /// <summary>
        /// Breaks text into pieces that will fit on the display.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <param name="indentAfterFirstLine">Indent the lines after the first.</param>
        /// <returns>Array of sized and broken strings.</returns>
        public static List<string> LineBreak(string text, bool indentAfterFirstLine)
        {
            int consoleWidthRemaining = ConsoleHelper.BufferWidth - IndentCharacterCount;
            int consoleWidthLine2 =
                indentAfterFirstLine ? (consoleWidthRemaining - IndentSize) : consoleWidthRemaining;
            return LineBreak(text, consoleWidthRemaining, consoleWidthLine2);
        }

        /// <summary>
        /// Breaks text into pieces that will fit on the display.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <param name="firstLineSize">Max size of first line of text.</param>
        /// <param name="remainingSize">Max size of following lines of text.</param>
        /// <returns>Array of sized and broken strings.</returns>
        public static List<string> LineBreak(string text, int firstLineSize, int remainingSize)
        {
            return LineBreak(text, firstLineSize, remainingSize, new char[] { ' ' });
        }

        /// <summary>
        /// Breaks text into pieces that will fit on the display.
        /// </summary>
        /// <param name="text">Input text.</param>
        /// <param name="firstLineSize">Max size of first line of text.</param>
        /// <param name="remainingSize">Max size of following lines of text.</param>
        /// <param name="preferredBreakValue">Preferred character to break.</param>
        /// <returns>Array of sized and broken strings.</returns>
        public static List<string> LineBreak(
            string text, 
            int firstLineSize, 
            int remainingSize, 
            char[] preferredBreakValue)
        {
            string programDescriptionExpanded = 
                text.Replace("\\r", string.Empty).Replace("\\n", Environment.NewLine);
            string[] lines = 
                programDescriptionExpanded.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            bool firstLine = true;

            List<string> result = new List<string>();

            foreach (string line in lines)
            {
                string displayLine = line;
                int width = firstLine ? firstLineSize : remainingSize;
                while (displayLine.Length >= width)
                {
                    string subLine = displayLine.Substring(0, width - 1);
                    for (int tryChar = 0; tryChar < preferredBreakValue.Length; tryChar++)
                    {
                        int indexOfPreviousWordBreak = subLine.LastIndexOf(preferredBreakValue[tryChar]);
                        if (indexOfPreviousWordBreak > 0)
                        {
                            subLine = subLine.Substring(0, indexOfPreviousWordBreak).Trim();
                            break;
                        }
                    }

                    displayLine = displayLine.Substring(subLine.Length).Trim();
                    result.Add(subLine);
                    firstLine = false;
                    width = remainingSize;
                }

                result.Add(displayLine);
            }

            return result;
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string output = message.PadLeft(message.Length + (indentLevel * 4));
            Console.Error.WriteLine(output);
            Console.ResetColor();

        }

        /// <summary>Writes a warning in yellow to the console.</summary>
        /// <param name="message">The message to be written.</param>
        public static void WriteWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            string output = message.PadLeft(message.Length + (indentLevel * 4));
            Console.Error.WriteLine(output);
            Console.ResetColor();

        }
        #endregion
    }
}
