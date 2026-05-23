using UnityEngine;


public class PurchaserManager : MonoBehaviour
{
	public enum eTab
	{
		NONE = -1,
		ADS,
		DONATION,
		MAX
	}

	private const int DONATION_MAX = 3;

	private Manager manager;

	private static PurchaserManager instance;

	private GameObject purchaser_menu;

	private GameObject remove_ad_menu;

	private GameObject donation_menu;

	private SpriteRenderer black_bg;

	private SpriteRenderer[] tab = new SpriteRenderer[2];

	private SpriteRenderer remove_ad_check_icon;

	private SpriteRenderer remove_ad_button;

	private SpriteRenderer[] donation_button = new SpriteRenderer[3];

	private SpriteRenderer[] donation_check_icon = new SpriteRenderer[3];

	public int tab_flag;

	private int button_on = -1;

	private TextMesh remove_ad_price_text;

	private TextMesh[] donation_price_text = new TextMesh[3];

	public void Init(GameObject obj)
	{
		instance = this;
		manager = GameObject.Find("Manager").GetComponent<Manager>();
		Manager.office.menu.gameObject.SetActive(value: false);
		purchaser_menu = obj;
		black_bg = purchaser_menu.transform.Find("black_bg").GetComponent<SpriteRenderer>();
		black_bg.transform.localScale = new Vector3(50f, 50f, 1f);
		black_bg.color = new Color(0f, 0f, 0f, 0.3f);
		TouchEvent touch_black = black_bg.GetComponent<TouchEvent>();
		touch_black.ClickUp.AddListener(delegate
		{
			TouchBlackBg();
		});
		for (int i = 0; i < 2; i++)
		{
			tab[i] = purchaser_menu.transform.Find("tab" + (i + 1)).GetComponent<SpriteRenderer>();
			SpriteRenderer spriteRenderer = tab[i];
			TouchEvent component = spriteRenderer.GetComponent<TouchEvent>();
			eTab tab_count = (eTab)i;
			component.ClickDown.AddListener(delegate
			{
				Manager.sound.PlaySe(Sound.eSe.CLICK);
			});
			component.ClickUp.AddListener(delegate
			{
				ChangeTab(touch_black, tab_count);
			});
		}
		SetTabSprite((eTab)tab_flag);
		remove_ad_menu = purchaser_menu.transform.Find("delete_ad").gameObject;
		donation_menu = purchaser_menu.transform.Find("donation").gameObject;
		remove_ad_check_icon = remove_ad_menu.transform.Find("check").GetComponent<SpriteRenderer>();
		remove_ad_button = remove_ad_menu.transform.Find("button/buy_button").GetComponent<SpriteRenderer>();
		remove_ad_price_text = remove_ad_menu.transform.Find("button/price_text").GetComponent<TextMesh>();
		for (int j = 0; j < 3; j++)
		{
			donation_check_icon[j] = donation_menu.transform.Find("check" + (j + 1)).GetComponent<SpriteRenderer>();
			donation_button[j] = donation_menu.transform.Find("button" + (j + 1) + "/buy_button").GetComponent<SpriteRenderer>();
			donation_price_text[j] = donation_menu.transform.Find("button" + (j + 1) + "/price_text").GetComponent<TextMesh>();
		}
		GameObject gameObject = remove_ad_menu.transform.Find("button_bg2").gameObject;
		gameObject.SetActive(value: false);
		SetSubMenu((eTab)tab_flag);
	}

	private void ChangeTab(TouchEvent touch, eTab type)
	{
		SetTabSprite(type);
		tab_flag = (int)type;
		SetSubMenu(type);
	}

	private void SetTabSprite(eTab type)
	{
		for (int i = 0; i < 2; i++)
		{
			tab[i].sprite = SpriteManager.GetPurchaserTab(0);
		}
		tab[(int)type].sprite = SpriteManager.GetPurchaserTab(1);
	}

	private void SetSubMenu(eTab type)
	{
		if (type == eTab.ADS)
		{
			remove_ad_menu.SetActive(value: true);
			donation_menu.SetActive(value: false);
			SetAdRemoveCheck();
		}
		else
		{
			remove_ad_menu.SetActive(value: false);
			donation_menu.SetActive(value: true);
			SetDonationCheck();
		}
	}

	public static void SetDonationCheck()
	{
		if (instance != null)
		{
			instance.setDonationCheck();
		}
	}

	private void setDonationCheck()
	{
		for (int i = 0; i < 3; i++)
		{
			if (manager.data.purchase[i + 1] == 0)
			{
				donation_price_text[i].transform.localPosition = new Vector3(0f, -0.267f, -10f);
			}
			else
			{
				donation_price_text[i].transform.localPosition = new Vector3(0f, -0.288f, -10f);
			}
		}
		if (!manager.purchaser.m_InitializeSuccess)
		{
			for (int j = 0; j < 3; j++)
			{
				donation_button[j].sprite = SpriteManager.GetPurchaserButton(1);
				donation_price_text[j].transform.localPosition = new Vector3(0f, -0.288f, -10f);
			}
		}
		else
		{
			for (int k = 0; k < 3; k++)
			{
				donation_button[k].sprite = SpriteManager.GetPurchaserButton(manager.data.purchase[k + 1]);
			}
		}
		for (int l = 0; l < 3; l++)
		{
			donation_check_icon[l].sprite = SpriteManager.GetPurchaserCheckIcon(manager.data.purchase[l + 1]);
		}
		for (int m = 0; m < 3; m++)
		{
			donation_price_text[m].text = Purchaser.price[m + 1];
		}
	}

