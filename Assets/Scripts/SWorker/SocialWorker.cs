using System;
using UnityEngine;

namespace SWorker
{
	public class SocialWorker : MonoBehaviour
	{
		private static Action<SocialWorkerResult> onResult;

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public static void PostTwitter(string message, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			PostTwitter(message, null, imagePath, onResult);
		}

		public static void PostTwitter(string message, string url, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			if (url == null)
			{
				url = string.Empty;
			}
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
		}

		public static void PostFacebook(string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
		}

		public static void PostLine(string message, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
		}

		public static void PostInstagram(string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
		}

		public static void PostMail(string message, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			PostMail(null, null, null, null, message, imagePath, onResult);
		}

		public static void PostMail(string[] to, string[] cc, string[] bcc, string subject, string message, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (to == null)
			{
				to = new string[1]
				{
					string.Empty
				};
			}
			if (cc == null)
			{
				cc = new string[1]
				{
					string.Empty
				};
			}
			if (bcc == null)
			{
				bcc = new string[1]
				{
					string.Empty
				};
			}
			if (subject == null)
			{
				subject = string.Empty;
			}
			if (message == null)
			{
				message = string.Empty;
			}
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
		}

		public static void CreateChooser(string message, string imagePath, Action<SocialWorkerResult> onResult = null)
		{
			if (message == null)
			{
				message = string.Empty;
			}
			if (imagePath == null)
			{
				imagePath = string.Empty;
			}
			SocialWorker.onResult = onResult;
#if UNITY_ANDROID
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			@static.Call("createChooser", message, imagePath);
			@static.Dispose();
#endif
		}

		public void OnSocialWorkerResult(string res)
		{
			if (onResult != null)
			{
				onResult((SocialWorkerResult)int.Parse(res));
				onResult = null;
			}
		}
	}
}
