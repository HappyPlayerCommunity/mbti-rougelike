using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEsfpStatus", menuName = "Status Data/Esfp Ult Data")]
public class StatusEsfpUlt : Status
{
    EsfpNoteManager esfpNoteManager;
    private float toungness = 10000.0f;
    public Player player;

    public override void OnApply(GameObject target)
    {
        base.OnApply(target);

        esfpNoteManager = target.GetComponentInChildren<EsfpNoteManager>();
        esfpNoteManager.isUlting = true;
        esfpNoteManager.timerAuto = 0.0f;
        esfpNoteManager.timerSp = 0.5f;
        stats.toughness += toungness;
        player = target.GetComponent<Player>();
        player.StatsUpdate();
    }

    public override void OnExpire(GameObject target)
    {
        base.OnExpire(target);

        esfpNoteManager = target.GetComponentInChildren<EsfpNoteManager>();
        esfpNoteManager.isUlting = false;

        stats.toughness -= toungness;
        var player = target.GetComponent<Player>();
        player.StatsUpdate();
    }
}
