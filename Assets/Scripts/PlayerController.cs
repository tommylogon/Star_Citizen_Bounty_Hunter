using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Transform shipTransform;
    private Ship ship;

    // Start is called before the first frame update
    void Start()
    {
        if (shipTransform == null)
        {
            shipTransform = transform.Find("PlayerShip");
        }
        ship = shipTransform.GetComponent<Ship>();

        
    }

    // Update is called once per frame
    void Update()
    {

        ship.AimShip();
        ship.HandleMovement();
        ship.HandleShooting();
        UpdatePlayerPosition();

    }

    private void UpdatePlayerPosition()
    {
        transform.position = shipTransform.position;
    }



}
