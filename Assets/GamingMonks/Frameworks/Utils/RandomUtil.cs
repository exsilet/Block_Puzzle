using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks.Utils
{
	/// <summary>
    /// Extention class generating random nummbers.
    /// </summary>
	public class RandomUtil : MonoBehaviour {

		// Returns unique random numbers from the given range.
		public static List<int> GetNonRepeatingRandomNumbers(int startRange, int endRange, int noOfRandoms) {
			List<int> randomsList = new List<int>();
			for(int index = startRange; index < endRange; index++) {
				randomsList.Add(index);
			}
			randomsList.Shuffle();
			return randomsList.GetRange(0,noOfRandoms);
		}
	}
}

