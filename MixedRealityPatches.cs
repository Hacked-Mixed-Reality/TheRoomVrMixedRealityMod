using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppValve.VR;
using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Il2CppValve.VR.SteamVR_ExternalCamera;
using static Il2CppValve.VR.SteamVR_Render;
using static MelonLoader.MelonLogger;

namespace MixedRealityMod
{
    public class MixedRealityPatches : MelonMod
    {


        [HarmonyPatch(typeof(Il2CppValve.VR.SteamVR_ExternalCamera), "ReadConfig")]
        static class ReadConfigPatch
        {
            public static bool Prefix(ref SteamVR_ExternalCamera __instance)
            {
                MelonLogger.Msg("Reading ExternalCamera.cfg");
                ReadConfig(ref __instance);
                FixLayers();
                return false;
            }

        }


        [HarmonyPatch(typeof(Il2CppValve.VR.SteamVR_ExternalCamera), "OnEnable")]
        static class ExternalCameraOnEnablePatch
        {
            public static bool Prefix(ref SteamVR_ExternalCamera __instance)
            {
                __instance.AutoEnableActionSet();
                return false;
            }

        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            base.OnSceneWasInitialized(buildIndex, sceneName);
            SteamVR_Render.instance.externalCamera.ReadConfig();
        }

        public static void ReadConfig(ref SteamVR_ExternalCamera instance)
        {

            try
            {
                var mCam = new HmdMatrix34_t();
                var readCamMatrix = false;

                object c = instance.config; // box
                var lines = System.IO.File.ReadAllLines(instance.configPath);
                foreach (var line in lines)
                {
                    var split = line.Split('=');
                    if (split.Length == 2)
                    {
                        var key = split[0];
                        if (key == "m")
                        {
                            var values = split[1].Split(',');
                            if (values.Length == 12)
                            {
                                mCam.m0 = float.Parse(values[0]);
                                mCam.m1 = float.Parse(values[1]);
                                mCam.m2 = float.Parse(values[2]);
                                mCam.m3 = float.Parse(values[3]);
                                mCam.m4 = float.Parse(values[4]);
                                mCam.m5 = float.Parse(values[5]);
                                mCam.m6 = float.Parse(values[6]);
                                mCam.m7 = float.Parse(values[7]);
                                mCam.m8 = float.Parse(values[8]);
                                mCam.m9 = float.Parse(values[9]);
                                mCam.m10 = float.Parse(values[10]);
                                mCam.m11 = float.Parse(values[11]);
                                readCamMatrix = true;
                            }
                        }
#if !UNITY_METRO
                        else if (key == "disableStandardAssets")
                        {
                            var field = c.GetType().GetField(key);
                            if (field != null)
                                field.SetValue(c, bool.Parse(split[1]));
                        }
                        else
                        {
                            var field = c.GetType().GetField(key);
                            if (field != null)
                                field.SetValue(c, float.Parse(split[1]));
                        }
#endif
                    }
                }
                instance.config = (Config)c; //unbox

                // Sorry - We're not supporting matrix yet.
                //if (readCamMatrix)
                //{
                //    var t = new SteamVR_Utils.RigidTransform(mCam);
                //    instance.config.x = t.pos.x;
                //    instance.config.y = t.pos.y;
                //    instance.config.z = t.pos.z;
                //    var angles = t.rot.eulerAngles;
                //    instance.config.rx = angles.x;
                //    instance.config.ry = angles.y;
                //    instance.config.rz = angles.z;
                //}
            }
            catch { }

            // Clear target so AttachToCamera gets called to pick up any changes.
            instance.target = null;

        }


        private static void FixLayers()
        {
            SteamVR_ExternalCamera? instance = SteamVR_Render.instance?.externalCamera;
            if (instance != null)
            {
                instance.cam.cullingMask = ModLayers.GetGameLayerMask();

            }
        }


        public override void OnUpdate()
        {
            if (Time.frameCount % 80 == 0)
            {
                if (File.Exists(MelonEnvironment.MelonBaseDirectory + "/updateexternalcamera"))
                {
                    try
                    {
                        SteamVR_Render.instance.externalCamera.ReadConfig();
                        File.Delete(MelonEnvironment.MelonBaseDirectory + "/updateexternalcamera");
                    }
                    catch (Exception e)
                    {
                        MelonLogger.Msg(e.Message);
                    }
                }
            }



            base.OnUpdate();

        }

        public enum GameLayer
        {
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Water = 4,
            UI = 5,
            BlackLight = 8,
            StereoRender_Ignore = 9,
            Imposter = 10,
            NonVisualGameplay_0 = 11,
            HeadCheck = 12,
            InventoryCheck = 13,
            FadeManager = 14,
            InventoryRaycast = 15,
            IngredientBottlePhysics = 16,
            PhysicsGameplay = 17,
            HideUI = 18,
            PauseMenu = 31,

        }

        public class ModLayers
        {
            public static LayerMask GetGameLayerMask()
            {
                LayerMask layerMask = ~0;
                layerMask &= ~(1 << (int)GameLayer.IgnoreRaycast); //Some lighting
                //layerMask &= ~(1 << (int)GameLayer.FadeManager);
                //layerMask &= ~(1 << (int)GameLayer.VRRenderingOnly);
                return layerMask;
            }
        }



        private void ShowDiagnostics()
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                if (cam != null)
                {
                    MelonLogger.Msg("Camera {0} has mask {1}", cam.name, cam.cullingMask);
                }
                else
                {
                    MelonLogger.Msg("There's a null camera...");
                }
            }
            for (int i = 0; i < 32; i++)
            {
                MelonLogger.Msg("Layer {0}: {1}", i.ToString(), LayerMask.LayerToName(i));

            }
        }
    }
}