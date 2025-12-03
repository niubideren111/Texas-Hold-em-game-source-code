namespace Runtime 
{
    using System.Collections;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class LuaUIObject : MonoBehaviour
    {
        public Action startEvent;
        public Action destroyEvent;
        public Action enableEvent;
        public Action disableEvent;
        public Action guiEvent;

        public void SetStartEvent(Action act) {
            startEvent = act;
        }

        public void SetDestroyEvent(Action act) {
            destroyEvent = act;
        }

        public void SetEnableEvent(Action act) {
            enableEvent = act;
        }

        public void SetDisableEvent(Action act) {
            disableEvent = act;
        }

        public void SetOnGUIEvent(Action act) {
            guiEvent = act;
        }

        void Start()
        {
            if (startEvent != null) {
                startEvent();
            }
        }

        void OnGUI()
        {
            if (guiEvent != null){
                guiEvent();
            }
        }

        void OnDestroy()
        {
            if (destroyEvent != null) {
                destroyEvent();
            }
        }

        void OnEnable() 
        {
            if (enableEvent != null) {
                enableEvent();
            }
        }

        void OnDisable() {
            if (disableEvent != null) {
                disableEvent();
            }
        }
    }
}