using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class ParameterPassingTests
{
    // int değerle geçilir: içerideki atama dışarıyı etkilemez.
    [Fact]
    public void IntByValue_DoesNotChangeCaller()
    {
        int price = 100;
        Example02_ParameterPassing.ApplyDiscount(price);
        Assert.Equal(100, price);
    }

    // ref int: çağıranın değişkeni değişir.
    [Fact]
    public void IntByRef_ChangesCaller()
    {
        int price = 100;
        Example02_ParameterPassing.ApplyDiscountByRef(ref price);
        Assert.Equal(50, price);
    }

    // class geçildiğinde alan değişikliği aynı nesneye yapılır.
    [Fact]
    public void ClassFieldChange_IsVisibleToCaller()
    {
        var parcel = new Parcel("TR100", 5);
        Example02_ParameterPassing.ChangeWeight(parcel);
        Assert.Equal(99, parcel.Weight);
    }

    // Parametreye new atamak çağıranın değişkenini yönlendirmez.
    [Fact]
    public void AssigningNewToParameter_DoesNotChangeCaller()
    {
        var parcel = new Parcel("TR100", 5);
        var before = parcel;
        Example02_ParameterPassing.ReplaceParcel(parcel);
        Assert.Same(before, parcel);
        Assert.Equal("TR100", parcel.TrackingNo);
    }

    // ref ile new atamak çağıranın değişkenini yeni nesneye yönlendirir.
    [Fact]
    public void AssigningNewToRefParameter_ChangesCaller()
    {
        var parcel = new Parcel("TR100", 5);
        var before = parcel;
        Example02_ParameterPassing.ReplaceParcelByRef(ref parcel);
        Assert.NotSame(before, parcel);
        Assert.Equal("YENI", parcel.TrackingNo);
    }
}
