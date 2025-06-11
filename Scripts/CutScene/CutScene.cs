using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Defective.JSON;
using DG.Tweening;

public class CutScene : MonoBehaviour
{
    public Image currentScene;
    public CanvasRenderer fadePanel;            // fade 효과를 적용하는데 필요한 패널
    public Button skipButton;
    public List<CutSceneData> cutSceneDatas;

    private Camera mainCamera;

    private float touchDelayTime = 1.5f;
    private int currentSceneNumber;
    CutSceneData currentCutSceneData;
    private GameObject activatedEffect;

    private IEnumerator floatScene;
    private IEnumerator cameraZoom;
    private IEnumerator fadeIn;
    private IEnumerator fadeOut;

    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        fadePanel.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);

        currentSceneNumber = 0;
    }

    private void Start()
    {
        floatScene = FloatScene(currentSceneNumber);
        StartCoroutine(floatScene);
    }

    private void InitializeMainCamera()
    {
        // README :: 메인 카메라 리셋을 자주 사용할 것 같아서 이렇게 수정해봤는데, 확인해주세요
        Game.Instance.SetMainCameraReset();
        // perspective로 사용하는 경우가 생겼으므로, orthographic으로 초기화
        mainCamera.orthographic = true; // orthographic으로 변경
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f)); // 카메라 각도 초기화

        //mainCamera.transform.position = 10f * Vector3.back;
        //mainCamera.orthographicSize = 5;
    }

    public void OnClickScene()
    {
        AudioManager.Instance.PlaySFX("event:/sfx_common_button_05", new Vector3(0,0,0));
        // 컷신이 남아있으면 다음 컷신을 띄운다
        if (currentSceneNumber < cutSceneDatas.Count)
        {
            if (floatScene != null)
            {
                StartCoroutine(floatScene);
                floatScene = null;
            }
        }
        // 더 이상 없으면 컷신 비활성화
        else
        {
            CloseCutScene();
        }
    }

    public void OnClickSkipButton()
    {
        // 스킵 버튼 클릭 시 바로 컷신 비활성화
        CloseCutScene();
    }

    private void CloseCutScene()
    {
        // 메인 카메라 초기화
        InitializeMainCamera();

        this.gameObject.SetActive(false);
        MoveToStage();
    }

    private void MoveToStage()
    {
        // 네비게이터 UI 표시
        UIManager.Instance.ShowNavigatorUI();

        int progress = UserParameterData.GetUserParameter("progress");

        // 진척도가 (최초 구동)일 때 = 프롤로그 마친 후
        if (progress == ConstantData.GetConstant("prologueprogress"))
        {
            // 프롤로그인 경우, 스테이지로 즉시 진입시켜줘야 한다.
            UIManager.Instance.ShowStageUI();
            UIManager.Instance.InitializeInGameStage();

            // 스테이지 BGM으로 변경
            AudioManager.Instance.PlayBGM("event:/bgm_ingame_02", new Vector3(0,0,0));

            //// 유저의 progress 정보 갱신 ////
            // 다이얼로그 생성과 동시에 서버 progress 정보 갱신 요청
            IncreaseProgress(1);
        }
        // 에필로그가 끝나면?
        // README :: 에필로그는 프롤로그와 달리, 마지막 progress의 Dialogue를 전부 읽고 난 다음에 출력될 예정입니다.
        // README :: 즉, 에필로그는 DialogueManager에서 Epilogue 프리팹을 instantiate
        // README :: 에필로그까지 보고 나면 로비로 되돌아 오게 됨.
        // FIXME :: 최종적으로 Epilogue 마지막 장면을 보면, 축하의 씬 이후 축하 팝업까지 보여주면 좋을 것 같은데...
        // Dialogue 출력의 progress가 epilogue progress인 경우, 에필로그 컷신 출력
        if (progress == ConstantData.GetConstant("epilogueprogress")) // Cutscene 상태에서는 progress가 +1 추가 갱신된 상태로 비교된다.
        {
            // 로비 BGM으로 변경
            AudioManager.Instance.PlayBGM("event:/bgm_lobby_01", new Vector3(0,0,0));
            // FIXME :: 보상 팝업 창은 생략해야할 듯한데, 우리 게임 특성 상 스테이지 순서가 정해져 있지가 않다.
            // FIXME :: stage보상 팝업 창을 분리하거나....(이건 손이 많이갈 것 같고) 그냥 이대로 둔다? → 모험 누르면 보상이 그제서야 뜸. 다시 스테이지 꺴을 때 보상 팝업 창 정상 동작하는지 확인 필요함
            UIManager.Instance.ShowLobbyUI();
            Debug.Log("Epilogue :: The End.");
        }
    }

    private IEnumerator FloatScene(int sceneNumber)
    {
        // 카메라에 적용중인 코루틴 정지
        StopAllCameraCoroutine();

        // 카메라 초기화
        InitializeMainCamera();
        // 이전 컷신에서 활성화 중인 이펙트 비활성화
        if (activatedEffect != null)
        {
            activatedEffect.SetActive(false);
            activatedEffect = null;
        }

        // 두번째 씬부터 skipButton 활성화
        if (currentSceneNumber > 0)
            skipButton.gameObject.SetActive(true);

        // fade 패널 비활성화
        if (fadePanel.gameObject.activeSelf)
            fadePanel.gameObject.SetActive(false);

        //Debug.Log(sceneNumber + 1 + "번째 씬 출력");
        currentCutSceneData = cutSceneDatas[sceneNumber];

        // BGM 변경이 있다면 적용
        if(!string.IsNullOrEmpty(currentCutSceneData.bgmEventPath))
        {
            AudioManager.Instance.PlayBGM(currentCutSceneData.bgmEventPath, new Vector3(0,0,0));
        }
        // 신 전용 효과음 적용
        if(!string.IsNullOrEmpty(currentCutSceneData.sfxEventPath))
        {
            AudioManager.Instance.PlaySFX(currentCutSceneData.sfxEventPath, new Vector3(0,0,0));
        }
        

        // 스프라이트 변경 전 fadeOut이 설정됐다면 먼저 적용
        if (currentCutSceneData.fadeOutData.isFadeOut)
        {
            fadeOut = FadeOut(currentCutSceneData.fadeOutData);
            StartCoroutine(fadeOut); // README :: yield return 부분 임시 삭제
        }
        
        currentScene.sprite = currentCutSceneData.cutSceneSprite;   // 스프라이트 변경
        currentScene.GetComponent<RectTransform>().sizeDelta = currentScene.sprite.rect.size;   // 컷신 크기 조정

        // 현재 컷신에 적용할 이펙트가 있다면 이펙트 활성화
        if (currentCutSceneData.effect != null)
        {
            activatedEffect = currentCutSceneData.effect;
            activatedEffect.SetActive(true);
        }

        if (currentCutSceneData.cameraMove.isMove)
        {
            //StartCoroutine(CameraMove(currentCutSceneData.cameraMove));
            CameraMove(currentCutSceneData.cameraMove);
        }

        if (currentCutSceneData.cameraShake.isShake)
        {
            //StartCoroutine(CameraShake(currentCutSceneData.cameraShake));
            CameraShake(currentCutSceneData.cameraShake);
        }

        if (currentCutSceneData.cameraZoom.isZoom)
        {
            cameraZoom = CameraZoom(currentCutSceneData.cameraZoom);
            StartCoroutine(cameraZoom);
        }
        else
        {
            // Coroutine으로 가지 않더라도, StartProjectionSize로 최초 변경은 하도록 수정
            mainCamera.orthographicSize = currentCutSceneData.cameraZoom.startProjectionSize;
        }

        if (currentCutSceneData.fadeInData.isFadeIn)
        {
            fadeIn = FadeIn(currentCutSceneData.fadeInData);
            StartCoroutine(fadeIn);
        }

        yield return StartCoroutine(TouchDelay());

        yield break;
    }

    private void StopAllCameraCoroutine()
    {
        // 카메라에 적용 중인 DOTween 종료 (move, shake)
        mainCamera.transform.DOComplete();

        // 카메라 줌, 페이드 인, 페이드 아웃 종료
        if (cameraZoom != null)
            StopCoroutine(cameraZoom);

        if (fadeIn != null)
            StopCoroutine(fadeIn);

        if (fadeOut != null)
            StopCoroutine(fadeOut);
    }

    // 터치 후 일정 시간동안 터치해도 다음 신으로 넘어가지 않게 딜레이 적용
    private IEnumerator TouchDelay()
    {
        float deltaTime = 0f;

        while(deltaTime <= touchDelayTime)
        {
            deltaTime += Time.deltaTime;
            yield return null;
        }

        
        floatScene = FloatScene(++currentSceneNumber);
        yield break;
    }
    
    private void CameraMove(CutSceneData.CameraMoveData cameraMoveData)
    {
        Ease ease = SetEase(cameraMoveData.moveTransitionType);

        mainCamera.transform.position = cameraMoveData.startPosition;
        mainCamera.transform.DOMove(cameraMoveData.endPosition, 1f / cameraMoveData.moveSpeed).SetEase(ease);
    }

    private void CameraShake(CutSceneData.CameraShakeData cameraShakeData)
    {
        Ease ease = SetEase(cameraShakeData.shakeTransitionType);

        mainCamera.transform.DOShakePosition(cameraShakeData.shakeDurationTime, cameraShakeData.shakeIntensity).SetEase(ease);
    }

    private IEnumerator CameraZoom(CutSceneData.CameraZoomData cameraZoomData)
    {
        float zoomTime = cameraZoomData.zoomTime; // Camera가 Zoom되는데 걸리는 전체 시간
        float progressTime = 0f; // 경과 시간 추가

        while (progressTime < zoomTime)
        {
            progressTime += Time.deltaTime;

            mainCamera.orthographicSize = 
                Mathf.Lerp(cameraZoomData.startProjectionSize, 
                           cameraZoomData.endProjectionSize, 
                           progressTime * ApplyTransition(cameraZoomData.zoomSpeed, cameraZoomData.zoomTransitionType)); // ZoomSpeed를 통해 변화 속도를 정할 수 있음
            yield return null;
        }

        cameraZoom = null;
        yield break;
    }

    private IEnumerator FadeIn(CutSceneData.FadeInData fadeInData)
    {
        // fade 패널 활성화
        fadePanel.gameObject.SetActive(true);

        float deltaTime = 0f;

        while (deltaTime < fadeInData.fadeTime)
        { 
            fadePanel.SetAlpha(Mathf.Lerp(1f, 0f, deltaTime / fadeInData.fadeTime));

            deltaTime += Time.deltaTime;

            yield return null;
        }

        fadeIn = null;
        yield break;
    }

    private IEnumerator FadeOut(CutSceneData.FadeOutData fadeOutData)
    {
        // fade 패널 활성화
        fadePanel.gameObject.SetActive(true);

        float deltaTime = 0f;

        while (deltaTime < fadeOutData.fadeTime)
        {
            fadePanel.SetAlpha(Mathf.Lerp(0f, 1f, deltaTime / fadeOutData.fadeTime));

            deltaTime += Time.deltaTime;

            yield return null;
        }

        fadeOut = null;
        yield break;
    }

    private float ApplyTransition(float time, CutSceneData.TransitionType transitionType)
    {
        switch (transitionType)
        {
            case CutSceneData.TransitionType.LINEAR:
                return Linear(time);
            case CutSceneData.TransitionType.EASE_IN:
                return EaseIn(time);
            case CutSceneData.TransitionType.EASE_OUT:
                return EaseOut(time);
            default:
                Debug.LogError("Transition 실패! : 해당 타입이 없거나 설정되지 않았습니다.");
                break;
        }

        return 0f;
    }

    private Ease SetEase(CutSceneData.TransitionType transitionType)
    {
        switch (transitionType)
        {
            case CutSceneData.TransitionType.LINEAR:
                return Ease.Linear;
            case CutSceneData.TransitionType.EASE_IN:
                return Ease.InQuad;
            case CutSceneData.TransitionType.EASE_OUT:
                return Ease.OutQuad;
            default:
                return Ease.Linear;
        }
    }

    private float Linear(float time)
    {
        return time;
    }

    private float EaseIn(float time)
    {
        return time * time;
    }

    private float EaseOut(float time)
    {
        return time * (2 - time);
    }
    public void IncreaseProgress(int num)
    {
        // Progress를 num만큼 증가하는 요청입니다.
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest()
        {
            FunctionName = "increaseProgress", // 호출할 CloudScript 함수의 이름
            FunctionParameter = new { playFabId = PlayFabUserData.GetPlayFabId(), increaseNum = num }, // 함수에 전달할 매개변수
            GeneratePlayStreamEvent = true // 선택 사항 - 이 이벤트를 PlayStream에서 보여줍니다.
        };
        PlayFabClientAPI.ExecuteCloudScript(request,
        // Success 콜백 함수
        (result) =>
        {
            OnCloudScriptSuccessIncreaseProgress(result);
        },
        // Failure 콜백 함수
        OnCloudScriptFailure
        );
    }
    private void OnCloudScriptSuccessIncreaseProgress(ExecuteCloudScriptResult result)
    {
        // CloudScript 결과에서 FunctionResult를 가져옴
        JSONObject functionResult = new JSONObject(result.FunctionResult.ToString());
        Debug.Log(functionResult);
        Debug.Log(functionResult["progress"]);

        // 서버 갱신이 완료된 이후, 클라이언트 데이터 상의 UserParameterData의 progress도 함께 갱신시켜 준다.
        UserParameterData.UpdateUserParameterData("progress", functionResult["progress"].intValue);

        // instantiate된 DialogueUI가 이미 있는지 확인
        GameObject existingDialogueUI = GameObject.Find("DialogueUI(Clone)");
        if (existingDialogueUI != null)
        {
            // 이미 존재하는 경우, enable 처리
            existingDialogueUI.SetActive(true);
            // OnEnable에서 자동으로 진행
        }
        else
        {
            // 존재하지 않는 경우, instantiate
            GameObject loadedPrefab = Resources.Load<GameObject>("DialogueUI");
            GameObject instantiatePrefab = Instantiate(loadedPrefab, Vector3.zero, Quaternion.identity);
            instantiatePrefab.transform.SetParent(UIManager.Instance.gameObject.transform);
            instantiatePrefab.GetComponent<Canvas>().worldCamera = UIManager.Instance.GetUICamera(); // UI 카메라를 연결
        }
    }
    private void OnCloudScriptFailure(PlayFabError error)
    {
        // 오류 로그를 출력합니다.
        Debug.LogError("CloudScript 호출 실패: " + error.GenerateErrorReport());
    }
}
