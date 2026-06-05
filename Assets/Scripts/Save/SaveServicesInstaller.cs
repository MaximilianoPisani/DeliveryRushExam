using System.Threading.Tasks;
using DeliveryRushExam.UGS;
using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class SaveServicesInstaller : MonoBehaviour
    {
        public enum SaveMode
        {
            Local,
            Cloud
        }

        [SerializeField] private SaveMode saveMode = SaveMode.Local;
        [SerializeField] private UgsInitializer ugsInitializer;

        private async void Awake()
        {
            if (ugsInitializer == null)
                ugsInitializer = FindFirstObjectByType<UgsInitializer>();

            if (saveMode == SaveMode.Cloud &&
                Application.internetReachability != NetworkReachability.NotReachable)
            {
                // Initialize UGS first, then register cloud service
                await ugsInitializer.InitializeAsync();

                if (ugsInitializer.IsReady)
                {
                    ServiceLocator.Register<ISaveService>(new UgsCloudSaveService());
                    Debug.Log("[SaveServicesInstaller] Registered UgsCloudSaveService");
                    return;
                }

                // UGS failed - fallback to local
                Debug.LogWarning("[SaveServicesInstaller] UGS failed, falling back to LocalSaveService");
            }

            ServiceLocator.Register<ISaveService>(new LocalSaveService());
            Debug.Log("[SaveServicesInstaller] Registered LocalSaveService");
        }
    }
}