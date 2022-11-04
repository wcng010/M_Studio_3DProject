using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType
    {SameScene,DifferentScene}

    [Header("Transition Info")]
    public string sceneName;
    public TransitionType transitionType;
    public TransitionDestination.DestinationTag destinationTag;
    private bool canTrans;
    // Start is called before the first frame update

     private void OnTriggerStay(Collider other)
    {if(other.CompareTag("Player"))
        canTrans = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            canTrans=false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans)
        {
            SceneController.Instance.TransitionToDestination(this);
        
        }
    }
}
