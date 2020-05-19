using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject planningObj;
    public GameObject timelineObj;
    public GameObject enactmentObj;

    public void SwitchToPlanningPhase() // opens the inital popup to character selection
    {
        List<Tracker> allSpawnedObjs = FindObjectsOfType<Tracker>().ToList();
        foreach (Tracker t in allSpawnedObjs)
            Destroy(t.gameObject);

        enactmentObj.SetActive(false);
        planningObj.SetActive(true);
    }

    public void PlayPressed() // spawns first slide models then closes the selection screens
    {
        SwitchToEnactmentPhase();
    }

    public void StopPlayPressed() {
        SwitchToPlanningPhase();
    }

    public void SwitchToEnactmentPhase() {
        Transform currentSlide = timelineObj.transform.GetChild(0);
        if (currentSlide == null)
            return;

        List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model == null)
                continue;
            Instantiate(ac.model, GetRandomPositionNearZero(), Quaternion.identity);
        }

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
    }

    Vector3 GetRandomPositionNearZero() {
        return new Vector3(Random.Range(0, 10f) - 5f, Random.Range(0, 10f) - 5f, 0);
    }
}
