using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using UnityEngine.UI;

namespace Character
{
    public class KaijuController : MonoBehaviour
    {

        public Animator anim;
        private PlayerInputManager inputManager;
        public new CharacterCamera camera;
        GameManager gameManager;

        //Stats
        public float energy = 0f;

        enum AttackButtonPressed
        {
            None = 0,
            Light = 1,
            Heavy = 2,
            Forward = 4,
            Alternative = 8
        }

        //Movement
        [SerializeField]
        private float movementSpeed = 1.0f;
        [SerializeField]
        private float attackingFactor = 0.5f;
        private CharacterController controller;
        private Vector3 direction;
        private float intensity = 0.0f;
        Transform pivot;

        //Attacks
        private Attacks attacks;
        bool lightAttackActing = false;
        bool heavyAttackActing = false;
        bool forwardAttackActing = false;
        bool alternativeAttackActing = false;

        [SerializeField]
        float ComboInterval = 0.5f; //seconds till action is 
        float runningInterval = 0.0f;

        bool shoulderLeftActing = false;
        bool shoulderRightActing = false;

        bool charging = false;
        float chargeRampUp = 0.0f;

        bool loadFireball = false;
        GameObject FireballPrefab;
        Fireball fireball;
        [SerializeField]
        GameObject firestormObj;
        FireStorm firestorm;
        [SerializeField]
        Transform fireballSpawn;
        float fireballLoadTime = 0.0f;
        float energyUsed = 0.0f;

        Text energyText;
        [SerializeField]
        Material energySpikesMaterial;
        [SerializeField]
        Material energyColorMaterial;

        //Growth
        Highscore highscoreRef;
        float damageMultiplier = 1f;
        float scaleDamageby = 5f;
        [SerializeField]
        int[] scoreToGrow = { 100, 200 };
        [SerializeField]
        int growthLevel = 0;
        [SerializeField]
        float growthScale = 0.0f;
        [SerializeField]
        bool stopJumpAttack = false;

        [SerializeField]
        bool DoCombo = false;


        // Start is called before the first frame update
        void Start()
        {
            gameManager = GameManager.Instance;
            inputManager = PlayerInputManager.Instance;
            if (anim == null || inputManager == null)
            {
                Debug.LogError("animator or input Manager missing");
            }
            controller = GetComponent<CharacterController>();
            if (controller == null)
            {
                Debug.LogError("character missing rigidbody");
            }
            //rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            direction = transform.forward;
            pivot = this.transform.Find("pivot");
            if (pivot == null)
            {
                Debug.LogError("GameCharacter misses a pivot to rotate around");
            }
            attacks = AttackList.InitializeAttacks();

            //find energy UI
            FireballPrefab = Resources.Load<GameObject>("prefabs/Effects/fireball");
            if (FireballPrefab == null)
            {
                Debug.LogError("fireball is missing");
            }
            highscoreRef = FindObjectOfType<Highscore>();
            if (highscoreRef == null)
            {
                Debug.LogError("character wont grow, highscoreRef not found");
            }
            firestorm = firestormObj.GetComponent<FireStorm>();

            GameManager.Instance.RestartGameEvent.AddListener(ResetCharacter);
        }

