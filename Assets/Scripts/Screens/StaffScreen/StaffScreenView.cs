using System;
using BaseEngine;
using PolyAndCode.UI;
using UnityEngine;

[Serializable]
public class StaffScreenView
{
    [SerializeField]
    public StaffScroller scroller;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon closeButton;
}
