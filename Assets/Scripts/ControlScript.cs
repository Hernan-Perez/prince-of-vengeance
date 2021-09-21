using UnityEngine;

#if UNITY_EDITOR
using System;   //SOLO PARA DEBUG! - es para el try catch al mover la posicion con wasd
using System.IO;
#endif

public sealed class ControlScript : MonoBehaviour {

	private Game game;
	public Texture2D[] textura;
	public Texture2D[] otrasTexturas;
	public Texture2D[] sprites;
	public Texture2D[] skills;

	private Vector2 _dragPosInicial;
	//private int _fps, _fpsParcial;
	private float _tiempoParcial;
    private string _nombreCurrentTileset;

    public string currentSlotPath = "";

    public string nombreCurrentTileset
    {
        get
        {
            return _nombreCurrentTileset;
        }
    }
    public bool touchLock;
    private AudioClip[] cancion;
    private AudioClip cancionActual;
    private int cancionElegida = 0;
    private float[] cancionVol;
    private AudioClip[] sonido;
    private AudioSource audioSonido;
    private AudioSource pasos;
    private AudioSource efectos;

    #if UNITY_EDITOR
    private bool colision = true;
    //public int currentOffsetAgregarEnemigos;
    //public int currentOffsetAgregarNPC;
    //public bool etiquetaNombreAgregada;
    private StreamWriter file;
    #endif

