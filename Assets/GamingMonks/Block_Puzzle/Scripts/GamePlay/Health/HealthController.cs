using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

namespace GamingMonks
{
    public class HealthController : Singleton<HealthController>
    {
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private int maxEnergy = 5;       
        [SerializeField] private int restoreDuration = 10;
        [SerializeField] private Button btnLives;
        public int currentEnergy=0;
        private DateTime nextEnergyTime;
        private DateTime lastEnergyTime;
        private bool isRestoring = false;


        void Start()
        {
            if (!PlayerPrefs.HasKey("currentEnergy"))
            {
                PlayerPrefs.SetInt("currentEnergy", maxEnergy);
                Load();
                StartCoroutine(RestoreEnergy());
            }
            else
            {
                Load();
                StartCoroutine(RestoreEnergy());
            }
        }

        public void UseEnergy()
        {
            if (currentEnergy >= 1)
            {
                currentEnergy--;
                UpdateEnergy();
                if (isRestoring == false)
                {
                    if (currentEnergy + 1 == maxEnergy)
                    {
                        nextEnergyTime = AddDuration(DateTime.Now, restoreDuration);
                    }
                    StartCoroutine(RestoreEnergy());
                }
            }

            else
            {
                Debug.Log("No more life");
            }
        }

        private IEnumerator RestoreEnergy()
        {
            UpdateEnergyTimer();
            UpdateEnergy();
            isRestoring = true;
            //currentEnergy = Mathf.Max(0, maxEnergy);
            while (currentEnergy < maxEnergy)
            {
                DateTime currentDatetime = DateTime.Now;
                DateTime nextDateTime = nextEnergyTime;
                bool isEnergyAdding = false;

                while (currentDatetime > nextDateTime)
                {
                    if (currentEnergy < maxEnergy)
                    {
                        isEnergyAdding = true;
                        currentEnergy++;
                        UpdateEnergy();
                        DateTime timeToAdd = lastEnergyTime > nextDateTime ? lastEnergyTime : nextDateTime;
                        nextDateTime = AddDuration(timeToAdd, restoreDuration);
                    }
                    else
                    {
                        break;
                    }
                }

                if (isEnergyAdding == true)
                {
                    lastEnergyTime = DateTime.Now;
                    nextEnergyTime = nextDateTime;
                }

                UpdateEnergyTimer();
                UpdateEnergy();
                Save();
                yield return null;
            }

            isRestoring = false;
        }

        private DateTime AddDuration(DateTime dateTime, int duration)
        {
            return dateTime.AddMinutes(duration);
        }

        public void UpdateEnergyTimer()
        {
            if (currentEnergy >= maxEnergy)
            {
                timerText.text = "full";
                return;
            }

            TimeSpan time = nextEnergyTime - DateTime.Now;
            string timeValue = string.Format("{0:00}:{1:00}", time.Minutes, time.Seconds);
            timerText.text = timeValue;
        }

        public void UpdateEnergy()
        {
            energyText.text = currentEnergy.ToString();
            //if (currentEnergy < 5)
            //{
            //    btnLives.interactable = true;
            //}
            //else
            //{
            //    btnLives.interactable = false;
            //}
        }

        private DateTime StrngToDate(string dateTime)
        {
            if (String.IsNullOrEmpty(dateTime))
            {
                return DateTime.Now;
            }
            else
            {
                return DateTime.Parse(dateTime);
            }
        }

        private void Load()
        {
            currentEnergy = PlayerPrefs.GetInt("currentEnergy");
            nextEnergyTime = StrngToDate(PlayerPrefs.GetString("nextEnergyTime"));
            lastEnergyTime = StrngToDate(PlayerPrefs.GetString("lastEnergyTime"));
        }

        private void Save()
        {
            PlayerPrefs.SetInt("currentEnergy", currentEnergy);
            PlayerPrefs.SetString("nextEnergyTime", nextEnergyTime.ToString());
            PlayerPrefs.SetString("lastEnergyTime", lastEnergyTime.ToString());
        }

        public int GetCurrentHealth()
        {
            return currentEnergy;
        }

        public string GetCurrentTime()
        {
            return timerText.text;
        }
    }
}
