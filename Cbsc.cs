using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CbScript : MonoBehaviour {
    // Seperators can be removed afterwards    
    public GameObject ScoreBoard;
    public GameObject ScoreBoardNumbers;
    public GameObject SettingsPanel;
    public GameObject QuitPanel;

    public GameObject UnitsPanel2;
    public GameObject DeployObject;
    public GameObject MessagesPanel;
    public GameObject MessagesObject;
    public GameObject TimerObject;
    public GameObject CoinIndicator;
    public GameObject myCostIndicator;
    public GameObject ArmyCountIndicator;
    public GameObject ReplayButtonObject;
    public GameObject AtckButtonObject;
    public GameObject ResultObject;
    public GameObject GloriusObject;
    public GameObject DefeatObject;
    public GameObject DrawObject;
    public Sprite AtckSprite;
    public int S____________0;// Sepetaror Field in Editor
    public GameObject P1_Info;
    public GameObject P2_Info;
    public GameObject P3_Info;
    public GameObject P4_Info;
    public int S____________1;// Sepetaror Field in Editor
    public GameObject[] CountDisplayTexts ;
    public GameObject[] CountDisplayBacks;
    public GameObject[] MinusObjects;
    public int S____________2;// Sepetaror Field in Editor
    public GameObject[] PlayerNameTexts;
    public int S____________3;// Sepetaror Field in Editor
    public GameObject[] PlayerHealthObjects;
    public int S____________4;// Sepetaror Field in Editor        
    public bool HUD_toggle;
    public bool Sound_toggle;
    public bool Music_toggle;
    public bool Settings_toggle;
    public bool Quit_toggle;
    public bool is_bigmatch;
    public SSound sfx;
    private float msgRemoveTime = 3f;
    private int msgTimeStart = 0;
    private int ArmyCap;
    private int msgShowTime = 2;
    void Start()
    {
        ArmyCap = XStatics.xArmyCap;
        DeployObject.SetActive(false);
        ZCode.BloodToggle = true;
        HUD_toggle = true;
        Sound_toggle = true;
        Music_toggle = true;
        Settings_toggle = false;
        SettingsPanel.SetActive(Settings_toggle);
        Quit_toggle = false; 
        QuitPanel.SetActive( Quit_toggle);
        
        ScoreBoard.SetActive(false);
        ResultObject.SetActive(false);
        GloriusObject.SetActive(false);
        DefeatObject.SetActive(false);
        DrawObject.SetActive(false);

        sfx = ZCode.MusicPlayer;

        for (int i = 1; i < ZCode.ZDECK.Length; i++)
        {
            CountDisplayTexts[i].GetComponent<Text>().text = "0";
            CountDisplayBacks[i].SetActive(false);
            MinusObjects[i].SetActive(false);
        }
        msgShow("Select and Deploy");        
        Invoke("Start2", 1f);//1Second to Load the Zcode variables
        InvokeRepeating("HealthUpdates", 1f,1f);//1Second to Load the Zcode variables
        if ( XStatics.NOP >= 2 )//Multiplayer
        {
            AtckButtonObject.SetActive(false);
            ReplayButtonObject.SetActive(false);
        }
    }
    void Start2()
    {
        if (ZCode.ZNOP < 4)
        {            
            is_bigmatch = false;
        }
        else
        {
            is_bigmatch = true;
        }
        P3_Info.SetActive(is_bigmatch);
        P4_Info.SetActive(is_bigmatch);                  
        IntroducingPlayers();
    }

    void IntroducingPlayers()
    {
        // Player Name Texts
        for (int j = 1; j < 5; j++)
        {            
            PlayerNameTexts[j].GetComponent<Text>().text = ZCode.ZPLayerNames[j] ;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ZShowQuit(1);//Pressed Hard Back Button on Mobile
        }
    }

    private bool isResultAnnounced = false ;
    
    void HealthUpdates()
    {        

        if ( ZCode.isGameOver )
        {
            UnitsPanel2.SetActive(false);

            for (int jui = 0; jui < ZCode.ZDECK.Length; jui++)
            {
                ZCode.ZDECK[jui] = 0;
            }

            if ( XStatics.NOP < 2 )
            {
                if (!isResultAnnounced)
                {
                    sfx.HaltAll();
                    sfx.PlayLongClip(5, sfx.ResultFolder[0], 0.5f);                    
                    isResultAnnounced = true;
                }                
                msgShow( "You have been defeated !!!");
                showJudgement(-1);
            }
            else
            {
                //No need to halt bcoz game may continue
                //Team Sounds here
                msgShow(ZCode.ZPLayerNames[ZCode.ZPID] + " got defeated !!!");                
            }            
            msgShowTime = 9999;
        }
        if ( ZCode.isAIdefeated )
        {
            if ( !isResultAnnounced )
            {
                sfx.HaltAll();
                sfx.PlayLongClip(6, sfx.ResultFolder[2], 0.5f);                
                isResultAnnounced = true;
            }
            msgShow(ZCode.ZPLayerNames[2] + " got defeated !!!");            
            showJudgement(1);// we won
            msgShowTime = 9999;
        }
        if ( ZCode.isTimeUp )
        {
            msgShow("Time's Up       ");
            showJudgement(0);
            msgShowTime = 9999;
        }
        // Player Health Objects + Coin Indicator
        // Doing the health of castle Now
        for (int j = 1; j < 5; j++)
        {
            if ( ZCode.ZMaxHealths[j] == 0 )
            {
                // This Player is dead                
                PlayerHealthObjects[j].SetActive(false);
            }
            else if (ZCode.ZMaxHealths[j] > 0)
            {                
                PlayerHealthObjects[j].transform.localScale = new Vector3( ZCode.ZHealths[j]/ (float)ZCode.ZMaxHealths[j], 1f, 1f);
            }
            else
            {                
                // now maxhealth is likely -9999
                // Yet to complete a loop and then we can get maxhealths 
            }
            PlayerNameTexts[j].GetComponent<Text>().text = ZCode.ZPLayerNames[j];
        }
        int myCost = 0;
        for ( int jui = 0; jui < ZCode.ZDECK.Length; jui++)
        {
            myCost += (ZCode.ZDECK[jui] * ZCode.ZUnitCosts[jui]);
        }
        CoinIndicator.GetComponent<Text>().text = ZCode.ZCOINS +"" ;
        myCostIndicator.GetComponent<Text>().text = myCost > 0 ? "(" + myCost + ")" : "";
        int PlayerAllUnitCountSum = 0;
        for ( int i44 = 1; i44 < ZCode.ZTUNITS+1 ; i44++)
        {
            PlayerAllUnitCountSum += ZCode.ZCurrentUnitCount[ZCode.ZPID, i44];
        }
        ArmyCountIndicator.GetComponent<Text>().text = PlayerAllUnitCountSum+"/"+XStatics.xArmyCap;

        // Timer Display
        int totalseconds = ZCode.ZMINUTES * 60;
        totalseconds -= ZCode.ZGTIME;
        int dispMinutes = totalseconds / 60;
        int dispSeconds = totalseconds % 60;        
        TimerObject.GetComponent<Text>().text = dispMinutes+" : "+ (dispSeconds<10?"0":"") + "" + dispSeconds ;        
        
        if (!ZCode.ZDepButtonToggle)
        {
            int showDeployCount = 0 ;
            for (int i = 1; i < ZCode.ZDECK.Length; i++)
            {
                if (ZCode.ZDECK[i] == 0)// After server response is echoed back
                {
                    CountDisplayTexts[i].GetComponent<Text>().text = "0";
                    CountDisplayBacks[i].SetActive(false);
                    MinusObjects[i].SetActive(false);
                }
                else
                {
                    showDeployCount += ZCode.ZDECK[i] ;// Some of units are selected
                }
            }
            /*
            if ( showDeployCount >= 3 )
            {
                DeployObject.SetActive(true);
            }            
            else
            {
                DeployObject.SetActive(false);
            }
            */
        }
        if (!ZCode.isGameOver)
        {
            msgHide();//just will hide msg when counter is more than 3 seconds
        }

    }

    private void showJudgement( int result )
    {
        ResultObject.SetActive(true);
        // +1 win
        // -1 Loss
        // 0 draw
        if (result == 1)
        {
            GloriusObject.SetActive(true);
        }
        else if ( result == -1 )
        {
            DefeatObject.SetActive(true);
        }
        else
        {
            DrawObject.SetActive(true); 
        }
        ZCode.isJudgeMentDone = true;
        // +1 win
        // -1 Loss
        // 0 draw
        int scored = PSCORE.GetScore(result, ZCode.GPG_UL , ZCode.GPG_GTIME );
        summaryScore(PSCORE.DetailedText);
        KID.PostScoreForLBoardGlobal( scored );
    }

    public void ZHideBlood()
    {
        ZCode.BloodToggle = !ZCode.BloodToggle;
        if ( !ZCode.BloodToggle)
        {
            ZCode.SpillScript.HideBlood();
        }
    }

    public void ZHideHud()
    {
        HUD_toggle = !HUD_toggle;
        P1_Info.SetActive(HUD_toggle);
        P2_Info.SetActive(HUD_toggle);
        P3_Info.SetActive(is_bigmatch && HUD_toggle );
        P4_Info.SetActive(is_bigmatch && HUD_toggle );
        MessagesObject.SetActive(HUD_toggle);
    }

    public void ZSoundMute()
    {        
        Sound_toggle = !Sound_toggle;
        ZCode.isAudioAllowed = Sound_toggle;        
    }

    public void ZMusicMute()
    {        
        Music_toggle = !Music_toggle;
        ZCode.BGMusicAdjustment(Music_toggle);
    }

    public void ZHideSettings()
    {
        Settings_toggle = !Settings_toggle;
        SettingsPanel.SetActive(Settings_toggle);
    }

    public void ZQuitReally( int yesNo )
    {
        if ( yesNo == 0 )
        {
            QuitPanel.SetActive(false);
        }
        else
        {
            Invoke("quitToHome",0.75f);
        }
    }

    private void quitToHome()
    {
        SceneManager.LoadScene("AHomeScreen");
    }

    public void ZReplay()
    {
        Invoke("replayBattle", 0.3f);
    }

    private void replayBattle()
    {
        SceneManager.LoadScene("ABattle");
    }

    private void ZShowQuitWithDelay()
    {
        Quit_toggle = true;
        QuitPanel.SetActive(Quit_toggle);
    }

    public void ZShowQuit( int _hardButton )
    {
        if ( _hardButton == 1 )
        {
            Quit_toggle = false;
            QuitPanel.SetActive(Quit_toggle);
            Invoke("ZShowQuitWithDelay",0.2f);            
        }
        else
        {
            Quit_toggle = !Quit_toggle;
            QuitPanel.SetActive(Quit_toggle);
        }                
    }

    public void ZRemoveCard( int _wid )
    {
        AddRemoveCard(_wid, -1);// Remove from deck
    }

    public void ZChooseCard(int _wid)
    {
        AddRemoveCard(_wid, 1);// adding to deck
    }

    private void AddRemoveCard( int _wid , int addCount )         
    {                
        int myCost = 0;
        int myUnitsInDeck = 0 ;
        for (int jui = 0; jui < ZCode.ZDECK.Length; jui++)
        {
            myCost += (ZCode.ZDECK[jui] * ZCode.ZUnitCosts[jui]);
            myUnitsInDeck += ZCode.ZDECK[jui] ;
        }                        
        myUnitsInDeck += addCount; // For this unit
        myCost += ( addCount * ZCode.ZUnitCosts[_wid]);

        if ( ZCode.ZCOINS < myCost )
        {
            msgShow("Not Enough Reinforcements");            
            return;
        }

        if ( (ZCode.ZAlive + myUnitsInDeck) > ArmyCap)// Upper Mark for controlling Maxunits
        {
            msgShow("Max Unit Limit Reached !!!");
            return;
        }

        CoinIndicator.GetComponent<Text>().text = ZCode.ZCOINS + "";
        myCostIndicator.GetComponent<Text>().text = myCost > 0 ? "(" + myCost + ")" : "";//InstantUpdate
        MessagesPanel.SetActive(false);//Remove Past Messages
        ZCode.ZDECK[_wid] += (addCount) ;        

        if ( ZCode.ZDECK[_wid] > ZCode.ZMAXGROUP || ZCode.ZDECK[_wid] < 0)//Clipped to zero
        {
            ZCode.ZDECK[_wid] = 0;
        }

        int myPostUnitsInDeck = 0;
        for (int jui = 0; jui < ZCode.ZDECK.Length; jui++)
        {            
            myPostUnitsInDeck += ZCode.ZDECK[jui];
        }
        if ( myPostUnitsInDeck <= 0 )
        {
            ZCode.ZDepButtonToggle = false;
        }
        else
        {
            ZCode.ZDepButtonToggle = true;
        }

        if (ZCode.ZDECK[_wid] > 0)
        {
            CountDisplayTexts[_wid].GetComponent<Text>().text = ZCode.ZDECK[_wid] + "";
            CountDisplayBacks[_wid].SetActive(true);
            MinusObjects[_wid].SetActive(true);
        }
        else
        {
            CountDisplayTexts[_wid].GetComponent<Text>().text = "0";// oka round vesadu
            CountDisplayBacks[_wid].SetActive(false);
            MinusObjects[_wid].SetActive(false);            
        }        
    }

    public void ZAttackMove()
    {
        ZCode.triggerAttack = !ZCode.triggerAttack;        
        if ( ZCode.triggerAttack)
        {
            AtckButtonObject.GetComponent<Image>().overrideSprite = AtckSprite;
        }        
        else
        {
            AtckButtonObject.GetComponent<Image>().overrideSprite = null;
        }
    }

    public void ZDeploy()
    {
        //print(ZCode.ZDECK[0] +":"+ ZCode.ZDECK[1]+":"+ ZCode.ZDECK[2] + ":" + ZCode.ZDECK[3]);        
        msgShow("click on screen to deploy");
        ZCode.ZDepButtonToggle = true;        
    }

    public void msgShow( string msg )
    {
        MessagesPanel.SetActive(true);
        msgTimeStart = 0;
        MessagesObject.GetComponent<Text>().text = msg;        
    }

    public void summaryScore( string summary )
    {        
        ScoreBoard.SetActive(true);
        ScoreBoardNumbers.GetComponent<Text>().text = summary ;
    }

    public void msgHide()
    {        
        if (msgTimeStart > msgShowTime )
        {
            MessagesPanel.SetActive(false);
            msgTimeStart = 0;
        }
        else
        {
            msgTimeStart++;            
        }
    }
}
