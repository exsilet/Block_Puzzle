using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YG;

public class RewardYG : MonoBehaviour
{
    public static Action RewardClosed;

    private void OnEnable()
    {
        YandexGame.OpenVideoEvent += OpenAd;
        YandexGame.RewardVideoEvent += Rewarded;
        YandexGame.CloseVideoEvent += CloseAd;
    }

    // Отписываемся от события открытия рекламы в OnDisable
    private void OnDisable()
    {
        YandexGame.OpenVideoEvent -= OpenAd;
        YandexGame.RewardVideoEvent -= Rewarded;
        YandexGame.CloseVideoEvent -= CloseAd;
    }

    // Метод для вызова видео рекламы
    public void OpenRewardAd()
    {
        YandexGame.RewVideoShow(0);
    }

    public void OpenAd()
    {
        Time.timeScale = 0;
    }

    public void CloseAd()
    {
        Time.timeScale = 1;
    }

    private void Rewarded(int id)
    {
        
    }
}
