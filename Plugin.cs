using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.UnityEngine;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using static Board;
using static CreateBullet;
using static MixData;

namespace TestWai;

[BepInPlugin("TestWai", "TestWai", "0.0.0")]
public class Plugin : BasePlugin
{
    public static string dllDirectory;
    public static MyConfig config= new MyConfig();
    public static ZombietType[] ZombieTypesArray = (ZombietType[])System.Enum.GetValues(typeof(ZombietType));
    public static BulletType[] bulletTypesArray = (BulletType[])System.Enum.GetValues(typeof(BulletType));
    public static PlantType[] plantTypesArray = (PlantType[])System.Enum.GetValues(typeof(PlantType));
    public static MethodInfo originalMethod;

    public class MyConfig
    {
        public bool isCardNoCD { get;set; } = false;
        public bool isGloveNoCD { get; set; } = false;
        public bool isHammerNoCD { get; set; } = false;
        public bool isFreePlant { get; set; } = false;
        public bool isDeveloperMode { get; set; } = false;
        public bool isFastShoot { get; set; } = false;
        public bool isGoldCountless { get; set; } = false;
        public bool isIgnoreZombieIn { get; set; } = false;
        public int plantMode { get; set; } = 1;
        public bool isBulletKill { get; set; } = false;
        public bool isRandomBullet { get; set; } = false;
        public bool isRandomPlant { get; set; } = false;
        public bool isRandomZombie { get; set; } = false;
        public bool isCreateZombieRate { get; set; } = false;
        public bool isPlantWD { get; set; } = false;
        public bool isZombieCold { get; set; } = false;
        public bool isZombieFreeze { get; set; } = false;
        public bool isZombieMindControlled { get; set; } = false;
        public bool isZombieGrap { get; set; } = false;
        public bool isZombieJalaed { get; set; } = false;
        public bool isClickZombie { get; set; } = false;
        public bool isClickZombie2 { get; set; } = false;
        public bool isClickPlant { get; set; } = false;
        public bool isOpenWords { get; set; } = false;
        public int createZombieRate { get; set; } = 1;
        public int clickZombieType { get; set; } = 1;
        public int clickPlantType { get; set; } = 1;


    }
    public override void Load()
    {
        string dllPath = Assembly.GetExecutingAssembly().Location;
        dllDirectory = Path.GetDirectoryName(dllPath);
        SocketServer.StopServer();
        SocketServer.StartServer();
        new Harmony("TestWai").PatchAll();
     }


    [HarmonyPatch(typeof(GameAPP), "Start")]
    class GameAPPPatch
    {
        [HarmonyPrefix]
        static void Prefix()
        {
            
        }

    }

