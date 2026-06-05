using System;
using System.Threading.Tasks;
using DeliveryRushExam.Data;
using UnityEngine;

namespace DeliveryRushExam.Save
{
    public class SaveManager : MonoBehaviour
    {
        public PlayerProgressData CurrentProgress { get; private set; } = new PlayerProgressData();

        public event Action<PlayerProgressData> ProgressLoaded;

        private ISaveService saveService;

        private async void Awake()
        {
            if (!ServiceLocator.TryGet<ISaveService>(out saveService))
            {
                Debug.LogError("[SaveManager] ISaveService not registered. Check SaveServicesInstaller execution order.");
                return;
            }

            try
            {
                await LoadProgressAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load progress on awake: {e.Message}");
            }
        }

        public async Task LoadProgressAsync()
        {
            try
            {
                CurrentProgress = await saveService.LoadAsync();
                ProgressLoaded?.Invoke(CurrentProgress);
                Debug.Log($"[SaveManager] Progress loaded - bestScore: {CurrentProgress.bestScore}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to load progress: {e.Message}");
            }
        }

        public async Task SaveMatchResultAsync(int score, int coins, int completedOrders)
        {
            try
            {
                CurrentProgress.bestScore = Mathf.Max(CurrentProgress.bestScore, score);
                CurrentProgress.totalCoins += coins;
                CurrentProgress.completedOrders += completedOrders;
                CurrentProgress.unlockedLevel = Mathf.Max(CurrentProgress.unlockedLevel, 1 + CurrentProgress.completedOrders / 10);

                await saveService.SaveAsync(CurrentProgress);
                Debug.Log($"[SaveManager] Progress saved - bestScore: {CurrentProgress.bestScore} | coins: {CurrentProgress.totalCoins}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] Failed to save match result: {e.Message}");
            }
        }
    }
}