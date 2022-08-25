using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;

public class LoadDevice : MonoBehaviour
{
    public GameObject DeviceRegisterGroup;
    public GameObject Register;
    public TextAsset DeviceRegisterFile;
    public TextAsset DeviceTopologyFile;

    // Start is called before the first frame update
    void Start()
    {
        // initialize registers
        string register_text = DeviceRegisterFile.text;
        string[] register_list = register_text.Split('\n');
        int x_offset = -20;
        int z_offset = -15;
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
            register.transform.position = new Vector3(x_pos * 10 + x_offset, 0, z_pos * 10 + z_offset);
            Text reg_text = register.GetComponentInChildren<Text>();
            reg_text.text = "Physical " + reg_id.ToString();
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
