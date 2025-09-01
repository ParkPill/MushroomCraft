using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;


//using StudioNAP;
using UnityEngine;
[Serializable]
public class PlayerData
{
    public ObscuredInt server;

    public ObscuredString _id; // 0
    public ObscuredString playID;
    public ObscuredString name;
    public ObscuredInt level;
    public ObscuredInt ban;

    public ObscuredString ticketmanager; // 5


    public PlayerData(int level)
    {
        this.level = level;
    }

    public PlayerData()
    {

    }

}
