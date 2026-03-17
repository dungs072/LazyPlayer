using System;
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
    private MenuGridData data;
    private MagicButtonWithIcon button;
    public MenuGridData Data => data;

    void Awake()
    {
        button = GetComponent<MagicButtonWithIcon>();
    }

    public void AddListener(Action action)
    {
        button.AddListener(action);
    }

    void OnDestroy()
    {
        button.RemoveAllListeners();
    }

    public void SetInfo(MenuGridData data)
    {
        this.data = data;
        icon.sprite = data.Icon;
        nameText.text = data.Name;
    }
}
