using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayStressListener : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StressManager.stressChangedEvent += OnStressChanged;
    }

    private void OnDestroy()
    {
        StressManager.stressChangedEvent -= OnStressChanged;
    }

    private bool AnyStatHasTag(string tag)
    {
        if (Player.instance == null) return false;

        foreach (var statOffset in Player.GetSafeOffsets())
        {
            if (statOffset.HasOffsetTag(tag)) return true;
        }

        return false;
    }

    private void OnStressChanged(object sender, StressEventArgs args)
    {
        // Don't add stat offsets if no player or stress decreased
        if (Player.instance == null)
        {
            Debug.Log("Cannot set stat offsets with no player!");
            return;
        }
        if (args.AmountChanged < 0) return;

        // We pick the index to debuff from a list so that we can remove it from the list when we're done,
        // so that that index can't be debuffed again
        List<int> indices = new List<int>(Player.GetSafeOffsets().Count);
        for (int i = 0; i < Player.GetSafeOffsets().Count; i++)
        {
            indices.Add(i);
        }

        // If 2 stress bars are filled, and the player doesn't already have the 2 bar penalty, apply it
        if (StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt) >= 2
            && !AnyStatHasTag(PlayerDeckScript.STRESS_THRESH_2))
        {
            int index = Random.Range(0, indices.Count);
            Player.instance.statOffsets[indices[index]].AddOffsetTag(PlayerDeckScript.STRESS_THRESH_2);
            indices.Remove(index);
        }

        // If 3 stress bars are filled, and the player doesn't already have the 3 bar penalty, apply it
        if (StressManager.GetStressBarsFilled(StressManager.instance.currentStressAmt) >= 3
            && !AnyStatHasTag(PlayerDeckScript.STRESS_THRESH_3))
        {
            int index = Random.Range(0, indices.Count);
            Player.instance.statOffsets[indices[index]].AddOffsetTag(PlayerDeckScript.STRESS_THRESH_3);
            indices.Remove(index);
        }
    }
}
