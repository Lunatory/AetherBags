using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AetherBags.Nodes.Configuration.Category;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;

namespace AetherBags.Addons;

public class AddonCategoryConfigurationWindow : NativeAddon
{
    private ModifyListNode<CategoryWrapper>? _selectionListNode;
    private VerticalLineNode? _separatorLine;
    private CategoryConfigurationNode? _configNode;
    private TextNode? _nothingSelectedTextNode;

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        List<CategoryWrapper> categoryDefinitionsWrappers = System.Config.Categories.UserCategories
            .Select(categoryDefinition => new CategoryWrapper(categoryDefinition))
            .ToList();

        _selectionListNode = new ModifyListNode<CategoryWrapper> {
            Position = ContentStartPosition,
            Size = new Vector2(250.0f, ContentSize.Y),
            SelectionOptions = categoryDefinitionsWrappers,
            OnOptionChanged = OnOptionChanged,
        };
        _selectionListNode.AttachNode(this);

        _separatorLine = new VerticalLineNode {
            Position = ContentStartPosition + new Vector2(250.0f + 8.0f, 0.0f),
            Size = new Vector2(4.0f, ContentSize.Y),
        };
        _separatorLine.AttachNode(this);

        _nothingSelectedTextNode = new TextNode {
            Position = ContentStartPosition + new Vector2(250.0f + 16.0f, 0.0f),
            Size = ContentSize - new Vector2(250.0f + 16.0f, 0.0f),
            AlignmentType = AlignmentType.Center,
            TextFlags = TextFlags.WordWrap | TextFlags.MultiLine,
            FontSize = 14,
            LineSpacing = 22,
            FontType = FontType.Axis,
            String = "Please select a category on the left or add one.",
            TextColor = ColorHelper.GetColor(1),
        };
        _nothingSelectedTextNode.AttachNode(this);

        _configNode = new CategoryConfigurationNode {
            Position = ContentStartPosition + new Vector2(250.0f + 16.0f, 0.0f),
            Size = ContentSize - new Vector2(250.0f + 16.0f, 0.0f),
            IsVisible = false,
        };

        _configNode.AttachNode(this);
    }

    private void OnOptionChanged(CategoryWrapper? newOption) {
        if (_configNode is null) return;

        _configNode.IsVisible = newOption is not null;
        _nothingSelectedTextNode?.IsVisible = newOption is null;

        _configNode.ConfigurationOption = newOption;
    }
}