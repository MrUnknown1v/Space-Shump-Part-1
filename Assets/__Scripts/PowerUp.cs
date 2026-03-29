using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]
public class PowerUp : MonoBehaviour
{

    [Header("Inscribed")]
    //Thhis is an unusual but hand use of Vector2s

    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 rotMinMax = new Vector2(15,90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 driftMinMax= new Vector2(.25f,2);
    public float lifeTime = 10;
    public float fadeTime = 4;

    [Header("Dynamic")]
    public eWeaponType _type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond;
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        //Find the Cube Reference
        cube = transform.GetChild(0).gameObject;
        //Find the TextMesh and other components
        letter= GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        //Set random Velocity
        Vector3 vel = Random.onUnitSphere; //Get Random XYZ velocity
        vel.z = 0;
        vel.Normalize();

        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        transform.rotation = Quaternion.identity;


        rotPerSecond = new Vector3(Random.Range(rotMinMax[0],rotMinMax[1]),Random.Range(rotMinMax[0],rotMinMax[1]),Random.Range(rotMinMax[0],rotMinMax[1]) );

        birthTime = Time.time;
    }
    

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }
        if (u > 0)
        {
            Color c= cubeMat.color;
            c = cubeMat.color;
            c.a = 1f - u;       // Set the alpha of PowerCube to 1-u
            cubeMat.color = c;

            // Fade the Letter too, just not too much
            c = letter.color;
            c.a = 1f - (u * 0.5f);      // Set the alpha of the letter to 1-(u/2)
            letter.color = c;
        }
        if (!bndCheck.isOnScreen)
        {
            // If the PowerUp has drifted entirely off screen, destroy it
            Destroy(gameObject);
        }



        
    }
    public eWeaponType type{get { return _type; }set { SetType(value); }}


    public void SetType(eWeaponType wt)
    {
        // Grab the WeaponDefinition from Main
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor;       // Set the color of PowerCube
        // letter.color = def.color;        // Colorize the letter too
        letter.text = def.letter;       // Set the letter that is shown
        _type = wt;      // Finally actually set the type
    }


    
    public void AbsorbedBy(GameObject target)
    {
        Destroy(this.gameObject);
    }



}
