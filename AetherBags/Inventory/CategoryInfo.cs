using System.Numerics;
using KamiToolKit.Classes;

namespace AetherBags.Inventory;

public class CategoryInfo
{
    public required string Name { get; set; }
    public Vector4 Color { get; set; } = ColorHelper.GetColor(50);
    public string Description { get; set; } = string.Empty;
}