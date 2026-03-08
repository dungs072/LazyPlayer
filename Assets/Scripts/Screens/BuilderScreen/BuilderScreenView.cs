using System;
using BaseEngine;
using UnityEngine;

[Serializable]
public class BuilderScreenView
{
    [SerializeField]
    public BuildingScroller scroller;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon closeButton;
}
