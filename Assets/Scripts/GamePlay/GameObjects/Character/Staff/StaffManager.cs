using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StaffManager : MonoBehaviour
{
    private List<Character> staffs = new();


    void Awake()
    {
        QueryBus.Subscribe<GetStaffDataList, IReadOnlyList<CharacterData>>(query => GetStaffDataList());
    }
    public IReadOnlyList<CharacterData> GetStaffDataList()
    {
        return staffs.Select(s => s.CharacterData).ToList().AsReadOnly();
    }
    public void AddStaffAndSetData(Character character)
    {
        staffs.Add(character);
        var data = GenerateStaffData();
        character.SetCharacterData(data);
    }
    public void RemoveStaff(Character character)
    {
        staffs.Remove(character);
    }

    private CharacterData GenerateStaffData()
    {
        var data = new CharacterData("Name", Random.Range(18, 60), Random.Range(0, 100), (Sex)Random.Range(1, 3));
        return data;
    }
}
