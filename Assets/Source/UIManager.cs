using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnHitButtonPressed()
    {
        GameManager.Instance.PlayerHit();
    }

    public void OnStandButtonPressed()
    {
        GameManager.Instance.PlayerStand();
    }
}
