
#region Button Clicked Events
public struct PreButtonClickedEvent { }
public struct NextButtonClickedEvent { }
#endregion

public struct ResourceAmountChangedEvent
{
    public string name;
    public int amount;
}
