namespace GymAdmin.Common
{
    public static class DiscountValues
    {
        public static double GetDiscountValue(string plan)
        {
            if (plan == "Black")
            {
                return 1;
            }
            else if(plan == "Regular")
            {
                return 0.5;
            }
            else
            {
                return 0;
            }
        }
    }
}
