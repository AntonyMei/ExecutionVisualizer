using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;


public class LoadDevice : MonoBehaviour
{
    public GameObject DeviceRegisterGroup;
    public GameObject DeviceEdgeGroup;
    public GameObject Register;
    public GameObject Edge;
    public TextAsset DeviceRegisterFile;
    public TextAsset DeviceTopologyFile;

    void CreateCylinderBetweenPoints(Vector3 start, Vector3 end, float width, int idx0, int idx1)
    {
        var offset = end - start;
        var scale = new Vector3(width, offset.magnitude / 2.0f, width);
        var position = start + (offset / 2.0f);

        var cylinder = Instantiate(Edge, position, Quaternion.identity);
        cylinder.transform.up = offset;
        cylinder.transform.localScale = scale;
        cylinder.transform.parent = DeviceEdgeGroup.transform;
        cylinder.name = idx0.ToString() + "-" + idx1.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        // initialize registers
        string register_text = DeviceRegisterFile.text;
        string[] register_list = register_text.Split('\n');
        int x_offset = -20;     // Q20: -20, Q65 -25
        int z_offset = -15;     // Q20: -15, Q65 -20
        int x_interval = 10;     // Q20: 10,  Q65 5
        int z_interval = 10;     // Q20: 10,  Q65 5
        foreach (string register_pos in register_list)
        {
            // unpack position
            string[] id_pos_list = register_pos.Split(' ');
            int reg_id = Int32.Parse(id_pos_list[0]);
            int x_pos = Int32.Parse(id_pos_list[1]);
            int z_pos = Int32.Parse(id_pos_list[2]);

            GameObject register = Instantiate<GameObject>(Register);
            register.name = reg_id.ToString();
            register.transform.parent = DeviceRegisterGroup.transform;
            register.transform.position = new Vector3(x_pos * x_interval + x_offset, 0, z_pos * z_interval + z_offset);
            Text reg_text = register.GetComponentInChildren<Text>();
            reg_text.text = "Physical " + reg_id.ToString();
        }

        // initialize device edges
        string register_topology_text = DeviceTopologyFile.text;
        string[] register_edge_list = register_topology_text.Split('\n');
        foreach (string edge in register_edge_list)
        {
            // unpack edge
            string[] id_list = edge.Split(' ');
            int idx0 = Int32.Parse(id_list[0]);
            int idx1 = Int32.Parse(id_list[1]);

            // create edge
            Vector3 pos0 = DeviceRegisterGroup.transform.GetChild(idx0).position;
            Vector3 pos1 = DeviceRegisterGroup.transform.GetChild(idx1).position;
            CreateCylinderBetweenPoints(pos0, pos1, 2, idx0, idx1);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
