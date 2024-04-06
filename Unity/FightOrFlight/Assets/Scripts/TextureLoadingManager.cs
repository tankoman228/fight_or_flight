using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Загружает текстуры из ресурсов, помогает доставать их из полученного кэша, а не ручками
/// </summary>
public static class TextureLoadingManager {

    public static Sprite[] spritesObjects;
    public static Dictionary<string, Sprite> spritesForCharacters = new Dictionary<string, Sprite>();

    static TextureLoadingManager()
    {
        if (spritesObjects == null)
            spritesObjects = Resources.LoadAll<Sprite>("Objects");

        foreach (var sprite in Resources.LoadAll<Sprite>("Players"))
        {
            spritesForCharacters.Add(sprite.name, sprite);
        }
    }

    /// <summary>
    /// Загружает текстуру предмета согласно указанному типу
    /// </summary>
    /// <param name="itemType">Тип предмета, для которого найдёт текстуру</param>
    /// <param name="image">То, где нужно текстуру отобразить. Должен иметь тип Image или SpriteRenderer</param>
    public static void loadSprite(ItemStats.ItemTypes itemType, object image)
    {
        switch (itemType)
        {
            case ItemStats.ItemTypes.pick:
                loadSprite("Pickaxe", image); break;
            case ItemStats.ItemTypes.chainsaw:
                loadSprite("Chainsaw", image); break;
            case ItemStats.ItemTypes.pistol:
                loadSprite("Pistol", image); break;
            case ItemStats.ItemTypes.reagents:
                loadSprite("ToxicGrenade", image); break;
            case ItemStats.ItemTypes.machine_gun:
                loadSprite("Machinegun", image); break;
            case ItemStats.ItemTypes.flamethrower:
                loadSprite("Flamethrower", image); break;
            case ItemStats.ItemTypes.sprayer:
                loadSprite("Sprayer", image); break;
            case ItemStats.ItemTypes.plasma_cutter:
                loadSprite("PlasmaCutter", image); break;
            case ItemStats.ItemTypes.knife:
                loadSprite("Knife", image); break;
            case ItemStats.ItemTypes.first_aid_kit:
                loadSprite("MedKit", image); break;
            case ItemStats.ItemTypes.mine:
                loadSprite("Mine", image); break;
            case ItemStats.ItemTypes.dynamite:
                loadSprite("Bomb", image); break;
            case ItemStats.ItemTypes.armor:
                loadSprite("Armor", image); break;
            case ItemStats.ItemTypes.stimulant:
                loadSprite("Stimulator", image); break;
            case ItemStats.ItemTypes.didgeridoo:
                loadSprite("Didgeridoo2", image); break;
            case ItemStats.ItemTypes.invisiblity_hat:
                loadSprite("InvisibleHat", image); break;
            case ItemStats.ItemTypes.goo_absorber:
                loadSprite("Plant", image); break;
            case ItemStats.ItemTypes.bite:
                loadSprite("Big_Blood", image); break;
            case ItemStats.ItemTypes.tongue:
                loadSprite("Tongue", image); break;
            case ItemStats.ItemTypes.goo_imitator:
                loadSprite("GooImitator", image); break;
            case ItemStats.ItemTypes.walls_breaker:
                loadSprite("WallsBreaker", image); break;
            case ItemStats.ItemTypes.claws:
                loadSprite("Claws", image); break;
            case ItemStats.ItemTypes.jump:
                loadSprite("Jump", image); break;
            default:
                Debug.Log("Unknown item!");
                break;
        }
    }

    private static void loadSprite(string spriteName, object image)
    {
        Sprite desiredSprite = null;
        foreach (Sprite sprite in spritesObjects)
        {
            if (sprite.name == spriteName)
            {
                desiredSprite = sprite;
                break;
            }
        }
        if (desiredSprite == null)
        {
            Debug.LogError("Спрайт не найден: " + spriteName);
            return;
        }

        if (image is Image)
        {
            Image image_ = (Image)image;

            image_.sprite = desiredSprite;
        }
        else if (image is SpriteRenderer)
        {
            SpriteRenderer image_ = (SpriteRenderer)image;

            image_.sprite = desiredSprite;
        }
        else
        {
            Debug.LogError("UNKNOWN TYPE");
        }
    }
}
