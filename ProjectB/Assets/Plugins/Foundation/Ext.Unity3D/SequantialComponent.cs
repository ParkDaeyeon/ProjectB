using UnityEngine;
using System;
using System.Collections.Generic;
using Ext.String;

namespace Ext.Unity3D
{
    public abstract class SequentialComponent : ManagedComponent
    {
#if UNITY_EDITOR
        [Serializable]
        protected abstract class SequentialSettingInfo
        {
            public abstract string GetTag();
            public abstract Type GetAssetType();
            public abstract bool IsUse
            {
                set;
                get;
            }
            public abstract UnityEngine.Object Folder
            {
                set;
                get;
            }
            public virtual string NamePrefix
            {
                set { }
                get { return null; }
            }
            public virtual string NameExt
            {
                set { }
                get { return null; }
            }
            public virtual int StartIdx
            {
                set { }
                get { return 0; }
            }
            public virtual int ZeroFilled
            {
                set { }
                get { return 0; }
            }
            public virtual string[] Names
            {
                set { }
                get { return null; }
            }
            public virtual bool Recursive
            {
                set { }
                get { return false; }
            }

            public enum LoadBy
            {
                DecimalIndex,
                HexadecimalIndex,
                Names,
                Extension,
                Type,
            }
            
            public abstract LoadBy Load
            {
                set;
                get;
            }
            
            public override string ToString()
            {
                return string.Format("{{TAG:{0}, TYPE:{1}, U:{2}, FOLDER:{3}, PREFIX:{4}, EXT:{5}, START:{6}, ZERO_FILL:{7}, LOAD_BY:{8}}}",
                                     this.GetTag(), this.GetAssetType(), this.IsUse, this.Folder, this.NamePrefix, this.NameExt, this.StartIdx, this.ZeroFilled, this.Load);
            }
        }

        [Serializable]
        protected abstract class SequentialSettingBasicInfo : SequentialSettingInfo
        {
            [SerializeField]
            bool isUse;
            public override bool IsUse
            {
                set { this.isUse = value; }
                get { return this.isUse; }
            }

            [SerializeField]
            UnityEngine.Object folder;
            public override UnityEngine.Object Folder
            {
                set { this.folder = value; }
                get { return this.folder; }
            }

            [SerializeField]
            string namePrefix;
            public override string NamePrefix
            {
                set { this.namePrefix = value; }
                get { return this.namePrefix; }
            }

            [SerializeField]
            string nameExt;
            public override string NameExt
            {
                set { this.nameExt = value; }
                get { return this.nameExt; }
            }

            // NOTE: optional
            [SerializeField]
            int startIdx = 0;
            public override int StartIdx
            {
                set { this.startIdx = value; }
                get { return this.startIdx; }
            }

            [SerializeField]
            int zeroFilled = 0;
            public override int ZeroFilled
            {
                set { this.zeroFilled = value; }
                get { return this.zeroFilled; }
            }

            // TODO: fileNames --> names
            [SerializeField]
            string[] fileNames = null;
            public override string[] Names
            {
                set { this.fileNames = value; }
                get { return this.fileNames; }
            }

            [SerializeField]
            bool recursive = false;
            public override bool Recursive
            {
                set { this.recursive = value; }
                get { return this.recursive; }
            }
            
            [SerializeField]
            LoadBy loadBy = LoadBy.DecimalIndex;
            public override LoadBy Load
            {
                set { this.loadBy = value; }
                get { return this.loadBy; }
            }
        }
        protected abstract SequentialSettingInfo[] EditorGetSequentialSettings();

        protected override void OnEditorSetting()
        {
            base.OnEditorSetting();

            var dict = default(SortedDictionary<string, UnityEngine.Object>);
            foreach (SequentialSettingInfo si in this.EditorGetSequentialSettings())
            {
                if (null == si)
                    continue;

                if (si.IsUse && si.Folder)
                {
                    if (null == dict)
                        dict = new SortedDictionary<string, UnityEngine.Object>(new UnityExtension.StringComparer());
                    
                    switch (si.Load)
                    {
                    case SequentialSettingInfo.LoadBy.DecimalIndex:
                    case SequentialSettingInfo.LoadBy.HexadecimalIndex:
                        UnityExtension.LoadAssetAtPathsByFilter
                        (
                            type: si.GetAssetType(),
                            dirObj: si.Folder,
                            dict: dict,
                            filter: new UnityExtension.LoadAssetFilter
                            {
                                iterators = new UnityExtension.LoadAssetFilter.Iterator[]
                                {
                                    new UnityExtension.LoadAssetFilter.Iterator
                                    {
                                        prefix = si.NamePrefix,
                                        ext = si.NameExt,
                                        start = si.StartIdx,
                                        zeroFilled = si.ZeroFilled,
                                        hex = SequentialSettingInfo.LoadBy.HexadecimalIndex == si.Load,
                                    },
                                },
                                recursive = si.Recursive,
                            }
                        );
                        break;

                    case SequentialSettingInfo.LoadBy.Names:
                        UnityExtension.LoadAssetAtPathsByFilter
                        (
                            type: si.GetAssetType(),
                            dirObj: si.Folder,
                            dict: dict,
                            filter: new UnityExtension.LoadAssetFilter
                            {
                                names = si.Names,
                                recursive = si.Recursive,
                            }
                        );
                        break;

                    case SequentialSettingInfo.LoadBy.Extension:
                        UnityExtension.LoadAssetAtPathsByFilter
                        (
                            type: si.GetAssetType(),
                            dirObj: si.Folder,
                            dict: dict,
                            filter: new UnityExtension.LoadAssetFilter
                            {
                                bruteForce = true,
                                // TODO: StringCond StringCond.FromString(inlineCond);
                                conds = new StringCond[] { new StringCond
                                {
                                    chunks = new StringCond.Chunk[] { new StringCond.Chunk
                                    {
                                        keywords = new StringCond.Keyword[] { new StringCond.Keyword
                                        {
                                            value = si.NameExt,
                                            cond = StringCond.Keyword.Cond.Suffix,
                                        }}
                                    }}
                                }},
                                recursive = si.Recursive,
                            }
                        );
                        break;

                    case SequentialSettingInfo.LoadBy.Type:
                        UnityExtension.LoadAssetAtPathsByFilter
                        (
                            type: si.GetAssetType(),
                            dirObj: si.Folder,
                            dict: dict,
                            filter: new UnityExtension.LoadAssetFilter
                            {
                                bruteForce = true,
                                recursive = si.Recursive,
                            }
                        );
                        break;
                    }
                }
                
                if (null != dict && 0 < dict.Count)
                    this.OnEditorSetting_Sequential(si.GetTag(), new List<UnityEngine.Object>(dict.Values).ToArray());
            }
        }

        protected abstract void OnEditorSetting_Sequential(string tag, UnityEngine.Object[] objects);
#endif// UNITY_EDITOR
    }
}
