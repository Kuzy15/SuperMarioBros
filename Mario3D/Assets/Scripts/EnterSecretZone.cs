using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSecretZone : MonoBehaviour
{
    private int _i;
    public void GoToSecretZone(Player player)
    {
        Vector3 _secretPosition = MapReader.GM.GetSecretZonePos(_i).position;
        //GameCamera.Instance.G
        player.transform.position = new Vector3(_secretPosition.x, _secretPosition.y - 9, _secretPosition.z);
    }

    public void SetEnterZoneIndex(int index)
    {
        _i = index;
    }
    
}