    // Use this for initialization
    void Start () 
	{
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        /*cancion = new AudioClip[12];
        cancion[0] = Resources.Load("musica/Bach-2Part_Invention_No8_in_F_BWV779") as AudioClip;//z1
        cancion[1] = Resources.Load("musica/cmballet") as AudioClip;//z2
        cancion[2] = Resources.Load("musica/hdn-gqd") as AudioClip;//z3
        cancion[3] = Resources.Load("musica/Bach-Little_Fugue_in_G-_BWV578") as AudioClip;//z4
        cancion[4] = Resources.Load("musica/cmveder") as AudioClip;//z5
        cancion[5] = Resources.Load("musica/mnsnt05") as AudioClip;//z6
        cancion[6] = Resources.Load("musica/etuded") as AudioClip;//z7
        cancion[7] = Resources.Load("musica/hndl_var") as AudioClip;//z8
        cancion[8] = Resources.Load("musica/fantaisie") as AudioClip;//z8 ultimo mapa (antes de que muera ultimo boss)
        cancion[9] = Resources.Load("musica/Bach-Jesu_Joy_of_Man_Desiring") as AudioClip;//z8 ultimo mapa desp de que muera ultimo boss
        cancion[10] = Resources.Load("musica/makropulos") as AudioClip;//intro a boss (general)
        cancion[11] = Resources.Load("musica/mnsnt01") as AudioClip;//intro a boss (boss final)
        */
        //cancionActual = Resources.Load("musica/Bach-2Part_Invention_No8_in_F_BWV779") as AudioClip;//z1
        cancionElegida = -1;

        audioSonido = GameObject.Find("Sonidos").GetComponent<AudioSource>();
        pasos = GameObject.Find("Pasos").GetComponent<AudioSource>();
        efectos = GameObject.Find("Efectos").GetComponent<AudioSource>();

        cancionVol = new float[12];
        cancionVol[0] = 1f;
        cancionVol[1] = 0.75f;
        cancionVol[2] = 0.75f;
        cancionVol[3] = 1f;
        cancionVol[4] = 1f;
        cancionVol[5] = 1f;
        cancionVol[6] = 1f;
        cancionVol[7] = 1f;
        cancionVol[8] = 1f;
        cancionVol[9] = 1f;
        cancionVol[10] = 0.75f;
        cancionVol[11] = 1f;

        sonido = new AudioClip[12];
        sonido[0] = Resources.Load("sonidos/click") as AudioClip;
        sonido[1] = Resources.Load("sonidos/clickWrong") as AudioClip;
        sonido[2] = Resources.Load("sonidos/t4") as AudioClip;
        sonido[3] = Resources.Load("sonidos/buffIn") as AudioClip;
        sonido[4] = Resources.Load("sonidos/buffOut") as AudioClip;
        sonido[5] = Resources.Load("sonidos/lvlUp") as AudioClip;
        sonido[6] = Resources.Load("sonidos/morir") as AudioClip;
        sonido[7] = Resources.Load("sonidos/noMorir") as AudioClip;
        sonido[8] = Resources.Load("sonidos/TormentaEspadas") as AudioClip;
        sonido[9] = Resources.Load("sonidos/corte2") as AudioClip;
        sonido[10] = Resources.Load("sonidos/hit") as AudioClip;
        sonido[11] = Resources.Load("sonidos/corte") as AudioClip;

        touchLock = false;
		_tiempoParcial = 1.0f;
        _nombreCurrentTileset = "";

        otrasTexturas = new Texture2D[60];
		otrasTexturas [0] = Resources.Load ("otros/fondomsg2") as Texture2D;
		otrasTexturas [1] = Resources.Load ("otros/Barra_vida_hud") as Texture2D;
		otrasTexturas [2] = Resources.Load ("otros/Barra_vida") as Texture2D;
		otrasTexturas [3] = Resources.Load ("otros/boton_pausa") as Texture2D;
		otrasTexturas [4] = Resources.Load ("otros/barraVidaEnemRoja") as Texture2D;
		otrasTexturas [5] = Resources.Load ("otros/barraVidaEnemGris") as Texture2D;
		otrasTexturas [6] = Resources.Load ("otros/skill_vacio") as Texture2D;
		otrasTexturas [7] = Resources.Load ("otros/globo") as Texture2D;
		otrasTexturas [8] = Resources.Load ("otros/skill_caminar") as Texture2D;
		otrasTexturas [9] = Resources.Load ("otros/skill_swordBasic") as Texture2D;
		otrasTexturas [10] = Resources.Load ("otros/skill_twist") as Texture2D;
		otrasTexturas [11] = Resources.Load ("otros/flecha_arriba") as Texture2D;
		otrasTexturas [12] = Resources.Load ("otros/flecha_abajo") as Texture2D;
		otrasTexturas [13] = Resources.Load ("otros/fuenteImagen") as Texture2D;
		otrasTexturas [14] = Resources.Load ("otros/skill_aumDmg") as Texture2D;
		otrasTexturas [15] = Resources.Load ("otros/grafExpFondo") as Texture2D;
		otrasTexturas [16] = Resources.Load ("otros/grafExpFrente") as Texture2D;
		otrasTexturas [17] = Resources.Load ("otros/pasivo_auraDePoder") as Texture2D;
		otrasTexturas [18] = Resources.Load ("otros/pasivo_voluntadInquebrantable") as Texture2D;
		otrasTexturas [19] = Resources.Load ("otros/skill_furiaCaotica") as Texture2D;
		otrasTexturas [20] = Resources.Load ("otros/skill_furiaDeVampiro") as Texture2D;
		otrasTexturas [21] = Resources.Load ("otros/skill_pielDeAcero") as Texture2D;
		otrasTexturas [22] = Resources.Load ("otros/skill_rechazoDeLaMuerte") as Texture2D;
		otrasTexturas [23] = Resources.Load ("otros/skill_milEspadas") as Texture2D;
		otrasTexturas [24] = Resources.Load ("otros/skill_corteLunar") as Texture2D;
        otrasTexturas [25] = Resources.Load("otros/FILTRO") as Texture2D;
        otrasTexturas [26] = Resources.Load("otros/cofre") as Texture2D;
        //otrasTexturas [27] = Resources.Load("otros/cofreRojo") as Texture2D;
        //otrasTexturas [28] = Resources.Load("otros/cofreVioleta") as Texture2D;
        otrasTexturas [29] = Resources.Load("otros/minimapaSimple") as Texture2D;
        otrasTexturas [30] = Resources.Load("otros/minimapa") as Texture2D;
        otrasTexturas [31] = Resources.Load("otros/punteroPersonaje") as Texture2D;
        otrasTexturas [32] = Resources.Load("otros/fade") as Texture2D;
        otrasTexturas [33] = Resources.Load("otros/signpost") as Texture2D;
        otrasTexturas [34] = Resources.Load("otros/globoInfo") as Texture2D;
        otrasTexturas [35] = Resources.Load("otros/monkS") as Texture2D;
        otrasTexturas [36] = Resources.Load("otros/buffBarnak") as Texture2D;
        otrasTexturas [37] = Resources.Load("otros/buffRukYNhar") as Texture2D;
        otrasTexturas [38] = Resources.Load("otros/buffPasarLvl") as Texture2D;
        otrasTexturas [39] = Resources.Load("otros/icoNuevaSkill") as Texture2D;
        otrasTexturas [40] = Resources.Load("otros/buffAlquimista") as Texture2D;
        otrasTexturas [41] = Resources.Load("otros/buffAlquimista2") as Texture2D;
        otrasTexturas [42] = Resources.Load("otros/Barra_vida_hud_f") as Texture2D;
        otrasTexturas [43] = Resources.Load("otros/skillMago0") as Texture2D;
        otrasTexturas [44] = Resources.Load("otros/skillMago1") as Texture2D;
        otrasTexturas [45] = Resources.Load("otros/globoShop") as Texture2D;
        otrasTexturas [46] = Resources.Load("otros/totem") as Texture2D;
        otrasTexturas [47] = Resources.Load("otros/portal") as Texture2D;
        otrasTexturas [48] = Resources.Load("otros/inventarioLleno") as Texture2D;
        otrasTexturas [49] = Resources.Load("otros/pedestral") as Texture2D;
        otrasTexturas [50] = Resources.Load("otros/skillMagoBoss0") as Texture2D;
        otrasTexturas [51] = Resources.Load("otros/skillMagoBoss1") as Texture2D;
        otrasTexturas [52] = Resources.Load("otros/skillMagoBoss2") as Texture2D;
        otrasTexturas [53] = Resources.Load("otros/skillMagoBoss3") as Texture2D;
        otrasTexturas [54] = Resources.Load("otros/skillMagoBoss4") as Texture2D;
        otrasTexturas [55] = Resources.Load("otros/skillMagoBoss5") as Texture2D;
        otrasTexturas[56] = Resources.Load("otros/fondoBoton") as Texture2D;
        otrasTexturas[57] = Resources.Load("otros/buffReyOscuridad") as Texture2D;
        otrasTexturas[58] = Resources.Load("otros/buffReyLuz") as Texture2D;
        /*otrasTexturas[33] = Resources.Load("otros/mecanismo") as Texture2D;
        otrasTexturas[34] = Resources.Load("otros/mecanismoP0") as Texture2D;
        otrasTexturas[35] = Resources.Load("otros/mecanismoP1") as Texture2D;*/

        sprites = new Texture2D[50];
		sprites [0] = Resources.Load ("pj/pj1") as Texture2D;
        sprites [1] = Resources.Load ("pj/soldado1") as Texture2D;
        sprites [2] = Resources.Load ("pj/soldado2") as Texture2D;
        sprites [3] = Resources.Load ("pj/soldado3") as Texture2D;
        sprites [4] = Resources.Load ("pj/soldadoCompleto") as Texture2D;
        sprites [5] = Resources.Load ("pj/soldadoElite1") as Texture2D;
        sprites [6] = Resources.Load ("pj/soldadoElite2") as Texture2D;
        sprites[7] = Resources.Load ("pj/npc00") as Texture2D;
		sprites [8] = Resources.Load ("pj/npc01") as Texture2D;
		sprites [9] = Resources.Load ("pj/npc02") as Texture2D;
		sprites [10] = Resources.Load ("pj/npc03") as Texture2D;
        //sprites[11] = Resources.Load("pj/soldadoImperial") as Texture2D;
        sprites[12] = Resources.Load("pj/npc04") as Texture2D;
        sprites[13] = Resources.Load("pj/npc05") as Texture2D;
        sprites[14] = Resources.Load("pj/npc06") as Texture2D;
        sprites[15] = Resources.Load("pj/npc07") as Texture2D;
        sprites[16] = Resources.Load("pj/npc08") as Texture2D;
        sprites[17] = Resources.Load("pj/bandido0") as Texture2D;
        sprites[18] = Resources.Load("pj/bandido1") as Texture2D;
        sprites[19] = Resources.Load("pj/bandido2") as Texture2D;
        sprites[20] = Resources.Load("pj/arquero1") as Texture2D;
        sprites[21] = Resources.Load("pj/arquero2") as Texture2D;
        sprites[22] = Resources.Load("pj/arqueroCompleto") as Texture2D;
        sprites[23] = Resources.Load("pj/arqueroElite3") as Texture2D;
        sprites[24] = Resources.Load("pj/barnakBoss") as Texture2D;
        //sprites[25] = Resources.Load("pj/monk") as Texture2D;
        sprites[26] = Resources.Load("pj/bossAlquimista") as Texture2D;
        sprites[27] = Resources.Load("pj/bossAsesino") as Texture2D;
        sprites[28] = Resources.Load("pj/mago0") as Texture2D;
        sprites[29] = Resources.Load("pj/mago1") as Texture2D;
        sprites[30] = Resources.Load("pj/mago2") as Texture2D;
        sprites[31] = Resources.Load("pj/mago3") as Texture2D;
        sprites[32] = Resources.Load("pj/bossRhilik") as Texture2D;
        sprites[33] = Resources.Load("pj/bossGeneral") as Texture2D;
        sprites[34] = Resources.Load("pj/bossHechicera") as Texture2D;
        sprites[35] = Resources.Load("pj/esqueleto0") as Texture2D;
        sprites[36] = Resources.Load("pj/esqueleto1") as Texture2D;
        sprites[37] = Resources.Load("pj/esqueleto2") as Texture2D;
        sprites[38] = Resources.Load("pj/bossRuk") as Texture2D;
        sprites[39] = Resources.Load("pj/bossNhar") as Texture2D;
        sprites[40] = Resources.Load("pj/capitan") as Texture2D;
        sprites[41] = Resources.Load("pj/magoJefe") as Texture2D;
        sprites[42] = Resources.Load("pj/Modrean") as Texture2D;


        skills = new Texture2D[30];
		skills[0] = Resources.Load("skills/ef01-00") as Texture2D;
		skills[1] = Resources.Load("skills/ef01-01") as Texture2D;
		skills[2] = Resources.Load("skills/ef01-02") as Texture2D;
		skills[3] = Resources.Load("skills/ef01-03") as Texture2D;
		skills[4] = Resources.Load("skills/ef02-00") as Texture2D;
		skills[5] = Resources.Load("skills/ef02-01") as Texture2D;
		skills[6] = Resources.Load("skills/ef02-02") as Texture2D;
		skills[7] = Resources.Load("skills/ef02-03") as Texture2D;
		skills[8] = Resources.Load("skills/ef02-04") as Texture2D;
		skills[9] = Resources.Load("skills/ef04-00") as Texture2D;
		skills[10] = Resources.Load("skills/ef03-00") as Texture2D;
		skills[11] = Resources.Load("skills/ef03-01") as Texture2D;
		skills[12] = Resources.Load("skills/ef03-02") as Texture2D;
		skills[13] = Resources.Load("skills/ef03-03") as Texture2D;
		skills[14] = Resources.Load("skills/ef03-04") as Texture2D;
		skills[15] = Resources.Load("skills/ef05-00") as Texture2D;
		//skills[16] = Resources.Load("skills/ef06-00") as Texture2D;
		skills[17] = Resources.Load("skills/ef07-00") as Texture2D;
		skills[18] = Resources.Load("skills/ef08-00") as Texture2D;
		skills[19] = Resources.Load("skills/ef09-00") as Texture2D;

        game = new Game(this, "mapas/Z1-0", 49, 3); //INICIO
        //game = new Game(this, "mapas/Z2-0", 24, 1);
        //game = new Game(this, "mapas/Z2-7", 9, 6);
        //game = new Game(this, "mapas/Z8-13", 50, 13);
        //game = new Game(this, "mapas/Z3-2", 33, 20);

        //PlayMusica(0);

        /*
#if !UNITY_EDITOR
        this.GetComponent<AudioSource>().PlayOneShot(musica, 0.75f);
#endif*/
    }
	
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            game.hud.Pausar();
        }
    }

    void Update()
    {
        
        _tiempoParcial += Time.deltaTime;

        if (_tiempoParcial >= 1.0f)     //esto es para que los fps se actualizen una vez por segundo y no cada instante, que sino no se entiende un carajo
        {
            _tiempoParcial = 0.0f;
            //_fps = _fpsParcial;
            //_fpsParcial = 1;    //ya si paso el segundo se contaria como el primero
        }
        else
        {
            //_fpsParcial++;
        }

        if (game.currentMapa == null || !game.currentMapa.mapaCargado)
        {
            return;
        }

        if (game.hud.state == Hud.estado.ventana || game.pausa)  //esto es para que cuando toca OK en el cartel no se mueva
        {
            touchLock = true;
        }

        //PARA EL SCROLL
        if (!touchLock && Input.touchCount > 0) 
		{
			if (game.hud.TouchHud(new Vector2(Input.mousePosition.x, Input.mousePosition.y)) == -1 && game.player.Estado != EntidadCombate.estado.atacando)
			{
				if (game.player.skillElegida != 0 && !(game.player.getSkill(game.player.skillElegida -1).enCooldown))
				{
						Vector2 dirAtt = new Vector2 (Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2);
						game.player.directionVector = dirAtt.normalized;
						game.JugadorAtacar();
                        touchLock = true;
                }
				else if (game.player.skillElegida == 0)
				{
					if (game.player.Estado != EntidadCombate.estado.atacando)	//NO PUEDE MOVERSE EN EL MEDIO DE UN ATAQUE
					{
                        if (game.player.Estado != EntidadCombate.estado.caminando)
                            PlayPasos();
                        game.player.CambiarEstado(EntidadCombate.estado.caminando);
						game.SeleccionarTile (Input.mousePosition.x, Input.mousePosition.y);
					}
				}
			}
            else
            {
                touchLock = true;
            }
		} 
		else
        {
            if (Input.touchCount == 0)
            {
                touchLock = false;
            }
        }

        ///////////////////////////////////////////PARA DEMO - CONTROLES JUGABLES///////////////////////////////////////////
       /* if (!game.pausa && game.player.getEstado == EntidadCombate.estado.idle)
        {
            if ((Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && game.player.pos.y + 1 < game.currentMapa.DIMY)
            {
                game.player.directionVector = new Vector2(0, 1);
                game.player.CambiarEstado(EntidadCombate.estado.caminando);
                game.player.posicionObjetivo = new Vector2(game.player.pos.x, game.player.pos.y + 1);
            }
            else if ((Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && game.player.pos.x - 1 >= 0)
            {
                game.player.directionVector = new Vector2(-1, 0);
                game.player.CambiarEstado(EntidadCombate.estado.caminando);
                game.player.posicionObjetivo = new Vector2(game.player.pos.x - 1, game.player.pos.y);
            }
            else if ((Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && game.player.pos.y - 1 >= 0)
            {
                game.player.directionVector = new Vector2(0, -1);
                game.player.CambiarEstado(EntidadCombate.estado.caminando);
                game.player.posicionObjetivo = new Vector2(game.player.pos.x, game.player.pos.y - 1);
            }
            else if ((Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && game.player.pos.x + 1 < game.currentMapa.DIMX)
            {
                game.player.directionVector = new Vector2(1, 0);
                game.player.CambiarEstado(EntidadCombate.estado.caminando);
                game.player.posicionObjetivo = new Vector2(game.player.pos.x + 1, game.player.pos.y);
            }

            else if (Input.GetKeyDown(KeyCode.Space))
            {
                
                game.JugadorAtacar();                                                                                                                                                                        //game.JugadorAtacar();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                game.player.posicionObjetivo = new Vector2(game.player.pos.x + game.player.directionVector.x, game.player.pos.y + game.player.directionVector.y);
                if (game._npcArray != null)
                {
                    for (int i = 0; i < game._npcArray.Length; i++)
                    {
                        if ((game.player.posicionObjetivo.x == game._npcArray[i].pos.x)
                            && (game.player.posicionObjetivo.y == game._npcArray[i].pos.y)
                            && (Vector2.Distance(game.player.pos, game._npcArray[i].pos) < 4f)
                            && game._npcArray[i].hayConversacion())
                        {
                            game.hud.textoConversacion = game._npcArray[i].getConversacion();
                            game.hud.state = Hud.estado.ventana;
                            game.player.posicionObjetivo = game.player.pos;
                            break;
                        }
                    }
                }
                if (game.puzzle != null)
                {
                    game.puzzle.Touch(game.player.posicionObjetivo);
                }
                return;                                                                                                                                                                                                 //game.JugadorAtacar();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!game.pausa)
            {
                game.pausa = true;
                game.hud.currentVentanaPausa = Hud.ventanaPausa.main;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            game.player.skillElegida = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            game.player.skillElegida = 1;
        }*/


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        #if UNITY_EDITOR
        
        int vel = 100;
		if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
		{
			game.player.directionVector = new Vector2(0,1);

			if ((int)game.player.pos.y != game.currentMapa.DIMY-1 && (!colision || (colision && game.currentMapa.esPosObstaculo((int)(game.player.pos.x + (game.player.pos.y + 1) * game.currentMapa.DIMX)) == false)))
			{
				game.player.microPos = new Vector2(game.player.microPos.x, game.player.microPos.y + vel);
			}

            game.player.Actualizar();
            Vector2 aux = new Vector2(game.player.pos.x, game.player.pos.y);
            try
            {
                game.CambioDeMapa();
            }
            catch (NullReferenceException ex)
            {
                Debug.Log(ex.Message);
                game.player.pos = aux;
            }
        }
		if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
			game.player.directionVector = new Vector2(-1,0);

			if ((int)game.player.pos.x != 0 && (!colision || (colision && game.currentMapa.esPosObstaculo((int)(game.player.pos.x - 1 + (game.player.pos.y) * game.currentMapa.DIMX)) == false)))
			{
                game.player.microPos = new Vector2(game.player.microPos.x - vel, game.player.microPos.y);
            }
			
			game.player.Actualizar();
            game.CambioDeMapa();
        }
		if (Input.GetKey(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
		{
			game.player.directionVector = new Vector2(0,-1);

			if ((int)game.player.pos.y != 0 && (!colision || (colision && game.currentMapa.esPosObstaculo((int)(game.player.pos.x + (game.player.pos.y - 1) * game.currentMapa.DIMX)) == false)))
			{
                game.player.microPos = new Vector2(game.player.microPos.x, game.player.microPos.y - vel);
            }

            game.player.Actualizar();
            game.CambioDeMapa();
        }
		if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
			game.player.directionVector = new Vector2(1,0);
			if ((int)game.player.pos.x != game.currentMapa.DIMX - 1 && (!colision || (colision && game.currentMapa.esPosObstaculo((int)(game.player.pos.x + 1 + (game.player.pos.y) * game.currentMapa.DIMX)) == false)))
			{
                game.player.microPos = new Vector2(game.player.microPos.x + vel, game.player.microPos.y);
            }

            game.player.Actualizar();
            game.CambioDeMapa();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (colision)
                colision = false;
            else
                colision = true;
        }
	
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (!game.pausa)
            {
                game.hud.Pausar();
            }


        }
        
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (CONFIG.DEBUG)
				CONFIG.DEBUG = false;
			else
				CONFIG.DEBUG = true;
		}
        /*
        if (Input.GetKeyDown(KeyCode.Alpha0))   //cierra el archivo
        {
            file.Close();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))   //agrega enemigo MELEE en movimiento
        {
            if (!etiquetaNombreAgregada)
            {
                file.WriteLine(game.currentMapa.nombreMapaActual);
                etiquetaNombreAgregada = true;
            }
                
            if (UnityEngine.Random.Range(0, 2) == 0)
                file.WriteLine("eAux = new Enemigo(refControl.sprites[17], " + game.player.pos.x + ", "+ game.player.pos.y + ", 1, true, ITEMLIST.Instance.getPreset(0)); enemigoArray[" + currentOffsetAgregarEnemigos + "] = eAux;");
            else
                file.WriteLine("eAux = new Enemigo(refControl.sprites[19], " + game.player.pos.x + ", " + game.player.pos.y + ", 1, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[" + currentOffsetAgregarEnemigos + "] = eAux;");
            currentOffsetAgregarEnemigos++;
            Debug.Log("ENEMIGO MELEE, POS: " + game.player.pos);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))   //agrega enemigo ARQUERO en movimiento
        {
            if (!etiquetaNombreAgregada)
            {
                file.WriteLine(game.currentMapa.nombreMapaActual);
                etiquetaNombreAgregada = true;
            }
            file.WriteLine("eAux = new Enemigo(refControl.sprites[18], " + game.player.pos.x + ", " + game.player.pos.y + ", 1, false, ITEMLIST.Instance.getPreset(0)); enemigoArray[" + currentOffsetAgregarEnemigos + "] = eAux;");
            currentOffsetAgregarEnemigos++;
            Debug.Log("ENEMIGO ARQUERO, POS: " + game.player.pos);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))   //agrega enemigo ESTATICO
        {
            if (!etiquetaNombreAgregada)
            {
                file.WriteLine(game.currentMapa.nombreMapaActual);
                etiquetaNombreAgregada = true;
            }
            file.WriteLine("eAux = new EnemigoPasivo(refControl.otrasTexturas[26], " + game.player.pos.x + ", " + game.player.pos.y + ", 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 0f, 0f)), true, 100); enemigoArray[" + currentOffsetAgregarEnemigos + "] = eAux;");
            currentOffsetAgregarEnemigos++;
            Debug.Log("ENEMIGO ESTATICO, POS: " + game.player.pos);
        }*/

