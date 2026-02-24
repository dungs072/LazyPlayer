using System.Collections.Generic;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    private List<Character> staffs = new();

    public void AddStaff(Character character)
    {
        staffs.Add(character);
    }
    public void RemoveStaff(Character character)
    {
        staffs.Remove(character);
    }


}
