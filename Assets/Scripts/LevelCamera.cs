
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
        float x = level.GetWidth() % 2 == 0 ? -0.5f : 0;
        transform.position = new Vector3(x,followTarget.position.y,-10) + Vector3.down * 3;
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
        float smooth = 1f / AssetLoader.GetCurrentLevel().StepsPerSecond;

        while (DoFollow && transform.position.y > lowerLimit)
        {
            if (followTarget.position.y <= transform.position.y)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    transform.position + Vector3.down,
                    smooth * Time.deltaTime
                );
            }

            yield return null;
        }
    }
}