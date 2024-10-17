using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle1 : ObstacleBase
{
    public Vector3Int direction;
    public int xChange = 0, zChange = 0;
    public Vector3Int lastTurnPos = new Vector3Int(10000, 10000, 10000);
}
