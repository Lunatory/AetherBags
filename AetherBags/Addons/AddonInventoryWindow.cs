using System;
using System.Collections.Generic;
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
    private WrappingGridNode<InventoryCategoryNode> _categoriesNode;

    // Window constraints
    private const float MinWindowWidth = 300;
    private const float MaxWindowWidth = 800;
    private const float MinWindowHeight = 200;
    private const float MaxWindowHeight = 1000;

    // Layout settings
    private const float CategorySpacing = 10;
    private const float ItemSize = 40;
    private const float ItemPadding = 6;

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        Services.AddonLifecycle.RegisterListener(AddonEvent.PostRequestedUpdate, "Inventory", OnInventoryUpdate);
        addon->SubscribeAtkArrayData(1, (int)NumberArrayType.Inventory);
        _categoriesNode = new WrappingGridNode<InventoryCategoryNode>
        {
            Position = ContentStartPosition,
            Size = ContentSize,
            HorizontalSpacing = CategorySpacing,
            VerticalSpacing = CategorySpacing
        };
        _categoriesNode.AttachNode(this);

        RefreshCategories();
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {

    }

    private void OnInventoryUpdate(AddonEvent type, AddonArgs args)
    {
        RefreshCategories();
    }

    protected override unsafe void OnRequestedUpdate(AtkUnitBase* addon, NumberArrayData** numberArrayData, StringArrayData** stringArrayData) {
        RefreshCategories();
    }

    private void RefreshCategories()
    {
        var categories = InventoryState.GetInventoryItemCategories();

        float maxContentWidth = MaxWindowWidth - (ContentStartPosition.X * 2);
        int maxItemsPerLine = CalculateOptimalItemsPerLine(maxContentWidth);

        _categoriesNode.SyncWithListData(
            categories,
            node => node.CategorizedInventory,
            data =>
            {
                var node = new InventoryCategoryNode
                {
                    Size = ContentSize with { Y = 120 },
                    CategorizedInventory = data
                };

                UpdateItemsPerLine(node, maxItemsPerLine);
                return node;
            });

        foreach (InventoryCategoryNode node in _categoriesNode.GetNodes<InventoryCategoryNode>())
        {
            UpdateItemsPerLine(node, maxItemsPerLine);
        }

        AutoSizeWindow();
    }

    private static void UpdateItemsPerLine(InventoryCategoryNode node, int maxItemsPerLine)
    {
        int itemCount = node.CategorizedInventory.Items.Count;
        int itemsPerLine = Math.Min(itemCount, maxItemsPerLine);
        node.SetItemsPerLine(itemsPerLine);
    }

    private int CalculateOptimalItemsPerLine(float availableWidth)
    {
        float itemWithPadding = ItemSize + ItemPadding;
        int maxItems = (int)Math.Floor((availableWidth + ItemPadding) / itemWithPadding);

        return Math.Clamp(maxItems, 1, 15);
    }

    private void AutoSizeWindow()
    {
        List<InventoryCategoryNode> childNodes = _categoriesNode.GetNodes<InventoryCategoryNode>().ToList();
        if (childNodes.Count == 0)
        {
            ResizeWindow(MinWindowWidth, MinWindowHeight);
            return;
        }

        float requiredWidth = childNodes.Max(node => node. Width);
        requiredWidth += ContentStartPosition.X * 2;
        float finalWidth = Math.Clamp(requiredWidth, MinWindowWidth, MaxWindowWidth);

        float contentWidth = finalWidth - (ContentStartPosition.X * 2);
        _categoriesNode.Size = new Vector2(contentWidth, MaxWindowHeight);

        _categoriesNode.RecalculateLayout();

        float requiredHeight = _categoriesNode.GetRequiredHeight();
        requiredHeight += ContentStartPosition.Y + ContentStartPosition.X;

        float finalHeight = Math.Clamp(requiredHeight, MinWindowHeight, MaxWindowHeight);

        ResizeWindow(finalWidth, finalHeight);
    }

    private void ResizeWindow(float width, float height)
    {
        SetWindowSize(width, height);
        _categoriesNode.Size = ContentSize;
        _categoriesNode.RecalculateLayout();
    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        Services.AddonLifecycle.UnregisterListener(OnInventoryUpdate);
        addon->UnsubscribeAtkArrayData(1, (int)NumberArrayType.Inventory);
    }
}