using System.Collections.Generic;

namespace AetherBags.Inventory;

public readonly record struct CategorizedInventory(CategoryInfo Category, List<ItemInfo> Items);