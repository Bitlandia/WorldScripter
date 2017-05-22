using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace UConsole
{
    public class ConsoleUtils : MonoBehaviour
    {

        public static System.Type FindType(string TypeName, bool UseFullName = false, bool IgnoreCase = false)
        {
            //I didn't write this, someone on unity answers did... I don't really know how it works so I wont touch it
            if (string.IsNullOrEmpty(TypeName)) return null;

            StringComparison e = (IgnoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            if (UseFullName)
            {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assemb.GetTypes())
                    {
                        if (string.Equals(t.FullName, TypeName, e)) return t;
                    }
                }
            }
            else
            {
                foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assemb.GetTypes())
                    {
                        if (string.Equals(t.Name, TypeName, e)) return t;
                    }
                }
            }
            return null;
        }
    }
}
