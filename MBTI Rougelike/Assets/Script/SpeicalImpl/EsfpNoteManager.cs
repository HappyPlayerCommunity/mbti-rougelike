using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EsfpNoteManager : MonoBehaviour
{
    [Tooltip("EsfpNote的Auto Prefab")]
    public GameObject esfpNotePrefabAuto;

    [Tooltip("EsfpNote的Sp Prefab")]
    public GameObject esfpNotePrefabSp;

    [Tooltip("生成EsfpNote Auto的时间间隔")]
    public float timeAuto = 2.0f;

    [Tooltip("生成EsfpNote Sp的时间间隔")]
    public float timeSp = 8.0f;

    [Tooltip("大招期间生成EsfpNote的时间间隔")]
    public float timeUlt = 1.0f;

    [Tooltip("EsfpNote的最大数量")]
    public int maxNotes = 10;

    [Tooltip("EsfpNote的最小旋转速度")]
    public float minRotationSpeed = 10.0f;

    [Tooltip("EsfpNote的最大旋转速度")]
    public float maxRotationSpeed = 100.0f;

    [Tooltip("EsfpNote的大招时的旋转速度")]
    public float ultRotationSpeed = 50.0f;

    public Aim aim;
    public Stats stats;

    private List<GameObject> activeNotes = new List<GameObject>();

    public float timerAuto;
    public float timerSp;

    public bool isUlting = false;


    void Start()
    {
        timerAuto = timeAuto;
        timerSp = timeSp;

        stats = transform.GetComponentInParent<Player>().stats;
    }

    void Update()
    {
        timerAuto -= Time.deltaTime;
        timerSp -= Time.deltaTime;

        if (activeNotes.Count < maxNotes)
        {
            SpawnEsfpNote(ref timerAuto, timeAuto * stats.Calculate_AttackSpeed(), esfpNotePrefabAuto);
            SpawnEsfpNote(ref timerSp, timeSp * stats.Calculate_SpecialCooldown(), esfpNotePrefabSp);
        }

        // 清理已销毁的音符
        activeNotes.RemoveAll(note => note == null);
    }

    void SpawnEsfpNote(ref float timer, float time, GameObject note)
    {
        if (timer < 0.0f)
        {
            // 生成 EsfpNote
            GameObject newNote = Instantiate(note, transform.position, Quaternion.identity);

            // 赋予随机的 rotationSpeed
            EsfpNote esfpNote = newNote.GetComponent<EsfpNote>();
            if (esfpNote != null)
            {
                esfpNote.rotationSpeed = isUlting ? ultRotationSpeed : Random.Range(minRotationSpeed, maxRotationSpeed);
                esfpNote.aim = aim;
                esfpNote.esfpNoteManager = this;
            }

            // 记录生成的 EsfpNote
            activeNotes.Add(newNote);

            timer = isUlting ? timeUlt : time;
        }
    }

    public void NoteRemoved(EsfpNote note)
    {
        activeNotes.Remove(note.gameObject);
    }
}
