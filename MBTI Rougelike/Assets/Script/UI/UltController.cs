using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用来管理屏幕左下角大招条的类。然而大招还没做好。
/// </summary>
public class UltController : MonoBehaviour
{
    public Slider ult;
    public Personality personality;

    private void Start()
    {
        ult = GetComponent<Slider>();
    }

    private void Update()
    {
        ult.value = personality.UltimateEnerge / personality.MaxUltimateEnerge;
    }

}
