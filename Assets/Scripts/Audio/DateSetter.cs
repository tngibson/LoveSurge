using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateSetter : MonoBehaviour
{
    // Start is called before the first frame update
    [field: SerializeField] public EnumDateProgress track { get; private set; }
    private void Start()
    {
        MusicManager.instance.DateProgress(track);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
