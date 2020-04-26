using UnityEngine;

public class ExitSecretZone : MonoBehaviour
{
    private GameObject _pipe;
    public void GoToSecretZone(Player player)
    {
        Vector3 _secretPosition = _pipe.transform.position;
        //GameCamera.Instance.G
        player.transform.position = new Vector3(_secretPosition.x, _secretPosition.y - 2, _secretPosition.z);
    }

    public Vector3 GetSecretZonePosition()
    {
        return _pipe.transform.position;
    }

    public void SetExitPipe(GameObject pipe)
    {
        _pipe = pipe;
    }
}
