using UnityEngine;
using System.Collections;
using Image = UnityEngine.UI.Image;

public class JLife : MonoBehaviour
{

    #region PreJ
    // Root Folder -8.0.-19.25 , ROT : 40 45 0 
    // RTS MAIN CAM 0,0,z=-60...For Future Changes
    
    public GameObject RoundRings;
    public int mode;
    public Animator anim;
    public float atime;
    public float ptime;
    public int[] miniTypes;
    public string controlParam;
    public int decay;
    public bool is_alive;
    public Color selectionHighlightColor;
    public bool do_once;
    public Color[] pColors;        
    public Transform healthCanvas;
    public Transform healthDisplay;    
    public Camera myCamera;
    public bool isAlphaChangedForHealthBar;
    public bool isOnHomeScreen = false ;
    public AudioSource HorseSound;
    private bool adjustedAttackSeperation = false;
    private int cHealth = int.MaxValue;
    private int cMaxHealth = -1;
    private float height;    
    private int countUp;
    private int sudIdleCounter;
    private bool instantColoring;
    private float distBtwTroops = 3.001f;
    private int _wid;
    private int _pid;
    private int _tid;
    private int _gid;
    private int _isTakingArrowDamage;
    private int _x;
    private int _y;
    private int _nx;
    private int _ny;
    private int _ex;
    private int _ey;
    private int onScreenSeconds = 0;

    void Start()
    {
        mode = 0 ;        
        countUp = 0;
        sudIdleCounter = 0;
        atime = 0f;
        ptime = 0f;
        miniTypes = new int[] { 5, 1, 3, 3, 0, 0, 3 };// IDLE , WALK , ATCK , DEAD , 4, 5, RNGE
        // WALK IS MADE 1 to obtain atleast one good walk mode
        //controlParam = "bmode" ;
        decay = 3 ;
        is_alive = true;
        do_once = true;
        height = 1.85f;        
        anim = this.GetComponent<Animator>();
        anim.SetInteger( controlParam , (0*10)+1); // 01 is the idle-1
        whenNotInUpdate();
        if (!isOnHomeScreen)
        {
            RoundRings.SetActive(true);
            gameObject.name = GetInstanceID().ToString() + "N";
            //this.GetComponent<Renderer>().material.color = pColors[ 2 ];//default Color--Testing Red
            //gameObject.GetComponent<Renderer>().material.color = pColors[2];
            //gameObject.GetComponent<Renderer>().sharedMaterial.color = pColors[2];        
            //this.GetComponent<Material>().color = pColors[2];                
            this.transform.Find("Mesh_Pike").GetComponent<SkinnedMeshRenderer>().material.color = pColors[0];
            instantColoring = true; // for checking the regrouping i set to false         
            changeAlphaForHealthBar();
            healthBarLookAtCamera();
        }
        else
        {
            Invoke("onScreenRunner",1f);
        }
    }
    
    public bool isStayIdleWhenLoadingBar = false;

    public void onScreenRunner()
    {
        onScreenSeconds++;
        if ( onScreenSeconds % 10 == 0 || isStayIdleWhenLoadingBar )
        {
            mode = 0;           
            onScreenSeconds = 0;
        }        
        else
        {
            mode = 1;
        }        
        int randomAction = Random.Range(1, miniTypes[mode] + 1);
        anim.SetInteger(controlParam, (mode * 10) + randomAction); // Running
        whenNotInUpdate();
        if ( mode == 0 )
        {
            HorseSound.Stop();
            Invoke("onScreenRunner",3f);
        }
        else
        {                   
            Invoke("onScreenRunner", 1f);
            Invoke("PlayHorseMusicLately", 1f);
        }                
    }

    private void PlayHorseMusicLately()
    {
        if ( !XStatics.isFullMute)
        {
            HorseSound.Play();
        }        
    }

    public void Die( int ina2 )
    {        
        if (is_alive)
        {
            mode = 3;//DEATH        
            is_alive = false;
            int postMode = Random.Range(1, miniTypes[mode] + 1);
            if (postMode > miniTypes[mode])
            {
                postMode = miniTypes[mode];
            }
            anim.SetInteger(controlParam, (mode * 10) + postMode);
            whenNotInUpdate();                       
        }
        if (ina2 == 1)// full death
        {
            InvokeRepeating("deathDecaying", 1f, 1f);
        }
    }

    public void deathDecaying()
    {
        decay--;        
        if (decay < 0)
        {
            Destroy(gameObject);
        }
    }
    
    public float DNewRotAngle = 180 ;// to deny the continuity check initially
    public int interestAngle = 180;    
    public bool isContinuity = false;

