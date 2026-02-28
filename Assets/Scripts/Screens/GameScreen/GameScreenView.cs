using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using BaseEngine;
[Serializable]
public class GameScreenView
{
    [SerializeField] public TMP_Text wheatAmount;
    [SerializeField] public TMP_Text breadAmount;
    [SerializeField] public TMP_Text moneyAmount;
    [SerializeField] public MagicButton preButton;
    [SerializeField] public MagicButton nextButton;
    [SerializeField] public MagicButton staffButton;
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
}
