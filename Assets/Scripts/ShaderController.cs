using System.Collections;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [SerializeField, Range(-1, 1)] private float curveX;
    [SerializeField, Range(-1, 1)] private float curveY;
    [SerializeField] private Material[] materials;
    private float waitTimeForCurveAgain = 3.0f;
    private float timeToComplete = 2000.0f;

    [SerializeField] private int curveXRandom, curveYRandom;
    GameManager gameManager;
    private bool isCurving;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        foreach (var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("_Curve_X"), 0);
            m.SetFloat(Shader.PropertyToID("_Curve_Y"), 0);
        }
    }

    private void FixedUpdate()
    {
        if (!gameManager.GameOver && gameManager.Go)
        {
            foreach (var m in materials)
            {
                if (!isCurving)
                    CurveRoad(m);
            }
        }
    }

    private void CurveRoad(Material m)
    {
        isCurving = true;
        curveXRandom = Random.Range(-1, 2);
        float curveXTarget;
        float timeElapsed = 0;

        while (timeElapsed < timeToComplete)
        {
            float t = (timeElapsed / timeToComplete);

            curveXTarget = Mathf.Lerp(m.GetFloat(Shader.PropertyToID("_Curve_X")), curveXRandom, t);
            m.SetFloat(Shader.PropertyToID("_Curve_X"), curveXTarget);
            timeElapsed += Time.deltaTime;
        }

        StartCoroutine(Sleep());        
    }

    IEnumerator Sleep()
    {
        yield return new WaitForSeconds(waitTimeForCurveAgain);

        isCurving = false;
    }
}
