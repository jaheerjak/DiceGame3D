using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerId = 0;
    public int currentTile = 1;
    public float moveSpeed = 4f;
    public Color playerColor = Color.white;
    public bool canBegin = false;

    private BoardManager board;
    private int Score;

    void Awake()
    {
        board = FindFirstObjectByType<BoardManager>();
    }

    public void Initialize(int startTile = 1)
    {
        currentTile = startTile;
        //transform.position = board.GetTilePosition(currentTile);
        var r = GetComponent<Renderer>();
        if (r != null) r.material.color = playerColor;
    }
    public void UpdateScore(int _score)
    {
        Score = _score;
    }
    public IEnumerator MoveBySteps(int steps)
    {
        
        int target = Mathf.Min(board.TileCount, currentTile + steps);
        while (currentTile < target)
        {
            currentTile++;
            Vector3 targetPos = board.GetTilePosition(currentTile);
            yield return SmoothMove(targetPos);
        }
        if (board.HasJumpAt(currentTile))
        {
            int dest = board.GetJumpDestination(currentTile);
            JumpType type = board.GetJumpType(currentTile);
            if (type == JumpType.Ladder)
            {
                SoundController.instance.PlayLadder();
                Score += 10;
            }
            else if (type == JumpType.Snake)
            {
                Score -= 10;
                if (Score <= 0)
                    Score = 0;
                SoundController.instance.PlaySnake();
                yield return new WaitForSeconds(0.3f);
            }
            GameManager.instance.UpdateScore(Score);
            if (dest != currentTile)
            {
                yield return new WaitForSeconds(0.3f);
                Vector3 destPos = board.GetTilePosition(dest);
                currentTile = dest;
                yield return SmoothMove(destPos);
            }
        }
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        Vector3 start = transform.position;
        float t = 0f;
        float duration = 1f / moveSpeed;
        while (t < duration)
        {
            SoundController.instance.PlayMove();
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, targetPos, t / duration);
            yield return null;
        }
        transform.position = targetPos;
    }
}
