using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public sealed class Game
{
    //STATIC
    private static float _tiempoTranscurridoPartida = 0.0f;
    private static float _ultimoTiempoTranscurridoPartida = 0.0f;
    public static float ultimoTiempoTranscurridoPartida
    {
        set
        {
            _ultimoTiempoTranscurridoPartida = value;
        }
        get
        {
            return _ultimoTiempoTranscurridoPartida;
        }
    }
    private static float _elapsed = 0;
    public static float elapsed
    {
        get
        {
            return _elapsed;
        }
    }
    public static float TiempoTranscurrido
    {
        get
        {
            return _tiempoTranscurridoPartida;
        }
    }

    //DEFINICIONES ENUM
    
    

    //VAR
    private ControlScript _refControl;
    public ControlScript refControl
    {
        get
        {
            return _refControl;
        }
    }
    
    private bool _pausa;  //no lo pongo en estado, porque puede estar pausado y tener abierta una ventana o no, es mas entendible asi
    public bool pausa
    {
        set
        {
            _pausa = value;
        }
        get
        {
            return _pausa;
        }
    }

    private Jugador _player;
    public Jugador player
    {
        get
        {
            return _player;
        }
    }
    public NPC[] _npcArray;
    private Enemigo[] _enemigoArray;
    public Enemigo[] enemigoArray
    {
        get
        {
            return _enemigoArray;
        }
        set
        {
            _enemigoArray = value;
        }
    }
    public Mapa currentMapa;
    private EnemigoZoneLoader enemigoZoneLoader;
    private float _acumTiempoEntidadesCerca = 2; //acumula tiempo y se reinicia cada 1 segundo, 
    private Entidad[] _entidadesCerca;
    public Hud hud;
    public Puzzle puzzle;
    public bool[] puzzleResuelto;
    private bool _peticionRedimencionarArrayEnemigos;
    public bool peticionRedimencionarArrayEnemigos
    {
        set
        {
            _peticionRedimencionarArrayEnemigos = value;
        }
        get
        {
            return _peticionRedimencionarArrayEnemigos;
        }
    }
    private float _timerActualizacionDiscreta1; //se usa para reordenar los enemigos para ver cual se dibuja antes

    public bool[] _cofresDestruidos;
    public bool[] cofresDestruidos
    {
        get
        {
            return _cofresDestruidos;
        }
    }

    //transicion
    private bool _enFadeIn = false, _enFadeOut = false;
    private float _porcentajeFade = 1f;
    private string _mapaACargar = "";
    public Vector2 _pNuevaPos; //otro parche para agregar el fade io
    public Vector2 posRespawn;
    public string mapaRespawn;
    public bool noRespawn = false;  //esto se puede poner true cuando carga cierto mapa que tiene un boss o algo, se cambia solo despues de carga el mp
    private string _tituloNuevaZona = "";
    public bool[] _bossDerrotado;
    //private int ultimoBossDerrotado = -1;
    public Boss bossActivo = null;
    public bool almaValor = false, almaPoder = false, almaSabiduria = false;

    //FUNC
    /// <summary>
    /// Constructor de Game, necesita referencia a ControlScript y un mapa para iniciar
    /// </summary>
    /// <param name="rfControl"></param>
    /// <param name="nombreMapa"></param>
    public Game(ControlScript rfControl, string nombreMapa = "mapas/mapaPueblo1", int px = 0, int py = 0)
    {
        //refControl.musica.
        posRespawn = new Vector2(px, py);
        _pNuevaPos = new Vector2(px, py);
        //Debug.Log(posRespawn);
        mapaRespawn = nombreMapa;

        _refControl = rfControl;
        Entidad.setRefGame(this);
        Entidad.setRefControl(refControl);
        Puzzle.refGame = this;
        Puzzle.refControl = refControl;
        hud = new Hud(refControl, this);
        hud.state = Hud.estado.normal;
        enemigoZoneLoader = null;

        puzzleResuelto = new bool[10];
        puzzleResuelto[0] = false;

        _peticionRedimencionarArrayEnemigos = false;
        _timerActualizacionDiscreta1 = 0.0f;
        
        
        _cofresDestruidos = new bool[30];   //son 25 cofres, cuentan desde el 1

        for (int c = 0; c < _cofresDestruidos.Length; c++)
        {
            _cofresDestruidos[c] = false;
        }

        _bossDerrotado = new bool[10];
        for (int i = 0; i < _bossDerrotado.Length; i++)
        {
            _bossDerrotado[i] = false;
        }

        //SelectorDeMapas(nombreMapa);
        
        Enemigo.setTexBarraVida(_refControl.otrasTexturas[4], _refControl.otrasTexturas[5]);
        NPC.setGloboTex(_refControl.otrasTexturas[7], _refControl.otrasTexturas[45]);
        
        _player = new Jugador(_refControl.sprites[0], px, py, 1);
        //CONFIG.MODO_EXTREMO = true;

        if (!VALORES.partidaNueva)
        {
            Cargar();
        }
        else
        {
            SelectorDeMapas(nombreMapa);
            hud.AgregarTextoConversacion(CONFIG.getTexto(159));
        }



        CONFIG.escala = 1.25f;
        hud.comprobarHabilidadSinAprender();
        
    }

    public void Update()
    {
        _elapsed = Time.time - _ultimoTiempoTranscurridoPartida;
        _ultimoTiempoTranscurridoPartida = Time.time;

        if (_pausa)
            return;
        
        _tiempoTranscurridoPartida += _elapsed;
        _acumTiempoEntidadesCerca += _elapsed;
        

        if (hud.state == Hud.estado.normal)
        {
            if (_player.Estado == EntidadCombate.estado.caminando)
                _player.MoverPersonaje();
            _player.Actualizar();

            if (_acumTiempoEntidadesCerca > 1)  //si paso mas de un segundo
            {
                _acumTiempoEntidadesCerca = 0;
                //rearma el array entidadesCerca

                armarArrayEntidadesCerca();
            }
            
            if (puzzle != null)
            {
                puzzle.Update();
            }

            if (_entidadesCerca != null)
            {
                for (int c = 0; c < _entidadesCerca.Length; c++)
                {
                    if (_entidadesCerca[c] == null)
                        continue;
                    _entidadesCerca[c].EjecutarAccionAI();
                    _entidadesCerca[c].Actualizar();
                }
            }

            if (_peticionRedimencionarArrayEnemigos == true)
            {
                _peticionRedimencionarArrayEnemigos = false;
                RedimensionarArrayEnemigos();   //lo hago de esta forma sino tengo el error de out of index si lo hago de la otra forma
            }
            
            if (_tiempoTranscurridoPartida - _timerActualizacionDiscreta1 > 1.0f) //los ordena para que se dibujen segun la posicion de Y
            {
                bool cambios = true;

                if (_enemigoArray != null)
                {
                    while (cambios)
                    {
                        cambios = false;

                        for (int i = 0; i < _enemigoArray.Length - 1; i++)
                        {
                            if (_enemigoArray[i].pos.y < _enemigoArray[i + 1].pos.y)
                            {
                                cambios = true;
                                Enemigo eaux = _enemigoArray[i + 1];
                                _enemigoArray[i + 1] = _enemigoArray[i];
                                _enemigoArray[i] = eaux;
                            }
                        }
                    }
                }
            }
        }
    }

    private void armarArrayEntidadesCerca()
    {
        int cont = 0;
        Vector2 aux;
        List<Entidad> lAux = new List<Entidad>();
        if (_enemigoArray != null)
        {
            for (int c = 0; c < _enemigoArray.Length; c++)
            {
                if (_enemigoArray[c] == null)
                    continue;
                aux = new Vector2(_enemigoArray[c].pos.x - _player.pos.x, _enemigoArray[c].pos.y - _player.pos.y);
                if (aux.magnitude <= 30 || _enemigoArray[c].clase == "boss")    //solo procesa los enemigos que esten cerca
                {
                    cont++;
                    lAux.Add(_enemigoArray[c]);
                }
            }
        }
        if (_npcArray != null)
        {
            for (int c = 0; c < _npcArray.Length; c++)
            {
                if (_npcArray[c] == null)
                    continue;
                aux = new Vector2(_npcArray[c].pos.x - _player.pos.x, _npcArray[c].pos.y - _player.pos.y);
                if (aux.magnitude <= 30)    //solo procesa los npcs que esten cerca
                {
                    cont++;
                    lAux.Add(_npcArray[c]);
                }
            }
        }

        if (cont != 0)
        {
            _entidadesCerca = new Entidad[cont];
            for (int i = 0; i < lAux.Count; i++)
            {
                _entidadesCerca[i] = lAux[i];
            }
        }
        else
        {
            _entidadesCerca = null;
        }
    }

    public void Draw(Texture2D[] textura)
    {
        currentMapa.DrawCapasInferiores((int)player.pos.x, (int)player.pos.y, (int)player.microPosAbsoluta.x, (int)player.microPosAbsoluta.y, refControl.textura);

        if (_entidadesCerca != null)
        {
            for (int i = 0; i < _entidadesCerca.Length; i++)
            {
                if (_entidadesCerca[i] != null)
                {
                    _entidadesCerca[i].Draw(_player.pos, _player.microPosAbsoluta);
                }
            }
        }
        /*
        if (_bosses != null)
        {
            for (int i = 0; i < _bosses.Length; i++)
            {
                _bosses[i].Draw(_player.pos, _player.microPosAbsoluta);
            }
        }*/

        _player.Draw(_player.pos, _player.microPosAbsoluta);

        if (puzzle != null)
        {
            puzzle.Draw();
        }

        currentMapa.DrawCapasSuperiores((int)player.pos.x, (int)player.pos.y, (int)player.microPosAbsoluta.x, (int)player.microPosAbsoluta.y, refControl.textura);

        if (_entidadesCerca != null)
        {
            for (int i = 0; i < _entidadesCerca.Length; i++)
            {
                if (_entidadesCerca[i] != null)
                {
                    _entidadesCerca[i].PostDraw(_player.pos, _player.microPosAbsoluta);
                }
            }
        }
        /*
        if (_bosses != null)
        {
            for (int i = 0; i < _bosses.Length; i++)
            {
                _bosses[i].PostDraw(_player.pos, _player.microPosAbsoluta);
            }
        }*/

        _player.PostDraw(_player.pos, _player.microPosAbsoluta);

        hud.Draw();

        /////////////////////////////////FILTRO FADE IN-OUT///////////////////////////////////////////////
        if (TransisionCambiandoMapa())
        {
            GUI.color = new Color(1f, 1f, 1f, _porcentajeFade);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), _refControl.otrasTexturas[32]);
            GUI.color = Color.white;
        }

        
    }

    public void SeleccionarTile(float mx, float my)
    {
        Vector2 centroPantalla = new Vector2(Screen.width / 2, Screen.height / 2);
        //Debug.Log ("x: " + mx + "|  y: " + my);
        int x = (int)_player.pos.x;
        int y = (int)_player.pos.y;

        for (int b = -50; (b < 50); b++)
        {
            for (int a = -50; (a < 50); a++)
            {
                if ((mx >= (centroPantalla.x - CONFIG.TAM / 2 + a * CONFIG.TAM - _player.microPosAbsoluta.x)) && (mx < (centroPantalla.x - CONFIG.TAM / 2 + (a + 1) * CONFIG.TAM - _player.microPosAbsoluta.x)) && (my >= (centroPantalla.y - CONFIG.TAM / 2 + b * CONFIG.TAM - _player.microPosAbsoluta.y)) && (my < (centroPantalla.y - CONFIG.TAM / 2 + (b + 1) * CONFIG.TAM - _player.microPosAbsoluta.y)))
                {
                    int Xa = a + x;
                    int Ya = b + y;
                    if (Xa < 0)
                        Xa = 0;
                    if (Xa >= currentMapa.DIMX)
                        Xa = currentMapa.DIMX - 1;
                    if (Ya < 0)
                        Ya = 0;
                    if (Ya >= currentMapa.DIMY)
                        Ya = currentMapa.DIMY - 1;
                    player.posicionObjetivo = new Vector2(Xa, Ya);
                    //refControl.PlayPasos();
                    /*
                     * NUEVA LOGICA PARA EL TEMA DE SHOPS Y CONVERSACIONES. NO TIENE QUE HACER COLISION SINO ES EL TEMA DE HACER CLICK JUSTO EN EL NPC
                     * Y QUE EL JUGADOR ESTE CERCA
                     * */

                    if (_npcArray != null)
                    {
                        for (int i = 0; i < _npcArray.Length; i++)
                        {
                            if (_npcArray[i] == null)
                                continue;

                            if ( (player.posicionObjetivo.x == _npcArray[i].pos.x)
                                && (player.posicionObjetivo.y == _npcArray[i].pos.y)
                                && (Vector2.Distance(_player.pos, _npcArray[i].pos) < 4f)
                                && _npcArray[i].hayConversacion() )
                            {
                                hud.textoConversacion = _npcArray[i].getConversacion();
                                hud.npcActivo = _npcArray[i];
                                hud.estadoNpc = Hud.estadoNPC.conversacion;
                                hud.state = Hud.estado.ventana;
                                player.posicionObjetivo = _player.pos;

                                _refControl.PausarSonido();
                                _refControl.PausarPasos();
                                _refControl.PausarEfectos();
                                break;
                            }
                        }
                    }
                    if (puzzle != null)
                    {
                        puzzle.Touch(player.posicionObjetivo);
                    }
                    return;
                }
            }
        }
    }

    public bool DetectarColision(Entidad e, Vector2 dirTarget)
    {
        int xx = (int)(e.pos.x + dirTarget.x);
        int yy = (int)(e.pos.y + dirTarget.y);
        int posArray = xx + yy * currentMapa.DIMX;
        if (posArray < 0 || posArray > currentMapa.MAXPOS || currentMapa.mundoObstaculos[posArray] == true)
            return true;
        return false;
    }

    public void JugadorAtacar()
    {
        _player.SkillAttack();
    }

    private bool InstanciarSkill(int tier, int nro = 1)
    {
        if (tier == 0 || tier == 1 || tier > 5)
            return false;
        switch (tier)
        {
            case 2:
                if (nro == 1)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[1]);
                }
                else if (nro == 2)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[2]);
                }
                else
                    return false;

                break;

            case 3:
                if (nro == 1)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[3]);
                }
                else if (nro == 2)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[4]);
                }
                else
                    return false;
                break;

            case 4:
                if (nro == 1)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[5]);
                }
                else if (nro == 2)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[6]);
                }
                else
                    return false;
                break;

            case 5:
                if (nro == 1)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[7]);
                }
                else if (nro == 2)
                {
                    _player.NuevaSkill(_player.skillsAuxiliar[8]);
                }
                else
                    return false;
                break;


            default:
                return false;
        }

        return true;
    }

    /// <summary>
    /// Comprueba la posicion del jugador en el mapa para ver si esta en una posicion que implique cambiar de mapa.
    /// Cambia de mapa llamando a CambioDeMapa()
    /// Devuelve verdadero si la posicion implica cambio de mapa.
    /// </summary>
    /// <returns></returns>
    public bool CambioDeMapa()
    {
        // DESACTIVO ESTO PARA DEBUG
        if (bossActivo != null && bossActivo.Estado != EntidadCombate.estado.miss)
            return false;
        
        _tituloNuevaZona = "";

        switch (currentMapa.nombreMapaActual)
        {
            case "mapas/Z1-0":
                if (_player.pos.y == 99 && _player.pos.x > 44 && _player.pos.x < 53)
                {
                    _pNuevaPos = new Vector2(_player.pos.x, 1);
                    IniciarTransicionMapa("mapas/Z1-1");
                    return true;
                }
                break;
            case "mapas/Z1-1":
                if (_player.pos.y == 0 && _player.pos.x > 44 && _player.pos.x < 53)
                {
                    _pNuevaPos = new Vector2(_player.pos.x, 98);
                    IniciarTransicionMapa("mapas/Z1-0");
                    return true;
                }
                if (_player.pos.y == 99 && _player.pos.x > 46 && _player.pos.x < 52)
                {
                    _pNuevaPos = new Vector2(_player.pos.x - 46 + 14, 1);
                    IniciarTransicionMapa("mapas/Z1-2");
                    return true;
                }
                if (_player.pos.x == 85 && _player.pos.y == 59)
                {
                    _pNuevaPos = new Vector2(2, 1);
                    IniciarTransicionMapa("mapas/Z1-1-0");
                    return true;
                }
                break;

            case "mapas/Z1-1-0":
                if (_player.pos.x == 2 && _player.pos.y == 0)
                {
                    _pNuevaPos = new Vector2(85, 58);
                    IniciarTransicionMapa("mapas/Z1-1");
                    return true;
                }
                break;
            case "mapas/Z1-2":
                if (_player.pos.y == 0 && _player.pos.x > 14 && _player.pos.x < 20)
                {
                    _pNuevaPos = new Vector2(_player.pos.x + 46 - 14, 98);
                    IniciarTransicionMapa("mapas/Z1-1");
                    return true;
                }
                if (_player.pos.y == 39 && _player.pos.x == 49)
                {
                    _pNuevaPos = new Vector2(24, 1);
                    _tituloNuevaZona = CONFIG.getTexto(86);
                    IniciarTransicionMapa("mapas/Z2-0");
                    return true;
                }
                break;
            case "mapas/Z2-0":
                if (_player.pos.y == 0 && _player.pos.x == 24)
                {
                    _pNuevaPos = new Vector2(49, 38);
                    _tituloNuevaZona = CONFIG.getTexto(87);
                    IniciarTransicionMapa("mapas/Z1-2");
                    return true;
                }
                if (_player.pos.y == 36 && _player.pos.x == 29)
                {
                    _pNuevaPos = new Vector2(29, 1);
                    IniciarTransicionMapa("mapas/Z2-1");
                    return true;
                }
                break;
            case "mapas/Z2-1":
                if (_player.pos.y == 0 && _player.pos.x == 29)
                {
                    _pNuevaPos = new Vector2(29, 35);
                    IniciarTransicionMapa("mapas/Z2-0");
                    return true;
                }
                if (_player.pos.y == 39 && _player.pos.x >= 34 && _player.pos.x <= 35)
                {
                    _pNuevaPos = new Vector2(9 + (_player.pos.x - 34), 1);
                    IniciarTransicionMapa("mapas/Z2-2");
                    return true;
                }
                if (_player.pos.y == 29 && _player.pos.x == 49)
                {
                    _pNuevaPos = new Vector2(1, 22);
                    IniciarTransicionMapa("mapas/Z2-3");
                    return true;
                }
                if (_player.pos.y == 15 && _player.pos.x == 49)
                {
                    _pNuevaPos = new Vector2(1, 9);
                    IniciarTransicionMapa("mapas/Z2-3");
                    return true;
                }
                if (_player.pos.y == 25 && _player.pos.x == 4)
                {
                    _pNuevaPos = new Vector2(48, 5);
                    IniciarTransicionMapa("mapas/Z2-4");
                    return true;
                }
                break;
            case "mapas/Z2-2":
                if (_player.pos.y == 0 && _player.pos.x >= 9 && _player.pos.x <= 10)
                {
                    _pNuevaPos = new Vector2(34 + (_player.pos.x - 9), 38);
                    IniciarTransicionMapa("mapas/Z2-1");
                    return true;
                }
                break;

            case "mapas/Z2-3":
                if (_player.pos.y == 9 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(48, 15);
                    IniciarTransicionMapa("mapas/Z2-1");
                    return true;
                }
                if (_player.pos.y == 22 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(48, 29);
                    IniciarTransicionMapa("mapas/Z2-1");
                    return true;
                }
                break;
            case "mapas/Z2-4":
                if (_player.pos.y == 5 && _player.pos.x == 49)
                {
                    _pNuevaPos = new Vector2(5, 25);
                    IniciarTransicionMapa("mapas/Z2-1");
                    return true;
                }
                if (_player.pos.y == 47 && _player.pos.x == 35)
                {
                    _pNuevaPos = new Vector2(22, 1);
                    IniciarTransicionMapa("mapas/Z2-5");
                    return true;
                }
                break;
            case "mapas/Z2-5":
                if (_player.pos.y == 0 && _player.pos.x == 22)
                {
                    _pNuevaPos = new Vector2(35, 46);
                    IniciarTransicionMapa("mapas/Z2-4");
                    return true;
                }
                if (_player.pos.y == 40 && _player.pos.x == 49)
                {
                    _pNuevaPos = new Vector2(21, 4);
                    IniciarTransicionMapa("mapas/Z2-6");
                    return true;
                }
                break;
            case "mapas/Z2-6":
                if (_player.pos.y == 4 && _player.pos.x == 20)
                {
                    _pNuevaPos = new Vector2(48, 40);
                    IniciarTransicionMapa("mapas/Z2-5");
                    return true;
                }
                if (_player.pos.y == 24 && _player.pos.x == 24)
                {
                    _pNuevaPos = new Vector2(9, 1);
                    IniciarTransicionMapa("mapas/Z2-7");
                    noRespawn = true;
                    return true;
                }
                break;

            case "mapas/Z2-7":
                if (_player.pos.y == 0 && _player.pos.x == 9)
                {
                    _pNuevaPos = new Vector2(24, 23);
                    IniciarTransicionMapa("mapas/Z2-6");
                    return true;
                }

                if (_player.pos.y == 11 && _player.pos.x == 15)
                {
                    _pNuevaPos = new Vector2(2, 1);
                    IniciarTransicionMapa("mapas/Z3-0");
                    return true;
                }
                if (_player.pos.y == 11 && _player.pos.x == 9)
                {
                    _pNuevaPos = new Vector2(4, 1);
                    IniciarTransicionMapa("mapas/Z5-0");
                    return true;
                }
                
                if (_player.pos.y == 11 && _player.pos.x == 3)
                {
                    _pNuevaPos = new Vector2(77, 1);
                    IniciarTransicionMapa("mapas/Z4-0");
                    return true;
                }
                break;

            case "mapas/Z3-0":
                if (_player.pos.y == 0 && _player.pos.x == 2)
                {
                    _pNuevaPos = new Vector2(15, 10);
                    IniciarTransicionMapa("mapas/Z2-7");
                    return true;
                }

                if (_player.pos.y == 6 && _player.pos.x == 17)
                {
                    _pNuevaPos = new Vector2(7, 50);
                    _tituloNuevaZona = CONFIG.getTexto(88);
                    IniciarTransicionMapa("mapas/Z3-1");
                    return true;
                }
                break;

            case "mapas/Z3-1":
                if (_player.pos.y == 51 && _player.pos.x == 7)
                {
                    _pNuevaPos = new Vector2(17, 7);
                    _tituloNuevaZona = CONFIG.getTexto(86);
                    IniciarTransicionMapa("mapas/Z3-0");
                    return true;
                }
                if (_player.pos.y == 84 && _player.pos.x >= 129 && _player.pos.x <= 130)
                {
                    _pNuevaPos = new Vector2(11 - (129 - _player.pos.x), 1);
                    IniciarTransicionMapa("mapas/Z3-1-0");
                    return true;
                }
                if (_player.pos.y == 104 && _player.pos.x >= 24 && _player.pos.x <= 25)
                {
                    _pNuevaPos = new Vector2(6 - (24 - _player.pos.x), 1);
                    IniciarTransicionMapa("mapas/Z3-1-1");
                    return true;
                }
                if (_player.pos.y == 89 && _player.pos.x >= 92 && _player.pos.x <= 93)
                {
                    _pNuevaPos = new Vector2(44 - (92 - _player.pos.x), 1);
                    IniciarTransicionMapa("mapas/Z3-1-1");
                    return true;
                }
                break;

            case "mapas/Z3-1-0":
                if (_player.pos.y == 0 && _player.pos.x >= 11 && _player.pos.x <= 12)
                {
                    _pNuevaPos = new Vector2(129 - (11 - _player.pos.x), 83);
                    IniciarTransicionMapa("mapas/Z3-1");
                    return true;
                }
                break;

            case "mapas/Z3-1-1":
                if (_player.pos.y == 0 && _player.pos.x >= 6 && _player.pos.x <= 7)
                {
                    _pNuevaPos = new Vector2(24 - (6 - _player.pos.x), 103);
                    IniciarTransicionMapa("mapas/Z3-1");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x >= 44 && _player.pos.x <= 45)
                {
                    _pNuevaPos = new Vector2(92 - (44 - _player.pos.x), 88);
                    IniciarTransicionMapa("mapas/Z3-1");
                    return true;
                }
                if (_player.pos.y == 43 && _player.pos.x >= 12 && _player.pos.x <= 13)
                {
                    _pNuevaPos = new Vector2(23 - (12 - _player.pos.x), 20);
                    IniciarTransicionMapa("mapas/Z3-2");
                    return true;
                }
                break;

            case "mapas/Z3-2":
                if (_player.pos.y == 21 && _player.pos.x >= 23 && _player.pos.x <= 24)
                {
                    _pNuevaPos = new Vector2(12 - (23 - _player.pos.x), 44);
                    IniciarTransicionMapa("mapas/Z3-1-1");
                    return true;
                }
                if (_player.pos.y == 84 && _player.pos.x == 52)
                {
                    _pNuevaPos = new Vector2(6, 1);
                    IniciarTransicionMapa("mapas/Z3-2-0");
                    return true;
                }
                break;

            case "mapas/Z3-2-0":
                if (_player.pos.y == 0 && _player.pos.x == 6)
                {
                    _pNuevaPos = new Vector2(52, 83);
                    IniciarTransicionMapa("mapas/Z3-2");
                    return true;
                }
                break;

            case "mapas/Z4-0":
                if (_player.pos.y == 0 && _player.pos.x == 77)
                {
                    _pNuevaPos = new Vector2(3, 10);
                    IniciarTransicionMapa("mapas/Z2-7");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x == 2)
                {
                    _pNuevaPos = new Vector2(9, 16);
                    IniciarTransicionMapa("mapas/Z4-0-0");
                    return true;
                }
                if (_player.pos.y == 9 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(78, 9);
                    _tituloNuevaZona = CONFIG.getTexto(89);
                    IniciarTransicionMapa("mapas/Z4-1");
                    return true;
                }
                break;

            case "mapas/Z4-0-0":
                if (_player.pos.y == 17 && _player.pos.x == 9)
                {
                    _pNuevaPos = new Vector2(2, 1);
                    IniciarTransicionMapa("mapas/Z4-0");
                    return true;
                }
                break;

            case "mapas/Z4-1":
                if (_player.pos.y == 9 && _player.pos.x == 79)
                {
                    _pNuevaPos = new Vector2(1, 9);
                    _tituloNuevaZona = CONFIG.getTexto(86);
                    IniciarTransicionMapa("mapas/Z4-0");
                    return true;
                }
                if (_player.pos.y == 9 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(78, 9);
                    IniciarTransicionMapa("mapas/Z4-2");
                    return true;
                }
                break;
            case "mapas/Z4-2":
                if (_player.pos.y == 9 && _player.pos.x == 79)
                {
                    _pNuevaPos = new Vector2(1, 9);
                    IniciarTransicionMapa("mapas/Z4-1");
                    return true;
                }
                if (_player.pos.y >= 8 && _player.pos.y <= 9 && _player.pos.x >= 10 && _player.pos.x <= 11)
                {
                    _pNuevaPos = new Vector2(18, 12);
                    noRespawn = true;
                    IniciarTransicionMapa("mapas/Z4-3");
                    return true;
                }
                break;
            case "mapas/Z4-3":
                if (_player.pos.y >= 12 && _player.pos.y <= 13 && _player.pos.x >= 26 && _player.pos.x <= 27)
                {
                    _pNuevaPos = new Vector2(14, 9);
                    IniciarTransicionMapa("mapas/Z4-2");
                    return true;
                }
                break;

            case "mapas/Z5-0":
                if (_player.pos.y == 0 && _player.pos.x == 4)
                {
                    _pNuevaPos = new Vector2(9, 10);
                    IniciarTransicionMapa("mapas/Z2-7");
                    return true;
                }
                if (_player.pos.y == 16 && _player.pos.x == 4)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    _tituloNuevaZona = CONFIG.getTexto(90);
                    IniciarTransicionMapa("mapas/Z5-1");
                    return true;
                }
                break;
            case "mapas/Z5-1":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(4, 15);
                    _tituloNuevaZona = CONFIG.getTexto(86);
                    IniciarTransicionMapa("mapas/Z5-0");
                    return true;
                }

                if (_player.pos.y == 36 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(10, 1);
                    noRespawn = true;
                    IniciarTransicionMapa("mapas/Z5-2");
                    return true;
                }
                if (_player.pos.y == 13 && _player.pos.x == 2)
                {
                    _pNuevaPos = new Vector2(2, 0);
                    _tituloNuevaZona = CONFIG.getTexto(91);
                    IniciarTransicionMapa("mapas/Z6-0");
                    return true;
                }
                if (_player.pos.y == 13 && _player.pos.x == 26)
                {
                    _pNuevaPos = new Vector2(26, 0);
                    _tituloNuevaZona = CONFIG.getTexto(91);
                    IniciarTransicionMapa("mapas/Z6-0");
                    return true;
                }
                break;
            case "mapas/Z5-2":
                if (_player.pos.y == 0 && _player.pos.x == 10)
                {
                    _pNuevaPos = new Vector2(14, 35);
                    _tituloNuevaZona = CONFIG.getTexto(90);
                    IniciarTransicionMapa("mapas/Z5-1");
                    return true;
                }
                if (_player.pos.y == 11 && _player.pos.x == 10)
                {
                    _pNuevaPos = new Vector2(24, 1);
                    _tituloNuevaZona = CONFIG.getTexto(92);
                    IniciarTransicionMapa("mapas/Z7-0");
                    return true;
                }
                break;
            case "mapas/Z6-0":
                if (_player.pos.y == 36 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z6-1");
                    return true;
                }
                if (_player.pos.y == 1 && _player.pos.x == 2)
                {
                    _pNuevaPos = new Vector2(2, 12);
                    _tituloNuevaZona = CONFIG.getTexto(90);
                    IniciarTransicionMapa("mapas/Z5-1");
                    return true;
                }
                if (_player.pos.y == 1 && _player.pos.x == 26)
                {
                    _pNuevaPos = new Vector2(26, 12);
                    _tituloNuevaZona = CONFIG.getTexto(90);
                    IniciarTransicionMapa("mapas/Z5-1");
                    return true;
                }
                break;

            case "mapas/Z6-1":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 35);
                    IniciarTransicionMapa("mapas/Z6-0");
                    return true;
                }
                break;

            case "mapas/Z7-0":
                if (_player.pos.y == 0 && _player.pos.x == 24)
                {
                    _pNuevaPos = new Vector2(10, 10);
                    IniciarTransicionMapa("mapas/Z5-2");
                    return true;
                }
                if (_player.pos.y == 76 && _player.pos.x == 5)
                {
                    _pNuevaPos = new Vector2(5, 1);
                    IniciarTransicionMapa("mapas/Z7-1");
                    return true;
                }
                if (_player.pos.y == 76 && _player.pos.x == 43)
                {
                    _pNuevaPos = new Vector2(43, 1);
                    IniciarTransicionMapa("mapas/Z7-1");
                    return true;
                }
                break;

            case "mapas/Z7-1":
                if (_player.pos.y == 76 && _player.pos.x == 5)
                {
                    _pNuevaPos = new Vector2(5, 1);
                    IniciarTransicionMapa("mapas/Z7-2");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x == 5)
                {
                    _pNuevaPos = new Vector2(5, 75);
                    IniciarTransicionMapa("mapas/Z7-0");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x == 43)
                {
                    _pNuevaPos = new Vector2(43, 75);
                    IniciarTransicionMapa("mapas/Z7-0");
                    return true;
                }
                if (_player.pos.y == 56 && _player.pos.x == 47)
                {
                    _pNuevaPos = new Vector2(1, 1);
                    IniciarTransicionMapa("mapas/Z7-1-0");
                    return true;
                }
                break;

            case "mapas/Z7-1-0":
                if (_player.pos.y == 0 && _player.pos.x == 1)
                {
                    _pNuevaPos = new Vector2(47, 55);
                    IniciarTransicionMapa("mapas/Z7-1");
                    return true;
                }
                if (_player.pos.y == 3 && _player.pos.x == 5)
                {
                    _pNuevaPos = new Vector2(14, 2);
                    IniciarTransicionMapa("mapas/Z7-1-1");
                    return true;
                }
                break;
            case "mapas/Z7-1-1":
                if (_player.pos.y == 3 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(5, 2);
                    IniciarTransicionMapa("mapas/Z7-1-0");
                    return true;
                }
                if (_player.pos.y == 26 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z7-1-2");
                    return true;
                }
                break;
            case "mapas/Z7-1-2":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 25);
                    IniciarTransicionMapa("mapas/Z7-1-1");
                    return true;
                }
                break;

            case "mapas/Z7-2":
                if (_player.pos.y == 0 && _player.pos.x == 5)
                {
                    _pNuevaPos = new Vector2(5, 75);
                    IniciarTransicionMapa("mapas/Z7-1");
                    return true;
                }
                if (_player.pos.y == 46 && _player.pos.x == 24)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    _tituloNuevaZona = CONFIG.getTexto(156);
                    IniciarTransicionMapa("mapas/Z8-0");
                    return true;
                }
                break;

            case "mapas/Z8-0":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(24, 45);
                    _tituloNuevaZona = CONFIG.getTexto(92);
                    IniciarTransicionMapa("mapas/Z7-2");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-1");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-4");
                    return true;
                }
                break;
            case "mapas/Z8-1":
                if (_player.pos.y == 25 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z8-2");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-0");
                    return true;
                }
                break;
            case "mapas/Z8-2":
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-3");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 24);
                    IniciarTransicionMapa("mapas/Z8-1");
                    return true;
                }
                break;
            case "mapas/Z8-3":
                if (_player.pos.y == 22 && _player.pos.x == 21)
                {
                    _pNuevaPos = new Vector2(4, 1);
                    IniciarTransicionMapa("mapas/Z8-3-0");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-2");
                    return true;
                }
                break;
            case "mapas/Z8-3-0":
                if (_player.pos.y == 0 && _player.pos.x == 4)
                {
                    _pNuevaPos = new Vector2(21, 21);
                    IniciarTransicionMapa("mapas/Z8-3");
                    return true;
                }
                break;
            case "mapas/Z8-4":
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-0");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-5");
                    return true;
                }
                break;
            case "mapas/Z8-5":
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-4");
                    return true;
                }
                if (_player.pos.y == 25 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z8-6");
                    return true;
                }
                break;
            case "mapas/Z8-6":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 24);
                    IniciarTransicionMapa("mapas/Z8-5");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-7");
                    return true;
                }
                break;
            case "mapas/Z8-7":
                if (_player.pos.y == 25 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z8-8");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-10");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-6");
                    return true;
                }
                break;
            case "mapas/Z8-8":
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 24);
                    IniciarTransicionMapa("mapas/Z8-7");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-9");
                    return true;
                }
                break;
            case "mapas/Z8-9":
                if (_player.pos.y == 12 && _player.pos.x == 28)
                {
                    _pNuevaPos = new Vector2(1, 12);
                    IniciarTransicionMapa("mapas/Z8-8");
                    return true;
                }
                break;
            case "mapas/Z8-10":
                if (_player.pos.y == 25 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 1);
                    IniciarTransicionMapa("mapas/Z8-11");
                    return true;
                }
                if (_player.pos.y == 12 && _player.pos.x == 0)
                {
                    _pNuevaPos = new Vector2(27, 12);
                    IniciarTransicionMapa("mapas/Z8-7");
                    return true;
                }
                break;
            case "mapas/Z8-11":
                if (_player.pos.y == 25 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(12, 1);
                    IniciarTransicionMapa("mapas/Z8-12");
                    return true;
                }
                if (_player.pos.y == 0 && _player.pos.x == 14)
                {
                    _pNuevaPos = new Vector2(14, 24);
                    IniciarTransicionMapa("mapas/Z8-10");
                    return true;
                }
                break;
            case "mapas/Z8-12":
                if (_player.pos.y == 0 && _player.pos.x == 12)
                {
                    _pNuevaPos = new Vector2(14, 24);
                    IniciarTransicionMapa("mapas/Z8-11");
                    return true;
                }

                if (_player.pos.y == 53 && _player.pos.x == 13)
                {
                    _pNuevaPos = new Vector2(47, 2);
                    IniciarTransicionMapa("mapas/Z8-13");
                    return true;
                }

                break;

            case "mapas/Z8-13":
                if (_player.pos.y == 0 && _player.pos.x >= 47 && _player.pos.x <= 48)
                {
                    _pNuevaPos = new Vector2(13, 52);
                    IniciarTransicionMapa("mapas/Z8-12");
                    return true;
                }

                break;
        }
        return false;
    }

    /// <summary>
    /// Carga el mapa deseado con los parametros correspondientes, carga tambien los arrays de npc y enemigos.
    /// Esta funcion se llama unicamente desde CambioDeMapa()
    /// Devuelve falso si no encuentra el mapa a cargar.
    /// </summary>
    /// <param name="nombreMapa"></param>
    /// <param name="cargarConTransicion"></param>
    /// <returns></returns>
    private bool SelectorDeMapas(string nombreMapa)
    {
        /*NO CONFUNDIR, LA POS DEL JUGADOR SE CAMBIA EN CAMBIO DE MAPAS, PORQUE A UN MISMO MAPA SE PUEDE ENTRAR POR DIFERENTES LADOS*/
        

        //agregar fadeout cuando cargartransicion = true
        Mapa mAux = new Mapa();
        if (mAux.CargarMundo(nombreMapa))
        {
            currentMapa = mAux;
        }
        else
        {
            return false;
        }
        _acumTiempoEntidadesCerca = 2;
        if (_player != null)
        {
            _player.CambiarEstado(EntidadCombate.estado.idle);
            _player.microPos = new Vector2();    //0,0
        }
        
        //borra los array actuales
        _enemigoArray = null;
        _entidadesCerca = null;         //<--------------------REPLICAR ESTO AL PROYECTO PRINCIPAL
        _npcArray = null;
        _timerActualizacionDiscreta1 = 0;
        puzzle = null;
        //_bosses = null;
        NPC aux;
        Enemigo eAux;
        BossBarnak bAux;
        BossRukYNhar bAux1;
        PuzzleC1 p;
        PuzzleC2 p2;
        PuzzleC3 p3;
        PuzzleT1 p1;
        List<Item> listaI = null;
        int index = 0;

        if (noRespawn)
        {
            noRespawn = false;
        }
        else
        {
            posRespawn = new Vector2(_pNuevaPos.x, _pNuevaPos.y);
            mapaRespawn = nombreMapa;
        }
        


        switch (nombreMapa)
        {
            case "mapas/Z1-0":
                refControl.PlayMusica(0);
                _refControl.CambiarTileset("tilesets/tilesetNormal");
                _npcArray = new NPC[25];
                aux = new NPC_INFO(53, 15, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(93));
                aux.setSolido();
                _npcArray[0] = aux;
                aux = new NPC_INFO(48, 20, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(94));
                aux.setSolido();
                _npcArray[1] = aux;
                aux = new NPC_INFO(56, 47, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(95));
                aux.setSolido();
                _npcArray[2] = aux;
                aux = new NPC_INFO(15, 74, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(96));
                aux.setSolido();
                _npcArray[3] = aux;
                aux = new NPC_INFO(52, 78, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(97));
                aux.setSolido();
                _npcArray[4] = aux;
                aux = new NPC_INFO(52, 96, _refControl.otrasTexturas[33], _refControl.otrasTexturas[34]);
                aux.AgregarConversacion(CONFIG.getTexto(98));
                aux.setSolido();
                _npcArray[5] = aux;

                if (!_bossDerrotado[0])
                {
                    aux = new NPC_INFO(49, 8, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(99));
                    _npcArray[6] = aux;
                }
                else
                    _npcArray[6] = null;

                index = 7;
                for (int j = 0; j < 3; j++)
                {
                    for (int i = 46; i < 52; i++)
                    {
                        aux = new NPC_INFO(i, j, null, null);
                        aux.AgregarConversacion(CONFIG.getTexto(100));
                        _npcArray[index] = aux;
                        index++;
                    }
                }

                _enemigoArray = new Enemigo[4];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new Enemigo(refControl.sprites[19], 54, 58, 1, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(0, refControl.otrasTexturas[26], 11, 72, 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[17], 50, 86, 1, true, ITEMLIST.Instance.getPreset(0)); enemigoArray[2] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 54, 86, 1, false, ITEMLIST.Instance.getPreset(0)); enemigoArray[3] = eAux;
                }
                else
                {
                    eAux = new Enemigo(refControl.sprites[19], 54, 58, 80, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(0, refControl.otrasTexturas[26], 11, 72, 1, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[17], 50, 86, 80, true, ITEMLIST.Instance.getPreset(8)); enemigoArray[2] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 54, 86, 80, false, ITEMLIST.Instance.getPreset(8)); enemigoArray[3] = eAux;
                }
                
                break;
            case "mapas/Z1-1":
                refControl.PlayMusica(0);
                _refControl.CambiarTileset("tilesets/tilesetNormal");
                _npcArray = new NPC[5];
                aux = new NPC_INFO(55, 71, null, null);
                aux.AgregarConversacion(CONFIG.getTexto(101));
                _npcArray[0] = aux;
                aux = new NPC_INFO(56, 71, null, null);
                aux.AgregarConversacion(CONFIG.getTexto(101));
                _npcArray[1] = aux;
                aux = new NPC_INFO(51, 89, null, null);
                aux.AgregarConversacion(CONFIG.getTexto(102));
                _npcArray[2] = aux;
                aux = new NPC_INFO(51, 90, null, null);
                aux.AgregarConversacion(CONFIG.getTexto(102));
                _npcArray[3] = aux;
                aux = new NPC_INFO(51, 91, null, null);
                aux.AgregarConversacion(CONFIG.getTexto(102));
                _npcArray[4] = aux;

                _enemigoArray = new Enemigo[16];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new Enemigo(refControl.sprites[19], 50, 36, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 56, 46, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 45, 45, 2, false, ITEMLIST.Instance.getPreset(0)); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(1, refControl.otrasTexturas[26], 81, 31, 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[3] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 51, 82, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[4] = eAux;
                    eAux = new EnemigoPasivo(2, refControl.otrasTexturas[26], 35, 73, 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[5] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 41, 73, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[6] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 47, 17, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[7] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 54, 19, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[8] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 56, 22, 2, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[9] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 60, 30, 2, false, ITEMLIST.Instance.getPreset(0)); enemigoArray[10] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 46, 41, 3, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[11] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 54, 61, 3, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[12] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 58, 63, 3, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[13] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 49, 76, 3, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[14] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 45, 80, 3, false, ITEMLIST.Instance.getPreset(0)); enemigoArray[15] = eAux;
                }
                else
                {
                    eAux = new Enemigo(refControl.sprites[19], 50, 36, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 56, 46, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 45, 45, 82, false, ITEMLIST.Instance.getPreset(8)); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(1, refControl.otrasTexturas[26], 81, 31, 1, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[3] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 51, 82, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[4] = eAux;
                    eAux = new EnemigoPasivo(2, refControl.otrasTexturas[26], 35, 73, 1, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[5] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 41, 73, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[6] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 47, 17, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[7] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 54, 19, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[8] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 56, 22, 82, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[9] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 60, 30, 82, false, ITEMLIST.Instance.getPreset(8)); enemigoArray[10] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 46, 41, 83, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[11] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 54, 61, 83, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[12] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 58, 63, 83, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[13] = eAux;
                    eAux = new Enemigo(refControl.sprites[19], 49, 76, 83, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[14] = eAux;
                    eAux = new Enemigo(refControl.sprites[18], 45, 80, 83, false, ITEMLIST.Instance.getPreset(8)); enemigoArray[15] = eAux;
                }

                    
                break;

            case "mapas/Z1-1-0":
                refControl.PlayMusica(0);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                _npcArray = new NPC[1];
                aux = new NPC(refControl.sprites[7], 2, 3, true);
                aux.AgregarConversacion(CONFIG.getTexto(103));
                _npcArray[0] = aux;
                break;
            case "mapas/Z1-2":
                refControl.PlayMusica(0);
                _refControl.CambiarTileset("tilesets/tilesetNormal");

                List<Enemigo> listAux = new List<Enemigo>();
                if (_bossDerrotado[BossBarnak.getCodBoss()])
                {
                    _enemigoArray = new Enemigo[9];
                    if (!CONFIG.MODO_EXTREMO)
                    {
                        eAux = new EnemigoPasivo(3, refControl.otrasTexturas[26], 80, 7, 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[0] = eAux;
                        eAux = new EnemigoPasivo(4, refControl.otrasTexturas[26], 9, 36, 1, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 29, 19, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[2] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 35, 21, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[3] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 35, 26, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[4] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 19, 11, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[5] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 20, 29, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[6] = eAux;
                        eAux = new Enemigo(refControl.sprites[22], 23, 32, 4, false, ITEMLIST.Instance.getPreset(0), 1); enemigoArray[7] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 29, 32, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[8] = eAux;
                        listAux.Add(eAux);
                    }
                    else
                    {
                        eAux = new EnemigoPasivo(3, refControl.otrasTexturas[26], 80, 7, 1, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[0] = eAux;
                        eAux = new EnemigoPasivo(4, refControl.otrasTexturas[26], 9, 36, 1, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 29, 19, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[2] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 35, 21, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[3] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 35, 26, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[4] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 19, 11, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[5] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 20, 29, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[6] = eAux;
                        eAux = new Enemigo(refControl.sprites[22], 23, 32, 84, false, ITEMLIST.Instance.getPreset(8), 1); enemigoArray[7] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 29, 32, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[8] = eAux;
                        listAux.Add(eAux);
                    }
                        
                    
                }
                else
                {
                    _enemigoArray = new Enemigo[12];
                    if (!CONFIG.MODO_EXTREMO)
                    {
                        eAux = new EnemigoPasivo(3, refControl.otrasTexturas[26], 80, 7, 4, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[0] = eAux;
                        eAux = new EnemigoPasivo(4, refControl.otrasTexturas[26], 9, 36, 4, ITEMLIST.Instance.getPreset(0, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 29, 19, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[2] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 35, 21, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[3] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 35, 26, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[4] = eAux;

                        eAux = new Enemigo(refControl.sprites[1], 19, 11, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[5] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 20, 29, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[6] = eAux;
                        eAux = new Enemigo(refControl.sprites[22], 23, 32, 4, false, ITEMLIST.Instance.getPreset(0), 1); enemigoArray[7] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 29, 32, 4, true, ITEMLIST.Instance.getPreset(0), 2); enemigoArray[8] = eAux;

                        eAux = new Enemigo(refControl.sprites[20], 53, 37, 4, false, ITEMLIST.Instance.getPreset(0), 1); enemigoArray[9] = eAux;
                        listAux.Add(eAux);
                        eAux = new Enemigo(refControl.sprites[21], 44, 37, 4, false, ITEMLIST.Instance.getPreset(0), 1); enemigoArray[10] = eAux;
                        listAux.Add(eAux);
                        bAux = new BossBarnak(refControl.sprites[24], 49, 37, refControl.otrasTexturas[36], -1, _bossDerrotado[BossBarnak.getCodBoss()]); _enemigoArray[11] = bAux;

                    }
                    else
                    {
                        eAux = new EnemigoPasivo(3, refControl.otrasTexturas[26], 80, 7, 4, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[0] = eAux;
                        eAux = new EnemigoPasivo(4, refControl.otrasTexturas[26], 9, 36, 4, ITEMLIST.Instance.getPreset(8, new Vector4(0f, 100f, 40f, 10f)), true, 5); enemigoArray[1] = eAux;
                        eAux = new Enemigo(refControl.sprites[1], 29, 19, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[2] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 35, 21, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[3] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 35, 26, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[4] = eAux;

                        eAux = new Enemigo(refControl.sprites[1], 19, 11, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[5] = eAux;
                        eAux = new Enemigo(refControl.sprites[2], 20, 29, 84, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[6] = eAux;
                        eAux = new Enemigo(refControl.sprites[22], 23, 32, 84, false, ITEMLIST.Instance.getPreset(8), 1); enemigoArray[7] = eAux;
                        eAux = new Enemigo(refControl.sprites[3], 29, 32, 48, true, ITEMLIST.Instance.getPreset(8), 2); enemigoArray[8] = eAux;

                        eAux = new Enemigo(refControl.sprites[20], 53, 37, 84, false, ITEMLIST.Instance.getPreset(8), 1); enemigoArray[9] = eAux;
                        listAux.Add(eAux);
                        eAux = new Enemigo(refControl.sprites[21], 44, 37, 84, false, ITEMLIST.Instance.getPreset(8), 1); enemigoArray[10] = eAux;
                        listAux.Add(eAux);
                        bAux = new BossBarnak(refControl.sprites[24], 49, 37, refControl.otrasTexturas[36], -1, _bossDerrotado[BossBarnak.getCodBoss()]); _enemigoArray[11] = bAux;

                    }

                    p = new PuzzleC1(0, bAux);
                    p.AgregarZonaCerrada(177, new Vector2(48, 39), true);
                    p.AgregarZonaCerrada(176, new Vector2(48, 40), true);
                    p.AgregarZonaCerrada(175, new Vector2(48, 41), true);
                    p.AgregarZonaCerrada(180, new Vector2(49, 39), true);
                    p.AgregarZonaCerrada(179, new Vector2(49, 40), true);
                    p.AgregarZonaCerrada(178, new Vector2(49, 41), true);
                    p.AgregarZonaCerrada(183, new Vector2(50, 39), true);
                    p.AgregarZonaCerrada(182, new Vector2(50, 40), true);
                    p.AgregarZonaCerrada(181, new Vector2(50, 41), true);
                    p.setMensajeAlActivarBoss(CONFIG.getTexto(104));
                    for (int c = 0; c < listAux.Count; c++)
                    {
                        p.agregarEnemigoAMorir(listAux[c]);
                    }
                    puzzle = p;
                }

                _npcArray = new NPC[30];

                index = 0;
                for (int j = 0; j < 6; j++)
                {
                    for (int i = 47; i < 52; i++)
                    {
                        aux = new NPC_INFO(i, j, null, null);
                        aux.AgregarConversacion(CONFIG.getTexto(100));
                        _npcArray[index] = aux;
                        index++;
                    }
                }

                break;

            case "mapas/Z2-0":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                _npcArray = new NPC[5];
                aux = new NPC(refControl.sprites[9], 44, 34, true);
                aux.AgregarConversacion(CONFIG.getTexto(105));
                _npcArray[0] = aux;

                aux = new NPC(_refControl.sprites[13], 45, 30, false, -1 , 0);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[0], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[0], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[0], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[0], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[0], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[0], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[8], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[8], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[8], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[8], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[8], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[8], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[1] = aux;

                aux = new NPC(refControl.sprites[12], 14, 19, false);
                aux.AgregarConversacion(CONFIG.getTexto(107));
                _npcArray[2] = aux;

                if (!_bossDerrotado[1])
                {
                    aux = new NPC_INFO(23, 4, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(108));
                    _npcArray[3] = aux;
                }
                else
                {
                    _npcArray[3] = null;
                }
                aux = new NPC(refControl.sprites[7], 16, 41, false, 1, 0);
                _npcArray[4] = aux;

                break;

            case "mapas/Z2-1":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 12, 14, ITEMLIST.Instance.getPreset(1), 4);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(4, 7, 40, 30));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(5, refControl.otrasTexturas[26], 17, 9, 1, ITEMLIST.Instance.getPreset(1, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[0] = eAux;

                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 92, 94, ITEMLIST.Instance.getPreset(9), 4);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(4, 7, 40, 30));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 92, true, ITEMLIST.Instance.getPreset(9), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 92, false, ITEMLIST.Instance.getPreset(9), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 92, false, ITEMLIST.Instance.getPreset(9), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 92, false, ITEMLIST.Instance.getPreset(9), 1));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(5, refControl.otrasTexturas[26], 17, 9, 1, ITEMLIST.Instance.getPreset(9, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[0] = eAux;

                }

                break;
                
            case "mapas/Z2-2":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!_bossDerrotado[1])
                {
                    _npcArray = new NPC[1];
                    aux = new NPC_INFO(9, 13, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(109));
                    _npcArray[0] = aux;
                }
                break;

            case "mapas/Z2-3":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                _enemigoArray = new Enemigo[3];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new Enemigo(refControl.sprites[1], 5, 32, 12, true, ITEMLIST.Instance.getPreset(1), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[2], 5, 15, 12, true, ITEMLIST.Instance.getPreset(1), 2); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(6, refControl.otrasTexturas[26], 15, 36, 1, ITEMLIST.Instance.getPreset(1, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[2] = eAux;

                }
                else
                {
                    eAux = new Enemigo(refControl.sprites[1], 5, 32, 92, true, ITEMLIST.Instance.getPreset(9), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[2], 5, 15, 92, true, ITEMLIST.Instance.getPreset(9), 2); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(6, refControl.otrasTexturas[26], 15, 36, 1, ITEMLIST.Instance.getPreset(9, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[2] = eAux;

                }

                _npcArray = new NPC[3];
                aux = new NPC(refControl.sprites[13], 15, 3, true);
                aux.AgregarConversacion(CONFIG.getTexto(110));
                _npcArray[0] = aux;
                aux = new NPC(refControl.sprites[10], 13, 12, false, 0, 1);
                _npcArray[1] = aux;
             
                aux = new NPC(_refControl.sprites[15], 15, 25, false, -1, 0);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[1], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[1], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[1], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[1], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[1], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[1], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[9], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[2] = aux;
                break;

            case "mapas/Z2-4":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                _enemigoArray = new Enemigo[2];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new EnemigoPasivo(7, refControl.otrasTexturas[26], 41, 18, 1, ITEMLIST.Instance.getPreset(1, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(8, refControl.otrasTexturas[26], 21, 45, 1, ITEMLIST.Instance.getPreset(1, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[1] = eAux;
                }
                else
                {
                    eAux = new EnemigoPasivo(7, refControl.otrasTexturas[26], 41, 18, 1, ITEMLIST.Instance.getPreset(9, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(8, refControl.otrasTexturas[26], 21, 45, 1, ITEMLIST.Instance.getPreset(9, new Vector4(0f, 100f, 40f, 10f)), true, 10); enemigoArray[1] = eAux;
                }
                _npcArray = new NPC[4];
                aux = new NPC(refControl.sprites[16], 47, 9, false);
                aux.AgregarConversacion(CONFIG.getTexto(111));
                _npcArray[0] = aux;
                aux = new NPC(refControl.sprites[10], 20, 7, true);
                _npcArray[1] = aux;
                aux = new NPC(refControl.sprites[7], 31, 18, false, 0, 1);
                _npcArray[2] = aux;
                aux = new NPC(refControl.sprites[8], 38, 21, false, -1, 0);
                _npcArray[3] = aux;
                break;

            case "mapas/Z2-5":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                _enemigoArray = new Enemigo[10];

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 15, 17, ITEMLIST.Instance.getPreset(1), 4);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(7, 13, 40, 29));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[4], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[4], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 95, 97, ITEMLIST.Instance.getPreset(10), 4);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(7, 13, 40, 29));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[4], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[4], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }

                _npcArray = new NPC[2];
                aux = new NPC(refControl.sprites[9], 16, 5, false, 0, 1);
                _npcArray[0] = aux;
                aux = new NPC(refControl.sprites[12], 29, 0, false, -1, 0);
                aux.AgregarConversacion(CONFIG.getTexto(112));
                _npcArray[1] = aux;
                break;

            case "mapas/Z2-6":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!puzzleResuelto[0])
                {
                    _npcArray = new NPC[1];
                    aux = new NPC_INFO(25, 19, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(113));
                    _npcArray[0] = aux;
                }
                

                p1 = new PuzzleT1(0);
                p1.AgregarControlMecanismo(new Vector2(18, 22));
                p1.AgregarControlMecanismo(new Vector2(18, 19));
                p1.AgregarControlMecanismo(new Vector2(30, 19));
                p1.AgregarControlMecanismo(new Vector2(30, 22));
                p1.AgregarRelojMecanismo(new Vector2(17, 26));
                p1.AgregarRelojMecanismo(new Vector2(21, 26));
                p1.AgregarRelojMecanismo(new Vector2(27, 26));
                p1.AgregarRelojMecanismo(new Vector2(31, 26));
                p1.AgregarZonaCerrada(70, new Vector2(23, 24), true);
                p1.AgregarZonaCerrada(69, new Vector2(23, 25), true);
                p1.AgregarZonaCerrada(68, new Vector2(23, 26), true);
                p1.AgregarZonaCerrada(73, new Vector2(24, 24), true);
                p1.AgregarZonaCerrada(72, new Vector2(24, 25), true);
                p1.AgregarZonaCerrada(71, new Vector2(24, 26), true);
                p1.AgregarZonaCerrada(76, new Vector2(25, 24), true);
                p1.AgregarZonaCerrada(75, new Vector2(25, 25), true);
                p1.AgregarZonaCerrada(74, new Vector2(25, 26), true);
                puzzle = p1;
                break;

            case "mapas/Z2-7":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!_bossDerrotado[BossRukYNhar.getCodBoss()])
                {
                    _enemigoArray = new Enemigo[2];
                    bAux1 = new BossRukYNhar(_refControl.sprites[38], 6, 7, 12, 7, refControl.otrasTexturas[37], 0, _bossDerrotado[BossRukYNhar.getCodBoss()]);
                    _enemigoArray[0] = bAux1;
                    _enemigoArray[1] = bAux1.getRefANhar();

                    p = new PuzzleC1(0, bAux1, false);
                    p.AgregarZonaCerrada(70, new Vector2(2, 11), true);
                    p.AgregarZonaCerrada(69, new Vector2(2, 12), true);
                    p.AgregarZonaCerrada(68, new Vector2(2, 13), true);
                    p.AgregarZonaCerrada(73, new Vector2(3, 11), true);
                    p.AgregarZonaCerrada(72, new Vector2(3, 12), true);
                    p.AgregarZonaCerrada(71, new Vector2(3, 13), true);
                    p.AgregarZonaCerrada(76, new Vector2(4, 11), true);
                    p.AgregarZonaCerrada(75, new Vector2(4, 12), true);
                    p.AgregarZonaCerrada(74, new Vector2(4, 13), true);

                    p.AgregarZonaCerrada(70, new Vector2(8, 11), true);
                    p.AgregarZonaCerrada(69, new Vector2(8, 12), true);
                    p.AgregarZonaCerrada(68, new Vector2(8, 13), true);
                    p.AgregarZonaCerrada(73, new Vector2(9, 11), true);
                    p.AgregarZonaCerrada(72, new Vector2(9, 12), true);
                    p.AgregarZonaCerrada(71, new Vector2(9, 13), true);
                    p.AgregarZonaCerrada(76, new Vector2(10, 11), true);
                    p.AgregarZonaCerrada(75, new Vector2(10, 12), true);
                    p.AgregarZonaCerrada(74, new Vector2(10, 13), true);

                    p.AgregarZonaCerrada(70, new Vector2(14, 11), true);
                    p.AgregarZonaCerrada(69, new Vector2(14, 12), true);
                    p.AgregarZonaCerrada(68, new Vector2(14, 13), true);
                    p.AgregarZonaCerrada(73, new Vector2(15, 11), true);
                    p.AgregarZonaCerrada(72, new Vector2(15, 12), true);
                    p.AgregarZonaCerrada(71, new Vector2(15, 13), true);
                    p.AgregarZonaCerrada(76, new Vector2(16, 11), true);
                    p.AgregarZonaCerrada(75, new Vector2(16, 12), true);
                    p.AgregarZonaCerrada(74, new Vector2(16, 13), true);
                    puzzle = p;
                }

                break;
            case "mapas/Z2-8":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                break;
            case "mapas/Z3-0":
                refControl.PlayMusica(1);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                break;
            case "mapas/Z3-1":
                _refControl.CambiarTileset("tilesets/tilesetNormal");
                refControl.PlayMusica(2);

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 20, 22, ITEMLIST.Instance.getPreset(2), 4);
                    //enemigoZoneLoader.setZonaSpawnCustom(new Rect(20, 20, 20, 20));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(61, 15, 135 - 61, 81 - 15));
                    enemigoZoneLoader.setZonaHotSpot(new Rect(34, 97, 92 - 34, 126 - 97), 20);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 4);

                    eAux = new EnemigoPasivo(9, refControl.otrasTexturas[26], 52, 80, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(10, refControl.otrasTexturas[26], 127, 28, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(11, refControl.otrasTexturas[26], 97, 88, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(12, refControl.otrasTexturas[26], 132, 127, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[3] = eAux;
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 100, 102, ITEMLIST.Instance.getPreset(10), 4);
                    //enemigoZoneLoader.setZonaSpawnCustom(new Rect(20, 20, 20, 20));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(61, 15, 135 - 61, 81 - 15));
                    enemigoZoneLoader.setZonaHotSpot(new Rect(34, 97, 92 - 34, 126 - 97), 20);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 4);

                    eAux = new EnemigoPasivo(9, refControl.otrasTexturas[26], 52, 80, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(10, refControl.otrasTexturas[26], 127, 28, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(11, refControl.otrasTexturas[26], 97, 88, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(12, refControl.otrasTexturas[26], 132, 127, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[3] = eAux;

                }
                break;
            case "mapas/Z3-1-0":
                refControl.PlayMusica(2);
                _refControl.CambiarTileset("tilesets/tilesetCueva");
                _enemigoArray = new Enemigo[6];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new Enemigo(refControl.sprites[1], 12, 5, 21, true, ITEMLIST.Instance.getPreset(2), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[20], 15, 8, 21, false, ITEMLIST.Instance.getPreset(2), 1); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[2], 16, 17, 21, true, ITEMLIST.Instance.getPreset(2), 2); enemigoArray[2] = eAux;
                    eAux = new Enemigo(refControl.sprites[21], 18, 20, 21, false, ITEMLIST.Instance.getPreset(2), 1); enemigoArray[3] = eAux;
                    eAux = new Enemigo(refControl.sprites[22], 8, 19, 21, false, ITEMLIST.Instance.getPreset(2), 1); enemigoArray[4] = eAux;
                    eAux = new EnemigoPasivo(13, refControl.otrasTexturas[26], 9, 23, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[5] = eAux;
                }
                else
                {
                    eAux = new Enemigo(refControl.sprites[1], 12, 5, 101, true, ITEMLIST.Instance.getPreset(10), 2); enemigoArray[0] = eAux;
                    eAux = new Enemigo(refControl.sprites[20], 15, 8, 101, false, ITEMLIST.Instance.getPreset(10), 1); enemigoArray[1] = eAux;
                    eAux = new Enemigo(refControl.sprites[2], 16, 17, 101, true, ITEMLIST.Instance.getPreset(10), 2); enemigoArray[2] = eAux;
                    eAux = new Enemigo(refControl.sprites[21], 18, 20, 101, false, ITEMLIST.Instance.getPreset(10), 1); enemigoArray[3] = eAux;
                    eAux = new Enemigo(refControl.sprites[22], 8, 19, 101, false, ITEMLIST.Instance.getPreset(10), 1); enemigoArray[4] = eAux;
                    eAux = new EnemigoPasivo(13, refControl.otrasTexturas[26], 9, 23, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[5] = eAux;
                }
                break;

            case "mapas/Z3-1-1":
                _refControl.CambiarTileset("tilesets/tilesetCueva");
                refControl.PlayMusica(2);
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 22, 23, ITEMLIST.Instance.getPreset(2), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 10, 46, 36));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaFree(new Rect(34, 21, 16, 9));
                    enemigoZoneLoader.setZonaFree(new Rect(0, 40, 26, 9));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 102, 103, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 10, 46, 36));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[1], 455, 254, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[2], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[3], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[20], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[21], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaFree(new Rect(34, 21, 16, 9));
                    enemigoZoneLoader.setZonaFree(new Rect(0, 40, 26, 9));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z3-2":
                _refControl.CambiarTileset("tilesets/tilesetNormal");
                refControl.PlayMusica(2);
                if (!_bossDerrotado[BossKarah.getCodBoss()])
                {
                    _npcArray = new NPC[1];

                    aux = new NPC(refControl.sprites[9], 65, 50, false);
                    aux.AgregarConversacion(CONFIG.getTexto(114));
                    _npcArray[0] = aux;

                    _enemigoArray = new Enemigo[1];
                    BossKarah bAux2 = new BossKarah(_refControl.sprites[26], 52, 73, 1, _bossDerrotado[BossKarah.getCodBoss()]);
                    _enemigoArray[0] = bAux2;

                    p = new PuzzleC1(0, bAux2, false, false);
                    p.AgregarZonaCerrada(152, new Vector2(52, 85), true);
                    p.AgregarZonaCerrada(153, new Vector2(52, 84), true);
                    puzzle = p;
                }
                    break;
            case "mapas/Z3-2-0":
                refControl.PlayMusica(2);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                _enemigoArray = new Enemigo[2];

                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new EnemigoPasivo(14, refControl.otrasTexturas[26], 5, 11, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 0f, 0f, 100f)), true, 20); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(15, refControl.otrasTexturas[26], 7, 11, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 0f, 0f, 100f)), true, 20); enemigoArray[1] = eAux;
                }
                else
                {
                    eAux = new EnemigoPasivo(14, refControl.otrasTexturas[26], 5, 11, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(15, refControl.otrasTexturas[26], 7, 11, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[1] = eAux;
                }
                break;

            case "mapas/Z4-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(1);

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(12, 23, 25, ITEMLIST.Instance.getPreset(2), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(10, 2, 42, 20));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(12, 103, 105, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(10, 2, 42, 20));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z4-0-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(1);
                _npcArray = new NPC[2];

                aux = new NPC(refControl.sprites[10], 11, 6, false, 1, 0);
                aux.AgregarConversacion(CONFIG.getTexto(115));
                _npcArray[0] = aux;

                aux = new NPC(_refControl.sprites[15], 8, 3, false, 0, 1);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[2], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[2], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[2], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[2], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[2], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[2], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[9], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[1] = aux;

                _enemigoArray = new Enemigo[1];
                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new EnemigoPasivo(16, refControl.otrasTexturas[26], 0, 9, 1, ITEMLIST.Instance.getPreset(2, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[0] = eAux;
                }
                else
                {
                    eAux = new EnemigoPasivo(16, refControl.otrasTexturas[26], 0, 9, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 20); enemigoArray[0] = eAux;
                }
                break;

            case "mapas/Z4-1":
                refControl.PlayMusica(3);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(25, 23, 25, ITEMLIST.Instance.getPreset(2), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 0, 68, 14));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(25, 103, 105, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 0, 68, 14));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z4-2":
                refControl.PlayMusica(3);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if(_bossDerrotado[BossNalyivia.getCodBoss()])
                {
                    _npcArray = new NPC[2];
                    aux = new NPC(_refControl.sprites[41], 16, 10, false, 0, -1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(116));
                    aux.setQuest();
                    _npcArray[0] = aux;

                    aux = new Portal(true);
                    _npcArray[1] = aux;
                }
                else
                {
                    _npcArray = new NPC[1];
                    aux = new Portal(true);
                    _npcArray[0] = aux;
                }

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(25, 23, 25, ITEMLIST.Instance.getPreset(2), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(30, 0, 42, 14));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(25, 103, 105, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(30, 0, 42, 14));

                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }

                break;
            case "mapas/Z4-3":
                refControl.PlayMusica(3);
                _refControl.CambiarTileset("tilesets/tilesetDesierto");

                if (!_bossDerrotado[BossNalyivia.getCodBoss()])
                {
                    _enemigoArray = new Enemigo[1];
                    BossNalyivia bAux3 = new BossNalyivia(_refControl.sprites[34], 18, 18, 3, _bossDerrotado[BossNalyivia.getCodBoss()]);
                    _enemigoArray[0] = bAux3;
                }
                else
                {
                    _npcArray = new NPC[1];
                    aux = new Portal(false);
                    _npcArray[0] = aux;
                }

                break;
            case "mapas/Z5-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(1);
                break;
            case "mapas/Z5-1":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(4);
                _npcArray = new NPC[19];
                aux = new NPC(_refControl.sprites[13], 20, 20, false, -1, 0);
                listaI = new List<Item>();
                for (int i = 0; i < 1; i++)
                {
                    listaI.Add(new Item(ITEMLIST.Armadura[0], Item.Calidad.epico));
                    listaI.Add(new Item(ITEMLIST.Casco[0], Item.Calidad.legendario));
                }
                aux.AgregarConversacion(CONFIG.getTexto(117));
                aux.setGambler();
                _npcArray[0] = aux;

                aux = new NPC(refControl.sprites[14], 14, 7, true, 1, 0);               _npcArray[1] = aux;
                aux = new NPC(refControl.sprites[14], 9, 9, false, 1, 0);                _npcArray[2] = aux;
                aux = new NPC(refControl.sprites[14], 14, 21, true, 1, 0);                _npcArray[3] = aux;
                aux = new NPC(refControl.sprites[7], 18, 17, false, 1, 0);                _npcArray[4] = aux;
                aux = new NPC(refControl.sprites[8], 18, 16, false, 1, 0);                _npcArray[5] = aux;
                aux = new NPC(refControl.sprites[9], 12, 18, false, -1, 0); _npcArray[6] = aux;
                aux = new NPC(refControl.sprites[10], 8, 22, false, 1, 0); _npcArray[7] = aux;
                aux = new NPC(refControl.sprites[12], 20, 8, false, -1, 0); _npcArray[8] = aux;
                aux = new NPC(refControl.sprites[13], 18, 3, false, -1, 0); _npcArray[9] = aux;
                aux = new NPC(refControl.sprites[15], 17, 2, false, 0, 1); _npcArray[10] = aux;
                aux = new NPC(refControl.sprites[16], 11, 4, false, 0, 1); _npcArray[11] = aux;
                aux = new NPC(refControl.sprites[8], 10, 6, false, -1, 0); _npcArray[12] = aux;
                aux = new NPC(refControl.sprites[9], 14, 13, true); _npcArray[13] = aux;

                aux = new NPC(_refControl.sprites[7], 20, 5, false, -1, 0);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[3], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[3], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[3], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[3], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[3], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[3], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[9], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[14] = aux;

                if (!almaPoder || !almaSabiduria || !almaValor)
                {
                    aux = new NPC_INFO(15, 31, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(118));
                    _npcArray[15] = aux;
                }
                else
                {
                    if (!_bossDerrotado[5])
                    {
                        aux = new NPC_INFO(15, 31, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                        aux.setSolido();
                        aux.AgregarConversacion(CONFIG.getTexto(119));
                        _npcArray[15] = aux;
                    }
                    else
                    {
                        _npcArray[15] = null;
                    }
                }

                if (!almaSabiduria)
                {
                    aux = new NPC_INFO(14, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(120));
                    _npcArray[16] = aux;
                }
                else
                {
                    aux = new NPC_INFO(14, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(121));
                    _npcArray[16] = aux;
                }

                if (!almaPoder)
                {
                    aux = new NPC_INFO(10, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(122));
                    _npcArray[17] = aux;
                }
                else
                {
                    aux = new NPC_INFO(10, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(123));
                    _npcArray[17] = aux;
                }

                if (!almaValor)
                {
                    aux = new NPC_INFO(18, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(124));
                    _npcArray[18] = aux;
                }
                else
                {
                    aux = new NPC_INFO(18, 33, _refControl.otrasTexturas[49], null);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(125));
                    _npcArray[18] = aux;
                }

                if (!almaPoder || !almaSabiduria || !almaValor)
                {
                    currentMapa._layer2[13 + 36 * currentMapa.DIMX] = 70;
                    currentMapa._layer2[13 + 37 * currentMapa.DIMX] = 69;
                    currentMapa._layer2[13 + 38 * currentMapa.DIMX] = 68;
                    currentMapa._layer2[14 + 36 * currentMapa.DIMX] = 73;
                    currentMapa._layer2[14 + 37 * currentMapa.DIMX] = 72;
                    currentMapa._layer2[14 + 38 * currentMapa.DIMX] = 71;
                    currentMapa._layer2[15 + 36 * currentMapa.DIMX] = 76;
                    currentMapa._layer2[15 + 37 * currentMapa.DIMX] = 75;
                    currentMapa._layer2[15 + 38 * currentMapa.DIMX] = 74;

                    currentMapa._mundoObstaculos[13 + 36 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[13 + 37 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[13 + 38 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 36 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 37 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 38 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[15 + 36 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[15 + 37 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[15 + 38 * currentMapa.DIMX] = true;
                }

                break;
            case "mapas/Z5-2":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(4);
                if (!_bossDerrotado[BossRaghtul.getCodBoss()])
                {
                    _enemigoArray = new Enemigo[1];
                    BossRaghtul bAux2 = new BossRaghtul(_refControl.sprites[27], 10, 6, 2, _bossDerrotado[BossRaghtul.getCodBoss()]);
                    _enemigoArray[0] = bAux2;

                    p = new PuzzleC1(0, bAux2, false);
                    p.AgregarZonaCerrada(70, new Vector2(9, 11), true);
                    p.AgregarZonaCerrada(69, new Vector2(9, 12), true);
                    p.AgregarZonaCerrada(68, new Vector2(9, 13), true);
                    p.AgregarZonaCerrada(73, new Vector2(10, 11), true);
                    p.AgregarZonaCerrada(72, new Vector2(10, 12), true);
                    p.AgregarZonaCerrada(71, new Vector2(10, 13), true);
                    p.AgregarZonaCerrada(76, new Vector2(11, 11), true);
                    p.AgregarZonaCerrada(75, new Vector2(11, 12), true);
                    p.AgregarZonaCerrada(74, new Vector2(11, 13), true);
                    puzzle = p;
                }
                break;
            case "mapas/Z6-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(5);
                if (_bossDerrotado[4])
                {
                    _npcArray = new NPC[2];
                    aux = new NPC(_refControl.sprites[40], 14, 4, false, 0, 1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(126));
                    aux.setQuest();
                    _npcArray[0] = aux;

                    aux = new NPC(refControl.sprites[10], 9, 3, false, 0, -1);
                    aux.AgregarConversacion(CONFIG.getTexto(127));
                    _npcArray[1] = aux;
                }
                else
                {
                    _npcArray = new NPC[1];
                    aux = new NPC(refControl.sprites[10], 9, 3, false, 0, -1);
                    aux.AgregarConversacion(CONFIG.getTexto(127));
                    _npcArray[0] = aux;
                }

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 30, 32, ITEMLIST.Instance.getPreset(3), 2);

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[35], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[37], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[36], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 10, 28, 20));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(20, 110, 112, ITEMLIST.Instance.getPreset(10), 2);

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[35], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[37], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[36], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));

                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(0, 10, 28, 20));

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z6-1":
                refControl.PlayMusica(5);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                
                if (!_bossDerrotado[BossRhilik.getCodBoss()])
                {
                    _enemigoArray = new Enemigo[2];
                    if (!CONFIG.MODO_EXTREMO)
                    {
                        eAux = new EnemigoPasivo(17, refControl.otrasTexturas[26], 23, 14, 1, ITEMLIST.Instance.getPreset(3, new Vector4(0f, 100f, 40f, 10f)), true, 30); enemigoArray[1] = eAux;
                    }
                    else
                    {
                        eAux = new EnemigoPasivo(17, refControl.otrasTexturas[26], 23, 14, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 30); enemigoArray[1] = eAux;
                    }

                    BossRhilik bAux3 = new BossRhilik(_refControl.sprites[32], 14, 34, 2, _bossDerrotado[BossRhilik.getCodBoss()]);
                    _enemigoArray[0] = bAux3;

                    p2 = new PuzzleC2(0, bAux3);
                    p2.AgregarZonaCerrada(new Vector2(11, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(12, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(13, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(14, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(15, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(16, 25), true);
                    p2.AgregarZonaCerrada(new Vector2(17, 25), true);
                    puzzle = p2;
                }
                else
                {
                    _enemigoArray = new Enemigo[1];
                    if (!CONFIG.MODO_EXTREMO)
                    {
                        eAux = new EnemigoPasivo(17, refControl.otrasTexturas[26], 23, 14, 1, ITEMLIST.Instance.getPreset(3, new Vector4(0f, 100f, 40f, 10f)), true, 30); enemigoArray[0] = eAux;
                    }
                    else
                    {
                        eAux = new EnemigoPasivo(17, refControl.otrasTexturas[26], 23, 14, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 30); enemigoArray[0] = eAux;
                    }
                }

                break;
            case "mapas/Z7-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 45, 52, ITEMLIST.Instance.getPreset(4), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 60));

                    //18 posibilidades, 12 melee, 4 magos 2 arquero

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 122, 128, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 60));

                    //18 posibilidades, 12 melee, 4 magos 2 arquero

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z7-1":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 50, 53, ITEMLIST.Instance.getPreset(5), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 60));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(19, refControl.otrasTexturas[26], 43, 74, 1, ITEMLIST.Instance.getPreset(5, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[0] = eAux;
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(50, 130, 133, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 60));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);

                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(19, refControl.otrasTexturas[26], 43, 74, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[0] = eAux;
                }
                break;
            case "mapas/Z7-1-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);
                break;
            case "mapas/Z7-1-1":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);
                if (!puzzleResuelto[1])
                {
                    _npcArray = new NPC[1];
                    aux = new NPC(_refControl.sprites[7], 13, 16, false, 0, -1);
                    aux.AgregarConversacion(CONFIG.getTexto(128));
                    _npcArray[0] = aux;
                }
                

                p1 = new PuzzleT1(1, true);
                p1.AgregarControlMecanismo(new Vector2(3, 23));
                p1.AgregarControlMecanismo(new Vector2(9, 23));
                p1.AgregarControlMecanismo(new Vector2(19, 23));
                p1.AgregarControlMecanismo(new Vector2(25, 23));
                p1.AgregarControlMecanismo(new Vector2(3, 19));
                p1.AgregarControlMecanismo(new Vector2(9, 19));
                p1.AgregarControlMecanismo(new Vector2(19, 19));
                p1.AgregarControlMecanismo(new Vector2(25, 19));
                
                p1.AgregarRelojMecanismo(new Vector2(2, 28));
                p1.AgregarRelojMecanismo(new Vector2(5, 28));
                p1.AgregarRelojMecanismo(new Vector2(8, 28));
                p1.AgregarRelojMecanismo(new Vector2(11, 28));
                p1.AgregarRelojMecanismo(new Vector2(17, 28));
                p1.AgregarRelojMecanismo(new Vector2(20, 28));
                p1.AgregarRelojMecanismo(new Vector2(23, 28));
                p1.AgregarRelojMecanismo(new Vector2(26, 28));
                p1.AgregarZonaCerrada(70, new Vector2(13, 26), true);
                p1.AgregarZonaCerrada(69, new Vector2(13, 27), true);
                p1.AgregarZonaCerrada(68, new Vector2(13, 28), true);
                p1.AgregarZonaCerrada(73, new Vector2(14, 26), true);
                p1.AgregarZonaCerrada(72, new Vector2(14, 27), true);
                p1.AgregarZonaCerrada(71, new Vector2(14, 28), true);
                p1.AgregarZonaCerrada(76, new Vector2(15, 26), true);
                p1.AgregarZonaCerrada(75, new Vector2(15, 27), true);
                p1.AgregarZonaCerrada(74, new Vector2(15, 28), true);
                puzzle = p1;
                break;
            case "mapas/Z7-1-2":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);
                enemigoArray = new Enemigo[4];
                if (!CONFIG.MODO_EXTREMO)
                {
                    eAux = new EnemigoPasivo(20, refControl.otrasTexturas[26], 3, 3, 1, ITEMLIST.Instance.getPreset(5, new Vector4(0f, 0f, 0f, 100f)), true, 50); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(21, refControl.otrasTexturas[26], 9, 3, 1, ITEMLIST.Instance.getPreset(5, new Vector4(0f, 0f, 0f, 100f)), true, 50); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(22, refControl.otrasTexturas[26], 19, 3, 1, ITEMLIST.Instance.getPreset(5, new Vector4(0f, 0f, 0f, 100f)), true, 50); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(23, refControl.otrasTexturas[26], 25, 3, 1, ITEMLIST.Instance.getPreset(5, new Vector4(0f, 0f, 0f, 100f)), true, 50); enemigoArray[3] = eAux;
                }
                else
                {
                    eAux = new EnemigoPasivo(20, refControl.otrasTexturas[26], 3, 3, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[0] = eAux;
                    eAux = new EnemigoPasivo(21, refControl.otrasTexturas[26], 9, 3, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[1] = eAux;
                    eAux = new EnemigoPasivo(22, refControl.otrasTexturas[26], 19, 3, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[2] = eAux;
                    eAux = new EnemigoPasivo(23, refControl.otrasTexturas[26], 25, 3, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 50); enemigoArray[3] = eAux;
                }
                break;
            case "mapas/Z7-2":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(6);
                _npcArray = new NPC[1];
                aux = new NPC(_refControl.sprites[15], 43, 74, false, 0, -1);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[4], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[4], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[4], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[4], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[4], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[4], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[9], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[0] = aux;

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(15, 50, 53, ITEMLIST.Instance.getPreset(5), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 13));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 2);
                    _enemigoArray[0] = new Enemigo(_refControl.sprites[5], 19, 15, 1, true, ITEMLIST.Instance.getPreset(5), 2);
                    eAux = new EnemigoPasivo(24, refControl.otrasTexturas[26], 5, 74, 1, ITEMLIST.Instance.getPreset(6, new Vector4(0f, 100f, 40f, 10f)), true, 60); enemigoArray[1] = eAux;
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(15, 130, 133, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 12, 41, 13));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[5], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[22], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[29], 39, 9, 1, false, null, 2);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[30], 28, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[44]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    eAux = new Enemigo(refControl.sprites[31], 16, 9, 1, false, null, 3);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 2);
                    _enemigoArray[0] = new Enemigo(_refControl.sprites[5], 19, 15, 130, true, ITEMLIST.Instance.getPreset(10), 2);
                    eAux = new EnemigoPasivo(24, refControl.otrasTexturas[26], 5, 74, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 60); enemigoArray[1] = eAux;
                }
                if (!_bossDerrotado[BossGhaldum.getCodBoss()])
                {
                    //_enemigoArray = new Enemigo[1];
                    BossGhaldum bAux4 = new BossGhaldum(_refControl.sprites[33], 24, 42, 2, _bossDerrotado[BossGhaldum.getCodBoss()]);
                    _enemigoArray[0] = bAux4;

                    p3 = new PuzzleC3(0, bAux4);
                    p3.AgregarZonaCerrada(6, new Vector2(14, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(15, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(16, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(17, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(18, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(19, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(20, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(21, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(22, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(23, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(24, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(25, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(26, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(27, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(28, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(29, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(30, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(31, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(32, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(33, 31), true);
                    p3.AgregarZonaCerrada(6, new Vector2(34, 31), true);



                    p3.AgregarZonaCerrada(54, new Vector2(35, 40), true);
                    p3.AgregarZonaCerrada(54, new Vector2(35, 41), true);
                    p3.AgregarZonaCerrada(54, new Vector2(35, 42), true);
                    p3.AgregarZonaCerrada(54, new Vector2(35, 43), true);
                    p3.AgregarZonaCerrada(54, new Vector2(35, 44), true);
                    p3.AgregarZonaCerrada(54, new Vector2(35, 45), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 40), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 41), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 42), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 43), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 44), true);
                    p3.AgregarZonaCerrada(54, new Vector2(13, 45), true);

                    p3.AgregarZonaCerrada(70, new Vector2(23, 46), true);
                    p3.AgregarZonaCerrada(69, new Vector2(23, 47), true);
                    p3.AgregarZonaCerrada(68, new Vector2(23, 48), true);
                    p3.AgregarZonaCerrada(73, new Vector2(24, 46), true);
                    p3.AgregarZonaCerrada(72, new Vector2(24, 47), true);
                    p3.AgregarZonaCerrada(71, new Vector2(24, 48), true);
                    p3.AgregarZonaCerrada(76, new Vector2(25, 46), true);
                    p3.AgregarZonaCerrada(75, new Vector2(25, 47), true);
                    p3.AgregarZonaCerrada(74, new Vector2(25, 48), true);
                    puzzle = p3;
                }
                break;
            case "mapas/Z8-0":
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                refControl.PlayMusica(7);
                if (!_bossDerrotado[7])
                {
                    _npcArray = new NPC[1];
                    aux = new NPC_INFO(14, 4, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(129));
                    _npcArray[0] = aux;
                }

                break;
            case "mapas/Z8-1":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-2":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-3":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-3-0":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                enemigoArray = new Enemigo[1];
                eAux = new EnemigoPasivo(25, refControl.otrasTexturas[26], 0, 5, 1, ITEMLIST.Instance.getPreset(6, new Vector4(0f, 100f, 40f, 10f)), true, 60); enemigoArray[0] = eAux;

                _npcArray = new NPC[2];
                aux = new NPC(_refControl.sprites[16], 5, 5, false, -1, 0);
                listaI = new List<Item>();
                if (!CONFIG.MODO_EXTREMO)
                {
                    listaI.Add(new Item(ITEMLIST.Espada[5], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[5], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[5], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[5], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[5], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[5], Item.Calidad.raro));
                }
                else
                {
                    listaI.Add(new Item(ITEMLIST.Espada[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Escudo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Casco[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Armadura[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Anillo[9], Item.Calidad.raro));
                    listaI.Add(new Item(ITEMLIST.Amuleto[9], Item.Calidad.raro));
                }
                aux.AgregarConversacion(CONFIG.getTexto(106));
                aux.setShop(listaI);
                _npcArray[0] = aux;

                aux = new NPC(_refControl.sprites[15], 10, 2, true);

                aux.AgregarConversacion(CONFIG.getTexto(130));
                _npcArray[1] = aux;

                break;
            case "mapas/Z8-4":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-5":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-6":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-7":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-8":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-9":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");

                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(26, refControl.otrasTexturas[26], 1, 12, 1, ITEMLIST.Instance.getPreset(6, new Vector4(0f, 100f, 40f, 10f)), true, 60); enemigoArray[0] = eAux;

                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa, 1);

                    eAux = new EnemigoPasivo(26, refControl.otrasTexturas[26], 1, 12, 1, ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 40f, 10f)), true, 60); enemigoArray[0] = eAux;

                }


                break;
            case "mapas/Z8-10":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-11":
                refControl.PlayMusica(7);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");
                if (!CONFIG.MODO_EXTREMO)
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(12, 25), 60, 70, ITEMLIST.Instance.getPreset(6), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                else
                {
                    enemigoZoneLoader = new EnemigoZoneLoader(Random.Range(5, 11), 140, 145, ITEMLIST.Instance.getPreset(10), 2);
                    enemigoZoneLoader.setZonaSpawnCustom(new Rect(3, 3, 23, 17));

                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[6], 450, 248, 12, true, ITEMLIST.Instance.getPreset(1), 2));
                    enemigoZoneLoader.agregarTipoDeEnemigo(new Enemigo(_refControl.sprites[23], 339, 240, 12, false, ITEMLIST.Instance.getPreset(1), 1));
                    eAux = new Enemigo(refControl.sprites[28], 50, 9, 1, false, ITEMLIST.Instance.getPreset(1), 2);
                    eAux.setMago(refControl.otrasTexturas[43]);
                    enemigoZoneLoader.agregarTipoDeEnemigo(eAux);
                    _enemigoArray = enemigoZoneLoader.GenerarEnemigos(currentMapa);
                }
                break;
            case "mapas/Z8-12":
                refControl.PlayMusica(11);
                _refControl.CambiarTileset("tilesets/tilesetInteriores");


                if (!_bossDerrotado[7])
                {
                    _npcArray = new NPC[3];
                    aux = new NPC_INFO(12, 7, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(131));
                    _npcArray[0] = aux;

                    aux = new NPC(_refControl.sprites[41], 10, 8, false, 0, -1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(132));
                    aux.setQuest();
                    _npcArray[1] = aux;

                    aux = new NPC(_refControl.sprites[40], 14, 8, false, 0, -1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(132));
                    aux.setQuest();
                    _npcArray[2] = aux;

                    currentMapa._layer2[12 + 53 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[12 + 54 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[12 + 55 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[13 + 53 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[13 + 54 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[13 + 55 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[14 + 53 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[14 + 54 * currentMapa.DIMX] = 14;
                    currentMapa._layer2[14 + 55 * currentMapa.DIMX] = 14;

                    currentMapa._mundoObstaculos[12 + 53 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[12 + 54 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[12 + 55 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[13 + 53 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[13 + 54 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[13 + 55 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 53 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 54 * currentMapa.DIMX] = true;
                    currentMapa._mundoObstaculos[14 + 55 * currentMapa.DIMX] = true;


                    _enemigoArray = new Enemigo[1];
                    BossModrean1 bAux4 = new BossModrean1(_refControl.sprites[42], 12, 51, 2, _bossDerrotado[BossModrean1.getCodBoss()]);
                    _enemigoArray[0] = bAux4;

                    p2 = new PuzzleC2(0, bAux4);
                    p2.AgregarZonaCerrada(new Vector2(0, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(1, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(2, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(3, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(4, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(5, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(6, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(7, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(8, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(9, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(10, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(11, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(12, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(13, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(14, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(15, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(16, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(17, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(18, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(19, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(20, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(21, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(22, 43), true);
                    p2.AgregarZonaCerrada(new Vector2(23, 43), true);
                    puzzle = p2;
                }
                else
                {
                    if (!_bossDerrotado[8])
                    {
                        _npcArray = new NPC[1];
                        aux = new NPC_INFO(15, 52, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                        aux.setSolido();
                        aux.AgregarConversacion(CONFIG.getTexto(133));
                        _npcArray[0] = aux;
                    }
                }

                break;

            case "mapas/Z8-13":
                _refControl.CambiarTileset("tilesets/tilesetCueva");

                if (!_bossDerrotado[8])
                {
                    refControl.PlayMusica(8);
                    _npcArray = new NPC[3];
                    aux = new NPC_INFO(47, 8, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(134));
                    _npcArray[0] = aux;

                    aux = new NPC(_refControl.sprites[41], 49, 10, false, 0, -1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(135));
                    aux.setQuest();
                    _npcArray[1] = aux;

                    aux = new NPC(_refControl.sprites[40], 45, 10, false, 0, -1);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(136));
                    aux.setQuest();
                    _npcArray[2] = aux;


                    _enemigoArray = new Enemigo[1];
                    BossModrean2 bAux5 = new BossModrean2(_refControl.sprites[42], 49, 60, 2, _bossDerrotado[BossModrean2.getCodBoss()]);
                    _enemigoArray[0] = bAux5;

                    p2 = new PuzzleC2(0, bAux5);
                    p2.AgregarZonaCerrada(new Vector2(45, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(46, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(47, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(48, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(49, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(50, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(51, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(52, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(53, 49), true);
                    p2.AgregarZonaCerrada(new Vector2(54, 49), true);
                    puzzle = p2;
                }
                else
                {
                    refControl.PlayMusica(9);
                    _npcArray = new NPC[4];
                    aux = new NPC_INFO(49, 60, _refControl.otrasTexturas[35], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(137));
                    _npcArray[0] = aux;

                    aux = new NPC(_refControl.sprites[41], 46, 62, false, 1, 0);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(138));
                    aux.setQuest();
                    _npcArray[1] = aux;

                    aux = new NPC(_refControl.sprites[40], 55, 59, false, -1, 0);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(139));
                    aux.setQuest();
                    _npcArray[2] = aux;

                    aux = new NPC_INFO(49, 64, _refControl.otrasTexturas[38], _refControl.otrasTexturas[34]);
                    aux.setSolido();
                    aux.AgregarConversacion(CONFIG.getTexto(140), false);
                    aux.setEspecial();
                    _npcArray[3] = aux;

                }
                break;

            default:
                return false;   //sino encuentra el mapa sale por aca
        }
        Guardar();
        return true;
    }

    /// <summary>
    /// Inicia el proceso de fade out -> cargar mapa -> fade in. Esta funcion se llama desde CambioDeMapa(). 
    /// Esta funcion unicamente INICIA, TrancicionCambiandoMapa() se encarga de hacer el proceso.
    /// </summary>
    /// <param name="mapa"></param>
    public void IniciarTransicionMapa(string mapa)
    {
        _mapaACargar = mapa;
        _enFadeOut = true;
        _porcentajeFade = 0f;
        _pausa = true;
        hud.currentVentanaPausa = Hud.ventanaPausa.nada;
        hud.IniciarTituloZonaFade();
    }

    private bool TransisionCambiandoMapa()
    {
        //tarda 1 segundo cada transicion
        if (_enFadeIn)
        {
            _porcentajeFade -= _elapsed;
            if (_porcentajeFade <= 0f)
            {
                _porcentajeFade = 0f;
                _enFadeIn = false;
                _pausa = false;
                hud.IniciarTituloZonaFade(_tituloNuevaZona);
            }
            return true;
        }
        else if (_enFadeOut)
        {
            _porcentajeFade += _elapsed;
            if (_tituloNuevaZona != "")
                refControl.cambiarVolumenMusica(CONFIG.vol_musica * (1f - _porcentajeFade));
            if (_porcentajeFade >= 1f)
            {
                _porcentajeFade = 1f;
                _enFadeOut = false;
                refControl.cambiarVolumenMusica(CONFIG.vol_musica);
                SelectorDeMapas(_mapaACargar);
                player.pos = _pNuevaPos;
                _enFadeIn = true;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Reacomoda el array de enemigos descartando los que ya estan muertos (en estado miss)
    /// </summary>
    private void RedimensionarArrayEnemigos()
    {
        Enemigo[] arrayAux;
        int cant = 0;
        int i;
        for (i = 0; i < _enemigoArray.Length; i++)
        {
            if (_enemigoArray[i].Estado != EntidadCombate.estado.miss)
            {
                cant++;
            }
        }

        if (cant <= 0)
        {
            return;
        }

        arrayAux = new Enemigo[cant];
        cant = 0;   //reuso la variable como contador
        for (i = 0; i < _enemigoArray.Length; i++)
        {
            if (_enemigoArray[i].Estado != EntidadCombate.estado.miss)
            {
                arrayAux[cant] = _enemigoArray[i];
                cant++;
            }
        }

        _enemigoArray = arrayAux;
    }

    public void setBossDerrotado(int cod)
    {
        if (cod >= 0 && cod < _bossDerrotado.Length)
            _bossDerrotado[cod] = true;
        //ultimoBossDerrotado = cod;
        bossActivo = null;
        refControl.PlayMusica(-1);
    }

    public void Respawn()
    {
        enemigoArray = null;
        _npcArray = null;
        bossActivo = null;
        _pNuevaPos = new Vector2(posRespawn.x, posRespawn.y);
        IniciarTransicionMapa(mapaRespawn);
    }

    public void Guardar()
    {


        if (VALORES.currentSlot == "")
            return;

        using (BinaryWriter bw = new BinaryWriter(File.Open(VALORES.currentSlot, FileMode.Create)))
        {
            //GENERAL
            bw.Write(player.getNivel());
            bw.Write(CONFIG.MODO_EXTREMO);
            bw.Write(player.getHp());
            bw.Write(player.Exp);
            bw.Write(player.inventario.oro);

            //HABILIDADES
            for (int i = 0; i < 5; i++)
            {
                if (player.existeSkill(i))
                {
                    bw.Write(player.getSkill(i).getCodigo());
                    bw.Write((int)player.getSkill(i).cooldownRestante);
                }
                else
                {
                    bw.Write(0);
                }
            }

            if (player.getPasiva(0) != null)
            {
                bw.Write(1);
            }
            else
            {
                bw.Write(0);
            }

            if (player.getPasiva(1) != null)
            {
                bw.Write(1);
            }
            else
            {
                bw.Write(0);
            }


            //INVENTARIO

            Item aux = null;
            bool equip = true;
            bool fin = false;
            int index = 0;
            while (!fin)
            {
                if (equip)
                {
                    switch (index)
                    {
                        case 0:
                            aux = player.inventario.equipadoCasco;
                            break;
                        case 1:
                            aux = player.inventario.equipadoArmadura;
                            break;
                        case 2:
                            aux = player.inventario.equipadoAmuleto;
                            break;
                        case 3:
                            aux = player.inventario.equipadoAnillo;
                            break;
                        case 4:
                            aux = player.inventario.equipadoArma;
                            break;
                        case 5:
                            aux = player.inventario.equipadoEscudo;
                            break;
                    }

                    index++;
                    if (index > 5)
                    {
                        index = 0;
                        equip = false;
                    }
                }
                else
                {
                    if (index == 0)
                    {
                        bw.Write(player.inventario.getListaItems().Count);////////////////////////////////////////////
                    }
                    if (player.inventario.getListaItems() != null && index < player.inventario.getListaItems().Count)
                    {
                        aux = player.inventario.getListaItems()[index];

                        index++;
                        if (index >= player.inventario.getListaItems().Count)
                        {
                            fin = true;
                        }
                    }
                    else
                    {
                        aux = null;
                        fin = true;
                    }

                    
                }
                
                if (aux != null)
                {
                    //guardar item
                    bw.Write(aux.CodigoItem);
                    bw.Write((int)aux.getCalidad);
                    bw.Write((int)aux.getTipoAtributo(1));
                    bw.Write((int)aux.getTipoAtributo(2));
                    bw.Write((int)aux.getTipoAtributo(3));
                }
                else if (!fin)    //sino cuando hace el cambio de equipado a inventario me graba uno de mas
                {
                    //ojo acordarse de esto
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                }

            }

            //MISC
            bw.Write(almaPoder);
            bw.Write(almaSabiduria);
            bw.Write(almaValor);
            for (int i = 0; i < _cofresDestruidos.Length; i++)
            {
                bw.Write(_cofresDestruidos[i]);
            }
            for (int i = 0; i < _bossDerrotado.Length; i++)
            {
                bw.Write(_bossDerrotado[i]);
            }
            for (int i = 0; i < puzzleResuelto.Length; i++)
            {
                bw.Write(puzzleResuelto[i]);
            }

            bw.Write(posRespawn.x);
            bw.Write(posRespawn.y);
            //Debug.Log("Pos respawn guardada: " + posRespawn);
            bw.Write(mapaRespawn); //posible problema con string?? https://social.msdn.microsoft.com/Forums/vstudio/en-US/0f426e43-cb1a-4069-865f-4ad2dfd730a6/binarywriter-write-string-problem?forum=csharpgeneral
        }

    }

    public void Cargar()
    {


        if (VALORES.currentSlot == "")
            return;

        using (BinaryReader br = new BinaryReader(File.Open(VALORES.currentSlot, FileMode.Open)))
        {
            //GENERAL
            int lvl = br.ReadInt32();
            CONFIG.MODO_EXTREMO = br.ReadBoolean();
            int hp = br.ReadInt32();
            int xp = br.ReadInt32();
            player.setStatsCargar(lvl, hp, xp);
            player.inventario.oro = br.ReadInt32();

            //--------HASTA ACA TODO OK FALTA SEGUIR CON LO DEMAS

            //HABILIDADES
            for (int i = 0; i < 5; i++)
            {
                int sk = br.ReadInt32();

                if (sk != 0)
                {
                    for (int c = 0; c < player.skillsAuxiliar.Length; c++)
                    {
                        if (sk == player.skillsAuxiliar[c].getCodigo())
                        {
                            player.NuevaSkill(player.skillsAuxiliar[c]);
                            player.getSkillPorCodigo(player.skillsAuxiliar[c].getCodigo()).setCooldownRestante(br.ReadInt32());
                            break;
                        }
                    }
                    
                }
            }
            int ps = br.ReadInt32();
            if (ps != 0)
            {
                player.NuevaPasiva(player.skillsAuxiliar[9]);
            }
            ps = br.ReadInt32();
            if (ps != 0)
            {
                player.NuevaPasiva(player.skillsAuxiliar[10]);
            }

            //INVENTARIO
            int cod, cal, at1, at2, at3;

            player.inventario.equipadoCasco = null;
            player.inventario.equipadoArma = null;
            player.inventario.equipadoArmadura = null;
            player.inventario.equipadoAmuleto = null;
            player.inventario.equipadoAnillo = null;
            player.inventario.equipadoEscudo = null;


            for (int i = 0; i < 6; i++)
            {
                cod = br.ReadInt32();
                cal = br.ReadInt32();
                at1 = br.ReadInt32();
                at2 = br.ReadInt32();
                at3 = br.ReadInt32();
                if (cod == 0)
                    continue;

                
                switch (i)
                {
                    case 0:
                        player.inventario.equipadoCasco = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                    case 1:
                        player.inventario.equipadoArmadura = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                    case 2:
                        player.inventario.equipadoAmuleto = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                    case 3:
                        player.inventario.equipadoAnillo = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                    case 4:
                        player.inventario.equipadoArma = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                    case 5:
                        player.inventario.equipadoEscudo = new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3);
                        break;
                }
            }
            int cant = br.ReadInt32();

            for (int i = 0; i < cant; i++)
            {
                cod = br.ReadInt32();
                cal = br.ReadInt32();
                at1 = br.ReadInt32();
                at2 = br.ReadInt32();
                at3 = br.ReadInt32();

                if (cod == 0)
                    continue;

                player.inventario.AgregarNuevoItem(new Item(ITEMLIST.Instance.getItemPorCodigo(cod), (Item.Calidad)cal, (Item.ATRIBUTO)at1, (Item.ATRIBUTO)at2, (Item.ATRIBUTO)at3));
            }

            //MISC
            almaPoder = br.ReadBoolean();
            almaSabiduria = br.ReadBoolean();
            almaValor = br.ReadBoolean();
            
            for (int i = 0; i < _cofresDestruidos.Length; i++)
            {
                _cofresDestruidos[i] = br.ReadBoolean();
            }
            for (int i = 0; i < _bossDerrotado.Length; i++)
            {
                _bossDerrotado[i] = br.ReadBoolean();
            }
            for (int i = 0; i < puzzleResuelto.Length; i++)
            {
                puzzleResuelto[i] = br.ReadBoolean();
            }

            float xx, yy;
            xx = br.ReadSingle();
            yy = br.ReadSingle();
            _pNuevaPos = new Vector2(xx, yy);
            posRespawn = new Vector2(xx, yy);
            player.pos = new Vector2(xx, yy);
            //Debug.Log(_pNuevaPos);
            mapaRespawn = br.ReadString();
        }
        
        SelectorDeMapas(mapaRespawn);

    }
}
