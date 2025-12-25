using System.Numerics;
using AetherBags.Configuration;
using AetherBags.Nodes.Color;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Action = Lumina.Excel.Sheets.Action;

namespace AetherBags.Nodes.Configuration.Category;

public sealed class CategoryDefinitionConfigurationNode : VerticalListNode {
    private readonly CheckboxNode enabledCheckbox;
    private readonly TextInputNode nameInputNode;
    private readonly TextInputNode descriptionInputNode;
    private readonly ColorInputRow colorInputNode;

    public UserCategoryDefinition CategoryDefinition { get; private set; }

    public CategoryDefinitionConfigurationNode(UserCategoryDefinition categoryDefinition) {
        CategoryDefinition = categoryDefinition;

        FirstItemSpacing = 35.0f;
        ItemSpacing = 5.0f;

        enabledCheckbox = new CheckboxNode {
            IsChecked = CategoryDefinition.Enabled,
            OnClick = isChecked => CategoryDefinition.Enabled = isChecked,
        };
        AddNode(enabledCheckbox);

        colorInputNode = new ColorInputRow
        {
            Label = "Color",
            CurrentColor = CategoryDefinition.Color,
            DefaultColor = new UserCategoryDefinition().Color,
            OnColorConfirmed = color => CategoryDefinition.Color = color,
            // OnColorChange = color => CategoryDefinition.Color = color,
            OnColorCanceled = color => CategoryDefinition.Color = color,
        };
        AddNode(colorInputNode);

        nameInputNode = new TextInputNode
        {
            String = CategoryDefinition.Name,
            OnInputComplete = name => CategoryDefinition.Name = name.ExtractText()
        };
        AddNode(nameInputNode);

        descriptionInputNode = new TextInputNode
        {
            String = CategoryDefinition.Description,
            OnInputComplete = name => CategoryDefinition.Description = name.ExtractText()
        };
        AddNode(descriptionInputNode);

        // TODO: Add Rules
    }

    public void SetCategory(UserCategoryDefinition newCategory) {
        CategoryDefinition = newCategory;
        RefreshValues();
    }

    private void RefreshValues()
    {
        enabledCheckbox.IsChecked = CategoryDefinition.Enabled;
        colorInputNode.CurrentColor = CategoryDefinition.Color;
        nameInputNode.String = CategoryDefinition.Name;
    }
}
