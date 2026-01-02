namespace AetherBags.Addons;

public interface IInventoryWindow
{
    bool IsOpen { get; }
    void Toggle();
    void Close();
    void ManualRefresh();
    void SetSearchText(string searchText);
}