using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    private Weapon weapon;
    public string TargetTag;
    

    public void Setup(Vector3 ShootDirection, Weapon weaponInfo, string targetTag)
    {
        direction = ShootDirection;
        weapon = weaponInfo;
        TargetTag = targetTag;
        transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVector(ShootDirection));
        Destroy(gameObject, weapon.lifeTime);
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * weapon.bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == TargetTag)
        {
            Ship hitShip = collision.gameObject.GetComponent<Ship>();
            if (hitShip != null)
            {
                
                hitShip.Damage(weapon.damage);

            }
            Destroy(gameObject);
        }
    }
}
