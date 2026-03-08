public class JobFactory
{
    private TableOrderManager tableOrderManager;
    
    public JobFactory(TableOrderManager tableOrderManager)
    {
        this.tableOrderManager = tableOrderManager;
    }

    public Chef CreateChef(float workDuration)
    {
        return new Chef(workDuration);
    }

    public Farmer CreateFarmer(float workDuration)
    {
        return new Farmer(workDuration);
    }

    public Diner CreateDiner()
    {
        return new Diner(tableOrderManager, this);
    }

    public Pedestrian CreatePedestrian()
    {
        return new Pedestrian(this);
    }

    public Server CreateServer(float workDuration)
    {
        return new Server(workDuration);
    }
}