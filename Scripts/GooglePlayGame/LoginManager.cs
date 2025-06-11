using UnityEngine;
using Google;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.DataModels;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Defective.JSON;
using TMPro;

public class LoginManager : MonoBehaviour
{
    // Singleton Instance
    private static LoginManager _instance;
    public static LoginManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 인스턴스를 찾거나 없으면 생성
                _instance = FindObjectOfType<LoginManager>();
                if (_instance == null)
                {
                    Debug.Log("CreateLoginManager");
                    GameObject singletonObject = new GameObject("LoginManager");
                    _instance = singletonObject.AddComponent<LoginManager>();
                }
            }
            return _instance;
        }
    }
    // request flag
    private bool isRequestInProgress = false;
    private bool isAuthenticated = false;

    public CustomSplashScreen splashScreen;
    public PlayFabLogin playFabLogin;

    // Google SignIn Configuration Define
    private GoogleSignInConfiguration configuration;

    private string deviceId;
    private System.Collections.IEnumerator checkDuplicateLogIn;

    // 중복 로그인 체크 간격
    public float checkDuplicateTimer = 15f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // 중복된 인스턴스 제거
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }

        deviceId = SystemInfo.deviceUniqueIdentifier;
    }

    public async Task AndroidGameStartWithGoogleLogin(int loginType)
    {
        GoogleSignInUser user =  await LoginWithGoogle(loginType);
        
        if (user == null)
        {
            // 오류 팝업 출력 필요
            HandleGoogleLoginFailure();
            return;
        }

        PlayerPrefs.SetString("IsAutoLogin", "GoogleLogin");
        LoginWithPlayFab(user.AuthCode);
    }

    // Google 로그인 시작 지점
    private async Task<GoogleSignInUser> LoginWithGoogle(int loginType) // loginType : 0 (최초), 1(자동 로그인)
    {
        // WebClientId를 외부 설정 파일이나 서버에서 가져오는 방법
        string webClientId = "1096344974350-kjq8df5fudp1ecucoo8r2okjt62gt3vo.apps.googleusercontent.com";
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestEmail = true,
            RequestIdToken = true,
            RequestAuthCode = true // AuthCode 요청
        };
        // GoogleSignIn 초기화
        GoogleSignIn.Configuration = configuration;

        try
        {
            GoogleSignInUser user;

            if (loginType == 0)
            {
                // GoogleSignIn 로그인 시도 (최초 로그인, 계정 선택)
                user = await GoogleSignIn.DefaultInstance.SignIn();
            }
            else // loginType == 1
            {
                // GoogleSignIn 로그인 시도 (최초 로그인, 계정 선택)
                user = await GoogleSignIn.DefaultInstance.SignInSilently();
            }

            return user;
        }
        catch(Exception e)
        {
            Debug.LogError($"Google Login Failed: {e}");
            return null;
        }
    }
    /*
    private void OnGoogleSignIn(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted || task.IsCanceled)
        {
            // 오류 팝업 출력 필요
            HandleGoogleLoginFailure();
            return;
        }
        string idToken = task.Result.IdToken;
        string serverAuthCode = task.Result.AuthCode;
        LoginWithPlayFab(serverAuthCode);
    }
    */
    public void GoogleSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        Debug.Log("Google 계정 로그아웃");
    }

    public async Task LoginWithLinkGoogleAccount(int loginType)
    {
        GoogleSignInUser user = await LoginWithGoogle(loginType);

        if (user == null)
        {
            // 오류 팝업 출력 필요
            HandleGoogleLoginFailure();
            return;
        }

        PlayerPrefs.SetString("IsAutoLogin", "GoogleLogin");
        playFabLogin.LoginWithAndroidDeviceID(user);
    }

    private void LoginWithPlayFab(string serverAuthCode)
    {
        PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest
        {
            TitleId = PlayFabSettings.TitleId,
            ServerAuthCode = serverAuthCode,
            CreateAccount = true
        },
        result => 
        {
            // 서버 로그인이 완전히 성공된 이후에 splashScreen을 날려준다.
            Game.Instance.splashScreenLoader.DestroySplashScreen();

            PlayFabUserData.SetPlayFabId(result.PlayFabId);
            PlayFabUserData.SetLoggedIn(true);

            // 기기 Id 갱신
            UpdateDeviceID();
            // 중복 로그인 탐지 시작
            checkDuplicateLogIn = checkDuplicateLogin();
            StartCheckDuplicateLogin();

            Debug.Log("StartCheckDuplicateLogin in LoginWtihPlayFab");
            
            if(result.NewlyCreated)
            {
                InitialUserDataSetting(); // 새 계정 초기화
            }
            else
            {
                Debug.Log("LoadDataAsync in LoginWithPlayFab");
                LoadDataAsync(); // 기존 계정 데이터 로드
            }
        },
        error => 
        {
            // 에러 팝업 출력 해줘야 좋을 것 같음. 재시도 요청
            Debug.LogError("PlayFab Login Failed: " + error.GenerateErrorReport());

            HandlePlayFabLoginFailure();
        });
    }

    public void UpdateDeviceID()
    {
        //var request = new updateus

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "registerDevice",
            FunctionParameter = new Dictionary<string, object> {
                { "deviceId", deviceId }
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result => 
        {
            JSONObject jsonResult = new JSONObject(result.FunctionResult.ToString());

            bool isDuplicate = jsonResult["isDuplicate"].boolValue;

            Debug.Log("CloudScript 실행 완료. 중복 여부: " + isDuplicate);
            
        }, 
        error => 
        {
            Debug.LogError("CloudScript 실행 실패: " + error.GenerateErrorReport());
        });
    }

    public void CheckDuplicateLogin()
    {
        Debug.Log("CheckDuplicateLogin()");

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "checkDuplicateLogin",
            FunctionParameter = new Dictionary<string, object>
            {
                { "deviceId", deviceId }
            }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, result =>
        {
            // 계속 new해서 생성하면 메모리 낭비인데...
            JSONObject jsonResult = new JSONObject(result.FunctionResult.ToString());

            Debug.Log("FunctionResult 원본: " + result.FunctionResult?.ToString());

            bool isDuplicate = jsonResult["isDuplicate"].boolValue;

            Debug.Log($"previousDeviceId : {jsonResult["previousDeviceId"]}");
            Debug.Log($"currentDeviceId : {jsonResult["currentDeviceId"]}");

            if (isDuplicate)
            {
                HandleDuplicateLogin();
            }
            
        },
        error =>
        {
            Debug.LogError("CloudScript 실행 실패: " + error.GenerateErrorReport());
        });
    }

    private void HandleDuplicateLogin()
    {
        // 경고 팝업 또는 알림 UI
        Debug.Log("다른 기기에서 로그인되어 로그아웃됩니다.");

        // 중복 로그인 체크 중지
        StopCheckDuplicateLogin();

        // 로그아웃 처리
        PlayFabClientAPI.ForgetAllCredentials();

        // UIManager에서 로그아웃 경고 팝업 출력
        if (UIManager.Instance != null)
        {
            UIManager.Instance.warningPopupUI.SetActive(true);

            // 경고 팝업 버튼 세팅
            UIManager.Instance.warningPopup.ClearButtonListener();
            UIManager.Instance.warningPopup.AddButtonListener(() =>
            {
                UIManager.Instance.warningPopup.ReturnToTitle();
            });
        }
    }

    private System.Collections.IEnumerator checkDuplicateLogin()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(checkDuplicateTimer);

            CheckDuplicateLogin();
        }
    }

    public void StartCheckDuplicateLogin()
    {
        if (checkDuplicateLogIn != null)
        {
            StartCoroutine(checkDuplicateLogIn);
        }
    }

    public void StopCheckDuplicateLogin()
    {
        StopCoroutine(checkDuplicateLogIn);
        checkDuplicateLogIn = null;
    }

    // Google Play 로그인 및 PlayFab 처리 통합 메서드
    public void LoginWithGooglePlay()
    {
        InitializeGooglePlayGames(HandleGooglePlayLoginResult);
    }
    // Google Play 인증 후 PlayFab 로그인 처리
    private void HandleGooglePlayLoginResult(bool success)
    {
        if (success)
        {
            Debug.Log("Google Play login successful. Proceeding with PlayFab login...");
            GetIdTokenAndLoginToPlayFab(HandlePlayFabLoginResult);
        }
        else
        {
            // FIXME :: 기기에 구글 계정이 설정되어 있지 않은 경우, UI 처리 필요
            Debug.LogError("Google Play login failed.");
        }
    }
    // PlayFab 로그인 결과 처리
    private void HandlePlayFabLoginResult(bool playFabSuccess)
    {
        if (playFabSuccess)
        {
            Debug.Log("PlayFab login successful!");
        }
        else
        {
            Debug.LogError("PlayFab login failed.");
        }
    }

    // Google Play Games 초기화 및 로그인 수행
    public void InitializeGooglePlayGames(Action<bool> callback)
    {
        if (PlayGamesPlatform.Instance != null)
        {
            Debug.Log("Google Play Games is already activated.");
        }
        else
        {
            PlayGamesPlatform.Activate();
        }

        PlayGamesPlatform.Instance.Authenticate((signInStatus) =>
        {
            bool success = signInStatus == SignInStatus.Success;

            if (success)
            {
                isAuthenticated = true;
                Debug.Log("Google login successful.");
            }
            else
            {
                Debug.LogError("Google login failed: " + signInStatus);
            }

            callback?.Invoke(success); // 로그인 결과 콜백 호출
        }); // 두 번째 인자 'true'는 자동 로그인 비활성화, 'false'는 자동 UI 표시 안 함
    }

    // 로그인 상태 확인
    public bool IsAuthenticated()
    {
        return isAuthenticated;
    }

    // Google 인증 후 PlayFab 로그인
    public void GetIdTokenAndLoginToPlayFab(Action<bool> callback)
    {
        if (!isAuthenticated)
        {
            Debug.LogError("User not authenticated with Google.");
            callback?.Invoke(false);
            return;
        }
        if (isRequestInProgress)
        {
            Debug.LogWarning("Server auth code request already in progress.");
            return;
        }
        isRequestInProgress = true;
        Debug.Log("Requesting Server Side Access from Google...");
        PlayGamesPlatform.Instance.RequestServerSideAccess(true, (serverAuthCode) =>
        {
            if (!string.IsNullOrEmpty(serverAuthCode))
            {
                Debug.Log("Received Server Auth Code: " + serverAuthCode);
                LoginWithPlayFab(serverAuthCode, callback);
            }
            else
            {
                Debug.LogError("Failed to retrieve Server Auth Code.");
                callback?.Invoke(false);
            }
        });
    }

    // PlayFab 로그인 처리
    private void LoginWithPlayFab(string serverAuthCode, Action<bool> callback)
    {
        var request = new LoginWithGooglePlayGamesServicesRequest
        {
            ServerAuthCode = serverAuthCode,
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = true
        };

        Debug.Log("Logging into PlayFab with Google account...");
        PlayFabSettings.LogLevel = PlayFabLogLevel.Debug;
        PlayFabClientAPI.LoginWithGooglePlayGamesServices(request,
            result =>
            {
                Debug.Log("PlayFab Login Successful! User ID: " + result.PlayFabId);
                // PlayFab ID 저장
                PlayFabUserData.SetPlayFabId(result.PlayFabId);
                PlayFabUserData.SetLoggedIn(true);

                if (result.NewlyCreated)
                {
                    Debug.Log("New account detected. Initializing user data...");
                    InitialUserDataSetting(); // 새 계정 초기화
                }
                else
                {
                    Debug.Log("Existing account detected. Loading user data...");
                    LoadDataAsync(); // 기존 계정 데이터 로드
                }
                callback?.Invoke(true);
            },
            error =>
            {
                Debug.LogError("PlayFab Login Failed: " + error.GenerateErrorReport());
                callback?.Invoke(false);
            });
    }
    // 초기 사용자 데이터 설정
    private void InitialUserDataSetting()
    {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "saveTitleDataToUserInternalData", // 호출할 CloudScript 함수 이름
            FunctionParameter = new { playerId = PlayFabUserData.GetPlayFabId() }, // PlayFab ID 송신
            GeneratePlayStreamEvent = true
        };

        PlayFabClientAPI.ExecuteCloudScript(request, InitialUserDataSettingFinished, OnCloudScriptError);
    }

    private void InitialUserDataSettingFinished(ExecuteCloudScriptResult result)
    {
        Debug.Log("Initial user data successfully set.");
        LoadDataAsync(); // 초기화 후 데이터 로드
    }

    private void OnCloudScriptError(PlayFabError error)
    {
        Debug.LogError("CloudScript execution failed: " + error.GenerateErrorReport());
    }

    // 데이터 로드 비동기 함수
    private async void LoadDataAsync()
    {
        Debug.Log("Loading user data...");
        await DataManager.Instance.LoadDataAsync();
        Debug.Log("User data loaded.");
    }

    public void HandleGoogleLoginFailure()
    {
        Debug.LogError("Google Sign-In failed.");

        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "구글 로그인 실패!");

        // 구글 로그인 실패 시 로그인 버튼을 화면에 띄움
#if UNITY_ANDROID && !UNITY_EDITOR
            splashScreen.autoLoginButton.gameObject.SetActive(false);
            splashScreen.signInWithGoogleLoginButton.gameObject.SetActive(true);
            splashScreen.signInWithGuestLoginButton.gameObject.SetActive(true);
#endif
    }

    public void HandlePlayFabLoginFailure()
    {
        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "PlayFab 로그인 실패!");

#if UNITY_ANDROID && !UNITY_EDITOR
            splashScreen.autoLoginButton.gameObject.SetActive(false);
            splashScreen.signInWithGoogleLoginButton.gameObject.SetActive(true);
            splashScreen.signInWithGuestLoginButton.gameObject.SetActive(true);
#elif UNITY_EDITOR
        splashScreen.signInWithGuestLoginButton.gameObject.SetActive(true);
#endif
    }

    public void SetSplashScreen(CustomSplashScreen splashScreen)
    {
        this.splashScreen = splashScreen;
    }

    public void SetPlayFabLogin(PlayFabLogin playFabLogin)
    {
        this.playFabLogin = playFabLogin;
    }
}