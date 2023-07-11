using System.Collections;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [Header("Curve Settings")]
    [SerializeField, Range(-1, 1)] private float curveX;
    [SerializeField, Range(-1, 1)] private float curveY;
    [SerializeField] private Material[] materials;
    [Tooltip("Tiempo de espera para curvar de nuevo el shader")]
    [SerializeField] private float waitTimeForCurveAgain = 3.0f;
    [Tooltip("Tiempo de curvatura del shader")]
    [SerializeField] private float timeToCurve = 2.0f;
    [SerializeField] private bool autoCurve = true;

    private float currentTime;

    private int curveXTarget, curveYTarget;
    private float curveXCurrent, curveYCurrent;
    private GameManager gameManager;

    private readonly int[] curvePosibleValues = { -1, 1 };
    
    private bool allowCurving = true;

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

    // Efecto de curva aleatoria del nivel
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

    // Corrutina para esperar el tiempo establecido de permitir la nueva curvatura
    IEnumerator Sleep()
    {
        allowCurving = false;
        yield return new WaitForSeconds(waitTimeForCurveAgain);
        allowCurving = true;
    }

    // Establecemos los valores de curvatura del nivel al shader
    private void CurveLevel(float x, float y)
    {
        foreach (var m in materials)
        {
            m.SetFloat(Shader.PropertyToID("_Curve_X"), x);
            m.SetFloat(Shader.PropertyToID("_Curve_Y"), y);
        }
    }

}
