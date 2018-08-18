using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Collection.AntiGC;
#if !UNITY_EDITOR && UNITY_SWITCH
using nn.hid;
#endif// !UNITY_EDITOR && UNITY_SWITCH
namespace Ext.Unity3D.UI
{
    public class ScreenTouches : Responsive
    {
        protected override void OnResize()
        {
            this.SetCustomSize();
            this.SetCustomOffset();
            this.ResetManualResolution();

            this.UpdateResolution();
        }


        Vector2 customSize = Vector2.zero;
        //public void SetCustomSize(Vector2 value)
        //{
        //    this.customSize = value;
        //}
        public void SetCustomSize()
        {
            var value = UnityExtension.ScreenSafeAreaRect.size;
            this.customSize = value;
        }
        public Vector2 GetCustomSize()
        {
            return this.customSize;
        }

        Vector2 customOffset = Vector2.zero;
        //public void SetCustomOffset(Vector2 value)
        //{
        //    this.customOffset = value;
        //}
        public void SetCustomOffset()
        {
            var value = UnityExtension.ScreenSafeAreaRect.min;            
            this.customOffset = value;
        }
        public Vector2 GetCustomOffset()
        {
            return this.customOffset;
        }
        
        //public void ResetManualResolution(Vector2 manualArea)
        //{
        //    this.manualResolution = manualArea;
        //    this.UpdateResolution();
        //}
        public void ResetManualResolution()
        {
            var safeArea = Responsive.GetAreaSizeByMode(AreaMode.Safe);
            var scale = UnityExtension.GetScaleOfAreaSize(safeArea, Responsive.GetAreaSizeByMode(AreaMode.Viewport));
            var manualArea = safeArea / scale;

            this.manualResolution = manualArea;
        }

        public virtual void UpdateResolution()
        {
            this.manualAspectRadio = this.manualResolution.x / this.manualResolution.y;

            var resolution = Vector2.zero != this.customSize ? this.customSize : Responsive.ViewportAreaSize;
            //var resolution = new Vector2(Screen.width, Screen.height);
            this.currentAspectRadio = resolution.x / resolution.y;

            this.toAreaOffset = Vector2.zero;
            if (this.useBezel)
            {
                if (this.manualAspectRadio < this.currentAspectRadio)
                {
                    var originSize = resolution.x;
                    resolution = new Vector2(resolution.y * this.manualAspectRadio, resolution.y);
                    this.toAreaOffset.x = (originSize - resolution.x) * 0.5f;
                }
                else
                {
                    var originSize = resolution.y;
                    resolution = new Vector2(resolution.x, resolution.x / this.manualAspectRadio);
                    this.toAreaOffset.y = (originSize - resolution.y) * 0.5f;
                }
            }
            this.toAreaOffset += this.customOffset;

            this.toScreen.x = resolution.x / this.manualResolution.x;
            this.toScreen.y = resolution.y / this.manualResolution.y;
            this.toArea.x = this.manualResolution.x / resolution.x;
            this.toArea.y = this.manualResolution.y / resolution.y;
        }


        [SerializeField]
        Vector2 manualResolution = new Vector2(1024, 640);
        public Vector2 ManualResolution
        {
            get { return this.manualResolution; }
        }

        float manualAspectRadio = 1;
        public float ManualAspectRadio
        {
            get { return this.manualAspectRadio; }
        }

        float currentAspectRadio = 1;
        public float CurrentAspectRadio
        {
            get { return this.currentAspectRadio; }
        }


        [SerializeField]
        bool useBezel = true;
        public bool UseBezel
        {
            get { return this.useBezel; }
        }
        

        Vector2 toScreen;
        public Vector2 ToScreen
        {
            get { return this.toScreen; }
        }
        Vector2 toArea;
        public Vector2 ToArea
        {
            get { return this.toArea; }
        }

        Vector2 toAreaOffset;
        public Vector2 ToAreaOffset
        {
            get { return this.toAreaOffset; }
        }

        void Start()
        {
            this.UpdateResolution();

            this.OnStart();
        }

        protected virtual void OnStart() { }


        public enum State
        {
            Start,
            Moved,
            Held,
            End,

            None,
        }

        public class Handle
        {
            public bool use = false;

            public int id = -1;
            public Vector2 inputPosition;
            public Vector2 screenPosition;
            public State state = State.None;
            public LinkedListNode<Handle> nodeHandle = null;

            public bool IsStarted { get { return State.Start == this.state; } }
            public bool IsMoved { get { return State.Moved == this.state; } }
            public bool IsHelded { get { return State.Held == this.state; } }
            public bool IsEnded { get { return State.End == this.state; } }

