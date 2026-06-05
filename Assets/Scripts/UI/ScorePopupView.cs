using TMPro;
using UnityEngine;

namespace DeliveryRushExam.UI
{
    public class ScorePopupView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float lifetime = 1.1f;
        [SerializeField] private float moveSpeed = 55f;

        private float age;
        private CanvasGroup canvasGroup;
        private ScorePopupPool pool;

        private void Awake()
        {
            // Cache component once instead of every frame
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Setup(string message, ScorePopupPool ownerPool)
        {
            age = 0f;
            messageText.text = message;
            pool = ownerPool;
        }

        private void Update()
        {
            age += Time.deltaTime;
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

            if (canvasGroup != null)
                canvasGroup.alpha = 1f - age / lifetime;

            if (age >= lifetime)
            {
                // Return to pool instead of destroying
                transform.localPosition = Vector3.zero;
                pool.Return(this);
            }
        }
    }
}