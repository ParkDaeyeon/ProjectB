using UnityEngine;
using Program.View.Content.Ingame;

public class TestUnit : MonoBehaviour
{
    [SerializeField]
    RectTransform spawnTr1;

    [SerializeField]
    RectTransform spawnTr2;

    [SerializeField]
    UnitFactory factory1;

    [SerializeField]
    UnitFactory factory2;

    [SerializeField]
    UnitFactory.UnitSpawnData[] datas1;
    [SerializeField]
    UnitFactory.UnitSpawnData[] datas2;

    [SerializeField]
    GamePlayer player;

    void Awake ()
    {
        this.player.Open();

        for (int n = 0, cnt = this.datas1.Length; n < cnt; ++n)
        {
            var data = this.datas1[n];

            var rt = this.factory1.CachedTransform as RectTransform;

            data.direction = 1;
            data.position = rt.anchoredPosition;
            data.time = n * 4;
        }
        for (int n = 0, cnt = this.datas2.Length; n < cnt; ++n)
        {
            var data = this.datas2[n];

            var rt = this.factory2.CachedTransform as RectTransform;

            data.direction = -1;
            data.position = rt.anchoredPosition;
            data.time = n * 4;
        }

        this.StartCoroutine(this.factory1.StartSpawn(this.datas1));
        this.StartCoroutine(this.factory2.StartSpawn(this.datas2));
    }
    private void OnDestroy()
    {
        this.player.Close();
    }
}
