using UnityEngine;
public enum BuildingState
{
    UNDER_CONSTRUCTION,
    READY,
}

public class BuildableEntity : Entity, Buildable
{
    private BuildingState buildingState = BuildingState.READY;
    public BuildingState BuildingState => buildingState;
    public void SetBuildingState(BuildingState newState)
    {
        buildingState = newState;
    }
    public void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public void Place(Vector3 position)
    {
        throw new System.NotImplementedException();
    }
    public void Destroy()
    {
        throw new System.NotImplementedException();
    }
}
