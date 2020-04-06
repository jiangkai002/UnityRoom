using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomModel
{

    public string Name;
    public string ElementId;
    public string GameObjectName { get { return $"{Name} [{ElementId}]"; } }

}
