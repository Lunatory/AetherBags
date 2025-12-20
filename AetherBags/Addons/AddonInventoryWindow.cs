using System.Linq;
using System.Numerics;
using AetherBags.Extensions;
using AetherBags.Inventory;
using AetherBags.Nodes;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;

namespace AetherBags.Addons;

public class AddonInventoryWindow : NativeAddon
{
    private InventoryCategoryNode _categoryNode;
    private InventoryDragDropNode _dragDropNode;
    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "Inventory", OnInventoryUpdate);
        _categoryNode = new InventoryCategoryNode
        {
            Position = ContentStartPosition,
            Size = ContentSize,
            Category = new CategoryInfo
            {
                Name = "AetherBags",
            },
            Items = InventoryState.GetInventoryItems()
        };
        _categoryNode.AttachNode(this);
        /*
        var data = InventoryState.GetInventoryItems().Find(item => item.Name.Contains("Cookie"));


        if (data != null)
        {
            var item = data.Item;
            _dragDropNode = new InventoryDragDropNode
            {
                Size = new Vector2(48),
                IsVisible = true,
                IconId = data.IconId,
                AcceptedType = DragDropType.Nothing,
                IsDraggable = false,
                Payload = new DragDropPayload
                {
                    Type = DragDropType.Item,
                    Int1 = (int)data.Item.Container,
                    Int2 = (int)data.Item.ItemId,
                },
                IsClickable = true,
                OnRollOver = node => node.ShowInventoryItemTooltip(data.Item.Container, data.Item.Slot),
                OnRollOut = node => node.HideTooltip(),
                OnClicked = _ =>
                {

                    AgentInventoryContext* context = AgentInventoryContext.Instance();
                    context->OpenForItemSlot(data.Item.Container, data.Item.Slot, 0, context->AddonId);
                    //item.UseItem();
                },
                ItemInfo = data
            };
            _dragDropNode.AttachNode(this);
        }
        */

    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {

    }

    private void OnInventoryUpdate(AddonEvent type, AddonArgs args)
    {

    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        Services.AddonLifecycle.UnregisterListener(OnInventoryUpdate);
    }
}