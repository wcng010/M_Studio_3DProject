using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;

    private CinemachineFreeLook followCamera;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;//由于场景中存在多个CharacterStates所以通过函传递的方式拿到player的CharacterStates;
        
        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        { followCamera.Follow = playerStats.transform;
          followCamera.LookAt = playerStats.transform;
        }

    }
    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }
    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }
    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        { observer.EndNotify(); }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    public Transform GetEntrance()
    {
        foreach(var item in FindObjectsOfType<TransitionDestination>())
        {
            if(item.destinationTag==TransitionDestination.DestinationTag.ENTER)
            {return item.transform;}
        }return null;


    }
}
