using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;

public class ProfilerController : MonoBehaviour
{
    public bool isGameOver = false;
    public string statsText;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder drawCallsCountRecorder;

    // Variables for average draw call count
    double totalDrawCallCount = 0;
    int frameCountForDrawCall = 0;

    // Variables for average FPS
    double totalFPS = 0;
    int frameCountForFPS = 0;
    float currentFPS = 0;

    // Variables for average frame time
    double totalFrameTime = 0;
    int frameCountForFrameTime = 0;

    // Timer for updating FPS display
    float fpsUpdateTimer = 0f;
    const float fpsUpdateInterval = 0.1f;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samplesCount;

        return r;
    }

    double GetAverageDrawCallCount()
    {
        if (frameCountForDrawCall == 0)
            return 0;
        return totalDrawCallCount / frameCountForDrawCall;
    }

    double GetAverageFPS()
    {
        if (frameCountForFPS == 0)
            return 0;
        return totalFPS / frameCountForFPS;
    }

    double GetAverageFrameTime()
    {
        if (frameCountForFrameTime == 0)
            return 0;
        return totalFrameTime / frameCountForFrameTime * 1e-6; // Convert to milliseconds
    }

    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");

        GetAvailableProfilerStats.EnumerateProfilerStats();
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();
        drawCallsCountRecorder.Dispose();
    }

    void Update()
    {
        if (isGameOver)
            return;
            
        // Accumulate the draw call count and increase the frame count
        totalDrawCallCount += drawCallsCountRecorder.LastValue;
        frameCountForDrawCall++;

        // Accumulate FPS for average calculation
        float fps = 1.0f / Time.deltaTime;
        totalFPS += fps;
        frameCountForFPS++;

        // Accumulate frame time for average calculation
        double frameTime = GetRecorderFrameAverage(mainThreadTimeRecorder);
        totalFrameTime += frameTime;
        frameCountForFrameTime++;

        // Update FPS display every 0.1 seconds
        fpsUpdateTimer += Time.deltaTime;
        if (fpsUpdateTimer >= fpsUpdateInterval)
        {
            currentFPS = fps;
            fpsUpdateTimer = 0f;
        }

        // Build stats display text, including the current and average stats
        var sb = new StringBuilder(500);
        sb.AppendLine($"Frame Time: {frameTime * 1e-6:F1} ms"); // Current frame time in ms
        sb.AppendLine($"Average Frame Time: {GetAverageFrameTime():F1} ms"); // Average frame time in ms
        sb.AppendLine($"FPS (Current): {currentFPS:F1}");
        sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Draw Calls (Current): {drawCallsCountRecorder.LastValue}");
        sb.AppendLine($"Draw Calls (Average): {GetAverageDrawCallCount():F1}");
        sb.AppendLine($"FPS (Average): {GetAverageFPS():F1}");
        statsText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 30, 500, 220), statsText);
    }

    public string GetStatsSummary()
    {
        // Build the stats summary string
        var sb = new StringBuilder();
        sb.AppendLine($"Average Frame Time (CPU): {GetAverageFrameTime():F1} ms");
        sb.AppendLine($"Average FPS: {GetAverageFPS():F1}");
        sb.AppendLine($"Average Draw Calls: {GetAverageDrawCallCount():F1}");
        sb.AppendLine($"System Memory Usage: {systemMemoryRecorder.LastValue / (1024 * 1024):F1} MB");
        sb.AppendLine($"GC Memory Usage: {gcMemoryRecorder.LastValue / (1024 * 1024):F1} MB");

        return sb.ToString();
    }
}
