using AetherBags.Inventory.Scanning;
using FFXIVClientStructs.FFXIV.Client.Game;

namespace AetherBags.Inventory.State;

public class SaddleBagState : InventoryStateBase
{
    public override InventorySourceType SourceType => InventorySourceType.SaddleBag;
    public override InventoryType[] Inventories => InventorySourceDefinitions.SaddleBag;
}