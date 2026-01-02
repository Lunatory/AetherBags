using System.Collections.Generic;
using AetherBags.Addons;
using AetherBags.Inventory.Context;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace AetherBags.Inventory;

public static unsafe class InventoryOrchestrator
{
    private static readonly InventoryNotificationState NotificationState = new();

    public static void RefreshAll(bool updateMaps = true)
    {
        if (updateMaps)
        {
            InventoryContextState.RefreshMaps();
            InventoryContextState.RefreshBlockedSlots();
        }

        var agent = AgentInventory.Instance();
        var contextId = agent != null ? agent->OpenTitleId : 0;
        var notification = NotificationState.GetNotificationInfo(contextId);

        Services.Framework.RunOnTick(() =>
        {
            if (System.AddonInventoryWindow.IsOpen)
                System.AddonInventoryWindow.SetNotification(notification!);

            foreach (var window in GetAllWindows())
            {
                if (window.IsOpen)
                    window.ManualRefresh();
            }
        });
    }

    public static void CloseAll()
    {
        foreach (var window in GetAllWindows())
        {
            window.Close();
        }
    }

    public static void RefreshHighlights()
    {
        Services.Framework.RunOnTick(() =>
        {
            foreach (var window in GetAllWindows())
            {
                window.ItemRefresh();
            }
        });
    }

    private static IEnumerable<IInventoryWindow> GetAllWindows()
    {
        yield return System.AddonInventoryWindow;
        yield return System.AddonSaddleBagWindow;
        yield return System.AddonRetainerWindow;
    }
}