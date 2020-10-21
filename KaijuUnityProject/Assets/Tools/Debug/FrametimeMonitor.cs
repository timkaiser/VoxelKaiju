using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    /// <summary>
    /// Frametime monitor for debugging. Sample count can be set via the config
    /// </summary>
    public class FrametimeMonitor : MonoBehaviour
    {
        //Sample count. Not updated during runtime
        private int sampleCount;
        /// <summary>
        /// List containing all the current frametime sample. Discards old samples if sample count is reached
        /// </summary>
        public float[] Samples;

        /// <summary>
        /// The average frametimes in ms across all frametime samples
        /// </summary>
        public float AverageFrametime { get; private set; }
        /// <summary>
        /// The minimum frametime in ms
        /// </summary>
        public float MinFrametime { get; private set; }
        /// <summary>
        /// The maximum frametime in ms
        /// </summary>
        public float MaxFrametime { get; private set; }

        //Startup
        private void Start()
        {
            //Setup samples
            sampleCount = Config.GetInt(Config.Type.Tools, "Debug.FPSMonitorSampleCount");
            Samples = new float[sampleCount];
        }

        //Update
        void Update()
        {
            //Update array and recalculate values
            AverageFrametime = 0;
            MinFrametime = 10000;
            MaxFrametime = 0;

            //Iterate through existing samples without the oldest one
            for (int i = 1; i < sampleCount; i++)
            {
                //Move old samples
                Samples[i - 1] = Samples[i];
                //Add to average
                AverageFrametime += Samples[i];
                //Update min and max
                if (Samples[i] < MinFrametime)
                    MinFrametime = Samples[i];
                if (Samples[i] > MaxFrametime)
                    MaxFrametime = Samples[i];
            }
            //Recalculate average
            AverageFrametime /= (sampleCount - 1);
            //Add new sample to array
            Samples[sampleCount - 1] = Time.unscaledDeltaTime * 1000.0f;
        }
    }
}