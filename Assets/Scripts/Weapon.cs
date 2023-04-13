using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Weapon : MonoBehaviour
{
    enum DamageType
    {
        Ballistic, 
        Energy,
        Explosive
    }

    public int damage;
    public float lifeTime;
    public int size;
    public Resource ammo;
    public int fireRate;
    public float nextFireTime;
    public bool recharges;
    public int rechargeRate;
    public int bulletSpeed;
    public string targetTag;
    [SerializeField] private Radar autoAimArea;
    [SerializeField] GameObject target;

    public GameObject bulletPrefab;

    public SoundManager.Sound weaponSound;

    public void Start()
    {
        autoAimArea.OnRadarStay += AutoAimArea_OnRadarStay;
        autoAimArea.OnRadarExit += AutoAimArea_OnRadarExit;
    }

    private void AutoAimArea_OnRadarExit(Collider2D obj)
    {
        target = null;
    }

    private void AutoAimArea_OnRadarStay(Collider2D racarContact)
    {
        if(racarContact.tag == targetTag)
        {
            target = racarContact.gameObject;

        }
    }


    public void Shoot(string newTargetTag)
    {
        targetTag = newTargetTag; 
        if(ammo.GetValue() > 0 && Time.time > nextFireTime)
        {
            Vector3 predictedAimDirection = transform.right;
            if (target != null)
            {

                Vector3 targetVeloicty = target.GetComponent<Rigidbody2D>().velocity;
                Vector3 predictedTargetPosition = target.transform.position + targetVeloicty * Time.deltaTime*30;

                predictedAimDirection = (predictedTargetPosition - transform.position).normalized;


            }
            
           
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            SoundManager.PlaySound(weaponSound);
            bullet.GetComponent<Bullet>().Setup(predictedAimDirection, this, targetTag);
             
            nextFireTime = Time.time + 1f / fireRate;
            ammo.UpdateValue(-1);
            ammo.ResetRefillTimer();
        }
        
    }
}
