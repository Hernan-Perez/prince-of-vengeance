using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class INTRO : MonoBehaviour {

    private GUIStyle estiloBoton;
    private GUIStyle estiloventana;
    private Texture2D fondoBoton;
    private float fadeIn, fadeOut;
    private string texto1, texto2;
    private bool parte1 = true;
    private float _elapsed, _ultimoTiempo;
    private bool cargar = false;


    // Use this for initialization
    void Start () {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        estiloventana = new GUIStyle();
        estiloventana.normal.textColor = Color.white;
        estiloventana.alignment = TextAnchor.UpperCenter;
        estiloventana.fontSize = UTIL.TextoProporcion(42);

        estiloBoton = new GUIStyle("button");
        estiloBoton.fontSize = UTIL.TextoProporcion(45);    //30 antes
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        fondoBoton = Resources.Load("otros/fondoBoton") as Texture2D;
        estiloBoton.normal.background = fondoBoton;
        _ultimoTiempo = Time.time;
        fadeIn = fadeOut = 0f;

        if (CONFIG.idioma == 0)
        {
            texto1 = "Hace muchos años, cuando aún era un niño, mi familia reinaba\nsobre la ciudad de Talmyr y sus alrededores.\nMi familia era muy querida por sus habitantes.\nUn día mientras viajaba con mi familia por el bosque,\nfuimos emboscados por un grupo de bandidos liderados\npor Modrean, los cuales comenzaron a asesinar a todos,\nni siquiera nuestros guardias pudieron con ellos.\nPor orden de mi padre escapé con uno de mis tíos.\nSolo mi tío y yo sobrevivimos a ese encuentro.";
            texto2 = "Desde ese entonces Modrean y sus hombres tomaron\nel castillo de mi familia y tomaron el poder, comenzando\nun reinado de miedo y oscuridad. Yo me crié escondido en\nlos bosques, entrené día y noche con la ayuda de mi tío...\nHoy pasaron varios años ya desde ese evento desafortunado.\nEstoy preparado para tomar venganza y recuperar lo que me\npertenece. La gente del pueblo esta de mi lado y también quiere\nque termine esta época de sufrimiento. Decidieron ayudarme\nenfrentándose a los guardias en la ciudad,\nasí Modrean enviaría refuerzos, dejando las defensas\ndel castillo debilitadas.\nYo por mi parte me dirijo al castillo tomando\nun camino antiguo y olvidado...";
        }
        else
        {
            //FALTA CHEQUEAR QUE ESTE BIEN TRADUCIDO Y PONER LOS EOL
            texto1 = "Years ago when i was still a boy, my family ruled over\nthe village of Talmyr and it's surroundings.\nMi family was appreciated by it's inhabitants.\nOne day, while i was traveling with my family\nthrough the forest, we were ambushed by a\ngroup of bandits led by Modrean. They\nstarted killing everyone, even our guards couldn't\nstand a chance. By my fathers order, i fled with my uncle.\nMy uncle and me where the only ones\nwho survived to that encounter.";
            texto2 = "Since then, Modrean and their men took my\nfamily's castle and started to rule Talmyr, starting\na reign of fear and darkness. I grew up\nhidden in the forest. With the help of my uncle\ni trained day and night...\nToday it has been several years since that\nunfortunate event. Now i am ready to take revenge\nand recover what belongs to me. The people of the\nvillage are on my side and also want to end this\ntime of suffering. They decided to help me by fighting\nguards in the village, so Modrean would send more troops\nleaving castle's defense weakened. I am heading now\nto the castle using and ancient and forgotten road...";

        }
        this.GetComponent<AudioSource>().volume = CONFIG.vol_musica;
    }

    // Update is called once per frame
    void Update () {
        

    }

    private void OnGUI()
    {
        if (cargar)
        {
            SceneManager.LoadScene(2);
            return;
        }

        _elapsed = Time.time - _ultimoTiempo;
        _ultimoTiempo = Time.time;


        if (fadeIn != 2f)
        {
            fadeIn += _elapsed;
            if (fadeIn > 2f)
            {
                fadeIn = 2f;
            }
            else
                GUI.color = new Color(1f, 1f, 1f, fadeIn / 2.0f);
        }

        if (fadeOut != 0f)
        {
            fadeOut -= _elapsed;
            if (fadeOut < 0f)
            {
                fadeOut = 0f;

                if (parte1)
                {
                    parte1 = false;
                    fadeIn = 0f;
                }
                else
                {
                    //comenzar
                    cargar = true;
                }
            }
            else
            {
                GUI.color = new Color(1f, 1f, 1f, fadeOut / 2.0f);
                if (!parte1)
                {
                    this.GetComponent<AudioSource>().volume = CONFIG.vol_musica * (fadeOut/2.0f);
                }
            }
                
        }
        estiloBoton.fontSize = UTIL.TextoProporcion(45);    //30 antes
        if (parte1)
        {
            estiloventana.fontSize = UTIL.TextoProporcion(35);
            GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.9f, Screen.height * 0.8f), texto1, estiloventana);
        }
        else
        {
            estiloventana.fontSize = UTIL.TextoProporcion(35);
            GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.9f, Screen.height * 0.8f), texto2, estiloventana);
        }
        GUI.color = Color.white;

        if (fadeIn != 2f || fadeOut != 0f)
            return;

        if (GUI.Button(new Rect(Screen.width * 0.65f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), (parte1)?(CONFIG.getTexto(15)):(CONFIG.getTexto(16)), estiloBoton))
        {
            fadeOut = 2f;
        }

        if (parte1 && GUI.Button(new Rect(Screen.width * 0.15f, Screen.height * 0.8f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(14), estiloBoton))
        {
            cargar = true;
        }
    }
}
