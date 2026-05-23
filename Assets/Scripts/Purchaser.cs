using System;
using UnityEngine;

public class Purchaser : MonoBehaviour

{
	public enum eTYPE
	{
		IGNORE = -1,
		ADS,
		DONATION_1,
		DONATION_2,
		DONATION_3,
		WORKER_FARM,
		WORKER_RESORT,
		MAX
	}

	public const int DONATION_MAX = 3;

	public static string[] price = new string[6]
	{
		"---",
		"---",
		"---",
		"---",
		"---",
		"---"
	};

	

	public bool m_InitializeSuccess;

	public bool m_PurchaseProcessing;

	public bool m_Restore;

	public static string[] kProductIDNonConsumable = new string[6]
	{
		"farm_ad_remove",
		"farm_donation1",
		"farm_donation2",
		"farm_donation3",
		"farm_worker_farm",
		"farm_worker_resort"
	};

	private Manager manager;

	public void Set(Manager _manager)
	{
		manager = _manager;
		
	}

	public void InitializePurchasing()
	{
		if (!IsInitialized())
		{
			//ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			//for (int i = 0; i < kProductIDNonConsumable.Length; i++)
			/**{
				configurationBuilder.AddProduct(kProductIDNonConsumable[i], ProductType.NonConsumable);
			}
			UnityPurchasing.Initialize(this, configurationBuilder);*/
		}
	}

	private bool IsInitialized()
	{
		return false;
	}

	public void BuyNonConsumable(eTYPE type)
	{
		BuyProductID(kProductIDNonConsumable[(int)type], type);
	}

	private void BuyProductID(string productId, eTYPE type)
	{
		
	}

	public void RestorePurchases()
	{
		
	}

	

	public void SetRestore()
	{
		
    }



	private eTYPE CompareProduct(string product)
	{
		for (int i = 0; i < kProductIDNonConsumable.Length; i++)
		{
			if (string.Equals(product, kProductIDNonConsumable[i], StringComparison.Ordinal))
			{
				return (eTYPE)i;
			}
		}
		Utils.Log("ILLEGAL: product=" + product);
		return eTYPE.IGNORE;
	}

	public bool CheckPurchaserState()
	{
		if (m_InitializeSuccess)
		{
			if (!m_PurchaseProcessing && !m_Restore)
			{
				return true;
			}
			return false;
		}
		return false;
	}
}
