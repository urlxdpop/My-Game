using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{

    public static ActiveWeapon Instance { get; private set; }

    [SerializeField] private Sword sword;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        IsRotate();
    }

    public Sword GetActiveWeapon() {
        return sword;
    }

    private void IsRotate()
    {
        transform.rotation = Quaternion.Euler(0, Player.Instance.IsFlip() ? 180 : 0, 0);
    }
}
