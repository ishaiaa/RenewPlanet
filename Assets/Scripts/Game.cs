using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Regions
{
    None,
    North,
    NorthWest,
    SouthWest,
    South,
    East,
    Central,
    Mazowsze
}

public enum CollisionState
{
    Static,
    Unrestricted,
    Touching,
    Blocked
}

public struct ColorPicker
{
    public static Color white   = new Color(1f, 1f, 1f, 1f);
    public static Color black   = new Color(0f, 0f, 0f, 1f);
    public static Color alpha   = new Color(0f, 0f, 0f, 0f);

    public static Color red     = new Color(1f, 0f, 0f, 1f);
    public static Color green   = new Color(0f, 1f, 0f, 1f);
    public static Color blue    = new Color(0f, 0f, 1f, 1f);

    public static Color yellow  = new Color(1f, 1f, 0f, 1f);
    public static Color cyan    = new Color(0f, 1f, 1f, 1f);
    public static Color magenta = new Color(1f, 0f, 1f, 1f);


    public static Color orange = new Color(1f, 0.5f, 0f, 1f);

    public static Color Translucent(Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }   
}

public static class Game
{
    public static bool trueValue = true;
}

public class ObjectData
{
    public string name;
    public string description;

    //Efficiency Levels (Always 1-3)[1-default level, 2,3-upgrades]
    //affects production, costs and emmision
    public int efficiencyLevel;
    public int maxEfficiencyLevel;
    public EfficiencyLevel[] efficiencyLevels;
    public float[] efficiencyUpgradesCost;
    public Sprite[] levelSprites;

    //Safety Levels (Usually 1-3, sometimes just 1)[1-default level, 2,3-upgrades]
    //reduces emission
    public int safetyLevel;
    public int maxSafetyLevel;
    public SafetyLevel[] safetyLevels;
    public float[] safetyUpgradesCost;



    //defines whether the build is operative or not
    public BuildState buildState;
    public Sprite[] stateSprites; //states for building/upgrading/demolition
    public float actionTime;      //time left for finishing an operation



    //deconstruction
    public float demolitionCost;
    public float demolitionTime;

}

public enum BuildState
{
    Contstruction,
    Working,
    Upgrade,
    Demolition
}

public class EfficiencyLevel
{
    //Build info
    public int level;
    public float cost;
    public float timeCost;
    public string name;
    public string description;

    //Functional Info
    public float emmision;                                  //CO2 emmision

    public float profit;                                    //generated profit
    public float operativeCost;                             //cost of running

    public float energyProduction;                          //produced energy
    public float energyConsumption;                         //consumed energy

    public Dictionary<Resources, float> resourceProduction;  //produced resources
    public Dictionary<Resources, float> resourceConsumption; //consumed resources
}

public class SafetyLevel
{
    //Build info
    public int level;
    public float cost;
    public float timeCost;
    public string name;
    public string description;

    //Functional Info
    public float emmisionReduction; //CO2 emmision reduction
}

public static class ResourceList
{
    public static Resource coal = new Resource()
    {
        name = "Wêgiel Kamienny",
        description = "Wungiel"
    };

    public static Resource lignite = new Resource()
    {
        name = "Wêgiel Brunatny",
        description = "Wungiel Braun"
    };
}

public struct Resource
{
    public string name;
    public string description;
}

