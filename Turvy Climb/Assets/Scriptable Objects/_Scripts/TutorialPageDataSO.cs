using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TutorialPageData", menuName = "ScriptableObjects/TutorialMenu/TutorialPageData")]
public class TutorialPageDataSO : ScriptableObject
{
    public string title;
    public Sprite previewImage;
    public string description;
}
