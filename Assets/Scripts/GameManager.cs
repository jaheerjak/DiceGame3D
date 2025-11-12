using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BoardManager boardManager;
    public Transform playersParent;
    public int playerCount = 2;
    public Color[] defaultPlayerColors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };

    [Header("UI")]
    public Button diceButton;
    public Text diceResultText;
    public Text turnText;
    public GameObject StartPanel;
    public TextMeshProUGUI p_PlayerCountText;
    public GameObject CompletedPanel;

    public TextMeshProUGUI c_PlayerName;
    public TextMeshProUGUI[] players_scoreText;

    public Dice dice;

    public List<PlayerController> players = new List<PlayerController>();
    public int MaxPlayerCount = 2;
    private int currentPlayerIndex = 0;
    private bool busy = false;

    void Start()
    {      
        instance = this;
        CompletedPanel.SetActive(false);
        StartPanel.SetActive(true);

        if (diceButton != null) diceButton.onClick.AddListener(OnDiceClicked);
        UpdateUI();
    }
    public void StartButtonClicked()
    {
        SoundController.instance.PlayClick();
        StartPanel.SetActive(false);
        SetupPlayers();
        UpdateUI();

    }
    public void SetPlayerCount(int index)
    {
        SoundController.instance.PlayClick();
        if (index == 0)
        {
            if (MaxPlayerCount < 4) MaxPlayerCount++;
        }
        else
        {
            if (MaxPlayerCount > 2) MaxPlayerCount--;
        }
        if (p_PlayerCountText != null)
        {
            p_PlayerCountText.text = $"{MaxPlayerCount}P";
        }

        
    }
    void SetupPlayers()
    {
        TrimExtraPlayers();

        for (int i = 0; i < players.Count; i++)
        {
            var controller = players[i];
            if (controller == null) continue;
            controller.playerId = i + 1;
            controller.playerColor = (i < defaultPlayerColors.Length) ? defaultPlayerColors[i] : Random.ColorHSV();
            controller.Initialize(1);
            players_scoreText[i].text = $"P{i + 1}: 0";
        }            

        currentPlayerIndex = Mathf.Clamp(currentPlayerIndex, 0, Mathf.Max(players.Count - 1, 0));
    }

    void TrimExtraPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i >= MaxPlayerCount)
            {
                var scoreParent = players_scoreText[i] != null ? players_scoreText[i].transform.parent : null;
                if (scoreParent != null)
                {
                    scoreParent.gameObject.SetActive(false);
                }
                
                players[i].gameObject.SetActive(false);
            }
        }
           
    }   

    public void OnDiceClicked()
    {
        if (busy) return;
        SoundController.instance.PlayDice();
        StartCoroutine(HandleTurn());
    }

    IEnumerator HandleTurn()
    {
        busy = true;
        int rollResult = 0;
        if (dice != null)
        {
            yield return StartCoroutine(dice.PlayRollAnimation((r) => { rollResult = r; }));
        }
        else
        {
            rollResult = Random.Range(1,7);
        }

        if (diceResultText != null) diceResultText.text = rollResult.ToString();

        var player = players[currentPlayerIndex];
        yield return StartCoroutine(player.MoveBySteps(rollResult));

        if (player.currentTile >= boardManager.TileCount)
        {
            if (turnText != null) turnText.text = $"Player {currentPlayerIndex + 1} wins!";
            if (diceButton != null) diceButton.interactable = false;
            busy = false;
            GameOver(currentPlayerIndex + 1);
            yield break;
        }

        if (rollResult != 6)
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % MaxPlayerCount;
        }
        UpdateUI();
        busy = false;
    }
    public void UpdateScore(int score)
    {
        players_scoreText[currentPlayerIndex].text = $"P{currentPlayerIndex + 1}: {score}";
    }
    void UpdateUI()
    {
        if (turnText != null) turnText.text = $"Player {currentPlayerIndex + 1}'s turn";
    }
    void GameOver(int playerIndex)
    {
        CompletedPanel.SetActive(true);
        c_PlayerName.text = $"Player {playerIndex} wins!";
    }
    public void RetryClicked()
    {
        SoundController.instance.PlayClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
