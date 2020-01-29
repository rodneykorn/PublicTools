//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------------- 

using System;
using System.Reflection;

namespace Microsoft.Interflow.Utility
{
    /// <summary>
    /// Extension Methods for the string class that are helpful.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Extension Method for TryParse string into any type that supports the TryParseMethod.
        /// </summary>
        /// <typeparam name="T">The type to parse.</typeparam>
        /// <param name="str">The current string.</param>
        /// <param name="result">The result.</param>
        /// <returns>True or false if parse succeeded.</returns>
        public static bool TryParse<T>(this string str, out T result)
        {
            result = default(T);

            bool success = TryParse(str, result?.GetType(), out object tempResult);

            if (success)
            {
                result = (T)tempResult;
            }

            return success;
        }

        /// <summary>
        /// Extension Method for TryParse string into any type that supports the TryParseMethod.
        /// </summary>
        /// <param name="str">The current string.</param>
        /// <param name="type">The type.</param>
        /// <param name="result">The result.</param>
        /// <returns>True or false if parse succeeded.</returns>
        /// <exception cref="System.ArgumentException">TryParse method does not exist for given type.</exception>
        public static bool TryParse(this string str, Type type, out object result)
        {
            result = null;

            if (type == null)
            {
                throw new ArgumentException("type");
            }

            if (type == typeof(string))
            {
                result = str;

                return true;
            }

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;

            Type[] parameterTypes = new Type[] { typeof(string), type.MakeByRefType() };

            MethodInfo method = type.GetMethod("TryParse", bindingFlags, null, parameterTypes, null);

            if (method == null)
            {
                throw new ArgumentException(type.ToString() + " does not contain a TryParse Method");
            }

            object[] parameters = new object[] { str, null };

            bool success = (bool)method.Invoke(null, parameters);

            if (success)
            {
                result = parameters[1];
            }

            return success;
        }

        /// <summary>
        /// Replaces the specified value from the current string using a comparison type.
        /// </summary>
        /// <param name="str">The current string.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>Returns the new search string after items have been altered.</returns>
        public static string Replace(this string str, string oldValue, string newValue, StringComparison comparisonType)
        {
            int index = str.IndexOf(oldValue, comparisonType);

            // perform the replace on the matched word
            if (index != -1)
            {
                str = str.Remove(index, oldValue.Length);
                str = str.Insert(index, newValue);
            }

            return str;
        }

        /// <summary>
        /// Determines whether the string contains the specified string with a comparison type.
        /// </summary>
        /// <param name="str">The string to be validated.</param>
        /// <param name="value">The value to be compared against.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified string]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string str, string value, StringComparison comparisonType)
        {
            return str.IndexOf(value, comparisonType) >= 0;
        }
    }
}
