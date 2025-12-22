using AetherBags.Configuration;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using System.Numerics;

namespace AetherBags.Nodes.Configuration.Layout;

internal sealed class CompactLookaheadNode : SimpleComponentNode
{
    public readonly NumericInputNode CompactLookahead = null!;

    public unsafe CompactLookaheadNode()
    {
        GeneralSettings config = System.Config.General;

        var titleNode = new LabelTextNode
        {
            Size = Size with { Y = 24 },
            String = "Compact Lookahead",
        };
        titleNode.AttachNode(this);

        CompactLookahead = new NumericInputNode
        {
            Position = Position with { X = 240 },
            Size = Size with { X = 88 },
            IsVisible = true,
            Value = config.CompactLookahead,
            OnValueUpdate = value =>
            {
                config.CompactLookahead = value;
                System.AddonInventoryWindow.ManualRefresh();
            }
        };
        CompactLookahead.ComponentBase->SetEnabledState(config.CompactPackingEnabled);
        CompactLookahead.AttachNode(this);
    }
}