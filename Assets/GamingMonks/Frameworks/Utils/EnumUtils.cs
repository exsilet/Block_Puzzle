using System;
using System.Collections.Generic;
using System.Linq;

namespace GamingMonks.Utils
{
	/// <summary>
    /// Extention class Enum utils.
    /// </summary>
	public static class EnumUtils
	{
		// Returns all values of enum as IEnumerable.
    	public static IEnumerable<T> GetValues<T>() {
        	return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    	}

		// Returns all values of enum as List.
		public static List<T> GetValuesAsList<T>() {
        	return Enum.GetValues(typeof(T)).Cast<T>().ToList();
    	}	

		/// <summary>
		/// Returns string array of all elements of enum.
		/// </summary>
		public static string[] GetValuesAsStringArray<T>() {
        	List<T> values = Enum.GetValues(typeof(T)).Cast<T>().ToList();

			string[] allElements = new string[values.Count];
			int index = 0;

			foreach (T t in values) {
				allElements[index] = t.ToString();
				index++;
			}	
			return allElements;
    	}	
	}
}
