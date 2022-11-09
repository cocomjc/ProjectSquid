using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WeaponComponent : MonoBehaviour
{
    private float aimAngle;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameModulator gameModulator;
    float timePassed = 0f;

    public void Aim(Vector2 aimDirection)
    {
        // Make the weapon face the direction pointed by the joystick
        if (aimDirection != Vector2.zero)
        {
            aimAngle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
        }
    }

    private void Update()
    {
        transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
        timePassed += Time.deltaTime;
        if (timePassed > gameModulator.projectileFireRate)
        {
            timePassed = 0f;
            FireProjectile();
        }
    }

    public void FireProjectile()
    {
        ProjectileComponent projectile = GetComponent<ObjectPool>().GetPooledObject().GetComponent<ProjectileComponent>();
        projectile.gameObject.SetActive(true);
        projectile.transform.position = firePoint.transform.position;
        projectile.transform.rotation = transform.rotation;
        projectile.transform.Rotate(0, 0, 90);
        projectile.FireProjectile(gameModulator.projectileSpeed, gameModulator.projectileDamage);
    }
}
