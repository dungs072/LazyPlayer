using System;
using UnityEngine;
using TMPro;
public class DiningTable : BuildableEntity
{
    public static event Action OnAvailableDiningTable;
    [SerializeField] private int capacity = 2;
    [SerializeField] private Transform[] seatPositions;
    [SerializeField] private TMP_Text[] tMP_Texts;

    private Transform[] occupiedSeats;
    protected override void Awake()
    {
        base.Awake();
        occupiedSeats = new Transform[capacity];
        for (int i = 0; i < capacity; i++)
        {
            occupiedSeats[i] = null;
            tMP_Texts[i].text = "Available";
        }
    }
    public bool IsAvailable
    {
        get
        {
            for (int i = 0; i < capacity; i++)
            {
                if (occupiedSeats[i] == null)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public Vector3? GetAvailableSeat()
    {
        for (int i = 0; i < capacity; i++)
        {
            if (occupiedSeats[i] == null)
            {
                return seatPositions[i].position;
            }
        }
        return null;
    }
    public void OccupySeat(Transform character)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (occupiedSeats[i] == null)
            {
                occupiedSeats[i] = character;
                tMP_Texts[i].text = "Occupied";
                return;
            }
        }
    }
    public void VacateSeat(Transform character)
    {
        for (int i = 0; i < capacity; i++)
        {
            if (occupiedSeats[i] == character)
            {
                occupiedSeats[i] = null;
                tMP_Texts[i].text = "Available";
                OnAvailableDiningTable?.Invoke();
                return;
            }
        }
    }


}
