using System.Collections;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [SerializeField, Range(-1, 1)] private float curveX;
    [SerializeField, Range(-1, 1)] private float curveY;
    [SerializeField] private Material[] materials;
    [SerializeField] private float waitTimeForCurveAgain = 3.0f;
    [SerializeField] private float timeToComplete = 2.0f;

    [SerializeField] private int curveXRandom, curveYRandom;
    private float curveXTarget, curveYTarget;
    private GameManager gameManager;
    [SerializeField] private bool autoCurve = true;
    private int[] curvePosibleValues = { -1, 1 };

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Inicializamos en x:0, y:0 la curvatura del nivel
        CurveLevel(x: -1, y: 0);

        StartCoroutine(CurveCoroutine());
    }

    private void Update()
    {
        CurveLevel(curveXTarget, curveYTarget);
    }

    IEnumerator CurveCoroutine()
    {
        for (;;)
        {
            if (!gameManager.GameOver && gameManager.Go && autoCurve)
            {
                curveXRandom = curvePosibleValues[Random.Range(0, 2)];
                curveYRandom = curvePosibleValues[Random.Range(0, 2)];

                Debug.Log("CurveXRandom: " + curveXRandom + " CurveYRandom: " + curveYRandom);

                float timeElapsed = 0;
                float t;
                
                while (timeElapsed < timeToComplete)
                {
                    t = Time.deltaTime / timeToComplete;
                    //curveXTarget = Mathf.MoveTowards(curveX, curveXRandom, t);
                    Debug.Log("curveXTarget: " + curveXTarget + " timeElapsed: " + timeElapsed + " t: " + t);
                    curveXTarget = Mathf.MoveTowards(curveXTarget, curveXRandom, t);
                    curveYTarget = Mathf.MoveTowards(curveYTarget, curveYRandom, t);

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
    }

    private void CurveLevel(float x, float y)
    {
        foreach (var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("_Curve_X"), x);
            m.SetFloat(Shader.PropertyToID("_Curve_Y"), y);
        }

        curveX = x;
        curveY = y;
    }

}
