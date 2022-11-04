using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
public class SceneController : Singleton<SceneController>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playagent;
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {switch(transitionPoint.transitionType)
    {case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
     case TransitionPoint.TransitionType.DifferentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
    }}
    
    public void TransitionToMain()
    {
        StartCoroutine(BackLevel());
    }


    IEnumerator Transition(string sceneName,TransitionDestination.DestinationTag destinationTag)
    {
        SaveManager.Instance.SavePlayerData();

        if (SceneManager.GetActiveScene().name != sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,GetDestination(destinationTag).transform.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {

            player = GameManager.Instance.playerStats.gameObject;
            playagent = player.GetComponent<NavMeshAgent>();
            playagent.enabled = false;
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playagent.enabled = true;
            yield return null;
        }
    }
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrance = FindObjectsOfType<TransitionDestination>();
        for (int i = 0; i < entrance.Length; i++)
        {
            if (entrance[i].destinationTag == destinationTag)
                return entrance[i];
        }
         return null;
      }

      IEnumerator LoadLevel (string scene)
      {  if(scene!=null)
          {yield return SceneManager.LoadSceneAsync(scene);
          yield return player =Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position,GameManager.Instance.GetEntrance().rotation);
          SaveManager.Instance.SavePlayerData();
          yield break;
          }
      }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("level1"));
    }
    public void TransitionToLoadGame()
    {
     StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    IEnumerator BackLevel()
    {
     yield return SceneManager.LoadSceneAsync("Main");
     yield break;
    }
}
