using BepInEx;
using HarmonyLib;
using SideLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomQuiverCounter
{
    [BepInPlugin(ID, NAME, VERSION)]
    [BepInDependency(SL.GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class QuiverSpriteUpdater : BaseUnityPlugin
    {
        private const string ID = "com.Zalamaur.CustomQuiverCounter";
        private const string NAME = "Custom Ammo Counter";
        private const string VERSION = "0.1";
        public static QuiverSpriteUpdater self;
        public static Harmony harmony;

        public void Awake()
        {
            try
            {
                self = this;

                SeverityLog("Initialising...");
                Logger.LogDebug("Creating Harmony instance...");
                harmony = new Harmony("com.Zalamaur.CustomQuiverCounter");

                Logger.LogDebug("Harmony patching initiating...");
                harmony.PatchAll();
                Logger.LogDebug("Harmony patching complete.");
                SeverityLog("Initialisation complete.");
            }
            catch (Exception e)
            {
                QuiverSpriteUpdater.SeverityLog(e.Message, 4);
                throw e;
            }
        }

        public static void SeverityLog(string message, int severity = 0)
        {
            switch (severity)
            {
                case 0:
                    self.Logger.LogInfo(message);
                    break;

                case 1:
                    self.Logger.LogMessage(message);
                    break;

                case 2:
                    self.Logger.LogWarning(message);
                    break;

                case 3:
                    self.Logger.LogError(message);
                    break;

                case 4:
                    self.Logger.LogFatal(message);
                    break;

                default:
                    self.Logger.LogWarning("[Message has invalid severity " + severity + "] " + message);
                    break;
            }
        }

        public void UpdateSprite(Character triggerCharacter, Equipment triggerEquipment)
        {
            this.SpriteUpdateLogic(GameObject.Find(triggerCharacter.name + " UI"), triggerEquipment);
        }

        private static GameObject CreateContainer(GameObject parentObject)
        {
            self.Logger.LogDebug("Creating container 'GameObject'...");
            GameObject containerObject = new GameObject();

            self.Logger.LogDebug("Setting up container properties...");
            containerObject.transform.SetParent(parentObject.transform);
            containerObject.transform.SetAsLastSibling();
            containerObject.name = "QuiverItemSprite";
            containerObject.transform.ResetLocal();
            containerObject.transform.localPosition = new Vector3(25f, 0f);

            self.Logger.LogDebug("Creating 'Image' component in container object...");
            containerObject.AddComponent<Image>();

            self.Logger.LogDebug("Setting up container 'RectTransform'...");
            RectTransform rectTransform = containerObject.GetComponent<RectTransform>();
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 33f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);

            self.Logger.LogDebug("Returning container 'GameObject'...");
            return containerObject;
        }

        public GameObject GetContainer(GameObject parentObject)
        {
            Transform childObject = null;
            self.Logger.LogDebug("Attempting to find container...");
            childObject = parentObject.transform.Find("QuiverItemSprite");
            if (childObject != null)
            {
                self.Logger.LogDebug("Container found, returning container...");
                return childObject.gameObject;
            }
            else
            {
                self.Logger.LogDebug("Container not found, creating container...");
                return CreateContainer(parentObject);
            }
        }

        private void SpriteUpdateLogic(GameObject triggerCharacterUIObject, Equipment triggerEquipment)
        {
            if (triggerEquipment.EquipSlot == EquipmentSlot.EquipmentSlotIDs.Quiver)
            {
                self.Logger.LogDebug("Creating 'parentObject'...");
                GameObject parentObject = triggerCharacterUIObject.transform.Find("Canvas/GameplayPanels/HUD/QuiverDisplay/Icon").gameObject;

                self.Logger.LogDebug("Creating 'childObject'...");
                GameObject childObject = self.GetContainer(parentObject);

                self.Logger.LogDebug("Loading a sprite into 'childObject'...");
                childObject.GetComponent<Image>().sprite = triggerEquipment.ItemIcon;
            }


        }

        [HarmonyPatch(typeof(CharacterEquipment), "EquipWithoutAssociating")]
        public class CharacterEquipment_EquipWithoutAssociating
        {
            [HarmonyPostfix]
            public static void Postfix(Equipment _itemToEquip, Character ___m_character)
            {
                try
                {
                    self.Logger.LogDebug("Postfix triggered -----------------------------");
                    self.UpdateSprite(___m_character, _itemToEquip);
                    self.Logger.LogDebug("Postfix complete  -----------------------------");
                }
                catch (Exception e)
                {
                    QuiverSpriteUpdater.SeverityLog(e.Message, 4);
                    throw e;
                }
            }
        }
    }
}