#endif
        game.Update();
	}

    void OnGUI()
    {
        //Font myFont = (Font)Resources.Load("fonts/courier");
        Font myFont = (Font)Resources.Load("fonts/BLKCHCRY");
   
        if (myFont != null)
			GUI.skin.font = myFont;
		else
			Debug.Log("FUENTE NO ENCONTRADA");

		GUIStyle estilo = new GUIStyle ();
		estilo.normal.textColor = new Color(1.0f, 1.0f, 0.0f, 0.5f);
		estilo.fontSize = 32;
		estilo.alignment = TextAnchor.MiddleCenter;

		if (game.currentMapa == null || !game.currentMapa.mapaCargado) 
		{
			estilo.normal.textColor = new Color(1.0f, 1.0f, 0.0f, 1f);
			GUI.Label (new Rect (0, 0, Screen.width, Screen.height), "LOADING...", estilo);
			return;
		}

		game.Draw (textura);
        
        //#if UNITY_EDITOR
		//GUI.Label (new Rect (0, Screen.height - 40, 200, 50), "(" + game.player.pos.x + ", " + game.player.pos.y + ")", estilo);
        //GUI.Label (new Rect (Screen.width - 200, Screen.height - 40, 200, 50),_fps + " fps", estilo);
        //GUI.Label(new Rect(Screen.width - 400, 0, 200, 50), game.currentMapa.nombreMapaActual, estilo);
        //#endif
    }

    public void CambiarTileset(string nombreTileset)
    {
        if (_nombreCurrentTileset == nombreTileset)
            return;

        TextAsset tilesetT = Resources.Load(nombreTileset) as TextAsset;  //PONER ACA EL NOMBRE DEL TILESET A CARGAR
        /*
        IMPORTANTE NO USAR la pos del tileset 76!!!!!!!!!!!!!!!!!!!
        CORRESPONDE AL CARACTER 'L' y PUEDE LLEGAR A PRODUCIR UN ERROR, (CAMBIO DE CAPA)
        DE TODAS FORMAS DEBERIA CAMBIAR EL CARACTER L POR OTRO
        */

        string tileSet = tilesetT.ToString();
        textura = new Texture2D[300];
        Texture2D aux = Resources.Load("textura/vacio") as Texture2D;
        for (int i = 0; i < 300; i++)
            textura[i] = aux;    //no es optimo pero es para evitar el warning de null texture

        {
            //LA RUTA DE LA TEXTURA ARRANCA DESPUES DE "] " y termina en \n. DESPUES HAY QUE AGREGARLE "textura/" DELANTE
            int iz = 0, nroAux = 0;
            string strAux = "";
            while (iz < tileSet.Length)
            {
                strAux = "";

                while (tileSet[iz] != ']')  //lee el numero de tile
                {
                    strAux += tileSet[iz];
                    iz++;
                }
                nroAux = int.Parse(strAux);
                strAux = "";    //libero strAux para ahora usarlo en el nombre del tile en vez del numero
                iz++;

                while (tileSet[iz] == ' ')  //avanza al comienzo del nombre del tile
                    iz++;

                while (iz < tileSet.Length && tileSet[iz] != '\r' && tileSet[iz] != '\n') //lee el nombre del tile hasta el eol
                {
                    strAux += tileSet[iz];
                    iz++;
                }
                textura[nroAux] = Resources.Load("textura/" + strAux) as Texture2D; //cargo la _textura
                iz++;
            }
        }

        _nombreCurrentTileset = nombreTileset;
    }

    public void PlaySonido(int index, float vol = 1f, bool principal = false)
    {
        if (!principal)
        {
            audioSonido.PlayOneShot(sonido[index], CONFIG.vol_sonido * vol);
        }
        else
        {
            audioSonido.clip = sonido[index];
            audioSonido.loop = true;
            audioSonido.Play();
        }
    }

    public void PausarSonido()
    {
        audioSonido.Pause();
    }

    public void ReanudarSonido()
    {
        audioSonido.UnPause();
    }

    public void StopSonido()
    {
        audioSonido.Stop();
    }

    public void cambiarVolumenSonido()
    {
        audioSonido.volume = CONFIG.vol_sonido;
    }

    public void PlayPasos()
    {
        pasos.Play();
    }

    public void StopPasos()
    {
        pasos.Stop();
    }

    public void PausarPasos()
    {
        pasos.Pause();
    }

    public void ReanudarPasos()
    {
        pasos.UnPause();
    }

    public void cambiarVolumenPasos()
    {
        pasos.volume = CONFIG.vol_sonido;
    }

    public void PlayEfectos(int index, float vol = 1f)
    {
        efectos.clip = sonido[index];
        efectos.loop = true;
        efectos.Play();
    }

    public void StopEfectos()
    {
        efectos.Stop();
    }

    public void PausarEfectos()
    {
        efectos.Pause();
    }

    public void ReanudarEfectos()
    {
        efectos.UnPause();
    }

    public void cambiarVolumenEfectos()
    {
        efectos.volume = CONFIG.vol_sonido;
    }

    public void PlayMusica(int index)
    {
        if (index == -1)
        {
            this.GetComponent<AudioSource>().Stop();
            return;
        }
        if (index == cancionElegida)
            return;
        cancionElegida = index;
        /*cancionElegida = index;
        if (this.GetComponent<AudioSource>().clip == cancion[index])
            return;*/
        //this.GetComponent<AudioSource>().volume = CONFIG.vol_musica;
        cambiarVolumenMusica(CONFIG.vol_musica);

        if (index == 0)
        {
            this.GetComponent<AudioSource>().pitch = 0.75f;
        }
        else
        {
            this.GetComponent<AudioSource>().pitch = 1f;
        }

        //this.GetComponent<AudioSource>().clip = cancion[index];

        switch (index)
        {
            case 0:
                cancionActual = Resources.Load("musica/Bach-2Part_Invention_No8_in_F_BWV779") as AudioClip;//z1
                break;
            case 1:
                cancionActual = Resources.Load("musica/cmballet") as AudioClip;//z2
                break;
            case 2:
                cancionActual = Resources.Load("musica/hdn-gqd") as AudioClip;//z3
                break;
            case 3:
                cancionActual = Resources.Load("musica/Bach-Little_Fugue_in_G-_BWV578") as AudioClip;//z4
                break;
            case 4:
                cancionActual = Resources.Load("musica/cmveder") as AudioClip;//z5
                break;
            case 5:
                cancionActual = Resources.Load("musica/mnsnt05") as AudioClip;//z6
                break;
            case 6:
                cancionActual = Resources.Load("musica/etuded") as AudioClip;//z7
                break;
            case 7:
                cancionActual = Resources.Load("musica/hndl_var") as AudioClip;//z8
                break;
            case 8:
                cancionActual = Resources.Load("musica/fantaisie") as AudioClip;//z8 ultimo mapa (antes de que muera ultimo boss)
                break;
            case 9:
                cancionActual = Resources.Load("musica/Bach-Jesu_Joy_of_Man_Desiring") as AudioClip;//z8 ultimo mapa desp de que muera ultimo boss
                break;
            case 10:
                cancionActual = Resources.Load("musica/makropulos") as AudioClip;//intro a boss (general)
                break;
            case 11:
                cancionActual = Resources.Load("musica/mnsnt01") as AudioClip;//intro a boss (boss final)
                break;
        }
        this.GetComponent<AudioSource>().clip = cancionActual;
        this.GetComponent<AudioSource>().loop = true;
        this.GetComponent<AudioSource>().Play();
    }

    public void cambiarVolumenMusica(float vol)
    {
        this.GetComponent<AudioSource>().volume = vol * cancionVol[cancionElegida];  //no pongo directamente CONFIG.vol_musica porque tambien necesito esto para el fade out
    }

    public void PausarMusica()
    {
        if (this.GetComponent<AudioSource>().clip != null)
            this.GetComponent<AudioSource>().Pause();
    }

    public void ReanudarMusica()
    {
        if (this.GetComponent<AudioSource>().clip != null)
            this.GetComponent<AudioSource>().UnPause();
    }
}