        // Update is called once per frame
        void Update()
        {
            //Pause Player
            var useFightAnimLayer = false;
            var isGrowing = false;
            if (gameManager.GameState == GameManager.GameStates.Paused || gameManager.GameState == GameManager.GameStates.Scores)
            {
                anim.enabled = false;
                intensity = 0;
                if (firestorm != null)
                    firestorm.Pause();

            }
            else //not paused
            {
                anim.enabled = true;
                if (firestorm != null)
                    firestorm.Resume();

                //Grow Character
                int highscore = highscoreRef.GetScore();
                if (growthLevel < scoreToGrow.Length && highscore >= scoreToGrow[growthLevel])
                {
                    Growth();
                }
                //Growing
                
                if (isCurrentAnimationName(0, "Reworked_Grow"))
                {
                    CoolAttackButtons();
                    anim.SetBool("grow", false);
                    transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale * growthScale, 0.1f * Time.deltaTime);
                    FireballPrefab.transform.localScale = Vector3.Lerp(FireballPrefab.transform.localScale, FireballPrefab.transform.localScale * growthScale, 0.1f * Time.deltaTime);
                    //controller.stepOffset = Mathf.Lerp(controller.stepOffset, controller.stepOffset * growthScale * 0.5f, 0.1f * Time.deltaTime);
                    movementSpeed = Mathf.Lerp(movementSpeed, movementSpeed * growthScale * 0.8f, 0.1f * Time.deltaTime);
                    energy = Mathf.Lerp(energy, 0.0f, Time.deltaTime);
                    camera.UpdateDefaultDistances(growthScale);
                    isGrowing = true;
                }

                charging = anim.GetCurrentAnimatorStateInfo(0).IsTag("charge");
                useFightAnimLayer = anim.GetCurrentAnimatorStateInfo(1).IsTag("attack");

                //fetch Input
                if (!inputManager.PlayOnPC())
                {
                    intensity = inputManager.GetLeftStick().magnitude;
                }
                else
                {
                    intensity = Mathf.Clamp(Mathf.Abs(inputManager.GetVerticalLeft()) + Mathf.Abs(inputManager.GetHorizontalLeft()), 0f, 1f);
                }

                //Character Combo System
                bool attacking = IsAttacking();
                if (AreAttackButtonsPressed())
                {
                    runningInterval = ComboInterval;
                    lightAttackActing |= inputManager.GetAttackButtonLightTab();
                    heavyAttackActing |= inputManager.GetAttackButtonHeavy();
                    forwardAttackActing |= inputManager.GetAttackButtonForward();
                    alternativeAttackActing |= inputManager.GetAttackButtonLeftTab();

                }
                if (AreAttackButtonsActing())
                {
                    if (runningInterval > 0.0f)
                    {
                        runningInterval -= Time.deltaTime;
                    }
                    else
                    {
                        CoolAttackButtons();
                    }
                }

                if (attacking)
                {
                    intensity *= attackingFactor; //slow movement, enforce attack animation to finish
                }

                //SPECIAL ATTACKS
                if (isCurrentAnimationName(1, "roar2"))
                {
                    if (!loadFireball && FireballPrefab != null)
                    {
                        GameObject fireballObj = Instantiate(FireballPrefab, fireballSpawn.position, Quaternion.identity);
                        fireball = fireballObj.GetComponent<Fireball>();
                        loadFireball = true;
                        energyUsed = 20f;
                        energy = Mathf.Max((float)(Math.Round((energy - 20f), 2)), 0.0f);
                        fireballLoadTime = 0.0f;
                    }
                    if (fireball != null && loadFireball && FireballPrefab != null)
                    {
                        fireball.gameObject.transform.position = fireballSpawn.position;
                        fireballLoadTime += Time.deltaTime * 0.5f;
                        energyUsed += Time.deltaTime * 5f;
                        anim.SetFloat("energyUsed", energyUsed);
                        energy = Mathf.Max((float)(Math.Round((energy - Time.deltaTime * 5f), 2)), 0.0f);
                        fireball.Increase(fireballLoadTime);

                    }
                }

                if (isCurrentAnimationName(1, "roar2_release"))
                {
                    if (loadFireball)
                    {
                        fireball.InitializeFireball(transform.forward, energyUsed, getDamageFactor());
                        loadFireball = false;
                        energyUsed = 0.0f;
                    }
                }

                if (isCurrentAnimationName(0, "roarWithFire2"))
                {
                    energy = Mathf.Max((float)(Math.Round((energy - Time.deltaTime * 7.5f), 2)), 0.0f);
                    intensity = 0;
                    lightAttackActing = false;
                    alternativeAttackActing = false;
                }

                if (isCurrentAnimationName(0, "kick") || isCurrentAnimationName(0, "kickFlipped"))
                {
                    //intensity = 0;
                }

                //block everything in halt animation
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("halt"))
                {
                    intensity = 0;
                    CoolAttackButtons();
                }