            public override string ToString()
            {
                return string.Format("use:{0}, id:{1}, ip:{2}, sp:{3}, st:{4}, node:{5}",
                                      this.use,
                                      this.id,
                                      this.inputPosition,
                                      this.screenPosition,
                                      this.state,
                                      this.nodeHandle);
            }
        }

        protected CachedPool<Handle> touches;
//#if !UNITY_EDITOR && UNITY_SWITCH
//        TouchScreenState5 touchScreenStates = new TouchScreenState5();
//#endif// !UNITY_EDITOR && UNITY_SWITCH
        void Awake()
        {
            this.touches = new CachedPool<Handle>(32, this.OnAllocateTouchHandle);

//#if !UNITY_EDITOR && UNITY_SWITCH
//            this.touchScreenStates.SetDefault();
//#endif// !UNITY_EDITOR && UNITY_SWITCH
        }

        protected virtual Handle OnAllocateTouchHandle(CachedPool<Handle> pool) { return new Handle(); }

        
        public int Count { get { return null != this.touches ? this.touches.Count : 0; } }
        public LinkedListNode<Handle> First { get { return null != this.touches ? this.touches.First : null; } }
        public LinkedListNode<Handle> Last { get { return null != this.touches ? this.touches.Last : null; } }

        public LinkedListNode<Handle> Find(int id)
        {
            if (null == this.touches)
                return null;

            var node = this.touches.First;
            while (null != node)
            {
                if (node.Value.id == id)
                {
                    if (State.End != node.Value.state)
                        return node;
                }

                node = node.Next;
            }

            return null;
        }

        public void Clear()
        {
            if (null == this.touches)
                return;

            var node = this.touches.First;
            while (null != node)
            {
                var touchHandle = node.Value;
                node = node.Next;

                touchHandle.state = State.None;
                touchHandle.use = false;

                this.OnRemoveTouch(touchHandle);

                touchHandle.id = -1;
                touchHandle.nodeHandle = null;
            }

            this.touches.Clear();
        }

        protected virtual void OnRemoveTouch(Handle touchHandle) { }





        void BeginTouches()
        {
            var node = this.touches.First;
            while (null != node)
            {
                var touchHandle = node.Value;
                node = node.Next;

                touchHandle.use = false;
            }
        }

        void EndTouches()
        {
            var node = this.touches.First;
            var nodePrev = node;
            while (null != node)
            {
                var touchHandle = node.Value;

                if (!touchHandle.use)
                {
                    if (State.End == touchHandle.state)
                    {
                        this.OnRemoveTouch(touchHandle);

                        touchHandle.id = -1;
                        touchHandle.nodeHandle = null;
                        touchHandle.state = State.None;

                        nodePrev = node;
                        node = node.Next;
                        this.touches.Remove(nodePrev);
                        continue;
                    }

                    touchHandle.state = State.End;
                }

                node = node.Next;
            }
        }
        
        void OnTouch(int id, Vector2 inputPosition, Vector2 screenPosition)
        {
            Handle touchHandle = null;

            var node = this.Find(id);
            if (null != node)
            {
                touchHandle = node.Value;
                touchHandle.state = touchHandle.screenPosition == screenPosition ? State.Held : State.Moved;
            }
            else
            {
                node = this.touches.AllocLast();
                touchHandle = node.Value;
                touchHandle.id = id;
                touchHandle.nodeHandle = node;
                touchHandle.state = State.Start;
            }

            touchHandle.use = true;
            touchHandle.inputPosition = inputPosition;
            touchHandle.screenPosition = screenPosition;

            this.OnUpdateTouch(touchHandle);
        }
        
        protected virtual void OnUpdateTouch(Handle touchHandle) { }


