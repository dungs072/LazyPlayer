using System.Threading;
using Building = EntityConstant.Building;
using Cysharp.Threading.Tasks;

public class Server : BaseWorker
{
    //private bool isWorking = false;
    public Server() : base()
    {
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
        //TODO: only activate server based on event?
        //if (isWorking) return;
        //character.DoJobAsync(DoJobAsync);
    }


    public override async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        var order = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
        if (order == null)
        {
            character.EnqueueJob(this);
            return;
        }
        
        var servingTable = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.SERVING_TABLE));
        var orderTable = QueryBus.Query<GetActiveEntityQuery, Entity>(new GetActiveEntityQuery(Building.ORDER_TABLE));
        //! race conditions
        //isWorking = true;
        var isAvailableFood = QueryBus.Query<IsAvailableFoodQuery, bool>(new IsAvailableFoodQuery(order.foodAmounts));
        if (isAvailableFood)
        {
            EventBus.Publish(new RemoveFoodOrderEvent());
            await character.MovementComponent.Move(cancellationToken, servingTable.transform.position);
               
            EventBus.Publish(new ConsumeFoodEvent(order.foodAmounts));
            await character.MovementComponent.Move(cancellationToken, order.diningTable.transform.position);
               
            //TODO: eat duration based on what?
            order.diner.EnqueueJob(new DinerEatFood(order.diningTable, 5f));
            var orderLeft = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
            if (orderLeft != null)
            {
                //isWorking = false;
                HandleFoodOrderAdded();
            }
            else
            {
                await character.MovementComponent.Move(cancellationToken, orderTable.transform.position);
                //isWorking = false;
            }
        }
        else
        {
            await character.MovementComponent.Move(cancellationToken, orderTable.transform.position);
               
            var orderLeft = QueryBus.Query<GetOldestFoodOrderQuery, FoodOrder>(new GetOldestFoodOrderQuery());
            if (orderLeft != null)
            {
                HandleFoodOrderAdded();
            }
            //isWorking = false;
        }
        
        //TODO: only activate server based on event?
        character.EnqueueJob(this);
    }
}
