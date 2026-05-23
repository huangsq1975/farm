using UnityEngine;

public class SpriteManager : MonoBehaviour
{
	private static SpriteManager sprite_manager;

	public Sprite[] harvest_farm_sprite;

	public Sprite[] harvest_fish_sprite;

	public Sprite construction_sprite;

	public Sprite[] animal_icon_farm_sprite;

	public Sprite[] facility_icon_sprite;

	public Sprite[] item_icon_sprite;

	public Sprite[] sale_icon_sprite;

	public Sprite[] cond_levelup_sprite;

	public Sprite[] album_levelup_harvest_sprite;

	public Sprite[] album_area_sprite;

	public Sprite[] map_coin_and_exp_sprite;

	public Sprite main_hat_sprite;

	public Sprite[] main_bag_sprite;

	public Sprite level_mark_sprite;

	public Sprite[] cond_season_sprite;

	public Sprite[] hotel_star;

	public Sprite[] cond_hotel_star;

	public Sprite[] casher_ng_icon;

	public Sprite[] commercial_icon;

	public Sprite[] sugoroku_mass;

	public Sprite[] casher_fishing_icon;

	public Sprite[] worker_icon;

	public Sprite[] green_icon;

	public Sprite[] worker_star;

	public Sprite[] worker_prompt_star;

	public Sprite[] worker_prompt_button;

	public Sprite[] purchaser_tab;

	public Sprite[] purchaser_check_icon;

	public Sprite[] purchaser_button;

	public Sprite[] ui_coin_sprite;

	public Sprite[] ui_level_sprite;

	public Sprite[] silo_icon_sprite;

	public Sprite[] silo_mini_icon_sprite;

	public Sprite[] room_deco_sprite;

	public Sprite[] character_hat;

	public Sprite[] character_cloth;

	[NamedArray(typeof(PartsController.ePartsCloth), 40, 25)]
	public Sprite[] character_cloth_worker;

	[NamedArray(typeof(PartsController.ePartsCloth), 40, 25)]
	public Sprite[] character_cloth_customer;

	public Sprite[] character_face;

	public Sprite[] character_hair;

	public Sprite[] character_item;

	public Sprite[] woker_store;

	public Sprite[] woker_hotel;

	public Sprite[] woker_office;

	public Sprite[] woker_sailo;

	public Sprite[] woker_facility;

	public Sprite[] present_prompt;

	public Sprite[] album_tab;

	public Sprite[] prompt_coin_type;

	public Sprite[] recommend_banner_en;

	public Sprite[] recommend_banner_jp;

	public void Init()
	{
		sprite_manager = this;
	}

	public static Sprite GetHarvest(FarmAnimal.eType type)
	{
		return sprite_manager.harvest_farm_sprite[(int)type];
	}

	public static Sprite GetHarvest(Fish.eType type)
	{
		return sprite_manager.harvest_fish_sprite[(int)type];
	}

	public static Sprite GetConstruction()
	{
		return sprite_manager.construction_sprite;
	}

	public static Sprite GetAnimalIcon(FarmAnimal.eType type)
	{
		return sprite_manager.animal_icon_farm_sprite[(int)type];
	}

	public static Sprite GetFacilityIcon(Facility.eType type)
	{
		return sprite_manager.facility_icon_sprite[(int)type];
	}

	public static Sprite GetItemIcon(Facility.eItem type)
	{
		return sprite_manager.item_icon_sprite[(int)type];
	}

	public static Sprite GetSaleIcon(int type)
	{
		return sprite_manager.sale_icon_sprite[type];
	}

	public static Sprite GetCondLevelUpHarvest(Data.CharacterData.eType type)
	{
		return sprite_manager.cond_levelup_sprite[(int)type];
	}

	public static Sprite GetLevelUpHarvest(Data.CharacterData.eType type)
	{
		return sprite_manager.album_levelup_harvest_sprite[(int)type];
	}

	public static Sprite GetAlbumArea(Data.CharacterData.eType type)
	{
		return sprite_manager.album_area_sprite[(int)type];
	}

	public static Sprite GetMapCoinExp(Common.eCOIN_EXP coin_exp)
	{
		return sprite_manager.map_coin_and_exp_sprite[(int)coin_exp];
	}

	public static Sprite GetHat()
	{
		return sprite_manager.main_hat_sprite;
	}

	public static Sprite GetBag(int index)
	{
		return sprite_manager.main_bag_sprite[index];
	}

	public static Sprite GetLevelMark()
	{
		return sprite_manager.level_mark_sprite;
	}

	public static Sprite GetCondSeasonHarvest(Common.eSEASON_EVENT event_type)
	{
		return sprite_manager.cond_season_sprite[(int)event_type];
	}

	public static Sprite GetHotelStar(int index)
	{
		return sprite_manager.hotel_star[index];
	}

	public static Sprite GetCondHotelStar(int hotel_level)
	{
		return sprite_manager.cond_hotel_star[hotel_level - 1];
	}

