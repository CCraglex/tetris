using UnityEngine;

public class ParticleDust : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private ParticleSystem particles;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
        var shape = particles.shape;
        var h = cam.orthographicSize * 3f;
        var w = h * cam.aspect;

        shape.scale = new Vector3(w,h,1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cam.transform.position - new Vector3(0,0,-5);
    }
}
