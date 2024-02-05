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

public class RegionManager : MonoBehaviour
{
    public Dictionary<Regions, string> regionsNames = new Dictionary<Regions, string>()
    {
        {Regions.None,       "Brak" },
        {Regions.North,      "P�nocny" },
        {Regions.NorthWest,  "P�nocno-Zachodni" },
        {Regions.SouthWest,  "Po�udniowo-Zachodni" },
        {Regions.South,      "Po�udniowy" },
        {Regions.East,       "Wschodni" },
        {Regions.Central,    "Centralny" },
        {Regions.Mazowsze,   "Mazowiecki" },
    };

    public Regions selectedRegion = Regions.None;
    public Collider2D regionCollider = null;
    public Collider2D edgeCollider = null;
}
