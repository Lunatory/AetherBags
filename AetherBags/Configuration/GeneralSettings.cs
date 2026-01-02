namespace AetherBags.Configuration;

public class GeneralSettings
{
    public InventoryStackMode StackMode { get; set; } = InventoryStackMode.AggregateByItemId;
    public SearchMode SearchMode { get; set; } = SearchMode.Highlight;
    public bool DebugEnabled { get; set; } = false;
    public bool CompactPackingEnabled { get; set; } = true;
    public int CompactLookahead { get; set; } = 24;
    public bool CompactPreferLargestFit { get; set; } = true;
    public bool CompactStableInsert { get; set; } = true;
    public bool OpenWithGameInventory { get; set; } = true;
    public bool HideGameInventory { get; set; } = false;
    public bool OpenSaddleBagsWithGameInventory { get; set; } = true;
    public bool HideGameSaddleBags { get; set; } = false;
    public bool OpenRetainerWithGameInventory { get; set; } = true;
    public bool HideGameRetainer { get; set; } = false;
    public bool ShowCategoryItemCount { get; set; } = false;
    public bool LinkItemEnabled { get; set; } = false;
}

public enum InventoryStackMode : byte
{
    NaturalStacks = 0,
    AggregateByItemId = 1,
}

public enum SearchMode : byte
{
    Filter = 0,
    Highlight = 1,
}