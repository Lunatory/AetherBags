using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AetherBags.Configuration;
using AetherBags.Configuration.Import;

namespace AetherBags.Helpers.Import;

public static class SortaKindaImportExport
{
    private static readonly JsonSerializerOptions ExternalJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        WriteIndented = true,
        IncludeFields = true
    };

    public static bool TryImportFromClipboard(
        SystemConfiguration targetConfig,
        bool replaceExisting,
        out string error)
    {
        error = string.Empty;
        string clipboard;
        try
        {
            clipboard = Dalamud.Bindings.ImGui.ImGui.GetClipboardText();
        }
        catch (Exception ex)
        {
            error = $"Failed to read clipboard: {ex.Message}";
            return false;
        }

        return TryImportFromJson(clipboard, targetConfig, replaceExisting, out error);
    }

    public static bool TryImportFromJson(
        string input,
        SystemConfiguration targetConfig,
        bool replaceExisting,
        out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Input was empty.";
            return false;
        }

        var external = Util.DeserializeCompressed<SortaKindaCategory[]>(input.Trim(), ExternalJsonOptions);

        if (external is null)
        {
            error = "Failed to parse SortaKinda input.";
            return false;
        }

        var mapped = external
            .Select(MapToUserCategory)
            .OrderBy(c => c.Order)
            .ToList();

        var dest = targetConfig.Categories.UserCategories;

        if (replaceExisting)
        {
            dest.Clear();
            dest.AddRange(mapped);
        }
        else
        {
            var byId = dest
                .Where(c => !string.IsNullOrWhiteSpace(c.Id))
                .ToDictionary(c => c.Id, StringComparer.OrdinalIgnoreCase);

            foreach (var incoming in mapped)
            {
                if (!string.IsNullOrWhiteSpace(incoming.Id) && byId.TryGetValue(incoming.Id, out var existing))
                {
                    existing.Name = incoming.Name;
                    existing.Description = incoming.Description;
                    existing.Order = incoming.Order;
                    existing.Priority = incoming.Priority;
                    existing.Color = incoming.Color;
                    existing.Rules = incoming.Rules;
                }
                else
                {
                    dest.Add(incoming);
                    if (!string.IsNullOrWhiteSpace(incoming.Id))
                        byId[incoming.Id] = incoming;
                }
            }
        }

        targetConfig.Categories.UserCategoriesEnabled = true;
        return true;
    }

    public static string ExportToJson(SystemConfiguration sourceConfig)
    {
        var exported = sourceConfig.Categories.UserCategories
            .OrderBy(c => c.Order)
            .Select(MapToExternal)
            .ToArray();

        return Util.SerializeCompressed(exported, ExternalJsonOptions);
    }

    public static void ExportToClipboard(SystemConfiguration sourceConfig)
        => Dalamud.Bindings.ImGui.ImGui.SetClipboardText(ExportToJson(sourceConfig));

    private static UserCategoryDefinition MapToUserCategory(SortaKindaCategory external)
        => new()
        {
            Id = string.IsNullOrWhiteSpace(external.Id) ? Guid.NewGuid().ToString("N") : external.Id,
            Name = external.Name,
            Description = string.Empty,
            Order = external.Index,
            Priority = 100,
            Color = external.Color,
            Rules = new CategoryRuleSet
            {
                AllowedItemIds = new List<uint>(),
                AllowedItemNamePatterns = external.AllowedItemNames?.ToList() ?? new List<string>(),
                AllowedUiCategoryIds = external.AllowedItemTypes?.ToList() ?? new List<uint>(),
                AllowedRarities = external.AllowedItemRarities?.ToList() ?? new List<int>(),
                ItemLevel = new RangeFilter<int>
                {
                    Enabled = external.ItemLevelFilter?.Enable ?? false,
                    Min = external.ItemLevelFilter?.MinValue ?? 0,
                    Max = external.ItemLevelFilter?.MaxValue ?? 2000,
                },
                VendorPrice = new RangeFilter<uint>
                {
                    Enabled = external.VendorPriceFilter?.Enable ?? false,
                    Min = external.VendorPriceFilter?.MinValue ?? 0u,
                    Max = external.VendorPriceFilter?.MaxValue ?? 9_999_999u,
                }
            }
        };

    private static SortaKindaCategory MapToExternal(UserCategoryDefinition internalCat)
        => new()
        {
            Color = internalCat.Color,
            Id = internalCat.Id,
            Name = internalCat.Name,
            Index = internalCat.Order,
            AllowedItemNames = internalCat.Rules.AllowedItemNamePatterns?.ToList() ?? new List<string>(),
            AllowedItemTypes = internalCat.Rules.AllowedUiCategoryIds?.ToList() ?? new List<uint>(),
            AllowedItemRarities = internalCat.Rules.AllowedRarities?.ToList() ?? new List<int>(),
            ItemLevelFilter = new ExternalRangeFilterDto<int>
            {
                Enable = internalCat.Rules.ItemLevel.Enabled,
                Label = "Item Level Filter",
                MinValue = internalCat.Rules.ItemLevel.Min,
                MaxValue = internalCat.Rules.ItemLevel.Max
            },
            VendorPriceFilter = new ExternalRangeFilterDto<uint>
            {
                Enable = internalCat.Rules.VendorPrice.Enabled,
                Label = "Vendor Price Filter",
                MinValue = internalCat.Rules.VendorPrice.Min,
                MaxValue = internalCat.Rules.VendorPrice.Max
            },
            Direction = 0,
            FillMode = 0,
            SortMode = 0
        };
}