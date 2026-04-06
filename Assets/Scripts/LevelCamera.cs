
using System.Collections;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public bool DoFollow;

    [SerializeField] private float CamInputStrength;
    [SerializeField] private float CamInputDuration;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Camera cam2D;

    private float lowerLimit;

    public void UpdateCameraStats(LevelSO level)
    {
        transform.position = new Vector3(-0.5f,followTarget.position.y,-10) + Vector3.down * 3;
        cam2D.orthographicSize = level.GetWidth() * 0.5f / cam2D.aspect + 0.25f;
        lowerLimit = -level.GetHeight() + cam2D.orthographicSize;
    }

    public void StartAction()
    {
        DoFollow = true;
        StartCoroutine(MovementAction());  
    }

    public void EndAction()
    {
        DoFollow = false;   
    }


    public IEnumerator MovementAction()
    {
        Vector3 speed = new(0,1 / AssetLoader.GetCurrentLevel().TimePerStep,0);
        while (DoFollow && transform.position.y > lowerLimit)
        {
            yield return null;
            if(followTarget.position.y > transform.position.y)
                continue;
                
            transform.Translate(-speed * Time.deltaTime);
        }
    }
}