using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSecretZone : MonoBehaviour
{
    int i;
    public void GoToSecretZone(Player player)
    {
        Vector3 _secretPosition = MapReader.GM.GetSecretZonePos(i).position;
        //GameCamera.Instance.G
        player.transform.position = new Vector3(_secretPosition.x, _secretPosition.y, _secretPosition.z);
    }

    public void SetEnterZoneIndex(int index)
    {
        i = index;
    }
    
}
