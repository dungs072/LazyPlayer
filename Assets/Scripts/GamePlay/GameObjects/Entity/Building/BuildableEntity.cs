using UnityEngine;

public enum BuildingState
{
    UNDER_CONSTRUCTION,
    READY,
}

public class BuildableEntity : Entity
{
    private BuildingState buildingState = BuildingState.READY;
    public BuildingState BuildingState => buildingState;

    public Sprite Skin => spriteRenderer.sprite;
    public Vector2 Size =>
        new Vector2(
            Skin.bounds.size.x * spriteRenderer.transform.localScale.x,
            Skin.bounds.size.y * spriteRenderer.transform.localScale.y
        );
    public Vector2 DisplaySize => spriteRenderer.transform.localScale;

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
