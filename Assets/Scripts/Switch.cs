using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Switch : MonoBehaviour {
    public GameObject planningObj;
    public GameObject timelineObj;
    public GameObject enactmentObj;
    int index = 0;


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
        index = 0;
        Transform currentSlide = timelineObj.transform.GetChild(0);
        if (currentSlide == null)
            return;

        List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();
        foreach (AttributeClass ac in allAttributes) {
            if (ac.model == null)
                continue;
            Instantiate(ac.model, GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
        }

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);

    }
    public void SwitchToEnactmentPhaseNext(bool backwards) {
        int totalSlideCount = timelineObj.transform.childCount;

        if (!backwards && index < totalSlideCount - 1)
            index++;
        else if (backwards && index > 0)
            index--;
        else {
            SwitchToPlanningPhase();
            return;
        }

        List<Tracker> allSpawnedObjs = FindObjectsOfType<Tracker>().ToList();
        foreach (Tracker t in allSpawnedObjs)
            Destroy(t.gameObject);

        Transform currentSlide = timelineObj.transform.GetChild(index);

        if (currentSlide == null)
            return;

        List<AttributeClass> allAttributes = currentSlide.GetComponentsInChildren<AttributeClass>().ToList();

        foreach (AttributeClass ac in allAttributes) {
            if (ac.model == null)
                continue;
            Instantiate(ac.model, GetRandomPositionNearZero(), Quaternion.Euler(-90, 0, 0));
        }

        planningObj.SetActive(false);
        enactmentObj.SetActive(true);
    }

    Vector3 GetRandomPositionNearZero() {
        return new Vector3(Random.Range(0, 10f) - 5f, Random.Range(0, 10f) - 5f, 0);
    }
}