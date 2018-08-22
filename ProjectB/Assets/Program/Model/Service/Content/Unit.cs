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
    using UnitAssets = MVC.Model<long, UnitData>;
    public sealed class Unit : ManagedSingleton<Unit._Singleton>, IDisposable
    {
        Unit(UnitData assetModel)
        {
            this.assetModel = assetModel;
        }

        public void Dispose()
        {
        }

        UnitData assetModel;
        [JsonIgnore]
        public UnitData UnsafeRawAccess
        {
            get { return this.assetModel; }
        }
        public long Code
        {
            get { return this.assetModel.code; }
        }
        public float Hp
        {
            get { return this.assetModel.hp; }
        }
        public float MoveSpeed
        {
            get { return this.assetModel.moveSpeed; }
        }
        public float Damage
        {
            get { return this.assetModel.damage; }
        }
        public float Range
        {
            get { return this.assetModel.range; }
        }
        public float AttackSpeed
        {
            get { return this.assetModel.attackSpeed; }
        }
        public short Order
        {
            get { return this.assetModel.order; }
        }

        public static Unit GetUnit(long code)
        {
            return Unit.Singleton.GetUnit(code);
        }

        public static int Count
        {
            get { return Unit.Singleton.Count; }
        }

        public static IEnumerable<Unit> Units
        {
            get { return Unit.Singleton.Units; }
        }
        public static IEnumerable<Unit> OrderedUnit
        {
            get { return Unit.Singleton.OrderedUnits; }
        }

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
                var assetMap = UnitAssets.Map;
                var map = this.map = new SortedDictionary<long, Unit>(new CodeComparer());
                var ordered = this.ordered = new List<Unit>(assetMap.Count);
                foreach (var asset in assetMap.Values)
                {
                    if (!this.VerifyModel(asset))
                        continue;

                    var unit = new Unit(asset);
                    map.Add(unit.Code, unit);
                }

                ordered.Sort((x, y) =>
                {
                    var orderX = x.Order;
                    var orderY = y.Order;
                    if (orderX < orderY)
                        return -1;
                    else if (orderX > orderY)
                        return +1;

                    var codeX = x.Code;
                    var codeY = y.Code;
                    if (codeX < codeY)
                        return -1;
                    else if (codeX > codeY)
                        return +1;

                    return 0;
                }); 
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

            bool VerifyModel(UnitData model)
            {
                if (this.map.ContainsKey(model.code))
                {
                    Debug.LogError(string.Format("UNITDATA:DUPLICATED_CODE_EXCEPT:CODE:{0},\nMODEL:{1},\nOLD_MODEL:{2}",
                                                                                       model.code,
                                                                                       model,
                                                                                       this.map[model.code]));
                    return false;
                }

                return true;
            }

            SortedDictionary<long, Unit> map;
            internal Unit GetUnit(long code)
            {
                Unit data;
                if (!this.map.TryGetValue(code, out data))
                    return null;

                return data;
            }

            internal int Count
            {
                get { return this.map.Count; }
            }

            internal IEnumerable<Unit> Units
            {
                get { return this.map.Values; }
            }


            List<Unit> ordered;
            internal Unit GetUnitAtIndex(int index)
            {
                if (index < 0 || ordered.Count <= index)
                    return null;

                return this.ordered[index];
            }
            internal IEnumerable<Unit> OrderedUnits
            {
                get { return this.ordered; }
            }
        }
    }
}
