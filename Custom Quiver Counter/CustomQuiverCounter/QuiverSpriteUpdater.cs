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

namespace CustomQuiverCounter
{
    [BepInPlugin(ID, NAME, VERSION)]
    public class QuiverSpriteUpdater : BaseUnityPlugin
    {
        const string ID = "com.Zalamaur.CustomQuiverCounter";
        const string NAME = "Custom Ammo Counter";
        const string VERSION = "0.1";
        static QuiverSpriteUpdater self;


        internal void Awake()
        {
            var harmony = new Harmony("com.Zalamaur.CustomQuiverCounter");
            harmony.PatchAll();
            Logger.Log(LogLevel.Message, "Loading Complete");

            //bepInExManager = this.transform.parent.gameObject;
            self = this;
        }

        public void UpdateSprite(Character triggerCharacter, Item triggerItem)
        {
            this.SpriteUpdateLogic(triggerCharacter, GameObject.Find(triggerCharacter.name + " UI"), triggerItem);
        }


        private void SpriteUpdateLogic(Character character, GameObject characterUIObject, Item item)
        {
            var icon = characterUIObject.transform
                    .Find("Canvas/GameplayPanels/HUD/QuiverDisplay/Icon")
                    .gameObject;

            /*
            GameObject newIcon = Instantiate(icon);
            newIcon.transform.SetParent(icon.transform);
            newIcon.transform.SetAsLastSibling();
            newIcon.name = "QuiverItemSprite";
            newIcon.transform.ResetLocal();
            

            Destroy(newIcon.transform.GetChild(0).gameObject);
            Destroy(newIcon.GetComponent<CanvasGroup>());
            Destroy(newIcon.GetComponent<CanvasRenderer>());
            */

            GameObject quiverSpriteUpdater = new GameObject();
            quiverSpriteUpdater.transform.SetParent(icon.transform);
            quiverSpriteUpdater.transform.SetAsLastSibling();
            quiverSpriteUpdater.name = "QuiverItemSprite";
            quiverSpriteUpdater.transform.ResetLocal();
            Image image = quiverSpriteUpdater.AddComponent<Image>();

            RectTransform rectTransform = quiverSpriteUpdater.GetComponent<RectTransform>();

            image.sprite = item.ItemIcon;
            quiverSpriteUpdater.transform.localPosition = new Vector3(25f, 0f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 33f);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);

            Debug.Log(item.ItemIconPath);
            //Canvas canvas = newIcon.AddComponent<Canvas>();


        }


        [HarmonyPatch(typeof(CharacterEquipment), "EquipWithoutAssociating")]
        public class CharacterEquipment_EquipWithoutAssociating
        {
            [HarmonyPostfix]
            public static void Postfix(Equipment _itemToEquip, Character ___m_character)
            {
                
                //Image imageHolder = iconObject.AddComponent<Image>() as Image;
                Debug.Log("test");
                self.UpdateSprite(___m_character, _itemToEquip);

                //Debug.Log(iconObject.name);

                //Image imageComponent = iconObject.GetComponent<Image>();
                //imageComponent.sprite = _itemToEquip.ItemIcon;

                //itemSprite = _itemToEquip.ItemIcon;
                //GameObject y = GameObject.Find("MenuManager");




            }
        }

        [HarmonyPatch(typeof(SplitScreenManager), "Awake")]
        public class SplitScreenManager_Awake
        {
            // m_charUIPrefab is a field on the SplitScreenManager class.
            // By adding three underscores before it, we can include it as an argument on our patch
            // and Harmony will use reflection for that field on the SplitScreenManager class for us.
            // In other words, ___m_charUIPrefab = SplitScreenManager.m_charUIPrefab

            [HarmonyPostfix]
            public static void Postfix(CharacterUI ___m_charUIPrefab)
            {
                Debug.Log("setting up UI");

                // sometimes a short-hand variable is easier to work with
                var ui = ___m_charUIPrefab;

                // To preserve changes when scenes are loaded, we need to call:
                GameObject.DontDestroyOnLoad(ui.gameObject);
                // now any new gameobjects we add to this transform will be preserved on scene changes.

                // Now to actually make a change, you'll likely want to get a certain part of the menu.
                // For example, to find the Character Skill menu prefab...

                var icon = ui.transform
                    .Find("Canvas/GameplayPanels/HUD/QuiverDisplay/Icon")
                    .gameObject;
                icon.AddComponent<Image>();

            }
        }

    }
}