	public void SetButtonId(int i)
	{
		button_on = i;
	}

	public void TouchDonation(int i)
	{
		if (manager.data.purchase[i] == 0 && button_on == i && manager.purchaser.CheckPurchaserState())
		{
			manager.purchaser.BuyNonConsumable((Purchaser.eTYPE)i);
		}
	}

	public void TouchRemoveAd(int i)
	{
		if (manager.data.purchase[0] == 0 && button_on == i && manager.purchaser.CheckPurchaserState())
		{
			manager.purchaser.BuyNonConsumable(Purchaser.eTYPE.ADS);
		}
	}

	public void TouchRestore()
	{
		if (manager.purchaser.CheckPurchaserState())
		{
			if (manager.purchaser.m_PurchaseProcessing)
			{
				Utils.Log("Currently PurchasesProcessing.");
				return;
			}
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			manager.purchaser.RestorePurchases();
		}
	}

	public static void SetAdRemoveCheck()
	{
		if (instance != null)
		{
			instance.setAdRemoveCheck();
		}
	}

	private void setAdRemoveCheck()
	{
		remove_ad_price_text.text = Purchaser.price[0];
		if (manager.data.purchase[0] == 0)
		{
			remove_ad_price_text.transform.localPosition = new Vector3(0f, -0.267f, -10f);
		}
		else
		{
			remove_ad_price_text.transform.localPosition = new Vector3(0f, -0.288f, -10f);
		}
		if (!manager.purchaser.m_InitializeSuccess)
		{
			remove_ad_button.sprite = SpriteManager.GetPurchaserButton(1);
			remove_ad_price_text.transform.localPosition = new Vector3(0f, -0.288f, -10f);
		}
		else
		{
			remove_ad_button.sprite = SpriteManager.GetPurchaserButton(manager.data.purchase[0]);
		}
		remove_ad_check_icon.sprite = SpriteManager.GetPurchaserCheckIcon(manager.data.purchase[0]);
	}

	public static void DecideNoAds()
	{
		Manager.GetData().SetPurchase(Purchaser.eTYPE.ADS, 1);
		WebMediator.RemoveBanner();
		if (instance != null)
		{
			instance.decideNoAds();
		}
	}

	private void decideNoAds()
	{
		if (purchaser_menu != null)
		{
			PurchaserUpdata();
		}
		SetButtonId(-1);
	}

	public static void DecideDonation(Purchaser.eTYPE type)
	{
		Manager.GetData().SetPurchase(type, 1);
		if (instance != null)
		{
			instance.decideDonation(type);
		}
	}

	private void decideDonation(Purchaser.eTYPE type)
	{
		if (purchaser_menu != null)
		{
			PurchaserUpdata();
		}
		SetButtonId(-1);
	}



	
	private void PurchaserUpdata()
	{
		if (tab_flag == 0)
		{
			SetAdRemoveCheck();
		}
		else
		{
			SetDonationCheck();
		}
	}

	public void RemoveAdButtonDownAnim(int i)
	{
		if (manager.data.purchase[0] == 0 && manager.purchaser.CheckPurchaserState() && button_on == -1)
		{
			SetButtonId(i);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			Animation component = remove_ad_menu.transform.Find("button").GetComponent<Animation>();
			Utils.Play(component, "purchaser_button_down", 1f, 1f);
		}
	}

	public static void RemoveAdButtonChangeAnim()
	{
		if (instance != null)
		{
			instance.removeAdButtonChangeAnim();
		}
	}

	private void removeAdButtonChangeAnim()
	{
		Animation component = remove_ad_menu.transform.Find("button").GetComponent<Animation>();
		Utils.Play(component, "purchaser_button_change", 1f, 1f);
	}

	public void DonationButtonDownAnim(int i)
	{
		if (manager.data.purchase[i] == 0 && manager.purchaser.CheckPurchaserState() && button_on == -1)
		{
			SetButtonId(i);
			Manager.sound.PlaySe(Sound.eSe.CLICK);
			Animation component = donation_menu.transform.Find("button" + i).GetComponent<Animation>();
			Utils.Play(component, "purchaser_button_down", 1f, 1f);
		}
	}

	public static void DonationButtonChangeAnim(Purchaser.eTYPE type)
	{
		if (instance != null)
		{
			instance.donationButtonChangeAnim(type);
		}
	}

	private void donationButtonChangeAnim(Purchaser.eTYPE type)
	{
		Animation component = donation_menu.transform.Find("button" + (int)type).GetComponent<Animation>();
		Utils.Play(component, "purchaser_button_change", 1f, 1f);
	}

	public static eTab GetTab()
	{
		return (eTab)((!(instance == null)) ? instance.tab_flag : (-1));
	}

	public void TouchBlackBg()
	{
		Manager.sound.PlaySe(Sound.eSe.CANCEL);
		if (purchaser_menu != null)
		{
			UnityEngine.Object.Destroy(purchaser_menu.gameObject);
			purchaser_menu = null;
		}
		Manager.office.menu.gameObject.SetActive(value: true);
		Manager.office.menu.ResetButton();
		Manager.office.menu.menu_flag = 1;
	}
}
