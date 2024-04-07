using UnityEngine;

namespace GamingMonks
{
	/// <summary>
    /// Creates a instance of monobehaviour of the given script component.static You can implement this singleton behaviour in generic 
	/// form which in turn will create instance of the class in static form which can be accesses easily.
    /// </summary>
	public class Singleton<T> : MonoBehaviour where T : Component {

		private static T instance;
		public static T Instance {
			get{
				if (instance == null) {
					instance = FindObjectOfType<T> ();

					if (instance == null) {
						GameObject g = new GameObject ("Controller");
						instance = g.AddComponent<T> ();
						//g.hideFlags = HideFlags.HideInHierarchy;

					}
				}
				return instance;
			}
		}

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		void Awake()
		{
			//DontDestroyOnLoad (gameObject);
			if (instance == null ) {
				instance = this as T;
			} else {
				if (instance != this) {
					Destroy (gameObject);
				}
			}
		}
	}
}
