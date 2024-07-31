using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingManager : MonoBehaviour
{
    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        
    }

    public void ResetPlayer()
    {
        player.gameObject.SetActive(true);
        player.Respawn();
    }
}
