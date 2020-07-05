using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity Library
using UnityEngine;

// Mod Loader Libraries
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomQuiverCounter
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class QuiverSpriteUpdater : BaseUnityPlugin
    {
        const string ID = "com.Zalamaur.CustomAmmoCounter";
        const string NAME = "Custom Ammo Counter";
        const string VERSION = "0.1";

        internal void Awake()
        {
            var harmony = new Harmony("com.Zalamaur.CustomAmmoCounter1"); // rename "author" and "project"
            harmony.PatchAll();
            Logger.Log(LogLevel.Message, "Loading Complete"); /* Prints to "BepInEx\LogOutput.log" */
        }
    }

    [HarmonyPatch(typeof(CharacterEquipment), "EquipWithoutAssociating")]
    public class CharacterEquipment_EquipWithoutAssociating
    {
        [HarmonyPostfix]
        public static void Postfix(Equipment _itemToEquip, bool _playAnim = false)
        {
            Debug.Log("Postfix trigger");
        }
    }



}
