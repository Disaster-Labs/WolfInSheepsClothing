// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using UnityEngine;

public class Wolf : MonoBehaviour
{ 
    // TO BE REMOVED !!!!
    // ------------
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            OnEnterForest?.Invoke(this, EventArgs.Empty);
        }
    }
    // ------------

    public event EventHandler OnEnterForest;
    private WolfInput wolfInput;

    public void SetBeingChased(bool beingChased) {
        alertSheep.CanAlertSheep(beingChased);
    }

    // Handles eating and herding sheep

    public LayerMask sheepLayerMask;
    public LayerMask sheepFoodLayerMask;
    [NonSerialized] public GameObject followingSheep;
    public HoldingFood holdingFood = new HoldingFood();
    public NotHoldingFood notHoldingFood = new NotHoldingFood();

    [SerializeField] private AlertSheep alertSheep;
    [SerializeField] private GameObject sheepFoodPrefab;
    [SerializeField] private Transform sheepFoodParent;

    private WolfState wolfState;

    private void Start() {
        wolfInput = GetComponent<WolfInput>();
        wolfInput.OnInteract += OnInteract;
        ChangeState(notHoldingFood);
    }

    private void OnInteract(object sender, EventArgs e) {
        wolfState.OnInteract();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        wolfState.OnCollisionEnter(col);
    }

    private void OnTriggerExit2D(Collider2D col) {
        wolfState.OnCollisionExit(col);
    }

    public void ChangeState(WolfState wolfState) {
        if (this.wolfState != null) this.wolfState.OnExit();
        this.wolfState = wolfState;
        this.wolfState.OnEnter(this);
    }

    public void HaveSheepFollow() { alertSheep.SheepCanFollow(); }

    public void SpawnFood() { Instantiate(sheepFoodPrefab, transform.position, Quaternion.identity, sheepFoodParent); }

    public void StopSheepFollowing() { alertSheep.StopSheepFollowing(followingSheep); }
}

public interface WolfState {
    public void OnEnter(Wolf wolf);
    public void OnInteract();
    public void OnCollisionEnter(Collider2D col);
    public void OnCollisionExit(Collider2D col);
    public void OnExit();
}

public class HoldingFood : WolfState {
    private Wolf wolf;
    private bool first = true;

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
    }

    public void OnInteract() {
        wolf.StopSheepFollowing();
        wolf.SpawnFood();
        wolf.ChangeState(wolf.notHoldingFood);
    }

    public void OnCollisionEnter(Collider2D col) {
        if (!first) return;
        if (wolf.sheepLayerMask == (wolf.sheepLayerMask | (1 << col.gameObject.layer))) {
            wolf.HaveSheepFollow();
            wolf.followingSheep = col.gameObject;
            first = false;
        }
    }
    public void OnCollisionExit(Collider2D col) {}

    public void OnExit() {}
}

public class NotHoldingFood : WolfState {
    private Wolf wolf;
    private GameObject eatenSheep;
    private GameObject sheepFood;

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
    }

    public void OnInteract() {
        if (eatenSheep != null) eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
        else if (sheepFood != null) {
            UnityEngine.Object.Destroy(sheepFood);
            wolf.ChangeState(wolf.holdingFood);
        }
    }

    public void OnCollisionEnter(Collider2D col) {
        if (wolf.sheepLayerMask == (wolf.sheepLayerMask | (1 << col.gameObject.layer))) {
            eatenSheep = col.gameObject;
        } 
        
        if (wolf.sheepFoodLayerMask == (wolf.sheepFoodLayerMask | (1 << col.gameObject.layer))) {
            sheepFood = col.gameObject;
        }
    }

    public void OnCollisionExit(Collider2D col) {
        if (wolf.sheepLayerMask == (wolf.sheepLayerMask | (1 << col.gameObject.layer))) {
            eatenSheep = null;
        }  
        
        if (wolf.sheepFoodLayerMask == (wolf.sheepFoodLayerMask | (1 << col.gameObject.layer))) {
            sheepFood = null;
        }
    }

    public void OnExit() {}
}
