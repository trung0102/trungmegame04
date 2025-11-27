using UnityEngine;
using System.Collections.Generic;
using System;
using Mirror;

public class Player : MonoBehaviour
{
    [SerializeField] protected List<Resource> resources = new List<Resource>();

    public Color color = Color.red;

    public virtual void AddResource(ResourceType resourcename, int number = 1)
    {
        Resource res = this.GetResByName(resourcename);
        res.number += number;
        Debug.Log($"Add: {res.resourceName} -- so luong: {number}");
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
