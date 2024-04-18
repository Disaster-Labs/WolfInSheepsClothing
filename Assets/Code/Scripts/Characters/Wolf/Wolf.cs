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

    // Handles eating and herding sheep
    private bool beingChased = false;
    public void SetBeingChased(bool beingChased) {
        alertSheep.CanAlertSheep(beingChased);
        this.beingChased = beingChased;
    }

    public LayerMask sheepLayerMask;
    public LayerMask sheepFoodLayerMask;
    public HoldingFood holdingFood = new HoldingFood();
    public NotHoldingFood notHoldingFood = new NotHoldingFood();

    [SerializeField] private AlertSheep alertSheep;
    [SerializeField] private GameObject sheepFoodPrefab;
    [SerializeField] private Transform sheepFoodParent;
    [SerializeField] private LayerMask hideObjectLayerMask;

    private WolfState wolfState;

    private bool isHiding = false;
    public bool GetIsHiding() { return isHiding; }

    private void Start() {
        wolfInput = GetComponent<WolfInput>();
        wolfInput.OnInteract += OnInteract;
        ChangeState(notHoldingFood);
    }

    private void OnInteract(object sender, EventArgs e) {
        wolfState.OnInteract();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (hideObjectLayerMask == (hideObjectLayerMask | (1 << col.gameObject.layer))) {
            isHiding = !beingChased;
        }

        wolfState.OnCollisionEnter(col);
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (hideObjectLayerMask == (hideObjectLayerMask | (1 << col.gameObject.layer))) {
            isHiding = false;
        }

        wolfState.OnCollisionExit(col);
    }

    public void ChangeState(WolfState wolfState) {
        if (this.wolfState != null) this.wolfState.OnExit();
        this.wolfState = wolfState;
        wolfState.OnEnter(this);
    }

    public void HaveSheepFollow() { alertSheep.SheepCanFollow(); }

    public void SpawnFood() { Instantiate(sheepFoodPrefab, transform.position, Quaternion.identity, sheepFoodParent); }

    public void StopSheepFollowing() { alertSheep.StopSheepFollowing(); }
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

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
        wolf.HaveSheepFollow();
    }

    public void OnInteract() {
        wolf.StopSheepFollowing();
        wolf.SpawnFood();
        wolf.ChangeState(wolf.notHoldingFood);
    }

    public void OnCollisionEnter(Collider2D col) {}

    public void OnCollisionExit(Collider2D col) {}

    public void OnExit() {
    }
}

public class NotHoldingFood : WolfState {
    private Wolf wolf;
    private GameObject eatenSheep;
    private GameObject sheepFood;

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
    }

    public void OnInteract() {
        if (eatenSheep != null) {
            eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
        } else if (sheepFood != null) {
            SheepFood food = sheepFood.GetComponent<SheepFood>();
            if (food != null && food.EatSheepFood()) wolf.ChangeState(wolf.holdingFood);
            else if (food == null) {
                UnityEngine.Object.Destroy(sheepFood);
                wolf.ChangeState(wolf.holdingFood);
            }
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

    public void OnExit() {
        eatenSheep = null;
        sheepFood = null;
    }
}
