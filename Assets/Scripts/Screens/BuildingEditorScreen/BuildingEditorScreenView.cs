using System;
using BaseEngine;
using UnityEngine;

[Serializable]
public class BuildingEditorScreenView
{
    [SerializeField]
    public BaseEngine.MagicButtonWithIcon closeButton;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon rotateButton;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon buildButton;
}
