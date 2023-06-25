using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateController : MonoBehaviour
{
    [SerializeField] int targetFrameRate;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
