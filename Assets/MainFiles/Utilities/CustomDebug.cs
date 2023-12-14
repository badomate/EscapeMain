using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class CustomDebug
    {
        public enum Type
        {
            NONE,
            GEN,
            ALEX,
            JANOS,
            MATE,
            DIOGO,
            HENRIQUE,
            AGENT,
            GAMEPLAY,
            SENSORS,
            OTHER
        }

        public static Dictionary<Type, bool> isTypeActiveInfo =
            new Dictionary<Type, bool>() {
                {Type.GEN, false},
                {Type.ALEX, true},
                {Type.JANOS, false},
                {Type.MATE, false},
                {Type.DIOGO, false},
                {Type.HENRIQUE, false},
                {Type.AGENT, false},
                {Type.GAMEPLAY, false},
                {Type.SENSORS, false},
                {Type.OTHER, false}
            };

        public static void Log(string msg, List<Type> debugTypes)
        {
            foreach(Type debugType in debugTypes)
            {
                bool isTypeActive = isTypeActiveInfo[debugType];
                if (isTypeActive)
                    Debug.Log(msg);
            }
        }
    }
}