public class JobFactory
{
    private EntityManager entityManager;
    private ResourcesManager resourcesManager;
    private FoodDictionary foodDictionary;
    private FoodOrderManager foodOrderManager;
    private TableOrderManager tableOrderManager;
    
    public JobFactory(EntityManager entityManager, ResourcesManager resourcesManager, FoodDictionary foodDictionary,  FoodOrderManager foodOrderManager, TableOrderManager tableOrderManager)
    {
        this.entityManager = entityManager;
        this.resourcesManager = resourcesManager;
        this.foodDictionary = foodDictionary;
        this.foodOrderManager = foodOrderManager;
        this.tableOrderManager = tableOrderManager;
    }

    public Chef CreateChef(float workDuration)
    {
        return new Chef(workDuration, entityManager, resourcesManager, foodDictionary, foodOrderManager);
    }

    public Farmer CreateFarmer(float workDuration)
    {
        return new Farmer(workDuration, entityManager, resourcesManager);
    }

    public Diner CreateDiner()
    {
        return new Diner(entityManager, resourcesManager, foodOrderManager, tableOrderManager, this);
    }

    public Pedestrian CreatePedestrian()
    {
        return new Pedestrian(this);
    }

    public Server CreateServer(float workDuration)
    {
        return new Server(workDuration, entityManager, resourcesManager, foodOrderManager);
    }
}