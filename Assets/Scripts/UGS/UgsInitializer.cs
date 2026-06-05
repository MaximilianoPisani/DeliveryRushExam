using System.Threading.Tasks;
using UnityEngine;
using System;

#if DELIVERY_RUSH_UGS
using Unity.Services.Authentication;
using Unity.Services.Core;
#endif

namespace DeliveryRushExam.UGS
{
    public class UgsInitializer : MonoBehaviour
    {
        [SerializeField] private bool verboseLogs = true;

        public bool IsReady { get; private set; }

        public async Task InitializeAsync()
        {
#if DELIVERY_RUSH_UGS
            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized)
                    await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                IsReady = true;

                if (verboseLogs)
                    Debug.Log($"[UgsInitializer] UGS ready - PlayerId: {AuthenticationService.Instance.PlayerId}");
            }
            catch (Exception e)
            {
                IsReady = false;
                Debug.LogError($"[UgsInitializer] Failed to initialize UGS: {e.Message}");
            }
#else
            IsReady = false;
            if (verboseLogs)
                Debug.Log("[UgsInitializer] DELIVERY_RUSH_UGS not defined - UGS disabled");

            await Task.Yield();
#endif
        }
    }
}