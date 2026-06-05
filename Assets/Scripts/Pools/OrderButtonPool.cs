using System.Collections.Generic;
using DeliveryRushExam.Data;
using UnityEngine;
using System;

namespace DeliveryRushExam.UI
{
    public class OrderButtonPool : MonoBehaviour
    {
        [SerializeField] private OrderButtonView prefab;
        [SerializeField] private RectTransform container;
        [SerializeField] private int preloadCount = 6;

        private readonly Queue<OrderButtonView> available = new Queue<OrderButtonView>();

        private void Awake()
        {
            // Preload pool with inactive instances
            for (int i = 0; i < preloadCount; i++)
            {
                OrderButtonView view = Instantiate(prefab, container);
                view.gameObject.SetActive(false);
                available.Enqueue(view);
            }

            Debug.Log($"[OrderButtonPool] Preloaded {preloadCount} order buttons");
        }

        public OrderButtonView Get(OrderData order, Action<string> onComplete)
        {
            OrderButtonView view;

            if (available.Count > 0)
            {
                view = available.Dequeue();
            }
            else
            {
                // Pool exhausted - create extra instance
                Debug.LogWarning("[OrderButtonPool] Pool exhausted, creating extra instance");
                view = Instantiate(prefab, container);
            }

            view.gameObject.SetActive(true);
            view.Setup(order, onComplete);
            return view;
        }

        public void Return(OrderButtonView view)
        {
            view.gameObject.SetActive(false);
            available.Enqueue(view);
        }
    }
}