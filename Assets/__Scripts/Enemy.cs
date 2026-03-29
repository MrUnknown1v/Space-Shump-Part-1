using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f; // The movement speed is 10m/s

    public float fireRate = 0.3f; // Seconds/shot (unused)

    public float health = 10; // Damage needed to destroy this enemy

    public int score = 100; // Points ; earned for destroying this

    public float powerUpDropChance = 1f;
    

    protected bool calledShipDestroyed= false;

    protected BoundsCheck bndCheck; // private to protected
    

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }
    public Vector3 pos
    {
        get
        {
            return this.transform.position;
        }
        set
        {
            this.transform.position = value;
        }
    }


    // Update is called once per frame
    void Update()
    {
        Move();

        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offDown))
        {
            Destroy(gameObject);
        }
        // if (!bndCheck.isOnScreen)
        // {
        //     if(pos.y < bndCheck.camHeight - bndCheck.radius)
        //     {
        //         //We're offf the bottom so destroy this GameObject
        //         Destroy(gameObject);
        //     }
        // }
    }

    //Check whether thsi Enemy has gone off the bottom of the screen
    

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed* Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if(p != null)
        {
            if (bndCheck.isOnScreen)
            {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                if (health <= 0)
                {
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                Destroy(this.gameObject); // Destroy this Enemy GameObject
                }
                
                
            }
            Destroy(otherGO); // Destroy the Projectile
            

        }else
        {
            Debug.Log("Enemy hit by non-ProjectileHero: "+otherGO.name);
        }
    }       
    
}
