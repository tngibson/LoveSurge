using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I've already added one of these to your title scene, so keep in mind that it's not gonna work
// if you load directly into another scene
public class AchievementComponent : MonoBehaviour
{
    public static IAchievementSystem AchievementSystem;
    public void OnEnable()
    {
        // We really only need one of these
        if (AchievementSystem != null)
        {
            Destroy(this);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        
        // If you want to support other platforms, you can ifdef this out and set
        // it based on the current platform, i.e. #if PLATFORM_PS5 AchievementSystem = new PS5Achievements()
        AchievementSystem = new SteamAchievements();
        
        AchievementSystem.Init();
    }

    public void Update()
    {
        AchievementSystem.Update();
        
        // TODO: Remove this once you're ready to set the game up with your own achievements
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (AchievementSystem.IsAchievementUnlocked(AchievementID.ACH_WIN_ONE_GAME))
            {
                AchievementSystem.ResetAchievements();
            }
            else
            {
                AchievementSystem.UnlockAchievement(AchievementID.ACH_WIN_ONE_GAME);
            }
        }
    }
}
