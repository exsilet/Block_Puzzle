using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GamingMonks
{
    [CreateAssetMenu(fileName = "IAPProducts", menuName = "Scriptable Object/IAP Products ")]
    public class IAPproduct : ScriptableObject
    {
        public Product[] Products;
    }

    [System.Serializable]
    public class Product
    {
        public IAPProductID Id;
        public ProductType productType;
        public RewardType rewardType;
        public string price;
        public Reward reward;
    }

    [System.Serializable]
    public class Reward
    {
        public int coin;
        public int rotatePowerUp;
        public int singleBlockPowerUp;
        public int bombPowerUp;
    }

    [System.Serializable]
    public enum RewardType
    {
        RemoveAds,
        Coin,
        Pack,
        Theme,
        OTHER
    }
}


