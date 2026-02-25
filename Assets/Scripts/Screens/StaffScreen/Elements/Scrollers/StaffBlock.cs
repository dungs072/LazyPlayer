using PolyAndCode.UI;
using UnityEngine;
using TMPro;
public class StaffBlock : MonoBehaviour, ICell
{
    [SerializeField] private TMP_Text indexText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text ageText;
    [SerializeField] private TMP_Text sexText;
    [SerializeField] private TMP_Text expText;



    public void SetInfo(CharacterData data, int index)
    {
        SetIndex(index.ToString());
        SetName(data.Name);
        SetAge(data.Age.ToString());
        SetSex(data.ToSexText());
        SetExp(data.Experience.ToString());
    }

    public void SetIndex(string index)
    {
        indexText.text = index;
    }


    public void SetName(string name)
    {
        nameText.text = name;
    }
    public void SetAge(string age)
    {
        ageText.text = "Age: " + age;
    }
    public void SetSex(string sex)
    {
        sexText.text = "Sex:" + sex;
    }
    public void SetExp(string exp)
    {
        expText.text = "Exp: " + exp;
    }
}
