using BaseEngine;
using UnityEngine;
using UnityEngine.UI;

public class MagicButtonWithIcon : BaseEngine.MagicButtonWithIcon
{
    [SerializeField]
    private Image iconImage;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }
}
