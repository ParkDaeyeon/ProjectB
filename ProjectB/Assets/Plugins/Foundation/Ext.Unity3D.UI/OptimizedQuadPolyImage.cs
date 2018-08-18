using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D;

namespace Ext.Unity3D.UI
{
    [ExecuteInEditMode]
    public class OptimizedQuadPolyImage : OptimizedImage
    {
        [SerializeField]
        bool fixToAffineUV = false;
        public bool FixToAffineUV
        {
            set
            {
                if (this.fixToAffineUV == value)
                    return;

                this.fixToAffineUV = value;
                this.SetAllDirty();
            }
            get { return this.fixToAffineUV; }
        }

        [Serializable]
        public struct Vertex
        {
            public Vector2 position;
            public Color32 color;
        }

        [Serializable]
        public class Quad
        {
            public Vertex LT;// V0
            public Vertex RT;// V1
            public Vertex RB;// V2
            public Vertex LB;// V3

            public Vertex this[int index]
            {
                get
                {
                    if (0 == index)
                        return this.LT;
                    else if (1 == index)
                        return this.RT;
                    else if (2 == index)
                        return this.RB;
                    else if (3 == index)
                        return this.LB;
                    throw new Exception("OTM_QUADPOLY_EXCEPT:INVALID_INDEX:" + index);
                }
            }

            public void SetColorAll(Color color)
            {
                this.LT.color = this.RT.color = this.RB.color = this.LB.color = color;
            }
        }
        [SerializeField]
        Quad quad;
        public Quad GetQuad()
        {
            return this.quad;
        }
        public void AssignQuad(OptimizedQuadPolyImage rhs)
        {
            this.quad.LT = rhs.quad.LT;
            this.quad.RT = rhs.quad.RT;
            this.quad.RB = rhs.quad.RB;
            this.quad.LB = rhs.quad.LB;
            this.SetVerticesDirty();
        }

        [SerializeField]
        Vector2 uvOffset = Vector2.zero;
        public Vector2 UVOffset
        {
            set
            {
                if (value == this.uvOffset)
                    return;

                this.uvOffset = value;
                this.SetVerticesDirty();
            }
            get { return this.uvOffset; }
        }

        [SerializeField]
        Vector2 uvScale = Vector2.one;
        public Vector2 UVScale
        {
            set
            {
                if (value == this.uvScale)
                    return;

                this.uvScale = value;
                this.SetVerticesDirty();
            }
            get { return this.uvScale; }
        }

        public override bool Cacheable
        {
            get { return false; }
        }


        UIVertex[] vbo = new UIVertex[4];
        public Vector2 ToLocalUV(Vector2 uv)
        {
            return this.uvOffset + new Vector2(uv.x * this.uvScale.x, uv.y * this.uvScale.y);
        }
        
        protected override void OnPopulateMeshMainProcess(Translator translator,
                                                          RectTransform rectTrans,
                                                          Sprite sprite,
                                                          VertexHelper toFill)
        {
            var baseColor = this.color;

            toFill.Clear();

            var uv = sprite.GetUVRect();

            var quad = this.quad;
            for (int n = 0; n < 4; ++n)
            {
                var vert = quad[n];
                var uivert = UIVertex.simpleVert;
                uivert.position = vert.position;
                switch (n)
                {
                case 0: uivert.uv0 = this.ToLocalUV(new Vector2(uv.xMin, uv.yMax)); break;
                case 1: uivert.uv0 = this.ToLocalUV(new Vector2(uv.xMax, uv.yMax)); break;
                case 2: uivert.uv0 = this.ToLocalUV(new Vector2(uv.xMax, uv.yMin)); break;
                case 3: uivert.uv0 = this.ToLocalUV(new Vector2(uv.xMin, uv.yMin)); break;
                }
                uivert.uv1 = Vector2.one;
                uivert.color = vert.color * baseColor;

                this.vbo[n] = uivert;
            }

            if (this.fixToAffineUV)
            {
                var p0 = quad.LT.position;
                var p1 = quad.RT.position;
                var p2 = quad.RB.position;
                var p3 = quad.LB.position;

                float ax = p2.x - p0.x;
                float ay = p2.y - p0.y;
                float bx = p3.x - p1.x;
                float by = p3.y - p1.y;

                float cross = ax * by - ay * bx;
                if (cross != 0)
                {
                    float cx = p0.x - p1.x;
                    float cy = p0.y - p1.y;

                    float s = (ax * cy - ay * cx) / cross;
                    if (0 < s && s < 1)
                    {
                        float q1 = 1 / (1 - s);
                        float q3 = 1 / s;

                        this.ReplaceUV(q1, 1);
                        this.ReplaceUV(q3, 3);

                        float t = (bx * cy - by * cx) / cross;
                        if (t > 0 && t < 1)
                        {
                            float q0 = 1 / (1 - t);
                            float q2 = 1 / t;

                            this.ReplaceUV(q0, 0);
                            this.ReplaceUV(q2, 2);
                        }
                    }
                }
            }

            toFill.AddUIVertexQuad(this.vbo);
        }

        void ReplaceUV(float q, int index)
        {
            var vbo = this.vbo[index];
            vbo.uv0 *= q;
            vbo.uv1 = new Vector2(q, 0);
            this.vbo[index] = vbo;
        }

#if UNITY_EDITOR
        [SerializeField]
        bool editorDrawGizmo;
        void OnDrawGizmos()
        {
            if (!this.editorDrawGizmo)
                return;

            var matPrev = Gizmos.matrix;
            Gizmos.matrix = this.transform.localToWorldMatrix;
            var colorPrev = Gizmos.color;
            Gizmos.color = Color.red;

            Gizmos.DrawLine(this.quad.LT.position, this.quad.RT.position);
            Gizmos.DrawLine(this.quad.RT.position, this.quad.RB.position);
            Gizmos.DrawLine(this.quad.RB.position, this.quad.LB.position);
            Gizmos.DrawLine(this.quad.LB.position, this.quad.LT.position);

            Gizmos.matrix = matPrev;
            Gizmos.color = colorPrev;
        }
#endif// UNITY_EDITOR
    }
}
