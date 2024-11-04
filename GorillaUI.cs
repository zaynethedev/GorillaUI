using System;
using System.ComponentModel.Design;
using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtilla;
using static UnityEngine.UIElements.UxmlAttributeDescription;


namespace GorillaUI
{

    /// <summary>
    /// This is your mod's main class.
    /// </summary>

    /* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
    /*[ModdedGamemode]*/
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class GorillaUI : BaseUnityPlugin
    {
        bool inRoom;
        bool GUIEnabled = false;
        bool isThirdPerson = true;
        bool GUI2Enabled = false;
        bool GUI3Enabled = false;
        bool NoClip = false;
        bool DisabledLeaves = false;




        void Start()
        {
            /* A lot of Gorilla Tag systems will not be set up when start is called /*
			/* Put code in OnGameInitialized to avoid null references */

            Newtilla.Newtilla.OnJoinModded += OnModdedJoined;
            Newtilla.Newtilla.OnLeaveModded += OnModdedLeft;
        }

        void OnEnable()
        {
            /* Set up your mod here */
            /* Code here runs at the start and whenever your mod is enabled*/

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            /* Undo mod setup here */
            /* This provides support for toggling mods with ComputerInterface, please implement it :) */
            /* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            /* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
        }

        void Update()
        {
            if (Keyboard.current.tabKey.wasPressedThisFrame)
            {
                GUIEnabled = !GUIEnabled;
            }

            if (inRoom || !PhotonNetwork.InRoom)
            {
                if (Keyboard.current[Key.Backquote].wasPressedThisFrame)
                {
                    GUI2Enabled = !GUI2Enabled;
                }

            }



            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                isThirdPerson = !isThirdPerson;
                GorillaTagger.Instance.thirdPersonCamera.SetActive(isThirdPerson);
            }



            if (inRoom)
            {
                if (Keyboard.current.cKey.wasPressedThisFrame)
                {
                    GUI3Enabled = !GUI3Enabled;
                }
            }
            else
            {
                GUI3Enabled = false;
            }

            if (inRoom) // NO CLIP INGAME HOTKEY
            {
                if (ControllerInputPoller.instance.rightControllerSecondaryButton) //  B
                {
                    NoClip = !NoClip;
                }
            }

            if (inRoom)
            {
                foreach (MeshCollider Coliders in Resources.FindObjectsOfTypeAll<MeshCollider>())//no clip mod
                {
                    Coliders.enabled = !NoClip;
                }
                if (DisabledLeaves) // leaf mod
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(18).gameObject.SetActive(false);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(false);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(false);
                }
                if (DisabledLeaves == false) // DISABLES LEAF MOD
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(18).gameObject.SetActive(true);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(true);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(true);
                }
            }





        }

        /* This attribute tells Utilla to call this method when a modded room is joined */
        /*[ModdedGamemodeJoin]*/
        void OnModdedJoined(string modeName)
        {
            /* Activate your mod here */
            /* This code will run regardless of if the mod is enabled*/

            inRoom = true;
        }

        /* This attribute tells Utilla to call this method when a modded room is left */
        /*[ModdedGamemodeLeave]*/
        void OnModdedLeft(string modeName)
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(true);//leaf mod disable
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(21).gameObject.SetActive(true);

            foreach (MeshCollider Coliders in Resources.FindObjectsOfTypeAll<MeshCollider>())//noclip disable
            {
                //no clip disable
                Coliders.enabled = true;
            }

            inRoom = false;
        }
        private string room = "";

        private void OnGUI()
        {
            if (GUIEnabled)
            {
                GUI.Box(new Rect(10, 10, 150, 300), "GorillaUI");

                room = GUI.TextField(new Rect(15, 50, 140, 30), room, 25);

                if (GUI.Button(new Rect(15, 100, 140, 40), "Join Room"))
                {

                    if (!string.IsNullOrEmpty(room))
                    {
                        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(room, JoinType.Solo);
                    }
                    else
                    {
                        Debug.Log("Room name is empty.");
                    }

                }

                if (GUI.Button(new Rect(15, 150, 140, 40), "Disconnect"))
                {
                    NetworkSystem.Instance.ReturnToSinglePlayer();
                    Debug.Log("Disconnected");
                }

                if (GUI.Button(new Rect(15, 200, 140, 40), "Quit Game"))
                {
                    Application.Quit();
                }

                if (GUI.Button(new Rect(15, 250, 140, 40), isThirdPerson ? "FPC" : "3rd Person"))
                {
                    {
                        isThirdPerson = !isThirdPerson;
                        GorillaTagger.Instance.thirdPersonCamera.SetActive(isThirdPerson);
                    }
                }

                /*if (GUI.Button(new Rect(15, 300, 140, 40), "Placeholder"))
                {
                        
                }

                /*if (GUI.Button(new Rect(15, 400, 140, 40), "Placeholder"))
                {
						
                }*/
            }

            if (GUI2Enabled)
            {
                GUI.Box(new Rect(170, 10, 150, 250), "Gamemode selector");

                if (GUI.Button(new Rect(175, 50, 140, 40), "Set Casual"))
                {
                    GorillaComputer.instance.currentGameMode.Value = "CASUAL";
                    Debug.Log("Gamemode changed to CASUAL.");
                }

                if (GUI.Button(new Rect(175, 100, 140, 40), "Set Infection"))
                {
                    GorillaComputer.instance.currentGameMode.Value = "INFECTION";
                    Debug.Log("Gamemode changed to INFECTION.");
                }

                if (GUI.Button(new Rect(175, 150, 140, 40), "Set Modded Casual"))
                {
                    GorillaComputer.instance.currentGameMode.Value = "MODDED_CASUAL";
                    Debug.Log("Gamemode changed to MODDED_CASUAL.");
                }

                if (GUI.Button(new Rect(175, 200, 140, 40), "Set Modded"))
                {
                    GorillaComputer.instance.currentGameMode.Value = "MODDED_INFECTION";
                    Debug.Log("Gamemode changed to MODDED_INFECTION	.");
                }
            }

            if (GUI3Enabled)
            {
                GUI.Box(new Rect(10, 350, 150, 250), "Gorilla Menu");

                if (GUI.Button(new Rect(15, 400, 140, 40), "NoClip"))
                {
                    NoClip = !NoClip;
                }

                if (GUI.Button(new Rect(15, 450, 140, 40), "Toggle Leaves"))
                {
                    DisabledLeaves = !DisabledLeaves;
                }

                if (GUI.Button(new Rect(15, 500, 140, 40), "Placeholder"))
                {

                }

                if (GUI.Button(new Rect(15, 550, 140, 40), "Placeholder"))
                {

                }
            }
        }
    }
}
