using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(BlinkColorOnHit))]
public class EnemyShield : MonoBehaviour
{
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;


    // Start is called before the first frame update
    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true;      // This will not yet compile

        if (transform.parent == null) { return; }
        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();
        if (shieldParent != null) {
            shieldParent.AddProtector(this);
        }
    }


    /// Called by another Enemyshield to join the protectors of this EnemyShield
    ///<param name="shieldChild">The EnmeyShield that will protect this</param>

    public void AddProtector(EnemyShield shieldChild)
    {
        protectors.Add(shieldChild);
    }
    public bool isActive
    {
        get { return gameObject.activeInHierarchy; }
        private set { gameObject.SetActive(value); }
    }
    public float TakeDamage(float dmg)
    {
        // Can we pass damage to a protector EnemyShield?
        foreach (EnemyShield es in protectors)
        {
            if (es.isActive)
            {
                dmg = es.TakeDamage(dmg);
                // If all damage was handled, return 0 damagee
                if (dmg == 0) return 0;
            }
        }
        blinker.SetColors();        // This will appear underlined in red for now
        health -= dmg;
        if (health <= 0)
        {
            // Deactivate this EnemyShield GameObject
            isActive = false;
            // Return any damage that was not absorbed by this EnemyShieldd
            return -health;
        }
        
        return 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
