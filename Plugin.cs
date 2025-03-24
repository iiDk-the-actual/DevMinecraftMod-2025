using BepInEx;
using Bepinject;
using DevMinecraftMod.Scripts;
using DevMinecraftMod.Scripts.Building;
using DevMinecraftMod.Scripts.Music;
using GorillaNetworking;
using HarmonyLib;
using Photon.Pun;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace DevMinecraftMod
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance;

        public MinecraftMod mf;
        private bool mfExists = false;

        public bool modFunction = true;

        public MinecraftMusic mm;
        private bool mmExists;

        public Recover mrf;
        private bool mrfExists;

        private bool inRoom { get { return PhotonNetwork.InRoom; } set { inRoom = value; } }

        public bool sIndicatorEnabled = true;
        public bool lIndicatorEnabled = false;
        public float musicVolume = 0.05f;
        public float blockVolume = 0.25f;

        public bool stainedGlass;

        public bool isPlayerModelExist = false;
        public string location;
        public string dataLocation;
        public int placed;
        public int removed;

        public SaveData data = new SaveData();

        internal void OnEnable()
        {
            if (mf == null) mfExists = false;
        }

        internal void OnDisable()
        {
            if (mf != null) mfExists = true;
        }

        internal void Awake()
        {
            Instance = this;
        }

        internal bool updated;
        internal void Update()
        {
            if (!updated && GorillaLocomotion.GTPlayer.Instance != null)
            {
                OnGameInitialized();
                updated = true;
            }
        }

        private void OnGameInitialized()
        {
            if (mf == null && !mfExists)
            {
                mf = gameObject.AddComponent<MinecraftMod>();
                mfExists = true;

                try
                {
                    mf.IsSteam = Traverse.Create(PlayFabAuthenticator.instance).Field("platform").GetValue().ToString().ToLower() == "steam";
                } catch { }

                try
                {
                    Destroy(GameObject.Find("NetworkTriggers/QuitBox").GetComponent<GorillaQuitBox>());
                    GameObject.Find("NetworkTriggers/QuitBox").AddComponent<MinecraftQuitBox>();
                } catch { }

                if (mm == null && !mmExists)
                {
                    mm = gameObject.AddComponent<MinecraftMusic>();
                    mmExists = true;
                }

                if (mrf == null && !mrfExists)
                {
                    mrf = gameObject.AddComponent<Recover>();
                    mrfExists = true;
                }

                location = Directory.GetCurrentDirectory();
                location += $"\\BepInEx\\plugins\\{PluginInfo.Name}\\Data";

                if (!Directory.Exists(location))
                    Directory.CreateDirectory(location);

                dataLocation = location + $"\\OptionData.devmoddata";

                GetSettings();
            }

            HarmonyPatches.ApplyHarmonyPatches();
        }

        public void GetSettings()
        {
            if (File.Exists(dataLocation))
                data = JsonUtility.FromJson<SaveData>(File.ReadAllText(dataLocation));
            else
                File.WriteAllText(dataLocation, JsonUtility.ToJson(data));

            sIndicatorEnabled = data.square;
            lIndicatorEnabled = data.line;
            musicVolume = data.musicVolume;
            blockVolume = data.blockVolume;
            placed = data.totalBlocksPlaced;
            removed = data.totalBlocksRemoved;
        }

        public void SetSettings()
        {
            data.square = sIndicatorEnabled;
            data.line = lIndicatorEnabled;
            data.musicVolume = musicVolume;
            data.blockVolume = blockVolume;
            data.totalBlocksPlaced = placed;
            data.totalBlocksRemoved = removed;

            File.WriteAllText(dataLocation, JsonUtility.ToJson(data));
        }

        public bool GetRoomState()
        {
            return inRoom;
        }
    }
}
