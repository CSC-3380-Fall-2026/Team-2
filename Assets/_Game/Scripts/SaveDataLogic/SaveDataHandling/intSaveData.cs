using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//For data that needs to be saved/loaded, make sure to implement "intSaveData" in the method header. 
//This interface is used to ensure that any class that needs to save/load data implements the necessary methods for doing so.
public interface intSaveData {
    
    void loadData(SaveData data);

    void saveData(ref SaveData data);

}
