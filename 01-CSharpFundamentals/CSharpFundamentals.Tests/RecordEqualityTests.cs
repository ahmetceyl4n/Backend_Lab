using DotnetBackendLab.CSharpFundamentals.Examples;

namespace DotnetBackendLab.CSharpFundamentals.Tests;

public class RecordEqualityTests
{
    // Aynı verili iki record eşittir (== ve Equals) ama ayrı nesnelerdir.
    [Fact]
    public void Records_WithSameValues_AreEqual()
    {
        var r1 = new LabelRecord("TR100", 5);
        var r2 = new LabelRecord("TR100", 5);

        Assert.True(r1 == r2);
        Assert.True(r1.Equals(r2));
        Assert.Equal(r1.GetHashCode(), r2.GetHashCode());
        Assert.False(ReferenceEquals(r1, r2));
    }

    // Aynı verili iki class nesnesi referans karşılaştırması yüzünden eşit değildir.
    [Fact]
    public void Classes_WithSameValues_AreNotEqual()
    {
        var c1 = new LabelClass("TR100", 5);
        var c2 = new LabelClass("TR100", 5);

        Assert.False(c1 == c2);
        Assert.False(c1.Equals(c2));
    }

    // struct Equals alanları karşılaştırır.
    [Fact]
    public void Structs_WithSameValues_AreEqual()
    {
        Assert.True(new LabelStruct("TR100", 5).Equals(new LabelStruct("TR100", 5)));
    }

    // with yeni bir nesne üretir; orijinal değişmez.
    [Fact]
    public void With_CreatesNewRecord_AndLeavesOriginalUnchanged()
    {
        var r1 = new LabelRecord("TR100", 5);

        var r3 = r1 with { Weight = 8 };

        Assert.Equal(5, r1.Weight);
        Assert.Equal(8, r3.Weight);
        Assert.Equal("TR100", r3.TrackingNo);
        Assert.NotEqual(r1, r3);
    }
}
