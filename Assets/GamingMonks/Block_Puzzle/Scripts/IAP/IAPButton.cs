using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamingMonks
{
    [RequireComponent(typeof(Button))]
    public class IAPButton : MonoBehaviour
    {
        [SerializeField] private IAPProductID productID;
        [SerializeField] Text priceText;

        private Button thisButton;
        private bool hasInitialized = false;
        private UnityEngine.Purchasing.Product thisProduct;


        private void Awake()
        {
            thisButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            //Invoke("InitIAPProduct", 1F);
            InitIAPProduct();
        }

        private void OnDisable()
        {
            thisButton.onClick.RemoveListener(OnPurchaseButtonPressed);
        }

        /// <summary>
        ///  Fetrched IAP product and assign to button which listener.
        /// </summary>
        void InitIAPProduct()
        {
            thisButton.onClick.AddListener(OnPurchaseButtonPressed);

            if (!hasInitialized)
            {
                if (IAPManagers.Instance.hasUnityIAPSdkInitialised)
                {
                    thisProduct = IAPManagers.Instance.GetProduct(productID.ToString());
                    if (thisProduct != null)
                    {
                        if (priceText != null)
                        {
                            priceText.text = thisProduct.metadata.localizedPriceString;
                        }
                        hasInitialized = true;
                    }
                }
                else
                {
                    if (priceText != null)
                    {
                        priceText.text = IAPManagers.Instance.GetDefaultPrice(productID);
                    }
                }
            }
        }

        // Purchase button click listner.
        void OnPurchaseButtonPressed()
        {
            IAPManagers.Instance.PurchaseProduct(productID.ToString());
        }
    }
}
