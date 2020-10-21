using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    /// <summary>
    /// Subpanel for the rendering information
    /// </summary>
    public class RenderingSubpanel : Subpanel
    {
        [Header("FPS Monitor")]
        /// <summary>
        /// Monitor class to get the frametime data
        /// </summary>
        public FrametimeMonitor FPSMonitor;

        [Header("Graph settings")]
        /// <summary>
        /// FPS graph UI image
        /// </summary>
        public Image FPSGraph;

        /// <summary>
        /// Colour used for fps average
        /// </summary>
        public Color AverageFrametimeColor;
        /// <summary>
        /// Colour used for fps values above 60 or frametime values below 16.6 ms
        /// </summary>
        public Color Below16FrametimeColor;
        /// <summary>
        /// Colour used for fps values above 30 or frametime values below 33.3 ms
        /// </summary>
        public Color Below33FrametimeColor;
        /// <summary>
        /// Colour used for fps values below 30 or frametime values above 33.3 ms
        /// </summary>
        public Color DefaultFrametimeColor;

        /// <summary>
        /// Rect transform for the average text
        /// </summary>
        public RectTransform GraphAVG;
        /// <summary>
        /// Average text component
        /// </summary>
        public Text GraphAVGText;
        /// <summary>
        /// Rect transform for the 30 text
        /// </summary>
        public RectTransform Graph33;
        /// <summary>
        /// 30 text component
        /// </summary>
        public Text Graph33Text;
        /// <summary>
        /// Rect transform for the 60 text
        /// </summary>
        public RectTransform Graph16;
        /// <summary>
        /// 60 text component
        /// </summary>
        public Text Graph16Text;
        
        [Header("FPS Texts")]
        /// <summary>
        /// Text element for displaying the minimum FPS in the given sample history
        /// </summary>
        public Text MinFPSText;
        /// <summary>
        /// Text element for displaying the maximum FPS in the given sample history
        /// </summary>
        public Text MaxFPSText;
        /// <summary>
        /// Text element for displaying the average FPS in the given sample history
        /// </summary>
        public Text AvgFPSText;
        /// <summary>
        /// Text element for displaying the minimum FPS in the given sample history in ms
        /// </summary>
        public Text MinMSText;
        /// <summary>
        /// Text element for displaying the maximum FPS in the given sample history in ms
        /// </summary>
        public Text MaxMSText;
        /// <summary>
        /// Text element for displaying the average FPS in the given sample history in ms
        /// </summary>
        public Text AvgMSText;

        [Header("Device information")]
        /// <summary>
        /// Text element for displaying device information on the graphics device name
        /// </summary>
        public Text GraphicsDeviceNameText;
        /// <summary>
        /// Text element for displaying device information on the graphics device memory size
        /// </summary>
        public Text GraphicsMemorySizeText;
        /// <summary>
        /// Text element for displaying device information on the graphics device shader level
        /// </summary>
        public Text GraphicsShaderLevelText;
        /// <summary>
        /// Text element for displaying device information on the graphics device version
        /// </summary>
        public Text GraphicsDeviceVersionText;
        /// <summary>
        /// Text element for displaying device information on compute shader support
        /// </summary>
        public Text SupportsComputeShadersText;
        /// <summary>
        /// Text element for displaying device information on async gpu support
        /// </summary>
        public Text SupportsAsyncGPUReadbackText;

        //The frametimes adjusted to be between 0 and 1
        float[] adjustedSamples;

        //Initialization
        private void Start()
        {
            int sampleCount = Config.GetInt(Config.Type.Tools, "Debug.FPSMonitorSampleCount");
            
            //Setup the material vars for the graph
            FPSGraph.material.SetColor("_Below16Color", Below16FrametimeColor);
            FPSGraph.material.SetColor("_Below33Color", Below33FrametimeColor);
            FPSGraph.material.SetColor("_DefaultColor", DefaultFrametimeColor);
            FPSGraph.material.SetInt("GraphValues_Length", sampleCount);

            //Setup the colour vars for the labels
            GraphAVGText.color = AverageFrametimeColor;
            Graph33Text.color = Below33FrametimeColor;
            Graph16Text.color = Below16FrametimeColor;
            
            //Fill the device information
            GraphicsDeviceNameText.text = SystemInfo.graphicsDeviceName;
            GraphicsMemorySizeText.text = SystemInfo.graphicsMemorySize + " MB Memory";
            GraphicsShaderLevelText.text = "Shader Level " + ((float)SystemInfo.graphicsShaderLevel / 10.0f).ToString("0.0");
            GraphicsDeviceVersionText.text = SystemInfo.graphicsDeviceVersion;
            SupportsComputeShadersText.text = "Compute support: " + SystemInfo.supportsComputeShaders;
            SupportsAsyncGPUReadbackText.text = "Async GPU support: " + SystemInfo.supportsAsyncGPUReadback;

            //Create the array for the adjusted values
            adjustedSamples = new float[sampleCount];
        }

        //Update
        private void Update()
        {
            //Update colour thresholds 
            FPSGraph.material.SetFloat("_16MS", 16.6f / FPSMonitor.MaxFrametime);
            FPSGraph.material.SetFloat("_33MS", 33.3f / FPSMonitor.MaxFrametime);
            FPSGraph.material.SetFloat("_Average", FPSMonitor.AverageFrametime / FPSMonitor.MaxFrametime);

            //Update graph frametimes
            for (int i = 0; i < adjustedSamples.Length; i++)
                adjustedSamples[i] = FPSMonitor.Samples[i] / FPSMonitor.MaxFrametime;
            FPSGraph.material.SetFloatArray("GraphValues", adjustedSamples);

            //Update graph labels
            GraphAVG.anchorMin.Set(0, FPSMonitor.AverageFrametime / FPSMonitor.MaxFrametime);
            GraphAVG.anchorMax.Set(0.1f, FPSMonitor.AverageFrametime / FPSMonitor.MaxFrametime);
            GraphAVG.sizeDelta.Set(0.0f, 100.0f);

            Graph33.anchorMin.Set(0, 33.3f / FPSMonitor.MaxFrametime);
            Graph33.anchorMax.Set(0.1f, 33.3f / FPSMonitor.MaxFrametime);
            if (33.3f / FPSMonitor.MaxFrametime > 1.0f)
                Graph33.sizeDelta = Vector2.zero;
            else
                Graph33.sizeDelta.Set(0.0f, 100.0f);

            Graph16.anchorMin.Set(0, 16.6f / FPSMonitor.MaxFrametime);
            Graph16.anchorMax.Set(0.1f, 16.6f / FPSMonitor.MaxFrametime);
            if(16.6f / FPSMonitor.MaxFrametime > 1.0f)
                Graph16.sizeDelta.Set(0.0f, 0.0f);
            else
                Graph16.sizeDelta.Set(0.0f, 100.0f);

            //Update fps text colours
            //Min
            if (FPSMonitor.MinFrametime < 16.6f)
            {
                MinFPSText.color = Below16FrametimeColor;
                MinMSText.color = Below16FrametimeColor;
            }
            else if (FPSMonitor.MinFrametime < 33.3f)
            {
                MinFPSText.color = Below33FrametimeColor;
                MinMSText.color = Below33FrametimeColor;
            }
            else
            {
                MinFPSText.color = DefaultFrametimeColor;
                MinMSText.color = DefaultFrametimeColor;
            }
            //Max
            if (FPSMonitor.MaxFrametime < 16.6f)
            {
                MaxFPSText.color = Below16FrametimeColor;
                MaxMSText.color = Below16FrametimeColor;
            }
            else if (FPSMonitor.MaxFrametime < 33.3f)
            {
                MaxFPSText.color = Below33FrametimeColor;
                MaxMSText.color = Below33FrametimeColor;
            }
            else
            {
                MaxFPSText.color = DefaultFrametimeColor;
                MaxMSText.color = DefaultFrametimeColor;
            }
            //Avg
            if (FPSMonitor.AverageFrametime < 16.6f)
            {
                AvgFPSText.color = Below16FrametimeColor;
                AvgMSText.color = Below16FrametimeColor;
            }
            else if (FPSMonitor.AverageFrametime < 33.3f)
            {
                AvgFPSText.color = Below33FrametimeColor;
                AvgMSText.color = Below33FrametimeColor;
            }
            else
            {
                AvgFPSText.color = DefaultFrametimeColor;
                AvgMSText.color = DefaultFrametimeColor;
            }

            //Update fps text elements
            MinFPSText.text = (1000.0f / FPSMonitor.MinFrametime).ToString("0.00");
            MaxFPSText.text = (1000.0f / FPSMonitor.MaxFrametime).ToString("0.00");
            AvgFPSText.text = (1000.0f / FPSMonitor.AverageFrametime).ToString("0.00");
            
            MinMSText.text = FPSMonitor.MinFrametime.ToString("0.00");
            MaxMSText.text = FPSMonitor.MaxFrametime.ToString("0.00");
            AvgMSText.text = FPSMonitor.AverageFrametime.ToString("0.00");
        }
    }
}