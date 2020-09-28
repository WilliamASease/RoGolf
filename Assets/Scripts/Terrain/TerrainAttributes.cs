﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainAttributes
{
    public const float SIMULATED_FRICTION = 5.0E-2f;
    public const float SIMULATED_BOUNCE = 0.3f;

    private TerrainType tee;
    private TerrainType green;
    private TerrainType fairway;
    private TerrainType rough;
    private TerrainType bunker;
    private TerrainType water;

    public TerrainAttributes() {
        //                     friction,bounce,lieRate,lieRange
        tee = new TerrainType(    5.0E-2f, 0.3f, 0.99f, 0.02f);
        green = new TerrainType(  5.0E-2f, 0.3f, 0.99f, 0.02f);
        fairway = new TerrainType(5.0E-2f, 0.3f, 0.99f, 0.02f);
        rough = new TerrainType(  3.0E-2f, 0.3f, 0.80f, 0.16f);
        bunker = new TerrainType( 1.0E-2f, 0.1f, 0.70f, 0.20f);
        water = new TerrainType(  1.0E-9f, 0.0f, 0.20f, 0.10f); 
    }

    /// <summary>
    /// Gets the bounce of a surface given a RaycastHit.
    /// </summary>
    public float GetBounce(RaycastHit terrainHit)
    {
        return GetTerrainType(terrainHit).GetBounce();
    }

    /// <summary>
    /// Gets the friction of a surface given a RaycastHit.
    /// </summary>
    public float GetFriction(RaycastHit terrainHit)
    {
        return GetTerrainType(terrainHit).GetFriction();
    }

    /// <summary>
    /// Gets the TerrainType of a surface given a string name.
    /// </summary>
    public TerrainType GetTerrainType(string name)
    {
        switch (name[0])
        {
            case 'B':
                return bunker;
            case 'F':
                return fairway;
            case 'G':
                return green;
            case 'R':
                return rough;
            case 'W':
                return water;
            default:
                throw new InvalidOperationException("Cannot not get TerrainType for name " + name);
        }
    }

    /// <summary>
    /// Gets the TerrainType of a surface given a RaycastHit.
    /// </summary>
    public TerrainType GetTerrainType(RaycastHit terrainHit) { return GetTerrainType(terrainHit.transform.gameObject.name); }

    public bool OnGreen(RaycastHit terrainHit) { return terrainHit.transform.gameObject.name[0] == 'G'; }

    public TerrainType GetTeeTerain() { return tee; }
    public TerrainType GetGreenTerain() { return green; }
    public TerrainType GetFairwayTerain() { return fairway; }
    public TerrainType GetRoughTerain() { return rough; }
    public TerrainType GetBunkerTerain() { return bunker; }
    public TerrainType GetWaterTerain() { return water; }
}
