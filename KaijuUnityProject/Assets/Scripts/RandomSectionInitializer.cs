using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RandomSectionInitializer : MonoBehaviour
{
    public enum SECTION_PREFAB{ EMPTY, CITY, CITY_CORNER, CITY_BACKYARD, PARK, SUBURB_HOUSE, SUBURB_GARDEN, HIGHRISER, BEACH, BEACH_CORNER, STREET_STRAIGHT, STREET_CURVE, STREET_T_INTERSECT, STREET_INTERSECT, GOAL, GOAL_EMPTY };
    public static GameObject[][] section_prefabs; //prefabs for tile, array order is: [Area] [(no_neigbors | straight | corner)] [index]

    public SECTION_PREFAB section_type;
    public float x_offset, y_offset, z_offset, rotation_offset;

    // Start is called before the first frame update
    void Start(){
        initializePrefabArray();
        instantiateTile();
    }

    void initializePrefabArray() {
        if (section_prefabs != null) { return; }
        section_prefabs = new GameObject[SECTION_PREFAB.GetNames(typeof(SECTION_PREFAB)).Length][];

        for (int i = 0; i < section_prefabs.Length; i++) {
            string name = "" + SECTION_PREFAB.GetNames(typeof(SECTION_PREFAB))[i];
            section_prefabs[i] = Resources.LoadAll("prefabs/Sections/" + name, typeof(GameObject)).Cast<GameObject>().ToArray();
        }
    }

    void instantiateTile() {
        int rand = Random.Range(0, section_prefabs[(int)section_type].Length);
        Vector3 pos = transform.position + transform.right * x_offset + transform.up * y_offset + transform.forward * z_offset;
        Instantiate(
            section_prefabs[(int)section_type][rand],
            pos,
            Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + rotation_offset, transform.rotation.eulerAngles.z)
            ).transform.SetParent(this.transform);
    }
}
