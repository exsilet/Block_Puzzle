using UnityEngine;
using System;

public class GamingServiceInit : MonoBehaviour
{
     public string environment = "production";

    private void Awake()
    {
        InitializeGamingService();
    }
   
    async void InitializeGamingService()
    {
        // Debug.Log("Initialize Gaming service");
        // try
        // {
        //     InitializationOptions options = new InitializationOptions()
        //         .SetEnvironmentName(environment);
        //
        //     await UnityServices.InitializeAsync(options);
        // }
        // catch (Exception exception)
        // {
        //     Debug.Log(exception);
        // }
    }

}
