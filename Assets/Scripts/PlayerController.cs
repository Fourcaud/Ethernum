using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public LayerMask hexLayer; // Assurez-vous que ce LayerMask est configuré pour utiliser HexLayer
    public float hexWidth = 1.732f; // Largeur de l'hexagone
    public Color availableHexColor = Color.green; // Couleur pour les hexagones disponibles
    public Color defaultHexColor = Color.white; // Couleur par défaut des hexagones
    private Vector3 targetPosition;
    private int remainingMoves;

    public Text movesText; // Référence au composant Text pour afficher les mouvements restants
    public Button rollDiceButton; // Référence au bouton Roll Dice

    private Queue<Vector3> pathPositions = new Queue<Vector3>();
    private List<GameObject> availableHexes = new List<GameObject>();

    void Start()
    {
        targetPosition = transform.position;
        remainingMoves = 0;
        UpdateMovesText();

        // Ajouter l'écouteur de bouton par script
        if (rollDiceButton != null)
        {
            rollDiceButton.onClick.AddListener(RollDice);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && remainingMoves > 0)
        {
            CalculatePath();
        }

        MovePlayer();
    }

    void CalculatePath()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, hexLayer);
        if (hit.collider != null)
        {
            Vector3 destination = hit.collider.transform.position;
            pathPositions.Clear();

            // Calculate path in a straight line
            Vector3 direction = (destination - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, destination);
            int steps = Mathf.CeilToInt(distance / (hexWidth * 0.75f));

            for (int i = 1; i <= steps && i <= remainingMoves; i++)
            {
                Vector3 stepPosition = transform.position + direction * (hexWidth * 0.75f) * i;
                pathPositions.Enqueue(stepPosition);
            }
        }
    }

    void MovePlayer()
    {
        if (remainingMoves > 0 && pathPositions.Count > 0)
        {
            targetPosition = pathPositions.Peek();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                pathPositions.Dequeue();
                remainingMoves--;
                UpdateMovesText();
                HighlightAvailableHexes(); // Update highlights as the player moves
            }
        }
    }

    public void RollDice()
    {
        remainingMoves = Random.Range(1, 7); // Génère un nombre entre 1 et 6
        Debug.Log("Rolled: " + remainingMoves);
        UpdateMovesText();
        HighlightAvailableHexes();
    }

    void HighlightAvailableHexes()
    {
        ClearHighlightedHexes();

        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, remainingMoves * hexWidth * 0.75f, Vector2.zero, Mathf.Infinity, hexLayer);
        foreach (RaycastHit2D hit in hits)
        {
            if (Vector3.Distance(transform.position, hit.collider.transform.position) <= remainingMoves * hexWidth * 0.75f)
            {
                availableHexes.Add(hit.collider.gameObject);
                hit.collider.GetComponent<SpriteRenderer>().color = availableHexColor;
            }
        }
    }

    void ClearHighlightedHexes()
    {
        foreach (GameObject hex in availableHexes)
        {
            hex.GetComponent<SpriteRenderer>().color = defaultHexColor;
        }
        availableHexes.Clear();
    }

    void UpdateMovesText()
    {
        if (movesText != null)
        {
            movesText.text = "Moves: " + remainingMoves.ToString();
        }
    }
}
