using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BodyParts
{
    Arm = 1,
    Hand = 2,
    ArmFull = 3,
    Leg = 4,
    Foot = 8,
    legFull = 12,
    Head = 16,
    Torso = 32
}
