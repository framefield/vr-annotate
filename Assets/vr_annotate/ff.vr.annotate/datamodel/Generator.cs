using System;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotate.datamodel
{
    [Serializable]
    public class Generator
    {
        public string id;
        public string type;
        public string name;

        public static Generator AnonymousUser
        {
            get
            {
                return new Generator()
                {
                    id = "_anonymous",
                    type = "anonymous",
                    name = "undefined"
                };
            }
        }
    }
}
