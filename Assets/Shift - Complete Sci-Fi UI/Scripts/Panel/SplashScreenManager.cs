using UnityEngine;

namespace Michsky.UI.Shift
{
    public class SplashScreenManager : MonoBehaviour
    {
        [Header("Resources")]
        public GameObject splashScreen;
        public GameObject mainPanels;
        public GameObject characterGameObject; // 캐릭터 게임 오브젝트 참조 추가

        private Animator splashScreenAnimator;
        private Animator mainPanelsAnimator;
        private TimedEvent ssTimedEvent;

        [Header("Settings")]
        public bool disableSplashScreen;
        public bool enablePressAnyKeyScreen;
        public bool enableLoginScreen;
        public bool showOnlyOnce = true;

        MainPanelManager mpm;

        void OnEnable()
        {
            mpm = FindObjectOfType<MainPanelManager>(); // MainPanelManager 찾기

            if (showOnlyOnce && GameObject.Find("[Shift UI - Splash Screen Helper]") != null) { disableSplashScreen = true; }
            if (splashScreenAnimator == null) { splashScreenAnimator = splashScreen.GetComponent<Animator>(); }
            if (ssTimedEvent == null) { ssTimedEvent = splashScreen.GetComponent<TimedEvent>(); }
            if (mainPanelsAnimator == null) { mainPanelsAnimator = mainPanels.GetComponent<Animator>(); }
            if (mpm == null) { mpm = gameObject.GetComponent<MainPanelManager>(); }

            if (disableSplashScreen == true)
            {
                splashScreen.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator.Play("Start");
                mpm.OpenFirstTab();
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Login");
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }

            if (showOnlyOnce == true && disableSplashScreen == false)
            {
                GameObject tempHelper = new GameObject();
                tempHelper.name = "[Shift UI - Splash Screen Helper]";
                DontDestroyOnLoad(tempHelper);
            }

            if (!disableSplashScreen)
            {
                Invoke("InitializeCharacterVisibility", 0.5f); // 스플래시 스크린 종료 후 캐릭터 초기화 지연
            }
        }

        void InitializeCharacterVisibility()
        {
            if (mpm.IsHomeScreenActive())
                characterGameObject.SetActive(true); // Home 화면이면 캐릭터 활성화
            else
                characterGameObject.SetActive(false); // Home 화면이 아니면 비활성화
        }

        public void LoginScreenCheck()
        {
            if (enableLoginScreen == true && enablePressAnyKeyScreen == true)
                splashScreenAnimator.Play("Press Any Key to Login");

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == true)
            {
                splashScreenAnimator.Play("Press Any Key to Loading");
                ssTimedEvent.StartIEnumerator();
            }

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == false)
            {
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }
    }
}