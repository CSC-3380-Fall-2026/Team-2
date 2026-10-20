using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//For data that needs to be saved/loaded, make sure to implement "iSaveData" in the method header. 
//This interface is used to ensure that any class that needs to save/load data implements the necessary 
//methods for doing so. 
public interface iSaveData {
    
    void loadData(SaveData data);

    void saveData(ref SaveData data);

}
