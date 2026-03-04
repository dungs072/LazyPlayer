using System.Threading;
using Building = EntityConstant.Building;
using Cysharp.Threading.Tasks;

public class Server : BaseWorker
{
    private float workDuration = 2f;
    private bool isWorking = false;
    public Server(float workDuration) : base()
    {
        this.workDuration = workDuration;
        FoodOrderManager.OnFoodOrderAdded += HandleFoodOrderAdded;

    }
    ~Server()
    {
        FoodOrderManager.OnFoodOrderAdded -= HandleFoodOrderAdded;
    }
    public override string JobName()
    {
        return "Server";
    }

    private void HandleFoodOrderAdded()
    {
        if (isWorking) return;
        doable.DoJobAsync(DoJobAsync);
    }


    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var entityManager = GameManager.Instance.GamePlay.EntityManager;
        var resourcesManager = GameManager.Instance.GamePlay.ResourcesManager;
        var foodOrderManager = GameManager.Instance.GamePlay.FoodOrderManager;
        var servingTable = entityManager.GetActiveEntity(Building.SERVING_TABLE);
        var orderTable = entityManager.GetActiveEntity(Building.ORDER_TABLE);
        var order = foodOrderManager.GetOldestFoodOrder();
        if (order == null) return;
        //! race conditions
        isWorking = true;
        if (resourcesManager.IsAvailableFood(order.foodAmounts))
        {
            foodOrderManager.RemoveFoodOrder();
            await movement.Move(cancellationToken, servingTable.transform.position);
            if (cancellationToken.IsCancellationRequested) return; 
            
            resourcesManager.ConsumeFood(order.foodAmounts);
            await movement.Move(cancellationToken, order.diningTable.transform.position);
            if (cancellationToken.IsCancellationRequested) return; 
            
            order.diner.EatFood();
            var orderLeft = foodOrderManager.GetOldestFoodOrder();
            if (orderLeft != null)
            {
                isWorking = false;
                HandleFoodOrderAdded();
            }
            else
            {
                await movement.Move(cancellationToken, orderTable.transform.position);
                isWorking = false;
            }
        }
        else
        {
            await movement.Move(cancellationToken, orderTable.transform.position);
            if (cancellationToken.IsCancellationRequested) return; 
            
            var orderLeft = foodOrderManager.GetOldestFoodOrder();
            if (orderLeft != null)
            {
                HandleFoodOrderAdded();
            }
            isWorking = false;

        }

    }

}
