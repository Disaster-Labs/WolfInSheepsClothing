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
    public GridGraph graph;
    [System.NonSerialized] public bool fleeingGraphGenerated = false;

    [SerializeField] public AstarPath astar; 
    [SerializeField] private GameObject sheepPrefab;
    [SerializeField] private bool isLoneSheep;
    public Sheep[] sheeps;
    private int maxSheepCount = 10;
    private int minSheepCount = 3;
    private float deadSheep = 0;

    private void Start() {
        graph = astar.data.graphs[0] as GridGraph;
        SpawnSheeps();
    }

    private void SpawnSheeps() {
        if (isLoneSheep) {
            sheeps = new Sheep[1];
        } else {
            sheeps = new Sheep[Random.Range(minSheepCount, maxSheepCount)];
        }

        for (int i = 0; i < sheeps.Length; i++) sheeps[i] = new Sheep();
        
        for(int i = 0; i < sheeps.Length; i++) {
            float randomStartX = Random.Range(-6f, 6f);
            float randomStartY = Random.Range(-6f, 6f);
            sheeps[i].gameObject = Instantiate(sheepPrefab, transform);
            sheeps[i].gameObject.transform.localPosition = new Vector2(randomStartX, randomStartY);
            sheeps[i].inHerd = !isLoneSheep;
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
            if (herdSheep.inHerd && herdSheep.sheepState.GetType() == typeof(Grazing)) {
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

