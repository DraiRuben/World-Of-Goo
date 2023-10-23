using Cinemachine;
using System.Collections;
using UnityEngine;

public class StartingCutscene : MonoBehaviour
{
    private CinemachineVirtualCamera m_virtualCam;
    // Start is called before the first frame update
    private void Awake()
    {
        m_virtualCam = GetComponent<CinemachineVirtualCamera>();
    }
    void Start()
    {
        m_virtualCam.Priority = 11;

        StartCoroutine(StartCutscene());
    }
    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(1.5f);
        m_virtualCam.Priority = 0;
    }
}
