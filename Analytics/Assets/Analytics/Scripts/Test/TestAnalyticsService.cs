using Analytics.Scripts.Analytics;
using Analytics.Scripts.LocalStorage;
using MEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Analytics.Scripts.Test
{
    public class TestAnalyticsService : MonoBehaviour
    {
        const string PlayerPrefsKeyServerURL = "ServerURL";

        [SerializeField]
        private TMP_InputField _serverURLInputField;

        [SerializeField]
        private Button _restartAnalyticsServiceBtn;


        [SerializeField]
        private Button _levelStartedBtn;

        [SerializeField]
        private Button _rewardGivenBtn;

        [SerializeField]
        private Button _coinsSpentBtn;


        [SerializeField]
        private TextMeshProUGUI _countdownText;

        [SerializeField]
        private TextMeshProUGUI _localEventsText;

        [SerializeField]
        private TextMeshProUGUI _responseText;


        [SerializeField]
        private float _cooldownBeforeSend = 1f;


        private FileLocalStorage _localStorage;
        private AnalyticsService _analyticsService;

        private float _countdown;
        private CoroutineHandle _countdownCoroutineHandle;

        private void Start()
        {
            _localStorage = FileLocalStorage.Instance;
            _localStorage.OnEventsCleared += OnLocalStorageCleared;
            _localStorage.OnEventAdded += OnAddedEventToLocalStogage;

            _serverURLInputField.text = PlayerPrefs.GetString(PlayerPrefsKeyServerURL, "");
            _serverURLInputField.onValueChanged.AddListener(delegate(string newServerURL) { PlayerPrefs.SetString(PlayerPrefsKeyServerURL, newServerURL); });

            _analyticsService = AnalyticsService.Instance;
            _analyticsService.Init(_serverURLInputField.text, _cooldownBeforeSend, _localStorage);
            _analyticsService.OnCountdownStarted += OnCountdownStarted;
            _analyticsService.OnResponse += OnResponse;

            if (!string.IsNullOrWhiteSpace(_serverURLInputField.text))
                _analyticsService.ForceSendingEvents();

            _restartAnalyticsServiceBtn.onClick.AddListener(OnRestartAnalyticsService);
            _levelStartedBtn.onClick.AddListener(OnLevelStarted);
            _rewardGivenBtn.onClick.AddListener(OnRewardGiven);
            _coinsSpentBtn.onClick.AddListener(OnCoinsSpent);
        }

        private void OnLocalStorageCleared()
        {
            _localEventsText.text = "";
        }

        private void OnAddedEventToLocalStogage(string addedAnalyticsEvent)
        {
            _localEventsText.text += $"\n{addedAnalyticsEvent}";
        }

        private void OnCountdownStarted(float countdown)
        {
            _countdown = countdown;

            Timing.KillCoroutines(_countdownCoroutineHandle);
            _countdownCoroutineHandle = Timing.CallPeriodically(countdown + 1f, 1f, OnCountdownTick);
        }

        private void OnCountdownTick()
        {
            _countdown -= 1f;

            if (_countdown <= 0)
            {
                _countdownText.text = "--:--";
                return;
            }

            var seconds = Mathf.FloorToInt(_countdown);
            var minutes = seconds / 60;
            seconds %= 60;

            _countdownText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
        }

        private void OnResponse(string response)
        {
            _responseText.text = response;
        }

        private void OnRestartAnalyticsService()
        {
            _analyticsService.Init(_serverURLInputField.text, _cooldownBeforeSend, _localStorage);
            _analyticsService.ForceSendingEvents();
        }

        void OnLevelStarted()
        {
            var dummyLevel = (int)Time.time % 10;

            _analyticsService.TrackEvent("levelStart", $"level:{dummyLevel}");
        }

        void OnRewardGiven()
        {
            var dummyReward = Time.frameCount;

            _analyticsService.TrackEvent("reward", $"coins:{dummyReward}");
        }

        void OnCoinsSpent()
        {
            var dummyCoinsSpent = Time.frameCount;

            _analyticsService.TrackEvent("spent", $"coins:{dummyCoinsSpent}");
        }
    }
}