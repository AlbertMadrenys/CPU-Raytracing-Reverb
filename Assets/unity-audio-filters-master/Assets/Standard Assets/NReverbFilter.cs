using UnityEngine;
using System.Collections;

public class NReverbFilter : MonoBehaviour
{
    // T60 decay time.
    [Range(0.0f, 4.0f)]
    public float
        decayTime = 1.0f;
    
    // Send/return level.
    [Range(0.0f, 1.0f)]
    public float
        sendLevel = 0.1f;

    // STK NReverb filter.
    Stk.NReverb reverb;

    // Used for detecting parameter changes.
    float prevDecayTime;

    // Used for error handling.
    string error;

    void Awake ()
    {
        if (reverb == null) reverb = new Stk.NReverb(decayTime);
        prevDecayTime = decayTime;
    }

    void Update ()
    {
        if (error == null) {
            if (decayTime != prevDecayTime) {
                reverb.DecayTime = decayTime;
                prevDecayTime = decayTime;
            }
        } else {
            Debug.LogError (error);
            Destroy (this);
        }
    }

    void OnAudioFilterRead (float[] data, int channels)
    {
        if (channels != 2) {
            error = "This filter only supports stereo audio (given:" + channels + ")";
            return;
        }

        if(reverb == null) reverb = new Stk.NReverb(decayTime);

        for (var i = 0; i < data.Length; i += 2) {
            var output = reverb.Tick (0.2f * (data [i] + data [i + 1]));
            data [i] += output.left * sendLevel;
            data [i + 1] += output.right * sendLevel;
        }
    }
}
