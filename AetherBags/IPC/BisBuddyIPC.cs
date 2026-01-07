using System;
using System.Collections.Generic;
using System.Numerics;
using AetherBags.Inventory;
using AetherBags.Inventory.Context;
using Dalamud.Plugin.Ipc;

namespace AetherBags.IPC;

public class BisBuddyIPC : IDisposable
{
    private ICallGateSubscriber<bool>? _isInitialized;
    private ICallGateSubscriber<bool, bool>? _initialized;
    private ICallGateSubscriber<List<uint>>? _getBisItems;
    private ICallGateSubscriber<List<uint>, bool>? _bisItemsChanged;

    public bool IsReady { get; private set; }
    private static readonly Vector3 BisColor = new(0.0f, 0.3f, 0.0f);

    public BisBuddyIPC()
    {
        try
        {
            _isInitialized = Services.PluginInterface.GetIpcSubscriber<bool>("BisBuddy.IsInitialized");
            _initialized = Services.PluginInterface.GetIpcSubscriber<bool, bool>("BisBuddy.Initialized");
            _getBisItems = Services.PluginInterface.GetIpcSubscriber<List<uint>>("BisBuddy.GetBisItems");
            _bisItemsChanged = Services.PluginInterface.GetIpcSubscriber<List<uint>, bool>("BisBuddy.BisItemsChanged");

            _initialized.Subscribe(OnInitialized);
            _bisItemsChanged.Subscribe(UpdateHighlights);

            try { IsReady = _isInitialized.InvokeFunc(); } catch { IsReady = false; }

            if (IsReady) RequestUpdate();
        }
        catch (Exception ex)
        {
            Services.Logger.DebugOnly($"BisBuddy not available: {ex.Message}");
        }
    }

    private void OnInitialized(bool ready)
    {
        IsReady = ready;
        if (ready) RequestUpdate();
        else HighlightState.ClearLabel(HighlightSource.BiSBuddy);
    }

    public void RequestUpdate()
    {
        if (!IsReady) return;
        try
        {
            var items = _getBisItems?.InvokeFunc();
            if (items != null) UpdateHighlights(items);
        }
        catch { IsReady = false; }
    }

    private void UpdateHighlights(List<uint>? itemIds)
    {
        if (!System.Config.Categories.BisBuddyEnabled || itemIds == null || itemIds.Count == 0)
        {
            HighlightState.ClearLabel(HighlightSource.BiSBuddy);
        }
        else
        {
            HighlightState.SetLabel(HighlightSource.BiSBuddy, itemIds, BisColor);
        }

        InventoryOrchestrator.RefreshHighlights();
    }

    public void Dispose()
    {
        _initialized?.Unsubscribe(OnInitialized);
        _bisItemsChanged?.Unsubscribe(UpdateHighlights);
    }
}