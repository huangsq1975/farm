using SWorker;
using System.Collections;
using System.IO;
using UnityEngine;

public class LinkEvent
{
	public enum eApp
	{
		SHIBAINU,
		MAX
	}

	public static string[] TITLE = new string[2]
	{
		"[微型牧场] \n",
		"Tiny Pixel Farm\n"
	};

	public static string[] LINK_IOS = new string[2]
	{
		"iOS: https://itunes.apple.com/jp/app/id1328266576 \n",
		"iOS: https://itunes.apple.com/app/id1328266576 \n"
	};

	public static string[] LINK_ANDROID = new string[2]
	{
		"Android: https://play.google.com/store/apps/details?id=net.appmaga.pixelfarm&hl=ja",
		"Android: https://play.google.com/store/apps/details?id=net.appmaga.pixelfarm"
	};

	public static string APPS_IOS_LINK = "itms-apps://itunes.apple.com/us/developer/game-start-llc/id345705530";

	public static string APPS_ANDROID_LINK = "https://play.google.com/store/apps/developer?id=GAME+START+LLC";

	public static string TWITTER_LINK = "https://twitter.com/game_start_llc";

	public static readonly string[] androidID = new string[2]
	{
		"jp.gamestart.Shibainu",
		"jp.gamestart.Shibainu"
	};

	public static readonly string[] iosID = new string[2]
	{
		"id1437764328",
		"id1437764328"
	};

	private static bool m_ImageSaving = false;

	private static Data data;

	public static void Init(Data d)
	{
		data = d;
	}

	public static void GoTodApps()
	{
		Application.OpenURL(APPS_ANDROID_LINK);
	}

	public static void GoToPrivacyPolicy()
	{
		Application.OpenURL(WebMediator.GetPrivacyPolicy());
	}

	public static void GoToSns()
	{
		Application.OpenURL(TWITTER_LINK);
	}

	public static void DoTweet(MonoBehaviour obj)
	{
		if (!m_ImageSaving)
		{
			obj.StartCoroutine(Share());
		}
	}

	private static IEnumerator Share()
	{
		m_ImageSaving = true;
		string file = "screenshot.png";
		string path = Application.persistentDataPath + "/" + file;
		if (Directory.Exists(Application.persistentDataPath))
		{
			File.Delete(path);
			yield return new WaitForEndOfFrame();
			float time2;
			for (time2 = 0f; time2 < 5f; time2 += Time.deltaTime)
			{
				yield return null;
				FileInfo fi2 = new FileInfo(path);
				if (fi2 == null || !fi2.Exists)
				{
					break;
				}
			}
			if (time2 >= 5f)
			{
				path = null;
			}
			else
			{
				ScreenCapture.CaptureScreenshot(file);
				long filesize = 0L;
				time2 = 0f;
				while (filesize == 0 && time2 < 5f)
				{
					yield return null;
					FileInfo fi = new FileInfo(path);
					if (fi != null && fi.Exists)
					{
						filesize = fi.Length;
					}
					time2 += Time.deltaTime;
				}
				if (filesize == 0)
				{
					path = null;
				}
			}
		}
		else
		{
			path = null;
		}
		string str = GetTweetStr();
		if (!WebMediator.no_native)
		{
			SocialWorker.CreateChooser(str, path);
		}
		m_ImageSaving = false;
	}

	public static string GetTweetStr()
	{
		int lang = (int)data.lang;
		string text = TITLE[lang] + LINK_IOS[lang] + LINK_ANDROID[lang];
		UnityEngine.Debug.Log(text);
		return text;
	}

	public static void PushedRecommend(int type)
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=" + androidID[type]);
	}
}
