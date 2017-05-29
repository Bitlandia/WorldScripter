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

    [ButtonCommand("replace_with")]
    public void ReplaceWith(string[] Args, GameObject Object)
    {
        Vector3 ObjectVec = Object.transform.position;
        Vector3 Scale = Object.transform.localScale;
        string Name = Object.name;
        Debug.Log(Name);
        Destroy(Object);
        if (Args.Length == 3)
            ParseCommand("create " + Args[1] + " " + ObjectVec.x.ToString() + " " + ObjectVec.y.ToString() + " " + ObjectVec.z.ToString() + " " + Scale.x.ToString() + " " + Scale.y.ToString() + " " + Scale.z.ToString() + " " + name + " " + Args[2]);
        else
            ParseCommand("create " + Args[1] + " " + ObjectVec.x.ToString() + " " + ObjectVec.y.ToString() + " " + ObjectVec.z.ToString() + " " + Scale.x.ToString() + " " + Scale.y.ToString() + " " + Scale.z.ToString() + " " + name + " " + "0");
    }
    [ButtonCommand("run_local")]
    public void RunLocal(string[] Args, GameObject Object)
    {
        //try
        {
            string Line;
            string FileName = PathToScriptFolder + "/"; //incase you have spaces
            foreach (string Arg in Args)
            {
                if (Arg == "run_local") { }
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
                            ParseButtonCommand(Line, Object);
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
    [ButtonCommand("self_destruct")]
    public void SelfDestruct(string[] Args, GameObject Object)
    {
        Destroy(Object);
    }
    [ButtonCommand("change_button")]
    public void ChangeButton(string[] Args, GameObject Object)
    {
        string CMDName = ""; //incase you have spaces
        foreach (string Arg in Args)
        {
            if (Arg == "add_button") { }
            else if (Arg == Args[1]) { }
            else if (Arg == Args[2]) { CMDName = CMDName + Arg; }
            else
                CMDName = CMDName + " " + Arg;
        }
        Object.GetComponent<OBJData>().IsButton = true;
        Object.GetComponent<OBJData>().ButtonCMD = CMDName;
        //Add to mesh - crappyhack
        List<GameObject> ChildrenGo = new List<GameObject>();
        int Children = Object.transform.childCount;
        for (int i = 0; i < Children; ++i)
            ChildrenGo.Add(Object.transform.GetChild(i).gameObject);
        foreach (GameObject ObjMesh in ChildrenGo)
        {
            ObjMesh.GetComponent<OBJData>().IsButton = true;
            ObjMesh.GetComponent<OBJData>().ButtonCMD = CMDName;
        }
    }
    [ButtonCommand("add_light_local")]
    public void AddLightLocal(string[] Args, GameObject Object)
    {
        Light LightToEdit = Object.AddComponent<Light>();
        LightToEdit.intensity = int.Parse(Args[1]);
        LightToEdit.range = int.Parse(Args[6]);
        LightToEdit.color = new Color32(byte.Parse(Args[2]), byte.Parse(Args[3]), byte.Parse(Args[4]), byte.Parse(Args[5]));
    }
    [Command("color_local")]
    public void ColorLocal(string[] Args, GameObject Object)
    {
        Object.GetComponent<Renderer>().material.color = new Color32(byte.Parse(Args[1]), byte.Parse(Args[2]), byte.Parse(Args[3]), byte.Parse(Args[4]));
    }
    [Command("move_local")]
    public void MoveLocal(string[] Args, GameObject Object)
    {
        Object.transform.position = new Vector3(float.Parse(Args[1]), float.Parse(Args[2]), float.Parse(Args[3]));
    }
    [Command("rotate_local")]
    public void RotateLocal(string[] Args, GameObject Object)
    {
        Object.transform.rotation = new Quaternion(int.Parse(Args[1]), int.Parse(Args[2]), int.Parse(Args[3]), 0);
    }
    [Command("scale_local")]
    public void ScaleLocal(string[] Args, GameObject Object)
    {
        Object.transform.localScale = new Vector3(float.Parse(Args[1]), float.Parse(Args[2]), float.Parse(Args[3]));
    }
    [Command("add_texture_local")]
    public void AddTextureLocal(string[] Args, GameObject Object)
    {
        Renderer Renderer;
        var FilePath = PathToTextureFolder + "/";
        foreach (string Arg in Args)
        {
            //Just if you have files in the name.
            if (Arg == "add_texture_local") { }
            else if (Arg == Args[1]) { FilePath = FilePath + Arg; }
            else
                FilePath = FilePath + " " + Arg;
        }
        if (System.IO.File.Exists(FilePath))
        {
            Renderer = Object.GetComponent<Renderer>();
            var Bytes = System.IO.File.ReadAllBytes(FilePath);
            var Tex = new Texture2D(1, 1); //Load the texture onto a texture
            Tex.LoadImage(Bytes);
            Renderer.material.mainTexture = Tex;
            //SendMSG("Added texture to " + Args[1]);
        }
    }
    [Command("add_comp_local")]
    public void AddCompLocal(string[] Args, GameObject Object)
    {
        Object.AddComponent(ConsoleUtils.FindType(Args[1], false, true));
    }
    [Command("del_button_local")]
    public void DelButtonLocal(string[] Args, GameObject Object)
    {
        Object.GetComponent<OBJData>().IsButton = false;
        //Remove from mesh - crappy hack
        List<GameObject> ChildrenGo = new List<GameObject>();
        int Children = Object.transform.childCount;
        for (int i = 0; i < Children; ++i)
            ChildrenGo.Add(Object.transform.GetChild(i).gameObject);
        foreach (GameObject ObjMesh in ChildrenGo)
        {
            ObjMesh.GetComponent<OBJData>().IsButton = false;
        }
    }
    [Command("add_sound_local")]
    public void AddSoundLocal(string[] Args, GameObject Object)
    {
        string FileName = "file:///" + PathToAudioFolder + "/"; //incase you have spaces
        foreach (string Arg in Args)
        {
            if (Arg == "add_sound_local") { }
            else if (Arg == Args[1]) { FileName = FileName + Arg; }
            else
                FileName = FileName + " " + Arg;
        }
        AudioHandler Audio = Object.AddComponent<AudioHandler>();
        Audio.Run(FileName);
    }
    [Command("del_comp_local")]
    public void DelCompLocal(string[] Args, GameObject Object)
    {
        System.Type ToRemove = ConsoleUtils.FindType(Args[1], false, true);
        Destroy(Object.GetComponent(ToRemove));
    }
    [Command("del_sound_local")]
    public void DelSoundLocal(string[] Args, GameObject Object)
    {
        Destroy(Object.GetComponent<AudioHandler>());
        Destroy(Object.GetComponent<AudioSource>());
    }
    [Command("del_texture_local")]
    public void DelTextureLocal(string[] Args, GameObject Object)
    {
        Renderer Renderer = Object.GetComponent<Renderer>();
        var Tex = new Texture2D(1, 1);
        Renderer.material.mainTexture = Tex; //litterally just sets it to a white square
    }
    [Command("add_particle_local")]
    public void AddParticleLocal(string[] Args, GameObject Object)
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
            if (Arg == "add_particle_local") { }
            else if (Arg == Args[1]) { FilePath = FilePath + Arg; }
            else
                FilePath = FilePath + " " + Arg;
        }
        try
        {
            if (System.IO.File.Exists(FilePath))
            {
                Emitter = Object.AddComponent<ParticleSystem>(); // Actually get the type
                Renderer = Object.GetComponent<ParticleSystemRenderer>();
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
