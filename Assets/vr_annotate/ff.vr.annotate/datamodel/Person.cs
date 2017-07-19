using System;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotate.datamodel
{
    [Serializable]
    public class Person
    {
        public string id;
        public string name;
        public string email;

        public static Dictionary<string, Person> PeopleById = new Dictionary<string, Person>();

        public static Person AnonymousUser
        {
            get
            {
                return new Person()
                {
                    id = "_anonymous",
                    name = "anonymous",
                    email = "undefined"
                };
            }
        }
    }
}
