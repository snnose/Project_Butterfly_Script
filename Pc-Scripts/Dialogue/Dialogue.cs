using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue : IDataFromDto<DialogueDTO>
{
    [SerializeField] private int _id;
    [SerializeField] private int _progress;
    [SerializeField] private string _scriptkey;
    [SerializeField] private string _character;
    [SerializeField] private string _anim;
    [SerializeField] private string _direction;

    public Dialogue() {  }
    public void FromDto(DialogueDTO dto)
    {
        _id = dto.id;
        _progress = dto.progress;
        _scriptkey = dto.scriptkey;
        _character = dto.character;
        _anim = dto.anim;
        _direction = dto.direction;
    }

    /// <summary>
    /// Dialogue의 고유 id
    /// </summary>
    public int id { get { return _id; } set { _id = value; } }
    /// <summary>
    /// Dialogue의 progress
    /// </summary>
    public int progress { get { return _progress; } set { _progress = value; } }
    /// <summary>
    /// Dialogue의 대사 key값
    /// </summary>
    public string scriptkey { get { return _scriptkey; } set { _scriptkey = value; } }
    /// <summary>
    /// Dialogue의 주체
    /// </summary>
    public string character{ get { return _character; } set { _character = value; } }
    /// <summary>
    /// Dialogue의 animation
    /// </summary>
    public string anim { get { return _anim; } set { _anim = value; } }
    /// <summary>
    /// Dialogue 주체의 이미지 표시 방향
    /// </summary>
    public string direction { get { return _direction; } set { _direction = value; } }

    protected DialogueBehaviour m_DialogueBehaviour;

    public DialogueBehaviour DialogueBehaviour
    {
        get { return m_DialogueBehaviour; }
        set
        {
            m_DialogueBehaviour = value;
            m_DialogueBehaviour.SetDialogue(this);
        }
    }
    
    internal Dialogue InstantiateCharacterObj(GameObject DialoguePrefab, Transform containerObj, Vector3 position)
    {
        //1. Dialogue 오브젝트를 생성
        GameObject Dialogue = Object.Instantiate(DialoguePrefab, position, Quaternion.identity);
        
        //2. 컨테이너에 Dialogue를 포함시킨다.
        Dialogue.transform.parent = containerObj;

        //3. Character 오브젝트에 적용된 DialogueBehaviour 컴포너트를 보관한다.
        this.DialogueBehaviour = Dialogue.transform.GetComponent<DialogueBehaviour>();

        return this;
    }
}

[System.Serializable]
public class DialogueDTO : IDto
{
    public int id;
    public int progress;
    public string scriptkey;
    public string character;
    public string anim;
    public string direction;
}