using System;
using System.Collections.Generic;
using System.Linq;

namespace GamingMonks.Utils
{
	/// <summary>
	/// Extention class for string operations.
	/// </summary>
	public static class StringUtil 
	{
		static List<char> characterSet = new List<char>{'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'};
		
		// Ommits all characters except letter from given string.
		public static string GetLetterString(this string str) {
			string formattedString = "";

			foreach(char c in str.ToCharArray()) {
				if(Char.IsLetter(c)) {
					formattedString = String.Concat(formattedString,c);
				}
			}
			return formattedString;
		}

		// Returns missing characters as string for the comparing strings.
		public static string GetMissingCharAsString(this string verifyingString, string intersactString) {
			List<char> missingCharacters = new List<char>();

            foreach(char c in intersactString) {
                if(verifyingString.Contains(c.ToString())) {
                    verifyingString = verifyingString.Replace(c,'?');
                } else {
                    missingCharacters.Add(c);
                }
            }

            string missingCharString = "";
            foreach(char c in missingCharacters) {
               missingCharString = String.Concat(missingCharString,c);
            }
            return missingCharString;
		}

		//Shuffle al characters of string in random order.
		public static string Shuffle(this string str) 
		{
			List<char> charList = new List<char>();
			foreach(char c in str.ToCharArray()) {
				charList.Add(c);
			}
			charList.Shuffle();

			string shuffledString = "";
			foreach(char c in charList) {
				shuffledString = String.Concat(shuffledString, c);
			}
			return shuffledString;
		}

		// Returns string of random characters for the given length.
		public static string GetRandomCharacterString(int stringLength) {
			string randomScrabbledString = "";
            characterSet.Shuffle();
            
            int characterSetLength = characterSet.Count;
            for(int index = 0; index < stringLength; index++) {
                randomScrabbledString = string.Concat(randomScrabbledString, characterSet[UnityEngine.Random.Range(0,characterSetLength)]);
            }
			return randomScrabbledString;
		}

		// Returns given number of random characters for given string.
		public static List<char> GetRandomCharactersFromString(String str, int noOfCharacters) {
			List<char> charArray = (str.ToCharArray().ToList());
			charArray.Shuffle();
			return charArray.GetRange(0, noOfCharacters);
		}
	}
}