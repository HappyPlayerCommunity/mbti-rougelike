using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用来控制玩家用来瞄准的【准星】类。
/// </summary>
public class Aim : MonoBehaviour
{
    [Header("互动组件")]
    [SerializeField, Tooltip("玩家Art的Transform。")]
    private Transform playerArtTransform;

    [SerializeField, Tooltip("武器Art的Transform。")]
    private Transform weaponArtTransform;

    [SerializeField, Tooltip("准星与角色中心的距离。")]
    private float aimPointdistance = 1.0f;

    private Transform shootPosition;

    private float reloadingTime = 1.0f;
    private float curretReloadingTime = 0.0f;
    private float scatterAngleHalf = 0.15f;

    public Vector3 aimDirection;

    void Update()
    {
        ArtUpdate();
    }

    /// <summary>
    /// 用于结算角色和准星（武器）的左右朝向等。
    /// </summary>
    void ArtUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0.0f;

        aimDirection = Vector3.Normalize(mouseWorldPos - playerArtTransform.position);
        aimDirection.z = 0.0f;

        float angle = Vector2.Angle(aimDirection, new Vector2(-1.0f, 0.0f));

        weaponArtTransform.position = playerArtTransform.position + aimDirection * aimPointdistance;

        if (aimDirection.x < 0.0f)
        {
            playerArtTransform.localScale = new Vector3(-1.0f, playerArtTransform.localScale.y, 0);
            weaponArtTransform.localScale = new Vector3(-1.0f, weaponArtTransform.localScale.y, 0);
            weaponArtTransform.localPosition = new Vector3(-1.0f * Mathf.Abs(weaponArtTransform.localPosition.x), weaponArtTransform.localPosition.y, 0.1f);

        }
        else if (aimDirection.x > 0.0f)
        {
            playerArtTransform.localScale = new Vector3(1.0f, playerArtTransform.localScale.y, 0);
            weaponArtTransform.localScale = new Vector3(1.0f, weaponArtTransform.localScale.y, 0);
            weaponArtTransform.localPosition = new Vector3(Mathf.Abs(weaponArtTransform.localPosition.x), weaponArtTransform.localPosition.y, -0.1f);
            angle += 180.0f;
        }

        //Find the angle for the two Vectors
        if (aimDirection.y > 0.0f)
        {
            weaponArtTransform.localEulerAngles = new Vector3(0.0f, 0.0f, -angle);
        }
        else
        {
            weaponArtTransform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
        }
    }
}
