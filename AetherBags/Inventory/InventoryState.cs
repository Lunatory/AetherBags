using AetherBags.Currency;
using Dalamud.Game.Inventory;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AetherBags.Inventory;

public static unsafe class InventoryState
{
    public static readonly InventoryType[] StandardInventories =
    [
        InventoryType.Inventory1,
        InventoryType.Inventory2,
        InventoryType.Inventory3,
        InventoryType.Inventory4,
        InventoryType.EquippedItems,
        InventoryType.ArmoryMainHand,
        InventoryType.ArmoryHead,
        InventoryType.ArmoryBody,
        InventoryType.ArmoryHands,
        InventoryType.ArmoryWaist,
        InventoryType.ArmoryLegs,
        InventoryType.ArmoryFeets,
        InventoryType.ArmoryOffHand,
        InventoryType.ArmoryEar,
        InventoryType.ArmoryNeck,
        InventoryType.ArmoryWrist,
        InventoryType.ArmoryRings,
        InventoryType.Currency,
        InventoryType.Crystals,
        InventoryType.ArmorySoulCrystal,
    ];

    private static readonly InventoryType[] BagInventories =
    [
        InventoryType.Inventory1,
        InventoryType.Inventory2,
        InventoryType.Inventory3,
        InventoryType.Inventory4,
    ];

    public static bool Contains(this IReadOnlyCollection<InventoryType> inventoryTypes, GameInventoryType type)
        => inventoryTypes.Contains((InventoryType)type);

    private static readonly Dictionary<uint, CategoryInfo> CategoryInfoCache = new(capacity: 256);

    public static List<CategorizedInventory> GetInventoryItemCategories(string filterString = "", bool invert = false)
    {
        List<ItemInfo> items = string.IsNullOrEmpty(filterString)
            ? GetInventoryItems()
            : GetInventoryItems(filterString, invert);

        if (items.Count == 0)
            return new List<CategorizedInventory>(0);

        var buckets = new Dictionary<uint, CategoryBucket>(capacity: Math.Min(128, items.Count));

        for (int i = 0; i < items.Count; i++)
        {
            ItemInfo info = items[i];
            uint catKey = info.UiCategory.RowId;

            if (!buckets.TryGetValue(catKey, out CategoryBucket? bucket))
            {
                bucket = new CategoryBucket
                {
                    Key = catKey,
                    Category = GetCategoryInfoForKeyCached(catKey, info),
                    Items = new List<ItemInfo>(capacity: 16),
                };
                buckets.Add(catKey, bucket);
            }

            bucket.Items.Add(info);
        }

        uint[] keys = new uint[buckets.Count];
        int k = 0;
        foreach (var key in buckets.Keys)
            keys[k++] = key;
        Array.Sort(keys);

        var result = new List<CategorizedInventory>(keys.Length);
        for (int i = 0; i < keys.Length; i++)
        {
            CategoryBucket bucket = buckets[keys[i]];
            bucket.Items.Sort(ItemCountDescComparer.Instance);
            result.Add(new CategorizedInventory(bucket.Category, bucket.Items));
        }

        return result;
    }

    public static List<ItemInfo> GetInventoryItems()
    {
        var dict = new Dictionary<uint, AggregatedItem>(capacity: 128);

        InventoryManager* mgr = InventoryManager.Instance();
        if (mgr == null)
            return new List<ItemInfo>(0);

        for (int invIndex = 0; invIndex < BagInventories.Length; invIndex++)
        {
            var container = mgr->GetInventoryContainer(BagInventories[invIndex]);
            if (container == null)
                continue;

            int size = container->Size;
            for (int slot = 0; slot < size; slot++)
            {
                ref var item = ref container->Items[slot];
                uint id = item.ItemId;
                if (id == 0)
                    continue;

                int qty = item.Quantity;

                if (dict.TryGetValue(id, out AggregatedItem agg))
                {
                    agg.Total += qty;
                    dict[id] = agg;
                }
                else
                {
                    dict.Add(id, new AggregatedItem { First = item, Total = qty });
                }
            }
        }

        if (dict.Count == 0)
            return new List<ItemInfo>(0);

        var list = new List<ItemInfo>(dict.Count);
        foreach (var kvp in dict)
        {
            AggregatedItem agg = kvp.Value;

            list.Add(new ItemInfo
            {
                Item = agg.First,
                ItemCount = agg.Total,
            });
        }

        return list;
    }

    public static List<ItemInfo> GetInventoryItems(string filterString, bool invert = false)
    {
        List<ItemInfo> all = GetInventoryItems();
        if (all.Count == 0)
            return all;

        var filtered = new List<ItemInfo>(all.Count);
        var re = new Regex(filterString, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        for (int i = 0; i < all.Count; i++)
        {
            ItemInfo info = all[i];

            bool isMatch = info.IsRegexMatch(re);
            if (isMatch != invert)
                filtered.Add(info);
        }

        return filtered;
    }

    private static CategoryInfo GetCategoryInfoForKeyCached(uint key, ItemInfo sample)
    {
        if (CategoryInfoCache.TryGetValue(key, out var cached))
            return cached;

        CategoryInfo info = GetCategoryInfoForKeySlow(key, sample);
        CategoryInfoCache[key] = info;
        return info;
    }

    private static CategoryInfo GetCategoryInfoForKeySlow(uint key, ItemInfo sample)
    {
        if (key == 0)
        {
            return new CategoryInfo
            {
                Name = "Misc",
                Description = "Uncategorized items",
            };
        }

        var uiCat = sample.UiCategory.Value;
        string? name = uiCat.Name.ToString();

        if (string.IsNullOrWhiteSpace(name))
            name = $"Category\\ {key}";

        return new CategoryInfo
        {
            Name = name,
        };
    }

    private static uint GetEmptyItemSlots() => InventoryManager.Instance()->GetEmptySlotsInBag();

    private static uint GetUsedItemSlots() => 140 - GetEmptyItemSlots();

    public static string GetEmptyItemSlotsString() => $"{GetUsedItemSlots()}/140";

    public static CurrencyInfo GetCurrencyInfo(uint itemId)
    {
        return new CurrencyInfo
        {
            Amount = InventoryManager.Instance()->GetInventoryItemCount(1),
            ItemId = itemId,
            IconId = Services.DataManager.GetExcelSheet<Item>().GetRow(itemId).Icon
        };
    }

    private struct AggregatedItem
    {
        public InventoryItem First;
        public int Total;
    }

    private sealed class ItemCountDescComparer : IComparer<ItemInfo>
    {
        public static readonly ItemCountDescComparer Instance = new();

        public int Compare(ItemInfo x, ItemInfo y)
        {
            if (x.ItemCount > y.ItemCount) return -1;
            if (x.ItemCount < y.ItemCount) return 1;
            return 0;
        }
    }

    private sealed class CategoryBucket
    {
        public uint Key;
        public CategoryInfo Category = null!;
        public List<ItemInfo> Items = null!;
    }
}
