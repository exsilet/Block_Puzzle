using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace GamingMonks
{
    public class DynamicOfferView : MonoBehaviour
    {
        [Header("All Offer Panel References")]
        [SerializeField] private GameObject m_coin300PackPanel;
        [SerializeField] private GameObject m_coin1000PackPanel;
        [SerializeField] private GameObject m_coin3500PackPanel;
        [SerializeField] private GameObject m_coin7500PackPanel;
        [SerializeField] private GameObject m_coin15000PackPanel;
        [SerializeField] private GameObject m_coin40000PackPanel;
        [SerializeField] private GameObject m_singularFocusPackPanel;
        [SerializeField] private GameObject m_rotateThemUpPackPanel;
        [SerializeField] private GameObject m_boomBoomPackPanel;
        [SerializeField] private GameObject m_basketOfPowersPackPanel;
        [SerializeField] private GameObject m_twistAndBlowPackPanel;
        [SerializeField] private GameObject m_turnUpPackPanel;
        [SerializeField] private GameObject m_powerUpBonanzaPackPanel;
        [SerializeField] private GameObject m_popularPackPanel;
        [SerializeField] private GameObject m_beginnerLootPackPanel;
        [SerializeField] private GameObject m_luxaryChestPackPanel;
        [SerializeField] private GameObject m_superiorJackpotPackPanel;

        [Space(3)]

        [Header("All Button Price Panel References")]
        [SerializeField] private Text m_coin300PackPrice;
        [SerializeField] private Text m_coin1000PackPrice;
        [SerializeField] private Text m_coin3500PackPrice;
        [SerializeField] private Text m_coin7500PackPrice;
        [SerializeField] private Text m_coin15000PackPrice;
        [SerializeField] private Text m_coin40000PackPrice;
        [SerializeField] private TextMeshProUGUI m_singularFocusPackPrice;
        [SerializeField] private TextMeshProUGUI m_rotateThemUpPackPrice;
        [SerializeField] private TextMeshProUGUI m_boomBoomPackPrice;
        [SerializeField] private TextMeshProUGUI m_basketOfPowersPackPrice;
        [SerializeField] private TextMeshProUGUI m_twistAndBlowPackPrice;
        [SerializeField] private TextMeshProUGUI m_turnUpPackPrice;
        [SerializeField] private TextMeshProUGUI m_powerUpBonanzaPackPrice;
        [SerializeField] private Text m_popularPackPrice;
        [SerializeField] private Text m_beginnerLootPackPrice;
        [SerializeField] private Text m_luxaryChestPackPrice;
        [SerializeField] private Text m_superiorJackpotPackPrice;

        private DynamicOfferController m_dynamicOfferController;

        private void OnEnable()
        {
            //m_buyButton.onClick.AddListener(OnBuyButtonClick);
        }

        private void OnDisable()
        {
            //m_buyButton.onClick.RemoveListener(OnBuyButtonClick);
            m_coin300PackPanel.gameObject.SetActive(false);
            m_coin1000PackPanel.gameObject.SetActive(false);
            m_coin3500PackPanel.gameObject.SetActive(false);
            m_coin7500PackPanel.gameObject.SetActive(false);
            m_coin15000PackPanel.gameObject.SetActive(false);
            m_coin40000PackPanel.gameObject.SetActive(false);
            m_singularFocusPackPanel.gameObject.SetActive(false);
            m_rotateThemUpPackPanel.gameObject.SetActive(false);
            m_boomBoomPackPanel.gameObject.SetActive(false);
            m_basketOfPowersPackPanel.gameObject.SetActive(false);
            m_twistAndBlowPackPanel.gameObject.SetActive(false);
            m_turnUpPackPanel.gameObject.SetActive(false);
            m_powerUpBonanzaPackPanel.gameObject.SetActive(false);
            m_popularPackPanel.gameObject.SetActive(false);
            m_beginnerLootPackPanel.gameObject.SetActive(false);
            m_luxaryChestPackPanel.gameObject.SetActive(false);
            m_superiorJackpotPackPanel.gameObject.SetActive(false);
        }

        public void ShowOffer(IAPProductID iAPProductID, PurchaseType purchaseType,string price)
        {
            m_coin300PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin300);
            m_coin1000PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin1000);
            m_coin3500PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin3500);
            m_coin7500PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin7500);
            m_coin15000PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin15000);
            m_coin40000PackPanel.gameObject.SetActive(iAPProductID == IAPProductID.coin40000);
            m_singularFocusPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.SingularFocus);
            m_rotateThemUpPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.RotateThemUp);
            m_boomBoomPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.BoomBoom);
            m_basketOfPowersPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.BasketOfPowers);
            m_twistAndBlowPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.TwistAndBlow);
            m_turnUpPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.TurnUp);
            m_powerUpBonanzaPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.PowerUpBonanza);
            m_popularPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.popularfortune);
            m_beginnerLootPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.beginnerloot);
            m_luxaryChestPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.luxurychest);
            m_superiorJackpotPackPanel.gameObject.SetActive(iAPProductID == IAPProductID.superiorjackpot);

            switch (iAPProductID)
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
                case IAPProductID.SingularFocus:
                    m_singularFocusPackPrice.text = price;
                    break;
                case IAPProductID.RotateThemUp:
                    m_rotateThemUpPackPrice.text = price;
                    break;
                case IAPProductID.BoomBoom:
                    m_boomBoomPackPrice.text = price;
                    break;
                case IAPProductID.BasketOfPowers:
                    m_basketOfPowersPackPrice.text = price;
                    break;
                case IAPProductID.TwistAndBlow:
                    m_twistAndBlowPackPrice.text = price;
                    break;
                case IAPProductID.TurnUp:
                    m_turnUpPackPrice.text = price;
                    break;
                case IAPProductID.PowerUpBonanza:
                    m_powerUpBonanzaPackPrice.text = price;
                    break;
                case IAPProductID.popularfortune:
                    m_popularPackPrice.text = price;
                    break;
                case IAPProductID.beginnerloot:
                    m_beginnerLootPackPrice.text = price;
                    break;
                case IAPProductID.luxurychest:
                    m_luxaryChestPackPrice.text = price;
                    break;
                case IAPProductID.superiorjackpot:
                    m_superiorJackpotPackPrice.text = price;
                    break;
            }
        }

        public void OnBuyButtonClick(GameObject activeGameObject)
        {
            m_dynamicOfferController.OnBuyButtonClick(activeGameObject);
        }

        public void SetDynamicOfferController(DynamicOfferController dynamicOfferController)
        {
            m_dynamicOfferController = dynamicOfferController;
        }
    }

}

