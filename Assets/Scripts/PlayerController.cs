using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController
{
    public static bool godMode;
    public Transform shipTransform;
    public Ship ship;

    public event Action OnDeath;
    public event Action OnShoot;
    public event Action<Ship.Direction> OnMove;
    public event Action<bool> OnBrake;
    public event Action<Vector3, Vector3> OnAim;
    

    public State playerState;


    // Start is called before the first frame update
    void Start()
    {
        playerState = State.Alive;
        if (shipTransform == null)
        {
            shipTransform = transform.Find("PlayerShip");
        }
        if(shipTransform.TryGetComponent(out Ship PlayerShip))
        {
            shipTransform.tag = "Player";
            ship = PlayerShip;
            ship.Setup(gameObject);
            ship.OnDeath += PlayerController_OnDeath;
        }
        else
        {
            Debug.Log("Player does not have a ship");
        }
        
        
    }

    private void PlayerController_OnDeath()
    {
        playerState = State.Dead;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState == State.Alive)
        {

            HandleInput();

            OnAim?.Invoke(UtilsClass.GetMouseWorldPosition(), new Vector3(0,0,0));

            UpdatePosition();

            if (Input.GetMouseButton(0))
            {
                OnShoot?.Invoke();
            }
        }



    }

    public void UpdatePosition()
    {
        if(shipTransform != null)
        {

            transform.position = shipTransform.position;
        }
    }

    public void HandleInput()
    {
        

            if (Input.GetKey(KeyCode.W))
            {
            OnMove?.Invoke(Ship.Direction.Forward);
            }


            if (Input.GetKey(KeyCode.A))
            {
            OnMove?.Invoke(Ship.Direction.Left);
            }


            if (Input.GetKey(KeyCode.D))
            {
            OnMove?.Invoke(Ship.Direction.Right);
            }


            if (Input.GetKey(KeyCode.S))
            {
            OnMove?.Invoke((Ship.Direction.Backward));
            }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnBrake?.Invoke( true);
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            OnBrake.Invoke( false);

        }
        
    }
    



}
