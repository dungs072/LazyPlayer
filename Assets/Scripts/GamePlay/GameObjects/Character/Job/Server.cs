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
        character.DoJobAsync(DoJobAsync);
    }


    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var servingTable = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.SERVING_TABLE));
        var orderTable = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.ORDER_TABLE));
        var order = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
        if (order == null) return;
        //! race conditions
        isWorking = true;
        var isAvailableFood = QueryBus.Query<IsAvailableFoodQuery, bool>(new IsAvailableFoodQuery(order.foodAmounts));
        if (isAvailableFood)
        {
            EventBus.Publish(new RemoveFoodOrderEvent());
            await movement.Move(cancellationToken, servingTable.transform.position);
               
            EventBus.Publish(new ConsumeFoodEvent(order.foodAmounts));
            await movement.Move(cancellationToken, order.diningTable.transform.position);
               
            order.diner.EatFood();
            var orderLeft = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
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
               
            var orderLeft = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
            if (orderLeft != null)
            {
                HandleFoodOrderAdded();
            }
            isWorking = false;
        }
    }
}
