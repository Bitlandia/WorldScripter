using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Text;
using System.IO;
using UConsole;
using System;

public class ConsoleInputMan : MonoBehaviour {

    public GameObject TextToUpdate;
    public GameObject PanelToUpdate;
    public GameObject ScrollViewToUpdate;
    public GameObject PlayerToUpdate;
    public GameObject TextboxToUpdate;
    public string PathToWSFolder;
    public string PathToTextureFolder;
    public string PathToScriptFolder;
    public string PathToModelFolder;
    public string PathToAudioFolder;
    bool IsEnabled = false;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Command : Attribute
    {
        /// <summary>
        /// The command name.
        /// </summary>
        public string name;

        /// <summary>
        /// Creates a new instance of the <see cref="Command"/>. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        public Command(string name)
        {
            this.name = name;
        }
    }

    public void Start()
    {
        //Directories
        PathToWSFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/WorldScripter";
        PathToTextureFolder = PathToWSFolder + "/Textures";
        PathToScriptFolder = PathToWSFolder + "/Scripts";
        PathToModelFolder = PathToWSFolder + "/Models";
        PathToAudioFolder = PathToWSFolder + "/Audio";
        if (!Directory.Exists(PathToWSFolder))
            Directory.CreateDirectory(PathToWSFolder);
        if (!Directory.Exists(PathToScriptFolder))
            Directory.CreateDirectory(PathToScriptFolder);
        if (!Directory.Exists(PathToTextureFolder))
            Directory.CreateDirectory(PathToTextureFolder);
        if (!Directory.Exists(PathToAudioFolder))
            Directory.CreateDirectory(PathToAudioFolder);
        //Initiate Console
        Text TextUPDComp = TextToUpdate.GetComponent<Text>();
        TextUPDComp.text = "WorldScripter 0.1a - (C) Bitlandia Studios 2017";
        SendMSG("For more information see the Credits tab.");
        SendMSG("For help see https://github.com/Bitlandia/WorldScripter/wiki.");
        PanelToUpdate.SetActive(false);
    }

