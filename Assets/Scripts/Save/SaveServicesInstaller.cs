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

        private void Awake()
        {
            if (saveMode == SaveMode.Cloud &&
                Application.internetReachability != NetworkReachability.NotReachable)
            {
                ServiceLocator.Register<ISaveService>(new UgsCloudSaveService());
                Debug.Log("[SaveServicesInstaller] Registered UgsCloudSaveService");
                return;
            }

            // Fallback to local if no internet or mode is Local
            ServiceLocator.Register<ISaveService>(new LocalSaveService());
            Debug.Log("[SaveServicesInstaller] Registered LocalSaveService");
        }
    }
}
