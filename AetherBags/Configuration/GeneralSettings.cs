using System.Numerics;
using KamiToolKit.Classes;

namespace AetherBags.Configuration;

public class GeneralSettings
{
    public InventoryStackMode StackMode { get; set; } = InventoryStackMode.AggregateByItemId;
    public bool DebugEnabled { get; set; } = false;
}

public enum InventoryStackMode : byte
{
    NaturalStacks = 0,
    AggregateByItemId = 1,
}