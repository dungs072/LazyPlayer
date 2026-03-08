using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public enum ButtonTab1Type
{
    MANAGEMENT,
    SHOP,
    GALLERY,
    SETTING,
}

public enum ButtonTab2Type
{
    STAFF,
    FARM_STORAGE,
    KITCHEN_STORAGE,
    BUILDER,
    SOUND,
}

[Serializable]
public class Tab2ButtonInfo
{
    public ButtonTab2Type type;
    public Sprite icon;
}

[Serializable]
public class Tab1ButtonInfo
{
    public ButtonTab1Type type;
    public List<Tab2ButtonInfo> icon;
}

[CreateAssetMenu(fileName = "NewTab2ButtonData", menuName = "UI/Tab2ButtonData")]
public class Tab2ButtonData : ScriptableObject
{
    [SerializeField]
    private Tab1ButtonInfo[] tab2ButtonsInfo;

    [SerializeField]
    private Sprite lockSprite;
    public Tab1ButtonInfo[] Tab2ButtonsData => tab2ButtonsInfo;
    public Sprite LockSprite => lockSprite;

    public Tab1ButtonInfo GetTab2ButtonsInfo(ButtonTab1Type tab1Type)
    {
        foreach (var info in tab2ButtonsInfo)
        {
            if (info.type == tab1Type)
                return info;
        }
        return null;
    }
}
