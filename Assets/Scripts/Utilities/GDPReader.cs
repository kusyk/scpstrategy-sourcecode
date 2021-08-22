using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GDPReader : MonoBehaviour
{
    public float supervalue = 100000;
    public static GDPReader instance;

    [SerializeField]
    private List<GDPInfo> gdpList = new List<GDPInfo>(); 

    public static List<GDPInfo> GDPList
    {
        get
        {
            return instance.gdpList;
        }
    }

    private void Awake()
    {
        instance = this;
        
        TextAsset tempList = Resources.Load<TextAsset>("Info/gdp");
        string tempLine;
        StringReader stringReader = new StringReader(tempList.text);

        while (true)
        {
            tempLine = stringReader.ReadLine();

            if (tempLine == null)
                break;

            string[] entries = tempLine.Split(',');
            gdpList.Add(new GDPInfo(entries[0], int.Parse(entries[1])));
        }
    }

    private void Start()
    {

    }

    [System.Serializable]
    public class GDPInfo
    {
        public string name = "";
        public int gdp = 0;
        public bool hasBeenUsed = false;

        public GDPInfo(string _name, int _gdp, bool _hasBeenUsed = false)
        {
            name = _name;
            gdp = _gdp;
            hasBeenUsed = _hasBeenUsed;
        }

    }
}
