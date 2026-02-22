using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private List<Character> characterPrefabs = new();

    private List<Character> characters = new();

    public Character[] SpawnCharacter(int amount, Vector3 startWorldPos)
    {
        Character[] spawnedCharacters = new Character[amount];
        for (int i = 0; i < amount; i++)
        {
            spawnedCharacters[i] = CreateOneCharacter(startWorldPos);
        }
        return spawnedCharacters;
    }

    private Character CreateOneCharacter(Vector3 startWorldPos)
    {

        var availableCharacter = characters.FirstOrDefault((c) => !c.gameObject.activeSelf);
        if (availableCharacter)
        {
            availableCharacter.gameObject.SetActive(true);
            availableCharacter.transform.position = startWorldPos;
        }
        else
        {
            var prefab = characterPrefabs[0];
            if (prefab == null) return null;
            var instance = Instantiate(prefab, startWorldPos, Quaternion.identity);
            characters.Add(instance);
            return instance;
        }
        return availableCharacter;
    }

}
