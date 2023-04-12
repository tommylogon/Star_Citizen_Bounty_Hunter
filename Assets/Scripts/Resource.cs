using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Resource : MonoBehaviour
{
    public enum ResourceType
    {
        Health, Shield, Fuel, Ammo, Energy, Cargo, Cells,
    }
    [SerializeField] private ResourceType type;
    [SerializeField] private float currentValue = 100;
    [SerializeField] private float maxValue = 100;
    [SerializeField] private float minValue = 0;

    [SerializeField] private float regenDelay = 5f;
    [SerializeField] private float regenTimer = 0f;
    [SerializeField] private float regenAmount = 1f;
    [SerializeField] private bool active = true;
    [SerializeField] private bool canDeactivate = false;
    [SerializeField] private bool canRecharge = false;

    private float nextTick;

    public event Action OnEmpty;
    public event Action OnFull;
    public event Action OnRecharge;
    public event Action OnValueChange;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Refill();
    }

    public bool IsActive()
    {
        return active;
    }
    public void SetActive(bool status)
    {
        active = status;
    }

    public bool IsValueBetweenMinAndMax()
    {
        if(currentValue > minValue && currentValue < maxValue)
        {
            
            return true;
        }
        
        return false;
    }

    public float GetValue() { return currentValue; }
    public void UpdateValue(float newValue) { currentValue += newValue;}
    
    public void ResetRefillTimer()
    {
        regenTimer = 0f;
    }

    public void Refill()
    {
        if (currentValue < maxValue && canRecharge)
        {
            
            regenTimer += Time.deltaTime;
            

            

            if (active && regenTimer > regenDelay)
            {
                currentValue += regenAmount;
                regenTimer -= .3f;

            }
            else if (!active)
            {
                currentValue += regenAmount;
                if (currentValue == maxValue)
                {
                    active = true;
                    
                }
            }

            if (currentValue > maxValue)
            {
                currentValue = maxValue;
                
            }
        }
    }
}
