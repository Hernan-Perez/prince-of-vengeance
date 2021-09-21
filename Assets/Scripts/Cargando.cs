using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Cargando : MonoBehaviour
{

    private GUIStyle estiloventana;
    private bool cargar = false;
    private float t = 0f;

    // Use this for initialization
    void Start()
    {
        estiloventana = new GUIStyle();
        estiloventana.normal.textColor = Color.white;
        estiloventana.alignment = TextAnchor.MiddleCenter;
        estiloventana.fontSize = UTIL.TextoProporcion(70);
        t = Time.time;
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnGUI()
    {
        estiloventana.fontSize = UTIL.TextoProporcion(50);
        GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), (CONFIG.idioma == 0)?("Cargando..."):("Loading..."), estiloventana);

        if (Time.time - t < 1f)
            return;

        if (!cargar)
        {
            cargar = true;
            if (CONFIG.volviendoAMenu)
            {
                CONFIG.volviendoAMenu = false;
                SceneManager.LoadScene(0);
            }
            else
            {
                SceneManager.LoadScene(3);
            }
            
            
        }

    }
}
