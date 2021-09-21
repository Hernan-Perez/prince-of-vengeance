using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class CONTROL_MENU : MonoBehaviour {

    private GUIStyle estiloBoton;
    private GUIStyle estiloventana;
    private GUIStyle estiloinventario;
    private GUIStyle estiloinventario2;
    private GUIStyle estiloinventariocentrado;
    private enum VENTANA {normal, jugar, jugar2, borrar, config, ayuda, ayuda2, idioma};
    private VENTANA ventana = VENTANA.normal;

    private Texture2D fondo, fondo2, fondoBoton;
    private float factZoom = 0f;
    private bool factZoomAum = true;
    private float _elapsed, _ultimoTiempo;
    private int slotElegido = 0;

    private int lvl1, lvl2, lvl3, lvl;
    private bool modo1, modo2, modo3, modo;
    private AudioClip click;
    private float vol = 0.4f;
    private AudioSource al;
    private string fileConfig;
    private string fileSlot1, fileSlot2, fileSlot3;

    // Use this for initialization
    void Start ()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        estiloventana = new GUIStyle();
        estiloventana.normal.textColor = Color.white;
        estiloventana.alignment = TextAnchor.UpperCenter;
        estiloventana.fontSize = UTIL.TextoProporcion(80);

        estiloBoton = new GUIStyle("button");
        estiloBoton.fontSize = UTIL.TextoProporcion(45);    //30 antes
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

        estiloinventario = new GUIStyle();
        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(32);

        estiloinventario2 = new GUIStyle(estiloinventario);

        estiloinventariocentrado = new GUIStyle(estiloinventario);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        fondoBoton = Resources.Load("otros/fondoBoton") as Texture2D;
        estiloBoton.alignment = TextAnchor.MiddleCenter;
        estiloBoton.normal.background = fondoBoton;
        fondo = Resources.Load("fondo/fondoPalace2") as Texture2D;
        fondo2 = Resources.Load("otros/fondomsg2") as Texture2D;
        _ultimoTiempo = Time.time;

        fileConfig = Application.persistentDataPath + "/settings.dat";
        fileSlot1 = Application.persistentDataPath + "/s1.dat";
        fileSlot2 = Application.persistentDataPath + "/s2.dat";
        fileSlot3 = Application.persistentDataPath + "/s3.dat";

        click = Resources.Load("sonidos/click") as AudioClip;
        al = GameObject.Find("Sonidos").GetComponent<AudioSource>();
        if (!File.Exists(fileConfig))
        {
            ventana = VENTANA.idioma;
        }
        else
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileConfig, FileMode.Open)))
            {
                CONFIG.vol_sonido = br.ReadSingle();
                CONFIG.vol_musica = br.ReadSingle();
                CONFIG.idioma = br.ReadInt32();
            }
        }

        if (!File.Exists(fileSlot1))
        {
            lvl1 = -1;
        }
        else
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileSlot1, FileMode.Open)))
            {
                lvl1 = br.ReadInt32();
                modo1 = br.ReadBoolean();
            }
        }

        if (!File.Exists(fileSlot2))
        {
            lvl2 = -1;
        }
        else
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileSlot2, FileMode.Open)))
            {
                lvl2 = br.ReadInt32();
                modo2 = br.ReadBoolean();
            }
        }

        if (!File.Exists(fileSlot3))
        {
            lvl3 = -1;
        }
        else
        {
            using (BinaryReader br = new BinaryReader(File.Open(fileSlot3, FileMode.Open)))
            {
                lvl3 = br.ReadInt32();
                modo3 = br.ReadBoolean();
            }
        }

        this.GetComponent<AudioSource>().volume = CONFIG.vol_musica;

    }

    void Update()
    {
        _elapsed = Time.time - _ultimoTiempo;
        _ultimoTiempo = Time.time;

        if (factZoomAum)
        {
            factZoom += _elapsed/25.0f;
            if (factZoom > 0.25f)
            {
                factZoom = 0.25f;
                factZoomAum = false;
            }
        }
        else
        {
            factZoom -= _elapsed/25.0f;
            if (factZoom < 0f)
            {
                factZoom = 0f;
                factZoomAum = true;
            }
        }
        
    }

    void OnGUI ()
    {
        estiloventana.fontSize = UTIL.TextoProporcion(80);
        estiloBoton.fontSize = UTIL.TextoProporcion(45);    //30 antes

        GUI.DrawTexture(new Rect(0f - factZoom * Screen.width / 2f, 0f - factZoom * Screen.height / 2f, Screen.width * (1f + factZoom), Screen.height * (1f + factZoom)), fondo, ScaleMode.ScaleAndCrop);

        switch (ventana)
        {
            case VENTANA.normal:
                estiloventana.normal.textColor = new Color(0.8f, 0f, 0f);
                GUI.Label(new Rect(Screen.width * 0f, Screen.height * 0.1f, Screen.width, Screen.height * 0.1f), (CONFIG.idioma==0)?("Príncipe De La Venganza"):("Prince Of Vengeance"), estiloventana);
                estiloventana.normal.textColor = Color.white;
                if (GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.4f, Screen.width * 0.4f, Screen.height * 0.12f), CONFIG.getTexto(0), estiloBoton))
                {
                    al.PlayOneShot(click, vol * CONFIG.vol_sonido);
                    ventana = VENTANA.jugar;
                    //SceneManager.LoadScene(1);
                }

                if (GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.55f, Screen.width * 0.4f, Screen.height * 0.12f), CONFIG.getTexto(1), estiloBoton))
                {
                    al.PlayOneShot(click, vol * CONFIG.vol_sonido);
                    ventana = VENTANA.config;
                }

                if (GUI.Button(new Rect(Screen.width * 0.3f, Screen.height * 0.7f, Screen.width * 0.4f, Screen.height * 0.12f), CONFIG.getTexto(2), estiloBoton))
                {
                    al.PlayOneShot(click, vol * CONFIG.vol_sonido);
                    ventana = VENTANA.ayuda;
                }
                break;

            case VENTANA.ayuda:
                DrawMenuAyuda();
                break;

            case VENTANA.ayuda2:
                DrawMenuAyuda2();
                break;

            case VENTANA.config:
                DrawMenuConfig();
                break;

            case VENTANA.idioma:
                DrawIdioma();
                break;

            case VENTANA.jugar:
                DrawJugar();
                break;

            case VENTANA.jugar2:
                DrawJugar2();
                break;

            case VENTANA.borrar:
                DrawBorrar();
                break;

            default:
                ventana = VENTANA.normal;
                break;
        }
        
        
    }

    private void DrawIdioma()   //NO OLVIDARSE, SI NO EXITE EL ARCHIVO CONFIG SE TIENE QUE INVOCAR ESTO ANTES QUE NADA
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(32);

        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(34);

        estiloinventario2.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario2.fontSize = UTIL.TextoProporcion(24);

        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(34);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);


        GUI.Label(new Rect(0f, Screen.height * 0.25f, Screen.width * 1f, Screen.height * 0.1f), "Please select your language:", estiloinventariocentrado);

        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.4f, Screen.width * 0.25f, Screen.height * 0.1f), "English", estiloBoton))
        {
            CONFIG.idioma = 1;
            ventana = VENTANA.normal;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            using (BinaryWriter bw = new BinaryWriter(File.Open(fileConfig, FileMode.Create)))
            {
                bw.Write(CONFIG.vol_sonido);
                bw.Write(CONFIG.vol_musica);
                bw.Write(CONFIG.idioma);
            }

        }

        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.6f, Screen.width * 0.25f, Screen.height * 0.1f), "Español", estiloBoton))
        {
            CONFIG.idioma = 0;
            ventana = VENTANA.normal;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            using (BinaryWriter bw = new BinaryWriter(File.Open(fileConfig, FileMode.Create)))
            {
                bw.Write(CONFIG.vol_sonido);
                bw.Write(CONFIG.vol_musica);
                bw.Write(CONFIG.idioma);
            }
        }
    }

    private void DrawJugar()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(32);

        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(42);

        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(42);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.15f, Screen.width * 0.9f, Screen.height * 0.75f), fondo2);
        string texto;
        if (lvl1 == -1)
        {
            texto = CONFIG.getTexto(3);
        }
        else
        {
            texto = CONFIG.getTexto(154) + "1:    " + CONFIG.getTexto(27) + lvl1 + "     " + ((modo1)?(CONFIG.getTexto(26)):(CONFIG.getTexto(25)));
        }

        if (lvl1 != -1 && modo1)
        {
            estiloBoton.normal.textColor = new Color(1f, 0.4f, 0f);
        }
        else
        {
            estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        }

        if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.25f, Screen.width * 0.6f, Screen.height * 0.125f), texto, estiloBoton))
        {
            slotElegido = 0;
            lvl = lvl1;
            modo = modo1;
            ventana = VENTANA.jugar2;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
        }
        if (lvl2 == -1)
        {
            texto = CONFIG.getTexto(4);
        }
        else
        {
            texto = CONFIG.getTexto(154) + "2:    " + CONFIG.getTexto(27) + lvl2 + "     " + ((modo2) ? (CONFIG.getTexto(26)) : (CONFIG.getTexto(25)));
        }

        if (lvl2 != -1 && modo2)
        {
            estiloBoton.normal.textColor = new Color(1f, 0.4f, 0f);
        }
        else
        {
            estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        }

        if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.4f, Screen.width * 0.6f, Screen.height * 0.125f), texto, estiloBoton))
        {
            slotElegido = 1;
            lvl = lvl2;
            modo = modo2;
            ventana = VENTANA.jugar2;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
        }

        if (lvl3 == -1)
        {
            texto = CONFIG.getTexto(5);
        }
        else
        {
            texto = CONFIG.getTexto(154) + "3:    " + CONFIG.getTexto(27) + lvl3 + "     " + ((modo3) ? (CONFIG.getTexto(26)) : (CONFIG.getTexto(25)));
        }
        if (lvl3 != -1 && modo3)
        {
            estiloBoton.normal.textColor = new Color(1f, 0.4f, 0f);
        }
        else
        {
            estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        }
        if (GUI.Button(new Rect(Screen.width * 0.2f, Screen.height * 0.55f, Screen.width * 0.6f, Screen.height * 0.125f), texto, estiloBoton))
        {
            slotElegido = 2;
            lvl = lvl3;
            modo = modo3;
            ventana = VENTANA.jugar2;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
        }

        GUI.Label(new Rect(0f, Screen.height * 0.175f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(6), estiloinventariocentrado);

        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.75f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            ventana = VENTANA.normal;
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
        }
    }

    private void DrawJugar2()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(32);
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(42);

        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(42);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);

        //if vacio
        GUI.Label(new Rect(0f, Screen.height * 0.225f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(8), estiloinventariocentrado);
        string texto;

        if (lvl == -1)
        {
            texto = ((CONFIG.idioma == 0) ? ("Ranura " + (slotElegido + 1) + ":\nPartida Nueva") : ("Slot " + (slotElegido + 1) + ":\nNew Game"));
        }
        else
        {
            texto = CONFIG.getTexto(154) + (slotElegido+1) + ":\n" + CONFIG.getTexto(27) + lvl + "\n" + ((modo) ? (CONFIG.getTexto(26)) : (CONFIG.getTexto(25)));
        }


        GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width * 1f, Screen.height * 0.1f), texto , estiloinventariocentrado);

        if (GUI.Button(new Rect(Screen.width * 0.16f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(8), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            VALORES.currentSlot = "";
            VALORES.partidaNueva = false;
            switch (slotElegido)
            {
                case 0:
                    VALORES.currentSlot = fileSlot1;
                    if (lvl1 == -1)
                    {
                        VALORES.partidaNueva = true;
                    }
                    break;
                case 1:
                    VALORES.currentSlot = fileSlot2;
                    if (lvl2 == -1)
                    {
                        VALORES.partidaNueva = true;
                    }
                    break;
                case 2:
                    VALORES.currentSlot = fileSlot3;
                    if (lvl3 == -1)
                    {
                        VALORES.partidaNueva = true;
                    }
                    break;
            }


            if (VALORES.partidaNueva)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(2);
            }
        }

        if (GUI.Button(new Rect(Screen.width * 0.6f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.jugar;
        }

        estiloBoton.normal.textColor = Color.red;
        if (lvl == -1)
        {
            GUI.enabled = false;
        }
        
        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.075f), CONFIG.getTexto(9), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            if (lvl != -1)
            {
                ventana = VENTANA.borrar;
            }
        }
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        GUI.enabled = true;

    }

    private void DrawBorrar()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(32);

        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(42);

        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(42);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);

        //if vacio
        GUI.color = Color.red;
        GUI.Label(new Rect(0f, Screen.height * 0.225f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(155), estiloinventariocentrado);
        GUI.color = Color.white;
        string texto;
        texto = CONFIG.getTexto(154) + (slotElegido + 1) + ":\n" + CONFIG.getTexto(27) + lvl + "\n" + ((modo) ? (CONFIG.getTexto(26)) : (CONFIG.getTexto(25)));


        GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width * 1f, Screen.height * 0.1f), texto, estiloinventariocentrado);

        GUI.color = Color.red;
        if (GUI.Button(new Rect(Screen.width * 0.16f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(9), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            //borrar
            switch (slotElegido)
            {
                case 0:
                    lvl1 = lvl = -1;
                    File.Delete(fileSlot1);
                    break;
                case 1:
                    lvl2 = lvl = -1;
                    File.Delete(fileSlot2);
                    break;
                case 2:
                    lvl3 = lvl = -1;
                    File.Delete(fileSlot3);
                    break;
            }
            ventana = VENTANA.jugar;
        }
        GUI.color = Color.white;

        if (GUI.Button(new Rect(Screen.width * 0.6f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.jugar2;
        }

    }

    private void DrawMenuConfig()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);
        estiloBoton.fontSize = UTIL.TextoProporcion(32);

        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(34);

        estiloinventario2.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario2.fontSize = UTIL.TextoProporcion(34);
        estiloinventario2.fontSize = UTIL.TextoProporcion(24);

        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(34);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);

        GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), fondo2);

        GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(1), estiloinventariocentrado);


        GUI.skin.horizontalSlider.fixedWidth = Screen.width * 0.25f;
        GUI.skin.horizontalSliderThumb.fixedWidth = Screen.width * 0.025f;
        GUI.skin.horizontalSlider.fixedHeight = Screen.height * 0.025f;
        GUI.skin.horizontalSliderThumb.fixedHeight = Screen.height * 0.025f;

        GUI.Label(new Rect(0f, Screen.height * 0.29f, Screen.width * 0.5f, Screen.height * 0.1f), CONFIG.getTexto(10), estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.40f, Screen.width * 0.5f, Screen.height * 0.1f), CONFIG.getTexto(11), estiloinventariocentrado);

        CONFIG.vol_sonido = GUI.HorizontalSlider(new Rect(Screen.width * 0.55f, Screen.height * 0.3f, Screen.width * 0.25f, Screen.height * 0.05f), CONFIG.vol_sonido, 0f, 1f);
        CONFIG.vol_musica = GUI.HorizontalSlider(new Rect(Screen.width * 0.55f, Screen.height * 0.41f, Screen.width * 0.25f, Screen.height * 0.05f), CONFIG.vol_musica, 0f, 1f);

        this.GetComponent<AudioSource>().volume = CONFIG.vol_musica;

        GUI.Label(new Rect(0f, Screen.height * 0.51f, Screen.width * 0.5f, Screen.height * 0.1f), CONFIG.getTexto(12), estiloinventariocentrado);

        if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.51f, Screen.width * 0.25f, Screen.height * 0.1f), (CONFIG.idioma==0)?("Español"):("English"), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            CONFIG.idioma++;
            if (CONFIG.idioma > 1)
            {
                CONFIG.idioma = 0;
            }
        }

        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.normal;

            using (BinaryWriter bw = new BinaryWriter(File.Open(fileConfig, FileMode.Create)))
            {
                bw.Write(CONFIG.vol_sonido);
                bw.Write(CONFIG.vol_musica);
                bw.Write(CONFIG.idioma);
            }
        }
    }

    private void DrawMenuAyuda()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(32);

        estiloinventario.fontSize = UTIL.TextoProporcion(42);
        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(42);

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);

        GUI.Label(new Rect(0f, Screen.height * 0.3f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(13), estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width * 1f, Screen.height * 0.1f), "2017", estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.5f, Screen.width * 1f, Screen.height * 0.1f), "Version: 1.1", estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.6f, Screen.width * 1f, Screen.height * 0.1f), "hernanperez.dev@gmail.com", estiloinventariocentrado);
        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.normal;
        }

        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.25f, Screen.width * 0.15f, Screen.height * 0.075f), CONFIG.getTexto(163), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.ayuda2;
        }
    }

    private void DrawMenuAyuda2()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);
        estiloBoton.fontSize = UTIL.TextoProporcion(32);
        estiloinventario.fontSize = UTIL.TextoProporcion(32);
        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(32);

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), fondo2);
        string text = "Luke Mehl, Johannes Sjölund,\nMarcel van de Steeg, Manuel Riecke, Thane Brimhall, Matthew Krohn,\nLori Angela Nagel, Lanea Zimmerman, Stephen Challener, Daniel Eddeland,\nShaun Williams, Joe White, Mark Weyer, Barbara Rivera, JRConway3,\nJaidynReiman, Nila122 & neo4cat6.";
        GUI.Label(new Rect(0f, Screen.height * 0.3f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(164), estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.4f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(165), estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.45f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(166) + text, estiloinventariocentrado);
        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            al.PlayOneShot(click, vol * CONFIG.vol_sonido);
            ventana = VENTANA.ayuda;
        }
    }


}
