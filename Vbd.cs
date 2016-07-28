using UnityEngine;
using System.Collections;

public class Vblood : MonoBehaviour {    

    public GameObject[] SpriteObjects ;
    public int MaxSprites = 16;
    public int spritePadding = 3;
    public int OnGroundTime = 20;// 1 minute
    public int[] timeOfSprite ;
    public int[] spriteX;
    public int[] spriteY;
    void Awake()
    {
        timeOfSprite = new int[MaxSprites];
        spriteX = new int[MaxSprites];
        spriteY = new int[MaxSprites];
        for ( int i = 0; i < MaxSprites;i++ )
        {
            timeOfSprite[i] = 0;
            spriteX[i] = 0;
            spriteY[i] = 0;
            SpriteObjects[i].SetActive(false);
        }
    }

    private int checkSprites()
    {
        int maxTime = -1 ;
        int result = -1;
        for (int i = 0; i < MaxSprites; i++)
        {
            if ( timeOfSprite[i] == 0 )
            {
                return i;// kaliga unna sprite
            }
            if ( timeOfSprite[i] > maxTime )
            {
                maxTime = timeOfSprite[i];
                result = i;
            }
        }
        return result;
    }

    public void HideBlood()
    {
        for (int i = 0; i < MaxSprites; i++)
        {
            timeOfSprite[i] = 0;
            spriteX[i] = 0;
            spriteY[i] = 0;
            SpriteObjects[i].SetActive(false);
        }
    }

    void Update()
    {
        if (ZCode.BloodToggle && (!ZCode.isGameOver) && (!ZCode.isTimeUp) && (!ZCode.isAIdefeated))
        {
            for (int i = 0; i < MaxSprites; i++)
            {
                if (timeOfSprite[i] > 0 && timeOfSprite[i] < 3)
                {
                    float tScale = Time.deltaTime;
                    SpriteObjects[i].transform.localScale += new Vector3(tScale, tScale, tScale);
                }
            }
        }
    }

    public void incSpriteTime()
    {
        if (ZCode.BloodToggle)
        {
            for (int i = 0; i < MaxSprites; i++)
            {
                if (timeOfSprite[i] > 0)
                {
                    timeOfSprite[i]++;
                    if (timeOfSprite[i] > OnGroundTime)
                    {
                        timeOfSprite[i] = 0;
                        SpriteObjects[i].SetActive(false);
                    }
                }
            }
        }    

    }

    public void deployBlood( int bx , int bz )
    {
        if (ZCode.BloodToggle)
        {
            for (int i = 0; i < MaxSprites; i++)
            {
                if ((((spriteX[i] - bx) * (spriteX[i] - bx)) + ((spriteY[i] - bz) * (spriteY[i] - bz))) < spritePadding && timeOfSprite[i] > 0)
                {
                    return;//already a sprite is in action
                }
            }
            int g = checkSprites();// Good Sprite
            if (g < 0)
            {
                return;// Cannot deploy ...sprites are busy 
                // or already a good sprite present in vicinity
            }
            SpriteObjects[g].transform.position = new Vector3(ZCode.ForVBloodDist * bx, 0.1f, ZCode.ForVBloodDist * bz);
            SpriteObjects[g].transform.localScale = new Vector3(2f, 2f, 2f);
            SpriteObjects[g].SetActive(true);
            spriteX[g] = bx;
            spriteY[g] = bz;
            timeOfSprite[g] = 1;
        }
    }

}
