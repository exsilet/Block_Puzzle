using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamingMonks.UITween;

namespace GamingMonks
{
    public class DynamicOfferService : Singleton<DynamicOfferService>
    {
        private DynamicOffersProduct m_dynamicOffer;
        private DynamicOfferController m_dynamicOfferController = null;

        [SerializeField] private DynamicOfferView m_goalScreenOfferInstance;
        [SerializeField] private DynamicOfferView m_levelWinScreenOfferInstance;
        [SerializeField] private DynamicOfferView m_QuitAndRetryScreenOfferInstance;
        [SerializeField] private DynamicOfferView m_levelOverScreenOfferInstance;

        private void Awake()
        {
            if(m_dynamicOffer == null)
            {
                m_dynamicOffer = (DynamicOffersProduct)Resources.Load("DynamicOffer");
            }

            Initialize();
        }

        private void Initialize()
        {
            m_dynamicOfferController = new DynamicOfferController(m_dynamicOffer);
        }

        public DynamicOfferProduct GetOfferProduct(IAPProductID ID)
        {
            foreach(DynamicOfferProduct offerProduct in m_dynamicOffer.dynamicOfferProducts)
            {
                if(offerProduct.productID == ID)
                {
                    return offerProduct;
                }
            }

            return null;
        }

        public void ShowGoalScreenOffer()
        {
            m_dynamicOfferController.ShowDynamicOffer(m_goalScreenOfferInstance);
        }

        public void ShowLevelWinOffer()
        {
            m_dynamicOfferController.ShowDynamicOffer(m_levelWinScreenOfferInstance);
        }

        public void ShowLevelQuitAndRetryOffer()
        {
            m_dynamicOfferController.ShowDynamicOffer(m_QuitAndRetryScreenOfferInstance);
        }

        public void ShowLevelLooseOffer()
        {
            m_dynamicOfferController.ShowDynamicOffer(m_levelOverScreenOfferInstance);
        }
    }
}


