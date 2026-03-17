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

public enum ButtonTab3Type
{
    // BUILDER
    BUILD,
    EDIT,
    BUILD_BACK,
    BUILDING_LIST,
}

[Serializable]
public class Tab1ButtonInfo
{
    public ButtonTab1Type type;
    public List<Tab2ButtonInfo> icon;
}

[Serializable]
public class Tab2ButtonInfo
{
    public ButtonTab2Type type;
    public Sprite icon;
    public List<Tab3ButtonInfo> tab3Buttons;
}

[Serializable]
public class Tab3ButtonInfo
{
    public ButtonTab3Type type;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "NewMenuPanelButtonData", menuName = "UI/MenuPanelButtonData")]
public class MenuPanelButtonData : ScriptableObject
{
    [SerializeField]
    private Tab1ButtonInfo[] tab1ButtonsInfo;

    [SerializeField]
    private Sprite lockSprite;
    public Tab1ButtonInfo[] Tab1ButtonsData => tab1ButtonsInfo;
    public Sprite LockSprite => lockSprite;

    public Tab1ButtonInfo GetTab1ButtonsInfo(ButtonTab1Type tab1Type)
    {
        foreach (var info in tab1ButtonsInfo)
        {
            if (info.type == tab1Type)
                return info;
        }
        return null;
    }
}
