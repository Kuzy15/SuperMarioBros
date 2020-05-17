using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that represents an enter zone. This is a pipe that goes to a secret zone (underneath or water)
/// </summary>
public class EnterSecretZone : MonoBehaviour
{
    //Index of the pipe
    private int _i;
    //Bool to check if water or underneath
    private bool _isUnderground;

    /// <summary>
    /// Method that moves the player to the enter secret zone position
    /// </summary>
    /// <param name="player"></param>
    public void GoToSecretZone(Player player)
    {
        if (!_isUnderground)
        {
            player.GetComponent<Rigidbody>().mass = 1f;
            player.GetComponent<Rigidbody>().useGravity = false;
        }
        Vector3 _secretPosition = MapReader.GM.GetSecretZonePos(_i).position;
        //GameCamera.Instance.G
        player.transform.position = new Vector3(_secretPosition.x, _secretPosition.y - 6, _secretPosition.z);
    }


    /// <summary>
    /// Setter for the pipe index
    /// </summary>
    /// <param name="index"></param>
    public void SetEnterZoneIndex(int index)
    {
        _i = index;
    }

    /// <summary>
    /// Check if player is going to enter to underground zone or underwater zone
    /// </summary>
    /// <param name="isUnder"></param>
    public void EnteringUnderground(bool isUnder)
    {
        _isUnderground = isUnder;
    }

    /// <summary>
    /// Getter for underground zone
    /// </summary>
    /// <returns></returns>
    public bool GetIfUnderground()
    {
        return _isUnderground;
    }
}
