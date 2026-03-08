using System;
using BaseEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameScreenView
{
    [SerializeField]
    private MenuGamePlayPanel menuGamePlayPanel;

    [SerializeField]
    public TMP_Text wheatAmount;

    [SerializeField]
    public TMP_Text breadAmount;

    [SerializeField]
    public TMP_Text moneyAmount;

    [SerializeField]
    public MagicButton preButton;

    [SerializeField]
    public MagicButton nextButton;

    [SerializeField]
    public MagicButton staffButton;

    [SerializeField]
    public MagicButton builderButton;

    [HideInInspector]
    public int currentMapIndex = 0;

    public void SetResourcesAmount(ResourceAmountChangedEvent e)
    {
        var resourceName = e.name;
        var amount = e.amount;
        if (resourceName == "wheat")
        {
            wheatAmount.text = amount.ToString();
        }
        if (resourceName == "bread")
        {
            breadAmount.text = amount.ToString();
        }
        if (resourceName == "money")
        {
            moneyAmount.text = amount.ToString();
        }
    }

    public void PrepareFadeIn()
    {
        menuGamePlayPanel.PrepareFadeIn();
    }

    public async UniTask FadeInAsync()
    {
        await menuGamePlayPanel.FadeInAsync();
    }
}
