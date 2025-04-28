using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateCharacter : MonoBehaviour
{
    [field: SerializeField] public EnumDateCharacter character { get; private set; }
    private void Start()
    {
        AudioManager.instance.DateCharacter(character);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
