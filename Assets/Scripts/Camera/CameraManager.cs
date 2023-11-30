using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] GameObject framingTransposerCam;
    [SerializeField] GameObject followCam;
    [SerializeField] GameObject freeLookCam;

    public enum CameraType {FramingTransposer, Follow, FreeLook};

    private CameraType activeCameraType = CameraType.FramingTransposer;
    private CameraType lastActiveCameraType = CameraType.FramingTransposer;

    public void FreeCamActivate() {
        Cursor.lockState = CursorLockMode.Locked;
        lastActiveCameraType = activeCameraType;
        UpdateActiveCamera(CameraType.FreeLook);
    }

    public void FreeCamDeactivate() {
        Cursor.lockState = CursorLockMode.None;
        UpdateActiveCamera(lastActiveCameraType);
    }

    public void CycleCameraType() {
        switch (activeCameraType) {
            case CameraType.FramingTransposer:
                UpdateActiveCamera(CameraType.Follow);
                break;
            case CameraType.Follow:
                UpdateActiveCamera(CameraType.FreeLook);
                break;
            case CameraType.FreeLook:
                UpdateActiveCamera(CameraType.FramingTransposer);
                break;
        }
    }

    public void UpdateActiveCamera(CameraType cameraTypeToActivate) {
        activeCameraType = cameraTypeToActivate;
        switch (cameraTypeToActivate) {
            case CameraType.FramingTransposer:
                framingTransposerCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                followCam.GetComponent<CinemachineVirtualCamera>().enabled = false;
                freeLookCam.GetComponent<CinemachineFreeLook>().enabled = false;
                break;
            case CameraType.Follow:
                framingTransposerCam.GetComponent<CinemachineVirtualCamera>().enabled = false;
                followCam.GetComponent<CinemachineVirtualCamera>().enabled = true;
                freeLookCam.GetComponent<CinemachineFreeLook>().enabled = false;
                break;
            case CameraType.FreeLook:
                framingTransposerCam.GetComponent<CinemachineVirtualCamera>().enabled = false;
                followCam.GetComponent<CinemachineVirtualCamera>().enabled = false;
                freeLookCam.GetComponent<CinemachineFreeLook>().enabled = true;
                break;
        }
    }

    public void UpdateAllCameraTargets(GameObject character) {
        framingTransposerCam.GetComponent<CinemachineVirtualCamera>().Follow = character.transform;

        followCam.GetComponent<CinemachineVirtualCamera>().Follow = character.transform;
        followCam.GetComponent<CinemachineVirtualCamera>().LookAt = character.transform;

        freeLookCam.GetComponent<CinemachineFreeLook>().Follow = character.transform;
        freeLookCam.GetComponent<CinemachineFreeLook>().LookAt = character.transform;
    }
}
