using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialMenuData", menuName = "ScriptableObjects/TutorialMenu/TutorialMenuData")]
public class TutorialMenuDataSO : ScriptableObject
{
    public TutorialPageDataSO[] pages;
}