	public static Sprite GetCasherNG(Data.eMainType type)
	{
		return sprite_manager.casher_ng_icon[(int)type];
	}

	public static Sprite GetCommercial(Data.eLang lang)
	{
		return sprite_manager.commercial_icon[(int)lang];
	}

	public static Sprite GetSugorokuMass(SugorokuManager.Mass.eType type)
	{
		return sprite_manager.sugoroku_mass[(int)type];
	}

	public static Sprite GetCasherFishing(Data.eMainType type)
	{
		return sprite_manager.casher_fishing_icon[(int)type];
	}

	public static Sprite GetWorkerIcon(Manager m, Worker.eType type, int i)
	{
		if (m.data.worker_data.worker_level[i] == 0)
		{
			return sprite_manager.worker_icon[0];
		}
		if (m.data.worker_data.worker_place[i] < 0)
		{
			return sprite_manager.worker_icon[(int)(type + 2)];
		}
		return sprite_manager.worker_icon[1];
	}

	public static Sprite GetWorkingIcon(Worker.eType type)
	{
		return sprite_manager.worker_icon[(int)(type + 2)];
	}

	public static Sprite GetGreenIcon()
	{
		return sprite_manager.green_icon[0];
	}

	public static Sprite GetWorkerStar(int i)
	{
		return sprite_manager.worker_star[i];
	}

	public static Sprite GetWorkerPromptStar(int i)
	{
		return sprite_manager.worker_prompt_star[i];
	}

	public static Sprite GetWorkerPromptButton(int i)
	{
		return sprite_manager.worker_prompt_button[i];
	}

	public static Sprite GetPurchaserTab(int i)
	{
		return sprite_manager.purchaser_tab[i];
	}

	public static Sprite GetPurchaserCheckIcon(int i)
	{
		return sprite_manager.purchaser_check_icon[i];
	}

	public static Sprite GetPurchaserButton(int i)
	{
		return sprite_manager.purchaser_button[i];
	}

	public static Sprite GetUiCoinSprite(Data.eFarmType ftype)
	{
		return sprite_manager.ui_coin_sprite[(int)ftype];
	}

	public static Sprite GetUiLevelSprite(Data.eFarmType ftype)
	{
		return sprite_manager.ui_level_sprite[(int)ftype];
	}

	public static Sprite GetSiloIcon(Data.eFarmType ftype)
	{
		return sprite_manager.silo_icon_sprite[(int)ftype];
	}

	public static Sprite GetSiloMiniIcon(Data.eFarmType ftype)
	{
		return sprite_manager.silo_mini_icon_sprite[(int)ftype];
	}

	public static Sprite GetRoomDeco(Data.eFarmType ftype)
	{
		return sprite_manager.room_deco_sprite[(int)ftype];
	}

	public static Sprite GetCharacterHat(int i)
	{
		return sprite_manager.character_hat[i];
	}

	public static Sprite GetCharacterCloth(PartsController.eCharacter character, int i)
	{
		Sprite sprite = null;
		switch (character)
		{
		case PartsController.eCharacter.MAIN:
			return sprite_manager.character_cloth[i];
		case PartsController.eCharacter.WORKER:
			return sprite_manager.character_cloth_worker[i];
		default:
			return sprite_manager.character_cloth_customer[i];
		}
	}

	public static Sprite GetCharacterFace(int i)
	{
		return sprite_manager.character_face[i];
	}

	public static Sprite GetCharacterHair(int i)
	{
		return sprite_manager.character_hair[i];
	}

	public static Sprite GetCharacterItem(int i)
	{
		return sprite_manager.character_item[i];
	}

	public static Sprite GetWorkeStore(Data.eFarmType ftype)
	{
		return sprite_manager.woker_store[(int)ftype];
	}

	public static Sprite GetWorkeOffice(Data.eFarmType ftype)
	{
		return sprite_manager.woker_office[(int)ftype];
	}

	public static Sprite GetWorkeSailo(Data.eFarmType ftype)
	{
		return sprite_manager.woker_sailo[(int)ftype];
	}

	public static Sprite GetWorkeHotel(Data.eFarmType ftype)
	{
		return sprite_manager.woker_hotel[(int)ftype];
	}

	public static Sprite GetWorkeFacility(Data.eFarmType ftype)
	{
		return sprite_manager.woker_facility[(int)ftype];
	}

	public static Sprite GetPresentPromptBg(Data.eFarmType ftype)
	{
		return sprite_manager.present_prompt[(int)ftype];
	}

	public static Sprite GetAlbumTabBg(int album_type)
	{
		return sprite_manager.album_tab[album_type];
	}

	public static Sprite GetPromptCoinType(Data.eFarmType ftype)
	{
		return sprite_manager.prompt_coin_type[(int)ftype];
	}

	public static Sprite GetRecommendEn(LinkEvent.eApp app)
	{
		return sprite_manager.recommend_banner_en[(int)app];
	}

	public static Sprite GetRecommendJp(LinkEvent.eApp app)
	{
		return sprite_manager.recommend_banner_jp[(int)app];
	}
}
