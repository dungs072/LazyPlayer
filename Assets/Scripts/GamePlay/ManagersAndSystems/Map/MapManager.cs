using UnityEngine;

public class MapManager : MonoBehaviour
{
    private EntityManager entityManager;
    private CharacterManager characterManager;
    private StaffManager staffManager;

    public void Initialize1(
        EntityManager entityManager,
        CharacterManager characterManager,
        StaffManager staffManager
    )
    {
        this.entityManager = entityManager;
        this.characterManager = characterManager;
        this.staffManager = staffManager;
    }

    public void Initialize2()
    {
        CreateFarmMap();
        // CreateKitchenMap();
        // CreateDiningRoom();
        // CreateOutsideMap();
    }

    private void CreateFarmMap()
    {
        SpawnFarm();
        SpawnFarmStorage();
        var chars = characterManager.SpawnCharacter(1, new Vector3(0, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i].EnqueueJob(new Farmer(0.2f));
            chars[i].StartJob();
            staffManager.AddStaffAndSetData(chars[i]);
        }
    }

    private void SpawnFarm()
    {
        var xOffset = 2;
        var yOffset = 2;
        var row = 0;
        var basePos = new Vector3(-3, -3, 0);
        for (int i = 0; i < 6; i++)
        {
            var position = new Vector3(basePos.x + i * xOffset, basePos.y + row * yOffset, 0);
            var gridPosition = QueryBus.Query(new GetSnapGridPositionQuery { position = position });
            var plot = entityManager.Acquire<Plot>(
                EntityId.PLOT,
                new EntityConfig { position = gridPosition }
            );
            var buildableEntity = plot as BuildableEntity;
            EventBus.Publish(
                new SetOccupiedGridEvent
                {
                    position = gridPosition,
                    size = buildableEntity.Size,
                    entityInstanceId = plot.InstanceId,
                }
            );
            row = (i + 1) % 2;
        }
    }

    private void SpawnFarmStorage()
    {
        var gridPosition = QueryBus.Query(
            new GetSnapGridPositionQuery { position = new Vector3(-5, 2, 0) }
        );
        var storage = entityManager.Acquire(
            EntityId.FARM_STORAGE,
            new EntityConfig { position = gridPosition }
        );
    }

    private void CreateKitchenMap()
    {
        var chars = characterManager.SpawnCharacter(1, new Vector3(18, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i].EnqueueJob(new Chef(5f));
            chars[i].StartJob();
            staffManager.AddStaffAndSetData(chars[i]);
        }
        var kitchen = entityManager.Acquire(
            EntityId.KITCHEN,
            new EntityConfig { position = new Vector3(18, 2.5f, 0) }
        );
        var servingTable = entityManager.Acquire(
            EntityId.SERVING_TABLE,
            new EntityConfig { position = new Vector3(28, -2, 0) }
        );
    }

    private void CreateDiningRoom()
    {
        var chars = characterManager.SpawnCharacter(1, new Vector3(40, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i].EnqueueJob(new Server());
            chars[i].StartJob();
            staffManager.AddStaffAndSetData(chars[i]);
        }
        var diningTable = entityManager.Acquire(
            EntityId.DINING_TABLE,
            new EntityConfig { position = new Vector3(38, 2.5f, 0) }
        );
        var diningTable2 = entityManager.Acquire(
            EntityId.DINING_TABLE,
            new EntityConfig { position = new Vector3(35, -2.5f, 0) }
        );
        var diningTable3 = entityManager.Acquire(
            EntityId.DINING_TABLE,
            new EntityConfig { position = new Vector3(40, -2.5f, 0) }
        );
        var orderTable = entityManager.Acquire(
            EntityId.ORDER_TABLE,
            new EntityConfig { position = new Vector3(45, -2, 0) }
        );
    }

    private void CreateOutsideMap()
    {
        var chars = characterManager.SpawnCharacter(10, new Vector3(60, 0, 0));
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i].EnqueueJob(new Pedestrian());
            chars[i].StartJob();
        }
    }
}
