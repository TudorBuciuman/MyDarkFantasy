using UnityEngine;

public class itemsManager : MonoBehaviour
{
    public GameObject itemp;
    public WorldManager world;
    public Toolbar tool;
    public Transform plpos;
    public void SetItem(byte id,byte size, Vector3 pos)
    {
        
        GameObject itemk = Instantiate(itemp, pos, Quaternion.identity);
        itemk.GetComponent<SpriteRenderer>().sprite= world.blockTypes[id].itemSprite;
        Item item=itemk.AddComponent<Item>();
        item.Initialize(id, size, (int)(pos.x / 16), (int)(pos.y / 16),this,itemk);
    }
}

public class Item : MonoBehaviour
{
    public byte id=0;
    public byte size=0;
    public int x, y;
    private itemsManager manager;
    public GameObject obj;
    private float timeSLU = 0.0f;
    public void Initialize(byte id, byte size, int x, int y, itemsManager manager,GameObject obj)
    {
        this.id = id;
        this.size = size;
        this.x = x;
        this.y = y;
        this.manager=manager;
        this.obj = obj;
    }

    public void Open()
    {
        Destroy(gameObject);
    }
    
    public void FixedUpdate()
    {
        if (!Toolbar.escape)
        {
            if (timeSLU > 1)
            {
                timeSLU = 0;
                if (Vector3.Distance(this.transform.position, this.manager.plpos.position) < 3)
                {
                    manager.tool.PickUp(id, size,this);
                } 
            }
            timeSLU += Time.deltaTime;
        }
    }
    
}
