﻿namespace CyclopsNuclearPower
{
    using System.Collections.Generic;
    using System.Reflection;
    using Harmony;
    using SMLHelper; // by ahk1221 https://github.com/ahk1221/SMLHelper/
    using SMLHelper.Patchers;
    using UnityEngine;
    using Object = UnityEngine.Object;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        public static TechType CyReactorRodType { get; private set; }
        public const string NameId = "CyclopsNuclearModule";
        public const string FriendlyName = "Cyclops Nuclear Battery Module";

        public static void Patch()
        {
            CreateItem();

            HarmonyInstance harmony = HarmonyInstance.Create("com.CyclopsNuclearPower.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        private static void CreateItem()
        {
            // Create a new TechType
            CyReactorRodType = TechTypePatcher.AddTechType(NameId, FriendlyName, "Portable nuclear power for your Cyclops. The days of running out of power are over.", true);
            
            // Make sure it's available from the start, it's hard enough to get as it is.
            KnownTechPatcher.unlockedAtStart.Add(CyReactorRodType);

            // Create the in-game item that will behave like any other Cyclops upgrade module
            CustomPrefabHandler.customPrefabs.Add(new CustomPrefab(NameId, $"WorldEntities/Tools/{NameId}", CyReactorRodType, GetGameObject));

            // Get the custom icon from the Unity assets bundle
            CustomSpriteHandler.customSprites.Add(new CustomSprite(CyReactorRodType, SpriteManager.Get(TechType.ReactorRod)));
            // TODO - Custom Sprite  - AssetBundle.LoadFromFile($"{ModFolder}/Assets/cysolar.assets").LoadAsset<Sprite>("CySolarIcon")));

            // Add the new recipe to the Modification Station crafting tree
            CraftTreePatcher.customNodes.Add(new CustomCraftNode(CyReactorRodType, CraftTree.Type.Workbench, $"CyclopsMenu/{NameId}"));

            // Pair the new recipie with the new TechType
            CraftDataPatcher.customTechData[CyReactorRodType] = GetRecipe();

            // Ensure that the new in-game item is classified as a Cyclops upgrade module. Otherwise you can't equip it.
            CraftDataPatcher.customEquipmentTypes[CyReactorRodType] = EquipmentType.CyclopsModule;
        }

        private static TechDataHelper GetRecipe()
        {            
            return new TechDataHelper()
            {
                _craftAmount = 1,
                _ingredients = new List<IngredientHelper>(new IngredientHelper[6]
                             {
                                 // This is to validate that the player has access to nuclear power already
                                 new IngredientHelper(TechType.ReactorRod, 1), 
                                 new IngredientHelper(TechType.Lubricant, 2),
                                 new IngredientHelper(TechType.EnameledGlass, 2),
                                 new IngredientHelper(TechType.ComputerChip, 1),
                                 new IngredientHelper(TechType.WiringKit, 1),
                                 new IngredientHelper(TechType.PlasteelIngot, 1)
                             }),
                _techType = CyReactorRodType
            };
        }

        private static GameObject GetGameObject()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/CyclopsThermalReactorModule");
            GameObject obj = Object.Instantiate(prefab);

            obj.GetComponent<PrefabIdentifier>().ClassId = NameId;
            obj.GetComponent<TechTag>().type = CyReactorRodType;

            // The battery component makes it easy to track the charge and saving the data is automatic.
            var pCell = obj.AddComponent<Battery>();
            pCell.name = FriendlyName;
            pCell._capacity = NuclearBatteryManager.MaxCharge;
            pCell._charge = NuclearBatteryManager.MaxCharge;

            return obj;
        }
    }
}
