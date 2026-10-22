using UnityEngine.UI;
using UnityEngine;

namespace Game01.Combat.UI
{
    public class DecisionTimerView : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Text timerLabel;

        public void Configure(Slider timeSlider, Text label)
        {
            slider = timeSlider;
            timerLabel = label;
        }

        public void SetTime(float remaining, float max, string actorName = null)
        {
            gameObject.SetActive(true);

            if (slider != null)
            {
                slider.maxValue = max;
                slider.value = remaining;
            }

            if (timerLabel != null)
            {
                timerLabel.text = $"{actorName} {remaining:0.0}s";
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
