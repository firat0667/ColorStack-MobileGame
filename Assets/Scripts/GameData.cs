using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class GameData 
{
    private int score_Count;
    
    public int ScoreCount
    {
        get
        {
            return score_Count;
        }
        set
        {
            score_Count = value;
        }
    }
}
