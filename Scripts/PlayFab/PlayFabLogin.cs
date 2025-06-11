using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Google;

public class PlayFabLogin : MonoBehaviour
{
    private bool isLoggingInFlag = false;

    // FIXME :: GooglePlay와 연동해서 GoogleEmail로 로그인하는걸로 교체해야함
    // https://www.youtube.com/watch?v=GTAROcfscDU&list=PL3KKSXoBRRW14-AYuFurtiDuYqfSPf1zQ&index=5

    // 비동기 호출의 경합 상태 (Race Condition) 방지를 위한 isLoggingInFlag 추가

    private void Awake()
    {
        LoginManager.Instance.SetPlayFabLogin(this);
    }

    public void LoginWithAndroidDeviceID(GoogleSignInUser user = null)
    {
        if (isLoggingInFlag) return; // 이미 로그인 요청 중인 경우

        isLoggingInFlag = true; // 로그인 요청 시작

        var request = new LoginWithAndroidDeviceIDRequest
        {
            AndroidDeviceId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true // 계정이 없는 경우 자동으로 생성
        };

        if (user == null)
        {
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, async result => await OnLoginSuccess(result), OnLoginFailure);
        }
        else // Google 연동하는 경우
        {
            PlayFabClientAPI.LoginWithAndroidDeviceID(request, async result => await OnLoginSuccess(result, user), OnLoginFailure);
        }
    }
    private async Task OnLoginSuccess(LoginResult result, GoogleSignInUser user = null)
    {
        isLoggingInFlag = false; // 로그인 성공 시 상태 리셋

        if (user != null)
        {
            bool isLinkSuccess = await LinkGoogleAccount(user);

            // 연동 실패 시 Google, PlayFab 로그아웃 후 메인 화면으로 복귀
            if (!isLinkSuccess)
            {
                GoogleSignIn.DefaultInstance.SignOut();
                GoogleSignIn.DefaultInstance.Disconnect();
                PlayFabClientAPI.ForgetAllCredentials();

                await Task.Delay(500);

                LoginManager.Instance.splashScreen.SetAutoLoginInteractable(true);
                // 오토 로그인 모드를 Guest로 복구
                PlayerPrefs.SetString("IsAutoLogin", "GuestLogin");
                return;
            }
        }

        // 서버 로그인이 완전히 성공된 이후에 splashScreen을 날려준다.
        Game.Instance.splashScreenLoader.DestroySplashScreen();

        // 기기 Id 갱신
        LoginManager.Instance.UpdateDeviceID();
        // 중복 로그인 탐지 시작
        LoginManager.Instance.StartCheckDuplicateLogin();

        PlayFabUserData.SetPlayFabId(result.PlayFabId);
        PlayFabUserData.SetLoggedIn(true);
        if (result.NewlyCreated)
        {
            InitialUserDataSetting();
            Debug.Log("NewAccount!!!!!!");
        }
        else
        {
            LoadDataAsync();
        }
        Debug.Log("Login Success!");
    }

    private Task<bool> LinkGoogleAccount(GoogleSignInUser user)
    {
        var isLinkSuccess = new TaskCompletionSource<bool>();

        var request = new LinkGoogleAccountRequest
        {
            ServerAuthCode = user.AuthCode,
            ForceLink = false
        };

        PlayFabClientAPI.LinkGoogleAccount(request,
            result =>
            {
                isLinkSuccess.SetResult(true);
                Debug.Log("Google 계정 연동 성공!");
            },
            error =>
            {
                isLinkSuccess.SetResult(false);
                Debug.LogError($"Google 계정 연동 실패: {error.GenerateErrorReport()}");
                Debug.LogError($"PlayFabErrorCode: {(int)error.Error}");

                switch (error.Error)
                {
                    case PlayFabErrorCode.LinkedAccountAlreadyClaimed:
                        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "이미 연동된 구글 계정입니다.");
                        break;
                    case PlayFabErrorCode.InvalidGoogleToken:
                        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "유효하지 않은 구글 인증 토큰입니다. 다시 로그인 해주세요.");
                        break;
                    case PlayFabErrorCode.LinkedDeviceAlreadyClaimed:
                        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "해당 기기가 이미 다른 계정에 연결되어 있습니다.");
                        break;
                    case PlayFabErrorCode.InvalidAuthToken:
                        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "유효하지 않은 인증 토큰입니다. 다시 로그인 해주세요.");
                        break;
                    case PlayFabErrorCode.NotAuthorized:
                        ObjectPoolManager.Instance.RequestObject("ShadowPopup", "Shadow_Popup",
                            "연결 권한이 없습니다.");
                        break;
                    default:
                        break;
                }
            });

        return isLinkSuccess.Task;
    }

    private void OnLoginFailure(PlayFabError error)
    {
        isLoggingInFlag = false; // 로그인 성공 시 상태 리셋

        LoginManager.Instance.HandlePlayFabLoginFailure();

        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    private void InitialUserDataSetting()
    {
        // CloudScript 함수를 호출하기 위한 요청 객체를 생성합니다.
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "saveTitleDataToUserInternalData", // 호출할 CloudScript 함수의 이름
            FunctionParameter = new { playerId = PlayFabUserData.GetPlayFabId() }, //PlayFabId를 파라미터로 송신
            GeneratePlayStreamEvent = true // PlayStream 이벤트 생성 여부
        };
        // CloudScript 함수를 호출합니다.
        PlayFabClientAPI.ExecuteCloudScript(request, InitialUserDataSettingFinished, OnLoginFailure);
    }
    private void InitialUserDataSettingFinished(ExecuteCloudScriptResult result)
    {
        LoadDataAsync();
    }
    private async void LoadDataAsync()
    {
        await DataManager.Instance.LoadDataAsync();
    }
}