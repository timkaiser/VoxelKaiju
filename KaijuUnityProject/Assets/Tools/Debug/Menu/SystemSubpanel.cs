using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    /// <summary>
    /// Subpanel for the system information
    /// </summary>
    public class SystemSubpanel : Subpanel
    {
        [Header("Memory Monitor")]
        /// <summary>
        /// Monitor class to get the memory data
        /// </summary>
        public MemoryMonitor MemoryMonitor;

        [Header("Graph settings")]
        /// <summary>
        /// Allocated memory graph UI image
        /// </summary>
        public Image AllocatedMemoryGraph;
        /// <summary>
        /// Reserved memory graph UI image
        /// </summary>
        public Image ReservedMemoryGraph;
        /// <summary>
        /// Colour used for fps average
        /// </summary>
        public Color AllocatedColor;
        /// <summary>
        /// Colour used for fps values above 60 or frametime values below 16.6 ms
        /// </summary>
        public Color ReservedColor;

        //The frametimes adjusted to be between 0 and 1
        float[] adjustedAllocatedSamples;
        float[] adjustedReservedSamples;

        [Header("Label settings")]
        /// <summary>
        /// UI element for the reserved memory amount
        /// </summary>
        public Text ReservedText;
        /// <summary>
        /// UI element for the allocated memory amount
        /// </summary>
        public Text AllocatedText;

        [Header("System Information")]
        /// <summary>
        /// UI element for the device identifier
        /// </summary>
        public Text DeviceIdentifier;
        /// <summary>
        /// UI element for the device type
        /// </summary>
        public Text DeviceType;
        /// <summary>
        /// UI element for the device OS
        /// </summary>
        public Text DeviceOS;
        /// <summary>
        /// UI element for the processor type
        /// </summary>
        public Text ProcessorType;
        /// <summary>
        /// UI element for the processor count
        /// </summary>
        public Text ProcessorCount;
        /// <summary>
        /// UI element for the memory size
        /// </summary>
        public Text MemorySize;
        
        //Initialization
        private void Start()
        {
            int sampleCount = Config.GetInt(Config.Type.Tools, "Debug.FPSMonitorSampleCount");

            //Setup the material vars for the graph
            AllocatedMemoryGraph.material.SetColor("_GraphColor", AllocatedColor);
            ReservedMemoryGraph.material.SetColor("_GraphColor", ReservedColor);
            AllocatedMemoryGraph.material.SetInt("GraphValues_Length", sampleCount);
            ReservedMemoryGraph.material.SetInt("GraphValues_Length", sampleCount);

            //Setup the colour vars for the labels
            ReservedText.color = ReservedColor;
            AllocatedText.color = AllocatedColor;

            //Create the array for the adjusted values
            adjustedAllocatedSamples = new float[sampleCount];
            adjustedReservedSamples = new float[sampleCount];

            //Set the device information
            DeviceIdentifier.text = SystemInfo.deviceUniqueIdentifier;
            DeviceType.text = SystemInfo.deviceType.ToString();
            DeviceOS.text = SystemInfo.operatingSystem;
            ProcessorType.text = SystemInfo.processorType;
            ProcessorCount.text = SystemInfo.processorCount + " Threads";
            MemorySize.text = SystemInfo.systemMemorySize + " MB RAM";
        }

        //Update
        private void Update()
        {
            //Update graph frametimes
            for (int i = 0; i < adjustedAllocatedSamples.Length; i++)
            {
                adjustedAllocatedSamples[i] = 0.95f * MemoryMonitor.SamplesAllocated[i] / MemoryMonitor.MaxValue;
                adjustedReservedSamples[i] = 0.95f * MemoryMonitor.SamplesReserved[i] / MemoryMonitor.MaxValue;
            }
            AllocatedMemoryGraph.material.SetFloatArray("GraphValues", adjustedAllocatedSamples);
            ReservedMemoryGraph.material.SetFloatArray("GraphValues", adjustedReservedSamples);

            //Upgrade labels
            ReservedText.text = MemoryMonitor.SamplesReserved[adjustedReservedSamples.Length - 1].ToString("0.0");
            AllocatedText.text = MemoryMonitor.SamplesAllocated[adjustedAllocatedSamples.Length - 1].ToString("0.0");
        }
    }
}