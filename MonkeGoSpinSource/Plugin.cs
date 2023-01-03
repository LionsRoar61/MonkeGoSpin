using BepInEx;
using System;
using UnityEngine;
using Utilla;
using BepInEx.Configuration;
using System.IO;
using System.Collections.Generic;
using UnityEngine.XR;
using System.ComponentModel;

namespace MonkeGoSpin
{
    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [ModdedGamemode("MONKEGOSPIN", "MonkeGoSpin", Utilla.Models.BaseGamemode.Infection)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        bool LT;
        bool RT;
        public bool hauntedModMenuEnabled = true;
        public float minTime = 1.0f; // Minimum time between spins
        public float maxTime = 10.0f; // Maximum time between spins
        private float timeUntilNextSpin; // Time remaining until next spin

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/
            hauntedModMenuEnabled = true;

            timeUntilNextSpin = UnityEngine.Random.Range(minTime, maxTime);

            HarmonyPatches.ApplyHarmonyPatches();
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/
            hauntedModMenuEnabled = false;

            HarmonyPatches.RemoveHarmonyPatches();
            Utilla.Events.GameInitialized -= OnGameInitialized;
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
        }

        void Update()
        {
            /* Code here runs every frame when the mod is enabled */
            if (inRoom)
            {
                //Input incase
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevices(list);

                for (int i = 0; i < list.Count; i++) //Get input
                {
                    if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Left))
                    {
                        list[i].TryGetFeatureValue(CommonUsages.triggerButton, out LT);
                    }
                    if (list[i].characteristics.HasFlag(InputDeviceCharacteristics.Right))
                    {
                        list[i].TryGetFeatureValue(CommonUsages.triggerButton, out RT);
                    }
                }

                //Make Monke Spin
                timeUntilNextSpin -= Time.deltaTime; // Decrease the time until the next spin

                if (timeUntilNextSpin <= 0) // If it's time to spin again
                {
                    GorillaLocomotion.Player.Instance.transform.Rotate(Vector3.up, 5000 * Time.deltaTime); // Spin the object
                    timeUntilNextSpin = UnityEngine.Random.Range(minTime, maxTime); // Choose a new random time until the next spin
                }
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            //Check if in the right gamemode
            if (gamemode.Contains("MONKEGOSPIN"))
            {
                inRoom = true;
            }
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            /* Deactivate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = false;
        }
    }
}
