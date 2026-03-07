public class JobFactory
{
    private FoodDictionary foodDictionary;
    private FoodOrderManager foodOrderManager;
    private TableOrderManager tableOrderManager;
    
    public JobFactory(FoodDictionary foodDictionary,  FoodOrderManager foodOrderManager, TableOrderManager tableOrderManager)
    {
        this.foodDictionary = foodDictionary;
        this.foodOrderManager = foodOrderManager;
        this.tableOrderManager = tableOrderManager;
    }

    public Chef CreateChef(float workDuration)
    {
        return new Chef(workDuration, foodDictionary, foodOrderManager);
    }

    public Farmer CreateFarmer(float workDuration)
    {
        return new Farmer(workDuration);
    }

    public Diner CreateDiner()
    {
        return new Diner(foodOrderManager, tableOrderManager, this);
    }

    public Pedestrian CreatePedestrian()
    {
        return new Pedestrian(this);
    }

    public Server CreateServer(float workDuration)
    {
        return new Server(workDuration, foodOrderManager);
    }
}