                shoulderRightActing = inputManager.GetButtonShoulderRight();
                shoulderLeftActing = inputManager.GetButtonShoulderLeft();

                //Charge
                if (shoulderRightActing)
                {
                    chargeRampUp = Mathf.Min(chargeRampUp + 0.2f * Time.deltaTime, 1.0f);
                    intensity = intensity + chargeRampUp;
                }
                else
                {
                    chargeRampUp = 0;
                }

                //Rotation
                float verticalLeft = inputManager.GetVerticalLeft();
                float horizontalLeft = inputManager.GetHorizontalLeft();


                Vector3 lookingDirection = camera.getCameraDirectionForward() * verticalLeft + camera.getCameraDirectionRight() * horizontalLeft;


                //jump attack
                if (isCurrentAnimationName(0, "jumpattack2"))
                {
                    if(!stopJumpAttack)
                    {
                        anim.applyRootMotion = true;
                        lookingDirection = direction;
                    }
                    else
                    {
                        intensity = 0;
                    }
                }
                else
                {
                    anim.applyRootMotion = false;
                }

                //rotation
                if(!isCurrentAnimationName(0, "Reworked_Grow"))
                {
                    Vector3 rotationDir = Vector3.RotateTowards(transform.forward, lookingDirection, 0.5f, 0.0f);
                    transform.rotation = Quaternion.LookRotation(rotationDir);
                    direction = lookingDirection;
                }

            } //not pause

