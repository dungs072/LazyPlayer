using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Resources;
public class GameScreen : BaseScreen
{
    [SerializeField] private TMP_Text wheatAmount;
    [SerializeField] private TMP_Text breadAmount;
    [SerializeField] private TMP_Text moneyAmount;
    [SerializeField] private Button preButton;
    [SerializeField] private Button nextButton;
    private int currentMapIndex = 0;

    private void Awake()
    {
        ResourcesManager.OnResourceAmountChanged += SetResourcesAmount;
        preButton.onClick.AddListener(OnPreButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }
    private void OnDestroy()
    {
        ResourcesManager.OnResourceAmountChanged -= SetResourcesAmount;
        preButton.onClick.RemoveListener(OnPreButtonClicked);
        nextButton.onClick.RemoveListener(OnNextButtonClicked);
    }
    private void Start()
    {
        currentMapIndex = 0;
    }

    public void SetResourcesAmount(string resourceName, int amount)
    {
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
    public void OnPreButtonClicked()
    {
        if (currentMapIndex == 0) return;
        var cameraController = GameManager.Instance.CameraController;
        cameraController.MovePreviousMap();
        currentMapIndex--;
    }
    public void OnNextButtonClicked()
    {
        if (currentMapIndex == MapConstant.TOTAL_MAPS - 1) return;
        var cameraController = GameManager.Instance.CameraController;
        cameraController.MoveNextMap();
        currentMapIndex++;
    }
}
