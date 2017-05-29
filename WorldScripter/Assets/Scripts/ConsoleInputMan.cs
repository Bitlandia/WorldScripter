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

public partial class ConsoleInputMan : MonoBehaviour {

    public GameObject TextToUpdate;
    public GameObject PanelToUpdate;
    public GameObject ScrollViewToUpdate;
    public GameObject PlayerToUpdate;
    public GameObject TextboxToUpdate;
    public GameObject CameraFPS;
    public AudioHandlerFPS AudioPlayer;
    public string PathToWSFolder;
    public string PathToTextureFolder;
    public string PathToScriptFolder;
    public string PathToModelFolder;
    public string PathToAudioFolder;
    public NetworkClient NetworkMan;
    bool NoClip = false;
    bool IsEnabled = false;
    List<GameObject> GOCache = new List<GameObject>();

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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ButtonCommand : Attribute
    {
        /// <summary>
        /// The command name.
        /// </summary>
        public string name;

        /// <summary>
        /// Creates a new instance of the <see cref="Command"/>. 
        /// </summary>
        /// <param name="name">The name of the command.</param>
        public ButtonCommand(string name)
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
        //Game Crucial Variables
        PanelToUpdate = GameObject.Find("ModAPICont");
        TextToUpdate = GameObject.Find("Console Out");
        ScrollViewToUpdate = GameObject.Find("TermScroll");
        TextboxToUpdate = GameObject.Find("TermInput");
        PlayerToUpdate = this.gameObject;
        CameraFPS = GameObject.Find("MainCamera");
        //Initiate Console
        Text TextUPDComp = TextToUpdate.GetComponent<Text>();
        TextUPDComp.text = "WorldScripter 0.3a - (C) Bitlandia Studios 2017";
        SendMSG("For more information see the Credits tab.");
        SendMSG("For help see https://github.com/Bitlandia/WorldScripter/wiki.");
        PanelToUpdate.SetActive(false);
        //Setup Networking
        NetworkMan = PlayerToUpdate.GetComponent<NetworkClient>();
    }

    public void Update()
    {
        CharControl();

        if (PanelToUpdate.activeSelf == true)
            return;
        OBJData OBJDataFind;

        try
        {
            if (Input.GetKeyDown("e"))
            {
                RaycastHit hit;
                Transform Cam = CameraFPS.transform;
                var Ray = new Ray(Cam.position, Cam.forward);
                if (Physics.Raycast(Ray, out hit, 500f))
                {
                    OBJDataFind = hit.collider.gameObject.GetComponent<OBJData>();
                    Debug.Log(OBJDataFind.ButtonCMD);
                    string[] Commands = OBJDataFind.ButtonCMD.Split(char.Parse(","));
                    Debug.Log(OBJDataFind.ButtonCMD);
                    if (OBJDataFind.IsButton == true)
                    {
                        foreach (string Commandtorun in Commands)
                        {
                            Debug.Log(Commandtorun);
                            ParseCommand(Commandtorun);
                            if (OBJDataFind.IsPartOfMesh)
                                ParseButtonCommand(Commandtorun, hit.collider.transform.parent.gameObject);
                            else
                                ParseButtonCommand(Commandtorun, hit.collider.gameObject);
                        }
                    }
                }
            }
        } catch (System.Exception e) { Debug.LogError(e.ToString()); }
    }

    public void CharControl()
    {
        RigidbodyFirstPersonController FPSCtrl = PlayerToUpdate.GetComponent<RigidbodyFirstPersonController>();
        MouseLook MLook = FPSCtrl.mouseLook;
        NoClipFirstPersonController NCFPS = PlayerToUpdate.GetComponent<NoClipFirstPersonController>();
        NoClipMouseLook NCML = PlayerToUpdate.GetComponent<NoClipMouseLook>();
        Rigidbody RigidbodyToUpd = PlayerToUpdate.GetComponent<Rigidbody>();
        CapsuleCollider ColliderToUpd = PlayerToUpdate.GetComponent<CapsuleCollider>();
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (PanelToUpdate.activeSelf == true)
                return;
            CameraFPS.transform.rotation = new Quaternion(0, 0, 0, 0);
            if (!NoClip)
            {
                NCFPS.enabled = true;
                NCML.enabled = true;
                FPSCtrl.enabled = false;
                RigidbodyToUpd.isKinematic = true;
                ColliderToUpd.enabled = false;
                NoClip = true;
            }
            else
            {
                NCFPS.enabled = false;
                NCML.enabled = false;
                FPSCtrl.enabled = true;
                RigidbodyToUpd.isKinematic = false;
                ColliderToUpd.enabled = true;
                NoClip = false;
            }
        }
        //Update GUI
        if (NoClip == true)
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsEnabled == false)
                {
                    PanelToUpdate.SetActive(true);
                    IsEnabled = true;
                    NCML.SetCursorLock(false);
                    NCFPS.enabled = false;
                    NCML.enabled = false;
                }
                else
                {
                    PanelToUpdate.SetActive(false);
                    IsEnabled = false;
                    NCML.SetCursorLock(true);
                    NCFPS.enabled = true;
                    NCML.enabled = true;
                }
            }
        }
        else
        {
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

    public void ParseButtonCommand(string Input, GameObject Object)
    {
        string[] Args = Input.Split(char.Parse(" "));
        var Type = typeof(ConsoleInputMan);
        MethodInfo[] Methods = Type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
        foreach (MethodInfo Method in Methods)
        {
            try
            {
                var SomeAttrib = Method.GetCustomAttributes(false).FirstOrDefault(x => x is ButtonCommand) as ButtonCommand;
                if (SomeAttrib != null)
                {
                    if (SomeAttrib.name == Args[0])
                    {
                        Method.Invoke(this, new object[] { Args, Object });
                    }
                }
            }
            catch (Exception ex) { Debug.Log(ex.ToString()); }
        }
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
                            if (Line.StartsWith("name-")) //Only get the meta data this time
                            {
                                Name = Line.Remove(0, 5);
                                SendMSG("Found " + Name + " at " + FileName);
                                Reader.Close();
                                return Name;
                            }
                            else
                            {
                                Reader.Close(); //Just to avoid memory leaks
                                return "Name not found.";
                            }
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