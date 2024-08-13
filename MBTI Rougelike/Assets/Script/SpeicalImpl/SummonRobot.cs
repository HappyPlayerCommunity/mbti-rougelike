using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonRobot : PersonalitySpecialImplementation
{
    [SerializeField, Tooltip("robot的Prefab")]
    private IstpRobot robotPrefab;

    public override void ExecuteSpecialImplementation(Personality personality)
    {
        if (robotPrefab == null)
        {
            Debug.LogError("robot prefab is not assigned.");
            return;
        }

        GameObject ultraSun = Instantiate(robotPrefab.gameObject, personality.aim.transform.position, Quaternion.identity);
    }
}
