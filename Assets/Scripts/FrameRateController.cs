using UnityEngine;

public class FramerateController : MonoBehaviour
{
    [SerializeField] int targetFrameRate;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
