using System;
using System.Collections.Generic;
using UnityEngine;

public enum SideType
{
    right,
    left,
    up,
    down,
    none,
}

public enum TimeScaleType
{
    None,
    Normal,
    Dialog,
}

public enum PanelsType
{
    Start,
    End,
    Win,
    Fail,
    Crash,
    Meta,
    SlowMotion,
    FullScreenDialog,
    HUD,
    Garage,
    Chapter,
    General,
    TopHud,
    CameraWindow,
    Settings,
    SelectCar,
}



public enum ColorType
{
    Yellow,
    Red,
    Green,
    Blue,
}




public enum ButtonState
{
    Normal,
    NormalForVideo,
    Disabled,
    NormalForInterstitial,
}

public enum ButtonPressState
{
    Down,
    Up
}


public enum FaceType
{
    RK,
    T,
    Skid,
    L,
    B,
    None,
    SJ,
    Buster,
    Bot1,
    Bot2,
    Bot3,
    Bot4,
    Police1,
    Police2,
    Police3,
    Police4,
    FatherMiguel,
}

public enum DialogPresetType
{
    Green1,
    Red1,
    Blue1,
    Transparent1,
}

public enum LevelType
{
    SmashAllCars,
    AtoB,
    Tutorial,
    GetScore,
    Special,
    None,
    Run,
}

public enum ItemType
{
    headColl_Money,
    standalone_Money,
    headColl_Parts,
    standalone_Parts,
    standalone_Fuel,
}

public enum ColliderType
{
    Mesh,
    MeshConvex,
    Box,
    Sphere,
    None,
}

public enum ConditionOnEnable
{
    None,
    ifEngineDmg,
    ifSteeringDmg,
    ifAttackDmg,
    ifBrakesDmg,
    ifStructureDmg,
}
public enum ConditionOnUpdate
{
    None,
    ifEngineDmg,
    ifSteeringDmg,
    ifAttackDmg,
    ifBrakesDmg,
    ifStructureDmg,
}

public class LayerMaskID
{
    public static readonly int Default = LayerMask.NameToLayer("Default");
    public static readonly int Vehicle = LayerMask.NameToLayer("Vehicle");
    public static readonly int Track = LayerMask.NameToLayer("Track");
    public static readonly int PartsCollider = LayerMask.NameToLayer("PartsCollider");


   
    public struct BitLayer
    {
        public static readonly int Default = 1 << LayerMask.NameToLayer("Default");
        public static readonly int Track = 1 << LayerMask.NameToLayer("Track");
        public static readonly int Vehicle = 1 << LayerMask.NameToLayer("Vehicle");
    }
}

public static class Globals
{
    private static readonly Color yellowIcon = new Color(255,220,0);
    private static readonly Color redIcon = new Color(255,0,0);
    public static Color black => new Color(0.0f, 0.0f, 0.0f, 0f);


    public static Color GetIconColor(ColorType colorType)
    {
        switch (colorType)
        {
            case ColorType.Yellow:
                return yellowIcon;
            case ColorType.Red:
                return redIcon;
                break;
            case ColorType.Green:
                break;
            case ColorType.Blue:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(colorType), colorType, null);
        }

        return default;
    }
}

public static class GlobalString
{
    public static string IsReloading = "IsReloading";
    public const string Player = "Player";
    public const string Bot = "Bot";
    public const string Track = "Track";
    public const string Obstacle = "Obstacle";
    public const string Untagged = "Untagged";
    public const string Enemy = "Enemy";
    public const string OnlyCollide = "OnlyCollide";
    public const string Bonus = "Bonus";
    public const string Attacker = "Attacker";
    public const string MetaCamera = "MetaCamera";
    public const string LEVEL = "LEVEL ";
    public const string EmptyString = "";
    public const string PlayerSpawn = "PlayerSpawn";
    public const string Special = "Special";
    public const string Attack = "Attack";
    public const string Magazine = "Magazine";
    public const string Ammo = "Ammo";
    public const string Bullet = "Bullet";
    public const string Icon = "Icon";
    public const string Cost = "Cost";
    public const string Locked = "Locked";
    public const string Gold = "Gold";
    public const string SimpleBullet = "SimpleBullet";
    public const string SimpleRocket = "SimpleRocket";
    public const string Debug = "Debug";

    
    public const string QualitySettingsIdentifier = "DefaultQualitySettings";
    
    //Sound

    
    // HCSDK
    
     
    // FIREBASE
    
    //ADS MANAGER

    // PREFS
    public const string user_lastPlayedLevelIndex = "user lastPlayedLevelIndex";
    public const string getLoopCount = "getLoopCount";
    
    // PREFS TUTORIAL

    // Animator
    public const string Enable = "Enable";
    public const string Enabling = "Enable";
    public const string Disable = "Disable";
    public const string RightToCenter = "RightToCenter";
    public const string LeftToCenter = "LeftToCenter";
    public const string CenterToRight = "CenterToRight";
    public const string CenterToLeft = "CenterToLeft";
    
    // ======= LANGUAGE TEXT 
    // PartType
    public const string Body = "Body";
    public const string Door = "Door";
    public const string Bumper = "Bumper";
    public const string Hood = "Hood";
    public const string Tailgate = "Tailgate";
    public const string Fender = "Fender";

    public const string Repair = "Repair ";
    //numbers
    public const string zeroNumber = "0";
    public const string oneNumber = "1";
    public const string twoNumber = "2";
    public const string threeNumber = "3";
    public const string fourNumber = "4";
    public const string fiveNumber = "5";
    public const string sixNumber = "6";
    public const string sevenNumber = "7";
    public const string eightNumber = "8";
    public const string nineNumber = "9";

    
    public static string GetNumber(int number)
    {
        switch (number)
        {
            case 0:
                return zeroNumber;
            case 1:
                return oneNumber;
            case 2:
                return twoNumber;
            case 3:
                return threeNumber;
            case 4:
                return fourNumber;
            case 5:
                return fiveNumber;
            case 6:
                return sixNumber;
            case 7:
                return sevenNumber;
            case 8:
                return eightNumber;
            case 9:
                return nineNumber;
            default:
                return number.ToString();
        }
    }
}


public static class AnimatorTagHash
{
    public static readonly int Enable = Animator.StringToHash("Enable");
    public static readonly int Enabling = Animator.StringToHash("Enabling");
    public static readonly int Disable = Animator.StringToHash("Disable");
    public static readonly int Disabling= Animator.StringToHash("Disabling");
    public static readonly int RightToCenter = Animator.StringToHash("RightToCenter");
    public static readonly int LeftToCenter = Animator.StringToHash("LeftToCenter");
    public static readonly int CenterToRight = Animator.StringToHash("CenterToRight");
    public static readonly int CenterToLeft = Animator.StringToHash("CenterToLeft");
}

