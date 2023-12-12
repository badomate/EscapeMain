using UnityEngine;

namespace AuxiliarContent
{
    public static class CustomDebug
    {
        public static bool genIsOn = false;
        public static bool alexIsOn = false;
        public static bool janosIsOn = true;
        public static bool mateIsOn = true;

        public static void LogGen(string msg)
        {
            if (genIsOn)
                Debug.Log(msg);
        }

        public static void LogAlex(string msg)
        {
            if (alexIsOn)
                Debug.Log(msg);
        }

        public static void LogJanos(string msg)
        {
            if (janosIsOn)
                Debug.Log(msg);
        }

        public static void LogMate(string msg)
        {
            if (mateIsOn)
                Debug.Log(msg);
        }
    }
}