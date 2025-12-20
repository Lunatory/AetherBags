using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace AetherBags.Extensions;

public static unsafe class AtkResNodeExtensions
{
    extension(ref AtkResNode node)
    {
        public void ShowInventoryItemTooltip(InventoryType container, short slot) {
            fixed (AtkResNode* nodePointer = &node) {
                AtkStage.Instance()->ShowInventoryItemTooltip(nodePointer, container, slot);
            }
        }
    }
}