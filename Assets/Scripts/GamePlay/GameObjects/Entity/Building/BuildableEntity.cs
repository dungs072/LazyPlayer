using UnityEngine;

public enum BuildingState
{
    UNDER_CONSTRUCTION,
    READY,
}

public class BuildableEntity : Entity, Buildable
{
    [SerializeField]
    protected SpriteRenderer skinSpriteRenderer;
    private BuildingState buildingState = BuildingState.READY;
    public BuildingState BuildingState => buildingState;

    public Sprite Skin => skinSpriteRenderer.sprite;
    public Vector2 Size =>
        new Vector2(
            Skin.bounds.size.x * skinSpriteRenderer.transform.localScale.x,
            Skin.bounds.size.y * skinSpriteRenderer.transform.localScale.y
        );
    public Vector2 DisplaySize => skinSpriteRenderer.transform.localScale;

    protected override void Awake()
    {
        base.Awake();
        // if (skinSpriteRenderer == null)
        // {
        //     Debug.LogError("Skin SpriteRenderer is not assigned!");
        // }
    }

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
