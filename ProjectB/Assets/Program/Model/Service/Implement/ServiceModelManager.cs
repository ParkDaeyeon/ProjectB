using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Ext;
using Ext.Unity3D;
using Ext.String;
using Ext.Event;
using Ext.Collection.AntiGC;
using Ext.Async;

using ReadWriteCsv;

using Program.Core;
using Program.Model.Domain.Asset;
using Program.Model.Domain.Asset.Base;
using Program.Model.Service.Content;
using Program.Model.Domain.Code;
using Program.Model.Domain.Type;
using Program.Presents.Helper;

namespace Program.Model.Service.Implement
{
    public sealed class ServiceModelManager : ManagedSingleton<ServiceModelManager._Singleton>
    {
        ServiceModelManager() { }

        public static IEnumerator OpenSheetTask(Asset.SubTaskHandle sth)
        {
            return ServiceModelManager.Singleton.OpenSheetTask(sth);
        }

        public static IEnumerator[] OpenServiceTask(Asset.SubTaskHandle sth)
        {
            return ServiceModelManager.Singleton.OpenServiceTask(sth);
        }

        static K GetAssetCode<K, V>(V assetData) where V : AssetData<K>
        {
            return assetData.code;
        }
        

        public class _Singleton : ISingleton
        {
            public void OpenSingleton()
            {
            }

            public void CloseSingleton()
            {
                ManagedSingletonDisposer.Run();

                // TODO: 수기로 추가한 놈들 Release
            }


            delegate void ParseFunc(SheetInfo si, TextAsset source);
            delegate object BuildFunc(SheetInfo si, object data);
            class SheetInfo
            {
                internal string sheetname;
                internal BuildFunc buildFunc;
                internal ParseFunc parseFunc;
            }

            SheetInfo CreateSheetInfo<K, V>(BuildFunc buildFunc, ParseFunc parseFunc = null) where V : AssetData<K>
            {
                var si = new SheetInfo();
                si.sheetname = typeof(V).Name;
                si.buildFunc = buildFunc;
                si.parseFunc = null != parseFunc ? parseFunc : this.ParseFromCsv<K, V>;
                return si;
            }

            internal IEnumerator OpenSheetTask(Asset.SubTaskHandle sth)
            {
                var signal = sth.Signal;
                if (CancellableSignal.IsCancelled(signal)) { yield break; }
                
                var sheetInfos = new SheetInfo[]
                {
                    //this.CreateSheetInfo<long, GameConfigData   >(this.OnOpenSheets_GameConfigData      ),
                    //this.CreateSheetInfo<long, RegionData       >(this.OnOpenSheets_RegionData          ),
                    //this.CreateSheetInfo<long, ComposerData     >(this.OnOpenSheets_ComposerData        ),
                    //this.CreateSheetInfo<long, MusicData        >(this.OnOpenSheets_MusicData           ),
                    //this.CreateSheetInfo<long, BgmData          >(this.OnOpenSheets_BgmData             ),
                    //this.CreateSheetInfo<long, MatineeData      >(this.OnOpenSheets_MatineeData         ),
                    //this.CreateSheetInfo<long, StarData         >(this.OnOpenSheets_StarData            ),
                    //this.CreateSheetInfo<long, JudgementData    >(this.OnOpenSheets_JudgementData       ),
                    //this.CreateSheetInfo<long, Concours4KeyData >(this.OnOpenSheets_Concours4KeyData    ),
                    //this.CreateSheetInfo<long, Concours6KeyData >(this.OnOpenSheets_Concours6KeyData    ),
                    //this.CreateSheetInfo<long, PianoData        >(this.OnOpenSheets_PianoData           ),
                };

                var progressStride = 1f / sheetInfos.Length;
                var progressStrideHalf = progressStride * 0.5f;
                var progress = 0f;
                for (int n = 0, cnt = sheetInfos.Length; n < cnt; ++n)
                {
                    var si = sheetInfos[n];
                    var source = Resources.Load<TextAsset>(string.Format("Data/{0}", si.sheetname));
                    if (!source)
                    {
                        Debug.LogError(string.Format("SERVICEMODEL:SHEET_NOT_FOUND_ERROR:{0}", si.sheetname));
                    }
                    sth.SetProgress(progress + progressStrideHalf);
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal)) { yield break; }

