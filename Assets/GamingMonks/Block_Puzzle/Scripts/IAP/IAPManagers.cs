using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;

namespace GamingMonks
{
    public class IAPManagers : Singleton<IAPManagers>, IStoreListener
    {
        [SerializeField] private IAPproduct IAPproduct;

        private IStoreController storeController;
        private IExtensionProvider extensionProvider;

        public static event Action<Product> OnPurchaseSuccessfulEvent;
        public static event Action<string> OnPurchaseFailedEvent;
        public static event Action<bool> OnRestoreCompletedEvent;

        public static event Action<bool> OnIAPInitializeEvent;

        private GameObject dynamicOfferGameObject;

        async void Start()
        {
            await UnityServices.InitializeAsync();
            InitializeUnityIAP();
            //CheckForAdIAPPurchased();
        }

        private void InitializeUnityIAP()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (Product product in IAPproduct.Products)
            {
                builder.AddProduct(product.Id.ToString(), product.productType);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        /// <summary>
        /// Call back of Unity IAP
        /// called when UnityIAP is ready to Purchases
        /// </summary>
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            storeController = controller;
            extensionProvider = extensions;
            if (OnIAPInitializeEvent != null)
            {
                OnIAPInitializeEvent.Invoke(true);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            if (OnIAPInitializeEvent != null)
            {
                OnIAPInitializeEvent.Invoke(false);
            }
        }

        /// <summary>
        /// Called when a purchase completes.
        ///
        /// May be called at any time after OnInitialized().
        /// </summary>
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            if (e.purchasedProduct.hasReceipt)
            {
                ProcessPurchaseRewards(e.purchasedProduct.definition.id);
            }

            //ProcessPurchaseRewards(e.purchasedProduct.definition.id);
            return PurchaseProcessingResult.Complete;
        }

        /// <summary>
        /// Called when a purchase fails.
        /// </summary>
        public void OnPurchaseFailed(UnityEngine.Purchasing.Product product, PurchaseFailureReason failureReason)
        {
            OnPurchaseFailure(failureReason.ToString());
        }

        /// <summary>
		/// Returns if IAP SDK has initialized or not.
		/// </summary>
		public bool hasUnityIAPSdkInitialised
        {
            get { return storeController != null && extensionProvider != null; }
        }

        public UnityEngine.Purchasing.Product GetProduct(string productName)
        {
            return storeController.products.WithID(productName);
        }

        /// <summary>
		/// Invokes purchase success event.
		/// </summary>
		public void PurchaseProduct(string productID)
        {
            if (hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product purchasingProduct = storeController.products.WithID(productID);
                if(purchasingProduct != null && purchasingProduct.availableToPurchase)
                {
                    storeController.InitiatePurchase(purchasingProduct);
                }
            }
        }

        public void PurchaseProduct(string productID, GameObject activeGameObject)
        {
            if (hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product purchasingProduct = storeController.products.WithID(productID);
                if (purchasingProduct != null && purchasingProduct.availableToPurchase)
                {
                    //activeGameObject.gameObject.SetActive(false);
                    dynamicOfferGameObject = activeGameObject;
                    storeController.InitiatePurchase(purchasingProduct);

                }
            }
        }

        public void PurchaseProduct(string productID, GameObject activeGameObject, GameObject parentscreen)
        {
            if (hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product purchasingProduct = storeController.products.WithID(productID);
                if (purchasingProduct != null && purchasingProduct.availableToPurchase)
                {
                    //activeGameObject.gameObject.SetActive(false);
                    dynamicOfferGameObject = activeGameObject;
                    parentscreen.gameObject.Deactivate();
                    storeController.InitiatePurchase(purchasingProduct);
                }
            }
        }


        public void ProcessPurchaseRewards(string productId)
        {
            if (OnPurchaseSuccessfulEvent != null)
            {
                /*
                if(PlayerPrefs.GetInt("IsIAPpurchased") == 1)
                {

                }
                else
                {
                    PlayerPrefs.SetInt("IsIAPpurchased", 1);
                    //int addAttempts = PlayerPrefs.GetInt("AttemptsToPlayLevelWithoutAds");
                    PlayerPrefs.SetInt("AttemptsToPlayLevelWithoutAds", 20);
                }
                */
                //AdmobManager.Instance.countToNotShowAds = 20;
                PlayerPrefs.SetInt("AttemptsToPlayLevelWithoutAds", 20);
                Product purchasingProduct = Array.Find(IAPproduct.Products, item => item.Id.ToString() == productId);
                OnPurchaseSuccessfulEvent.Invoke(purchasingProduct);
                if(dynamicOfferGameObject != null)
                {
                    dynamicOfferGameObject.SetActive(false);
                }
                //FBManeger.Instance.IAPpurchased(GetIAPPrice(productId), GetCurrencySymbol(productId), productId);
            }
        }

        public void OnPurchaseFailure(string reason)
        {
            if (OnPurchaseFailedEvent != null)
            {
                OnPurchaseFailedEvent.Invoke(reason);
            }
        }

        public string GetIAPproductPrice(string productID)
        {
            if(hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product product = storeController.products.WithID(productID);
                if(product != null && product.availableToPurchase)
                {
                    return product.metadata.localizedPriceString;
                }
            }

            return null;
        }

        public string GetDefaultPrice(IAPProductID id)
        {
            foreach (Product product in IAPproduct.Products)
            {
                if(product.Id == id)
                {
                    return product.price;
                }
            }
            return null;
        }

        private string GetCurrencySymbol(string productID)
        {
            if (hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product product = storeController.products.WithID(productID);
                if (product != null && product.availableToPurchase)
                {
                    return product.metadata.isoCurrencyCode;
                }
            }

            return null;
        }

        private float GetIAPPrice(string productID)
        {
            if (hasUnityIAPSdkInitialised)
            {
                UnityEngine.Purchasing.Product product = storeController.products.WithID(productID);
                if (product != null && product.availableToPurchase)
                {
                    return (float)product.metadata.localizedPrice;
                }
            }
            return 0;
        }

        private void CheckForAdIAPPurchased()
        {
            
            UnityEngine.Purchasing.Product purchasedProduct = storeController.products.WithID(IAPProductID.noads.ToString());

            if (purchasedProduct != null && purchasedProduct.hasReceipt)
            {
                ProfileManager.Instance.SetAppAsAdFree();
            }
        }

        public Product GetCoinPackProduct(int coinDeficit)
        {
            foreach(Product product in IAPproduct.Products)
            {
                if(product.rewardType == RewardType.Coin && product.reward.coin >= coinDeficit)
                {
                    return product;
                }
            }

            return null;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            
        }
    }

}
