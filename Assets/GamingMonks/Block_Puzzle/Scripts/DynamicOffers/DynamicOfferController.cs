using GamingMonks.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamingMonks
{
    public class DynamicOfferController
    {
        private DynamicOfferView m_dynamicOfferView;
        private DynamicOffersProduct m_dynamicOffer;
        private DynamicOfferProduct m_dynamicOfferProduct;

        private List<DynamicOfferProduct> m_dynamicOffersPool;
        private List<DynamicOfferProduct> m_upcomingDynamicOffersPool;

        public DynamicOfferController(DynamicOffersProduct dynamicOffer)
        {
            m_dynamicOffer = dynamicOffer;
            m_dynamicOffersPool = new List<DynamicOfferProduct>();
            m_upcomingDynamicOffersPool = new List<DynamicOfferProduct>();
            PrepareDynamicOfferPool();
            PrepareUpcomingDynamicOfferPool();
        }

        private void PrepareDynamicOfferPool()
        {
            for(int i = 0; i < m_dynamicOffer.dynamicOfferProducts.Length; i++)
            {
                m_dynamicOffersPool.Add(m_dynamicOffer.dynamicOfferProducts[i]);
            }
        }

        private void PrepareUpcomingDynamicOfferPool()
        {
            m_upcomingDynamicOffersPool.AddRange(m_dynamicOffersPool);
            m_upcomingDynamicOffersPool.Shuffle();
        }

        public void ShowDynamicOffer(DynamicOfferView dynamicOfferView)
        {
            Reset();
            m_dynamicOfferView = dynamicOfferView;

            string price = "";
            m_dynamicOfferProduct = GetOfferProduct();
           
            if (m_dynamicOfferProduct.purchaseType == PurchaseType.Money)
            {
                // if(IAPManagers.Instance.hasUnityIAPSdkInitialised)
                // {
                //     price = IAPManagers.Instance.GetIAPproductPrice(m_dynamicOfferProduct.productID.ToString());
                // }
                // else
                // {
                //     price = IAPManagers.Instance.GetDefaultPrice(m_dynamicOfferProduct.productID);
                // }
            }
            else
            {
                price = m_dynamicOfferProduct.purchasePrice.ToString();
            }

            m_dynamicOfferView.SetDynamicOfferController(this);
            m_dynamicOfferView.ShowOffer(m_dynamicOfferProduct.productID, m_dynamicOfferProduct.purchaseType, price);


        }

        private DynamicOfferProduct GetOfferProduct()
        {
            if(m_upcomingDynamicOffersPool.Count <= 0)
            {
                PrepareUpcomingDynamicOfferPool();
            }

            DynamicOfferProduct dynamicOfferProduct = m_upcomingDynamicOffersPool[0];
            m_upcomingDynamicOffersPool.RemoveAt(0);
            return dynamicOfferProduct;
        }

        public void OnBuyButtonClick(GameObject activeGameObject)
        {
            if(m_dynamicOfferProduct.purchaseType == PurchaseType.Money)
            {
                //IAPManagers.Instance.PurchaseProduct(m_dynamicOfferProduct.productID.ToString(), activeGameObject);
            }
            else
            {
                if(CurrencyManager.Instance.GetCurrentCoinsBalance() <= m_dynamicOfferProduct.purchasePrice)
                {
                    int coinDeficit = (int)(m_dynamicOfferProduct.purchasePrice - CurrencyManager.Instance.GetCurrentCoinsBalance());
                    UIController.Instance.contextualOfferScreen.GetComponentInChildren<ContextualOfferController>().Initialize(coinDeficit);
                    UIController.Instance.OpenContextualOfferScreen();
                }
                else
                {
                    CurrencyManager.Instance.DeductCoins((int)m_dynamicOfferProduct.purchasePrice);
                    CurrencyManager.Instance.SetPacks(m_dynamicOfferProduct.reward);
                    UIController.Instance.purchaseSuccessScreen.Activate();
                    activeGameObject.gameObject.SetActive(false);
                    //FBManeger.Instance.SpentCredit(m_dynamicOfferProduct.purchasePrice, m_dynamicOfferProduct.productID.ToString());
                }
            }
        }

        private void Reset()
        {
            m_dynamicOfferView = null;
            m_dynamicOfferProduct = null;
        }
    }
}