                    si.parseFunc(si, source);
                    sth.SetProgress(progress += progressStride);
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal)) { yield break; }
                }

                sth.Complete();
            }

            void ParseFromCsv<K, V>(SheetInfo si, TextAsset source) where V : AssetData<K>
            {
                using (var reader = new CsvFileReader(new MemoryStream(source.bytes)))
                {
                    // NOTE: HEADER
                    reader.ReadLine();

                    var list = new List<V>(512);
                    var row = new CsvRow();
                    while (reader.ReadRow(row))
                    {
                        var elem = (V)si.buildFunc(si, row);
                        list.Add(elem);
                    }
#if LOG_ASSET
                    Debug.Log(string.Format("SERVICEMODEL:SHEET:{0}:{1}", si.sheetname, ConvertString.Execute(list)));
#endif// LOG_ASSET
                    MVC.Model<K, V>.Open(ServiceModelManager.GetAssetCode<K, V>, list);
                }
            }

            delegate T ParseT<T>(string value) where T : struct;
            T? ParseNullable<T>(string value, ParseT<T> parse) where T : struct
            {
                if (string.IsNullOrEmpty(value))
                    return null;

                return parse(value);
            }

            int OnOpenSheets_IAssetable<CodeType>(CsvRow row, IAssetable<CodeType> assetModel, Func<string, CodeType> parser, int counter)
            {
                assetModel.code = parser(row[counter++]);
                return counter;
            }

            int OnOpenSheets_IOrderedAssetable<CodeType>(CsvRow row, IOrderedAssetable<CodeType> assetModel, Func<string, CodeType> parser, int counter)
            {
                counter = this.OnOpenSheets_IAssetable(row, assetModel, parser, counter);
                assetModel.Index = int.Parse(row[counter++]);
                return counter;
            }

            //GameConfigData OnOpenSheets_GameConfigData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new GameConfigData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    ++counter;// NOTE: Comment
            //    assetModel.integerValue = this.ParseNullable(row[counter++], int.Parse);
            //    assetModel.dobuleValue = this.ParseNullable(row[counter++], double.Parse);
            //    assetModel.stringValue = row[counter++];
            //    if ("" == assetModel.stringValue)
            //        assetModel.stringValue = null;
            //    return assetModel;
            //}

            //RegionData OnOpenSheets_RegionData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new RegionData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.name = row[counter++];
            //    return assetModel;
            //}

            //ComposerData OnOpenSheets_ComposerData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new ComposerData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.name = row[counter++];
            //    assetModel.era = short.Parse(row[counter++]);
            //    assetModel.order = short.Parse(row[counter++]);
            //    assetModel.saveSlot = byte.Parse(row[counter++]);
            //    return assetModel;
            //}

            //MusicData OnOpenSheets_MusicData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new MusicData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.title = row[counter++];
            //    assetModel.composer = long.Parse(row[counter++]);
                
            //    assetModel.diff4k1 = byte.Parse(row[counter++]);
            //    assetModel.diff4k2 = byte.Parse(row[counter++]);
            //    assetModel.diff4k3 = byte.Parse(row[counter++]);

            //    assetModel.diff6k1 = byte.Parse(row[counter++]);
            //    assetModel.diff6k2 = byte.Parse(row[counter++]);
            //    assetModel.diff6k3 = byte.Parse(row[counter++]);

            //    assetModel.diff2p1 = byte.Parse(row[counter++]);
            //    assetModel.diff2p2 = byte.Parse(row[counter++]);
            //    assetModel.diff2p3 = byte.Parse(row[counter++]);

            //    assetModel.ensembleOrder = int.Parse(row[counter++]);

            //    assetModel.hidden = "1" == row[counter++];
            //    return assetModel;
            //}

            //BgmData OnOpenSheets_BgmData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new BgmData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.bgmStart = float.Parse(row[counter++]);
            //    assetModel.bgmEnd = float.Parse(row[counter++]);
            //    assetModel.bgmFI = float.Parse(row[counter++]);
            //    assetModel.bgmFO = float.Parse(row[counter++]);
            //    assetModel.bgmNextTerm = float.Parse(row[counter++]);

            //    assetModel.previewStart = float.Parse(row[counter++]);
            //    assetModel.previewEnd = float.Parse(row[counter++]);
            //    assetModel.previewFI = float.Parse(row[counter++]);
            //    assetModel.previewFO = float.Parse(row[counter++]);
            //    assetModel.previewNextTerm = float.Parse(row[counter++]);

            //    assetModel.outgame = "1" == row[counter++];
            //    return assetModel;
            //}

            //MatineeData OnOpenSheets_MatineeData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new MatineeData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.stage4k1 = short.Parse(row[counter++]);
            //    assetModel.order4k1 = short.Parse(row[counter++]);
            //    assetModel.stage4k2 = short.Parse(row[counter++]);
            //    assetModel.order4k2 = short.Parse(row[counter++]);
            //    assetModel.stage4k3 = short.Parse(row[counter++]);
            //    assetModel.order4k3 = short.Parse(row[counter++]);

            //    assetModel.stage6k1 = short.Parse(row[counter++]);
            //    assetModel.order6k1 = short.Parse(row[counter++]);
            //    assetModel.stage6k2 = short.Parse(row[counter++]);
            //    assetModel.order6k2 = short.Parse(row[counter++]);
            //    assetModel.stage6k3 = short.Parse(row[counter++]);
            //    assetModel.order6k3 = short.Parse(row[counter++]);
            //    return assetModel;
            //}

            //StarData OnOpenSheets_StarData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new StarData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.class1 = double.Parse(row[counter++]);
            //    assetModel.class2 = double.Parse(row[counter++]);
            //    assetModel.class3 = double.Parse(row[counter++]);
            //    return assetModel;
            //}

            //JudgementData OnOpenSheets_JudgementData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new JudgementData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.tier = (JudgementType)int.Parse(row[counter++]);
            //    assetModel.groupKey = row[counter++];

            //    assetModel.ratio = double.Parse(row[counter++]);

            //    assetModel.life = double.Parse(row[counter++]);

            //    assetModel.score = int.Parse(row[counter++]);
            //    assetModel.difficultBonus = int.Parse(row[counter++]);

            //    assetModel.noteF = double.Parse(row[counter++]);
            //    assetModel.noteR = double.Parse(row[counter++]);

            //    assetModel.longF = double.Parse(row[counter++]);
            //    assetModel.longR = double.Parse(row[counter++]);
            //    return assetModel;
            //}

            //int OnOpenSheets_ConcoursData(CsvRow row, ConcoursData assetModel, int counter)
            //{
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.name = row[counter++];
                
            //    assetModel.music1 = long.Parse(row[counter++]);
            //    assetModel.difficultType1 = (DifficultType)int.Parse(row[counter++]);
            //    assetModel.hidden1 = "1" == row[counter++];

            //    assetModel.music2 = long.Parse(row[counter++]);
            //    assetModel.difficultType2 = (DifficultType)int.Parse(row[counter++]);
            //    assetModel.hidden2 = "1" == row[counter++];

            //    assetModel.music3 = long.Parse(row[counter++]);
            //    assetModel.difficultType3 = (DifficultType)int.Parse(row[counter++]);
            //    assetModel.hidden3 = "1" == row[counter++];

            //    assetModel.music4 = long.Parse(row[counter++]);
            //    assetModel.difficultType4 = (DifficultType)int.Parse(row[counter++]);
            //    assetModel.hidden4 = "1" == row[counter++];

            //    assetModel.judgeGroupKey = row[counter++];

            //    assetModel.missionActive = (ActiveMissionType)int.Parse(row[counter++]);
            //    assetModel.missionValue = uint.Parse(row[counter++]);
                
            //    assetModel.missionMiss = byte.Parse(row[counter++]);
                
            //    assetModel.missionSpeed = byte.Parse(row[counter++]);
            //    assetModel.missionFade = (NoteVisibleType)int.Parse(row[counter++]);
            //    assetModel.missionSequence = (SequenceType)int.Parse(row[counter++]);

            //    {
            //        var valueL = 0L;
            //        if (long.TryParse(row[counter++], out valueL))
            //            assetModel.rewardPiano = valueL;
            //        else
            //            assetModel.rewardPiano = null;
            //    }

            //    {
            //        var valueL = 0L;
            //        if (long.TryParse(row[counter++], out valueL))
            //            assetModel.rewardMusic = valueL;
            //        else
            //            assetModel.rewardMusic = null;
            //    }
            //    return counter;
            //}

            //Concours4KeyData OnOpenSheets_Concours4KeyData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new Concours4KeyData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_ConcoursData(row, assetModel, counter);
            //    return assetModel;
            //}

            //Concours6KeyData OnOpenSheets_Concours6KeyData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new Concours6KeyData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_ConcoursData(row, assetModel, counter);
            //    return assetModel;
            //}

            //PianoData OnOpenSheets_PianoData(SheetInfo si, object data)
            //{
            //    var row = (CsvRow)data;
            //    var assetModel = new PianoData();
            //    var counter = 0;
            //    counter = this.OnOpenSheets_IAssetable(row, assetModel, long.Parse, counter);
            //    assetModel.name = row[counter++];
            //    assetModel.keyColorCode = row[counter++];
            //    assetModel.access = int.Parse(row[counter++]);
            //    assetModel.order = int.Parse(row[counter++]);
            //    return assetModel;
            //}

            internal IEnumerator[] OpenServiceTask(Asset.SubTaskHandle sth)
            {
                return new IEnumerator[]
                {
                    //this.OnOpenServiceTask(sth, Locale.Open),
                    //this.OnOpenServiceTask(sth, Config.Open),
                    //this.OnOpenServiceTask(sth, Region.Open),
                    //this.OnOpenServiceTask(sth, Composer.Open),
                    //this.OnOpenServiceTask(sth, Music.Open),
                    //this.OnOpenServiceTask(sth, Game.Open),
                    //this.OnOpenServiceTask(sth, Library.Open),
                    //this.OnOpenServiceTask(sth, Matinee.Open),
                    //this.OnOpenServiceTask(sth, Piano.Open),
                    //this.OnOpenServiceTask(sth, Concours.Open),
                    //this.OnOpenServiceTask(sth, Stage.Open),
                    //this.OnOpenServiceTask(sth, Ranking.Open),
                    //this.OnOpenServiceTask(sth, MusicInventory.Open),
                    //this.OnOpenServiceTask(sth, ManagedAtlas.Open),
                    //this.OnOpenServiceTask(sth, ManagedTexture.Open),
                    //this.OnOpenServiceTask(sth, ManagedTextureAtlas.Open),
                };
            }

            IEnumerator OnOpenServiceTask(Asset.SubTaskHandle sth,
                                          Action loadStart,
                                          Func<bool> loadDone = null)
            {
                var signal = sth.Signal;
                if (CancellableSignal.IsCancelled(signal)) { yield break; }

                try
                {
                    loadStart();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("SERVICEMODEL:WARN:OPEN_SERVICE_EXCEPT:{0}", e));
                    sth.SetFailed(e);
                    yield break;
                }

                if (null != loadDone)
                {
                    bool isDone = false;
                    do
                    {
                        try
                        {
                            isDone = loadDone();
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning(string.Format("SERVICEMODEL:WARN:OPEN_SERVICE_EXCEPT#2:{0}", e));
                            sth.SetFailed(e);
                            yield break;
                        }

                        yield return null;
                        if (CancellableSignal.IsCancelled(signal)) { yield break; }
                    }
                    while (!isDone);
                }
                else
                {
                    yield return null;
                    if (CancellableSignal.IsCancelled(signal)) { yield break; }
                }

                sth.Complete();
            }
        }
    }
}
