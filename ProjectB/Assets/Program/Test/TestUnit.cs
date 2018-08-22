using UnityEngine;
using Program.View.Content.Ingame;
using System.Collections.Generic;
using Program.View.Content.Ingame.IngameObject;

public class TestUnit : MonoBehaviour
{
    [SerializeField]
    RectTransform spawnTr1;

    [SerializeField]
    RectTransform spawnTr2;
    
    [SerializeField]
    UnitFactory.UnitSpawnData[] datas1;
    [SerializeField]
    UnitFactory.UnitSpawnData[] datas2;

    [SerializeField]
    GamePlayer player;

    void Awake ()
    {
        for (int n = 0, cnt = this.datas1.Length; n < cnt; ++n)
        {
            var data = this.datas1[n];
            var rt = this.spawnTr1;

            data.direction = 1;
            data.position = rt.anchoredPosition;
            data.time = n * 4;
        }
        for (int n = 0, cnt = this.datas2.Length; n < cnt; ++n)
        {
            var data = this.datas2[n];

            var rt = this.spawnTr2;

            data.direction = -1;
            data.position = rt.anchoredPosition;
            data.time = n * 4;
        }

        this.player.StartFactory(0, datas1);
        this.player.StartFactory(1, datas2);
    }
}
