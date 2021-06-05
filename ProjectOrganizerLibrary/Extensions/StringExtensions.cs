using System;
using System.ComponentModel;

namespace ProjectOrganizerLibrary.Extensions
{
	public static class StringExtensions
	{
		/// <summary>
		/// Converts the string to the appropriate value of the enumeration
		/// </summary>
		/// <typeparam name="T">Enumeration to find value of</typeparam>
		/// <param name="value">Name of emuneration value to convert to</param>
		/// <returns></returns>
		public static T ToEnum<T>(this string value)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), value, true);
			} catch
			{
				throw new InvalidEnumArgumentException($"{typeof(T)} does not have a value of {value}");
			}
		}
	}
}
