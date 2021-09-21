using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC : Entidad
{
    public enum TIPO_NPC { NULL, CONVERSACION, SHOP, GAMBLER, ESPECIAL };
    protected TIPO_NPC tipo_npc;
    public TIPO_NPC tipoNPC
    {
        get
        {
            return tipo_npc;
        }
    }


    protected static Texture2D globo;
    protected static Texture2D globoShop;

    public static void setGloboTex(Texture2D tex, Texture2D texShop)
    {
        globo = tex;
        globoShop = texShop;
    }

    protected bool _conversacion;
    protected string[] paginasConversacion;

    //variables de ROAM. Se usan en enemigos y NPC
    protected bool _roam;
    protected Vector2 _posBase; //posicion a la que vuelve desp de perseguir al jugador o durante roam
    protected Vector2 _ptoRoam;
    protected bool _buscarNuevoPuntoRoam;
    protected float _tiempoPausaRoam;    //el tiempo que espera es variable
    protected float _tiempoInicioPausaRoam;

    protected Texture2D postDrawDisplayTex = null;
    protected List<Item> listaItems;


    public NPC() : base()
    {
        //constructor default
    }

    public NPC(Texture2D spr, int posX, int posY, bool seMueve, int dirX = 0, int dirY = -1) : base(spr, posX, posY, new Vector2(dirX, dirY))
    {
        _tipo = TIPO.NPC;
        _conversacion = false;
        _roam = seMueve;
        _posBase = new Vector2(_pos.x, _pos.y);
        _buscarNuevoPuntoRoam = true;
        _tiempoPausaRoam = _tiempoInicioPausaRoam = 0.0f;
        _velocidadBase = 2;
        if (!_roam)  //por ahora lo hago para todos los que esten quietos
        {
            setSolido();
        }
    }


    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (postDrawDisplayTex == null)
            return;



        int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
        int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y + 1) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
        if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
        {
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), postDrawDisplayTex);
        }
    }

    public override void EjecutarAccionAI()
    {
        if (_roam)
        {
            if (_tiempoInicioPausaRoam != 0.0f)  //si es 0 significa que no esta en pausa
            {
                if ((_tiempoInicioPausaRoam + _tiempoPausaRoam) <= Game.TiempoTranscurrido)   //fin de pausa
                {
                    _tiempoInicioPausaRoam = 0.0f;
                    _buscarNuevoPuntoRoam = true;
                }
                else  //sigue en pausa
                {
                    return;
                }
            }

            if (_buscarNuevoPuntoRoam)   //calcular punto random para desplazarse
            {
                int auxX, auxY;
                auxX = (int)Random.Range(_posBase.x - 3, _posBase.x + 3);
                auxY = (int)Random.Range(_posBase.y - 3, _posBase.y + 3);
                _ptoRoam = new Vector2(auxX, auxY);
                _buscarNuevoPuntoRoam = false;
            }
            //moverse hacia el nuevo pto
            if (MoverAPos(_ptoRoam, velocidad))
            {
                _tiempoInicioPausaRoam = Game.TiempoTranscurrido;
                _tiempoPausaRoam = Random.Range(1.5f, 3.0f);
            }
        }
    }

    public override void Actualizar()
    {
        while (_microPos.x < 0)
        {
            _microPos.x = 100 + _microPos.x;
            _pos.x--;
        }

        while (_microPos.x >= 100)
        {
            _microPos.x -= 100;
            _pos.x++;
        }

        while (_microPos.y < 0)
        {
            _microPos.y = 100 + _microPos.y;
            _pos.y--;
        }

        while (_microPos.y >= 100)
        {
            _microPos.y -= 100;
            _pos.y++;
        }

        if (_direccionVect.x == 0 && _direccionVect.y == 0)
            _direccionVect = new Vector2(0, -1);

        Vector2 v1 = new Vector2(1, 0);
        float ang = Vector2.Angle(v1, _direccionVect);
        Vector3 cross = Vector3.Cross(v1, _direccionVect);
        if (cross.z < 0)
            ang = 360 - ang;


        if ((ang >= 315) || (ang < 45))
            _dirEnum = direccionEnum.derecha;
        if ((ang >= 45) && (ang < 135))
            _dirEnum = direccionEnum.arriba;
        if ((ang >= 135) && (ang < 225))
            _dirEnum = direccionEnum.izquierda;
        if ((ang >= 225) && (ang < 315))
            _dirEnum = direccionEnum.abajo;


        if (!_roam || (_roam && _tiempoInicioPausaRoam != 0.0f))
        {
            if (_dirEnum == direccionEnum.abajo)
                _sprActual = _idle[0];
            if (_dirEnum == direccionEnum.izquierda)
                _sprActual = _idle[1];
            if (_dirEnum == direccionEnum.arriba)
                _sprActual = _idle[2];
            if (_dirEnum == direccionEnum.derecha)
                _sprActual = _idle[3];
        }
        else //se mueve
        {
            if (Game.TiempoTranscurrido - _tiempoUltimaAnim > CONFIG.TiempoAnimCaminar)
            {
                int offset = 0;
                _tiempoUltimaAnim = Game.TiempoTranscurrido;
                switch (_dirEnum)
                {
                    case direccionEnum.abajo:
                        offset = 0;
                        break;
                    case direccionEnum.arriba:
                        offset = 16;
                        break;
                    case direccionEnum.izquierda:
                        offset = 8;
                        break;
                    case direccionEnum.derecha:
                        offset = 24;
                        break;
                }

                _fase++;
                if (_fase > _faseCaminarMax)
                {
                    _fase = 0;
                }

                _sprActual = _caminar[_fase + offset]; //* CONFIG.escala; //EL TEMA ESTA ACA, QUE SE ACTUALIZA CADA X TIEMPO. LO MISMO PARA ENEMIGOS Y JUGADOR
            }
        }
    }


    // 25/11/16 AGREGAR: SI MANUALMENTE PONGO UN /n TIENE QUE DETECTARLO Y PONER ALGUN CARACTER ESPECIAL PARA INDICAR EL CAMBIO DE PAG, Y QUE LO DETECTE
     
    public void AgregarConversacion(string texto, bool setEol = true)
    {
        //primero ajusta el formato agregando eol
        paginasConversacion = new string[(int)((texto.Length / 286) + 2)];
        //con +1 deberia alcanzar para margenes normales, pero sospecho que en un caso especial podrian no alcanzar las paginas
        // 590/300 = 1

        List<int> indicePag = new List<int>();
        int a = 0;
        int index = 0;
        
        //RECORRE UNA PRIMERA VEZ PARA SETEAR LAS PAGINAS
        if (texto.Length > 286)
        {
            for (int i = 0; i < texto.Length; i++)  
            {
                if (a % 286 == 0 && a != 0)
                {
                    for (index = 0; i + index > 0; index--)
                    {
                        if (texto[i + index] == ' ')    //"rebobina" para encontrar un espacio, y asi seguir en la otra linea
                        {
                            i = i + index;
                            a = a + index;
                            indicePag.Add(a);
                            a = -1; //cuando termina el ciclo suma +1
                            break;
                        }
                    }
                }
                a++;
            }
            //a--;
            indicePag.Add(a);
        }
        //----------------------------------------------
        
        
        //SEPARA EL TEXTO POR PAGINAS
        int indAnt = 0;
        if (indicePag.Count > 0)
        {
            for (int aaa = 0; aaa < indicePag.Count; aaa++)
            {

                paginasConversacion[aaa] = texto.Substring(indAnt, indicePag[aaa]);
                indAnt += indicePag[aaa] + 1;
            }
        }
        else
        {
            paginasConversacion[0] = texto;
        }
        //----------------------------------------------

        //RECORRE LAS PAGINAS PARA PONER LOS EOL
        if (setEol)
        {
            int linea = 0;
            for (int pag = 0; pag < paginasConversacion.Length; pag++)
            {
                if (paginasConversacion[pag] == "" || paginasConversacion[pag] == null)
                {
                    continue;
                }
                linea = 0;
                for (int i = 0; i < paginasConversacion[pag].Length; i++)
                {
                    if (linea == 47)
                    {
                        index = 0;
                        for (index = 0; i + index > 0; index--)
                        {
                            if (paginasConversacion[pag][i + index] == ' ')    //"rebobina" para encontrar un espacio, y asi seguir en la otra linea
                            {
                                i = i + index;
                                char[] arr = paginasConversacion[pag].ToCharArray();
                                arr[i] = '\n';
                                paginasConversacion[pag] = new string(arr);
                                linea = -1;
                                break;
                            }
                        }
                    }
                    linea++;
                }
            }
        }
        
        //----------------------------------------------
        _conversacion = true;
        postDrawDisplayTex = globo;
    }

    public bool hayConversacion()
    {
        return _conversacion;
    }

    public string[] getConversacion()
    {
        return paginasConversacion;
    }

    public void setShop(List<Item> listaItems)
    {
        tipo_npc = TIPO_NPC.SHOP;
        this.listaItems = listaItems;
        postDrawDisplayTex = globoShop;
    }

    public void setSolido()
    {
        refGame.currentMapa.mundoObstaculos[(int)(_pos.x + _pos.y * refGame.currentMapa.DIMX)] = true;
    }

    public List<Item> getItemList()
    {
        return listaItems;
    }

    public void setGambler()
    {
        tipo_npc = TIPO_NPC.GAMBLER;
        postDrawDisplayTex = globoShop;
    }

    public void setEspecial()
    {
        tipo_npc = TIPO_NPC.ESPECIAL;
    }

    public void setQuest()
    {
        postDrawDisplayTex = refControl.otrasTexturas[34];
    }
}
