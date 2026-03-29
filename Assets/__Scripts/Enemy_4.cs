using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EnemyShield))]


public class Enemy_4 : Enemy
{
    [Header("Enemy_4 Inscribed Fields")]
    public float duration =  4;
    private EnemyShield[] allShields;
    private EnemyShield thisShield;

    private Vector3 p0, p1;
    private float timeStart;
    // Start is called before the first frame update
    void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();
        
        
        p0=p1= pos;
        Debug.Log("Initial pos: " + pos);

        InitMovement();
    }
    void InitMovement()
    {
        p0 = p1;

        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
         Debug.Log("camWidth: " + bndCheck.camWidth + " camHeight: " + bndCheck.camHeight);
        p1.x = Random.Range(-widMinRad,widMinRad);
        p1.y = Random.Range(-hgtMinRad,hgtMinRad);
        Debug.Log("New p1: " + p1);

        if(p0.x*p1.x >0 && p0.y*p1.y > 0)
        {
            if(Mathf.Abs(p0.x)> Mathf.Abs(p0.y))
            {
                p1.x*=-1;
            }
            else
            {
                p1.y *=-1;
            }
        }
       

        timeStart = Time.time;
    }

    public override void Move()
    {
        Debug.Log("Moving Enemy_4");

        float u = (Time.time - timeStart) /duration;
         Debug.Log("u: " + u);

        if (u >= 1)
        {
            InitMovement();
            u=0;
        }
        Debug.Log("p0: " + p0 + " p1: " + p1);
       
        u = u - 0.15f * Mathf.Sin(u* 2 * Mathf.PI);
        pos = (1-u)*p0 +u*p1;
        
        

    }
    private void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        // Make sure this was hit by a ProjectileHero
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            // Destroy the ProjectileHero regardlesss of bndCheck.isOnScreen
            Destroy(otherGO);

            // Only damage this Enemy if it's on screen
            if (bndCheck.isOnScreen)
            {
                // Find the GameObject of this Enemy_4 that was actually hit
                GameObject hitGO = coll.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                {
                    hitGO = coll.contacts[0].otherCollider.gameObject;
                }

                // Get the damage amount from the Main WEAP_DICT
                float dmg = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                // Find the EnemyShield that was hit (if there was one)
                bool shieldFound = false;
                foreach (EnemyShield es in allShields)
                {
                    if (es.gameObject == hitGO)
                    {
                        es.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }
                if (!shieldFound) { thisShield.TakeDamage(dmg); }

                // If thisShield is still active, then it has not been destroyed
                if (thisShield.isActive) { return; }

                // This ship was destroyed so tell Main about it
                if (!calledShipDestroyed)
                {
                    Main.SHIP_DESTROYED(this);
                    calledShipDestroyed = true;
                }

                // Destroy this Enemy_4
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
        }
    }
}
