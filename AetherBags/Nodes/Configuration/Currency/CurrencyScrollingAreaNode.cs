using KamiToolKit.Nodes;

namespace AetherBags.Nodes.Configuration.Currency;

public sealed class CurrencyScrollingAreaNode : ScrollingListNode
{
    public CurrencyScrollingAreaNode()
    {
        AddNode(new CurrencyGeneralConfigurationNode
        {
            Size = Size
        });
    }
}