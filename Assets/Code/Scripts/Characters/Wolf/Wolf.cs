// ---------------------------------------
// Creation Date: 4/4/24
// Author: Abigail Andam
// Modified By:
// ---------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : MonoBehaviour
{ 
    public event EventHandler OnEnterForest;
    public event EventHandler OnExitForest;
    private WolfInput wolfInput;

    // Sounds
    public class MusicEventArgs { public WolfStatus currentState; public WolfStatus newState; }
    public event EventHandler<MusicEventArgs> UpdateBGMusic;
    public event EventHandler SheepEaten;

    // Handles eating and herding sheep
    private bool beingChased = false;
    public void SetBeingChased(bool beingChased) {
        alertSheep.CanAlertSheep(beingChased);
        this.beingChased = beingChased;

        if (beingChased) {
            UpdateBGMusic?.Invoke(this, new MusicEventArgs { currentState = WolfStatus.Undetected, newState = WolfStatus.Identified});
        } else {
            UpdateBGMusic?.Invoke(this, new MusicEventArgs { currentState = WolfStatus.Identified, newState = WolfStatus.Undetected});
        }
    }

    public LayerMask sheepLayerMask;
    public LayerMask sheepFoodLayerMask;
    public GameObject food;
    public HoldingFood holdingFood = new HoldingFood();
    public NotHoldingFood notHoldingFood = new NotHoldingFood();

    [SerializeField] private AlertSheep alertSheep;
    [SerializeField] private GameObject sheepFoodPrefab;
    [SerializeField] private Transform sheepFoodParent;
    [SerializeField] private LayerMask hideObjectLayerMask;
    [SerializeField] private LayerMask wolfHideInLayerMask;

    private WolfState wolfState;

    private bool isHiding = false;
    public bool GetIsHiding() { return isHiding; }

    [NonSerialized] public GameObject hidingInObject;

    // Animations
    public Animator anim;

    public const string IS_HOLDING_FOOD = "IsHoldingFood";
    public const string EAT_SHEEP = "EatSheep";
    public const string KILLED = "Killed";
    public const string SHOT = "Shot";

    private void Start() {
        anim = transform.GetChild(0).GetComponent<Animator>();

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
        
        if (wolfHideInLayerMask == (wolfHideInLayerMask | (1 << col.gameObject.layer))) {
            hidingInObject = col.gameObject;
            isHiding = !beingChased;
            OnEnterForest?.Invoke(this, EventArgs.Empty);
        }

        wolfState.OnCollisionEnter(col);
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (hideObjectLayerMask == (hideObjectLayerMask | (1 << col.gameObject.layer))) {
            isHiding = false;
        } else if (wolfHideInLayerMask == (wolfHideInLayerMask | (1 << col.gameObject.layer))) {
            OnExitForest?.Invoke(this, EventArgs.Empty);
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

    public void EatSheep() {
        SheepEaten?.Invoke(this, EventArgs.Empty);
    }
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
    private GameObject eatenSheep;

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
        wolf.anim.SetBool(Wolf.IS_HOLDING_FOOD, true);
        wolf.HaveSheepFollow();
        wolf.food.SetActive(true);
    }

    public void OnInteract() {
        if (eatenSheep != null) {
            eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
            wolf.anim.SetTrigger(Wolf.EAT_SHEEP);
            wolf.EatSheep();
        } else {
            wolf.StopSheepFollowing();
        }

        wolf.ChangeState(wolf.notHoldingFood);
    }

    public void OnCollisionEnter(Collider2D col) {
        if (wolf.sheepLayerMask == (wolf.sheepLayerMask | (1 << col.gameObject.layer))) {
            eatenSheep = col.gameObject;
        } 
    }

    public void OnCollisionExit(Collider2D col) {
        if (wolf.sheepLayerMask == (wolf.sheepLayerMask | (1 << col.gameObject.layer))) {
            eatenSheep = null;
        } 
    }

    public void OnExit() {
    }
}

public class NotHoldingFood : WolfState {
    private Wolf wolf;
    private GameObject eatenSheep;
    private GameObject sheepFood;

    public void OnEnter(Wolf wolf) {
        this.wolf = wolf;
        wolf.anim.SetBool(Wolf.IS_HOLDING_FOOD, false);
        wolf.food.SetActive(false);
    }

    public void OnInteract() {
        if (eatenSheep != null) {
            eatenSheep.transform.parent.GetComponent<SheepHerd>().EatSheep(eatenSheep);
            wolf.anim.SetTrigger(Wolf.EAT_SHEEP);
            wolf.EatSheep();
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
