using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private GameObject PresionarE; 
    [SerializeField] private GameObject dialoguePanel;//Panel de UI donde aparece el dialogo
    [SerializeField] private TMP_Text dialogueText;//Texto de dialogo
    [SerializeField, TextArea(4, 6)] private string[] dialogueLines;//Lineas de dialogo que se van a mostrar

    private float typingTime = 0.05f;//Tiempo que tarda en escribirse cada letra

    private bool isPlayerInRange;//Verificamos si el player esta dentro del rango
    private bool didDialogueStart;//Verificamos si el dialogo comenzo
    private int lineIndex;//Indice de la linea actual del dialogo

    private PlayerController1 player; //Referencia al controlador del jugador

    void Update()
    {

        if (didDialogueStart && Input.GetKeyDown(KeyCode.Escape))
        {
            CerrarDialogo();
            return;
        }


        // Solo se puede interactuar si el jugador esta dentro del rango
        if (isPlayerInRange && Input.GetButtonDown("Interactuar"))
        {
            if (!didDialogueStart)
            {
                //Si no comenzo el dialogo lo iniciamos
                StartDialogue();
            }
            else if (dialogueText.text == dialogueLines[lineIndex])
            {
                //Si ya terminó de escribir la linea actual pasa a la siguiente linea
                NextDialogueLine();
            }
            else
            {
                // Si todavia se esta escribiendo, muesttra la línea completa inmediatamente
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }

    private void StartDialogue()
    {
        didDialogueStart = true;//Marcamos que empezo el dialogo
        dialoguePanel.SetActive(true);// Mostramos el panel de dialogo
        lineIndex = 0; // Empezamos desde la primera linea

        if (player != null)
        {
            //Desactiva movimiento del player
            player.enabled = false;

            //Reinicia velocidad y animacion
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            //Reiniciamos animaciones de movimiento
            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                //Paramos las animaciones de caminar y correr
                anim.SetBool("IsMoving", false);
                anim.SetBool("IsRunning", false);
            }
        }


        // Iniciamos la escritura de la primera línea
        StartCoroutine(ShowLine());
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty; // Vaciamos el texto antes de empezar

        // Vamos agregando letra por letra con un delay
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    private void NextDialogueLine()
    {
        lineIndex++; // Avanzamos al siguiente índice
        if (lineIndex < dialogueLines.Length)
        {
            // Si quedan mas lineas,sigue escribiendo
            StartCoroutine(ShowLine());
        }
        else
        {
            // Si no quedan lineas, terminamos el dialogo
            didDialogueStart = false;
            dialoguePanel.SetActive(false);// Ocultamos el panel

            if (player != null)
                player.enabled = true; //Reactiva movimiento del player
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectamos si el objeto que entra es el Player
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            PresionarE.SetActive(true);
            player = collision.GetComponent<PlayerController1>(); //Guarda la referencia
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Cuando el jugador sale del area ya no puede interactuar

        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            PresionarE.SetActive(false);
        }
    }

    private void CerrarDialogo()
    {
        StopAllCoroutines(); // Detiene la escritura de texto
        dialoguePanel.SetActive(false); // Oculta el panel
        didDialogueStart = false; // Marca que terminó
        lineIndex = 0; // Reinicia el índice

        if (player != null)
            player.enabled = true; // Reactiva el movimiento del jugador
    }

}
