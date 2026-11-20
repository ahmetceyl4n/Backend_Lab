using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class HashCodeEqualityTests
{
    // Doğru GetHashCode'lu kargo aynı takip numarasıyla ikinci kez eklenmez.
    [Fact]
    public void ShipmentWithHash_IsNotAddedTwice()
    {
        var set = new HashSet<ShipmentWithHash>
        {
            new("TR100"),
            new("TR100"),
        };

        Assert.Single(set);
    }

    // HashSet.Add ilk eklemede true, aynı değerin ikinci eklemesinde false döner.
    [Fact]
    public void HashSetAdd_ReturnsFalse_OnSecondAdd()
    {
        var set = new HashSet<ShipmentWithHash>();

        Assert.True(set.Add(new ShipmentWithHash("TR100")));
        Assert.False(set.Add(new ShipmentWithHash("TR100")));
    }

    // Eşit sayılan nesnelerin hash kodu da eşittir (Equals/GetHashCode sözleşmesi).
    [Fact]
    public void EqualShipments_HaveEqualHashCodes()
    {
        var a = new ShipmentWithHash("TR100");
        var b = new ShipmentWithHash("TR100");

        Assert.Equal(a, b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}
