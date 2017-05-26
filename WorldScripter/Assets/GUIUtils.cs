using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GUIUtils : MonoBehaviour {

    public GameObject Content;
    public GameObject FPSController;
    public ConsoleInputMan Console;
    public GameObject ButtonPrefab;

    public void Awake()
    {
        Content = GameObject.Find("ScriptsButtons");
    }
	// Use this for initialization
	public void Start () {
        //Clear all the old buttons
        List<GameObject> ChildrenGo = new List<GameObject>();
        int Children = Content.transform.childCount;
        for (int i = 0; i < Children; ++i)
            ChildrenGo.Add(Content.transform.GetChild(i).gameObject);
        foreach (GameObject objmesh in ChildrenGo)
        {
            Destroy(objmesh);
        }
        //Get the console
        Console = FPSController.GetComponent<ConsoleInputMan>();
        //Get the directory
        string Path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/WorldScripter/Scripts";
        DirectoryInfo Dir = new DirectoryInfo(Path + "/");
        FileInfo[] Info = Dir.GetFiles("*.*");
        //Make the button
        GameObject ButtonToAdd;
        foreach (FileInfo F in Info)
        {
            ButtonToAdd = Instantiate(ButtonPrefab); //Create Clone
            ButtonToAdd.transform.parent = Content.transform; //Set parent to ScrollView's content
            ButtonToAdd.transform.GetChild(0).GetComponent<Text>().text = Console.GetName(Dir + F.Name); //Get the name and set the text to it
            ButtonToAdd.GetComponent<Button>().onClick.AddListener(delegate { Console.ParseCommand("run " + F.Name); }); //Add the click event of the button
        }
    }
}
