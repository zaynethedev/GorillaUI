using System;
using System.ComponentModel.Design;
using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using Newtilla;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using System.Xml.Linq;


namespace GorillaUI
{
    [BepInDependency("Lofiat.Newtilla", "1.1.0")]
    [BepInIncompatibility("org.iidk.gorillatag.iimenu")]
    [BepInIncompatibility("com.goldentrophy.gorillatag.nametags")]
    [BepInIncompatibility("com.dedouwe26.gorillatag.cosmetx")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class GorillaUI : BaseUnityPlugin
    {
        bool inRoom;
        bool GUIEnabled = false;
        bool isThirdPerson = true;
        bool GUI2Enabled = false;
        bool GUI3Enabled = false;
        bool NoClip = false;
        bool NoClip2 = false;
        bool DisabledLeaves = false;
        bool tped;

        void Start()
        {
            Newtilla.Newtilla.OnJoinModded += OnModdedJoined;
            Newtilla.Newtilla.OnLeaveModded += OnModdedLeft;
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
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

            if (inRoom)
            {
                foreach (MeshCollider Coliders in Resources.FindObjectsOfTypeAll<MeshCollider>())
                {
                    Coliders.enabled = !NoClip;
                }
                if (DisabledLeaves)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(18).gameObject.SetActive(false);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(false);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(false);
                }
                if (DisabledLeaves == false)
                {
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(18).gameObject.SetActive(true);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(true);
                    GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(true);
                }
            }
            if(tped)
            {

                NoClip = false;
                GameObject forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
                GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
                GameObject city1 = GameObject.Find("Environment Objects/LocalObjects_Prefab/Vista_Prefab");
                GameObject city2 = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab");//TreeRoom
                GameObject tree = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables");
                GameObject treeroom = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom");
                forest.SetActive(true);
                treeroom.SetActive(true);
                tree.SetActive(true);
                sky.SetActive(true);
                city1.SetActive(false);
                city2.SetActive(false);
                tped = false;
            }
            /*Vector3 tped = new Vector3(-68, 12, -83);
            Vector3 prepos = GorillaLocomotion.Player.Instance.headCollider.transform.position;
            if (prepos.Equals(tped))
            {
                NoClip = false;
                GameObject forest = GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest");
                GameObject sky = GameObject.Find("Environment Objects/LocalObjects_Prefab/Standard Sky");
                GameObject city1 = GameObject.Find("Environment Objects/LocalObjects_Prefab/Vista_Prefab");
                GameObject city2 = GameObject.Find("Environment Objects/LocalObjects_Prefab/City_WorkingPrefab");//TreeRoom
                GameObject tree = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom/TreeRoomInteractables");
                GameObject treeroom = GameObject.Find("Environment Objects/LocalObjects_Prefab/TreeRoom");
                forest.SetActive(true);
                treeroom.SetActive(true);
                tree.SetActive(true);
                sky.SetActive(true);
                city1.SetActive(false);
                city2.SetActive(false);
                NoClip = false;
            }*/
        }
	
        void OnModdedJoined(string modeName)
        {
            inRoom = true;
        }
        void OnModdedLeft(string modeName)
        {
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(18).gameObject.SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(19).gameObject.SetActive(true);
            GameObject.Find("Environment Objects/LocalObjects_Prefab/Forest").transform.GetChild(20).gameObject.SetActive(true);

            foreach (MeshCollider Coliders in Resources.FindObjectsOfTypeAll<MeshCollider>())
            {
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

                if (GUI.Button(new Rect(15, 500, 140, 40), "Tp to stump"))
                {
                    tped = true;
                    NoClip = true;
                    GorillaLocomotion.Player.Instance.headCollider.transform.position = new Vector3(-68, 12, -83);
                }

                if (GUI.Button(new Rect(15, 550, 140, 40), "Placeholder"))
                {

                }
            }
        }
    }
}
