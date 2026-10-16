using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Save Data, variables that need to be saved are initialized in the
//public class, their values are then set in the public method. 
[System.Serializable]
public class SaveData {
            public int placeholder; 

    public SaveData() {
        this.placeholder = 0;
    }
}

