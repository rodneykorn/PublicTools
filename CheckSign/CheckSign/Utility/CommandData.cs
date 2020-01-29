//-----------------------------------------------------------------------
// <copyright file="CommandData.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------- 

namespace Microsoft.Interflow.Utility
{
    using System;
    using System.Text;

    /// <summary>
    /// Data that holds information about each command.
    /// </summary>
    public class CommandData
    {
        #region Private Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CommandData class.
        /// </summary>
        /// <param name="nameAndSynonyms">Name of the command with synonyms as a comma delimited list.</param>
        /// <param name="isRequired">Parameter is required.</param>
        /// <param name="hasParameter">Command has a parameter.</param>
        /// <param name="parameterDescription">The parameter description.</param>
        /// <param name="description">Description used in Usage page.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <param name="allowMultiple">Command can appear multiple times.</param>
        public CommandData(
            string nameAndSynonyms, 
            bool isRequired, 
            bool hasParameter, 
            string parameterDescription, 
            string description, 
            Type objectType = null, 
            object defaultValue = null,
            bool allowMultiple = false)
        {
            this.Initialize(
                nameAndSynonyms, 
                isRequired,
                hasParameter,
                parameterDescription, 
                description, 
                objectType, 
                defaultValue,
                allowMultiple);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandData" /> class.
        /// </summary>
        /// <param name="nameAndSynonyms">The name and synonyms.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="hasParameter">if set to <c>true</c> [has parameter].</param>
        /// <param name="parameterDescription">The parameter description.</param>
        /// <param name="description">The description.</param>
        /// <param name="regex">The regex.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="allowMultiple">Command can appear multiple times.</param>
        public CommandData(
            string nameAndSynonyms, 
            bool isRequired,
            bool hasParameter,
            string parameterDescription, 
            string description, 
            string regex, 
            object defaultValue = null,
            bool allowMultiple = false)
        {
            this.Initialize(
                nameAndSynonyms, 
                isRequired,
                hasParameter, 
                parameterDescription, 
                description,
                typeof(string), 
                defaultValue,
                allowMultiple);

            this.RegexPattern = regex;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the name of the Command.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the command has a parameter.
        /// This will cause the parser to read the next value as the parameter.
        /// </summary>
        public bool HasParameter { get; private set; }

        /// <summary>
        /// Gets the description text used to display the parameter description in the usage page.
        /// </summary>
        public string ParameterDescription { get; private set; }

        /// <summary>
        /// Gets the description text used to produce usage page.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the command is required.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Gets the alternate form(s) that may be used.
        /// </summary>
        public string Synonyms { get; private set; }

        /// <summary>
        /// Gets the alternate form(s) that may be used.
        /// </summary>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the default Value.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets a value indicating whether multiples are allowed.
        /// </summary>
        public bool AllowMultiple { get; private set; }

        /// <summary>
        /// Gets the default Value.
        /// </summary>
        public string RegexPattern { get; private set; }

        #endregion

        #region Private Methods
        /// <summary>
        /// Initializes the specified name and synonyms.
        /// </summary>
        /// <param name="nameAndSynonyms">The name and synonyms.</param>
        /// <param name="isRequired">if set to <c>true</c> [is required].</param>
        /// <param name="hasParameter">if set to <c>true</c> [has parameter].</param>
        /// <param name="parameterDescription">The parameter description.</param>
        /// <param name="description">The description.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="allowMultiple">Command can appear multiple times.</param>
        private void Initialize(
            string nameAndSynonyms, 
            bool isRequired, 
            bool hasParameter, 
            string parameterDescription, 
            string description, 
            Type objectType, 
            object defaultValue,
            bool allowMultiple)
        {
            // Trim the input
            StringBuilder removedSpaces = new StringBuilder();

            foreach (string str in
                nameAndSynonyms.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                removedSpaces.Append(str.Trim());
                removedSpaces.Append(",");
            }

            // Remove the last ,
            if (removedSpaces.Length != 0)
            {
                removedSpaces.Remove(removedSpaces.Length - 1, 1);
            }

            nameAndSynonyms = removedSpaces.ToString();

            // Remove the full word from the synonyms
            int synonymsStart = nameAndSynonyms.IndexOf(",", StringComparison.Ordinal);
            if (synonymsStart > 0)
            {
                this.Synonyms = nameAndSynonyms.Substring(synonymsStart + 1);
            }
            else
            {
                this.Synonyms = string.Empty;
                synonymsStart = nameAndSynonyms.Length;
            }

            if (objectType == null)
            {
                this.ObjectType = typeof(string);
            }
            else
            {
                this.ObjectType = objectType;
            }

            this.Name = nameAndSynonyms.Substring(0, synonymsStart);
            this.HasParameter = hasParameter;
            this.ParameterDescription = parameterDescription;
            this.Description = description;
            this.IsRequired = isRequired;
            this.DefaultValue = defaultValue;
            this.AllowMultiple = allowMultiple;
        }
        #endregion
    }
}
