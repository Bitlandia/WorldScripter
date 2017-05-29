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

partial class ConsoleInputMan {

    [Command("create")]
    public void Create(string[] Args)
    {
        try
        {
            GameObject Obj = null;
            OBJData DataToUpd;
            var FilePath = PathToModelFolder + "/" + Args[1];
            //Object Types
            if (Args[1] == "cube")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                DataToUpd = Obj.AddComponent<OBJData>();
            }
            else if (Args[1] == "sphere")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                DataToUpd = Obj.AddComponent<OBJData>();
            }
            else if (Args[1] == "cylinder")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                DataToUpd = Obj.AddComponent<OBJData>();
            }
            else if (Args[1] == "capsule")
            {
                Obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                DataToUpd = Obj.AddComponent<OBJData>();
            }
            else if (File.Exists(FilePath))
            {
                bool NeedToLoad = true;
                if (GOCache.ToArray().Length > 0)
                {
                    foreach (GameObject GameOBJ in GOCache)
                    {
                        if (GameOBJ.GetComponent<OBJData>().CachePath == FilePath)
                        {
                            NeedToLoad = false;
                            Obj = Instantiate(GameOBJ); //Load it from the cache!
                            Obj.SetActive(true);
                        }
                    }
                }
                MeshCollider MeshCollide;
                if (NeedToLoad == true)
                    Obj = OBJLoader.LoadOBJFile(FilePath); //Create the object
                //Add the collision - crappy hack
                List<GameObject> ChildrenGo = new List<GameObject>();
                int Children = Obj.transform.childCount;
                for (int i = 0; i < Children; ++i)
                    ChildrenGo.Add(Obj.transform.GetChild(i).gameObject);
                foreach (GameObject ObjMesh in ChildrenGo)
                {
                    MeshCollide = ObjMesh.AddComponent<MeshCollider>();
                    MeshCollide.convex = true;
                    MeshCollide.inflateMesh = true;
                    MeshCollide.skinWidth = float.Parse(Args[9]);
                    DataToUpd = ObjMesh.AddComponent<OBJData>();
                    DataToUpd.IsPartOfMesh = true;
                }
                DataToUpd = Obj.AddComponent<OBJData>();
                if (NeedToLoad == true)
                {
                    DataToUpd.CachePath = FilePath;
                    GameObject ObjToCache = Instantiate(Obj);
                    GOCache.Add(ObjToCache);
                    ObjToCache.SetActive(false);
                }
            }
            else
                return; //No shape, stop
            Obj.SetActive(true); //just incase
            Obj.transform.position = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[4])); //Set the position
            Obj.transform.localScale = new Vector3(float.Parse(Args[5]), float.Parse(Args[6]), float.Parse(Args[7])); //Set the scale
            Obj.name = Args[8]; //Set the name
            Obj.tag = "ConsoleCreated"; //Set the tag
            //SendMSG("Created " + Args[1] + " at " + Args[2] + ", " + Args[3] + ", " + Args[4] + " with scale of " + Args[5] + ", " + Args[6] + ", " + Args[7] + " named " + Args[8] + ".");
        }
        catch (System.Exception e) { SendMSG("Unexpected error"); Debug.LogError(e.ToString()); }
    }
    [Command("list")]
    public void List(string[] Args)
    {
        //Take every object with ConsoleCreated and list it.
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            SendMSG(Obj.name + " @ " + Obj.transform.localPosition.x + ", " + Obj.transform.localPosition.y + ", " + Obj.transform.localPosition.z);
        }
    }
    [Command("color")]
    public void Color(string[] Args)
    {
        try
        {
            GameObject[] Objs;
            Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
            foreach (GameObject Obj in Objs)
            {
                if (Obj.name == Args[1])
                {
                    Obj.GetComponent<Renderer>().material.color = new Color32(byte.Parse(Args[2]), byte.Parse(Args[3]), byte.Parse(Args[4]), byte.Parse(Args[5]));
                    //SendMSG("Changed color of " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
                }
            }
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("destroy")]
    public void Destroy(string[] Args)
    {
        try
        {
            GameObject[] Objs;
            Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
            foreach (GameObject Obj in Objs)
            {
                if (Obj.name == Args[1])
                {
                    GameObject.Destroy(Obj);
                    //SendMSG("Destroyed " + Args[1] + ".");
                }
            }
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("move")]
    public void Move(string[] Args)
    {
        try
        {
            GameObject[] Objs;
            Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
            foreach (GameObject Obj in Objs)
            {
                if (Obj.name == Args[1])
                {
                    Obj.transform.position = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[4]));
                    //SendMSG("Moved " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
                }
            }
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
                    ToOut = ToOut + " " + Arg;
            }
            SendMSG(ToOut);
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("run")]
    public void Run(string[] Args)
    {
        //try
        {
            string Line;
            string FileName = PathToScriptFolder + "/"; //incase you have spaces
            foreach (string Arg in Args)
            {
                if (Arg == "run") { }
                else if (Arg == Args[1]) { FileName = FileName + Arg; }
                else
                    FileName = FileName + " " + Arg;
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
                        if (!Line.StartsWith("name") | !Line.StartsWith("//"))  //Ignore the meta data line
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
        //catch
        {
            //SendMSG("Unexpected error");
        }
    }
    [Command("rotate")]
    public void Rotate(string[] Args)
    {
        try
        {
            GameObject[] Objs;
            Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
            foreach (GameObject Obj in Objs)
            {
                if (Obj.name == Args[1])
                {
                    Obj.transform.rotation = new Quaternion(int.Parse(Args[2]), int.Parse(Args[3]), int.Parse(Args[4]), int.Parse(Args[5]));
                    //SendMSG("Rotated " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ", " + Args[5] + ".");
                }
            }
        }
        catch { SendMSG("Unexpected error"); }
    }
    [Command("scale")]
    public void Scale(string[] Args)
    {
        try
        {
            GameObject[] Objs;
            Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
            foreach (GameObject Obj in Objs)
            {
                if (Obj.name == Args[1])
                {
                    Obj.transform.localScale = new Vector3(float.Parse(Args[2]), float.Parse(Args[3]), float.Parse(Args[5]));
                    //SendMSG("Scaled " + Args[1] + " to " + Args[2] + ", " + Args[3] + ", " + Args[4] + ".");
                }
            }
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
            else if (Arg == Args[2]) { FilePath = FilePath + Arg; }
            else
                FilePath = FilePath + " " + Arg;
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
                    //SendMSG("Added texture to " + Args[1]);
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
            if (Obj.name == Args[1])
            {
                try
                {
                    Obj.AddComponent(ConsoleUtils.FindType(Args[2], false, true)); // Actually get the type
                    //SendMSG("Added " + Args[2] + " to " + Args[1]);
                }
                catch { }
            }
        }
    }
    [Command("add_button")]
    public void AddButton(string[] Args)
    {
        GameObject[] Objs;
        string CMDName = ""; //incase you have spaces
        foreach (string Arg in Args)
        {
            if (Arg == "add_button") { }
            else if (Arg == Args[1]) { }
            else if (Arg == Args[2]) { CMDName = CMDName + Arg; }
            else
                CMDName = CMDName + " " + Arg;
        }
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                Obj.GetComponent<OBJData>().IsButton = true;
                Obj.GetComponent<OBJData>().ButtonCMD = CMDName;
                //Add to mesh - crappyhack
                List<GameObject> ChildrenGo = new List<GameObject>();
                int Children = Obj.transform.childCount;
                for (int i = 0; i < Children; ++i)
                    ChildrenGo.Add(Obj.transform.GetChild(i).gameObject);
                foreach (GameObject ObjMesh in ChildrenGo)
                {
                    ObjMesh.GetComponent<OBJData>().IsButton = true;
                    ObjMesh.GetComponent<OBJData>().ButtonCMD = CMDName;
                }
            }
        }
    }
    [Command("del_button")]
    public void DelButton(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                Obj.GetComponent<OBJData>().IsButton = false;
                //Remove from mesh - crappy hack
                List<GameObject> ChildrenGo = new List<GameObject>();
                int Children = Obj.transform.childCount;
                for (int i = 0; i < Children; ++i)
                    ChildrenGo.Add(Obj.transform.GetChild(i).gameObject);
                foreach (GameObject ObjMesh in ChildrenGo)
                {
                    Obj.GetComponent<OBJData>().IsButton = false;
                }
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
            else if (Arg == Args[2]) { FileName = FileName + Arg; }
            else
                FileName = FileName + " " + Arg;
        }
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                try
                {
                    AudioHandler Audio = Obj.AddComponent<AudioHandler>();
                    Audio.Run(FileName);
                    //SendMSG("Added sound to " + Args[1]);
                }
                catch { }
            }
        }
    }
    [Command("play_sound")]
    public void PlaySound(string[] Args)
    {
        string FileName = "file:///" + PathToAudioFolder + "/"; //incase you have spaces
        foreach (string Arg in Args)
        {
            if (Arg == "play_sound") { }
            else if (Arg == Args[1]) { FileName = FileName + Arg; }
            else
                FileName = FileName + " " + Arg;
        }
        AudioPlayer.Run(FileName);
    }
    [Command("del_comp")]
    public void DelComp(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                System.Type ToRemove = ConsoleUtils.FindType(Args[2], false, true);
                Destroy(Obj.GetComponent(ToRemove));
            }
        }
    }
    [Command("del_sound")]
    public void DelSound(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                Destroy(Obj.GetComponent<AudioHandler>());
                Destroy(Obj.GetComponent<AudioSource>());
            }
        }
    }
    [Command("del_texture")]
    public void DelTexture(string[] Args)
    {
        GameObject[] Objs;
        Renderer Renderer;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                Renderer = Obj.GetComponent<Renderer>();
                var Tex = new Texture2D(1, 1);
                Renderer.material.mainTexture = Tex; //litterally just sets it to a white square
                //SendMSG("Removed texture from " + Args[1]);
            }
        }
    }
    [Command("clear")]
    public void Clear(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            Destroy(Obj);
        }
        SendMSG("Cleared all objects");
    }
    [Command("create_raycast")]
    public void CreateRaycast(string[] Args)
    {
        RaycastHit hit;
        Transform Cam = CameraFPS.transform;
        var Ray = new Ray(Cam.position, Cam.forward);
        if (Physics.Raycast(Ray, out hit, 500f))
        {
            //Debug.Log(hit.distance); //debug code, uncomment if something breaks
            Debug.Log(hit.point.x + " " + hit.point.y + " " + hit.point.z);
            float y = hit.point.y;
            if (Args[1].Contains(".obj"))
                ParseCommand("create " + Args[1] + " " + hit.point.x + " " + y + " " + hit.point.z + " " + Args[2] + " " + Args[3] + " " + Args[4] + " " + Args[5] + " " + Args[6]);
            else
                ParseCommand("create " + Args[1] + " " + hit.point.x + " " + y + " " + hit.point.z + " " + Args[2] + " " + Args[3] + " " + Args[4] + " " + Args[5]);
        }
    }
    [Command("add_light")]
    public void AddLight(string[] Args)
    {
        GameObject[] Objs;
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated");
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                Light LightToEdit = Obj.AddComponent<Light>();
                LightToEdit.intensity = int.Parse(Args[2]);
                LightToEdit.range = int.Parse(Args[7]);
                LightToEdit.color = new Color32(byte.Parse(Args[3]), byte.Parse(Args[4]), byte.Parse(Args[5]), byte.Parse(Args[6]));
            }
        }
    }
    [Command("add_particle")]
    public void AddParticle(string[] Args)
    {
        GameObject[] Objs;
        ParticleSystem Emitter;
        ParticleSystemRenderer Renderer;
        ParticleSystem.ShapeModule ShapeModule;
        ParticleSystem.MainModule Emission;
        var FilePath = PathToTextureFolder + "/";
        foreach (string Arg in Args)
        {
            //Just if you have files in the name.
            if (Arg == "add_particle") { }
            else if (Arg == Args[1]) { }
            else if (Arg == Args[2]) { FilePath = FilePath + Arg; }
            else
                FilePath = FilePath + " " + Arg;
        }
        Objs = GameObject.FindGameObjectsWithTag("ConsoleCreated"); // Make sure we aren't doing something stupid here
        foreach (GameObject Obj in Objs)
        {
            if (Obj.name == Args[1])
            {
                try
                {
                    if (System.IO.File.Exists(FilePath))
                    {
                        Emitter = Obj.AddComponent<ParticleSystem>(); // Actually get the type
                        Renderer = Obj.GetComponent<ParticleSystemRenderer>();
                        var Bytes = System.IO.File.ReadAllBytes(FilePath);
                        var Tex = new Texture2D(1, 1); //Load the texture onto a texture
                        ShapeModule = Emitter.shape;
                        Tex.LoadImage(Bytes);
                        Renderer.material.mainTexture = Tex;
                        ShapeModule.shapeType = ParticleSystemShapeType.Sphere;
                        Renderer.material.shader = Shader.Find("Particles/Alpha Blended");
                        Emission = Emitter.main;
                        Emission.maxParticles = 100;
                        Emission.startLifetime = 0.5F;
                        Emitter.Play();
                        //SendMSG("Added particle to " + Args[1]);
                    }
                }
                catch { }
            }
        }
    }

}
