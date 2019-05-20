using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Purpose: Used to manage timing and speed
/// handles toggling fast and slow animation speeds
/// </summary>
public static class SpeedManager {
    static float actionDelay = 0.3f;
    public static float ActionDelay { get { return (fastAnimations) ? actionDelay * fastFactor : actionDelay; } }
    static float actionSpeed = 0.5f;
    public static float ActionSpeed { get { return (fastAnimations) ? actionSpeed * fastFactor : actionSpeed; } }
    static float headOnSpeed = 0.25f;
    public static float HeadOnSpeed { get { return headOnSpeed * ((fastAnimations) ? fastFactor : 1); } }
    static float combatDelay = 0.6f;
    public static float CombatDelay { get { return (fastAnimations) ? combatDelay * fastFactor : combatDelay; } }
    static float combatPostDelay = 1.5f;
    public static float CombatPostDelay { get { return (fastAnimations) ? combatPostDelay * fastFactor : combatPostDelay; } }
    static float combatSinking = 3f;
    public static float CombatSinking { get { return (fastAnimations) ? combatSinking * fastFactor : combatPostDelay; } }
    static float captureSpeed = 0.6f;
    public static float CaptureSpeed { get { return (fastAnimations) ? captureSpeed * fastFactor : captureSpeed; } }
    static float captureDelay = 1f;
    public static float CaptureDelay { get { return (fastAnimations) ? captureDelay * fastFactor : captureDelay; } }

    static float catapultLaunchDelay = 0.2f;
    public static float CatapultLaunchDelay { get { return (fastAnimations) ? catapultLaunchDelay * fastFactor : catapultLaunchDelay; } }

    static float catapultTravelTime = 2f;
    public static float CatapultTravelTime { get { return (fastAnimations) ? catapultTravelTime * fastFactor : catapultTravelTime; } }

    static float cameraFocusSpeed = 0.3f;
    public static float CameraFocusSpeed { get { return (fastAnimations) ? cameraFocusSpeed * fastFactor : cameraFocusSpeed; } }

    public static float fastFactor = 0.1f;
    static bool fastAnimations = false;
    static bool subPhaseFast = false;


    public static void allAnimationFast() {
        subPhaseFast = false;
        fastAnimations = true;
    }

    public static void skipSubPhase() {
        subPhaseFast = true;
        fastAnimations = true;
    }

    public static void endSubPhaseSkip() {
        if (subPhaseFast) {
            subPhaseFast = false;
            fastAnimations = false;
        }        
    }

    public static void toggleFastAnimations() {
        if (fastAnimations) {
            subPhaseFast = false;
            fastAnimations = false;
        } else {
            fastAnimations = true;
            subPhaseFast = false;
        }
    }
}
