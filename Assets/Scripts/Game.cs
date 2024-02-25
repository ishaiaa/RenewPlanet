using System;
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

public enum ShopCategory
{
    None,
    Powerplants,
    Mines,
    Nature,
    Infrastructure
}

public enum CollisionState
{
    Static,
    Unrestricted,
    Touching,
    Blocked
}

public enum CursorMode
{
    Idle,
    Action,
    Info,
    Move,
    Build,
    Restricted,
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
    public static string FormatCash(double cash)
    {
        return string.Format("{0:N0}", cash);
    }

    public static double UnixTimeStamp()
    {
        DateTime currentTime = DateTime.UtcNow;
        double unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();
        return unixTime;
    }

    public static string FormatTime(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"hh") + "h " + time.ToString(@"mm") + "m " + time.ToString(@"ss") + "s";
    }

    public static string FormatTime(double seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"hh") + "h " + time.ToString(@"mm") + "m " + time.ToString(@"ss") + "s";
    }

    public static string FormatUnits(double watts)
    {
        //Peta
        if (watts >= 1000000000000000)
            return (watts / 1000000000000000).ToString("F2") + "P";
        //Tera
        if (watts >= 1000000000000)
            return (watts / 1000000000000).ToString("F2") + "T";
        //Giga
        if (watts >= 1000000000)
            return (watts / 1000000000).ToString("F2") + "G";
        //Mega
        if (watts >= 1000000)
            return (watts / 1000000).ToString("F2") + "M";
        //Kile
        if (watts >= 1000)
            return (watts / 1000).ToString("F2") + "k";
        return watts.ToString("F2");
    }
}

[Serializable]
public class ObjectData
{
    public string name;
    public string description;

    public bool deconstructable;

    //Efficiency Levels (Always 1-3)[1-default level, 2,3-upgrades]
    //affects production, costs and emmision
    public int efficiencyLevel;
    public int maxEfficiencyLevel;
    public EfficiencyLevel[] efficiencyLevels;
    public Sprite[] levelSprites;


    //defines whether the build is operative or not
    public BuildState buildState;
    public Sprite[] stateSprites; //states for building/upgrading/demolition
    public double finishTime;      //timestamp for when the build will be finished



    //deconstruction
    public double demolitionCost;
    public double demolitionTime;

    public ObjectData() { }
    public ObjectData(ObjectData clone)
    {
        name = clone.name;
        description = clone.description;

        deconstructable = clone.deconstructable;
        efficiencyLevel = clone.efficiencyLevel;
        maxEfficiencyLevel = clone.maxEfficiencyLevel;
        efficiencyLevels = clone.efficiencyLevels;
        levelSprites = clone.levelSprites;

        buildState = clone.buildState;
        stateSprites = clone.stateSprites;
        finishTime = clone.finishTime;    

        demolitionCost = clone.demolitionCost;
        demolitionTime = clone.demolitionTime;
    }
}

public enum BuildState
{
    Contstruction,
    Working,
    Upgrade,
    Demolition
}

public enum ResearchState
{
    Locked,
    Researchable,
    Researching,
    Researched
}

[Serializable]
public class EfficiencyLevel
{
    //Build info
    public int level;
    public double cost;
    public double timeCost;
    public string name;
    public string description;

    public ResearchState researchState;
    public double researchCost;
    public double researchTime;
    public double researchFinishTime;

    //Functional Info
    public double emmision;                                  //CO2 emmision

    public double profit;                                    //generated profit
    public double operativeCost;                             //cost of running

    public double energyProduction;                          //produced energy
    public double energyConsumption;                         //consumed energy

    public ResourceProduction[] resourceProduction;  //produced resources
    public ResourceProduction[] resourceConsumption; //consumed resources

    public EfficiencyLevel() { }
}

[Serializable]
public class SafetyLevel
{
    //Build info
    public int level;
    public double cost;
    public double timeCost;
    public string name;
    public string description;

    //Functional Info
    public double emmisionReduction; //CO2 emmision reduction

    public SafetyLevel() { }
}

public enum ResourceType
{
    Coal,
    Uranium,
    Gas
}

public static class ResourceList
{
    public static Resource coal = new Resource()
    {
        name = "W�giel Kamienny",
        description = "Wungiel",
        resourceType = ResourceType.Coal
    };

    public static Resource uranium = new Resource()
    {
        name = "Uran",
        description = "From jurij ovsienko",
        resourceType = ResourceType.Uranium
    };

    public static Resource gas = new Resource()
    {
        name = "Gaz Ziemny",
        description = "Gaziemny",
        resourceType = ResourceType.Gas
    };
}


[Serializable]
public class Resource
{
    public string name;
    public string description;
    public ResourceType resourceType;

    public Resource() {}
}


[Serializable]
public class ResourceStorage
{
    public Resource resource;
    public int quantity;

    public ResourceStorage() { }

    public ResourceStorage(Resource r, int q)
    {
        resource = r;
        quantity = q;
    }
}

[Serializable]
public class ResourceProduction
{
    public ResourceType resource;
    public int quantity;

    public ResourceProduction() { }

    public ResourceProduction(ResourceType r, int q)
    {
        resource = r;
        quantity = q;
    }
}