    public void Update()
    {
        RigidbodyFirstPersonController FPSCtrl = PlayerToUpdate.GetComponent<RigidbodyFirstPersonController>();
        MouseLook MLook = FPSCtrl.mouseLook;
        if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsEnabled == false)
            {
                PanelToUpdate.SetActive(true);
                IsEnabled = true;
                MLook.SetCursorLock(false);
                FPSCtrl.enabled = false;
            }
            else
            {
                FPSCtrl.enabled = true;
                PanelToUpdate.SetActive(false);
                IsEnabled = false;
                MLook.SetCursorLock(true);
            }
        }
    }


    public void SendMSG(string Out)
    {
        Text TextUPDComp = TextToUpdate.GetComponent<Text>();
        ScrollRect ScrollUPDComp = ScrollViewToUpdate.GetComponent<ScrollRect>();
        //Update Text
        string CurrText = TextUPDComp.text;
        string NewText = CurrText + "\n" + Out;
        TextUPDComp.text = NewText;
        //Scroll to bottom
        StartCoroutine(WaitForUpdate(ScrollUPDComp)); //Sometimes scrollbar doesn't update fast enough
        ScrollUPDComp.verticalScrollbar.value = 0;
    }

    IEnumerator WaitForUpdate(ScrollRect ScrollUPDComp)
    {
        yield return new WaitForSeconds(0.02f);
        ScrollUPDComp.verticalScrollbar.value = 0;
    }

    public void RunFromTextBox()
    {
        InputField InpField = TextboxToUpdate.GetComponent<InputField>();
        ParseCommand(InpField.text);
        InpField.text = "";
    }

    //Parser
    public void ParseCommand(string Input)
    {
        string[] Args = Input.Split(char.Parse(" "));
        var Type = typeof(ConsoleInputMan);
        MethodInfo[] Methods = Type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (MethodInfo Method in Methods)
        {
            try
            {
                var SomeAttrib = Method.GetCustomAttributes(false).FirstOrDefault(x => x is Command) as Command;
                if (SomeAttrib != null)
                {
                    if (SomeAttrib.name == Args[0])
                    {
                        Method.Invoke(this, new object[] { Args });
                    }
                }
            }
            catch (Exception ex) { Debug.Log(ex.ToString()); }
        }
    }

    //Commands
    [Command("create")]
    public void Create(string[] Args)
    {
        try
        {
            GameObject Obj = null;
            var FilePath = PathToModelFolder + "/" + Args[1];
            //Object Types
            if (Args[1] == "cube")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            else if (Args[1] == "sphere")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            }
            else if (Args[1] == "cylinder")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            }
            else if (Args[1] == "capsule")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            }
            else if (File.Exists(FilePath))
            {
                MeshCollider MeshCollide;
                Obj = OBJLoader.LoadOBJFile(FilePath); //Create the object
                //Add the collision - crappy hack
                List<GameObject> ChildrenGo = new List<GameObject>();
                int Children = Obj.transform.childCount;
                for (int i = 0; i < Children; ++i)
                    ChildrenGo.Add(Obj.transform.GetChild(i).gameObject);
                foreach(GameObject ObjMesh in ChildrenGo)
                {
                    MeshCollide = ObjMesh.AddComponent<MeshCollider>();
                    MeshCollide.convex = true;
                    MeshCollide.inflateMesh = true;
                    MeshCollide.skinWidth = float.Parse(Args[9]);
                }
            }
            else
                return; //No shape, stop
            Obj.SetActive(true); //just incase
            Obj.transform.position = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[4])); //Set the position
            Obj.transform.localScale = new Vector3(float.Parse(Args[5]), float.Parse(Args[6]), float.Parse(Args[7])); //Set the scale
            Obj.name = Args[8]; //Set the name
            Obj.tag = "ConsoleCreated"; //Set the tag
            SendMSG("Created " + Args[1] + " at " + Args[2] + ", " + Args[3] + ", " + Args[4] + " with scale of " + Args[5] + ", " + Args[6] + ", " + Args[7] + " named " + Args[8] + ".");
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("list")]
    public void List()
    {
        //Take every object with ConsoleCreated and list it.
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs) {
            SendMSG(Obj.name + " @ " + Obj.transform.localPosition.x + ", " + Obj.transform.localPosition.y + ", " + Obj.transform.localPosition.z);
        }
    }
    [Command("color")]
    public void Color(string[] Args)
    {
        try
        {
            //Try to set the color
            GameObject.Find(Args[1]).GetComponent<Renderer>().material.color = new Color32(byte.Parse(Args[2]), byte.Parse(Args[3]), byte.Parse(Args[3]), byte.Parse(Args[3]));
            SendMSG("Changed color of " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
        }
       catch { SendMSG("Unexpected error"); }
    }
    [Command("destroy")]
    public void Destroy(string[] Args)
    {
        try
        {
            GameObject ToDestroy = GameObject.Find(Args[1]);
            GameObject.Destroy(ToDestroy);
            SendMSG("Destroyed " + Args[1] + ".");
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("move")]
    public void Move(string[] Args)
    {
        try
        {
            GameObject ToMove = GameObject.Find(Args[1]);
            ToMove.transform.position = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[4]));
            SendMSG("Moved " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("echo")]
    public void Echo(string[] Args)
    {
        try
        {
            string ToOut = ""; //incase you have spaces
            foreach (string Arg in Args)
            {
                if (Arg == "echo") { }
                else
                    ToOut = ToOut + Arg;
            }
            SendMSG(ToOut);
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("run")]
    public void Run(string[] Args)
    {
        try
        {
            string Line;
            string FileName = PathToScriptFolder + "/"; //incase you have spaces
            foreach (string Arg in Args)
            {
                if (Arg == "run") { }
                else
                    FileName = FileName + Arg;
            }
            StreamReader Reader = new StreamReader(FileName, Encoding.Default);
            Debug.Log(FileName);
            using (Reader)
            {
                do
                {
                    Line = Reader.ReadLine(); //Get the line
                    Debug.Log(Line);
                    if (Line != null)
                    {
                        if(!Line.StartsWith("name"))  //Ignore the meta data line
                        {
                            ParseCommand(Line);
                            Debug.Log(Line);
                        }
                    }
                }
                while (Line != null); 
                Reader.Close(); //Close the reader
                SendMSG("Done loading script.");
            }
        }
        catch
        {
            SendMSG("Unexpected error");
        }
    }
    [Command("rotate")]
    public void Rotate(string[] Args)
    {
        try
        {
            GameObject ToRotate = GameObject.Find(Args[1]);
            ToRotate.transform.rotation = new Quaternion(int.Parse(Args[2]), int.Parse(Args[3]), int.Parse(Args[4]), int.Parse(Args[5]));
            SendMSG("Rotated " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ", " + Args[5] + ".");
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("scale")]
    public void Scale(string[] Args)
    {
        try
        {
            GameObject ToScale = GameObject.Find(Args[1]);
            ToScale.transform.localScale = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[5]));
            SendMSG("Scaled " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("add_texture")]
    public void AddTexture(string[] Args)
    {
        GameObject[] Objs;
        Renderer Renderer;
        var FilePath = PathToTextureFolder + "/";
        foreach (string Arg in Args)
        {
            //Just if you have files in the name.
            if (Arg == "add_texture") { }
            else if (Arg == Args[1]) { }
            else
                FilePath = FilePath + Arg;
        }
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated"); // Make sure we aren't doing something stupid here
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                if (System.IO.File.Exists(FilePath))
                {
                    Renderer = Obj.GetComponent<Renderer>();
                    var Bytes = System.IO.File.ReadAllBytes(FilePath);
                    var Tex = new Texture2D(1, 1); //Load the texture onto a texture
                    Tex.LoadImage(Bytes);
                    Renderer.material.mainTexture = Tex;
                    SendMSG("Added texture to " + Args[1]);
                }
            }
        }
    }
    [Command("add_comp")]
    public void AddComp(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if(Obj.name == Args[1])
            {
                Obj.AddComponent(ConsoleUtils.FindType(Args[2], false, true)); // Actually get the type
                SendMSG("Added " + Args[2] + " to " + Args[1]);
            }   
        }
    }
    [Command("add_sound")]
    public void AddSound(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        string FileName = "file:///" + PathToAudioFolder + "/"; //incase you have spaces
        foreach (string Arg in Args)
        {
            if (Arg == "add_sound") { }
            else if (Arg == Args[1]) { }
            else
                FileName = FileName + Arg;
        }
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                AudioHandler Audio = Obj.AddComponent<AudioHandler>();
                Audio.Run(FileName);
                SendMSG("Added sound to " + Args[1]);
            }
        }
    }
    [Command("del_comp")]
    public void DelComp(string[] Args)
    {
        GameObject ToComp = GameObject.Find(Args[1]);
        System.Type ToRemove = ConsoleUtils.FindType(Args[2], false, true);
        Destroy(ToComp.GetComponent(ToRemove));
    }
    [Command("del_sound")]
    public void DelSound(string[] Args)
    {
        GameObject ToSound = GameObject.Find(Args[1]);
        Destroy(ToSound.GetComponent<AudioHandler>());
        Destroy(ToSound.GetComponent<AudioSource>());
    }
    [Command("del_texture")]
    public void DelTexture(string[] Args)
    {
        GameObject[] Objs;
        Renderer Renderer;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
                    Renderer = Obj.GetComponent<Renderer>();
                    var Tex = new Texture2D(1, 1);
                    Renderer.material.mainTexture = Tex; //litterally just sets it to a white square
                    SendMSG("Removed texture from " + Args[1]);
        }
    }
    [Command("clear")]
    public void Clear()
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            Destroy(Obj);
        }
        SendMSG("Cleared all objects");
    }

    public string GetName(string FileName)
    {
        try
        {
            string Line;
            string Name;
            Debug.Log(FileName);
            StreamReader Reader = new StreamReader(FileName, Encoding.Default);
            using (Reader)
            {
                do
                {
                    Line = Reader.ReadLine();

                    if (Line != null)
                    {
                        try
                        {
                            if (!Line.StartsWith("name-")) //Only get the meta data this time
                            {
                                Reader.Close(); //Just to avoid memory leaks
                                return "null";
                            }
                            else
                                Name = Line.Remove(0, 5);
                            SendMSG("Found " + Name + " at " + FileName);
                            Reader.Close();
                            return Name;
                        }
                        catch
                        {
                            Reader.Close();
                            return "Name not found.";
                        }
                    }
                }
                while (Line != null);
                Reader.Close();
                SendMSG("Scanned file - no content found");
                return "null";
            }
        }
        catch
        {
            SendMSG("Unexpected error trying to find name");
            return "err";
        }
    }
}