using FFXIVClientStructs.FFXIV.Client.Game;

namespace AetherBags.Extensions;

public static class InventoryTypeExtensions
{
    extension(InventoryType inventoryType)
    {
        public uint AgentItemContainerId =>
            inventoryType switch
            {
                InventoryType.EquippedItems => 4,
                InventoryType.KeyItems => 7,
                InventoryType.Inventory1 => 48,
                InventoryType.Inventory2 => 49,
                InventoryType.Inventory3 => 50,
                InventoryType.Inventory4 => 51,
                InventoryType.ArmoryMainHand => 57,
                InventoryType.ArmoryHead => 58,
                InventoryType.ArmoryBody => 59,
                InventoryType.ArmoryHands => 60,
                InventoryType.ArmoryLegs => 61,
                InventoryType.ArmoryFeets => 62,
                InventoryType.ArmoryOffHand => 63,
                InventoryType.ArmoryEar => 64,
                InventoryType.ArmoryNeck => 65,
                InventoryType.ArmoryWrist => 66,
                InventoryType.ArmoryRings => 67,
                InventoryType.ArmorySoulCrystal => 68,
                InventoryType.SaddleBag1 => 69,
                InventoryType.SaddleBag2 => 70,
                InventoryType.PremiumSaddleBag1 => 71,
                InventoryType.PremiumSaddleBag2 => 72,
                _ => 0
            };

        public static InventoryType GetInventoryTypeFromContainerId(int id) =>
            id switch
            {
                4 => InventoryType.EquippedItems,
                7 => InventoryType.KeyItems,
                48 => InventoryType.Inventory1,
                49 => InventoryType.Inventory2,
                50 => InventoryType.Inventory3,
                51 => InventoryType.Inventory4,
                57 => InventoryType.ArmoryMainHand,
                58 => InventoryType.ArmoryHead,
                59 => InventoryType.ArmoryBody,
                60 => InventoryType.ArmoryHands,
                61 => InventoryType.ArmoryLegs,
                62 => InventoryType.ArmoryFeets,
                63 => InventoryType.ArmoryOffHand,
                64 => InventoryType.ArmoryEar,
                65 => InventoryType.ArmoryNeck,
                66 => InventoryType.ArmoryWrist,
                67 => InventoryType.ArmoryRings,
                68 => InventoryType.ArmorySoulCrystal,
                69 => InventoryType.SaddleBag1,
                70 => InventoryType.SaddleBag2,
                71 => InventoryType.PremiumSaddleBag1,
                72 => InventoryType.PremiumSaddleBag2,
                _ => (InventoryType)0
            };
    }
}