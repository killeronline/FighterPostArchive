using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    // add scenes in build
    public GameObject P2 ;
    public GameObject P4 ;
    public GameObject LoadingPanel;
    public GameObject ProgressImage;
    public GameObject SPButton;
    public GameObject MPButton;
    public GameObject BetaMessage;
    public AudioSource IntroSound;    
    public GameObject SoundIcon;
    public AudioSource RunningHorseSound;
    public GameObject theRunner;
    public Sprite MuteIcon;

    private float totalloadtime;
    private float loadtime;
    private string _sceneName;
    private bool toggleP2P4;    

    void Awake()
    {
        P2.SetActive(false);
        P4.SetActive(false);
        BetaMessage.SetActive(false);
        LoadingPanel.SetActive(false);
        totalloadtime = 200;
        loadtime = 0;
        _sceneName = "";
        toggleP2P4 = false;
        checkSound();
    }

    void LoadBar()
    {
        if ( loadtime < totalloadtime )
        {
            loadtime += ((int)Random.Range(1,20)) ;
            if (loadtime >= totalloadtime)
            {
                loadtime = totalloadtime;
            }            
            ProgressImage.transform.localScale = new Vector3( loadtime/totalloadtime , 1f, 1f);
            Invoke("LoadBar", 0.05f );
        }
        else
        {            
            SceneManager.LoadScene(_sceneName);
        }
    }

    private void checkSound()
    {
        if (XStatics.isFullMute)
        {
            SoundIcon.GetComponent<Image>().overrideSprite = MuteIcon;
            IntroSound.Pause();
            RunningHorseSound.Stop();            
        }
        else
        {
            SoundIcon.GetComponent<Image>().overrideSprite = null;
            IntroSound.Play();
        }
    }

    public void ToggleMute()
    {
        XStatics.isFullMute = !XStatics.isFullMute;
        checkSound();    
    }
    
	public void ChangeScene( int _nop )
    {
        if (_nop > 0)
        {
            IntroSound.volume = 0f;
            IntroSound.Stop();
            RunningHorseSound.volume = 0f;
            RunningHorseSound.Stop();     
                   
            theRunner.GetComponent<JLife>().isStayIdleWhenLoadingBar = true;
            theRunner.GetComponent<JLife>().onScreenRunner();

            SPButton.SetActive(false);
            MPButton.SetActive(false);
            P2.SetActive(false);
            P4.SetActive(false);
            BetaMessage.SetActive(false);
            XStatics.NOP = _nop;
            _sceneName = "ABattle";
            LoadingPanel.SetActive(true);
            ProgressImage.transform.localScale = new Vector3(0f, 1f, 1f);
            Invoke("LoadBar", 0.1f);
        }
    }

    public void ToggleThe2P4P()
    {
        toggleP2P4 = !toggleP2P4;// the toggle line
        P2.SetActive( toggleP2P4 );
        P4.SetActive( toggleP2P4 );
        BetaMessage.SetActive(toggleP2P4);
    }

    public void GPGShowLeaderBoards()
    {
        KID.ShowLBoards();
    }

    public void GPGPostRandomScore()
    {
        int randScore = UnityEngine.Random.Range(1,30);
        KID.PostScoreForLBoardGlobal(randScore);
    }
}
