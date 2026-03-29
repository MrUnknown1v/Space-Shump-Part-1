using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Main : MonoBehaviour
{
    static private Main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;  //Array of Enemy prefabs
    public float  enemySpawnPerSecond =  0.5f; // # of Enemies  spawned/seconds

    public float enemyInsetDefault = 1.5f; // Inset from the sides
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;

    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.blaster, eWeaponType.spread, eWeaponType.shield
    };

    


    private BoundsCheck bndCheck;

    void Awake()
    {
        S = this;

        // Set bndCheck to reference the BoundsCheck component on this 
        // GameObject
        bndCheck = GetComponent<BoundsCheck>();

        //Invoke SpawnEnemy() once ( in 2 seconds , based on default values)
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type]= def;
        }
    }
    
    
    public void SpawnEnemy()
    {
        //If spawnEnemies is false, skip to the next invoke of spawn enemy
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f/ enemySpawnPerSecond);
            return;
        }
        // Pick aa random Enemy prfab to instantiate
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>( prefabEnemies[ndx]);

        // position the Enemy above the screen with a random x position
        
        float enemyInset = enemyInsetDefault;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            
            enemyInset = Mathf.Abs( go.GetComponent<BoundsCheck>().radius);
        }

        //Set the initial position for the spawned Enemy
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        // Invoke SpawnEnemy() again
        Invoke( nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }
    void DelayedRestart()
    {
        // Invoke the Restart() method in gameRestartDelay seconds
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        //Reload __Scene_0 to restart the game
        //"__Scene_0" below starts with 2 underscores and ends with a zero
        SceneManager.LoadScene("__Scene_2");

    }

    static public void HERO_DIED()
    {
        S.DelayedRestart();
    }
    ///<summary> Static funtion that gets a WeaponDefinition from WEAP_DICT static
    ///  protected field of the Main Class
    /// </summary>
    /// <returns> The WeaponDefinition, or if there is no WeaponDefintion with
    /// the eWeaponType passed in, returns a new WeaponDefinition with
    /// eWeaponType of eWeaponType.none.</returns>
    /// <param name="wt"> The eWeapon type of the desired WeaponDefinition</param>
    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt)) {
            return(WEAP_DICT[wt]);
        }
        return(new WeaponDefinition());
    }

    ///<summary>
    /// Called by  an Enemy ship whenever it is destroyed. It sometimes creates
    /// a PowerUp in place of the destroyed ship.
    /// </summary>
    /// <param name="e"> The Enemy that was destroy</param>
    static public void SHIP_DESTROYED(Enemy e)
    {
        //Pontentially generate a PowerUp
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];

            // Spawn a PowerUp
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            // Set it to the proper WeaponType
            pUp.SetType(pUpType);

            // Set it to the position of the destroyed ship
            pUp.transform.position = e.transform.position;
        }
    } 
}
