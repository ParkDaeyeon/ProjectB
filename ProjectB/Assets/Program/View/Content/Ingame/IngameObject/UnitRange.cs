using UnityEngine;

using System;

using Ext.Unity3D;

namespace Program.View.Content.Ingame.IngameObject
{
    public class UnitRange : ManagedComponent
    {
        Action<Collision2D> onColEnter;
        public Action<Collision2D> OnColEnter
        {
            set { this.onColEnter = value; }
            get { return this.onColEnter; }
        }
        Action<Collision2D> onColStay;
        public Action<Collision2D> OnColStay
        {
            set { this.onColStay = value; }
            get { return this.onColStay; }
        }
        Action<Collision2D> onColExit;
        public Action<Collision2D> OnColExit
        {
            set { this.onColExit = value; }
            get { return this.onColExit; }
        }
        Action<Collider2D> onTriEnter;
        public Action<Collider2D> OnTriEnter
        {
            set { this.onTriEnter = value; }
            get { return this.onTriEnter; }
        }
        Action<Collider2D> onTriStay;
        public Action<Collider2D> OnTriStay
        {
            set { this.onTriStay = value; }
            get { return this.onTriStay; }
        }
        Action<Collider2D> onTriExit;
        public Action<Collider2D> OnTriExit
        {
            set { this.onTriExit = value; }
            get { return this.onTriExit; }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (null != this.onColEnter)
                this.onColEnter(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (null != this.onColStay)
                this.onColStay(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (null != this.onColExit)
                this.onColExit(collision);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (null != this.onTriEnter)
                this.onTriEnter(collision);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (null != this.onTriStay)
                this.onTriStay(collision);
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (null != this.onTriExit)
                this.onTriExit(collision);
        }
    }
}