    public void rotationFunction(int _dx, int _dy, int mR)
    {
        double radians = System.Math.Atan2(_dy - _y, _dx - _x);
        int degrees = (int)(radians * 180 / System.Math.PI);
        float curAngle = transform.rotation.eulerAngles.y;
        curAngle = -curAngle;
        float rotAngle = curAngle - degrees;
        //DNewRotAngle = rotAngle < 0 ? -rotAngle : rotAngle;        
        DNewRotAngle = rotAngle;
        interestAngle = ((9 * 360) - (int)DNewRotAngle) % 360;
        if (mR != 1 || 
        ( ! ((40 < interestAngle && interestAngle < 50) || (310 < interestAngle && interestAngle < 320)) ) )
        {
            transform.Rotate(new Vector3(0, rotAngle, 0));
            isContinuity = false;
        }
        else
        {
            if ( 310 < interestAngle && interestAngle < 320 )
            {
                //DNewRotAngle = -DNewRotAngle;
                if (DNewRotAngle < 0)
                {
                    DNewRotAngle = 360 + DNewRotAngle;
                    //print("*c:" + DNewRotAngle);
                }
                else
                {
                    //print("*:" + DNewRotAngle);
                }
            }
            else// 40 to 50
            {
                if ( DNewRotAngle < -360 )
                {
                    DNewRotAngle = 360 + DNewRotAngle;
                    //print("?c" + DNewRotAngle);
                }                
                else
                {
                    //print("?" + DNewRotAngle);
                }                
            }
            isContinuity = true;
        }        
        healthBarLookAtCamera();
    }

    public void movementFunction()// The Walk Checker
    {                
        if ( mode == 1 && _nx != -1 && _ny != -1 && (!(_nx == _x && _ny == _y)))
        {
            float diagFactor = 1;
            if ( (((_nx - _x )*(_nx - _x)) + ((_ny - _y)*(_ny - _y))) == 2 )
            {// diagonal movement
                diagFactor = ZCode.ZROOT2;
            }
            float moveFactor = 1f;// earlier 0.9f
            if ( _wid == 3 )
            {
                moveFactor = 2f;// earlier 1.9f
            }
            Vector3 dest = new Vector3( _nx * distBtwTroops , height , _ny * distBtwTroops );
            Vector3 curr = transform.position ;
            Vector3 d2mini = dest - curr;
            float zimmick;
            //zimmick = 2.5f;
            zimmick = distBtwTroops;
            if (d2mini.magnitude > 0.1f)            
            {
                float moveDistance = zimmick * moveFactor * Time.deltaTime;
                transform.Translate(new Vector3(moveDistance, 0, 0));                
            }
            else
            {                
                suddenModelHalt();   // For halting the unit after reaching the point             
            }
            if ( isContinuity )
            {
                transform.Rotate(new Vector3(0, 2 * DNewRotAngle * Time.deltaTime, 0));
                healthBarLookAtCamera();                
            }
            // for performance can this be inside of magnitude above if
        }
    }

    #endregion preJ

    public void ModeUpdate(int[] _packet)
    {
        _wid = _packet[0];
        _pid = _packet[1];
        _tid = _packet[2];

        mode = _packet[3];

        _x = _packet[4];
        _y = _packet[5];
        _nx = _packet[6];
        _ny = _packet[7];

        if (_nx != -1 && _ny != -1 && (!(_nx == _x && _ny == _y)))
        {
            rotationFunction(_nx, _ny, mode);
        }

        if (_packet[8] == 1)// CRITICAL UPDATE - 1 is the normal update - deemed to be the modeImplement 0 
        {
            transform.position = new Vector3(distBtwTroops * _x, height, distBtwTroops * _y);            
            _gid = _packet[9];
            _isTakingArrowDamage = _packet[10];
            modeImplement(0);

        }
        else
        {
            modeImplement(1);//Immediately Implementing Mode
        }        
    }

