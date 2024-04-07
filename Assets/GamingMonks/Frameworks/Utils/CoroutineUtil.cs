using System.Collections;
using UnityEngine;

namespace GamingMonks.Utils
{
	/// <summary>
    /// Extention class for wait coroutine for real seconds.
    /// </summary>
	public class CoroutineUtil : MonoBehaviour 
	{
		public static IEnumerator WaitForRealSeconds(float time)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + time)
			{
				yield return null;
			}
		}
	}
}
