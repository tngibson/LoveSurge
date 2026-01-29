using UnityEngine;
public class GameItem : MonoBehaviour
{
    // Add properties and methods relevant to game items here
    public virtual void UseItem()
    {
        Debug.Log("Using item: " + gameObject.name);
        Player.instance.DeleteItem(this);
    } 
}