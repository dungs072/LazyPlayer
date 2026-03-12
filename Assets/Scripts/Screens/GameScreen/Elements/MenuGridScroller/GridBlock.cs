using PolyAndCode.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridBlock : MonoBehaviour, ICell
{
    [SerializeField]
    private Image icon;

    [SerializeField]
    private TMP_Text nameText;

    public void SetInfo(MenuGridData data)
    {
        icon.sprite = data.Icon;
        nameText.text = data.Name;
    }
}