    public void modeImplement( int _skip )
    {
        if (is_alive)
        {
            #region DefaultActions

            if ( _wid == 1 )// only for shielded units later swordsmen
            {
                if ( _isTakingArrowDamage == 1 )
                {
                    anim.SetInteger(controlParam, 4 );//0+4 for idle guard action  
                    whenNotInUpdate();
                    _isTakingArrowDamage = 0;
                    return;
                }
            }
            if ( mode == 0 )
            {                
                if ( _pid > 0 )
                {
                    if ( _pid % 2 == 0 )
                    {
                        rotationFunction(_x - 1, _y, mode);
                    }
                    else
                    {
                        rotationFunction(_x + 1, _y, mode);
                    }                    
                }                
            }
            if (_pid > 0 && instantColoring)// When Donated to other player units change color
            {
                if (do_once)
                {
                    this.transform.Find("Mesh_Pike").GetComponent<SkinnedMeshRenderer>().material.color = pColors[_pid];
                    do_once = false;// will remove this code and also manage selection rings if the donation of army is not an idea
                }                
                ManageSelectionRings();
            }

            if (mode == 1 && (_nx == -1 && _ny == -1))
            {
                //should move but cannot move
                mode = 0;// may be this is the ghost movement like object moving but legs straight
                // in skip:0 and countUp%3:1 i think i debugged the idle mode circle , check in new update
                // check with a print statement if this case is coming after aug9
            }            
            
            int postMode = Random.Range(1, miniTypes[mode] + 1);
            if (postMode > miniTypes[mode])
            {
                postMode = miniTypes[mode];
            }                        

            #endregion

            #region Decision Anims

            if ( _skip == 0 )
            {      
                adjustedAttackSeperation = false ;
            }
            else
            {                                
                if ( mode == 0 )
                {                    
                    suddenModelHalt();                                        
                }
                else
                {
                    anim.SetInteger(controlParam, (mode * 10) + postMode);
                    whenNotInUpdate();
                    sudIdleCounter = 0;
                }                

                if ( mode == 2 && !adjustedAttackSeperation)
                {// for archer ..write for diff units basing on seperation dist
                    if ( _wid == 2 )
                    {
                        transform.Translate( new Vector3( distBtwTroops / 4 , 0 , 0 ));
                        adjustedAttackSeperation = true;
                    }
                    if ( _wid == 1 )
                    {
                        transform.Translate(new Vector3(distBtwTroops / 4, 0, 0));//0.75
                        adjustedAttackSeperation = true;
                    }
                }
            }

            #endregion
        }
    }

    #region PostJ

    public void showHealthBar( int[] _data )// Helper Octoman
    {
        if (_data[1] > 0)// I used brilliant skip , when max health == 0 means only to look at camera
        {
            if ( _data[0] < cHealth )
            {
                cHealth = _data[0];
            }
            else
            {
                // Here we skip for dyna arrow rediculous inc of health : thealth
                return;
            }
            float health;
            if ( cHealth <= 0)
            {
                health = 0f;
            }
            else
            {
                health = (_data[0] / (float)_data[1]);
            }
            if (health < 0.40f)
            {
                healthDisplay.GetComponent<Image>().color = Color.red;
            }
            else if (health < 0.70f)
            {
                healthDisplay.GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                healthDisplay.GetComponent<Image>().color = Color.green;
            }
            healthDisplay.localScale = new Vector3(health, 1f, 1f);// health = 0f....1f                            
        }
        else if (_data[1] == 0)
        {
            healthBarLookAtCamera();
        }
        else
        {     
            //special case of cHealth and cMaxHealth updates : move above only in this case      
            //healthDisplay.localScale = new Vector3(0f, 1f, 1f);
        }
    }

    public void changeAlphaForHealthBar()
    {
        Color k23 = healthDisplay.GetComponent<Image>().color;
        k23.a = 255;//actually 255
        healthDisplay.GetComponent<Image>().color = k23;
    }

    public void healthBarLookAtCamera()
    {
        healthCanvas.LookAt(healthCanvas.position + (myCamera.transform.rotation * Vector3.back),
            myCamera.transform.rotation * Vector3.up
            );
    }

    public void suddenModelHalt()
    {
        sudIdleCounter++;
        if (sudIdleCounter%3==1)
        {
            anim.SetInteger(controlParam, (0 * 10) + 1);//01              
            whenNotInUpdate();
            sudIdleCounter = 1; //Similar to 4Mod3 logic modImp skip:0 mode:0 case
        }        
    }

    public void ManageSelectionRings()
    {
        if (ZCode.ZSelectedGID > 0 && ZCode.ZSelectedGID == _gid )
        {
            RoundRings.SetActive(true);
            //this.transform.Find("Mesh_Pike").
            //GetComponent<SkinnedMeshRenderer>()
            //.material.color = selectionHighlightColor ;
        }
        else
        {
            RoundRings.SetActive(false);
        }
    }    

    public void set99()
    {
        anim.SetInteger(controlParam, 99);
    }

    public void whenNotInUpdate()
    {
        Invoke("set99", 0.01f);              
    }

    public void TimeUp()
    {
        anim.Stop();
    }

    #endregion PostJ

}
