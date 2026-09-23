using System.Numerics;

public static class BigIntCalculator
{
    public static BigInteger GetCost(CostData cost, int currentLevel)
    {
        return (BigInteger)cost.baseCost
             * BigInteger.Pow(new BigInteger(cost.costMultiplier), currentLevel);
    }
}
