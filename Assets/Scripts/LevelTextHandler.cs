using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LevelTextHandler : MonoBehaviour
{
    private TextMeshProUGUI text;
    private Vector2 StartPos;
    const float moveDistance = 1920 / 2 + 300 ;
    [SerializeField] float moveTimer;

    private void Awake()
    {
        LevelHandler.levelTextHandler = this;
        text = GetComponent<TextMeshProUGUI>();
        var rect = transform as RectTransform;
        StartPos = rect.anchoredPosition;
    }

    public void SetLevel(string newLevel)
    {
        var rect = transform as RectTransform;
        rect.anchoredPosition = StartPos;
        text.text = $"Level\n{newLevel}";
        text.fontSize /= 3;
    }
    
    public IEnumerator PlayAnim()
    {
        var rect = transform as RectTransform;
        
        float dt = 0;
        
        yield return new WaitForSeconds(1);
        while (dt < moveTimer)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            dt += Time.deltaTime;

            rect.anchoredPosition = Vector2.Lerp(StartPos,new(StartPos.x,StartPos.y + moveDistance),dt / moveTimer);
        }

        rect.anchoredPosition = new(StartPos.x,StartPos.y + moveDistance);
    }

    public async Task PlayBeginning()
    {
        var rect = transform as RectTransform;
        rect.anchoredPosition = StartPos;

        int start = 3;

        text.fontSize *= 3;
        while(start > 0)
        {
            text.text = start.ToString();
            await Task.Delay(LevelHandler.IntroTickTimer);
            start -= 1;
        }
    }
}
