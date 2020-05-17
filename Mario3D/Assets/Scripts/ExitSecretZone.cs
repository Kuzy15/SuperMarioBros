using UnityEngine;

/// <summary>
/// Class that represents an exit zone. This is a pipe that exits a secret zone (underneath or water)
/// </summary>
public class ExitSecretZone : MonoBehaviour
{
    //Pipe that represents the exit zone
    private GameObject _pipe;

    /// <summary>
    /// Method that moves the player to the exit secret zone position
    /// </summary>
    /// <param name="player"></param>
    public void GoToSecretZone(Player player)
    {
        Vector3 _secretPosition = _pipe.transform.position;
        //GameCamera.Instance.G
        player.transform.position = new Vector3(_secretPosition.x, _secretPosition.y - 2, _secretPosition.z);
    }

    /// <summary>
    /// Getter of the exit pipe position
    /// </summary>
    /// <returns></returns>
    public Vector3 GetSecretZonePosition()
    {
        return _pipe.transform.position;
    }

    /// <summary>
    /// Setter of the exit pipe
    /// </summary>
    /// <param name="pipe"></param>
    public void SetExitPipe(GameObject pipe)
    {
        _pipe = pipe;
    }
}
