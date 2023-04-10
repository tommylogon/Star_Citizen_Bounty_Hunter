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
        Energy
    }

    public int damage;
    public float lifeTime;
    public int size;
    public int ammo;
    public int fireRate;
    public float nextFireTime;
    public bool recharges;
    public int rechargeRate;
    public int bulletSpeed;

    public GameObject bulletPrefab;

    public SoundManager.Sound weaponSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        if(ammo > 0 && Time.time > nextFireTime)
        {
            Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
            Vector3 aimDirection = (mousePosition - transform.position).normalized;


            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            SoundManager.PlaySound(weaponSound);
            bullet.GetComponent<Bullet>().Setup(aimDirection, this);
             
            nextFireTime = Time.time + 1f / fireRate;
        }
        
    }
}
