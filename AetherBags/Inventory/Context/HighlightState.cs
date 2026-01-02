using System.Collections.Generic;
using System.Numerics;

namespace AetherBags.Inventory.Context;

public enum HighlightSource
{
    Search,
    AllaganTools,
    BiSBuddy,
}

public static class HighlightState
{
    private static readonly Dictionary<HighlightSource, HashSet<uint>> Filters = new();
    private static readonly Dictionary<HighlightSource, (HashSet<uint> ids, Vector3 color)> Labels = new();

    public static string? SelectedAllaganToolsFilterKey { get; set; } = string.Empty;

    public static bool IsFilterActive => Filters.Count > 0;

    public static void SetFilter(HighlightSource source, IEnumerable<uint> ids)
        => Filters[source] = new HashSet<uint>(ids);

    public static bool IsInActiveFilters(uint itemId)
    {
        if (Filters.Count == 0) return true;
        foreach (var filter in Filters.Values)
            if (filter.Contains(itemId)) return true;
        return false;
    }

    public static Vector3? GetLabelColor(uint itemId)
    {
        foreach (var label in Labels.Values)
            if (label.ids.Contains(itemId)) return label.color;
        return null;
    }

    public static void SetLabel(HighlightSource source, IEnumerable<uint> ids, Vector3 color)
        => Labels[source] = (new HashSet<uint>(ids), color);


    public static void ClearAll()
    {
        Filters.Clear();
        Labels.Clear();
        SelectedAllaganToolsFilterKey = string.Empty;
    }

    public static void ClearFilter(HighlightSource source) => Filters.Remove(source);
    public static void ClearLabel(HighlightSource source) => Labels.Remove(source);
}