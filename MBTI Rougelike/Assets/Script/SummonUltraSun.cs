using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonUltraSun : PersonalitySpecialImplementation
{
    [SerializeField, Tooltip("UltraSun的Prefab")]
    private GameObject ultraSunPrefab;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        if (ultraSunPrefab == null)
        {
            Debug.LogError("UltraSun prefab is not assigned.");
            return;
        }

        Vector3 playerPosition = personality.transform.position;

        GameObject ultraSun = Instantiate(ultraSunPrefab, playerPosition, Quaternion.identity);
    }
}
