using UnityEngine;

namespace GamingMonks.Tutorial
{
    /// <summary>
    /// This script component is attached to all block shape containers.
    /// </summary>
	public class ShapeContainer : MonoBehaviour {

        // Rect transfrom of the container inside which block shape spawns.
        [System.NonSerialized] public RectTransform blockParent;
        
        // Assigned block shape.
        [System.NonSerialized] public BlockShape blockShape;

        /// <summary>
        /// Awakes the script instance and initializes block parent to cache it.
		/// Awake is called when the script instance is being loaded.
		/// </summary>
        private void Awake() {
            blockParent = GetComponent<RectTransform>();
        }

        /// <summary>
        /// Resets and destroy block shape on game over or game leave.
        /// </summary>
        public void Reset() {
            if(blockShape != null) {
                Destroy(blockShape.gameObject);
                blockShape = null;
            }
        }
    }
}

