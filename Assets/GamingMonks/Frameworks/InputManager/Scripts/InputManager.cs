using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace GamingMonks
{
	public class InputManager : Singleton<InputManager>
	{
		static bool isTouchAvailable = true;
		public EventSystem eventSystem;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		private void Awake() {
			if(eventSystem == null) {
				eventSystem = FindObjectOfType<EventSystem>() as EventSystem;
			}	
		}

		public bool canInput (float delay = 0.25F, bool disableOnAvailable = true)
		{
			bool status = isTouchAvailable;
			if (status && disableOnAvailable) {
				isTouchAvailable = false;
				eventSystem.enabled = false;

				StopCoroutine ("EnableTouchAfterDelay");
				StartCoroutine ("EnableTouchAfterDelay", delay);

			}
			return status;
		}

		public void DisableTouch()
		{
			isTouchAvailable = false;
			eventSystem.enabled = false;
		}

		public void DisableTouchForDelay (float delay = 0.25F)
		{
			isTouchAvailable = false;
			eventSystem.enabled = false;

			StopCoroutine ("EnableTouchAfterDelay");
			StartCoroutine ("EnableTouchAfterDelay", delay);
		}

		public void EnableTouch ()
		{
			isTouchAvailable = true;
			eventSystem.enabled = true;
		}

		public IEnumerator EnableTouchAfterDelay (float delay)
		{
			yield return new WaitForSeconds (delay);
			EnableTouch ();
		}
	}
}
