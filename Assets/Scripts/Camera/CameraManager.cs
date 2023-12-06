using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{

    [System.Serializable]
    public struct MyCamera {
        public GameObject Camera;
        public CameraType CameraType;
        public CameraType NextCameraType;
    }

    [Header ("Camera List")]
    [SerializeField] List<MyCamera> myCameras;
    [Header ("Follow Cam Shake Settings")]
    [SerializeField] float maxShake = .33f;
    [SerializeField] float shakeOffset = 0f;
    [Header ("Follow Cam FOV Settings")]
    [SerializeField] float FOVAtRest = 47f;
    [SerializeField] float FOVAdjustAfterSpeed = 50f;
    [SerializeField] float FOVAdjustmentRatio = 5f;
    [SerializeField] float FOVMax = 90f;

    [System.Serializable]
    public enum CameraType {FramingTransposer, FollowNormalFOV, FollowMaxFOV, FreeLook};

    private MyCamera activeCam;
    private GameObject player;
    private float playerVelocity;
    bool applyCameraShake = false;

    void Update() {
        ProcessFollowCamFOV();
        ProcessFollowCamShake();
    }

    public void ProcessFollowCamShake() {
        CinemachineVirtualCamera followCamVirtualCamera = myCameras
            .Find(cam => cam.CameraType == CameraType.FollowNormalFOV).Camera.GetComponent<CinemachineVirtualCamera>();
        if (!applyCameraShake) {
            followCamVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
            return;
        } 
        float currentFOV = followCamVirtualCamera.m_Lens.FieldOfView;
        float fovRatio = (currentFOV - FOVAtRest) / (FOVMax - FOVAtRest);
        float shakeAmount = fovRatio * maxShake;
        followCamVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shakeAmount;
        
    }

    public void ApplyCameraShake(bool applyShake) {
        applyCameraShake = applyShake;
    }

    public void ProcessFollowCamFOV() {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        playerVelocity = rb.velocity.magnitude;

        MyCamera followCam = myCameras.Find(cam => cam.CameraType == CameraType.FollowNormalFOV);
        Vector3 camPos = followCam.Camera.transform.position;
        Vector3 playerPosition = rb.position;
        // Debug.Log("camPosition: " + camPos + " playerPosition: " + playerPosition);



        CinemachineVirtualCamera followCamVirtualCamera = myCameras
            .Find(cam => cam.CameraType == CameraType.FollowNormalFOV).Camera.GetComponent<CinemachineVirtualCamera>();
        float diff = playerVelocity - FOVAdjustAfterSpeed;
        float FOVAdjustment = playerVelocity / FOVAdjustmentRatio;

        if (FOVAtRest + FOVAdjustment >= FOVMax) {
            followCamVirtualCamera.m_Lens.FieldOfView = FOVMax;
        } else {
            followCamVirtualCamera.m_Lens.FieldOfView = FOVAtRest + FOVAdjustment;
        }
    }

    void UpdatePlayerReference(GameObject obj) {
        player = obj;
    }

    public void CycleCameraType() {
        UpdateActiveCamera(activeCam.NextCameraType);
    }

    public void UpdateActiveCamera(CameraType cameraTypeToActivate) {
        activeCam = myCameras.Find(cam => cam.CameraType == cameraTypeToActivate);
        activeCam.Camera.SetActive(true);
        myCameras
            .FindAll(cam => cam.CameraType != cameraTypeToActivate)
            .ForEach(cam => cam.Camera.SetActive(false));
    }

    public void UpdateAllCameraTargets(GameObject character) {
        UpdatePlayerReference(character);
        List<CameraType> camerasWithLookAtTarget = new List<CameraType>() {CameraType.FollowNormalFOV, CameraType.FollowMaxFOV, CameraType.FreeLook};
        
        myCameras.ForEach(cam => {
            cam.Camera.GetComponent<CinemachineVirtualCameraBase>().Follow = character.transform;
            if (camerasWithLookAtTarget.Contains(cam.CameraType)) {
                cam.Camera.GetComponent<CinemachineVirtualCameraBase>().LookAt = character.transform;
            }
            if (cam.CameraType == CameraType.FreeLook) {
                cam.Camera.GetComponent<CinemachineFreeLook>().m_XAxis.Value = 0f;
            }
        });
    }

    public void UpdateTransitionTime(float time) {
        CinemachineBlendDefinition def = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, time);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CinemachineBrain>().m_DefaultBlend = def;
    }
}
