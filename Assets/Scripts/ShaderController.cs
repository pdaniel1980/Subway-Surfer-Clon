using System.Collections;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [SerializeField, Range(-1, 1)] private float curveX;
    [SerializeField, Range(-1, 1)] private float curveY;
    [SerializeField] private Material[] materials;
    [SerializeField] private float waitTimeForCurveAgain = 3.0f;
    [SerializeField] private float timeToCurve = 2.0f;
    private float currentTime;

    [SerializeField] private int curveXTarget, curveYTarget;
    private float curveXCurrent, curveYCurrent;
    private GameManager gameManager;
    [SerializeField] private bool autoCurve = true;
    private int[] curvePosibleValues = { -1, 1 };

    [SerializeField] private bool allowCurving = true;


    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        curveXTarget = curvePosibleValues[Random.Range(0, 2)];
        curveYTarget = curvePosibleValues[Random.Range(0, 2)];

        // Inicializamos en x:0, y:0 la curvatura del nivel
        curveX = 0;
        curveY = 0;
        CurveLevel(x: curveX, y: curveY);
    }

    private void Update()
    {
        if (!gameManager.GameOver && gameManager.Go && autoCurve)
        {
            Curve();
        }
    }

    private void Curve()
    {

        if (allowCurving)
        {
            currentTime += Time.deltaTime;

            float t = Mathf.Clamp01(currentTime / timeToCurve);

            curveXCurrent = Mathf.Lerp(curveX, curveXTarget, t);
            curveYCurrent = Mathf.Lerp(curveY, curveYTarget, t);

            CurveLevel(curveXCurrent, curveYCurrent);

            if (t >= 1f)
            {
                currentTime = 0f;
                curveX = curveXTarget;
                curveY = curveYTarget;
                curveXTarget = curvePosibleValues[Random.Range(0, 2)];
                curveYTarget = curvePosibleValues[Random.Range(0, 2)];
                _ = StartCoroutine(Sleep());
            }
        }
    }

    IEnumerator Sleep()
    {
        allowCurving = false;
        Debug.Log("curveX: " + curveX + " CurveRandom: " + curveXTarget + " Current Time: " + currentTime);
        yield return new WaitForSeconds(waitTimeForCurveAgain);
        allowCurving = true;
    }

    /*IEnumerator CurveCoroutine()
    {
        for (;;)
        {
            if (!gameManager.GameOver && gameManager.Go && autoCurve)
            {
                curveX = curvePosibleValues[Random.Range(0, 2)];
                curveY = curvePosibleValues[Random.Range(0, 2)];

                //Debug.Log("CurveXRandom: " + curveXRandom + " CurveYRandom: " + curveYRandom);

                float timeElapsed = 0;
                float t;
                
                while (timeElapsed < timeToCurve)
                {
                    t = Time.deltaTime / timeToCurve;
                    //curveXTarget = Mathf.MoveTowards(curveX, curveXRandom, t);
                    //Debug.Log("curveXTarget: " + curveXTarget + " timeElapsed: " + timeElapsed + " t: " + t);
                    curveXTarget = Mathf.MoveTowards(curveXTarget, curveX, t);
                    curveYTarget = Mathf.MoveTowards(curveYTarget, curveY, t);

                    timeElapsed += Time.deltaTime;

                    yield return null;
                }

                //Debug.Log("Terminamos");
                yield return new WaitForSeconds(waitTimeForCurveAgain);
                //Debug.Log("Despues de " + waitTimeForCurveAgain + " segundos");
            }
            else
            {
                yield return null;
            }
        }
    }*/

    private void CurveLevel(float x, float y)
    {
        foreach (var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("_Curve_X"), x);
            m.SetFloat(Shader.PropertyToID("_Curve_Y"), y);
        }
    }

}
