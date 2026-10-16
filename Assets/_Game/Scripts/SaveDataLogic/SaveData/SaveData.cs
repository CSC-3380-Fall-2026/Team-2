using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Save Data, variables that need to be saved are initialized in the
//public class, their values are then set in the public method.
//Additional changes will be needed down the line to deal with diff
//things that need saving, but for now this works fine.

[System.Serializable]
public class SaveData {
    //public int placeholder; <- Create var like this. 

    public SaveData() {
        //this.placeholder = 0; <- Assign default here.
    }
}

