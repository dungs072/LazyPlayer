using BaseEngine;
using UnityEngine;
using UnityEngine.UI;

public class Tab2ButtonsPanel : MonoBehaviour
{
    [SerializeField]
    private MagicButtonWithIcon[] tab2Buttons;

    [SerializeField]
    Tab2ButtonData data;
    private MagicButtonWithIcon selectedButton;
    private ButtonTab1Type tab1Type;

    void Awake()
    {
        for (int i = 0; i < tab2Buttons.Length; i++)
        {
            int index = i;

            var button = tab2Buttons[i];
            button.AddListener(() =>
            {
                HandleButtonClicked(index);
            });
        }
    }

    private void Start()
    {
        SetSelectedTabButton(null);
    }

    void OnDestroy()
    {
        for (int i = 0; i < tab2Buttons.Length; i++)
        {
            var button = tab2Buttons[i];
            button.RemoveAllListeners();
        }
    }

    public void SetTab2Buttons(ButtonTab1Type tab1Type)
    {
        this.tab1Type = tab1Type;
        var tab2ButtonsInfo = data.GetTab2ButtonsInfo(tab1Type);
        SetSelectedTabButton(null);
        if (tab2ButtonsInfo == null)
        {
            Debug.LogError($"No tab2 buttons info found for tab1 type {tab1Type}");
            return;
        }
        for (int i = 0; i < tab2Buttons.Length; i++)
        {
            var button = tab2Buttons[i];
            if (i >= tab2ButtonsInfo.icon.Count)
            {
                button.SetIcon(data.LockSprite);
                continue;
            }
            button.SetIcon(tab2ButtonsInfo.icon[i].icon);
        }
    }

    private void SetSelectedTabButton(MagicButtonWithIcon button)
    {
        if (selectedButton != null)
        {
            var prevImage = selectedButton.GetComponent<Image>();
            prevImage.color = Color.white;
        }
        selectedButton = button;
        if (button == null)
            return;
        var image = button.GetComponent<Image>();

        image.color = Color.gray;
    }

    private void HandleButtonClicked(int index)
    {
        var tab2ButtonsInfo = data.GetTab2ButtonsInfo(tab1Type);
        if (tab2ButtonsInfo == null)
        {
            Debug.LogError($"No tab2 buttons info found for tab1 type {tab1Type}");
            return;
        }
        var button = tab2Buttons[index];
        if (index >= tab2ButtonsInfo.icon.Count)
        {
            return;
        }
        SetSelectedTabButton(button);
    }
}
