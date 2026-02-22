using System.Collections.Generic;

public static class EntityConstant
{
    public static class Building
    {
        public const string NOTHING = "Nothing";
        public const string PLOT = "Plot";
        public const string FARM_STORAGE = "FarmStorage";
        public const string KITCHEN = "Kitchen";
        public const string SERVING_TABLE = "ServingTable";
        public const string DINING_TABLE = "DiningTable";
        public const string ORDER_TABLE = "OrderTable";

        public static List<string> AllBuildingNames = new()
        {
            NOTHING,
            PLOT,
            FARM_STORAGE,
            KITCHEN,
            SERVING_TABLE,
            DINING_TABLE,
            ORDER_TABLE,
        };

        public enum BuildingType
        {
            NOTHING = 0,
            PLOT = 1,
            FARM_STORAGE = 2,
            KITCHEN = 3,
            SERVING_TABLE = 4,
            DINING_TABLE = 5,
            ORDER_TABLE = 6,

        }

        public static string GetBuildingName(BuildingType type)
        {
            return type switch
            {
                BuildingType.NOTHING => NOTHING,
                BuildingType.PLOT => PLOT,
                BuildingType.FARM_STORAGE => FARM_STORAGE,
                BuildingType.KITCHEN => KITCHEN,
                BuildingType.SERVING_TABLE => SERVING_TABLE,
                BuildingType.DINING_TABLE => DINING_TABLE,
                BuildingType.ORDER_TABLE => ORDER_TABLE,
                _ => null
            };
        }
    }

}
