using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SSound : MonoBehaviour {

    public AudioSource masterMixer;
    public AudioSource[] efxSources ;
    public AudioSource DecisionSource ;
    public AudioSource[] DecisionSourceLong ;    
    public int[] DecisionSourceLongInd;
    public int[] lCount ;
    public int SoundsLastCall;
    public bool shouldMasterPlay;
    public GameObject FxCheckBox;
    public GameObject MusicCheckBox;

    public void Awake()
    {
        SoundsLastCall = 0;
        lCount = new int[efxSources.Length];
        for (int ik = 0; ik < efxSources.Length; ik++)
        {
            efxSources[ik].clip = null;
            efxSources[ik].playOnAwake = false;
            efxSources[ik].volume = 0f;
        }
        DecisionSourceLongInd = new int[DecisionSourceLong.Length];
        for (int ik = 0; ik < DecisionSourceLongInd.Length; ik++)
        {
            DecisionSourceLongInd[ik] = 0;
        }
        shouldMasterPlay = true;        
        Invoke("playBG", 1f);
    }

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;    

    public AudioClip[] ArrowFolder;
    public AudioClip[] AttackFolder;
    public AudioClip[] CavalryFolder;
    public AudioClip[] EDrumsFolder;
    public AudioClip[] DrumsFolder;
    public AudioClip[] DyingFolder;    
    public AudioClip[] MarchingFolder;
    public AudioClip[] ResultFolder;
    public AudioClip[] SpawnFolder;

    private bool killingAudioForNow = false ;// For Debugging Purposes

    public void SlowPass()
    {
        masterMixer.volume = 0.1f;
        if (shouldMasterPlay) { masterMixer.Play(); }
    }

    public void BGMusicBool( bool bgShouldPlay )
    {
        shouldMasterPlay = bgShouldPlay;
        if ( shouldMasterPlay )
        {
            masterMixer.Play();
        }
        else
        {
            masterMixer.Stop();
        }
    }

    public void playBG()
    {
        if (XStatics.isFullMute)
        {
            FxCheckBox.GetComponent<Toggle>().isOn = false;
            MusicCheckBox.GetComponent<Toggle>().isOn = false;         
            // The toggle script will call the toggle function automatically
        }
        if (shouldMasterPlay)
        {            
            masterMixer.Play();
        }
        else
        {            
            masterMixer.Stop();
        }
    }

    public void HaltMoreAudioSources()
    {
        for (int ik = 0; ik < efxSources.Length; ik++)
        {
            efxSources[ik].Stop();           
        }
        for (int sDLongInd = 0; sDLongInd < DecisionSourceLong.Length; sDLongInd++)
        {            
            DecisionSourceLong[sDLongInd].Stop();            
        }
    }

    public void HaltAll()
    {        
        masterMixer.Stop();
        HaltMoreAudioSources();
        Invoke("SlowPass",3f);
    }


    public void PlayOneClip( AudioClip solo , float _volume )
    {
        if (killingAudioForNow)// Killing the Audio
        {
            return;
        }
        if (ZCode.isAudioAllowed)
        {            
            DecisionSource.Stop();
            DecisionSource.clip = solo;            
            DecisionSource.PlayOneShot( solo , _volume );
        }
    }

    public void PlayLongClip( int _channel,AudioClip solo, float _volume)
    {
        if (killingAudioForNow)// Killing the Audio
        {
            return;
        }
        if (ZCode.isAudioAllowed)
        {
            /*
            bool settled = false;
            for ( int sDLongInd = 0; sDLongInd < DecisionSourceLong.Length; sDLongInd++ )
            {
                if ( ( !settled && !DecisionSourceLong[sDLongInd].isPlaying) || DecisionSourceLongInd[sDLongInd] < 0 )
                {
                    DecisionSourceLong[sDLongInd].Stop();
                    DecisionSourceLong[sDLongInd].clip = solo;
                    DecisionSourceLong[sDLongInd].PlayOneShot(solo, _volume);
                    DecisionSourceLongInd[sDLongInd] = 2;
                    settled = true;                    
                }
                else
                {
                    DecisionSourceLongInd[sDLongInd]--;
                }
            }
            */            
            if ( !DecisionSourceLong[_channel].isPlaying )
            {
                DecisionSourceLong[_channel].Stop();//FailSafe
                DecisionSourceLong[_channel].clip = solo;
                DecisionSourceLong[_channel].PlayOneShot(solo, _volume);              
            }
        }
    }

    public void RandomizeSfx(AudioClip[] _clips, float _volume)
    {
        if ( killingAudioForNow )// Killing the Audio
        {
            return;
        }
        if (ZCode.isAudioAllowed)
        {
            int randomIndex = Random.Range(0, _clips.Length);
            float randomPitch = Random.Range(lowPitchRange, highPitchRange);
            int randomIK = Random.Range(0, efxSources.Length - 1);
            efxSources[randomIK].Stop();
            efxSources[randomIK].pitch = randomPitch;
            efxSources[randomIK].volume = _volume;
            efxSources[randomIK].clip = _clips[randomIndex];
            efxSources[randomIK].PlayOneShot( _clips[randomIndex] , _volume );
            //print("Playing On:" + randomIK);
            SoundsLastCall = 0;
        }
        else
        {
            SoundsLastCall++;
            if ( SoundsLastCall == 1 )
            {
                for (int ik = 0; ik < efxSources.Length; ik++)
                {
                    efxSources[ik].Stop();
                }
            }
        }
    }

}
