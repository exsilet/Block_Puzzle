using UnityEngine;
using GamingMonks;

[CreateAssetMenu(fileName = "DynamicOffer", menuName = "Scriptable Object/Dynamic Offer")]
public class DynamicOffersProduct : ScriptableObject
{
    public DynamicOfferProduct[] dynamicOfferProducts;
}

[System.Serializable]
public class DynamicOfferProduct
{
    public IAPProductID productID;
    public PurchaseType purchaseType;
    public float discount;
    public float purchasePrice;
    public Reward reward;
}

[System.Serializable]
public enum PurchaseType
{
    Money,
    Coin
}

