
using System.Collections;
using UnityEngine;

public class LevelCamera : MonoBehaviour
{
    public bool DoFollow;
    public bool Shaking;


    [SerializeField] private float CamInputStrength;
    [SerializeField] private float CamInputDuration;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Camera cam2D;


    private float camWidth;
    private float camMoveSpeed;

    private float lowerLimit;

    public void UpdateCameraStats(float boundX,float boundY)
    {
        transform.position = new Vector3(-0.5f,followTarget.position.y,-10) + Vector3.down * 3;
        cam2D.orthographicSize = boundX * 0.5f / cam2D.aspect + 0.25f;
        lowerLimit = -boundY + cam2D.orthographicSize;
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
            if(Shaking)
                continue;
                
            transform.Translate(-speed * Time.deltaTime);
        }
    }

    public IEnumerator ShakeAction()
    {
        float elapsed = 0f;
        float speed = 25f;          
        float radiusMin = CamInputStrength * 0.5f;
        float radiusMax = CamInputStrength;

        Vector2 shakeOffset = Vector2.zero;
        Vector2 shakeTarget = Vector2.zero;
        Shaking = true;

        while (elapsed < CamInputDuration)
        {
            elapsed += Time.deltaTime;

            Vector2 origin = followTarget.position;

            if ((shakeTarget - shakeOffset).sqrMagnitude < 0.001f)
            {
                shakeTarget = Random.insideUnitCircle.normalized *
                            Random.Range(radiusMin, radiusMax);
            }

            shakeOffset = Vector2.MoveTowards(
                shakeOffset,
                shakeTarget,
                speed * Time.deltaTime
            );

            transform.position = origin + shakeOffset;
            yield return null;
        }

        transform.position = followTarget.position;
        Shaking = false;
    }

}