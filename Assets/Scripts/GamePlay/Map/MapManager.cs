using UnityEngine;

public class MapManager : MonoBehaviour
{
    private EntityManager entityManager;
    private CharacterManager characterManager;
    private StaffManager staffManager;
    private JobFactory jobFactory;

    public void Initialize1(EntityManager entityManager, CharacterManager characterManager, StaffManager staffManager, JobFactory jobFactory)
    {
        this.entityManager = entityManager;
        this.characterManager = characterManager;
        this.staffManager = staffManager;
        this.jobFactory = jobFactory;
    }
    
    public void Initialize2()
    {
        CreateFarmMap();
        CreateKitchenMap();
        CreateDiningRoom();
        CreateOutsideMap();
    }
    private void CreateFarmMap()
    {
        SpawnFarm();
        SpawnFarmStorage();
        var chars = characterManager.SpawnCharacter(1, new Vector3(0, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i].SetJob(jobFactory.CreateFarmer(0.2f));
            chars[i].StartJob();
            staffManager.AddStaffAndSetData(chars[i]);
        }

    }
    private void SpawnFarm()
    {
        var xOffset = 1;
        var yOffset = 1;
        var row = 0;
        var basePos = new Vector3(3, -3, 0);
        for (int i = 0; i < 6; i++)
        {
            var position = new Vector3(basePos.x + i * xOffset, basePos.y + row * yOffset, 0);
            var gridPosition = QueryBus.Query<GetSnapGridPositionQuery, Vector3>(new GetSnapGridPositionQuery { position = position });
            var farm = entityManager.GetEntity(EntityConstant.Building.PLOT, gridPosition);
            row = (i + 1) % 2;
        }

    }
    private void SpawnFarmStorage()
    {
        var gridPosition = QueryBus.Query<GetSnapGridPositionQuery, Vector3>(new GetSnapGridPositionQuery { position = new Vector3(-5, 2, 0) });
        var storage = entityManager.GetEntity(EntityConstant.Building.FARM_STORAGE, gridPosition);
    }

    private void CreateKitchenMap()
    {
        var chars = characterManager.SpawnCharacter(1, new Vector3(18, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i].SetJob(jobFactory.CreateChef(5f));
            chars[i].StartJob();
            staffManager.AddStaffAndSetData(chars[i]);
        }
        var kitchen = entityManager.GetEntity(EntityConstant.Building.KITCHEN, new Vector3(18, 2.5f, 0));
        var servingTable = entityManager.GetEntity(EntityConstant.Building.SERVING_TABLE, new Vector3(28, -2, 0));
    }
    private void CreateDiningRoom()
    {
        var chars = characterManager.SpawnCharacter(1, new Vector3(40, 0, 0));
        for (var i = 0; i < chars.Length; i++)
        {
            chars[0].SetIsLoopingDoJob(false);
            chars[i].SetJob(jobFactory.CreateServer(2f));
            staffManager.AddStaffAndSetData(chars[i]);
        }
        var diningTable = entityManager.GetEntity(EntityConstant.Building.DINING_TABLE, new Vector3(38, 2.5f, 0));
        var diningTable2 = entityManager.GetEntity(EntityConstant.Building.DINING_TABLE, new Vector3(35, -2.5f, 0));
        var diningTable3 = entityManager.GetEntity(EntityConstant.Building.DINING_TABLE, new Vector3(40, -2.5f, 0));
        var orderTable = entityManager.GetEntity(EntityConstant.Building.ORDER_TABLE, new Vector3(45, -2, 0));
    }
    private void CreateOutsideMap()
    {
        var chars = characterManager.SpawnCharacter(10, new Vector3(60, 0, 0));
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i].SetIsLoopingDoJob(false);
            chars[i].SetJob(jobFactory.CreatePedestrian());
            chars[i].StartJob();
        }

    }

}
