using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    public static StressBar instance;

    public GameObject stressBar;
    public float currentStressAmt;
    [SerializeField] private GameObject StressBarParent;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);  // Ensures only one instance of Stress Bar
        }
        else
        {
            instance = this;
        }
    }
    void Start()
    {
        currentStressAmt = StressManager.instance.GetCurrentStressAmount();
        UpdateStressBar();
        
    }

    // Update is called once per frame

    public void UpdateStressBar()
    {
        if (StressBarParent != null)
        {
            currentStressAmt = StressManager.instance.GetCurrentStressAmount();
            StressBarParent.transform.localScale = new Vector3(currentStressAmt, StressBarParent.transform.localScale.y, StressBarParent.transform.localScale.z);
        }
    }
}
