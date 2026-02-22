using UnityEngine;
using TMPro;
public class ChatPanel : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private TMP_Text chatText;


    public void ShowChat(string text)
    {
        chatText.text = text;
        canvasGroup.alpha = 1f;
    }
    public void HideChat()
    {
        canvasGroup.alpha = 0f;
    }
}
