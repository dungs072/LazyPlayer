
public static class ResourceConstant
{
    public static class Food
    {
        public enum FoodType
        {
            NONE = 0,
            WHEAT = 1,
            TOMATO = 2,
        }
        public static string GetFoodName(FoodType type)
        {
            return type switch
            {
                FoodType.NONE => "None",
                FoodType.WHEAT => "Wheat",
                FoodType.TOMATO => "Tomato",
                _ => null
            };
        }
    }

    
}