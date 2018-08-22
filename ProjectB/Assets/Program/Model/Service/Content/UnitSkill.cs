using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ext;
using Ext.Unity3D;
using Ext.Async;
using Ext.String;
using Ext.Event;
using Newtonsoft.Json;
using Program.Core;
using Program.Model.Domain.Asset;
using Program.Model.Domain.Type;
using Program.Model.Domain.Code;
using Program.Model.Domain.Entity;

namespace Program.Model.Service.Content
{
    using UnitSkillAssets = MVC.Model<long, UnitSkillData>;
    public sealed class UnitSkill : ManagedSingleton<UnitSkill._Singleton>, IDisposable
    {
        UnitSkill(UnitSkillData assetModel)
        {
            this.assetModel = assetModel;
        }

        public void Dispose()
        {
        }

        UnitSkillData assetModel;
        [JsonIgnore]
        public UnitSkillData UnsafeRawAccess
        {
            get { return this.assetModel; }
        }
        public long Code
        {
            get { return this.assetModel.code; }
        }
        public float CoolTime
        {
            get { return this.assetModel.coolTime; }
        }

        public static UnitSkill GetUnitSkill(long code)
        {
            return UnitSkill.Singleton.GetUnitSkill(code);
        }

        public static int Count
        {
            get { return UnitSkill.Singleton.Count; }
        }

        public static IEnumerable<UnitSkill> UnitSkills
        {
            get { return UnitSkill.Singleton.UnitSkills; }
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                var assetMap = UnitSkillAssets.Map;
                var map = this.map = new SortedDictionary<long, UnitSkill>(new CodeComparer());
                foreach (var asset in assetMap.Values)
                {
                    if (!this.VerifyModel(asset))
                        continue;

                    var unitSkill = new UnitSkill(asset);
                    map.Add(unitSkill.Code, unitSkill);
                }
#if LOG_DATA
                Debug.Log(string.Format("UNITSKILL:{0}", ConvertString.Execute(map)));
                Debug.Log(string.Format("UNITSKILL:ORDERED:{0}", ConvertString.Execute(ordered)));
#endif// LOG_DATA
            }

            public void CloseSingleton()
            {
                if (null != this.map)
                {
                    foreach (var elem in this.map.Values)
                        elem.Dispose();

                    this.map.Clear();
                    this.map = null;
                }
            }

            bool VerifyModel(UnitSkillData model)
            {
                if (this.map.ContainsKey(model.code))
                {
                    Debug.LogError(string.Format("UNITSKILL:DUPLICATED_CODE_EXCEPT:CODE:{0},\nMODEL:{1},\nOLD_MODEL:{2}",
                                                                                       model.code,
                                                                                       model,
                                                                                       this.map[model.code]));
                    return false;
                }

                return true;
            }

            SortedDictionary<long, UnitSkill> map;
            internal UnitSkill GetUnitSkill(long code)
            {
                UnitSkill data;
                if (!this.map.TryGetValue(code, out data))
                    return null;

                return data;
            }

            internal int Count
            {
                get { return this.map.Count; }
            }

            internal IEnumerable<UnitSkill> UnitSkills
            {
                get { return this.map.Values; }
            }
        }
    }
}
