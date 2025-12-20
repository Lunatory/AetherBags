using System;
using System.Collections.Generic;
using System.Numerics;
using AetherBags.Extensions;
using AetherBags.Inventory;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace AetherBags.Nodes;

public class InventoryCategoryNode : SimpleComponentNode
{
    private readonly TextNode _categoryNameTextNode;
    private readonly HybridDirectionalFlexNode<DragDropNode> _itemGridNode;

    private const float ItemSize = 40;
    private const float ItemHorizontalPadding = 6;
    private const float ItemVerticalPadding = 6;
    private const float HeaderHeight = 16;
    private const float MinWidth = 40;

    private float?  _fixedWidth = null;

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
            Position = new Vector2(0, HeaderHeight),
            Size = new Vector2(240, 100),
            FillRowsFirst = true,
            ItemsPerLine = 10,
            HorizontalPadding = ItemHorizontalPadding,
            VerticalPadding = ItemVerticalPadding,
        };
        _itemGridNode.AttachNode(this);
    }

    public required CategorizedInventory CategorizedInventory
    {
        get;
        set
        {
            field = value;

            _categoryNameTextNode.String = value.Category.Name;
            _categoryNameTextNode.TextColor = value.Category.Color;
            _categoryNameTextNode.TooltipString = value.Category.Description;

            UpdateItemGrid();
            RecalculateSize();
        }
    }

    public void SetItemsPerLine(int itemsPerLine)
    {
        _itemGridNode.ItemsPerLine = itemsPerLine;
        RecalculateSize();
    }

    public void SetFixedWidth(float width)
    {
        _fixedWidth = width;
        RecalculateSize();
    }

    private void RecalculateSize()
    {
        int itemCount = CategorizedInventory.Items.Count;
        if (itemCount == 0)
        {
            float width = _fixedWidth ?? MinWidth;
            Size = new Vector2(width, HeaderHeight);
            _categoryNameTextNode.Size = _categoryNameTextNode.Size with { X = width };
            return;
        }

        int itemsPerLine = Math.Max(1, _itemGridNode.ItemsPerLine);
        int rows = (int)Math.Ceiling((float)itemCount / itemsPerLine);

        float calculatedWidth;
        if (_fixedWidth. HasValue)
        {
            calculatedWidth = _fixedWidth.Value;
        }
        else
        {
            int actualColumns = Math.Min(itemCount, itemsPerLine);
            calculatedWidth = actualColumns * ItemSize + (actualColumns - 1) * ItemHorizontalPadding;
            calculatedWidth = Math.Max(calculatedWidth, MinWidth);
        }

        float height = HeaderHeight + rows * ItemSize + (rows - 1) * ItemVerticalPadding;

        Size = new Vector2(calculatedWidth, height);
        _itemGridNode.Size = new Vector2(calculatedWidth, height - HeaderHeight);
        _categoryNameTextNode.Size = _categoryNameTextNode.Size with { X = calculatedWidth };
    }

    private bool UpdateItemGrid()
    {
        var listUpdated = _itemGridNode.SyncWithListData(CategorizedInventory.Items, node => node.ItemInfo, CreateInventoryDragDropNode);
        return listUpdated;
    }

    private InventoryDragDropNode CreateInventoryDragDropNode(ItemInfo data)
    {
        InventoryItem item = data.Item;
        InventoryDragDropNode node = new InventoryDragDropNode
        {
            Size = new Vector2(40),
            IsVisible = true,
            IconId = item.IconId,
            AcceptedType = DragDropType.Nothing,
            IsDraggable = false,
            Payload = new DragDropPayload
            {
                Type = DragDropType.Item,
                Int1 = (int)item.Container,
                Int2 = (int)item.ItemId,
            },
            IsClickable = true,
            OnRollOver = node => node.ShowInventoryItemTooltip(item.Container, item.Slot),
            OnRollOut = node => node.HideTooltip(),
            ItemInfo = data
        };
        return node;
    }
}