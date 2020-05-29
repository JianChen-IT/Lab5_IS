using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using System.Linq;
using System.Diagnostics;

public class TrackingReceiverSimplified : MonoBehaviour
{
    //GameObjects to be controlled with Posenet
    public GameObject nose;
    public GameObject leftWrist;
    public GameObject rightWrist;
    public GameObject leftKnee;
    public GameObject rightKnee;


    //OSC Variables
    private OSCReceiver _receiver;
    private const string _oscAddress = "/pose/0";

    //Dictionary to store pose data
    public Dictionary<string, Vector3> pose = new Dictionary<string, Vector3>();

    //The number of lines
    int lengthOfLineRenderer = 18;

    //Set active the line renderer
    public bool lineRenderOn = false; 

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Set up OSC receiver
        StartOSCReceiver();

        //Initialize pose
        StartPose();

        //Creates the lines
        if (lineRenderOn)
        {
            LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.widthMultiplier = 2f;
            lineRenderer.positionCount = lengthOfLineRenderer;
        }

    }

    void StartOSCReceiver() {
        // Creating a receiver.
        _receiver = gameObject.AddComponent<OSCReceiver>();

        // Set local port.
        _receiver.LocalPort = 9876;

        // Bind "MessageReceived" method to special address.
        _receiver.Bind(_oscAddress, MessageReceived);
    }

    void StartPose() {
        pose.Add("nose", Vector3.zero);
        pose.Add("leftWrist", Vector3.zero);
        pose.Add("rightWrist", Vector3.zero);
        pose.Add("leftKnee", Vector3.zero);
        pose.Add("rightKnee", Vector3.zero);
    }
    

    // Update is called once per frame
    void Update()
    {
        //Update GameObjects positions that represents the joins in PoseNet
        //Note: these mappings are mirror, to obtain a correct side position when you see the character from the back
        nose.transform.position = pose["nose"];
        leftWrist.transform.position = pose["rightWrist"];
        rightWrist.transform.position = pose["leftWrist"];
        leftKnee.transform.position = pose["rightKnee"];
        rightKnee.transform.position = pose["leftKnee"];


        //If lineRenderOn = true draw the lines
        if (lineRenderOn)
        {
            //Update lines
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            var points = new Vector3[lengthOfLineRenderer];
            lineRenderer.SetPosition(0, leftWrist.transform.position);
            lineRenderer.SetPosition(1, leftKnee.transform.position);
            lineRenderer.SetPosition(2, leftKnee.transform.position);
            lineRenderer.SetPosition(3, rightKnee.transform.position);
            lineRenderer.SetPosition(4, rightKnee.transform.position);
            lineRenderer.SetPosition(5, rightWrist.transform.position);
        }

    }

    protected void MessageReceived(OSCMessage message)
    {
        List<OSCValue> list = message.Values;
        //UnityEngine.Debug.Log(list.Count);

        for(int i=0;i<list.Count; i+=3)
        {
            string key = "";
            Vector2 position = Vector3.zero; 

            OSCValue val0 = list.ElementAt(i);
            if (val0.Type == OSCValueType.String) key = val0.StringValue;
            OSCValue val1 = list.ElementAt(i+1);
            if (val1.Type == OSCValueType.Float) position.x = val1.FloatValue-250;
            OSCValue val2 = list.ElementAt(i+2);
            if (val2.Type == OSCValueType.Float) position.y = -(val2.FloatValue-250);

            if (pose.ContainsKey(key)) {
                pose[key] = position; 
            }
        }

    }

}
