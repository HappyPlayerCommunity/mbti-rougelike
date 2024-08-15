using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsfpNoteTrigger : MonoBehaviour
{
    [Tooltip("DamageCollider的引用")]
    public DamageCollider damageCollider;

    [Tooltip("检测范围的半径")]
    public float detectionRadius;

    [Tooltip("各个距离等级的阈值")]
    public float greatThreshold;
    public float perfectThreshold;

    private EsfpNote collidedNote;

    public float perfectMultiplier = 4.0f;
    public float greatMultiplier = 2.0f;

    private bool isInitialized = false;

    public bool isBigNote = false;

    public enum NoteResult
    {
        Perfect,
        Great,
        Good,
        Miss,
        Default
    }

    private void OnEnable()
    {
        if (!isInitialized)
        {
            isInitialized = true;
            return;
        }

        // 获取所有的 EsfpNote
        EsfpNote[] esfpNotes = FindObjectsOfType<EsfpNote>();

        if (esfpNotes.Length == 0)
        {
            StrengthenDamageCollider(NoteResult.Miss);
            return;
        }

        // 按距离排序
        System.Array.Sort(esfpNotes, (note1, note2) =>
        {
            float distance1 = Vector3.Distance(transform.position, note1.transform.position);
            float distance2 = Vector3.Distance(transform.position, note2.transform.position);
            return distance1.CompareTo(distance2);
        });

        // 处理最近的 EsfpNote
        collidedNote = esfpNotes[0];
        float distance = Vector3.Distance(transform.position, collidedNote.transform.position);

        if (distance <= detectionRadius && isBigNote == collidedNote.isBigNote)
        {
            damageCollider.ResetObjectState();
            // 根据距离强化 DamageCollider 的属性
            if (distance <= perfectThreshold)
            {
                StrengthenDamageCollider(NoteResult.Perfect);
            }
            else if (distance <= greatThreshold)
            {
                StrengthenDamageCollider(NoteResult.Great);
            }
            else
            {
                StrengthenDamageCollider(NoteResult.Good);
            }
        }
        else
        {
            StrengthenDamageCollider(NoteResult.Miss);
        }
    }

    private void StrengthenDamageCollider(NoteResult result)
    {
        var spriteRenderTransform = damageCollider.spriteRenderer.transform;
        switch (result)
        {
            case NoteResult.Perfect:
                damageCollider.damage *= (int)perfectMultiplier;
                spriteRenderTransform.localScale *= perfectMultiplier;
                TriggeredEsfpNote();
                DamagePopupManager.Instance.Popup(PopupType.Note, transform.position, 0, false, NoteResult.Perfect);
                break;
            case NoteResult.Great:
                damageCollider.damage *= (int)greatMultiplier;
                spriteRenderTransform.localScale *= greatMultiplier;
                TriggeredEsfpNote();
                DamagePopupManager.Instance.Popup(PopupType.Note, transform.position, 0, false, NoteResult.Great);
                break;
            case NoteResult.Good:
                TriggeredEsfpNote();
                DamagePopupManager.Instance.Popup(PopupType.Note, transform.position, 0, false, NoteResult.Good);
                break;
            case NoteResult.Miss:
                // 回收 DamageCollider
                damageCollider.Deactivate();
                DamagePopupManager.Instance.Popup(PopupType.Note, transform.position, 0, false, NoteResult.Miss);

                // 销毁最近的 EsfpNote
                if (collidedNote != null)
                {
                    TriggeredEsfpNote();
                }
                break;
        }
    }

    private void TriggeredEsfpNote()
    {
        if (collidedNote != null)
        {
            collidedNote.esfpNoteManager.NoteRemoved(collidedNote);
            Destroy(collidedNote.gameObject);
        }
    }
}
