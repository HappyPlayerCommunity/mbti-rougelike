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

    [Tooltip("EsfpNote的最大数量")]
    public int maxNotes = 10;

    [Tooltip("EsfpNote的最小旋转速度")]
    public float minRotationSpeed = 10.0f;

    [Tooltip("EsfpNote的最大旋转速度")]
    public float maxRotationSpeed = 100.0f;

    public Aim aim;

    private List<GameObject> activeNotes = new List<GameObject>();

    private float timerAuto;
    private float timerSp;


    void Start()
    {
        timerAuto = timeAuto;
        timerSp = timeSp;
    }

    void Update()
    {
        timerAuto -= Time.deltaTime;
        timerSp -= Time.deltaTime;

        if (activeNotes.Count < maxNotes)
        {
            SpawnEsfpNote(ref timerAuto, timeAuto, esfpNotePrefabAuto);
            SpawnEsfpNote(ref timerSp, timeSp, esfpNotePrefabSp);
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
                esfpNote.rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
                esfpNote.aim = aim;
                esfpNote.esfpNoteManager = this;
            }

            // 记录生成的 EsfpNote
            activeNotes.Add(newNote);

            timer = time;
        }
    }

    public void NoteRemoved(EsfpNote note)
    {
        activeNotes.Remove(note.gameObject);
    }
}
