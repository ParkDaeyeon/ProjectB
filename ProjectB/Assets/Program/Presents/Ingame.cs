using Program.Core;
using Program.View;
using Program.View.Content;
using Program.View.Content.Ingame;
using Program.View.Content.Ingame.IngameObject;
using Program.View.Content.Ingame.IngameObject.UI;
using Program.View.Content.Ingame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Program.Presents
{
    public class Ingame : ImplPresent<IngameView>
    {
        public enum State
        {
            Loading,
            Intro,
            Play,
        }


        protected override void OnOpen(object openArg)
        {
            base.OnOpen(openArg);

            this.SetStateLoading();
        }
        protected override void OnClose(Type nextPresent)
        {
            this.ReleaseBodySprites();

            base.OnClose(nextPresent);
        }

        void SetStateLoading()
        {
            this.SetState(State.Loading);

            this.InitBodySprites();
            this.OpenManagers();

            this.SetStateIntro();
        }

        void SetStateIntro()
        {
            this.SetState(State.Intro);

            Fade.In(0.5f, () =>
            {
                this.SetStatePlay();
            });
        }

        void SetStatePlay()
        {
            this.SetState(State.Play);

            this.SpawnUnits();
        }

        List<UnitFactory.UnitSpawnData> spawnData1;
        List<UnitFactory.UnitSpawnData> spawnData2;
        void SpawnUnits()
        {
            var view = this.View;
            var player = view.GamePlayer;

            {
                var factory = player.GetFactoryAt(0);
                var rt = factory.transform as RectTransform;
                var spawnPos = rt.anchoredPosition;
                this.spawnData1 = new List<UnitFactory.UnitSpawnData>();
                this.spawnData1.Add(new UnitFactory.UnitSpawnData
                {
                    direction = 1,
                    position = spawnPos,
                    time = 1f,
                    unitDatas = new Unit.UnitData[]
                    {
                        new Unit.UnitData
                        {
                            hp = 10,
                            aSpeed = 10,
                            skillKinds = new Program.View.Content.Ingame.Skill.SkillKind[]
                            {
                                Program.View.Content.Ingame.Skill.SkillKind.Hit
                            },
                            mSpeed = 10,
                            code = 0,
                            damage = 0,
                            knockbackPower= 0,
                            range = 0,
                            teamId = 0,
                        }
                    }
                });
            }

            {
                var factory = player.GetFactoryAt(1);
                var rt = factory.transform as RectTransform;
                var spawnPos = rt.anchoredPosition;
                this.spawnData2 = new List<UnitFactory.UnitSpawnData>();
                this.spawnData2.Add(new UnitFactory.UnitSpawnData
                {
                    direction = -1,
                    position = spawnPos,
                    time = 1f,
                    unitDatas = new Unit.UnitData[]
                    {
                        new Unit.UnitData
                        {
                            hp = 10,
                            aSpeed = 10,
                            skillKinds = new Program.View.Content.Ingame.Skill.SkillKind[]
                            {
                                Program.View.Content.Ingame.Skill.SkillKind.Hit
                            },
                            mSpeed = 10,
                            code = 0,
                            damage = 0,
                            knockbackPower = 0,
                            range = 0,
                            teamId = 1,
                        }
                    }
                });
            }
            player.StartFactory(0, spawnData1);
            player.StartFactory(1, spawnData2);
        }


        Dictionary<long, UnitBody.UnitBodySprites> bodySprites;
        void InitBodySprites()
        {
            this.bodySprites = new Dictionary<long, UnitBody.UnitBodySprites>();

            var units = Program.Model.Service.Content.Unit.Units;
            var e = units.GetEnumerator();
            while(e.MoveNext())
            {
                var unit = e.Current;
                var code = unit.Code;
                this.bodySprites.Add(unit.Code, new UnitBody.UnitBodySprites
                {
                    head = Resources.Load<Sprite>("unit_h_" + code),
                    body = Resources.Load<Sprite>("unit_b_" + code),
                    leftArm = Resources.Load<Sprite>("unit_l_" + code),
                    rightArm = Resources.Load<Sprite>("unit_r_" + code),
                });
            }
        }
        void ReleaseBodySprites()
        {
            if (null != this.bodySprites)
                this.bodySprites.Clear();

            this.bodySprites = null;
        }

        void OpenManagers()
        {
            var view = this.View;
            var player = view.GamePlayer;
            IngameObjectManager.Open(player.UnitsPool, player.ProjectilesPool);
            IngameObjectUIManager.Open();
            UnitSpritesManager.Open(bodySprites);
        }

        void CloseManagers()
        {
            UnitSpritesManager.Close();
            IngameObjectUIManager.Close();
            IngameObjectManager.Close();
        }
    }
}
