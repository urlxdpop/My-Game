using UnityEngine;
using UnityEngine.UI;

public class HP : MonoBehaviour {
    [SerializeField] private Image[] lives;
    [SerializeField] private Sprite fullLive;
    [SerializeField] private Sprite emptyLive;

    private void Start() {
        VisualMaxHp();
    }

    private void Update() {
        VisualCurrentHp();
    }

    private void VisualMaxHp() {
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].enabled = i < Player.Instance.GetMaxHp();
        }
    }

    private void VisualCurrentHp() {
        for (int i = 0; i < lives.Length; i++)
        {
            lives[i].sprite = i < Player.Instance.GetHp() ? fullLive : emptyLive;
        }
    }
}
