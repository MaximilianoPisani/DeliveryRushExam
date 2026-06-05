using System.Collections.Generic;
using UnityEngine;

namespace DeliveryRushExam.UI
{
    public class ScorePopupPool : MonoBehaviour
    {
        [SerializeField] private ScorePopupView prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private int preloadCount = 5;

        private readonly Queue<ScorePopupView> available = new Queue<ScorePopupView>();

        private void Awake()
        {
            for (int i = 0; i < preloadCount; i++)
            {
                ScorePopupView popup = Instantiate(prefab, container);
                popup.gameObject.SetActive(false);
                available.Enqueue(popup);
            }

            Debug.Log($"[ScorePopupPool] Preloaded {preloadCount} score popups");
        }

        public ScorePopupView Get()
        {
            ScorePopupView popup;

            if (available.Count > 0)
            {
                popup = available.Dequeue();
            }
            else
            {
                Debug.LogWarning("[ScorePopupPool] Pool exhausted, creating extra instance");
                popup = Instantiate(prefab, container);
            }

            popup.gameObject.SetActive(true);
            return popup;
        }

        public void Return(ScorePopupView popup)
        {
            popup.gameObject.SetActive(false);
            available.Enqueue(popup);
        }
    }
}