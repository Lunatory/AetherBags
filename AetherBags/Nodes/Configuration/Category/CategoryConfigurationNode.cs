using System;
using AetherBags.Addons;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;

namespace AetherBags.Nodes.Configuration.Category;

public class CategoryConfigurationNode :  ConfigNode<CategoryWrapper>
{
    private readonly ScrollingListNode _categoryList;
    private CategoryDefinitionConfigurationNode? _activeNode;

    public Action? OnCategoryChanged { get; set; }

    public CategoryConfigurationNode()
    {
        _categoryList = new ScrollingListNode
        {
            AutoHideScrollbar = true,
        };
        _categoryList.FitContents = true;
        _categoryList.AttachNode(this);
    }

    protected override void OptionChanged(CategoryWrapper? option)
    {
        if (option?.CategoryDefinition is null)
        {
            _categoryList.IsVisible = false;
            return;
        }

        _categoryList.IsVisible = true;

        if (_activeNode is null)
        {
            _activeNode = new CategoryDefinitionConfigurationNode(option.CategoryDefinition)
            {
                Size = _categoryList.Size,
                OnLayoutChanged = UpdateScrollHeight,
                OnCategoryPropertyChanged = OnCategoryChanged,
            };
            _categoryList.AddNode(_activeNode);
        }
        else
        {
            _activeNode.SetCategory(option.CategoryDefinition);
        }

        UpdateScrollHeight();
    }

    private void UpdateScrollHeight()
    {
        _categoryList.RecalculateLayout();
        //_categoryList.ContentHeight = _categoryList.Height;
    }

    protected override void OnSizeChanged()
    {
        base.OnSizeChanged();
        _categoryList.Size = Size;
        _categoryList.Width = Width;

        foreach (var node in _categoryList.GetNodes<CategoryDefinitionConfigurationNode>())
        {
            node.Width = Width;
        }

        UpdateScrollHeight();
    }
}