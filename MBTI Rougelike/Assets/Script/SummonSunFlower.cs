using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSunFlower : PersonalitySpecialImplementation
{
    [SerializeField, Tooltip("SunFLower的Prefab")]
    private GameObject sunFlowerPrefab;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        if (sunFlowerPrefab == null)
        {
            Debug.LogError("SunFLower的Prefab prefab is not assigned.");
            return;
        }


        Debug.Log("personality.aim.transform.position" + personality.aim.transform.position);
        GameObject ultraSun = Instantiate(sunFlowerPrefab, personality.ultSkill_InitPosition.position, Quaternion.identity);
    }
}
