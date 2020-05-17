using UnityEngine;

/// <summary>
/// Class that manages all the magic blocks, giving them its entities, animationes, etc.
/// </summary>
public class MagicBlockManager : MonoBehaviour
{
    //Instance of MagicBlockManager
    public static MagicBlockManager GM;
    //Array with the animations to give
    public Sprite[] anim;
    //Disable block sprite to give
    public Sprite disableBlock;
    //Animation speed to give
    public float animSpeed = 0;
    //Array with the entities (Coin or mushroom)
    public GameObject[] entityArr;
    //Entity to give to a block (Coin or mushroom)
    public GameObject entity;

    void Awake()
    {
        if (GM != null)
           Destroy(GM);
        else
            GM = this;

        //DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Getter of the animation array
    /// </summary>
    /// <returns></returns>
    public Sprite[] GetAnim()
    {
        return anim;
    }

    /// <summary>
    /// Getter of the disable block sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetDisableBlock()
    {
        return disableBlock;
    }

    /// <summary>
    /// Setter of the animation speed
    /// </summary>
    /// <param name="s"></param>
    public void SetAnimSpeed(float s)
    {
        animSpeed = s;
    }

    /// <summary>
    /// Getter of the animation speed
    /// </summary>
    /// <returns></returns>
    public float GetAnimSpeed()
    {
        return animSpeed;
    }

    /// <summary>
    /// Getter of the selected entity
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public GameObject GetEntity(int i)
    {
        return entity;
    }

    /// <summary>
    /// Setter of the entity to give to a magic block.
    /// A coin appears the 70% of the times. A mushroom the 30%
    /// </summary>
    /// <returns></returns>
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
