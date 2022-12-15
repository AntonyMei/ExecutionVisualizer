using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyboardHandler : MonoBehaviour
{
    // game related
    public GameObject Canvas;
    public GameObject RCanvas;
    private bool is_panel_visible = true;
    private bool is_r_canvas_visible = true;
    public bool TextUseAbbrevation = false;

    // visualization related
    public InputField ExecutionHistoryInput;
    public GameObject RegisterGroup;
    public GameObject EdgeGroup;
    public Material IdleMat;
    public Material UsedMat;
    public Material SelectedMat;
    public Text CurrentStepField;
    public Text Physical2LogicalField;
    public Text Logical2PhysicalField;
    public Text CurrentExecutionField;

    // visualizaiton related
    private string raw_execution_history;
    private int num_registers;
    private int num_qubits;
    private int num_steps;
    private int step_pointer;
    private string[] execution_history;
    private int[] physical2logical;
    private int[] logical2phsyical;
    private string highlighted_edge_name = null;
    private bool initialized = false;

    // reset related
    public string CurrentSceneName;


    void Update()
    {
        // game related
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            is_panel_visible = !is_panel_visible;
            Canvas.SetActive(is_panel_visible);
        } else if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            is_r_canvas_visible = !is_r_canvas_visible;
            RCanvas.SetActive(is_r_canvas_visible);
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        } else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (initialized && step_pointer < num_steps - 1)
            {
                step_pointer += 1;
                render(true);
            }
        } else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (initialized && step_pointer > 0)
            {
                render(true);
                step_pointer -= 1;
                render(false);
            }
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(CurrentSceneName);
    }

    public void HandleVisualizeRequest()
    {
        // parse input
        raw_execution_history = ExecutionHistoryInput.text;
        string[] _tmp_execution_history = raw_execution_history.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
        num_registers = Int32.Parse(_tmp_execution_history[0].Split(' ')[0]);
        num_qubits = Int32.Parse(_tmp_execution_history[0].Split(' ')[1]);
        num_steps = Int32.Parse(_tmp_execution_history[3]);
        Debug.Assert(_tmp_execution_history.Length == num_steps + 4);
        // execution history
        execution_history = new string[num_steps];
        for (int i = 4; i < 4 + num_steps; ++i)
        {
            execution_history[i - 4] = _tmp_execution_history[i];
        }
        // mapping table
        string[] _raw_physical2logical = _tmp_execution_history[1].Split(' ');
        physical2logical = new int[num_registers];
        for (int idx = 0; idx < num_registers; ++idx)
        {
            physical2logical[idx] = Int32.Parse(_raw_physical2logical[idx]);
        }
        string[] _raw_logical2physical = _tmp_execution_history[2].Split(' ');
        logical2phsyical = new int[num_registers];
        for (int idx = 0; idx < num_registers; ++idx)
        {
            logical2phsyical[idx] = Int32.Parse(_raw_logical2physical[idx]);
        }
        initialized = true;

        // visualize registers
        step_pointer = 0;
        render(true);
    }

    void render(bool enable_swap_effect)
    {
        // parse current step
        string[] cur_step = execution_history[step_pointer].Split(' ');
        int cur_guid = Int32.Parse(cur_step[0]);
        string cur_gate_type = cur_step[1];
        int physical0 = Int32.Parse(cur_step[2]);
        int physical1 = Int32.Parse(cur_step[3]);
        int logical0 = Int32.Parse(cur_step[4]);
        int logical1 = Int32.Parse(cur_step[5]);

        // change mapping table
        if (cur_gate_type == "swap" && enable_swap_effect)
        {
            // we need a tmp here to make sure the operation can be reversed
            int log_tmp = logical2phsyical[logical0];
            logical2phsyical[logical0] = logical2phsyical[logical1];
            logical2phsyical[logical1] = log_tmp;
            int phy_tmp = physical2logical[physical0];
            physical2logical[physical0] = physical2logical[physical1];
            physical2logical[physical1] = phy_tmp;
        }

        // background
        for (int idx = 0; idx < num_registers; ++idx)
        {
            RegisterGroup.transform.GetChild(idx).GetComponent<Renderer>().material = IdleMat;
        }
        if (!(highlighted_edge_name is null))
        {
            EdgeGroup.transform.Find(highlighted_edge_name).GetComponent<Renderer>().material = IdleMat;
            highlighted_edge_name = null;
        }

        // used
        for (int idx = 0; idx < num_qubits; ++idx)
        {
            int physical_id = logical2phsyical[idx];
            RegisterGroup.transform.GetChild(physical_id).GetComponent<Renderer>().material = UsedMat;
        }

        // highlight
        // RegisterGroup.transform.GetChild(physical0).GetComponent<Renderer>().material = SelectedMat;
        // RegisterGroup.transform.GetChild(physical1).GetComponent<Renderer>().material = SelectedMat;
        var cur_device_edge = EdgeGroup.transform.Find(physical0 + "-" + physical1);
        highlighted_edge_name = physical0 + "-" + physical1;
        if (cur_device_edge is null)
        {
            cur_device_edge = EdgeGroup.transform.Find(physical1 + "-" + physical0);
            highlighted_edge_name = physical1 + "-" + physical0;
        }
        cur_device_edge.GetComponent<Renderer>().material = SelectedMat;

        // set text
        CurrentStepField.text = step_pointer.ToString();
        CurrentExecutionField.text = execution_history[step_pointer];
        Logical2PhysicalField.text = "";
        Physical2LogicalField.text = "";
        for (int idx = 0; idx < num_registers; ++idx)
        {
            Logical2PhysicalField.text += idx + "->" + logical2phsyical[idx] + " ";
            Physical2LogicalField.text += idx + "->" + physical2logical[idx] + " ";
        }

        // set floating canvas
        for (int idx = 0; idx < num_registers; ++idx)
        {
            string cur_text;
            if (TextUseAbbrevation)
            {
                cur_text = "P " + idx + "\nL " + physical2logical[idx];
            } else
            {
                cur_text = "Physical " + idx + "\nLogical " + physical2logical[idx];
            }
            RegisterGroup.transform.GetChild(idx).GetComponentInChildren<Text>().text = cur_text;
        }
    }
}
