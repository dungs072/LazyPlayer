using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Resources;
public class GameScreen : BaseScreen
{
    [SerializeField] private GameScreenView view = new();


    private void Awake()
    {
        ResourcesManager.OnResourceAmountChanged += view.SetResourcesAmount;
        view.preButton.AddListener(OnPreButtonClicked);
        view.nextButton.AddListener(OnNextButtonClicked);
    }
    private void OnDestroy()
    {
        ResourcesManager.OnResourceAmountChanged -= view.SetResourcesAmount;
        view.preButton.RemoveListener(OnPreButtonClicked);
        view.nextButton.RemoveListener(OnNextButtonClicked);
    }
    public void OnPreButtonClicked()
    {
        if (view.currentMapIndex == 0) return;
        GamePlugin.BlockInput(true);
        var cameraController = GameManager.Instance.CameraController;
        cameraController.MovePreviousMap();
        view.currentMapIndex--;
        GamePlugin.BlockInput(false);
    }
    public void OnNextButtonClicked()
    {
        if (view.currentMapIndex == MapConstant.TOTAL_MAPS - 1) return;
        GamePlugin.BlockInput(true);
        var cameraController = GameManager.Instance.CameraController;
        cameraController.MoveNextMap();
        view.currentMapIndex++;
        GamePlugin.BlockInput(false);
    }
}
