using UnityEngine;
using UnityEngine.UI;

namespace GamingMonks
{
    public class ContextualOfferController : MonoBehaviour
    {
        [SerializeField] private GameObject m_coin300PackPanel;
        [SerializeField] private GameObject m_coin1000PackPanel;
        [SerializeField] private GameObject m_coin3500PackPanel;
        [SerializeField] private GameObject m_coin7500PackPanel;
        [SerializeField] private GameObject m_coin15000PackPanel;
        [SerializeField] private GameObject m_coin40000PackPanel;

        [SerializeField] private Text m_coin300PackPrice;
        [SerializeField] private Text m_coin1000PackPrice;
        [SerializeField] private Text m_coin3500PackPrice;
        [SerializeField] private Text m_coin7500PackPrice;
        [SerializeField] private Text m_coin15000PackPrice;
        [SerializeField] private Text m_coin40000PackPrice;

        private Product m_purchasingProduct;

        private void Awake()
        {
            //m_buyButton.onClick.AddListener(OnBuyButtonClick);
            //m_closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        public void Initialize(int deficitCoin)
        {
            m_purchasingProduct = IAPManagers.Instance.GetCoinPackProduct(deficitCoin);

            if(m_purchasingProduct != null)
            {
                m_coin300PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin300);
                m_coin1000PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin1000);
                m_coin3500PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin3500);
                m_coin7500PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin7500);
                m_coin15000PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin15000);
                m_coin40000PackPanel.gameObject.SetActive(m_purchasingProduct.Id == IAPProductID.coin40000);

                string price = "";

                if (IAPManagers.Instance.hasUnityIAPSdkInitialised)
                {
                    price = IAPManagers.Instance.GetIAPproductPrice(m_purchasingProduct.Id.ToString());
                }
                else
                {
                    price = IAPManagers.Instance.GetDefaultPrice(m_purchasingProduct.Id);
                }

                switch (m_purchasingProduct.Id)
                {
                    case IAPProductID.coin300:
                        m_coin300PackPrice.text = price;
                        break;
                    case IAPProductID.coin1000:
                        m_coin1000PackPrice.text = price;
                        break;
                    case IAPProductID.coin3500:
                        m_coin3500PackPrice.text = price;
                        break;
                    case IAPProductID.coin7500:
                        m_coin7500PackPrice.text = price;
                        break;
                    case IAPProductID.coin15000:
                        m_coin15000PackPrice.text = price;
                        break;
                    case IAPProductID.coin40000:
                        m_coin40000PackPrice.text = price;
                        break;
                }
            }
        }

        public void OnBuyButtonClick(GameObject activeGameObject)
        {
            IAPManagers.Instance.PurchaseProduct(m_purchasingProduct.Id.ToString(), activeGameObject, this.gameObject);
        }

        public void OnCloseButtonClick(GameObject activeGameObject)
        {
            UIController.Instance.CloseContextualOfferScreen(activeGameObject);
        }
    }
}

