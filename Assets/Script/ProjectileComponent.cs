using System.Collections;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    bool m_isActive = false;

    Vector3 m_direction = new Vector3(1,0,0);
    float m_speed = 0;
    float m_damage = 0;


    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_isActive)
        {
            transform.Translate(m_direction * m_speed * Time.deltaTime);
        }
    }

    internal void FireProjectile(float _speed, float _damage)
    {
        m_isActive = true;
        m_speed = _speed;
        m_damage = _damage;
        StartCoroutine(DisableObjectDelayed(5));
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Projectile hit something");
    }


    private IEnumerator DisableObjectDelayed(float _delay)
    {
        yield return new WaitForSeconds(_delay);
        m_isActive = false;
        gameObject.SetActive(false);
    }
}
