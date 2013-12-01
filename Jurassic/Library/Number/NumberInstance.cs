﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Library
{
    /// <summary>
    /// Represents an instance of the Number object.
    /// </summary>
    /// <remarks>
    /// None of the methods of the Number prototype are generic; they should throw <c>TypeError</c>
    /// if the <c>this</c> value is not a Number object or a number primitive.
    /// </remarks>
    public class NumberInstance : ObjectInstance
    {
        /// <summary>
        /// The primitive value.
        /// </summary>
        private double value;


        //     INITIALIZATION
        //_________________________________________________________________________________________

        /// <summary>
        /// Creates a new Number instance and initializes it to the given value.
        /// </summary>
        /// <param name="prototype"> The next object in the prototype chain. </param>
        /// <param name="value"> The value to initialize to. </param>
        public NumberInstance(ObjectInstance prototype, double value)
            : base(prototype)
        {
            this.value = value;
        }



        //     .NET ACCESSOR PROPERTIES
        //_________________________________________________________________________________________

        /// <summary>
        /// Gets the internal class name of the object.  Used by the default toString()
        /// implementation.
        /// </summary>
        protected override string InternalClassName
        {
            get { return "Number"; }
        }

        /// <summary>
        /// Gets the primitive value of the number.
        /// </summary>
        public double Value
        {
            get { return this.value; }
        }



        //     JAVASCRIPT FUNCTIONS
        //_________________________________________________________________________________________

        /// <summary>
        /// Returns a string representing a number represented in exponential notation.
        /// </summary>
        /// <param name="fractionDigits"> Number of digits after the decimal point. Must be in the
        /// range 0 – 20, inclusive.  Defaults to the number of digits necessary to specify the
        /// number. </param>
        /// <returns> A string representation of a number in exponential notation. The string
        /// contains one digit before the significand's decimal point, and may contain
        /// fractionDigits digits after it. </returns>
        [JSFunction(Name = "toExponential")]
        public string ToExponential(int fractionDigits = 20)
        {
            if (fractionDigits < 0 || fractionDigits > 20)
                throw new JavaScriptException("RangeError", "toExponential() argument must be between 0 and 20.");
            return this.value.ToString(string.Concat("0.", new string('#', fractionDigits), "e+0"),
                System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string representing a number in fixed-point notation.
        /// </summary>
        /// <param name="fractionDigits"> Number of digits after the decimal point. Must be in the
        /// range 0 – 20, inclusive. </param>
        /// <returns> A string representation of a number in fixed-point notation. The string
        /// contains one digit before the significand's decimal point, and must contain
        /// fractionDigits digits after it.
        /// If fractionDigits is not supplied or undefined, the toFixed method assumes the value
        /// is zero. </returns>
        [JSFunction(Name = "toFixed")]
        public string ToFixed(int fractionDigits = 0)
        {
            if (fractionDigits < 0 || fractionDigits > 20)
                throw new JavaScriptException("RangeError", "toFixed() argument must be between 0 and 20.");
            return this.value.ToString("f" + fractionDigits, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string containing a locale-dependant version of the number.
        /// </summary>
        /// <returns> A string containing a locale-dependant version of the number. </returns>
        [JSFunction(Name = "toLocaleString")]
        public new string ToLocaleString()
        {
            return this.value.ToString();
        }
        
        /// <summary>
        /// Returns a string containing a number represented either in exponential or fixed-point
        /// notation.
        /// </summary>
        /// <returns> A string containing a number represented either in exponential or fixed-point
        /// notation </returns>
        /// <remarks>
        /// If precision is not supplied or is undefined, the toString method is called instead.
        /// </remarks>
        [JSFunction(Name = "toPrecision")]
        public string ToPrecision()
        {
            return ToString();
        }

        /// <summary>
        /// Returns a string containing a number represented either in exponential or fixed-point
        /// notation with a specified number of digits.
        /// </summary>
        /// <param name="precision"> Number of significant digits. Must be in the range 1 – 21, inclusive. </param>
        /// <returns> A string containing a number represented either in exponential or fixed-point
        /// notation with a specified number of digits. </returns>
        /// <remarks>
        /// For numbers in exponential notation, precision - 1 digits are returned after the
        /// decimal point. For numbers in fixed notation, precision significant digits are
        /// returned.
        /// If precision is not supplied or is undefined, the toString method is called instead.
        /// </remarks>
        [JSFunction(Name = "toPrecision", Flags = FunctionBinderFlags.HasThisObject)]
        public string ToPrecision(int precision)
        {
            if (precision < 0 || precision > 21)
                throw new JavaScriptException("RangeError", "toPrecision() argument must be between 0 and 21.");
            return this.value.ToString("g" + precision, System.Globalization.CultureInfo.InvariantCulture).
                Replace("e+0", "e+").Replace("e-0", "e-");  // Hack: remove the extra zero in the exponent.
        }

        /// <summary>
        /// Returns the textual representation of the number.
        /// </summary>
        /// <param name="radix"> Specifies a radix for converting numeric values to strings. </param>
        /// <returns> The textual representation of the number. </returns>
        [JSFunction(Name = "toString")]
        public string ToStringJS(int radix = 10)
        {
            if (radix < 2 || radix > 36)
                throw new JavaScriptException("RangeError", "The radix must be between 2 and 36, inclusive.");

            // Check for common case: base 10.
            if (radix == 10)
                return this.value.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Unusual base.
            double value = this.value;
            var result = new System.Text.StringBuilder(10);

            // Handle NaN.
            if (double.IsNaN(value))
                return "NaN";

            // Handle negative numbers.
            if (value < 0)
            {
                value = -value;
                result.Append('-');
            }

            // Handle infinity.
            if (double.IsInfinity(value))
            {
                result.Append("Infinity");
                return result.ToString();
            }

            // Keep track of how many significant digits we have outputted.
            bool significantDigitsEncountered = false;
            int significantFigures = 0;

            // Calculate the number of digits in front of the decimal point.
            int numDigits = (int)Math.Max(Math.Log(value, radix), 0.0) + 1;

            // Output the digits in front of the decimal point.
            double radixPow = Math.Pow(radix, -numDigits);
            for (int i = numDigits; i > 0; i--)
            {
                radixPow *= radix;
                int digit = (int)(value * radixPow);
                if (digit < 10)
                    result.Append((char)('0' + digit));
                else
                    result.Append((char)('a' + digit - 10));
                if (digit != 0)
                    significantDigitsEncountered = true;
                if (significantDigitsEncountered == true)
                    significantFigures++;
                value -= digit / radixPow;
            }

            if (value != 0)
            {
                // Output the digits after the decimal point.
                result.Append('.');
                do
                {
                    radixPow *= radix;
                    int digit = (int)(value * radixPow);
                    if (digit < 10)
                        result.Append((char)('0' + digit));
                    else
                        result.Append((char)('a' + digit - 10));
                    if (digit != 0)
                        significantDigitsEncountered = true;
                    if (significantDigitsEncountered == true)
                        significantFigures++;
                    value -= digit / radixPow;
                } while (value > 0 && significantFigures < 19);
            }

            return result.ToString();

            // 0.3333333333333333
            // 0.3333333333333333
            // 0.333333333333333
        }

        /// <summary>
        /// Returns the primitive value of the specified object.
        /// </summary>
        /// <returns> The primitive value of the specified object. </returns>
        [JSFunction(Name = "valueOf", Flags = FunctionBinderFlags.HasThisObject)]
        public new double ValueOf()
        {
            return this.value;
        }
    }
}
