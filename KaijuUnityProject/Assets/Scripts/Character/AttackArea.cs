using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public Vector3 RaycastDirectionCorrection = new Vector3(0, 0, 0);
    public ParticleSystem Particles1, Particles2;
    public Material mat1, mat2;  
    
    Character.KaijuController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<Character.KaijuController>();
        if(controller == null)
        {
            Debug.LogError(this.name + ":  character not found");
        }
        //BoxCollider collider;
    }

    /// <summary>
    /// This function is called whenever the hitbox collider for attacks is triggered by another collider, e.g. a destructible.
    /// </summary>
    /// <param name="other">The colliding collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        Destruction script = other.gameObject.GetComponent<Destruction>();
        if (script != null)
        {
            float attackDamage = controller.GetCurrentAttackDamage();
	    //SoundEffect
            AudioSource source = controller.gameObject.GetComponent<AudioSource>();
            SoundManager.Instance.RandomizePunches(source);
            //            
	    script.DamageByKaiju(attackDamage, controller.transform.position);
            float attackTiming = Mathf.Abs(Mathf.Min(controller.GetCurrentTiming(), 0.99f));
            if(script.GetLevel() >= controller.GetGrowthLevel())
            {
                controller.AddEnergy(controller.GetCurrentAttack().getEnergy());
            }

            //Particles

            DestructionColor destructColor = other.transform.GetComponent<DestructionColor>();
            if (destructColor != null)
            {
                mat1.color = other.gameObject.GetComponent<DestructionColor>().color1;
                Particles1.Play();
                if (destructColor.secondCol == true)
                {
                    mat2.color = other.gameObject.GetComponent<DestructionColor>().color2;
                    Particles2.Play();
                }
            }
            /*Ray ray = new Ray(transform.position, ((transform.right * RaycastDirectionCorrection.x) + (transform.up * RaycastDirectionCorrection.y) + (transform.forward * RaycastDirectionCorrection.z)));
            RaycastHit rayCastHit = new RaycastHit();
            if (Physics.Raycast(ray, out rayCastHit, 100f, LayerMask.GetMask("Destructible"), QueryTriggerInteraction.Ignore)) {
                if(other.gameObject.GetComponent<DestructionColor>().secondCol == true)
                {
                    mat2.color = other.gameObject.GetComponent<DestructionColor>().color2;
                    Particles2.Play();
                }
                mat1.color = other.gameObject.GetComponent<DestructionColor>().color1;
                Particles1.Play();
            }
            if (Physics.Raycast(ray, out rayCastHit, 100f, LayerMask.GetMask("Cars"), QueryTriggerInteraction.Ignore))
            {
                if (other.gameObject.GetComponent<DestructionColor>().secondCol == true)
                {
                    mat2.color = other.gameObject.GetComponent<DestructionColor>().color2;
                    Particles2.Play();
                }
                mat1.color = other.gameObject.GetComponent<DestructionColor>().color1;
                Particles1.Play();
            }*/
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(this.transform.position, transform.position + ((transform.right * RaycastDirectionCorrection.x) + (transform.up * RaycastDirectionCorrection.y) + (transform.forward * RaycastDirectionCorrection.z)));
    }
}
