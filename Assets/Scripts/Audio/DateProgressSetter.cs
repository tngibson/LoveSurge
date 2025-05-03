using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateProgressSetter : MonoBehaviour
{
    // Start is called before the first frame update
    [field: SerializeField] public EnumDateProgress track { get; private set; }
    private void Start()
    {
        //AudioManager.instance.DateProgress(track);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
