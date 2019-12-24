using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBodyPart : MonoBehaviour
{
    [SerializeField]
    private int hitPointScoreValue,multiplier = 0;
    public int HitPointScore { get { return hitPointScoreValue; } }
    public int ScoreMultiplier { get { return multiplier; } }
}
