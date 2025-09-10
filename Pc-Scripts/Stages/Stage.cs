using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stage : IDataFromDto<StageDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private string _key;
    [SerializeField] private StageMode _mode;
    [SerializeField] private StageDifficulty _difficulty;
    [SerializeField] private int[] _firstGimmick;
    [SerializeField] private int[] _repeatGimmickList;
    [SerializeField] private string _world;
    [SerializeField] private int _targetScore;
    //[SerializeField] private int _limitScore;
    [SerializeField] private float[] _targetScoreRatio;
    //[SerializeField] private int _limitTurnCount;
    //[SerializeField] private float _limitTime;
    //[SerializeField] private int _perfectCount;
    [SerializeField] private int _requiredProgress;
    //[SerializeField] private DropColor _bonusColor;
    //[SerializeField] private int _bonusColorScore;
    [SerializeField] private int _boardShapeId;
    [SerializeField] private string _bosskey;
    [SerializeField] private AchievementDropType[] _achievementDropType;
    protected StageBehaviour m_stageBehaviour;

    public Stage() {  }
    public void FromDto(StageDTO dto)
    {
        _id = dto.id;
        _key = dto.key;
        _mode = (StageMode)System.Enum.Parse(typeof(StageMode), dto.mode);
        _difficulty = (StageDifficulty)System.Enum.Parse(typeof(StageDifficulty), dto.difficulty);
        _firstGimmick = dto.firstgimmick;
        _repeatGimmickList = dto.repeatgimmick;
        _world = dto.world;
        _targetScore = dto.TargetScore;
        //_limitScore = dto.LimitScore;
        _targetScoreRatio = dto.TargetScoreRatio;
        //_limitTurnCount = dto.LimitTurnCount;
        //_limitTime = dto.LimitTime;
        //_perfectCount = dto.PerfectCount;
        _requiredProgress = dto.RequiredProgress;
        //_bonusColor = (DropColor)System.Enum.Parse(typeof(DropColor), dto.BonusColor);
        //_bonusColorScore = dto.BonusColorScore;
        _boardShapeId = dto.boardShape;
        _bosskey = dto.bosskey;

        AchievementDropType[] achievementDropTypeArray = new AchievementDropType[dto.AchievementDropType.Length];
        for (int i = 0; i < dto.AchievementDropType.Length; i++)
        {
            achievementDropTypeArray[i] = (AchievementDropType)System.Enum.Parse(typeof(AchievementDropType), dto.AchievementDropType[i]);
        }
        _achievementDropType = achievementDropTypeArray;
    }

    /// <summary>
    /// 개별 Stage의 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// 개별 Stage의 key 값
    /// </summary>
    public string key { get { return _key; } set { _key = value; } }
    /// <summary>
    /// 개별 Stage의 모드 : 이 값에 따라, 인게임 UI가 조금씩 상이해짐
    /// </summary>
    public StageMode mode { get { return _mode; } set { _mode = value; } }
    /// <summary>
    /// 개별 Stage의 난이도 값
    /// </summary>
    public StageDifficulty difficulty { get { return _difficulty; } set { _difficulty = value; } }
    /// <summary>
    /// 게임 시작 시 최초 1회만 발동할 기믹. 여러 종류가 있다면, 각 기믹들이 동시에 동작한다. 없는 경우 0
    /// </summary>
    public int[] firstGimmick { get { return _firstGimmick; } set { _firstGimmick = value; } }
    /// <summary>
    /// N턴마다 반복할 기믹 리스트. 여러 종류가 있다면, 각 기믹들이 개별 턴으로 동작한다. 없는 경우 0
    /// </summary>
    public int[] repeatGimmickList { get { return _repeatGimmickList; } set { _repeatGimmickList = value; } }
    /// <summary>
    /// 개별 Stage가 속하는 world 명칭 (섬 이름)
    /// </summary>
    public string world { get { return _world; } set { _world = value; } }
    /// <summary>
    /// 해당 Stage 클리어를 위한 목표 점수
    /// </summary>
    public int targetScore { get { return _targetScore; } set { _targetScore = value; } }
    /// <summary>
    /// 해당 Stage에서 획득 가능한 최대 상한 점수
    /// </summary>
    //public int limitScore { get { return _limitScore; } set { _limitScore = value; } }
    /// <summary>
    /// 목표 점수 비례로 1,2,3별 나누기
    /// </summary>
    public float[] targetScoreRatio { get { return _targetScoreRatio; } set { _targetScoreRatio = value; } }
    /// <summary>
    /// 해당 Stage 클리어를 위한 제한 턴 수
    /// </summary>
    //public int limitTurnCount { get { return _limitTurnCount; } set { _limitTurnCount = value; } }
    /// <summary>
    /// 해당 Stage의 제한 시간(일부 난이도만 적용)
    /// </summary>
    //public float limitTime { get { return _limitTime; } set { _limitTime = value; } }
    /// <summary>
    /// 해당 Stage 클리어를 위한 완벽한 성공 횟수
    /// </summary>
    //public int perfectCount { get { return _perfectCount; } set { _perfectCount = value; } }
    /// <summary>
    /// 해당 Stage 오픈을 위한 요구 진행도
    /// </summary>
    public int requiredProgress { get { return _requiredProgress; } set { _requiredProgress = value; } }
    /// <summary>
    /// 해당 스테이지 추천 속성. 추천 속성의 점수 상승
    /// </summary>
    //public DropColor bonusColor{ get { return _bonusColor; } set { _bonusColor = value; } }
    /// <summary>
    /// 
    /// </summary>
    //public int bonusColorScore { get { return _bonusColorScore; } set { _bonusColorScore = value; } }
    /// <summary>
    /// 
    /// </summary>
    public int boardShapeId { get { return _boardShapeId; } set { _boardShapeId = value; } }
    /// <summary>
    /// 스테이지에서 발생하는 achievement의 DropType
    /// </summary>
    public AchievementDropType[] achievementDropType { get { return _achievementDropType; } set { _achievementDropType = value; } }
    /// <summary>
    /// 스테이지에서 등장하는 보스 key 값
    /// </summary>
    public string bosskey { get { return _bosskey; } set { _bosskey = value; } }

    public StageBehaviour stageBehaviour
    {
        get { return m_stageBehaviour; }
        set
        {
            m_stageBehaviour = value;
            m_stageBehaviour.SetStage(this);
        }
    }
    

    internal Stage InstantiateStageObj(GameObject stagePrefab, Transform containerObj, Vector3 position)
    {
        //1. Stage 오브젝트를 생성
        GameObject stage = Object.Instantiate(stagePrefab, position, Quaternion.identity);
        
        // FIXME :: Stage 생성 동작은 달라야할 것 
        //2. 컨테이너에 Stage를 포함시킨다.
        //stage.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 CharacterBehaviour 컴포너트를 보관한다.
        this.stageBehaviour = stage.transform.GetComponent<StageBehaviour>();

        return this;
    }
}

[System.Serializable]
public class StageDTO : IDto
{
    public int id;
    public string key;
    public string mode;
    public string difficulty;
    public int[] firstgimmick;
    public int[] repeatgimmick;
    public string world;
    public int TargetScore;
    //public int LimitScore;
    public float[] TargetScoreRatio;
    //public int LimitTurnCount;
    //public float LimitTime;
    //public int PerfectCount;
    public int RequiredProgress;
    //public string BonusColor;
    //public int BonusColorScore;
    public int boardShape;
    public string bosskey;
    public string[] AchievementDropType;
}