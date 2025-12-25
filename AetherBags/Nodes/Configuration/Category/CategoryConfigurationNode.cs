using System.Collections.Generic;
using System.Numerics;
using AetherBags.Addons;
using AetherBags.Configuration;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;
using Lumina.Excel.Sheets;

namespace AetherBags.Nodes.Configuration.Category;

public class CategoryConfigurationNode : ConfigNode<CategoryWrapper> {
    private readonly ScrollingAreaNode<VerticalListNode> _categoryList;
    private CategoryDefinitionConfigurationNode? _activeNode;

    public CategoryConfigurationNode() {
        _categoryList = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 100.0f,
            AutoHideScrollBar = true,
        };
        _categoryList.ContentNode.FitContents = true;
        _categoryList.AttachNode(this);
    }


    protected override void OptionChanged(CategoryWrapper? option) {
        if (option?.CategoryDefinition is null) {
            _categoryList.IsVisible = false;
            return;
        }

        _categoryList.IsVisible = true;

        if (_activeNode is null) {
            _activeNode = new CategoryDefinitionConfigurationNode(option.CategoryDefinition) {
                Size = new Vector2(_categoryList.ContentNode.Width, 0f),
            };
            _categoryList.ContentNode.AddNode(_activeNode);
        } else {
            _activeNode.SetCategory(option.CategoryDefinition);
        }

        _categoryList.ContentNode.RecalculateLayout();
        _categoryList.ContentHeight = _categoryList.ContentNode.Height;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        _categoryList.Size = Size;
        _categoryList.ContentNode.Width = Width;

        foreach (var node in _categoryList.ContentNode.GetNodes<CategoryDefinitionConfigurationNode>()) {
            node.Width = Width;
        }
    }
}
