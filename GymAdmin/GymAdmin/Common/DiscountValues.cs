using GymAdmin.Enums;

namespace GymAdmin.Common
{
    public static class DiscountValues
    {
        public static double GetDiscountValue(PlanType plan)
        {
            if (plan == PlanType.Black)
            {
                return 1;
            }
            else if(plan == PlanType.Regular)
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
