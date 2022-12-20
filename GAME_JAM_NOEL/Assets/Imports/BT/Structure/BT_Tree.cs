using System.Threading;
using UnityEngine;

public class BT_Tree : MonoBehaviour
{
    private BT_Root _baseRoot;
    
    [SerializeField] private BT_Root _root;

    [SerializeField] private bool isRunning;
    [SerializeField] private float _timeSinceLastTick;
    [SerializeField] private float _timeBetweenTicks;

    private void Update()
    {
        if(!isRunning)
            return;
        
        _timeSinceLastTick += Time.deltaTime;

        if (_timeSinceLastTick < _timeBetweenTicks)
            return;
        
        Tick();
        _timeSinceLastTick = 0f;
    }

    private void Tick()
    {
        _root.Execute(new CancellationToken());
    }

    public void StartBehaviour()
    {
        isRunning = true;
    }

    public void SetupTree()
    {
        BT_BlackBoard blackBoard = BT_BlackBoard.Clone();
        _root = _root.Clone(blackBoard) as BT_Root;
        blackBoard.SetObject("GameObject", gameObject);
        _root.Init();
    }
}