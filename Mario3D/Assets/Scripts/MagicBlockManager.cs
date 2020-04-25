using UnityEngine;

public class MagicBlockManager : MonoBehaviour
{
    public static MagicBlockManager GM;
    public Sprite[] anim;
    public Sprite disableBlock;
    public float animSpeed = 0;
    public GameObject[] entityArr;
    public GameObject entity;

    void Awake()
    {
        if (GM != null)
           Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    public Sprite[] GetAnim()
    {
        return anim;
    }

    public Sprite GetDisableBlock()
    {
        return disableBlock;
    }

    public void SetAnimSpeed(float s)
    {
        animSpeed = s;
    }


    public float GetAnimSpeed()
    {
        return animSpeed;
    }

    public GameObject GetEntity(int i)
    {
        return entity;
    }

    public GameObject SetEntity()
    {
        int p = Random.Range(0, 10);
        if(p > 7)
        {
            entity = entityArr[entityArr.Length - 1];
        }
        else
        {
            entity = entityArr[0];
        }
        return entity;
    }
}
