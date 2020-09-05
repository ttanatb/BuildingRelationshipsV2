using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env
{
    public class Utils
    {
        public static Transform FillVar(string substr, Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);

                if (child.name.Contains(substr))
                    return child;
            }

            return null;
        }


        public static Transform[] FillVars(string substr, Transform transform, string doesNotIncl = "")
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name.Contains(substr) && !
                    (doesNotIncl != "" && child.name.Contains(doesNotIncl)))
                    list.Add(child);
            }

            return list.ToArray();
        }
    }
}