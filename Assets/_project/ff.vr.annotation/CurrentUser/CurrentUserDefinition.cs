using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff.vr.annotate
{
    /** A quick hack to provide stub information about the current user. 
    This information is used for saving annotations.*/
    public class CurrentUserDefinition : MonoBehaviour
    {
        public Person CurrentUser;

        void Start()
        {
            if (_instance != null)
            {
                throw new UnityException("Only one instance of " + this + " allowed");
            }
            _instance = this;
        }

        public static CurrentUserDefinition _instance;
    }
}
