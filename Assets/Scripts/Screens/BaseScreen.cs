using System.Collections;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    public IEnumerator OpenAsync()
    {
        yield return null;
        gameObject.SetActive(true);
    }

    public IEnumerator CloseAsync()
    {
        yield return null;
        gameObject.SetActive(false);
    }
}
