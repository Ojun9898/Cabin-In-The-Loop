using System;
using UnityEngine;

public class DataClass2 : EventArgs
{
    public string dataName;

    public DataClass2(string dataName)
    {
        this.dataName = dataName;
    }
}
    
public class ExampleEventHandler : MonoBehaviour
{
    private EventHandler startHandler;
    
    void OnEnable()
    {
        DataClass2 data = new DataClass2("New Data Name");
        
        startHandler += StartEvent; // 구독

        startHandler(this, data);
        //startHandler(this, EventArgs.Empty);
    }

    public void StartEvent(object o, EventArgs e)
    {
        Debug.Log(((DataClass2)e).dataName);

        DataClass2 data = (DataClass2)e;
        Debug.Log(data.dataName);
    }
    
}