            //block everything in halt animation
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("halt"))
            {
                intensity = 0;
            }

            Vector3 moveTowards = transform.position + (direction * movementSpeed * intensity);
            //gravity
            var speed = direction * movementSpeed * intensity;
            if (isCurrentAnimationName(1, "headbutt2"))
            {
                //speed += transform.forward * 10.0f * transform.localScale.x;
            }
            if (anim.applyRootMotion == false)
            {
                speed.y -= 9.8f;
            }
            else
            {
                speed.y -= 0.2f;
            }
            controller.Move((speed * Time.deltaTime));

            //set animator values
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Damaged"))
            {
                anim.SetBool("isRoaring", false);
            }

            intensity = AbsThreshold(intensity, 0.2f);
            anim.SetFloat("movement", intensity);
            anim.SetFloat("signedMovement", inputManager.GetVerticalLeft() * inputManager.GetVerticalLeftSinceRightShoulder());
            anim.SetBool("lightAttackButton", lightAttackActing);
            anim.SetBool("heavyAttackButton", heavyAttackActing);
            anim.SetBool("forwardAttackButton", forwardAttackActing);
            anim.SetBool("alternativeAttackButton", alternativeAttackActing);
            anim.SetBool("doCombo", DoCombo);
            anim.SetBool("isCharging", shoulderRightActing);
            anim.SetBool("isGrowing", isGrowing);
            anim.SetBool("usingFightingLayer", useFightAnimLayer);
            anim.SetFloat("chargeRampUp", chargeRampUp);
            anim.SetBool("leftShoulderActing", shoulderLeftActing);
            anim.SetFloat("energy", energy);

            if(energySpikesMaterial != null)
            {
                energySpikesMaterial.SetFloat("_Energy", energy);
                var time = energySpikesMaterial.GetFloat("_Damaged") + Time.deltaTime;
                energySpikesMaterial.SetFloat("_Damaged", time);
            }
            if(energyColorMaterial != null)
            {
                energyColorMaterial.SetFloat("_Energy", energy);
            }
        }

        public void ResetCharacter()
        {
            energy = 0;
            FireballPrefab.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            firestormObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        public void OnApplicationQuit()
        {
            FireballPrefab.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            firestormObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }


        /// <summary>
        /// Call that to get the metadata from the current running attack
        /// </summary>
        public Attack GetCurrentAttack()
        {

            //check base Layer
            AttackID attackId;
            if (anim.GetCurrentAnimatorStateInfo(1).IsTag("attack"))
            {
                string currentAnimationName = anim.GetCurrentAnimatorClipInfo(1)[0].clip.name;
                attackId = (AttackID)System.Enum.Parse(typeof(AttackID), currentAnimationName);
            }
            else
            {
                if (anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
                {
                    string currentAnimationName = anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                    attackId = (AttackID)System.Enum.Parse(typeof(AttackID), currentAnimationName);
                }
                else
                {
                    attackId = AttackID.None;
                }
            }
            return attacks.GetAttackById(attackId);
        }

        public float GetCurrentAttackDamage()
        {
            Attack attack = GetCurrentAttack();
            return attack.getDamage() * damageMultiplier;
        }

        /// <summary>
        /// returns a value, which is bigger if the timing is on point
        /// </summary>
        public float GetCurrentTiming()
        {
            float currentAnimationTime = GetCurrentAnimationLength();
            return runningInterval * currentAnimationTime;
        }

        private float AbsThreshold(float f, float threshold)
        {
            return (Mathf.Abs(f) > threshold) ? f : 0.0f;
        }

        private void CoolAttackButtons()
        {
            runningInterval = 0;
            lightAttackActing = false;
            heavyAttackActing = false;
            forwardAttackActing = false;
            alternativeAttackActing = false;
            anim.SetBool("lightAttackButton", lightAttackActing);
            anim.SetBool("heavyAttackButton", heavyAttackActing);
            anim.SetBool("forwardAttackButton", forwardAttackActing);
            anim.SetBool("alternativeAttackButton", alternativeAttackActing);
        }

        private bool IsAttacking()
        {
            //check in base and fighting layer
            return anim.GetCurrentAnimatorStateInfo(0).IsTag("attack") || anim.GetCurrentAnimatorStateInfo(1).IsTag("attack");
        }

        public float GetCurrentAnimationLength()
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
            {
                return anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            }
            if (anim.GetCurrentAnimatorStateInfo(1).IsTag("attack"))
            {
                return anim.GetCurrentAnimatorClipInfo(1)[0].clip.length;
            }
            return 1.0f; //default
        }

        private bool isCurrentAnimationName(int layer, string name)
        {
            return name == anim.GetCurrentAnimatorClipInfo(layer)[0].clip.name;
        }

        private bool AreAttackButtonsActing()
        {
            return lightAttackActing || heavyAttackActing || forwardAttackActing  || alternativeAttackActing;
        }
        private bool AreAttackButtonsPressed()
        {
            return inputManager.GetAttackButtonLightTab() || inputManager.GetAttackButtonHeavy() || inputManager.GetAttackButtonForward() || inputManager.GetAttackButtonLeftTab();
        }

        public void Growth()
        {
            anim.SetInteger("growLevel", growthLevel);
            anim.SetBool("grow", true);
            growthLevel++;
            damageMultiplier *= scaleDamageby;
        }

        /// <summary>
        /// Call this to damage the character. TODO : discuss that
        /// </summary>
        public void DamageCharacter ()
        {
            if(fireball != null)
            {
                fireball.markDestroy();
            }
            CoolAttackButtons();
            anim.SetBool("isRoaring", true);
            energy = 0;
            //StartDamageDisplay();
        }

        public float getDamageFactor() { return damageMultiplier; }

        public void AddEnergy (float energy)
        {
            this.energy = Mathf.Clamp(this.energy + energy, 0.0f, 100.0f);
        }

        public int GetGrowthLevel()
        {
            return growthLevel;
        }

        public float GetEnergy()
        {
            return energy;
        }

        public void StartDamageDisplay()
        {
            energySpikesMaterial.SetFloat("_Damaged", 0.0f);
        }
    }

}


