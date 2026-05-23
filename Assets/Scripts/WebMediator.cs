#define DEFINE_NATIVE

using System;
using System.Collections;
using UnityEngine;


public class WebMediator : MonoBehaviour
{
	[Serializable]
	public class AdmobUnitID
	{
		public string app = string.Empty;

		public string banner = string.Empty;

		public string interstitial = string.Empty;

		public string video = string.Empty;
	}

	[Serializable]
	public class NativeSetting
	{
		public string log_tag = string.Empty;

		public string publisher_id = string.Empty;

		public string privacy_policy_url = string.Empty;

		public AdmobUnitID android;

		public AdmobUnitID ios;

		public string object_name = string.Empty;
	}

	public class ScreenInfo
	{
		public float Xper;

		public float Yper;

		public float Width;

		public float Height;

		public ScreenInfo(float x_per, float y_per, float width, float height)
		{
			Xper = x_per;
			Yper = y_per;
			Width = width;
			Height = height;
		}
	}

	public const string WebMediatorPath = "WebMediator";

	public NativeSetting setting;

	public static bool no_native;

	private static WebMediator instance;

	public static void Install(bool __no_native)
	{
		if (!IsThereInstance())
		{
            Debug.Log("InstallWeb");
			no_native = __no_native;
			GameObject gameObject = Resources.Load("WebMediator") as GameObject;
			instance = UnityEngine.Object.Instantiate(gameObject).GetComponent<WebMediator>();
			instance.name = gameObject.name;
			UnityEngine.Object.DontDestroyOnLoad(instance);
			InstallPlatform();
		}
	}

	public static bool IsThereInstance()
	{
		return instance != null;
	}

	public static string GetPrivacyPolicy()
	{
		return (!(instance != null)) ? string.Empty : instance.setting.privacy_policy_url;
	}

	private static void InstallPlatform()
	{
		if (no_native)
		{
			Utils.Log("InstallPlatform");
			return;
		}
#if DEFINE_NATIVE
        AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        @static.Call("Init", instance.setting.log_tag, instance.setting.publisher_id, instance.setting.android.app,
            instance.setting.android.banner, instance.setting.android.interstitial, instance.setting.android.video, instance.setting.privacy_policy_url, instance.setting.object_name);
        @static.Dispose();
#endif
    }

	public static void IOSLog(string msg)
	{
	}

	public static string GetVersion()
	{
		if (no_native)
		{
			return Application.version;
		}
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		string result = @static.Call<string>("getVersion", new object[0]);
		@static.Dispose();
		return result;
#else
        return Application.version;
#endif
    }

    public static void DoTweet(string tweet, string path)
	{
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("doTweet", tweet, path);
		@static.Dispose();
#endif
	}

	public static void Quit()
	{
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("quit");
		@static.Dispose();
#endif
	}

	public static void SaveGallery(string path)
	{
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("doSaveGallery", path);
		@static.Dispose();
#else

#endif
	}

    public static void ShowInterstitial()
	{
		if (no_native)
		{
			Utils.Log("no_native: ShowInterstitial");
			instance.StartCoroutine(DebugCoroutine(instance.setting.object_name, "interstitialAdShowed", 0.1f));
			return;
		}
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("showInterstitial");
		@static.Dispose();
#else
		FindObjectOfType<Manager>().ShowInterstitial();
#endif
	}

	public static void LoadInterstitial()
	{
		if (no_native)
		{
			Utils.Log("no_native: LoadInterstitial");
			instance.StartCoroutine(DebugCoroutine(instance.setting.object_name, "interstitialAdLoaded", 0.1f));
			return;
		}
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("loadInterstitial");
		@static.Dispose();
#else
        FindObjectOfType<Manager>().LoadInterstitial();
#endif
    }

    public static void PlayVideo()
	{
		if (no_native)
		{
			instance.StartCoroutine(DebugCoroutine(instance.setting.object_name, "videoAdPlayed", 0.1f));
			return;
		}
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("showVideo");
		@static.Dispose();
#endif
	}

	public static void LoadVideo()
	{
		if (no_native)
		{
			GameObject.Find(instance.setting.object_name).SendMessage("videoAdLoaded", string.Empty);
			return;
		}
#if DEFINE_NATIVE
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		@static.Call("loadVideo");
		@static.Dispose();
#endif
	}

	public static void ShowBanner()
	{
		if (!no_native)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			@static.Call("showBanner");
			@static.Dispose();
#endif
        }
    }

        public static void RemoveBanner()
	{
		if (!no_native)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			@static.Call("removeBanner");
			@static.Dispose();
#endif
		}
	}

	public static ScreenInfo GetScreenInfo()
	{
		return new ScreenInfo(0f, 0f, Screen.width, Screen.height);
	}

	public static bool ChkAmazon()
	{
		if (no_native)
		{
			return false;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			bool result = @static.Call<bool>("chkAmazon", new object[0]);
			@static.Dispose();
			return result;
#endif
		}
		return false;
	}

	public static bool IsEEA()
	{
		bool result = false;
		if (!no_native)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			result = @static.Call<bool>("isEEA", new object[0]);
			@static.Dispose();
#endif
		}
		return result;
	}

	public static void SetConsentForm()
	{
		if (!no_native)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			@static.Call("setConsentForm");
			@static.Dispose();
#endif
		}
		else
		{
			Utils.Log("SetContentForm...");
		}
	}

	public static void PostReportMail(string[] filePaths)
	{
		if (!no_native)
		{
#if DEFINE_NATIVE
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			@static.Call("initFilePathData");
			for (int i = 0; i < filePaths.Length; i++)
			{
				@static.Call("addFilePathData", filePaths[i]);
			}
			@static.Call("postReportMail");
			@static.Dispose();
#endif
		}
		else
		{
			Utils.Log("PostReportMail...");
			foreach (string str in filePaths)
			{
				Utils.Log(" -> Report File Name [ " + str + " ]");
			}
		}
	}

	private static IEnumerator DebugCoroutine(string obj_name, string method_name, float delay_time)
	{
		yield return new WaitForSeconds(delay_time);
		GameObject.Find(obj_name).SendMessage(method_name, string.Empty);
	}
}
