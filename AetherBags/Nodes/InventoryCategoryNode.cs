using System;
using System.Collections.Generic;
using System.Numerics;
using AetherBags.Extensions;
using AetherBags.Inventory;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace AetherBags.Nodes;

public class InventoryCategoryNode : SimpleComponentNode
{
    private readonly TextNode _categoryNameTextNode;
    private readonly HybridDirectionalFlexNode<DragDropNode> _itemGridNode;
    public InventoryCategoryNode()
    {
        _categoryNameTextNode = new TextNode
        {
            Size = new Vector2(100, 14),
            AlignmentType = AlignmentType.Left
        };
        _categoryNameTextNode.AttachNode(this);

        _itemGridNode = new HybridDirectionalFlexNode<DragDropNode>
        {
            Position = new Vector2(0, 16),
            Size = new Vector2(240, 100),
            FillRowsFirst = true,
            ItemsPerLine = 10,
            HorizontalPadding = 6,
            VerticalPadding = 6,
        };
        _itemGridNode.AttachNode(this);
    }

    public required CategoryInfo Category
    {
        get;
        set
        {
            field = value;

            _categoryNameTextNode.String = value.Name;
            _categoryNameTextNode.TextColor = value.Color;
            _categoryNameTextNode.TooltipString = value.Description;
        }
    }

    public required List<ItemInfo> Items
    {
        get;
        set
        {
            field = value;

            UpdateItemGrid();
        }
    }

    private bool UpdateItemGrid()
    {
        var listUpdated = _itemGridNode.SyncWithListData(Items, node => node.ItemInfo, data => CreateInventoryDragDropNode(data));
        return listUpdated;
    }

    private unsafe InventoryDragDropNode CreateInventoryDragDropNode(ItemInfo data)
    {
        InventoryDragDropNode node = new InventoryDragDropNode
        {
            Size = new Vector2(40),
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
            ItemInfo = data
        };
        return node;
    }
}