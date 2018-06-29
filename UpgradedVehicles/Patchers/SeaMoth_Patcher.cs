﻿namespace UpgradedVehicles.Patchers
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance, TechType techType)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance, techType);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("vehicleDefaultName", PropertyMethod.Getter)]
    internal class SeaMoth_vehicleDefaultName_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref SeaMoth __instance, ref string __result)
        {
            bool isUpgradedSeamoth = __instance.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (isUpgradedSeamoth)
            {
                __result = "SEAMOTH MK2";
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("vehicleDefaultColors", PropertyMethod.Getter)]
    internal class SeaMoth_vehicleDefaultColors_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref SeaMoth __instance, ref Vector3[] __result)
        {
            bool isUpgradedSeamoth = __instance.GetComponent<TechTag>().type == SeaMothMk2.TechTypeID;

            if (isUpgradedSeamoth)
            {
                __result = new Vector3[]
                            {
                                new Vector3(1f, 1f, 1f),
                                new Vector3(0f, 0f, 0f),
                                new Vector3(0f, 0f, 0f),
                                new Vector3(0.577f, 0.447f, 0.604f),
                                new Vector3(0.114f, 0.729f, 0.965f)
                            };
                return false;
            }

            return true;
        }

    }

}