    [HarmonyPatch(typeof(Board), "Update")]
    class PatchBoard
    {
        [HarmonyPrefix]
        static void Prefix(Board __instance)
        {
            if(config.isGoldCountless)
            {
                __instance.theMoney = 999999999;
            }
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                if (config.isClickZombie)
                {
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, config.clickZombieType, Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition).x).GetComponent<Zombie>();
                }
                if (config.isClickZombie2)
                {
                    CreateZombie.Instance.SetZombie(Mouse.Instance.theMouseRow, config.clickZombieType, Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition).x).GetComponent<Zombie>().SetMindControl(100);
                }
                if (config.isClickPlant)
                {
                    CreatePlant.Instance.SetPlant(Mouse.Instance.theMouseColumn, Mouse.Instance.theMouseRow, config.clickPlantType);
                }
            }
        }

    }

    [HarmonyPatch(typeof(CardUI), "Update")]
    class PatchCardUI
    {
        [HarmonyPrefix]
        static void Prefix(CardUI __instance)
        {
            if (config.isCardNoCD)
            {
                __instance.CD = __instance.fullCD;
            }
        }
    }

    [HarmonyPatch(typeof(GloveMgr), "Update")]
    class PatchGloveMgr
    {
        [HarmonyPrefix]
        static void GloveCD(GloveMgr __instance)
        {
            if (config.isGloveNoCD)
            {
                __instance.CD = __instance.fullCD;
            }
        }

    }

    [HarmonyPatch(typeof(HammerMgr), "Update")]
    class PatchHammerMgr
    {
        [HarmonyPrefix]
        static void Prefix(HammerMgr __instance)
        {
            if (config.isHammerNoCD)
            {
                __instance.CD = __instance.fullCD;
            }

        }
    }


    [HarmonyPatch(typeof(Plant), "PlantShootUpdate")]
    class PatchPlant
    {
        [HarmonyPrefix]
        static void fastPlantShoot(Plant __instance)
        {
            if (config.isFastShoot)
            {
                __instance.thePlantAttackCountDown = 0;
            }

        }
    }

    [HarmonyPatch(typeof(GameLose), "OnTriggerEnter2D")]
    class PatchGameLose
    {
        [HarmonyPrefix]
        static bool fastPlantShoot(Plant __instance)
        {
            if (config.isIgnoreZombieIn)
            {
                return false;
            }
            return true;

        }
    }

    [HarmonyPatch(typeof(CreatePlant), "SetPlant")]
    class PatchCreatePlant
    {

        [HarmonyPrefix]
        static bool Prefix(ref int newColumn, ref int newRow, ref int theSeedType,ref bool isFreeSet)

        {
            
            if (config.isFreePlant)
            {
                isFreeSet = true;
            }
            if (config.isRandomPlant)
            {
                int rand = UnityEngine.Random.Range(0, plantTypesArray.Length);
                theSeedType = (int)plantTypesArray[rand];
            }

            if (config.plantMode == 1 || config.plantMode >= 4)
            {
                return true;
            }
            if (config.plantMode == 2)
            {
                config.plantMode = 1;
                for (int i = 0; i < GameAPP.board.GetComponent<Board>().rowNum; i++)
                {

                    CreatePlant.Instance.SetPlant(newColumn, i, theSeedType);
                }
                config.plantMode = 2;
            }
            else if (config.plantMode == 3)
            {
                config.plantMode = 1;
                for (int i = 0; i < GameAPP.board.GetComponent<Board>().rowNum; i++)
                {
                    for (int j = 0; j < GameAPP.board.GetComponent<Board>().columnNum; j++)
                    {
                        CreatePlant.Instance.SetPlant(j, i, theSeedType);
                    }
                }
                config.plantMode = 3;
            }
            return false;
        }

    }

    [HarmonyPatch(typeof(Zombie), "TakeDamage")]
    class PatchZombie
    {
        [HarmonyPrefix]
        static void Prefix(ref int theDamage)
        {
            if (config.isBulletKill)
            {
                theDamage = 99999999;
            }
        }

    }

    [HarmonyPatch(typeof(Plant), "TakeDamage")]
    class PatchPlant3
    {
        [HarmonyPrefix]
        static void Prefix(ref int damage)
        {
            if (config.isPlantWD)
            {
                damage = 0;
            }
        }

    }

    [HarmonyPatch(typeof(CreateZombie), "SetZombie")]
    class PatchCreateZombie
    {
        [HarmonyPrefix]
        static bool Prefix(ref int theRow, ref int theZombieType, ref float theX, ref bool isIdle)
        {
            if (config.isRandomZombie)
            {
                int rand = UnityEngine.Random.Range(0, ZombieTypesArray.Length);
                theZombieType = (int)ZombieTypesArray[rand];
            }
            if (!config.isCreateZombieRate)
            {
                return true;
            }
            if (config.isCreateZombieRate)
            {
                config.isCreateZombieRate = false;
                for (int i = 0; i < config.createZombieRate; i++)
                {
                    CreateZombie.Instance.SetZombie(theRow,theZombieType,theX,isIdle);
                }
                config.isCreateZombieRate = true;
                return false;
            }
            return true;

        }

    }

    [HarmonyPatch(typeof(CreateBullet), "SetBullet")]
    class PatchCreateBullet
    {
        [HarmonyPrefix]
        static void Prefix(ref int theBulletType)
        {
            if (config.isRandomBullet)
            {
                int rand = UnityEngine.Random.Range(0, bulletTypesArray.Length);
                theBulletType = (int)bulletTypesArray[rand];
            }
        }

    }

    [HarmonyPatch(typeof(Zombie), "Update")]
    class PatchZombieUpdate
    {
        [HarmonyPrefix]
        static void ZombieCold(Zombie __instance)
        {
            if (config.isZombieCold)
            {
                __instance.SetCold(1f);
            }
            if (config.isZombieFreeze)
            {
                __instance.SetCold(1f);
                __instance.SetFreeze(1f);
            }
            if (config.isZombieMindControlled && !__instance.isMindControlled)
            {
                __instance.SetMindControl(100);
            }
            if (config.isZombieGrap)
            {
                __instance.SetGrap(1f);
            }
            if (config.isZombieJalaed)
            {
                __instance.SetJalaed();
            }
        }
    }


}



