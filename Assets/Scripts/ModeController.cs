using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameModes {CONTINUOUS, PREPLANNED};

public class ModeController : MonoBehaviour
{
    private GameModes currentGameMode;
    bool useYOLO;

    public Button continuousButton;
    public Button preplannedButton;

    private void Start() {

        // This sets what the buttons do when clicked. That is, the game mode is set, then the game is started.

        continuousButton.onClick.AddListener(delegate { SetGameMode(GameModes.CONTINUOUS); });
        preplannedButton.onClick.AddListener(delegate { SetGameMode(GameModes.PREPLANNED); });
        continuousButton.onClick.AddListener(delegate { StartGame(); });
        preplannedButton.onClick.AddListener(delegate { StartGame(); });
    }

    public void SetGameMode(GameModes gm) {
        currentGameMode = gm;
    }

    public GameModes GetCurGameMode() {
        return currentGameMode;
    }

    void StartGame() {
        if (GetCurGameMode() == GameModes.PREPLANNED)
            GetComponent<Switch>().SwitchToPlanningPhase();
        else if (GetCurGameMode() == GameModes.CONTINUOUS)
            GetComponent<Switch>().SwitchToEnactmentPhase();
    }

    public void SetUsingYOLO(bool usingYOLO) {
        useYOLO = usingYOLO;
    }

    public bool IsUsingYOLO() {
        return useYOLO;
    }
}
