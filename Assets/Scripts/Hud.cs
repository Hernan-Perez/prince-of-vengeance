using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class Hud
{
    public enum ventanaPausa { nada, main, inventario, habilidades, Configuracion, salir };
    public ventanaPausa currentVentanaPausa;

    public enum estado { normal, ventana };
    private estado _state;
    public estado state
    {
        set
        {
            _state = value;
        }
        get
        {
            return _state;
        }
    }
    public enum estadoNPC { conversacion, shop, gambler, confirmacionModoExtremo };
    private estadoNPC _estadoNpc;
    public estadoNPC estadoNpc
    {
        get
        {
            return _estadoNpc;
        }
        set
        {
            _estadoNpc = value;
        }
    }

    public enum estadoShopNpc { Main, Compra, Venta };
    private estadoShopNpc _estadoShop;

    public NPC npcActivo;
    private int _npcShopOffset;
    private Item _itemSeleccionadoShop;

    private ControlScript _refControl;
    private Game _refGame;

    public string[] textoConversacion;
    private int pag = 0;
    public bool _habilidadesEscondidas;
    private bool _verEquipadoEnVentanaInventario;
    private int _scrollOffsetEnVentanaInventario;
    private Item _itemMostradoEnVentanaInventario;
    
    private bool _enFadeInTitulo = false, _enFadeOutTitulo = false;
    private float _porcentajeFadeTitulo = 0f;
    private string _tituloAMostrar = "";    //este es el titulo que se muestra al entrar el zona, al terminar de mostrarse esta variable vuelve a ""

    //private Jugador refPlayer;
    private GUIStyle estiloventana;
    private GUIStyle estiloHud;

    private GUIStyle estiloBoton;
    private GUIStyle estiloinventario;
    private GUIStyle estiloinventario2;
    private GUIStyle estiloinventariocentrado;
    private GUIStyle estiloventanacentrado;
    private GUIStyle estilocentrado, estilocentrado2;
    private GUIStyle style;
    private Texture2D texture;
    private bool habilidadSinAprender = false;  //esta variable cambia desde la func comprobarHabilidadSinAprender

    private int[] precioGambler;
    private int[] tierLvls;
    private string fileConfig;

    public Hud(ControlScript reff, Game refGame)
    {
        _refControl = reff;
        _refGame = refGame;
        currentVentanaPausa = ventanaPausa.main;
        _habilidadesEscondidas = false;
        fileConfig = Application.persistentDataPath + "/settings.dat";

        _verEquipadoEnVentanaInventario = false;
        _scrollOffsetEnVentanaInventario = 0;
        _itemMostradoEnVentanaInventario = null;

        estiloventana = new GUIStyle();
        estiloventana.normal.textColor = Color.white;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloHud = new GUIStyle();
        estiloHud.normal.textColor = Color.white;
        estiloHud.alignment = TextAnchor.MiddleCenter;

        estiloBoton = new GUIStyle("button");
        estiloBoton.fontSize = UTIL.TextoProporcion(32);    //30 antes
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        estiloBoton.normal.background = _refControl.otrasTexturas[56];

        estiloinventario = new GUIStyle();
        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(30);

        estiloinventario2 = new GUIStyle(estiloinventario);
        estiloinventario2.normal.textColor = Color.white;
        estiloinventario2.alignment = TextAnchor.UpperRight;

        estiloinventariocentrado = new GUIStyle();
        estiloinventariocentrado.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventariocentrado.alignment = TextAnchor.UpperCenter;
        estiloinventariocentrado.fontSize = UTIL.TextoProporcion(34);
        estiloinventariocentrado.normal.textColor = Color.white;

        estiloventanacentrado = new GUIStyle();
        estiloventanacentrado.normal.textColor = Color.white;
        estiloventanacentrado.fontSize = UTIL.TextoProporcion(34);
        estiloventanacentrado.alignment = TextAnchor.UpperCenter;
        
        estilocentrado = new GUIStyle();
        estilocentrado.fontSize = UTIL.TextoProporcion(34);
        estilocentrado.alignment = TextAnchor.UpperCenter;
        estilocentrado.normal.textColor = Color.white;

        estilocentrado2 = new GUIStyle(estilocentrado);
        estilocentrado2.fontSize = UTIL.TextoProporcion(22);

        style = new GUIStyle();
        texture = new Texture2D(128, 128);
        for (int y = 0; y < texture.height; ++y)
        {
            for (int x = 0; x < texture.width; ++x)
            {
                Color color = new Color(0.16f, 0.48f, 0.28f, 0.2f);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        style.normal.background = texture;

        ITEMLIST refItemList = ITEMLIST.Instance;
        if (refItemList.dummy)
        {
            precioGambler = new int[1];//para que no joda el warning
        }

        precioGambler = new int[] {
        (int)(ITEMLIST.Armadura[0].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[1].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[2].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[3].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[4].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[5].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[6].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[7].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[8].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[9].ValorOro * 1.5f),
        (int)(ITEMLIST.Armadura[10].ValorOro * 1.5f)};

        tierLvls = new int[] { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 95 };
    }

    public void Draw()
    {
        estiloventana.fontSize = UTIL.TextoProporcion(34);
        estiloHud.fontSize = UTIL.TextoProporcion(24);
        estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

        /////////////////////////////////////VENTANA DISPLAY//////////////////////////////////////////////////////////////////////
        if (_state == estado.ventana)
        {
            _refControl.StopPasos();
            switch (_estadoNpc)
            {
                case estadoNPC.conversacion:
                    GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.6f), _refControl.otrasTexturas[0]);
                    if (textoConversacion != null)
                    {
                        estiloventana.alignment = TextAnchor.UpperCenter;
                        estiloventana.normal.textColor = Color.white;
                        GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.38f), textoConversacion[pag], estiloventana);
                        estiloventana.alignment = TextAnchor.UpperLeft;
                    }

                    if (pag + 1 < textoConversacion.Length && textoConversacion[pag + 1] != null && textoConversacion[pag + 1] != "")
                    {
                        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(15), estiloBoton)) 
                        {
                            pag++;
                            _refControl.PlaySonido(0, 0.4f);
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), "Ok", estiloBoton))  
                        {
                            _refControl.PlaySonido(0, 0.4f);
                            //pag++;
                            pag = 0;
                            if (npcActivo != null && npcActivo.tipoNPC == NPC.TIPO_NPC.SHOP)
                            {
                                estadoNpc = estadoNPC.shop;
                            }
                            else if (npcActivo != null && npcActivo.tipoNPC == NPC.TIPO_NPC.GAMBLER)
                            {
                                estadoNpc = estadoNPC.gambler;
                            }
                            else if (npcActivo != null && npcActivo.tipoNPC == NPC.TIPO_NPC.ESPECIAL)
                            {
                                estadoNpc = estadoNPC.confirmacionModoExtremo;
                            }
                            else //normal
                            {
                                estadoNpc = estadoNPC.conversacion;
                                _state = estado.normal;
                                _refControl.touchLock = true;

                                _refControl.ReanudarSonido();
                                _refControl.ReanudarPasos();
                                _refControl.ReanudarEfectos();

                                //Debug.Log("asdasd");
                            }
                        }
                    }

                    break;
                case estadoNPC.confirmacionModoExtremo:

                    GUI.DrawTexture(new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.8f, Screen.height * 0.6f), _refControl.otrasTexturas[0]);
                    estiloventana.alignment = TextAnchor.UpperCenter;
                    estiloventana.normal.textColor = Color.white;
                    string textoAux;

                    if (!CONFIG.MODO_EXTREMO)
                    {
                        textoAux = CONFIG.getTexto(161);
                    }
                    else
                    {
                        textoAux = CONFIG.getTexto(162);
                    }

                    GUI.Label(new Rect(Screen.width * 0.25f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.38f), textoAux, estiloventana);
                    estiloventana.alignment = TextAnchor.UpperLeft;
                    GUI.color = Color.green;
                    if (GUI.Button(new Rect(Screen.width * 0.15f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(160), estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        estadoNpc = estadoNPC.conversacion;
                        _state = estado.normal;
                        _refControl.touchLock = true;
                        CONFIG.MODO_EXTREMO = true;
                        _refGame.almaPoder = _refGame.almaSabiduria = _refGame.almaValor = false;

                        for (int i = 0; i < _refGame._cofresDestruidos.Length; i++)
                            _refGame._cofresDestruidos[i] = false;
                        for (int i = 0; i < _refGame._bossDerrotado.Length; i++)
                            _refGame._bossDerrotado[i] = false;
                        for (int i = 0; i < _refGame.puzzleResuelto.Length; i++)
                            _refGame.puzzleResuelto[i] = false;

                        _refGame.mapaRespawn = "mapas/Z1-0";
                        _refGame.posRespawn = new Vector2(49, 3);
                        _refGame.Guardar();
                        CONFIG.volviendoAMenu = true;
                        SceneManager.LoadScene(2);

                    }
                    GUI.color = Color.white;
                    if (GUI.Button(new Rect(Screen.width * 0.65f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);

                        estadoNpc = estadoNPC.conversacion;
                        _state = estado.normal;
                        _refControl.touchLock = true;

                        _refControl.ReanudarSonido();
                        _refControl.ReanudarPasos();
                        _refControl.ReanudarEfectos();
                    }
                    

                    break;
                case estadoNPC.shop:
                    List<Item> listaItems = null;
                    switch (_estadoShop)
                    {
                        case estadoShopNpc.Main:
                            GUI.DrawTexture(new Rect(Screen.width * 0.35f, Screen.height * 0.15f, Screen.width * 0.30f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
                            GUI.Label(new Rect(Screen.width * 0.35f, Screen.height * 0.2f, Screen.width * 0.3f, Screen.height * 0.38f), CONFIG.getTexto(17), estiloventanacentrado);
                            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.35f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(18), estiloBoton))
                            {
                                _refControl.PlaySonido(0, 0.4f);
                                _estadoShop = estadoShopNpc.Compra;
                                _npcShopOffset = 0;
                            }
                            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(19), estiloBoton))
                            {
                                _refControl.PlaySonido(0, 0.4f);
                                _estadoShop = estadoShopNpc.Venta;
                                _npcShopOffset = 0;
                                estiloinventariocentrado.normal.textColor = Color.white;
                            }
                            if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(20), estiloBoton))
                            {
                                _refControl.PlaySonido(0, 0.4f);
                                _state = estado.normal;
                                _refControl.touchLock = true;
                                _refControl.ReanudarSonido();
                                _refControl.ReanudarPasos();
                                _refControl.ReanudarEfectos();
                            }
                            break;

                        case estadoShopNpc.Compra:
                            GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.7f), _refControl.otrasTexturas[0]);
                            GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), _refControl.otrasTexturas[0]);
                            estiloinventariocentrado.normal.textColor = Color.white;
                            GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(18), estiloinventariocentrado);
                            estiloventana.normal.textColor = Color.yellow;
                            estiloventana.alignment = TextAnchor.UpperLeft;
                            GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.145f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);
                            estiloventana.normal.textColor = Color.white;
                            listaItems = npcActivo.getItemList();

                            //probar esto: poner una lista de items con items
                            //tener en cuenta que para la compra si o si tienen precio en oro
                            //pero para la venta los items que sean 'no vendibles' van a tener de precio 0g, asi que hacer la comparacion con eso
                            //de todas formas se tendrian que mostrar pero desabilitados, si hago que no aparezcan va a quedar un hueco en la tanda de 5 items
                            if (_itemSeleccionadoShop == null)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (i + _npcShopOffset < listaItems.Count)
                                    {
                                        if (_refGame.player.inventario.oro < listaItems[i + _npcShopOffset].ValorOro)
                                            estiloinventariocentrado.normal.textColor = Color.red;
                                        else
                                            estiloinventariocentrado.normal.textColor = Color.white;
                                        GUI.Label(new Rect(Screen.width * 0.7f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.1f, Screen.height * 0.08f), listaItems[i + _npcShopOffset].ValorOro.ToString() + "g", estiloinventariocentrado);
                                        estiloBoton.normal.textColor = listaItems[i + _npcShopOffset].ColorTexto;
                                        if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.4f, Screen.height * 0.08f), listaItems[i + _npcShopOffset].getNombre(), estiloBoton))
                                        {
                                            _refControl.PlaySonido(0, 0.4f);
                                            _itemSeleccionadoShop = listaItems[i + _npcShopOffset];
                                        }
                                    }
                                    else
                                    {
                                        GUI.enabled = false;
                                        GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.4f, Screen.height * 0.08f), "", estiloBoton);
                                        GUI.enabled = true;
                                    }
                                }

                                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

                                if (_npcShopOffset > 0)
                                {
                                    if (GUI.Button(new Rect(Screen.width * 0.225f, Screen.height * 0.75f, Screen.width * 0.08f, Screen.height * 0.08f), "<--", estiloBoton))
                                    {
                                        _refControl.PlaySonido(0, 0.4f);
                                        _npcShopOffset -= 5;
                                        if (_npcShopOffset < 0)
                                            _npcShopOffset = 0;
                                    }
                                }

                                if (_npcShopOffset + 5 < listaItems.Count)
                                {
                                    if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.75f, Screen.width * 0.08f, Screen.height * 0.08f), "-->", estiloBoton))
                                    {
                                        _refControl.PlaySonido(0, 0.4f);
                                        _npcShopOffset += 5;
                                    }
                                }

                                if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.75f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
                                {
                                    _refControl.PlaySonido(0, 0.4f);
                                    _estadoShop = estadoShopNpc.Main;
                                }
                            }
                            else //Mostrar item seleccionado
                            {
                                //GUI.DrawTexture(new Rect(Screen.width * 0.15f, Screen.height * 0.25f, Screen.width * 0.7f, Screen.height * 0.6f), _refControl.otrasTexturas[0]);
                                int res = DrawVentanaObjetoShop(_itemSeleccionadoShop, true);
                                if (res != 0)
                                {
                                    if (res == 1)
                                    {
                                        if (_refGame.player.inventario.oro < _itemSeleccionadoShop.ValorOro || _refGame.player.inventario.getInventarioLleno())
                                        {
                                            //esto nunca se deberia ejecutar (la opcion comprar deberia estar desabilitada) pero lo pongo como comprobacion de seguridad
                                            //_itemSeleccionadoShop = null;
                                            _refControl.PlaySonido(1, 1f);
                                        }
                                        else
                                        {
                                            _refGame.player.inventario.cambiarCantidadOro(-_itemSeleccionadoShop.ValorOro);
                                            _refGame.player.inventario.AgregarNuevoItem(_itemSeleccionadoShop);
                                            npcActivo.getItemList().Remove(_itemSeleccionadoShop);
                                            //_itemSeleccionadoShop = null;
                                        }
                                    }
                                    else
                                    {
                                        //_itemSeleccionadoShop = null;
                                    }
                                    _itemSeleccionadoShop = null;
                                }
                            }

                            break;
                        case estadoShopNpc.Venta:
                            GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.2f, Screen.width * 0.6f, Screen.height * 0.7f), _refControl.otrasTexturas[0]);
                            GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), _refControl.otrasTexturas[0]);
                            estiloinventariocentrado.normal.textColor = Color.white;
                            GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(19), estiloinventariocentrado);
                            estiloventana.normal.textColor = Color.yellow;
                            estiloventana.alignment = TextAnchor.UpperLeft;
                            GUI.Label(new Rect(Screen.width * 0.65f, Screen.height * 0.145f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);
                            estiloventana.normal.textColor = Color.white;
                            listaItems = _refGame.player.inventario.getListaItems();

                            //probar esto: poner una lista de items con items
                            //tener en cuenta que para la compra si o si tienen precio en oro
                            //pero para la venta los items que sean 'no vendibles' van a tener de precio 0g, asi que hacer la comparacion con eso
                            //de todas formas se tendrian que mostrar pero desabilitados, si hago que no aparezcan va a quedar un hueco en la tanda de 5 items
                            if (_itemSeleccionadoShop == null)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    if (i + _npcShopOffset < listaItems.Count)
                                    {
                                        if (listaItems[i + _npcShopOffset].ValorOro == 0)
                                        {
                                            GUI.enabled = false;
                                            estiloBoton.normal.textColor = listaItems[i + _npcShopOffset].ColorTexto;
                                            GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.4f, Screen.height * 0.08f), listaItems[i + _npcShopOffset].getNombre(), estiloBoton);
                                            GUI.enabled = true;
                                        }
                                        else
                                        {
                                            GUI.Label(new Rect(Screen.width * 0.7f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.1f, Screen.height * 0.08f), ((int)((listaItems[i + _npcShopOffset].ValorOro) * 0.5f)).ToString() + "g", estiloinventariocentrado);
                                            estiloBoton.normal.textColor = listaItems[i + _npcShopOffset].ColorTexto;
                                            if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.4f, Screen.height * 0.08f), listaItems[i + _npcShopOffset].getNombre(), estiloBoton))
                                            {
                                                _refControl.PlaySonido(0, 0.4f);
                                                _itemSeleccionadoShop = listaItems[i + _npcShopOffset];
                                            }
                                        }

                                    }
                                    else
                                    {
                                        GUI.enabled = false;
                                        GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.25f + i * 0.1f), Screen.width * 0.4f, Screen.height * 0.08f), "", estiloBoton);
                                        GUI.enabled = true;
                                    }
                                }

                                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

                                if (_npcShopOffset > 0)
                                {
                                    if (GUI.Button(new Rect(Screen.width * 0.225f, Screen.height * 0.75f, Screen.width * 0.08f, Screen.height * 0.08f), "<--", estiloBoton))
                                    {
                                        _refControl.PlaySonido(0, 0.4f);
                                        _npcShopOffset -= 5;
                                        if (_npcShopOffset < 0)
                                            _npcShopOffset = 0;
                                    }
                                }

                                if (_npcShopOffset + 5 < listaItems.Count)
                                {
                                    if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.75f, Screen.width * 0.08f, Screen.height * 0.08f), "-->", estiloBoton))
                                    {
                                        _refControl.PlaySonido(0, 0.4f);
                                        _npcShopOffset += 5;
                                    }
                                }

                                if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.75f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
                                {
                                    _refControl.PlaySonido(0, 0.4f);
                                    _estadoShop = estadoShopNpc.Main;
                                }
                            }
                            else //Mostrar item seleccionado
                            {
                                //GUI.DrawTexture(new Rect(Screen.width * 0.15f, Screen.height * 0.25f, Screen.width * 0.7f, Screen.height * 0.6f), _refControl.otrasTexturas[0]);
                                int res = DrawVentanaObjetoShop(_itemSeleccionadoShop, false);
                                if (res != 0)
                                {
                                    if (res == 1)
                                    {
                                        if (_refGame.player.inventario.getListaItems().Remove(_itemSeleccionadoShop))
                                        {
                                            _refGame.player.inventario.cambiarCantidadOro((int)(_itemSeleccionadoShop.ValorOro * 0.5f));
                                            npcActivo.getItemList().Add(_itemSeleccionadoShop);
                                        }
                                        else
                                        {
                                            //_mensajePopUp = "El item no se encuentra en el inventario!";
                                            // _ventanaMensajePopUp = true;
                                            Debug.LogError("error item no esta en inventario");
                                        }

                                        //_itemSeleccionadoShop = null;
                                    }
                                    else
                                    {
                                        //_itemSeleccionadoShop = null;
                                    }
                                    _itemSeleccionadoShop = null;
                                }
                            }

                            break;
                    }
                    break;

                case estadoNPC.gambler:

                    //tengo que tener definido un array teniendo un precio para cada tier
                    //y ver a que tier pertenece el jugador segun su nivel
                    //cuando compra un item estaria bueno que automaticamente se cierre el dialogo y se muestre el item al estilo drop
                    int categoriaActual = 0;
                    while (categoriaActual < 10 && _refGame.player.getNivel() >= tierLvls[categoriaActual + 1])
                    {
                        categoriaActual++;
                    }
                    GUI.DrawTexture(new Rect(Screen.width * 0.35f, Screen.height * 0.15f, Screen.width * 0.30f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
                    GUI.Label(new Rect(Screen.width * 0.35f, Screen.height * 0.2f, Screen.width * 0.3f, Screen.height * 0.38f), CONFIG.getTexto(22), estiloventanacentrado);

                    Color aux = estiloventanacentrado.normal.textColor;
                    estiloventanacentrado.normal.textColor = Color.yellow;
                    GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.35f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(21) + precioGambler[categoriaActual] + "g", estiloventanacentrado);
                    estiloventanacentrado.normal.textColor = aux;

                    if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.5f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(18), estiloBoton))
                    {
                        
                        //generar item al azar
                        Item ii = null;

                        int ic = Random.Range(0, 6);
                        float cal = Random.Range(0.0f, 100.0f); //10% legendario, 20% epico, 30% raro, 40% normal
                        Item.Calidad call;

                        if (cal <= 10.0f)
                        {
                            call = Item.Calidad.legendario;
                        }
                        else if (cal <= 30.0f)
                        {
                            call = Item.Calidad.epico;
                        }
                        else if (cal <= 60.0f)
                        {
                            call = Item.Calidad.raro;
                        }
                        else
                        {
                            call = Item.Calidad.normal;
                        }

                        switch (ic)
                        {
                            case 0:
                                ii = new Item(ITEMLIST.Armadura[categoriaActual], call);
                                break;
                            case 1:
                                ii = new Item(ITEMLIST.Espada[categoriaActual], call);
                                break;
                            case 2:
                                ii = new Item(ITEMLIST.Escudo[categoriaActual], call);
                                break;
                            case 3:
                                ii = new Item(ITEMLIST.Casco[categoriaActual], call);
                                break;
                            case 4:
                                ii = new Item(ITEMLIST.Amuleto[categoriaActual], call);
                                break;
                            case 5:
                                ii = new Item(ITEMLIST.Anillo[categoriaActual], call);
                                break;
                        }

                        if (_refGame.player.inventario.oro < precioGambler[categoriaActual] ||_refGame.player.inventario.getInventarioLleno())
                        {
                            //podria aparecer sonido de error
                            _refControl.PlaySonido(1, 1f);
                        }
                        else
                        {
                            _refControl.PlaySonido(0, 0.4f);
                            _state = estado.normal;
                            _refControl.touchLock = true;
                            _refControl.ReanudarSonido();
                            _refControl.ReanudarPasos();
                            _refControl.ReanudarEfectos();
                            _refGame.Guardar();

                            _refGame.player.inventario.cambiarCantidadOro(-precioGambler[categoriaActual]);
                            _refGame.player.inventario.AgregarNuevoItem(ii);

                            AgregarTextoConversacion(CONFIG.getTexto(24) + ii.getNombre() + " (" + ii.getCalidadTexto() + ")!");

                            //poner mensaje pop up has conseguido bla bla bla (legendario)
                        }
                    }

                    if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(20), estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _state = estado.normal;
                        _refControl.touchLock = true;
                        _refControl.ReanudarSonido();
                        _refControl.ReanudarPasos();
                        _refControl.ReanudarEfectos();
                    }
                    break;
            }

            
            
        }

        //////////////////////////////////BARRA ZOOM////////////////////////////////////////////////////
        if (state == estado.normal/* && !_refGame.pausa*/)
        {
            if (_refGame.pausa)
                GUI.enabled = false;
            if (CONFIG.mostrarPanelZoom)
            {
                GUI.skin.verticalSlider.fixedWidth = Screen.width * 0.025f;
                GUI.skin.verticalSliderThumb.fixedWidth = Screen.width * 0.025f;
                GUI.skin.verticalSlider.fixedHeight = Screen.height * 0.25f;
                GUI.skin.verticalSliderThumb.fixedHeight = Screen.height * 0.025f;
                CONFIG.escala = GUI.VerticalSlider(new Rect(Screen.width * 0.95f, Screen.height * 0.375f, Screen.width * 0.05f, Screen.height * 0.25f), CONFIG.escala, 2f, 0.5f);
            }
            GUI.enabled = true;
        }

        

        /////////////////////////////////TITULO FADE IN-OUT///////////////////////////////////////////////
        if (TituloZonaFade())
        {
            int fAux = estiloHud.fontSize;
            estiloHud.fontSize = UTIL.TextoProporcion(100);
            GUI.color = new Color(1f, 1f, 1f, _porcentajeFadeTitulo);
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), _tituloAMostrar, estiloHud);
            GUI.color = Color.white;
            estiloHud.fontSize = fAux;
        }


        //no se si esto deberia aparecer por ensima de las ventanas de texto
        ///////////////////////////////////////////////////////////////Dibujar HUD////////////////////////////////////////////////////////////
        /*GUI.DrawTexture(new Rect(Screen.width * 0.316f, Screen.height * 0.025f, Screen.width * (1.0f - 0.316f * 2), Screen.height * 0.075f), _refControl.otrasTexturas[1]);
        GUI.DrawTexture(new Rect(Screen.width * (0.34f), Screen.height * 0.025f, Screen.width * (1.0f - 0.34f * 2) * ((float)_refGame.player.getHp() / (float)_refGame.player.getHpMax()), Screen.height * 0.075f), _refControl.otrasTexturas[2], ScaleMode.ScaleAndCrop);
        GUI.DrawTexture(new Rect(Screen.width * 0.316f, Screen.height * 0.025f, Screen.width * (1.0f - 0.316f * 2), Screen.height * 0.075f), _refControl.otrasTexturas[42]);
        */

        GUI.DrawTexture(new Rect(Screen.width * 0.225f, Screen.height * 0.020f, Screen.width * (1.0f - 0.225f * 2), Screen.height * 0.09f), _refControl.otrasTexturas[1]);
        GUI.DrawTexture(new Rect(Screen.width * (0.26f), Screen.height * 0.02f, Screen.width * (1.0f - 0.26f * 2) * ((float)_refGame.player.getHp() / (float)_refGame.player.getHpMax()), Screen.height * 0.09f), _refControl.otrasTexturas[2]);//, ScaleMode.ScaleAndCrop);
        GUI.DrawTexture(new Rect(Screen.width * 0.225f, Screen.height * 0.020f, Screen.width * (1.0f - 0.225f * 2), Screen.height * 0.09f), _refControl.otrasTexturas[42]);

        GUI.Label(new Rect(Screen.width * 0.32f, Screen.height * 0.025f, Screen.width * (1.0f - 0.32f * 2), Screen.height * 0.075f), "Hp: " + _refGame.player.getHp() + "/" + _refGame.player.getHpMax(), estiloHud);

        //boton pausa
        GUI.DrawTexture(new Rect(Screen.width * 0.01f, Screen.width * 0.01f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[3]);

        //grafico visual experiencia ganada
        //GUI.DrawTexture(new Rect(Screen.width * 0.9f, Screen.height * 0.01f, Screen.width * (0.09f * _refGame.player.Exp/(float)_refGame.player.ExpMax), Screen.width * 0.1f), _refControl.otrasTexturas[16]);
        
        GUI.BeginGroup(new Rect(Screen.width * 0.9f, Screen.height * (0.01f) + Screen.width * 0.1f - Screen.width * 0.1f * ((_refGame.player.getNivel() == 99) ? (1f) : (_refGame.player.Exp / (float)_refGame.player.ExpMax)), Screen.width * 0.09f, Screen.width * 0.1f));
        GUI.DrawTexture(new Rect(0, -Screen.width * 0.1f + Screen.width * 0.1f * ((_refGame.player.getNivel()==99)?(1f):(_refGame.player.Exp / (float)_refGame.player.ExpMax)), Screen.width * 0.09f, Screen.width * 0.1f), _refControl.otrasTexturas[16]);
        GUI.EndGroup();
        GUI.DrawTexture(new Rect(Screen.width * 0.9f, Screen.height * 0.01f, Screen.width * 0.09f, Screen.width * 0.1f), _refControl.otrasTexturas[15]);
        GUI.Label(new Rect(Screen.width * 0.9f, Screen.height * 0.01f, Screen.width * 0.09f, Screen.width * 0.1f), (int)(100.0f * ((_refGame.player.getNivel() == 99) ? (1f) : (_refGame.player.Exp / (float)_refGame.player.ExpMax))) + "%", estiloHud);

        if (habilidadSinAprender)
        {
            GUI.DrawTexture(new Rect(Screen.width * 0.02f, Screen.height * 0.2f, Screen.width * 0.05f, Screen.width * 0.05f), _refControl.otrasTexturas[39]);
        }

        if (_refGame.player.inventario.getInventarioLleno())
        {
            GUI.DrawTexture(new Rect(Screen.width * 0.02f, Screen.height * 0.4f, Screen.width * 0.05f, Screen.width * 0.05f), _refControl.otrasTexturas[48]);
        }

        //habilidades incluyendo caminar
        if (!_habilidadesEscondidas)
        {
            if (_refGame.player.skillElegida == 0)
            {
                GUI.color = Color.yellow;
                GUI.DrawTexture(new Rect(Screen.width * 0.15f, Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[8]);
                GUI.color = Color.white;
            }
            else
                GUI.DrawTexture(new Rect(Screen.width * 0.15f, Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[8]);

            GUIStyle estilo = new GUIStyle();
            estilo.normal.textColor = Color.white;
            estilo.fontSize = UTIL.TextoProporcion(30);

            for (int i = 0; i < 5; i++)
            {
                if (_refGame.player.getSkill(i) != null)
                {
                    if (_refGame.player.skillElegida == i + 1)
                        GUI.color = Color.yellow;
                    else
                        GUI.color = Color.white;
                    if (_refGame.player.getSkill(i).enCooldown)
                    {
                        GUI.color = new Color(GUI.color.r * 0.5f, GUI.color.g * 0.5f, GUI.color.b * 0.5f);
                        GUI.DrawTexture(new Rect(Screen.width * (0.3f + 0.1f * i), Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refGame.player.getSkill(i).refIcono);
                        GUI.color = Color.white;
                        GUI.Label(new Rect(Screen.width * (0.31f + 0.1f * i), Screen.height * 0.89f, Screen.width * 0.075f, Screen.width * 0.075f), ((double)(_refGame.player.getSkill(i).cooldownRestante)).ToString("0.0"), estilo);
                    }
                    else
                    {
                        GUI.DrawTexture(new Rect(Screen.width * (0.3f + 0.1f * i), Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refGame.player.getSkill(i).refIcono);
                    }


                }
                else
                {
                    GUI.color = Color.white;
                    GUI.DrawTexture(new Rect(Screen.width * (0.3f + 0.1f * i), Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[6]);
                }

            }
            GUI.color = Color.white;

            //GUI.DrawTexture(new Rect(Screen.width * 0.8f, Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[12]);
        }
        //else
            //GUI.DrawTexture(new Rect(Screen.width * 0.8f, Screen.height * 0.85f, Screen.width * 0.075f, Screen.width * 0.075f), _refControl.otrasTexturas[11]);

        if (_refGame.pausa && currentVentanaPausa != ventanaPausa.nada)
        {
            if (currentVentanaPausa == ventanaPausa.main)
            {
                Draw_menuPausa();
            }
            if (currentVentanaPausa == ventanaPausa.inventario)
            {
                DrawMenuInventario();
            }
            if (currentVentanaPausa == ventanaPausa.habilidades)
            {
                DrawMenuHabilidades();
            }
            if (currentVentanaPausa == ventanaPausa.Configuracion)
            {
                DrawMenuConfig();
            }
            if (currentVentanaPausa == ventanaPausa.salir)
            {
                DrawMenuSalir();
            }
        }
    }

    /// <summary>
    /// Cada vez que se toca en la pantalla, comprueba a partir de la posicion de pantalla si toca algun boton.
    /// </summary>
    /// <param name="mpos"></param>
    /// <returns></returns>
    public int TouchHud(Vector2 mpos)
    {
        mpos.y = Screen.height - mpos.y;

        //boton pausa
        if (mpos.x >= Screen.width * 0.01f && mpos.x <= Screen.width * (0.01f + 0.075f) && mpos.y >= Screen.height * 0.01f && mpos.y <= Screen.height * 0.01f + Screen.width * 0.075f)
        {
            Pausar();
            return 0;
        }

        //panel de zoom
        if (CONFIG.mostrarPanelZoom && (mpos.x >= Screen.width * 0.925f && mpos.x <= Screen.width * (0.925f + 0.075f) && mpos.y >= Screen.height * 0.35f && mpos.y <= Screen.height * (0.35f + 0.75f)))
        {
            return 0;
        }

        if (!_habilidadesEscondidas)
        {
            //skill caminar
            if (mpos.x >= Screen.width * 0.15f && mpos.x <= Screen.width * (0.15f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                _refGame.player.skillElegida = 0;
                return 1;
            }

            //skill1
            if (mpos.x >= Screen.width * 0.3f && mpos.x <= Screen.width * (0.3f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                if (_refGame.player.existeSkill(0))
                    _refGame.player.skillElegida = 1;
                return 2;
            }
            //skill2
            if (mpos.x >= Screen.width * 0.4f && mpos.x <= Screen.width * (0.4f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                if (_refGame.player.existeSkill(1))
                    _refGame.player.skillElegida = 2;
                return 3;
            }
            //skill3
            if (mpos.x >= Screen.width * 0.5f && mpos.x <= Screen.width * (0.5f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                if (_refGame.player.existeSkill(2))
                    _refGame.player.skillElegida = 3;
                return 4;
            }
            //skill4
            if (mpos.x >= Screen.width * 0.6f && mpos.x <= Screen.width * (0.6f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                if (_refGame.player.existeSkill(3))
                    _refGame.player.skillElegida = 4;
                return 5;
            }
            //skill5
            if (mpos.x >= Screen.width * 0.7f && mpos.x <= Screen.width * (0.7f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _refControl.PlaySonido(0, 0.4f);
                if (_refGame.player.existeSkill(4))
                    _refGame.player.skillElegida = 5;
                return 6;
            }
            //ocultarSkills
            /*if (mpos.x >= Screen.width * 0.8f && mpos.x <= Screen.width * (0.8f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _habilidadesEscondidas = true;
                _refControl.touchLock = true;
                _refGame.player.skillElegida = 0;
                return 7;
            }*/
        }
        else //mostrarSkills
        {
            /*if (mpos.x >= Screen.width * 0.8f && mpos.x <= Screen.width * (0.8f + 0.075f) && mpos.y >= Screen.height * 0.85f && mpos.y <= Screen.height * 0.85f + Screen.width * 0.075f)
            {
                _habilidadesEscondidas = false;
                _refControl.touchLock = true;
                return 7;
            }*/
        }


        return -1;
    }

    private void Draw_menuPausa()
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(34);

        estiloBoton.fontSize = UTIL.TextoProporcion(28);

        estiloinventario.normal.textColor = new Color(0.9f, 0.6f, 0.0f);
        estiloinventario.fontSize = UTIL.TextoProporcion(30);

        estiloinventario2.fontSize = UTIL.TextoProporcion(30);
        estiloinventario2.normal.textColor = Color.white;
        estiloinventario2.alignment = TextAnchor.UpperRight;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.DrawTexture(new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.55f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.2f, Screen.width * 0.55f, Screen.height * 0.65f), "");
        GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), (!CONFIG.MODO_EXTREMO)?(CONFIG.getTexto(25)): (CONFIG.getTexto(26)), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(27) + _refGame.player.getNivel(), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.1f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);

        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.23f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(29), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.29f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(30), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.35f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(28), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.41f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(31), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.47f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(32), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.53f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(33), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.59f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(34), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.65f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(35), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.71f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(36), estiloinventario);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.77f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(37), estiloinventario);

        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.23f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.getHp() + "/" + _refGame.player.getHpMax(), estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.29f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.Exp + "/" + _refGame.player.ExpMax, estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.35f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.getDmgMin() + " - " + _refGame.player.getDmgMax(), estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.41f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.getDmgMinBase() + " - " + _refGame.player.getDmgMaxBase(), estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.47f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.GolpeCritico + "%", estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.53f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.ReduccionCD + "%", estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.59f, Screen.width * 0.5f, Screen.height * 0.38f), ((int)_refGame.player.Armadura).ToString(), estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.65f, Screen.width * 0.5f, Screen.height * 0.38f), (100 - _refGame.player.ReduccionDmg).ToString("0.##") + "%", estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.71f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.Esquivar + "%", estiloinventario2);
        GUI.Label(new Rect(Screen.width * 0.125f, Screen.height * 0.77f, Screen.width * 0.5f, Screen.height * 0.38f), _refGame.player.VelMovimiento + "%", estiloinventario2);

        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.2f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            _refGame.pausa = false;
            _refControl.touchLock = true;
            Game.ultimoTiempoTranscurridoPartida = Time.time;
            comprobarHabilidadSinAprender();
            _refGame.Guardar();
            _refControl.ReanudarMusica();
            _refControl.ReanudarSonido();
            _refControl.ReanudarPasos();
            _refControl.ReanudarEfectos();
        }
        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.35f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(38), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.inventario;
        }
        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.5f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(39), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.habilidades;
        }
        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.65f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(1), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.Configuracion;
        }
        if (GUI.Button(new Rect(Screen.width * 0.7f, Screen.height * 0.8f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(40), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.salir;
        }
    }

    private void DrawMenuInventario()
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
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), "");
        GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), (!CONFIG.MODO_EXTREMO) ? (CONFIG.getTexto(25)) : (CONFIG.getTexto(26)), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(27) + _refGame.player.getNivel(), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.1f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);

        GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), _refControl.otrasTexturas[0]);

        if (_verEquipadoEnVentanaInventario)
        {
            GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(41), estiloinventariocentrado);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.25f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(42), estiloinventario2);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.35f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(43), estiloinventario2);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.45f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(34), estiloinventario2);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.55f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(44), estiloinventario2);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.65f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(45), estiloinventario2);
            GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * 0.75f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(46), estiloinventario2);

            if (_itemMostradoEnVentanaInventario == null)
            {
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("casco");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.22f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("casco"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoCasco;
                }
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("amuleto");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.32f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("amuleto"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoAmuleto;
                }
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("armadura");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.42f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("armadura"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoArmadura;
                }
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("anillo");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.52f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("anillo"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoAnillo;
                }
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("arma");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.62f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("arma"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoArma;
                }
                estiloBoton.normal.textColor = _refGame.player.inventario.getColorItemEquipado("escudo");
                if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.72f, Screen.width * 0.6f, Screen.height * 0.1f), _refGame.player.inventario.getNameEquipado("escudo"), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    _itemMostradoEnVentanaInventario = _refGame.player.inventario.equipadoEscudo;
                }

                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

                if (GUI.Button(new Rect(Screen.width * 0.025f, Screen.height * 0.875f, Screen.width * 0.25f, Screen.height * 0.1f), (_verEquipadoEnVentanaInventario) ? CONFIG.getTexto(38) : CONFIG.getTexto(41), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    if (_verEquipadoEnVentanaInventario)
                        _verEquipadoEnVentanaInventario = false;
                    else
                        _verEquipadoEnVentanaInventario = true;
                    _scrollOffsetEnVentanaInventario = 0;
                }
                
                if (GUI.Button(new Rect(Screen.width * 0.725f, Screen.height * 0.875f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(48), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    currentVentanaPausa = ventanaPausa.main;
                }
            }
            else //itemMostrado != null
            {
                if (DrawVentanaObjetoDescripcion(_itemMostradoEnVentanaInventario))
                {
                    //si pone cerrar ventana
                    _itemMostradoEnVentanaInventario = null;
                }
                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
                if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(47), estiloBoton) )
                {
                    if (_refGame.player.inventario.getInventarioLleno())
                    {
                        //reproducir algun sonido
                        _refControl.PlaySonido(1, 1f);
                    }
                    else
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        if (_refGame.player.inventario.equipadoArma != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoArma.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoArma = null;
                        }
                        else if (_refGame.player.inventario.equipadoArmadura != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoArmadura.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoArmadura = null;
                        }
                        else if (_refGame.player.inventario.equipadoCasco != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoCasco.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoCasco = null;
                        }
                        else if (_refGame.player.inventario.equipadoEscudo != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoEscudo.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoEscudo = null;
                        }
                        else if (_refGame.player.inventario.equipadoAnillo != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoAnillo.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoAnillo = null;
                        }
                        else if (_refGame.player.inventario.equipadoAmuleto != null && _itemMostradoEnVentanaInventario.CodigoItem == _refGame.player.inventario.equipadoAmuleto.CodigoItem)
                        {
                            _refGame.player.inventario.equipadoAmuleto = null;
                        }
                        else
                        {
                            if (CONFIG.DEBUG) Debug.LogWarning("ERROR ITEM EQUIPADO NO COINCIDE");
                            return;
                        }


                        _refGame.player.inventario.AgregarNuevoItem(_itemMostradoEnVentanaInventario);
                        _itemMostradoEnVentanaInventario = null;
                    }
                    
                }
            }

        }
        else //inventario
        {
            GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(38), estiloinventariocentrado);
            if (_itemMostradoEnVentanaInventario == null)
            {
                for (int i = _scrollOffsetEnVentanaInventario; (i < _refGame.player.inventario.getTamInventario() && i < (_scrollOffsetEnVentanaInventario + 6)); i++)
                {
                    string texto = "";
                    if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Amuleto)
                        texto = CONFIG.getTexto(55);
                    else if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Anillo)
                        texto = CONFIG.getTexto(56);
                    else if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Armadura)
                        texto = CONFIG.getTexto(57);
                    else if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Casco)
                        texto = CONFIG.getTexto(58);
                    else if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Escudo)
                        texto = CONFIG.getTexto(59);
                    else if (_refGame.player.inventario.getItemByIndex(i).Tipo == Item.TipoItem.Espada)
                        texto = CONFIG.getTexto(60);

                    GUI.Label(new Rect(Screen.width * 0.075f, Screen.height * (0.25f + 0.1f * (i - _scrollOffsetEnVentanaInventario)), Screen.width * 0.5f, Screen.height * 0.38f), texto, estiloinventario2);
                    estiloBoton.normal.textColor = _refGame.player.inventario.getItemByIndex(i).ColorTexto;
                    if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * (0.22f + 0.1f * (i - _scrollOffsetEnVentanaInventario)), Screen.width * 0.6f, Screen.height * 0.1f), (i + 1).ToString() + ". " + _refGame.player.inventario.getItemByIndex(i).Nombre, estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _itemMostradoEnVentanaInventario = _refGame.player.inventario.getItemByIndex(i);
                    }
                }
                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
                if (_scrollOffsetEnVentanaInventario > 0)
                {
                    if (GUI.Button(new Rect(Screen.width * 0.86f, Screen.height * 0.215f, Screen.width * 0.08f, Screen.height * 0.08f), CONFIG.getTexto(61), estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _scrollOffsetEnVentanaInventario -= 6;
                        if (_scrollOffsetEnVentanaInventario < 0)
                            _scrollOffsetEnVentanaInventario = 0;
                    }
                }

                if (_scrollOffsetEnVentanaInventario + 6 < _refGame.player.inventario.getTamInventario())
                {
                    if (GUI.Button(new Rect(Screen.width * 0.86f, Screen.height * 0.75f, Screen.width * 0.08f, Screen.height * 0.08f), CONFIG.getTexto(62), estiloBoton))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _scrollOffsetEnVentanaInventario += 6;
                    }
                }

                if (_refGame.player.inventario.getTamInventario() == _refGame.player.inventario.getTamInventarioMax())
                {
                    estiloinventariocentrado.normal.textColor = Color.red;
                }
                else
                {
                    estiloinventariocentrado.normal.textColor = Color.white;
                }
                GUI.Label(new Rect(Screen.width * 0f, Screen.height * 0.875f, Screen.width * 1f, Screen.height * 0.1f), "Items: " + _refGame.player.inventario.getTamInventario() + "/" + _refGame.player.inventario.getTamInventarioMax(), estiloinventariocentrado);
                estiloinventariocentrado.normal.textColor = Color.white;

                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);

                if (GUI.Button(new Rect(Screen.width * 0.025f, Screen.height * 0.875f, Screen.width * 0.25f, Screen.height * 0.1f), (_verEquipadoEnVentanaInventario) ? CONFIG.getTexto(38) : CONFIG.getTexto(41), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    if (_verEquipadoEnVentanaInventario)
                        _verEquipadoEnVentanaInventario = false;
                    else
                        _verEquipadoEnVentanaInventario = true;
                    _scrollOffsetEnVentanaInventario = 0;
                }
                if (GUI.Button(new Rect(Screen.width * 0.725f, Screen.height * 0.875f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(48), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    currentVentanaPausa = ventanaPausa.main;
                }
            }
            else //itemMostrado != null
            {
                if (DrawVentanaObjetoDescripcion(_itemMostradoEnVentanaInventario, true))
                {
                    //si pone cerrar ventana
                    _itemMostradoEnVentanaInventario = null;
                }
                estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
                if (_refGame.player.esEquipable(_itemMostradoEnVentanaInventario) && GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(63), estiloBoton))
                {
                    _refControl.PlaySonido(0, 0.4f);
                    Item aux = null;
                    if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Espada)
                    {
                        if (_refGame.player.inventario.equipadoArma != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoArma;
                        }
                        _refGame.player.inventario.equipadoArma = _itemMostradoEnVentanaInventario;

                    }
                    else if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Armadura)
                    {
                        if (_refGame.player.inventario.equipadoArmadura != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoArmadura;
                        }
                        _refGame.player.inventario.equipadoArmadura = _itemMostradoEnVentanaInventario;

                    }
                    else if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Casco)
                    {
                        if (_refGame.player.inventario.equipadoCasco != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoCasco;
                        }
                        _refGame.player.inventario.equipadoCasco = _itemMostradoEnVentanaInventario;

                    }
                    else if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Escudo)
                    {
                        if (_refGame.player.inventario.equipadoEscudo != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoEscudo;
                        }
                        _refGame.player.inventario.equipadoEscudo = _itemMostradoEnVentanaInventario;

                    }
                    else if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Amuleto)
                    {
                        if (_refGame.player.inventario.equipadoAmuleto != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoAmuleto;
                        }
                        _refGame.player.inventario.equipadoAmuleto = _itemMostradoEnVentanaInventario;

                    }
                    else if (_itemMostradoEnVentanaInventario.Tipo == Item.TipoItem.Anillo)
                    {
                        if (_refGame.player.inventario.equipadoAnillo != null)
                        {
                            //desequipar
                            aux = _refGame.player.inventario.equipadoAnillo;
                        }
                        _refGame.player.inventario.equipadoAnillo = _itemMostradoEnVentanaInventario;

                    }
                    else
                    {
                        if (CONFIG.DEBUG) Debug.LogWarning("ERROR ITEM EQUIPADO NO COINCIDE");
                        return;
                    }

                    for (int i = 0; i < _refGame.player.inventario.getTamInventario(); i++)
                    {
                        if (Object.ReferenceEquals(_itemMostradoEnVentanaInventario, _refGame.player.inventario.getItemByIndex(i)))
                        {
                            _refGame.player.inventario.sacarItemInventarioByIndex(i);
                            _itemMostradoEnVentanaInventario = null;
                            _refGame.player.inventario.AgregarNuevoItem(aux);
                            break;
                        }
                    }
                }
            }
        }
        /*
        if (_itemMostradoEnVentanaInventario != null)
        {
            GUI.enabled = false;
        }

        

        GUI.enabled = true;*/
    }

    private bool DrawVentanaObjetoDescripcion(Item it, bool comparacion = false)
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(32);
        estiloventana.alignment = TextAnchor.UpperCenter;

        estiloBoton.fontSize = UTIL.TextoProporcion(28);

        GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.15f, Screen.width * 0.6f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        estiloventana.normal.textColor = it.ColorTexto;

        int valorComparacion = 0;

        if (comparacion)
        {
            valorComparacion = it.ValorTotal;
            if (_refGame.player.inventario.getEquipadoSegunTipo(it.Tipo) != null)
                valorComparacion -= _refGame.player.inventario.getEquipadoSegunTipo(it.Tipo).ValorTotal;
        }

        string calidadtexto = "";

        int i = 0, iMax = 1;

        if (it.getCalidad == Item.Calidad.normal)
        {
            calidadtexto = "(Normal)";
            iMax = 1;
        }
        else if (it.getCalidad == Item.Calidad.raro)
        {
            calidadtexto = CONFIG.getTexto(49);
            iMax = 2;
        }
        else if (it.getCalidad == Item.Calidad.epico)
        {
            calidadtexto = CONFIG.getTexto(50);
            iMax = 3;
        }
        else if (it.getCalidad == Item.Calidad.legendario)
        {
            calidadtexto = CONFIG.getTexto(51);
            iMax = 4;
        }

        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.17f, Screen.width * 0.6f, Screen.height * 0.38f), it.Nombre + " " + calidadtexto, estiloventana);
        estiloventana.normal.textColor = Color.white;

        if (it.getTipoAtributo(0) != Item.ATRIBUTO.ARMADURA)    //armadura se muestra como numero, y los demas como porcentaje
        {
            GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(0)) + ": " + it.getValorAtributo(i) + "% (" + it.ValorBase + "% + " + (it.ValorTotal - it.ValorBase).ToString() + "%)", estiloventana);
        }
        else
        {
            GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(0)) + ": " + it.ValorTotal + " (" + it.ValorBase + " + " + (it.ValorTotal - it.ValorBase).ToString() + ")", estiloventana);

        }

        if (comparacion && _refGame.player.esEquipable(it))
        {
            string texto = "";
            estiloventana.alignment = TextAnchor.UpperLeft;
            //elegir color de texto segun si es + n - y restaurar color, poner en un label diferente pero a misma altura de y. y tambien poner en el de abajo
            if (valorComparacion > 0)
            {
                estiloventana.normal.textColor = Color.green;
                texto = "+" + valorComparacion.ToString();
            }
            else if (valorComparacion < 0)
            {
                estiloventana.normal.textColor = Color.red;
                texto = valorComparacion.ToString();
            }
            else
            {
                estiloventana.normal.textColor = Color.white;
                texto = valorComparacion.ToString();
            }
            GUI.Label(new Rect(Screen.width * 0.725f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), "(" + texto + ")", estiloventana);
            estiloventana.alignment = TextAnchor.UpperCenter;
        }


        for (i = 1; i < iMax; i++)  //PARA LOS ATRIBUTOS SECUNDARIOS
        {
            estiloventana.normal.textColor = Color.white;

            if (it.getTipoAtributo(i) == Item.ATRIBUTO.NADA)
                continue;

            if (it.getTipoAtributo(i) != Item.ATRIBUTO.ARMADURA)    //armadura se muestra como numero, y los demas como porcentaje
            {
                GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * (0.3f + 0.06f * i), Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(i)) + ": " + it.getValorAtributo(i) + "%", estiloventana);
            }
            else
            {
                GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * (0.3f + 0.06f * i), Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(i)) + ": " + it.getValorAtributo(i), estiloventana);

            }
        }



        estiloventana.normal.textColor = Color.yellow;
        
        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.55f, Screen.width * 0.6f, Screen.height * 0.38f), CONFIG.getTexto(52) + it.ValorOro + CONFIG.getTexto(53), estiloventana);

        if (it.lvl > _refGame.player.getNivel())
        {
            estiloventana.normal.textColor = Color.red;
        }
        else
        {
            estiloventana.normal.textColor = Color.white;
        }
        estiloventana.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(Screen.width * 0.225f, Screen.height * 0.24f, Screen.width * 0.6f, Screen.height * 0.38f), CONFIG.getTexto(54) + it.lvl, estiloventana);

        if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), "Ok", estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            return true;
        }

        return false;
    }

    private void DrawMenuHabilidades()
    {
        estiloventana.normal.textColor = Color.yellow;
        estilocentrado.normal.textColor = Color.white;

        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");
        GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "");

        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), "");
        GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), (!CONFIG.MODO_EXTREMO) ? (CONFIG.getTexto(25)) : (CONFIG.getTexto(26)), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(27) + _refGame.player.getNivel(), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.1f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);

        GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), "");
        GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(39), estilocentrado);

        GUI.Box(new Rect(Screen.width * 0.09f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.21f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.33f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.45f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.57f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.69f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
        GUI.Box(new Rect(Screen.width * 0.81f, Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);

        int i = 0;
        int[] niveles = new int[] { 1, 5, 12, 25, 40, 50, 80 };


        foreach (int nivel in niveles)
        {
            char estadoSkill = 'b';
            bool bloqueado = _refGame.player.getNivel() < nivel;
            GUI.Box(new Rect(Screen.width * (0.09f + 0.12f * i), Screen.height * 0.325f, Screen.width * 0.1f, Screen.height * 0.45f), "", style);
            if (bloqueado)
                estilocentrado2.normal.textColor = new Color(0.8f, 0f, 0f);
            GUI.Label(new Rect(Screen.width * (0.09f + 0.12f * i), Screen.height * 0.275f, Screen.width * 0.1f, Screen.height * 0.1f), CONFIG.getTexto(54) + nivel, estilocentrado2);
            estilocentrado2.normal.textColor = Color.white;

            if (bloqueado)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.5f);
            }

            switch (i)
            {
                case 0:
                    estadoSkill = 'a';
                    GUI.DrawTexture(new Rect(Screen.width * 0.1f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[0].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.1f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[0];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;

                case 1:
                    if (!bloqueado)
                    {
                        if (_refGame.player.getSkill(1) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(1).Comparar(_refGame.player.skillsAuxiliar[1]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }


                    GUI.DrawTexture(new Rect(Screen.width * 0.22f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[1].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.22f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[1];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }

                    if (!bloqueado)
                    {
                        if (_refGame.player.getSkill(1) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(1).Comparar(_refGame.player.skillsAuxiliar[2]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }

                    GUI.DrawTexture(new Rect(Screen.width * 0.22f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[2].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.22f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[2];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;

                case 2:

                    if (!bloqueado && _refGame.player.getSkill(1) != null)
                    {
                        if (_refGame.player.getPasiva(0) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else
                        {
                            estadoSkill = 'a';
                            GUI.color = Color.white;
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.34f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[9].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.34f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[9];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;

                case 3:
                    if (!bloqueado && (_refGame.player.getPasiva(0) != null))
                    {
                        if (_refGame.player.getSkill(2) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(2).Comparar(_refGame.player.skillsAuxiliar[3]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                    {
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);
                        estadoSkill = 'b';
                    }

                    GUI.DrawTexture(new Rect(Screen.width * 0.46f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[3].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.46f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[3];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }

                    if (!bloqueado && (_refGame.player.getPasiva(0) != null))
                    {
                        if (_refGame.player.getSkill(2) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(2).Comparar(_refGame.player.skillsAuxiliar[4]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.46f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[4].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.46f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[4];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;


                case 4:

                    if (!bloqueado && (_refGame.player.getSkill(2) != null))
                    {
                        if (_refGame.player.getSkill(3) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(3).Comparar(_refGame.player.skillsAuxiliar[5]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.58f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[5].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.58f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[5];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }

                    if (!bloqueado && (_refGame.player.getSkill(2) != null))
                    {
                        if (_refGame.player.getSkill(3) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(3).Comparar(_refGame.player.skillsAuxiliar[6]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);
                    GUI.DrawTexture(new Rect(Screen.width * 0.58f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[6].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.58f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[6];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;

                case 5:

                    if (!bloqueado && (_refGame.player.getSkill(3) != null))
                    {
                        if (_refGame.player.getSkill(4) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(4).Comparar(_refGame.player.skillsAuxiliar[7]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.70f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[7].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.70f, Screen.height * 0.36f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[7];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }

                    if (!bloqueado && (_refGame.player.getSkill(3) != null))
                    {
                        if (_refGame.player.getSkill(4) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else if (_refGame.player.getSkill(4).Comparar(_refGame.player.skillsAuxiliar[8]))
                        {
                            GUI.color = Color.white;
                            estadoSkill = 'a';
                        }
                        else
                        {
                            GUI.color = new Color(0.5f, 0.1f, 0.1f, 0.5f);
                            estadoSkill = 'n';
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.70f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[8].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.70f, Screen.height * 0.6f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[8];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;
                
                case 6:

                    if (!bloqueado && _refGame.player.getSkill(4) != null)
                    {
                        if (_refGame.player.getPasiva(1) == null)
                        {
                            GUI.color = new Color(0f, 1f, 1f, 1f);
                            estadoSkill = 'd';
                        }
                        else
                        {
                            estadoSkill = 'a';
                            GUI.color = Color.white;
                        }
                    }
                    else
                        GUI.color = new Color(1f, 1f, 1f, 0.5f);

                    GUI.DrawTexture(new Rect(Screen.width * 0.82f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), _refGame.player.skillsAuxiliar[10].refIcono);
                    if (GUI.Button(new Rect(Screen.width * 0.82f, Screen.height * 0.48f, Screen.width * 0.08f, Screen.width * 0.08f), "", estilocentrado))
                    {
                        _refControl.PlaySonido(0, 0.4f);
                        _refGame.player.skillAux = _refGame.player.skillsAuxiliar[10];
                        _refGame.player.estadoSkillDisplay = estadoSkill;
                    }
                    break;
            }
            GUI.color = Color.white;

            if (_refGame.player.skillAux != null)
            {
                //Debug.Log("ASDASD");
                if (DrawVentanaSkillDescripcion(_refGame.player.skillAux, _refGame.player.estadoSkillDisplay))
                {
                    _refGame.player.skillAux = null;
                }
                else
                    GUI.enabled = false;
            }

            i++;
        }



        if (GUI.Button(new Rect(Screen.width * 0.725f, Screen.height * 0.875f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(48), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.main;
        }

        GUI.enabled = true;
    }

    private bool DrawVentanaSkillDescripcion(Skill s, char e = 'b')
    {
        GUI.enabled = true;

        estilocentrado.fontSize = UTIL.TextoProporcion(34);
        estilocentrado.alignment = TextAnchor.UpperCenter;

        if (s.pasiva)
        {
            estilocentrado.normal.textColor = new Color(0f, 0.7f, 0f);
        }
        else
        {
            estilocentrado.normal.textColor = new Color(0.2f, 0.2f, 1f);
        }



        estilocentrado2.alignment = TextAnchor.UpperCenter;
        estilocentrado2.fontSize = UTIL.TextoProporcion(28);
        estilocentrado2.normal.textColor = Color.white;

        estiloBoton.fontSize = UTIL.TextoProporcion(28);
        //estiloBoton.normal.textColor = Color.white;


        GUI.DrawTexture(new Rect(Screen.width * 0.2f, Screen.height * 0.15f, Screen.width * 0.6f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        GUI.Label(new Rect(0f, Screen.height * 0.18f, Screen.width * 1f, Screen.height * 0.1f), s.Nombre, estilocentrado);
        GUI.DrawTexture(new Rect(Screen.width * 0.45f, Screen.height * 0.25f, Screen.width * 0.1f, Screen.width * 0.1f), s.refIcono);
        GUI.Label(new Rect(0f, Screen.height * 0.45f, Screen.width * 1f, Screen.height * 0.7f), s.Descripcion, estilocentrado2);

        if (e == 'b') //bloqueado
        {
            GUI.enabled = false;
            if (GUI.Button(new Rect(Screen.width * 0.63f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), CONFIG.getTexto(64), estiloBoton))
            {

            }

            GUI.enabled = true;
        }
        if (e == 'd') //disponible
        {
            GUI.enabled = true;
            if (GUI.Button(new Rect(Screen.width * 0.63f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), CONFIG.getTexto(64), estiloBoton))
            {
                _refControl.PlaySonido(0, 0.4f);
                if (s.pasiva)
                    _refGame.player.NuevaPasiva(s);
                else
                    _refGame.player.NuevaSkill(s);
                //estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
                return true;
            }
        }

        if (e == 'a') //ya aprendida
        {
            GUI.enabled = false;
            if (GUI.Button(new Rect(Screen.width * 0.63f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), CONFIG.getTexto(65), estiloBoton))
            {

            }

            GUI.enabled = true;
        }

        if (e == 'n') //no disponible
        {
            GUI.enabled = false;
            // if (GUI.Button(new Rect(Screen.width * 0.63f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), "NO DISPONIBLE", estiloBoton))
            if (GUI.Button(new Rect(Screen.width * 0.63f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), "---", estiloBoton))
            {

            }

            GUI.enabled = true;
        }

        if (GUI.Button(new Rect(Screen.width * 0.22f, Screen.height * 0.7f, Screen.width * 0.15f, Screen.height * 0.07f), "Ok", estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            //estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
            return true;
        }
        //estiloBoton.normal.textColor = new Color(0.0f, 1.0f, 0.1f);
        return false;
    }

    private void DrawMenuConfig()
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
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), "");
        GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), (!CONFIG.MODO_EXTREMO) ? (CONFIG.getTexto(25)) : (CONFIG.getTexto(26)), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(27) + _refGame.player.getNivel(), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.1f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);

        GUI.DrawTexture(new Rect(Screen.width * 0.4f, Screen.height * 0.13f, Screen.width * 0.2f, Screen.height * 0.07f), _refControl.otrasTexturas[0]);

        GUI.Label(new Rect(0f, Screen.height * 0.145f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(1), estiloinventariocentrado);


        GUI.skin.horizontalSlider.fixedWidth = Screen.width * 0.25f;
        GUI.skin.horizontalSliderThumb.fixedWidth = Screen.width * 0.025f;
        GUI.skin.horizontalSlider.fixedHeight = Screen.height * 0.025f;
        GUI.skin.horizontalSliderThumb.fixedHeight = Screen.height * 0.025f;

        GUI.Label(new Rect(0f, Screen.height * 0.29f, Screen.width * 0.5f, Screen.height * 0.1f), CONFIG.getTexto(10), estiloinventariocentrado);
        GUI.Label(new Rect(0f, Screen.height * 0.44f, Screen.width * 0.5f, Screen.height * 0.1f), CONFIG.getTexto(11), estiloinventariocentrado);

        CONFIG.vol_sonido = GUI.HorizontalSlider(new Rect(Screen.width * 0.55f, Screen.height * 0.3f, Screen.width * 0.25f, Screen.height * 0.05f), CONFIG.vol_sonido, 0f, 1f);
        CONFIG.vol_musica = GUI.HorizontalSlider(new Rect(Screen.width * 0.55f, Screen.height * 0.45f, Screen.width * 0.25f, Screen.height * 0.05f), CONFIG.vol_musica, 0f, 1f);

        _refControl.cambiarVolumenMusica(CONFIG.vol_musica);
        _refControl.cambiarVolumenEfectos();
        _refControl.cambiarVolumenPasos();

        if (GUI.Button(new Rect(Screen.width * 0.375f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(48), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.main;
            using (BinaryWriter bw = new BinaryWriter(File.Open(fileConfig, FileMode.Create)))
            {
                bw.Write(CONFIG.vol_sonido);
                bw.Write(CONFIG.vol_musica);
                bw.Write(CONFIG.idioma);
            }
        }
    }

    private void DrawMenuSalir()
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
        GUI.DrawTexture(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        //GUI.Box(new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.65f), "");
        GUI.Label(new Rect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), (!CONFIG.MODO_EXTREMO) ? (CONFIG.getTexto(25)) : (CONFIG.getTexto(26)), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.05f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(27) + _refGame.player.getNivel(), estiloventana);
        GUI.Label(new Rect(Screen.width * 0.80f, Screen.height * 0.1f, Screen.width * 0.5f, Screen.height * 0.38f), CONFIG.getTexto(21) + _refGame.player.inventario.oro, estiloventana);


        GUI.Label(new Rect(0f, Screen.height * 0.29f, Screen.width * 1f, Screen.height * 0.1f), CONFIG.getTexto(66), estiloinventariocentrado);

        if (GUI.Button(new Rect(Screen.width * 0.15f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), "Ok", estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            CONFIG.volviendoAMenu = true;
            SceneManager.LoadScene(2);
        }
        if (GUI.Button(new Rect(Screen.width * 0.6f, Screen.height * 0.725f, Screen.width * 0.25f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            currentVentanaPausa = ventanaPausa.main;
        }
    }

    /// <summary>
    /// Muestra la informacion del item a comprar/vender.
    /// La funcion devuelve: 0 si todavia se esta mostrando la ventana, 1 si compra/vende el item y -1 si cancela la operacion.
    /// </summary>
    /// <param name="it">Item a mostrar.</param> 
    /// <param name="compra">Si es compra o venta.</param>
    /// <returns></returns>
    private int DrawVentanaObjetoShop(Item it, bool compra)
    {
        estiloventana.normal.textColor = Color.yellow;
        estiloventana.fontSize = UTIL.TextoProporcion(32);
        estiloventana.alignment = TextAnchor.UpperCenter;

        estiloBoton.fontSize = UTIL.TextoProporcion(28);
        //estiloBoton.normal.textColor = Color.white;

        GUI.DrawTexture(new Rect(Screen.width * 0.15f, Screen.height * 0.15f, Screen.width * 0.7f, Screen.height * 0.65f), _refControl.otrasTexturas[0]);
        estiloventana.normal.textColor = it.ColorTexto;

        int valorComparacion = 0;

        bool comparacion = _refGame.player.esEquipable(it);

        if (comparacion)
        {
            valorComparacion = it.ValorTotal;
            if (_refGame.player.inventario.getEquipadoSegunTipo(it.Tipo) != null)
                valorComparacion -= _refGame.player.inventario.getEquipadoSegunTipo(it.Tipo).ValorTotal;
        }

        string calidadtexto = "";

        int i = 0, iMax = 1;

        if (it.getCalidad == Item.Calidad.normal)
        {
            calidadtexto = "(Normal)";
            iMax = 1;
        }
        else if (it.getCalidad == Item.Calidad.raro)
        {
            calidadtexto = CONFIG.getTexto(49);
            iMax = 2;
        }
        else if (it.getCalidad == Item.Calidad.epico)
        {
            calidadtexto = CONFIG.getTexto(50);
            iMax = 3;
        }
        else if (it.getCalidad == Item.Calidad.legendario)
        {
            calidadtexto = CONFIG.getTexto(51);
            iMax = 4;
        }

        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.17f, Screen.width * 0.6f, Screen.height * 0.38f), it.Nombre + " " + calidadtexto, estiloventana);
        estiloventana.normal.textColor = Color.white;

        if (it.getTipoAtributo(0) != Item.ATRIBUTO.ARMADURA)    //armadura se muestra como numero, y los demas como porcentaje
        {
            GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(0)) + ": " + it.getValorAtributo(i) + "% (" + it.ValorBase + "% + " + (it.ValorTotal - it.ValorBase).ToString() + "%)", estiloventana);
        }
        else
        {
            GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(0)) + ": " + it.ValorTotal + " (" + it.ValorBase + " + " + (it.ValorTotal - it.ValorBase).ToString() + ")", estiloventana);

        }

        if (comparacion && _refGame.player.esEquipable(it))
        {
            string texto = "";
            estiloventana.alignment = TextAnchor.UpperLeft;
            //elegir color de texto segun si es + n - y restaurar color, poner en un label diferente pero a misma altura de y. y tambien poner en el de abajo
            if (valorComparacion > 0)
            {
                estiloventana.normal.textColor = Color.green;
                texto = "+" + valorComparacion.ToString();
            }
            else if (valorComparacion < 0)
            {
                estiloventana.normal.textColor = Color.red;
                texto = valorComparacion.ToString();
            }
            else
            {
                estiloventana.normal.textColor = Color.white;
                texto = valorComparacion.ToString();
            }
            GUI.Label(new Rect(Screen.width * 0.7f, Screen.height * 0.3f, Screen.width * 0.6f, Screen.height * 0.38f), "(" + texto + ")", estiloventana);
            estiloventana.alignment = TextAnchor.UpperCenter;
        }


        for (i = 1; i < iMax; i++)  //PARA LOS ATRIBUTOS SECUNDARIOS
        {
            estiloventana.normal.textColor = Color.white;

            if (it.getTipoAtributo(i) == Item.ATRIBUTO.NADA)
                continue;

            if (it.getTipoAtributo(i) != Item.ATRIBUTO.ARMADURA)    //armadura se muestra como numero, y los demas como porcentaje
            {
                GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * (0.3f + 0.06f * i), Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(i)) + ": " + it.getValorAtributo(i) + "%", estiloventana);
            }
            else
            {
                GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * (0.3f + 0.06f * i), Screen.width * 0.6f, Screen.height * 0.38f), Item.tipoAtributoToString(it.getTipoAtributo(i)) + ": " + it.getValorAtributo(i), estiloventana);

            }
        }

        if (compra && _refGame.player.inventario.oro < it.ValorOro)
        {
            estiloventana.normal.textColor = Color.red;
            GUI.enabled = false;////++++++++++++++++
        }
        else
        {
            estiloventana.normal.textColor = Color.yellow;
        }
        if (GUI.Button(new Rect(Screen.width * 0.25f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), (compra) ? (CONFIG.getTexto(18)) : (CONFIG.getTexto(19)), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            return 1;
        }

        GUI.enabled = true;///-----------------

        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.55f, Screen.width * 0.6f, Screen.height * 0.38f),
                    CONFIG.getTexto(52) + ((compra) ? (it.ValorOro.ToString()) : ((int)(it.ValorOro * 0.5f)).ToString()) + CONFIG.getTexto(53), estiloventana);

        if (it.lvl > _refGame.player.getNivel())
        {
            estiloventana.normal.textColor = Color.red;
        }
        else
        {
            estiloventana.normal.textColor = Color.white;
        }
        estiloventana.alignment = TextAnchor.UpperLeft;
        GUI.Label(new Rect(Screen.width * 0.16f, Screen.height * 0.17f, Screen.width * 0.6f, Screen.height * 0.38f), CONFIG.getTexto(54) + it.lvl, estiloventana);


        if (GUI.Button(new Rect(Screen.width * 0.55f, Screen.height * 0.65f, Screen.width * 0.2f, Screen.height * 0.1f), CONFIG.getTexto(7), estiloBoton))
        {
            _refControl.PlaySonido(0, 0.4f);
            return -1;
        }



        return 0;
    }

    public void IniciarTituloZonaFade(string titulo = "", bool reset = true)
    {
        if (!reset && (_enFadeOutTitulo || _enFadeInTitulo))
        {
            return;
        }

        if (titulo == "")
        {
            _enFadeInTitulo = _enFadeOutTitulo = false;
            _tituloAMostrar = "";
            return;
        }
        _tituloAMostrar = titulo;
        _enFadeInTitulo = true;
        _porcentajeFadeTitulo = 0f;
    }

    private bool TituloZonaFade()
    {
        //tarda 1 segundo cada transicion
        if (_enFadeInTitulo)
        {
            _porcentajeFadeTitulo += Game.elapsed / 4;
            if (_porcentajeFadeTitulo >= 1f)
            {
                _porcentajeFadeTitulo = 1f;
                _enFadeInTitulo = false;
                _enFadeOutTitulo = true;
            }
            return true;
        }
        else if (_enFadeOutTitulo)
        {
            _porcentajeFadeTitulo -= Game.elapsed / 4;
            if (_porcentajeFadeTitulo <= 0f)
            {
                _porcentajeFadeTitulo = 0f;
                _enFadeOutTitulo = false;
            }
            return true;
        }
        return false;
    }

    public void AgregarTextoConversacion(string texto, bool setEol = true)
    {
        /*if (textoConversacion != null)
            return;*/

        if (_state == estado.ventana)
            return;

        estadoNpc = estadoNPC.conversacion;
        npcActivo = null;
        _estadoShop = estadoShopNpc.Main;

        //primero ajusta el formato agregando eol
        textoConversacion = new string[(int)((texto.Length / 286) + 2)];
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
            //----------------------------------------------
        }

        //SEPARA EL TEXTO POR PAGINAS
        int indAnt = 0;
        if (indicePag.Count > 0)
        {
            for (int aaa = 0; aaa < indicePag.Count; aaa++)
            {

                textoConversacion[aaa] = texto.Substring(indAnt, indicePag[aaa]);
                indAnt += indicePag[aaa] + 1;
            }
        }
        else
        {
            textoConversacion[0] = texto;
        }
        //----------------------------------------------

        if (setEol)
        {
            //RECORRE LAS PAGINAS PARA PONER LOS EOL
            int linea = 0;
            for (int pag = 0; pag < textoConversacion.Length; pag++)
            {
                if (textoConversacion[pag] == "" || textoConversacion[pag] == null)
                {
                    continue;
                }
                linea = 0;
                for (int i = 0; i < textoConversacion[pag].Length; i++)
                {
                    if (linea == 47)
                    {
                        index = 0;
                        for (index = 0; i + index > 0; index--)
                        {
                            if (textoConversacion[pag][i + index] == ' ')    //"rebobina" para encontrar un espacio, y asi seguir en la otra linea
                            {
                                i = i + index;
                                char[] arr = textoConversacion[pag].ToCharArray();
                                arr[i] = '\n';
                                textoConversacion[pag] = new string(arr);
                                linea = -1;
                                break;
                            }
                        }
                    }
                    linea++;
                }
            }
        }
        

        _state = estado.ventana;
        _refControl.PausarSonido();
        _refControl.PausarPasos();
        _refControl.PausarEfectos();
    }

    public void comprobarHabilidadSinAprender()
    {

        int[] niveles = new int[] { 1, 5, 12, 25, 40, 50, 80 };

        for (int i = 1; i < 7; i++)
        {
            if (_refGame.player.getNivel() < niveles[i])  //si esta bloqueado ya ni hace falta seguir, las habilidades que pueden seguir van a ser lvl mas alto
            {
                break;
            }


            switch (i)
            {
                case 1:
                    //seguir el modelo del case 1, aplicar a lo demas y borrar lo que queda
                    //despues probar bien leveleando y aprendiendo habilidades a ver que onda
                    //acordarse de llamar a esta funcion al pasar de lvl y al salir de pausa?

                    if (_refGame.player.getSkill(1) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;

                case 2:
                    if (_refGame.player.getPasiva(0) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;
                case 3:
                    if (_refGame.player.getSkill(2) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;

                case 4:

                    if (_refGame.player.getSkill(3) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;


                case 5:
                    if (_refGame.player.getSkill(4) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;


                case 6:

                    if (_refGame.player.getPasiva(1) == null)
                    {
                        habilidadSinAprender = true;
                        return;
                    }
                    break;

            }
        }

        //si llego hasta aca, es que no entro en ningun case
        habilidadSinAprender = false;

    }

    //lo hago publico asi lo puedo llamar desde control en el evento on
    public void Pausar()
    {
        //GameObject.Find("AdManager").GetComponent<AdManager>().RequestInterstitial();
        _refGame.pausa = true;
        currentVentanaPausa = ventanaPausa.main;
        _refGame.Guardar();
        _refControl.PausarMusica();
        _refControl.PausarSonido();
        _refControl.PausarPasos();
        _refControl.PausarEfectos();
        _refControl.PlaySonido(0, 0.4f);
    }
}