        public void UpdateTouches()
        {
            if (null == this.touches)
                return;

            this.BeginTouches();

#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButton(0))
            {
                var inputPosition = Input.mousePosition;
                var screenPosition = new Vector2(inputPosition.x, inputPosition.y);
                screenPosition -= this.toAreaOffset;
                screenPosition.x *= this.toArea.x;
                screenPosition.y *= this.toArea.y;
                this.OnTouch(0, inputPosition, screenPosition);
            }
//#elif !UNITY_EDITOR && UNITY_SWITCH
//            var height = Screen.height;
//            TouchScreen.GetState(ref this.touchScreenStates);
//            for (int n = 0; n < this.touchScreenStates.count; n++)
//            {
//                var touchState = this.touchScreenStates.touches[n];
//                switch (touchState.attributes)
//                {
//                case TouchAttribute.End:
//                    break;

//                case TouchAttribute.Start:
//                default:
//                    {
//                        var inputPosition = new Vector2(touchState.x, height - touchState.y);
//                        var screenPosition = inputPosition;
//                        screenPosition -= this.toAreaOffset;
//                        screenPosition.x *= this.toArea.x;
//                        screenPosition.y *= this.toArea.y;
//                        this.OnTouch(touchState.fingerId, inputPosition, screenPosition);
//                        break;
//                    }
//                }
//            }
#else// UNITY_EDITOR || UNITY_STANDALONE
            for (int n = 0, cnt = Input.touchCount; n < cnt; ++n)
            {
                var unityTouch = Input.GetTouch(n);

                switch (unityTouch.phase)
                {
                case TouchPhase.Began:
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    {
                        var inputPosition = unityTouch.position;
                        var screenPosition = new Vector2(inputPosition.x, inputPosition.y);
                        screenPosition -= this.toAreaOffset;
                        screenPosition.x *= this.toArea.x;
                        screenPosition.y *= this.toArea.y;
                        this.OnTouch(unityTouch.fingerId, inputPosition, screenPosition);
                        break;
                    }
                }
            }
#endif// UNITY_EDITOR || UNITY_STANDALONE

            var vpNode = this.virtualPresses.First;
            while (null != vpNode)
            {
                var virtualPress = vpNode.Value;
                vpNode = vpNode.Next;
                
                this.OnTouch(virtualPress.id, virtualPress.screenPosition, virtualPress.screenPosition);
            }
            this.virtualPresses.Clear();

            this.EndTouches();
        }


        struct VirtualPress
        {
            public int id;
            public Vector2 screenPosition;
        }

        CachedList<VirtualPress> virtualPresses = new CachedList<VirtualPress>(32);

        public void PressSimulate(int id, Vector2 screenPosition)
        {
            this.virtualPresses.AddLast(new VirtualPress { id = id, screenPosition = screenPosition, });
        }



//        // NOTE: TEST!
//        [SerializeField]
//        RectTransform testBase;

//        [SerializeField]
//        Sprite testSpritePressStart;
//        [SerializeField]
//        Sprite testSpritePressMoved;
//        [SerializeField]
//        Sprite testSpritePressHeld;
//        [SerializeField]
//        Sprite testSpritePressEnd;

//        FastArray<Image> testImages;

//        void LateUpdate()
//        {
//            if (null == this.touches)
//                return;
//
////            if (!this.testBase)
////                return;
////
////            if (null == this.testImages)
////            {
////                this.testImages = new FastArray<Image>(32);
////                for (int n = 0; n < 32; ++n)
////                {
////                    var img = this.OnCreateTestImage(n);
////                    this.SetActive(false, img);
////                    this.testImages.Add(img);
////                }
////            }
////
////            int useCount = 0;
////            var node = this.touches.First;
////            while (null != node)
////            {
////                var touchHandle = node.Value;
////                node = node.Next;
////
////                Image img = null;
////                if (useCount < this.testImages.Count)
////                    img = this.testImages[useCount++];
////                else
////                    this.testImages.Add(img = this.OnCreateTestImage(useCount++));
////
////                this.SetActive(true, img);
////
////                var t = img.rectTransform;
////                t.position = touchHandle.screenPosition;
////
////                var pos = t.localPosition;
////                pos.z = 0;
////                t.localPosition = pos;
////                img.sprite = State.Start == touchHandle.state ? this.testSpritePressStart :
////                             State.Moved == touchHandle.state ? this.testSpritePressMoved :
////                             State.Held == touchHandle.state ? this.testSpritePressHeld :
////                             this.testSpritePressEnd; 
////                var idMod = this.mod(touchHandle.id, 8);
////                var color = 0 == idMod ? Color.red :
////                            1 == idMod ? Color.green :
////                            2 == idMod ? Color.blue :
////                            3 == idMod ? Color.white :
////                            4 == idMod ? Color.magenta :
////                            5 == idMod ? Color.yellow :
////                            6 == idMod ? Color.cyan :
////                                         Color.gray;
////                color.a = 0.5f;
////                img.color = color;
////            }
////
////
////            for (int n = useCount, cnt = this.testImages.Count; n < cnt; ++n)
////                this.SetActive(false, this.testImages[n]);
//        }

//        int mod(int x, int m) {
//            return (x%m + m)%m;
//        }
//
//        protected virtual Image OnCreateTestImage(int n)
//        {
//            var go = new GameObject(n.ToString());
//            var t = go.AddComponent<RectTransform>();
//            t.SetParent(this.testBase);
//            t.localPosition = Vector3.zero;
//            t.localScale = Vector3.one;
//            t.localRotation = Quaternion.identity;
//            t.sizeDelta = new Vector2(200, 200);
//            var img = go.AddComponent<Image>();
//            img.sprite = this.testSpritePressEnd;
//            return img;
//        }
    }
}
