using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class that representes a DeadZone
/// </summary>
public class DeadZone : MonoBehaviour
{
    /// <summary>
    /// When the player enters on this trigger, the current scene is reset.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameCamera.Instance.ResetCameraToInitialPos();
            other.GetComponentInParent<Player>().ResetMarioPosition();
        }
    }
}
