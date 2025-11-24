using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] protected List<Resource> resources = new List<Resource>();

    public Color color = Color.red;

    protected void Awake()
    {
        if(Player.instance != null) Debug.LogError("On 1 Player");
        Player.instance = this;
    }

    public virtual void AddResource(ResourceType resourcename, int number = 1)
    {
        Resource res = this.GetResByName(resourcename);
        res.number += number;
    }

    public virtual Resource GetResByName(ResourceType resourcename)
    {
        Resource res = this.resources.Find((x) => x.resourceName == resourcename);
        if (res == null)
        {
            res = new Resource();
            res.resourceName = resourcename;
            res.number = 0;
            this.resources.Add(res);
        }
        return res;
    }
}
