using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Unity Library
using UnityEngine;
using UnityEngine.UI;

// Mod Loader Libraries
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using System.IO;

namespace CustomQuiverCounter
{
    [BepInPlugin(ID, NAME, VERSION)]
    [BepInDependency(SL.GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class QuiverSpriteUpdater : BaseUnityPlugin
    {
        const string ID = "com.Zalamaur.CustomQuiverCounter";
        const string NAME = "Custom Ammo Counter";
        const string VERSION = "0.1";
        static QuiverSpriteUpdater self;
        static Harmony harmony;
        static bool debug;

        internal void Awake()
        {
            harmony = new Harmony("com.Zalamaur.CustomQuiverCounter");
            self = this;
 
            harmony.PatchAll();
            Logger.Log(LogLevel.Message, "Harmony patching complete.");
        }

        public void UpdateSprite(Character triggerCharacter, Equipment triggerEquipment)
        {
            self.Logger.Log(LogLevel.Message, "Executing function 'UpdateSprite'...");
            this.SpriteUpdateLogic(triggerCharacter, GameObject.Find(triggerCharacter.name + " UI"), triggerEquipment);
        }

        static GameObject CreateContainer(GameObject parentObject)
        {
            self.Logger.Log(LogLevel.Message, "Executing function 'CreateContainer'...");

            self.Logger.Log(LogLevel.Message, "Creating container 'GameObject'...");
            GameObject containerObject = new GameObject();

            self.Logger.Log(LogLevel.Message, "Setting up container settings...");
            containerObject.transform.SetParent(parentObject.transform);
            containerObject.transform.SetAsLastSibling();
            containerObject.name = "QuiverItemSprite";
            containerObject.transform.ResetLocal();
            containerObject.transform.localPosition = new Vector3(25f, 0f);

            self.Logger.Log(LogLevel.Message, "Creating 'Image' component in container object...");
            containerObject.AddComponent<Image>();

            self.Logger.Log(LogLevel.Message, "Setting up container 'RectTransform'...");
            RectTransform rectTransform = containerObject.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 33f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);

            self.Logger.Log(LogLevel.Message, "Returning container 'GameObject'...");
            return containerObject;
        }

        public GameObject GetContainer(GameObject parentObject)
        {
            self.Logger.Log(LogLevel.Message, "Executing function 'GetContainer'...");
            Transform childObject = null;
            self.Logger.Log(LogLevel.Message, "Attempting to find container...");
            childObject = parentObject.transform.Find("QuiverItemSprite");
            if (childObject != null)
            {
                self.Logger.Log(LogLevel.Message, "Container found, returning container...");
                return childObject.gameObject;
            }
            else
            {
                self.Logger.Log(LogLevel.Message, "Container not found, creating container...");
                return CreateContainer(parentObject);
            }
          
        }

        private void SpriteUpdateLogic(Character triggerCharacter, GameObject triggerCharacterUIObject, Equipment triggerEquipment)
        {

            self.Logger.Log(LogLevel.Message, "Executing function 'SpriteUpdateLogic'...");
            self.Logger.Log(LogLevel.Message, "Creating 'parentObject'...");
            GameObject parentObject = triggerCharacterUIObject.transform.Find("Canvas/GameplayPanels/HUD/QuiverDisplay/Icon").gameObject;

            self.Logger.Log(LogLevel.Message, "Creating 'childObject'...");
            GameObject childObject = self.GetContainer(parentObject);

            self.Logger.Log(LogLevel.Message, "Loading a sprite into 'childObject'...");
            childObject.GetComponent<Image>().sprite = triggerEquipment.ItemIcon;

        }


        [HarmonyPatch(typeof(CharacterEquipment), "EquipWithoutAssociating")]
        public class CharacterEquipment_EquipWithoutAssociating
        {
            [HarmonyPostfix]
            public static void Postfix(Equipment _itemToEquip, Character ___m_character)
            {
                self.Logger.Log(LogLevel.Message, "Postfix triggered -----------------------------");
                self.UpdateSprite(___m_character, _itemToEquip);
                self.Logger.Log(LogLevel.Message, "Postfix complete  -----------------------------");
            }
        }
    }
}
