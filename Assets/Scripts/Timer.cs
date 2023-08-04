using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class Timer : NetworkBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainTime;
    private bool timercheck;
    public int clientCount;

    private void Start()
    {
        // Diyelim ki, oyun başladığında tüm oyuncular timer başlatma komutunu almalı.
        // Bunun için timerall() metodu bir şekilde çağrılmalı.
    }

    void Update()
    {
        // Timer çalışmıyorsa güncelleme yapma
        if (!timercheck)
            return;

        // Eğer iki oyuncu da bağlı değilse, timer güncellenmeyecek.
        // Eğer bağlı oyuncu sayısı kontrolü yerel oyuncu için de geçerli olsun istiyorsanız,
        // bu kontrolü timerall() metodu içinde yapmalısınız.
        if (clientCount < 2)
            return;

        if (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
        }
        else if (remainTime <= 0)
        {
            remainTime = 0;
            timerText.color = Color.red;
            // Zaman bittiğinde bir şeyler yapmak istiyorsanız burada ekleme yapabilirsiniz.
        }

        int minutes = Mathf.FloorToInt(remainTime / 60);
        int seconds = Mathf.FloorToInt(remainTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void timerall()
    {
        timercheck = true;

        if (!IsServer && IsOwner)
        {
            clientCount += 1;
        }
    }
}