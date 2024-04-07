using System.Collections;
using UnityEngine;

namespace GamingMonks
{
    public class BoomerangHudController : MonoBehaviour
    {
        private bool startRotate = false;
        private void OnEnable()
        {
            startRotate = true;
        }
        private void OnDisable()
        {
            startRotate = false;
        }
        void Update()
        {
            if (startRotate == true)
            {
                transform.Rotate(0, 0, -15f);
            }
        }
    }
}