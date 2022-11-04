using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SaveManager :Singleton<SaveManager>
{
    string sceneName="";
    public string SceneName{get{return PlayerPrefs.GetString(sceneName);}}
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        
    }
    public void Save(object data,string key)
    { 
       var jsonData=JsonUtility.ToJson(data,true);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName,SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void Load(object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        { JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data); }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        { SavePlayerData(); }
        if (Input.GetKeyUp(KeyCode.L))
        { LoadPlayerData(); }
        if(Input.GetKeyDown(KeyCode.Escape))
        {SceneController.Instance.TransitionToMain();}
    }
    public void SavePlayerData()
    { Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name); }
    public void LoadPlayerData()
    {Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name); }
}
