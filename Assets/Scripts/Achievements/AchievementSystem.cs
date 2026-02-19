using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure these match the achievement identifiers in Steamworks
// Currently all platforms must have the same set of achievements, but
// the system could be extended later to support different sets.
public enum AchievementID
{
    NEW_ACHIEVEMENT_1_0,
    NEW_ACHIEVEMENT_1_1,
    NEW_ACHIEVEMENT_1_2,
    NEW_ACHIEVEMENT_1_3,
    NEW_ACHIEVEMENT_1_4,
    NEW_ACHIEVEMENT_1_5,
    NEW_ACHIEVEMENT_1_6,
    NEW_ACHIEVEMENT_1_7,
    NEW_ACHIEVEMENT_1_8,
    NEW_ACHIEVEMENT_1_9,
    NEW_ACHIEVEMENT_1_10,
    NEW_ACHIEVEMENT_1_11,
    NEW_ACHIEVEMENT_1_12,
    NEW_ACHIEVEMENT_1_13,
    NEW_ACHIEVEMENT_1_14,
    NEW_ACHIEVEMENT_1_15,
    NEW_ACHIEVEMENT_1_16,
    NEW_ACHIEVEMENT_1_17
}

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
    public static readonly Dictionary<AchievementID, Achievement_t> sm_Achievements =
        new Dictionary<AchievementID, Achievement_t>
    {
    { AchievementID.NEW_ACHIEVEMENT_1_0, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_0, "Achievement 0", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_1, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_1, "Achievement 1", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_2, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_2, "Achievement 2", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_3, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_3, "Achievement 3", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_4, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_4, "Achievement 4", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_5, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_5, "Achievement 5", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_6, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_6, "Achievement 6", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_7, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_7, "Achievement 7", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_8, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_8, "Achievement 8", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_9, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_9, "Achievement 9", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_10, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_10, "Achievement 10", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_11, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_11, "Achievement 11", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_12, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_12, "Achievement 12", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_13, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_13, "Achievement 13", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_14, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_14, "Achievement 14", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_15, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_15, "Achievement 15", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_16, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_16, "Achievement 16", "") },
    { AchievementID.NEW_ACHIEVEMENT_1_17, new Achievement_t(AchievementID.NEW_ACHIEVEMENT_1_17, "Achievement 17", "") },
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
    public void UnlockAchievement(AchievementID achievementID)
    {
        if (!m_bStatsValid)
            return;

        if (IsAchievementUnlocked(achievementID))
            return;

        Achievement_t achievement = Achievement_t.sm_Achievements[achievementID];

        achievement.m_bAchieved = true;

        SteamUserStats.SetAchievement(achievementID.ToString());

        Debug.LogFormat("Granting achievement - {0}", achievementID);

        m_bStoreStats = true;
    }

    // These just returns whatever achievement data is currently stored,
    // so you should call FetchAchievementData to get the latest data
    // from the platform service
    public Achievement_t GetAchievementInfo(AchievementID achievementID);
    public bool IsAchievementUnlocked(AchievementID achievementID);
    
    // This is debug only! This will clear not just achievements but also all
    // stats related to this game on your account!
    public void ResetAchievements();
}
