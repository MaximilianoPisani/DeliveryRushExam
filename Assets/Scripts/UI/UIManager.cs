using System.Collections.Generic;
using DeliveryRushExam.Core;
using DeliveryRushExam.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeliveryRushExam.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private OrderManager orderManager;
        [SerializeField] private ScoreManager scoreManager;

        [Header("HUD")]
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text ordersCountText;

        [Header("Orders")]
        [SerializeField] private RectTransform ordersContainer;
        [SerializeField] private OrderButtonView orderButtonPrefab;

        [Header("Popups")]
        [SerializeField] private RectTransform popupsContainer;
        [SerializeField] private ScorePopupView scorePopupPrefab;

        [Header("Panels")]
        [SerializeField] private GameObject gameplayPanel;
        [SerializeField] private GameObject resultsPanel;
        [SerializeField] private TMP_Text resultsText;

        [Header("Pools")]
        [SerializeField] private OrderButtonPool orderButtonPool;
        [SerializeField] private ScorePopupPool scorePopupPool;

        private readonly List<OrderButtonView> orderViews = new List<OrderButtonView>();

        private void Awake()
        {
            if (gameManager == null)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }

            if (orderManager == null)
            {
                orderManager = FindFirstObjectByType<OrderManager>();
            }

            if (scoreManager == null)
            {
                scoreManager = FindFirstObjectByType<ScoreManager>();
            }
        }

        private void OnEnable()
        {
            orderManager.OrdersChanged += RefreshOrderList;
            scoreManager.OrderScored += ShowScorePopup;
            scoreManager.ScoreChanged += RefreshScoreHUD;
        }

        private void OnDisable()
        {
            if (orderManager != null) orderManager.OrdersChanged -= RefreshOrderList;
            if (scoreManager != null) scoreManager.OrderScored -= ShowScorePopup;
            if (scoreManager != null) scoreManager.ScoreChanged -= RefreshScoreHUD;
        }
        private void RefreshScoreHUD(int score, int coins, int completedOrders)
        {
            scoreText.text = $"Score: {score}";
            coinsText.text = $"Coins: {coins}";
            ordersCountText.text = $"Orders: {completedOrders}";
        }

        private void Update()
        {
            if (gameManager == null) return;

            timerText.text = $"Time: {Mathf.CeilToInt(gameManager.RemainingTime)}";
        }

        public void ShowGameplay()
        {
            gameplayPanel.SetActive(true);
            resultsPanel.SetActive(false);
            RefreshOrderList();
        }

        public void ShowResults(int score, int coins, int completedOrders, PlayerProgressData progressData)
        {
            gameplayPanel.SetActive(false);
            resultsPanel.SetActive(true);

            resultsText.text = $"Delivery Rush Results\n" +
                               $"Score: {score}\n" +
                               $"Coins earned: {coins}\n" +
                               $"Completed orders: {completedOrders}\n" +
                               $"Best score: {progressData.bestScore}\n" +
                               $"Total coins: {progressData.totalCoins}";
        }

        private void RefreshOrderList()
        {
            // Return all active views to pool
            for (int i = 0; i < orderViews.Count; i++)
                orderButtonPool.Return(orderViews[i]);

            orderViews.Clear();

            IReadOnlyList<OrderData> orders = orderManager.ActiveOrders;
            for (int i = 0; i < orders.Count; i++)
            {
                OrderButtonView view = orderButtonPool.Get(orders[i], orderManager.CompleteOrder);
                orderViews.Add(view);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(ordersContainer);
        }

        private void ShowScorePopup(OrderData order)
        {
            ScorePopupView popup = scorePopupPool.Get();
            popup.transform.localPosition = new Vector3(
                Random.Range(-90f, 90f),
                Random.Range(-25f, 35f),
                0f);
            popup.Setup($"+{order.rewardPoints} points", scorePopupPool);
        }

    }
}
