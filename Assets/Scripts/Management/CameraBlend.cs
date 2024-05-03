using System.Collections;
using Cinemachine;
using UnityEngine;

//Used in the Dungeon so that the camera instantly snaps to the player and then has a blend when changing rooms
public class CameraBlend : MonoBehaviour
{
    private CinemachineBrain _brain;

    private void Awake()
    {
        _brain = GetComponent<CinemachineBrain>();
        _brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.Cut, 0);
        StartCoroutine(SetCameraBlend());
    }

    private IEnumerator SetCameraBlend()
    {
        yield return new WaitForSeconds(1f);
        _brain.m_DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, 0.5f);
    }
}
