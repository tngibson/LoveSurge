using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StressBar : MonoBehaviour
{
    public GameObject stressBar;
    public float currentStressAmt;
    public float maxStressAmt;

    [SerializeField] private GameObject StressBarParent;
    // Start is called before the first frame update
    void Start()
    {
        currentStressAmt = GameManager.instance.GetCurrentStressAmt();
        maxStressAmt = GameManager.instance.getMaxStressAmt();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void addToCurrentStress()
    {
        currentStressAmt += 01f;
    }
    public void updateStressBar()
    {
        float stressPercent = currentStressAmt/ maxStressAmt;
        StressBarParent.transform.localScale += new Vector3(stressPercent, 0);
    }
}
