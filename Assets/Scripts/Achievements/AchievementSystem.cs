using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure these match the achievement identifiers in Steamworks
// Currently all platforms must have the same set of achievements, but
// the system could be extended later to support different sets.
public enum AchievementID : int {
    ACH_WIN_ONE_GAME,
    ACH_WIN_100_GAMES,
    ACH_HEAVY_FIRE,
    ACH_TRAVEL_FAR_ACCUM,
    ACH_TRAVEL_FAR_SINGLE,
};

// Most of the time you will refer to achievements by their ID, this structure is
// just to store data about them for things like displaying to the user what 
// achievements they have. You can add a field to track the icon related to an achievement,
// to display it in-game, but that might be platform-specific, so I would recommend creating
// a subclass that holds the extra data in that case
public class Achievement_t {
    public AchievementID m_eAchievementID;
    public string m_strName;
    public string m_strDescription;
    public bool m_bAchieved;
    
    // Make sure you have all your achievements listed here and that the IDs match, achievements will be
    // looked up in this table by their ID, so it's important that the Achievement object matches the associated ID.
    // Names and descriptions will be filled in from Steam when loaded, so you can just put
    // fallbacks in the constructors here.
    public static readonly Dictionary<AchievementID, Achievement_t> sm_Achievements = new Dictionary<AchievementID, Achievement_t> {
        { AchievementID.ACH_WIN_ONE_GAME, new Achievement_t(AchievementID.ACH_WIN_ONE_GAME, "Winner", "") },
        { AchievementID.ACH_WIN_100_GAMES, new Achievement_t(AchievementID.ACH_WIN_100_GAMES, "Champion", "") },
        { AchievementID.ACH_TRAVEL_FAR_ACCUM, new Achievement_t(AchievementID.ACH_TRAVEL_FAR_ACCUM, "Interstellar", "") },
        { AchievementID.ACH_TRAVEL_FAR_SINGLE, new Achievement_t(AchievementID.ACH_TRAVEL_FAR_SINGLE, "Orbiter", "") },
    };

    /// <summary>
    /// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
    /// </summary>
    /// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
    /// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
    /// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
    public Achievement_t(AchievementID achievementID, string name, string desc) {
        m_eAchievementID = achievementID;
        m_strName = name;
        m_strDescription = desc;
        m_bAchieved = false;
    }
}

// I wrote this as a platform agnostic API so that you can support non-Steam platforms relatively easily.
// Alternatively you can implement a 'Default' platform where you handle this stuff yourself (like storing achievements
// on the user's hard drive or something)
public interface IAchievementSystem
{
    // Initialize the platform's achievement system. Do not initialize the platform API itself here
    // e.g. SteamManager.cs handles connecting to steam, this runs after and just sets up callbacks
    public void Init();
    // Handle updating achievement system state
    public void Update();
    
    // Update the currently stored achievement data with the latest from the platform's servers
    public void FetchAchievementData();
    
    // Do you really need me to explain what this does?
    public void UnlockAchievement(AchievementID achievementID);
    
    // These just returns whatever achievement data is currently stored,
    // so you should call FetchAchievementData to get the latest data
    // from the platform service
    public Achievement_t GetAchievementInfo(AchievementID achievementID);
    public bool IsAchievementUnlocked(AchievementID achievementID);
    
    // This is debug only! This will clear not just achievements but also all
    // stats related to this game on your account!
    public void ResetAchievements();
}
