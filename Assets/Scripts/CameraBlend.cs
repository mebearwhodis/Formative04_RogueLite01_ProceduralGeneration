using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraBlend : MonoBehaviour
{
    private CinemachineBrain _brain;

    private void Awake()
    {
        _brain = GetComponent<CinemachineBrain>();
        _brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        StartCoroutine("SetCameraBlend");
    }

    IEnumerator SetCameraBlend()
    {
        yield return new WaitForSeconds(1f);
        _brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);
    }
}
