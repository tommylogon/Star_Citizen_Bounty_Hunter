using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController 
{
    public event Action OnDeath;
    public event Action OnShoot;
    public event Action<Ship.Direction> OnMove;
    public event Action<bool> OnBrake;
    public event Action<Vector3> OnAim;
}
public enum State
{
    Alive,
    Dead
}
