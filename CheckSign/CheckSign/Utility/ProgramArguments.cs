//-----------------------------------------------------------------------
// <copyright file="ProgramArguments.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------- 

namespace Microsoft.Interflow.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents data passed on the command line.
    /// </summary>
    public class ProgramArguments
    {
        #region Private Fields
        /// <summary>
        /// Collection of parameters.
        /// </summary>
        private Dictionary<string, object> commandLineParameters;

        /// <summary>
        /// Translation table for the command synonyms.
        /// </summary>
        private Dictionary<string, CommandData> synonyms;

        /// <summary>
        /// Unexpected commands encountered.
        /// </summary>
        private List<string> errors = new List<string>();

        /// <summary>
        /// Name of the executing assembly.
        /// </summary>
        private string assemblyName;

        /// <summary>
        /// Version of executing assembly.
        /// </summary>
        private string version;

        private string commandLineUsage;


        #endregion

        public static List<CommandData> CommandRules { get; } = new List<CommandData>
        {
            new CommandData(
                nameAndSynonyms: "Help,h,?",
                isRequired: false,
                hasParameter: false,
                parameterDescription: "Get usage information",
                description: @"Display program usage information.",
                objectType: typeof(bool),
                defaultValue:false),
        };


        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ProgramArguments class.
        /// </summary>
        /// <param name="header">Header for the Usage page.</param>
        /// <param name="description">Description of the program.</param>
        /// <param name="commands">Command rules to parse.</param>
        /// <param name="commandLine">Arguments passed from the command line.</param>
        /// <param name="caseInsensitive">if set to <c>true</c> [case insensitive].</param>
        public ProgramArguments(
            string header,
            string description,
            List<CommandData> commands,
            string[] commandLine,
            bool caseInsensitive = true)
        {
            StringComparer comparer;

            if (caseInsensitive)
            {
                comparer = StringComparer.InvariantCultureIgnoreCase;
            }
            else
            {
                comparer = StringComparer.Ordinal;
            }

            this.synonyms = new Dictionary<string, CommandData>(comparer);
            this.commandLineParameters = new Dictionary<string, object>(comparer);

            Assembly assembly = Assembly.GetCallingAssembly();
            AssemblyName assemblyName = assembly.GetName();
            string version = string.Format(
                CultureInfo.CurrentCulture,
                "{0}.{1}.{2}.{3}",
                assemblyName.Version.Major,
                assemblyName.Version.MajorRevision,
                assemblyName.Version.Minor,
                assemblyName.Version.MinorRevision);

            this.assemblyName = assemblyName.Name;
            this.version = version;

            this.ProgramDescription = description;
            this.ProgramUsageHeader = header;
            foreach (CommandData command in commands)
            {
                this.synonyms.Add(command.Name, command);
                if (!string.IsNullOrEmpty(command.Synonyms))
                {
                    string[] splitSynonyms = command.Synonyms.Split(new char[] { ',' });
                    foreach (string synonym in splitSynonyms)
                    {
                        this.synonyms.Add(synonym, command);
                    }
                }
            }

            this.ParseCommandLine(commandLine);

            if ((bool)this["Help"] == true)
            {
                this.DisplayUsage();
                Environment.Exit(1);
            }

            this.VerifyRequiredParameters();
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value indicating whether errors were found in the command line.
        /// </summary>
        public bool HasErrors
        {
            get
            {
                return this.errors.Count > 0;
            }
        }

        /// <summary>
        /// Gets or sets the program description displayed on the Usage/Help page.
        /// </summary>
        public string ProgramDescription { get; set; }

        /// <summary>
        /// Gets or sets the header for use on the Usage/Help page.
        /// </summary>
        public string ProgramUsageHeader { get; set; }

        #endregion

        #region Public Indexers
        /// <summary>
        /// Gets the value of the parameter or an empty string.
        /// </summary>
        /// <param name="parameterName">Parameter name to return.</param>
        /// <returns>Command line parameter for the parameter.</returns>
        public object this[string parameterName]
        {
            get
            {
                if (this.commandLineParameters.ContainsKey(parameterName))
                {
                    return this.commandLineParameters[parameterName];
                }

                return this.synonyms[parameterName].DefaultValue;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Prints the full usage text.
        /// </summary>
        public void DisplayUsage()
        {
            int largestKey = 0;
            int largestSynonyms = 0;

            DisplayDescription(
                string.Format(
                CultureInfo.CurrentCulture,
                this.ProgramUsageHeader,
                this.assemblyName,
                this.version),
                false);

            this.commandLineUsage = this.CreateCommandLineUsage(ref largestKey, ref largestSynonyms);

            DisplayDescription(this.commandLineUsage, true, new char[] { '[', '/', ' ' });
            ConsoleHelper.Write(string.Empty);

            ConsoleHelper.Write("Description: ");
            ConsoleHelper.Indent(true);
            DisplayDescription(this.ProgramDescription, false);
            ConsoleHelper.Indent(false);

            ConsoleHelper.Write(string.Empty);
            this.DisplayParameterDescriptions(largestKey, largestSynonyms);

            ConsoleHelper.Write(string.Empty);

            ConsoleHelper.Indent(false);
        }

        private string CreateCommandLineUsage(ref int largestKey, ref int largestSynonyms)
        {
            string commandLineString = "";

            StringBuilder commandLine = new StringBuilder();
            commandLine.Append(string.Format(CultureInfo.CurrentCulture, "{0}.exe ", this.assemblyName));

            foreach (string key in this.synonyms.Keys)
            {
                if (this.synonyms[key].Name == key)
                {
                    largestKey = Math.Max(largestKey, key.Length);
                    largestSynonyms = Math.Max(largestSynonyms, this.synonyms[key].Synonyms.Length);

                    if (!this.synonyms[key].IsRequired)
                    {
                        commandLine.Append("[");
                    }

                    commandLine.Append("/");
                    commandLine.Append(key);
                    if (this.synonyms[key].HasParameter)
                    {
                        commandLine.Append(" ");

                        if (this.synonyms[key].ParameterDescription != null)
                        {
                            commandLine.Append(this.synonyms[key].ParameterDescription);
                        }
                    }

                    if (!this.synonyms[key].IsRequired)
                    {
                        commandLine.Append("]");
                    }

                    commandLine.Append(" ");
                }
            }

            commandLineString = commandLine.ToString();
            return commandLineString;
        }

        /// <summary>
        /// Displays the basic help with any Errors that were encountered.
        /// </summary>
        public void DisplayErrors()
        {
            if (this.errors.Count == 0)
            {
                return;
            }

            DisplayDescription(
                string.Format(
                CultureInfo.CurrentCulture,
                this.ProgramUsageHeader,
                this.assemblyName,
                this.version),
                false);

            ConsoleHelper.Indent(true);
            ConsoleHelper.Write($"{this.commandLineUsage}");
            ConsoleHelper.Write("");
            ConsoleHelper.Write($"Get usage information : \"{this.assemblyName}.exe /?\"");
            ConsoleHelper.Write("");
            ConsoleHelper.Indent(true);
            ConsoleHelper.Write("Errors:");
            ConsoleHelper.Indent(true);

            foreach (string error in this.errors)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleHelper.Write(error);
                Console.ResetColor();
            }

            ConsoleHelper.Indent(false);
            ConsoleHelper.Indent(false);
            ConsoleHelper.Write(string.Empty);
            ConsoleHelper.Indent(false);
        }

        /// <summary>
        /// Determines whether the program args contains the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return this.commandLineParameters.ContainsKey(key);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Displays simple formatted and word broken console output.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="indentAfterFirstLine">Force an indent on follow up lines.</param>
        private static void DisplayDescription(string text, bool indentAfterFirstLine)
        {
            DisplayDescription(text, indentAfterFirstLine, new char[] { ' ' });
        }

        /// <summary>
        /// Displays simple formatted and word broken console output.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="indentAfterFirstLine">Force an indent on follow up lines.</param>
        /// <param name="preferedBreakChar">Preferred line break character.</param>
        private static void DisplayDescription(string text, bool indentAfterFirstLine, char[] preferedBreakChar)
        {
            int displayWidth = ConsoleHelper.BufferWidth - ConsoleHelper.IndentCharacterCount;
            List<string> lines = ConsoleHelper.LineBreak(
                text,
                displayWidth,
                displayWidth - (indentAfterFirstLine ? ConsoleHelper.IndentSize : 0),
                preferedBreakChar);

            bool indented = false;

            // Cut the lines into lines that will fit without breaking words if necessary.
            foreach (string line in lines)
            {
                ConsoleHelper.Write(line);
                if (indentAfterFirstLine && !indented)
                {
                    ConsoleHelper.Indent(true);
                    indented = true;
                }
            }

            if (indented)
            {
                ConsoleHelper.Indent(false);
            }
        }

        /// <summary>
        /// Display the parameter help.
        /// </summary>
        /// <param name="text">The text to be displayed.</param>
        /// <param name="indentAfterFirstLine">The count of characters to add to the indent.</param>
        private static void DisplayParameter(string text, int indentAfterFirstLine)
        {
            int displayWidth = ConsoleHelper.BufferWidth - ConsoleHelper.IndentCharacterCount;
            List<string> lines = ConsoleHelper.LineBreak(
                text,
                displayWidth,
                displayWidth - indentAfterFirstLine);

            bool firstLine = true;

            // Cut the lines into lines that will fit without breaking words if necessary.
            foreach (string line in lines)
            {
                if (!firstLine)
                {
                    string paddedLine = line.PadLeft(indentAfterFirstLine + line.Length + 1);
                    ConsoleHelper.Write(paddedLine);
                }
                else
                {
                    firstLine = false;
                    ConsoleHelper.Write(line);
                }
            }
        }

        /// <summary>
        /// Displays the parameter descriptions.
        /// </summary>
        /// <param name="largestKey">Size of the largest key used to determine indent.</param>
        /// <param name="largestSynonyms">The largest synonyms length.</param>
        private void DisplayParameterDescriptions(int largestKey, int largestSynonyms)
        {
            ConsoleHelper.Write("Parameter List: (* == Required)");
            ConsoleHelper.Indent(true);
            List<string> sorted = new List<string>();

            foreach (string key in this.synonyms.Keys)
            {
                sorted.Add(key);
            }

            sorted.Sort();

            foreach (string key in sorted)
            {
                StringBuilder line = new StringBuilder();
                if (this.synonyms[key].Name == key)
                {
                    line.Append("/");
                    line.Append(key.PadRight(largestKey + 2));
                    line.Append(this.synonyms[key].IsRequired ? "*" : " ");

                    this.DisplaySynonyms(key, line, largestKey + largestSynonyms + 12);

                    int indentLength = line.Length;

                    this.DisplayDefaultValue(key, line);

                    line.Append(this.synonyms[key].Description);

                    DisplayParameter(line.ToString(), indentLength);
                }
            }
        }

        /// <summary>
        /// Displays the default value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="line">The line.</param>
        private void DisplayDefaultValue(string key, StringBuilder line)
        {
            if (this.synonyms[key].DefaultValue != null)
            {
                string defaultValue = this.synonyms[key].DefaultValue.ToString();
                 
                if (defaultValue != string.Empty)
                {
                    line.Append(string.Format("Default: \"{0}\" : ", this.synonyms[key].DefaultValue.ToString()));
                }
            }
        }

        /// <summary>
        /// Displays the Synonyms.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="line">The line.</param>
        /// <param name="padding">The padding.</param>
        private void DisplaySynonyms(string key, StringBuilder line, int padding)
        {
            if (this.synonyms[key].Synonyms.Length != 0)
            {
                line.Append(" (");

                foreach (string synonym in this.synonyms[key].Synonyms.Split(','))
                {
                    line.Append(" /");
                    line.Append(synonym);
                    line.Append(" ");
                }

                line.Append(") ");
            }

            line.Replace(line.ToString(), line.ToString().PadRight(padding));
        }

        /// <summary>
        /// Verify that all the required parameters are provided.
        /// </summary>
        private void VerifyRequiredParameters()
        {
            foreach (string key in this.synonyms.Keys)
            {
                if (this.synonyms[key].Name == key)
                {
                    if (this.synonyms[key].IsRequired && this[key] == null)
                    {
                        this.errors.Add(
                            string.Format(
                            CultureInfo.CurrentCulture,
                            "Missing required command line parameter \"{0}\".",
                            key));
                    }
                }
            }
        }

        /// <summary>
        /// Parse the input arguments.
        /// </summary>
        /// <param name="commandLine">Input arguments from the commandLine.</param>
        private void ParseCommandLine(string[] commandLine)
        {
            this.errors.Clear();

            for (int index = 0; index < commandLine.Length; index++)
            {
                string key = commandLine[index];
                if (key.StartsWith("/", StringComparison.Ordinal) ||
                    key.StartsWith("-", StringComparison.Ordinal) ||
                    key.StartsWith("\\", StringComparison.Ordinal))
                {
                    key = key.Substring(1); // remove the delimiter

                    if (this.synonyms.ContainsKey(key))
                    {
                        // get the parameters for the command if any.
                        if (!this.synonyms.ContainsKey(key))
                        {
                            throw new ArgumentException(
                                string.Format(
                                "Parameter \"{0}\" not found in rules", key));
                        }

                        CommandData command = this.synonyms[key];
                        if (this.commandLineParameters.ContainsKey(command.Name) && command.AllowMultiple == false)
                        {
                            this.errors.Add(
                                string.Format(
                                CultureInfo.CurrentCulture,
                                "Duplicate command used : \"{0}\"",
                                commandLine[index]));
                        }

                        if (command.HasParameter)
                        {
                            index++;
                            if (index < commandLine.Length)
                            {
                                object result = null;

                                if (!commandLine[index].TryParse(command.ObjectType, out result))
                                {
                                    this.errors.Add(
                                        string.Format(
                                        CultureInfo.CurrentCulture,
                                        "Parameter \"{0}\" with value \"{1}\" does not match expected type : \"{2}\"",
                                        commandLine[index - 1],
                                        commandLine[index],
                                        command.ObjectType.ToString()));
                                }

                                if (!(string.IsNullOrEmpty(command.RegexPattern) || command.RegexPattern.Trim().Length == 0))
                                {
                                    Regex regex = null;

                                    try
                                    {
                                        regex = new Regex(command.RegexPattern);
                                    }
                                    catch (ArgumentException e)
                                    {
                                        this.errors.Add("Invalid Regex Exception: " + e.Message);

                                        return;
                                    }

                                    if (!regex.IsMatch(commandLine[index]))
                                    {
                                        this.errors.Add(
                                            string.Format(
                                            CultureInfo.CurrentCulture,
                                            "Parameter \"{0}\" with value \"{1}\" does not match expected regex : \"{2}\"",
                                            commandLine[index - 1],
                                            commandLine[index],
                                            command.RegexPattern));
                                    }
                                }

                                if (this.errors.Count == 0)
                                {
                                    if (command.AllowMultiple)
                                    {
                                        if (this.commandLineParameters.ContainsKey(command.Name))
                                        {
                                            var l = (List<object>)this.commandLineParameters[command.Name];
                                            l.Add(result);
                                        }
                                        else
                                        {
                                            this.commandLineParameters[command.Name] = new List<object> { result };
                                        }
                                    }
                                    else
                                    {
                                        this.commandLineParameters[command.Name] = result;
                                    }
                                }
                            }
                            else
                            {
                                this.errors.Add(
                                    string.Format(
                                    CultureInfo.CurrentCulture,
                                    "Expected to find a parameter for commmand : \"{0}\"",
                                    commandLine[index - 1]));
                            }
                        }
                        else
                        {
                            this.commandLineParameters[command.Name] = true;
                        }
                    }
                    else
                    {
                        this.errors.Add(
                            string.Format(
                            CultureInfo.CurrentCulture,
                            "Command not recognized \"{0}\"",
                            commandLine[index]));
                    }
                }
                else
                {
                    this.errors.Add(
                        string.Format(
                        CultureInfo.CurrentCulture,
                        "Command not recognized \"{0}\"",
                        commandLine[index]));
                }
            }
        }
        #endregion
    }
}
