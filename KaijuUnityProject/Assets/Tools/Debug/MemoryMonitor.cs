using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Tools
{
    /// <summary>
    /// Memory monitor for debugging. Sample count can be set via the config
    /// </summary>
    public class MemoryMonitor : MonoBehaviour
    {
        //Sample count. Not updated during runtime
        private int sampleCount;
        /// <summary>
        /// List containing all the current allocated memory samples. Discards old samples if sample count is reached
        /// </summary>
        public float[] SamplesAllocated;
        /// <summary>
        /// List containing all the current reserved memory samples. Discards old samples if sample count is reached
        /// </summary>
        public float[] SamplesReserved;
        /// <summary>
        /// The maximum value for displaying the graph
        /// </summary>
        public float MaxValue;

        // Start is called before the first frame update
        void Start()
        {
            //Setup samples
            sampleCount = Config.GetInt(Config.Type.Tools, "Debug.MemoryMonitorSampleCount");
            SamplesAllocated = new float[sampleCount];
            SamplesReserved = new float[sampleCount];
        }

        // Update is called once per frame
        void Update()
        {
            MaxValue = 0.0f;

            //Iterate through existing samples without the oldest one
            for (int i = 1; i < sampleCount; i++)
            {
                //Move old samples
                SamplesAllocated[i - 1] = SamplesAllocated[i];
                SamplesReserved[i - 1] = SamplesReserved[i];

                //Upgrade max
                if (SamplesAllocated[i] > MaxValue)
                    MaxValue = SamplesAllocated[i];
                if (SamplesReserved[i] > MaxValue)
                    MaxValue = SamplesReserved[i];
            }
            //Add new sample to arrays
            SamplesAllocated[sampleCount - 1] = Profiler.GetTotalAllocatedMemoryLong() / 1048576f;
            SamplesReserved[sampleCount - 1] = Profiler.GetTotalReservedMemoryLong() / 1048576f;
        }
    }
}