public class JobFactory
{
    private FoodOrderManager foodOrderManager;
    private TableOrderManager tableOrderManager;
    
    public JobFactory(FoodOrderManager foodOrderManager, TableOrderManager tableOrderManager)
    {
        this.foodOrderManager = foodOrderManager;
        this.tableOrderManager = tableOrderManager;
    }

    public Chef CreateChef(float workDuration)
    {
        return new Chef(workDuration, foodOrderManager);
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