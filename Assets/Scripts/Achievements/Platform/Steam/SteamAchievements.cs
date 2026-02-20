using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : IAchievementSystem
{
	protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    
	private CGameID m_GameID = new CGameID(3612670);
	
	private bool m_bShouldRequestStats;
	private bool m_bStatsValid;
	
	private bool m_bStoreStats;
	private bool m_bFetchAfterStoring;
	
	public void Init()
	{
		if (SteamManager.Initialized) {
			string name = SteamFriends.GetPersonaName();
			Debug.LogFormat("Steam logged in w/ user {0}", name);
			
			m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
			m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
			m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

			m_bShouldRequestStats = true;
			m_bStatsValid = false;
			m_bStoreStats = false;
			m_bFetchAfterStoring = false;
		}
	}

	public void Update()
	{
		if (!SteamManager.Initialized)
			return;
	
		// Only need to fetch once, so don't do this if we're gonna fetch after we store
		if (m_bShouldRequestStats && !m_bFetchAfterStoring) {
			FetchAchievementData();
		}

		if (!m_bStatsValid)
			return;

		if (m_bStoreStats)
		{
			bool bSuccess = SteamUserStats.StoreStats();
			// If this failed, we never sent anything to the server, try
			// again later.
			m_bStoreStats = !bSuccess;
		}
		
		if (m_bFetchAfterStoring) {
			FetchAchievementData();
		}
	}

	public void FetchAchievementData()
	{
		SteamUserStats.RequestUserStats(SteamUser.GetSteamID());
		m_bShouldRequestStats = false;
		m_bFetchAfterStoring = false;
	}

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

    public Achievement_t GetAchievementInfo(AchievementID achievementID)
	{
		Debug.AssertFormat(Achievement_t.sm_Achievements.ContainsKey(achievementID), 
			"Could not find the achievement ID {0} in the achievement table!", achievementID);
		return Achievement_t.sm_Achievements[achievementID];
	}

	public bool IsAchievementUnlocked(AchievementID achievementID)
	{
		Debug.AssertFormat(Achievement_t.sm_Achievements.ContainsKey(achievementID), 
			"Could not find the achievement ID {0} in the achievement table!", achievementID);
		return Achievement_t.sm_Achievements[achievementID].m_bAchieved;
	}

	public void ResetAchievements()
	{
		// Just in case, this only works in the editor
		#if UNITY_EDITOR
		SteamUserStats.ResetAllStats(true);
		m_bStoreStats = true;
		m_bFetchAfterStoring = true;
	#endif
	}

	private void OnUserStatsReceived(UserStatsReceived_t pCallback) {
		if (!SteamManager.Initialized)
			return;

		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (EResult.k_EResultOK == pCallback.m_eResult) {
				Debug.Log("Received stats and achievements from Steam\n");

				m_bStatsValid = true;

				// load achievements
				foreach (Achievement_t ach in Achievement_t.sm_Achievements.Values) {
					bool ret = SteamUserStats.GetAchievement(ach.m_eAchievementID.ToString(), out ach.m_bAchieved);
					if (ret) {
						ach.m_strName = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "name");
						ach.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "desc");
						SteamUserStats.GetAchievement(ach.m_eAchievementID.ToString(), out var bAchieved);
						ach.m_bAchieved = bAchieved;
					}
					else {
						Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ach.m_eAchievementID + "\nIs it registered in the Steam Partner site?");
					}
				}
			}
			else {
				Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: Our stats data was stored!
	//-----------------------------------------------------------------------------
	private void OnUserStatsStored(UserStatsStored_t pCallback) {
		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (EResult.k_EResultOK == pCallback.m_eResult) {
				Debug.Log("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult) {
				// One or more stats we set broke a constraint. They've been reverted,
				// and we should re-iterate the values now to keep in sync.
				Debug.Log("StoreStats - some failed to validate");
				// Fake up a callback here so that we re-load the values.
				UserStatsReceived_t callback = new UserStatsReceived_t();
				callback.m_eResult = EResult.k_EResultOK;
				callback.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(callback);
			}
			else {
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: An achievement was stored
	//-----------------------------------------------------------------------------
	private void OnAchievementStored(UserAchievementStored_t pCallback) {
		// We may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (0 == pCallback.m_nMaxProgress) {
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else {
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
			}
		}
	}
}

/*public class SteamAchievements_OLD : MonoBehaviour
{
    protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    protected Callback<UserStatsStored_t> m_UserStatsStored;
    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    
    private CGameID m_GameID = new CGameID(480);
	
    private bool m_bShouldRequestStats;
    private bool m_bStatsValid;
	
    private bool m_bStoreStats;
    
    private void OnEnable() {
        if (SteamManager.Initialized) {
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
            m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

            m_bShouldRequestStats = true;
        }
    }

    private void Update()
    {
	    if (!SteamManager.Initialized)
		    return;

	    if (m_bShouldRequestStats) {
		    // Is Steam Loaded? if no, can't get stats, done
			
		    // If yes, request our stats
		    SteamUserStats.RequestUserStats(SteamUser.GetSteamID());

		    // This function should only return false if we weren't logged in, and we already checked that.
		    // But handle it being false again anyway, just ask again later.
		    m_bShouldRequestStats = false;
	    }

	    if (!m_bStatsValid)
		    return;

	    if (m_bStoreStats)
	    {
		    bool bSuccess = SteamUserStats.StoreStats();
		    // If this failed, we never sent anything to the server, try
		    // again later.
		    m_bStoreStats = !bSuccess;
	    }
    }
    
    // To be honest I have no idea if this even works, I can't test it because the overlay doesn't work in the editor
    private void OnGameOverlayActivated(GameOverlayActivated_t pCallback) {
        if(pCallback.m_bActive != 0)
        {
            StartCoroutine(PauseMenu.instance.PauseGame());
        }
        else {
            PauseMenu.instance.ResumeGame();
        }
    }
    
    private void UnlockAchievement(Achievement_t achievement) {
		achievement.m_bAchieved = true;

		// the icon may change once it's unlocked
		//achievement.m_iIconImage = 0;

		// mark it down
		SteamUserStats.SetAchievement(achievement.MEAchievementIDID.ToString());

		// Store stats end of frame
		m_bStoreStats = true;
	}
	
	//-----------------------------------------------------------------------------
	// Purpose: We have stats data from Steam. It is authoritative, so update
	//			our data with those results now.
	//-----------------------------------------------------------------------------
	private void OnUserStatsReceived(UserStatsReceived_t pCallback) {
		if (!SteamManager.Initialized)
			return;

		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (EResult.k_EResultOK == pCallback.m_eResult) {
				Debug.Log("Received stats and achievements from Steam\n");

				m_bStatsValid = true;

				// load achievements
				foreach (Achievement_t ach in Achievement_t.sm_Achievements) {
					bool ret = SteamUserStats.GetAchievement(ach.MEAchievementIDID.ToString(), out ach.m_bAchieved);
					if (ret) {
						ach.m_strName = SteamUserStats.GetAchievementDisplayAttribute(ach.MEAchievementIDID.ToString(), "name");
						ach.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(ach.MEAchievementIDID.ToString(), "desc");
						SteamUserStats.GetAchievement(ach.MEAchievementIDID.ToString(), out var bAchieved);
						ach.m_bAchieved = bAchieved;
					}
					else {
						Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + ach.MEAchievementIDID + "\nIs it registered in the Steam Partner site?");
					}
				}
			}
			else {
				Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: Our stats data was stored!
	//-----------------------------------------------------------------------------
	private void OnUserStatsStored(UserStatsStored_t pCallback) {
		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (EResult.k_EResultOK == pCallback.m_eResult) {
				Debug.Log("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult) {
				// One or more stats we set broke a constraint. They've been reverted,
				// and we should re-iterate the values now to keep in sync.
				Debug.Log("StoreStats - some failed to validate");
				// Fake up a callback here so that we re-load the values.
				UserStatsReceived_t callback = new UserStatsReceived_t();
				callback.m_eResult = EResult.k_EResultOK;
				callback.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(callback);
			}
			else {
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: An achievement was stored
	//-----------------------------------------------------------------------------
	private void OnAchievementStored(UserAchievementStored_t pCallback) {
		// We may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (0 == pCallback.m_nMaxProgress) {
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else {
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
			}
		}
	}
}*/

