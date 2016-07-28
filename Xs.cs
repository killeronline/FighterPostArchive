using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class XStatics {

    public static int NOP = 0;

    public static int xArmyCap = 30 ;// 60 when profiler

    public static bool isFullMute = false;

}

class KID
{
    public static string LBoardGlobal = "CgkI3p_jiqkeEAIQBw";
    
    public static void Login()
    {
        try
        {
            // PRE-LOGIN
            PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();

            //LOGIN
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    Debug.Log("Login Success");
                }
                else
                {
                    Debug.Log("Login Failed");
                }
            });
        }
        catch{;}
    }

    public static void ShowLBoards()
    {
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            PlayGamesPlatform.Instance.ShowLeaderboardUI(LBoardGlobal);// Triarii
        }
        catch{;}
        //Social.ShowLeaderboardUI();        // To Show All Leader Boards
    }

    public static void PostScoreForLBoardGlobal(int _score)
    {
        try
        {
            if (!(PlayGamesPlatform.Instance.IsAuthenticated()))
            {
                Login();
            }
            Social.ReportScore(_score, LBoardGlobal, (bool success) =>
            {
                if (success)
                {
                    Debug.Log("Submitted Score : " + _score);
                }
                else
                {
                    Debug.Log("Failed Score : " + _score);
                }
            });
        }
        catch{;}

    }

}

class PSCORE
{
    public static string DetailedText = "";

    public static int GetScore( int decision , int UL , int gtime )
    {
        DetailedText = "";
        int tTime = ZCode.ZMINUTES * 60;
        int result = 0;
        if ( decision > 0 )
        {
            int[] dScores = new int[] { 20000, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2] ;
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n"+result+" \n ";
        }
        else if ( decision == 0 )
        {
            int[] dScores = new int[] { 10000, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2];
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n" + result + " \n ";
        }
        else
        {
            int[] dScores = new int[] {  0, (int)(10000 * (1 - (UL / (gtime + 1f)))), (int)(-10000 * (1 - (gtime / (tTime + 1f)))) };
            result = dScores[0] + dScores[1] + dScores[2];
            DetailedText = dScores[0] + "\n" + dScores[1] + "\n" + dScores[2] + "\n\n" + result + " \n ";
        }

        return result;
    }
}



