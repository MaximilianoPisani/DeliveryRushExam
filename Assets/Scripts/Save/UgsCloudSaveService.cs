using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DeliveryRushExam.Data;
using UnityEngine;

#if DELIVERY_RUSH_UGS
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
#endif

namespace DeliveryRushExam.Save
{
    public class UgsCloudSaveService : ISaveService
    {
        // Single key for the whole progress object - keeps cloud save lightweight
        private const string ProgressKey = "delivery_rush_progress";

        public async Task<PlayerProgressData> LoadAsync()
        {
#if DELIVERY_RUSH_UGS
            try
            {
                HashSet<string> keys = new HashSet<string> { ProgressKey };
                Dictionary<string, Unity.Services.CloudSave.Models.Item> result =
                    await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

                if (result.ContainsKey(ProgressKey))
                {
                    PlayerProgressData data = result[ProgressKey].Value.GetAs<PlayerProgressData>();
                    Debug.Log($"[UgsCloudSaveService] Progress loaded - bestScore: {data.bestScore}");
                    return data ?? new PlayerProgressData();
                }

                Debug.Log("[UgsCloudSaveService] No saved progress found, returning default");
                return new PlayerProgressData();
            }
            catch (Exception e)
            {
                Debug.LogError($"[UgsCloudSaveService] Failed to load progress: {e.Message}");
                return new PlayerProgressData();
            }
#else
            await Task.Yield();
            return new PlayerProgressData();
#endif
        }

        public async Task SaveAsync(PlayerProgressData progressData)
        {
#if DELIVERY_RUSH_UGS
            try
            {
                progressData.TouchSaveDate();

                // Serialize entire object as single key-value - small and efficient
                Dictionary<string, object> data = new Dictionary<string, object>
                {
                    { ProgressKey, progressData }
                };

                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
                Debug.Log($"[UgsCloudSaveService] Progress saved - bestScore: {progressData.bestScore} | coins: {progressData.totalCoins}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[UgsCloudSaveService] Failed to save progress: {e.Message}");
            }
#else
            await Task.Yield();
#endif
        }
    }
}