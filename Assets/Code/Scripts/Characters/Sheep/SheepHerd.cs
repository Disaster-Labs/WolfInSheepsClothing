// ---------------------------------------
// Creation Date: 4/2/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Sheep {
    public SheepState sheepState;
    public GameObject gameObject;
    public bool inHerd;
}

public class SheepHerd : MonoBehaviour
{
    public GridGraph gridGraph;
    public bool fleeingGraphGenerated = false;

    [SerializeField] public AstarPath astar; 
    [SerializeField] private GameObject sheepPrefab;
    public Sheep[] sheeps;
    private int maxSheepCount = 10;
    private int minSheepCount = 3;
    private float deadSheep = 0;

    private void Start() {
        SpawnSheeps();
    }

    public void UpdateGraph(Vector3 dimensions, bool generateFleeingGraph) {
        if (gridGraph != null) astar.data.RemoveGraph(gridGraph);
        gridGraph = astar.data.AddGraph(typeof(GridGraph)) as GridGraph;
        gridGraph.SetDimensions((int) dimensions.x, (int) dimensions.y, dimensions.z);
        gridGraph.center = transform.localPosition;
        gridGraph.is2D = true;
        gridGraph.collision.use2D = true;
        AstarPath.active.Scan();

        if (generateFleeingGraph) {
            fleeingGraphGenerated = true;
        }
    }

    private void SpawnSheeps() {
        sheeps = new Sheep[Random.Range(minSheepCount, maxSheepCount)];

        for (int i = 0; i < sheeps.Length; i++) sheeps[i] = new Sheep();
        
        for(int i = 0; i < sheeps.Length; i++) {
            float randomStartX = Random.Range(-6f, 6f);
            float randomStartY = Random.Range(-6f, 6f);
            sheeps[i].gameObject = Instantiate(sheepPrefab, transform);
            sheeps[i].gameObject.transform.localPosition = new Vector2(randomStartX, randomStartY);
            sheeps[i].gameObject.GetComponent<SpriteRenderer>().sortingOrder = i;
            sheeps[i].inHerd = true;
            ChangeState(sheeps[i], new Grazing());
        }
    }

    private void Update() {
        foreach (Sheep sheep in sheeps) {
            sheep.sheepState.OnUpdate();
        }
    }

    public void EatSheep(GameObject eatenSheep) {
        foreach (Sheep sheep in sheeps) {
            if (sheep.gameObject == eatenSheep) {
                deadSheep++;
                ChangeState(sheep, new Dead());
            }
        }

        if (deadSheep == sheeps.Length) {
            astar.data.RemoveGraph(gridGraph);
            Destroy(gameObject);
        }
    }

    public void ChangeStateByGameObject(GameObject sheep, SheepState sheepState) {
        foreach(Sheep herdSheep in sheeps) {
            if (herdSheep.gameObject == sheep) {
                ChangeState(herdSheep, sheepState);
                break;
            }
        }
    }

    public void SheepFlee() {
        foreach(Sheep herdSheep in sheeps) {
            if (herdSheep.sheepState.GetType() != typeof(Dead)) {
                ChangeState(herdSheep, new Fleeing());
            }
        }
    }

    public void ChangeState(Sheep sheep, SheepState sheepState) {
        if (sheep.sheepState != null) sheep.sheepState.OnExit();
        sheep.sheepState = sheepState;
        sheep.sheepState.OnEnter(this, sheep);
    }

    public bool IsAStray() {
        foreach (Sheep sheep in sheeps) {
            if (!sheep.inHerd) return true;
        } 

        return false;
    }
}

public interface SheepState {
    public void OnEnter(SheepHerd herd, Sheep sheep);
    public void OnUpdate();
    public void OnExit();
}

