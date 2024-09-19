using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
[CreateAssetMenu()]

public class Choices : ScriptableObject
{
    [SerializeField] public string[] choiceOptions;
    [SerializeField] public string[] choiceFilePath;
    [SerializeField] public string afterChoiceFilePath;
    private FileInfo source1;
    public FileInfo InitFileInfo(int choiceIndex)
    {
        source1 = new FileInfo(choiceFilePath[choiceIndex]);
        return source1;
    }
    
}
