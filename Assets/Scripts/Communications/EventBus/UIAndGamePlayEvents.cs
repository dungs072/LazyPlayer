
//! be cautious when using reference types for queries
#region Button Clicked Events
public struct PreButtonClickedEvent { }
public struct NextButtonClickedEvent { }
#endregion

public struct ResourceAmountChangedEvent
{
    public string name;
    public int amount;
}

