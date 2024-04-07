using UnityEngine;

namespace GamingMonks
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
