using UnityEngine;

public abstract class Boss : Enemigo
{
    protected bool _activado;
    public bool activado
    {
        get
        {
            return _activado;
        }
    }
    protected float distanciaAccion;
    protected float distanciaPostDraw;
    protected string _nombre;
    protected GUIStyle estiloNombre;
    protected int _codigo = -1;

    public Boss() : base()
    {
        _activado = false;
        distanciaAccion = 10;
        distanciaPostDraw = 30;
        _clase = "boss";
    }

    public Boss(string nombre, Texture2D spr, int posX, int posY, int presetAnim = -1): base(spr, posX, posY, 1, true, null, presetAnim)
    {
        _activado = false;
        distanciaAccion = 10;
        distanciaPostDraw = 30;
        _nombre = nombre;

        estiloNombre = new GUIStyle();
        estiloNombre.normal.textColor = Color.white;
        estiloNombre.alignment = TextAnchor.MiddleCenter;
        estiloNombre.fontSize = UTIL.TextoProporcion(60);
        _clase = "boss";
    }


    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (_state == estado.miss || !_activado)
        {
            return;
        }
        
        int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+pos.x - posPlayer.x) * CONFIG.TAM - microPosPlayer.x + microPosAbsoluta.x);
        int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-pos.y + posPlayer.y) * CONFIG.TAM + microPosPlayer.y - microPosAbsoluta.y);
        if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
        {
            //los mensajes de dmg se muestran aunque este muerto, PERO EL DROP POR ENSIMA DE ESTO

            if (_state == estado.muerto) //dibujar texto de drop
            {
                for (int i = 0; i < _dmgRecibidoMensaje.Count; i++)
                {
                    if ((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) > 1.5f)
                    {
                        _dmgRecibidoMensaje.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
                    estiloDmg.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) / 1.5f);
                    GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) / 1.5f) * 1.0f + i * 0.3f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString(), estiloDmg);
                }
                estilo.normal.textColor = Color.yellow;

                int c = 0;
                for (c = 0; tieneDrop && c < _drop.GetLength(0); c++)
                {
                    if (_drop[c] == null)
                        continue;
                    //cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
                    estilo.normal.textColor = new Color(_drop[c].ColorTexto.r, _drop[c].ColorTexto.g, _drop[c].ColorTexto.b, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dragListaDrop) / 2.0f);
                    GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dragListaDrop) / 2.0f) * 1.0f + c * 0.3f), CONFIG.TAM * 3, 5), "+" + _drop[c].Nombre, estilo);
                }
                estilo.normal.textColor = Color.white;
                if (_expDrop != 0)
                    GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dragListaDrop) / 2.0f) * 1.0f + c * 0.3f), CONFIG.TAM * 3, 5), "+" + _expDrop + " exp", estilo);
            }
            else
            {
                

                for (int i = 0; i < _dmgRecibidoMensaje.Count; i++)
                {
                    if ((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) > 1.5f)
                    {
                        _dmgRecibidoMensaje.RemoveAt(i);
                        i--;
                        continue;
                    }
                    //cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
                    estiloDmg.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) / 1.5f);
                    if (_dmgRecibidoMensaje[i].z == 1)
                    {
                        estiloDmg.fontSize = UTIL.TextoProporcion(30);
                        estiloDmg.normal.textColor = new Color(1f, 0.5f, 0f);
                        GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) / 1.5f) * 1.0f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString() + "!", estiloDmg);
                        estiloDmg.fontSize = UTIL.TextoProporcion(24);
                        estiloDmg.normal.textColor = Color.red;
                    }
                    else
                        GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) / 1.5f) * 1.0f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString(), estiloDmg);
                }
            }

        }

        if (_barraVidaVisible)
        {
            Rect rectaAux = new Rect(Screen.width * 0.175f, Screen.height * 0.7f, Screen.width * 0.65f, Screen.height * 0.05f);
            GUI.DrawTexture(rectaAux, _barraGris);
            GUI.BeginGroup(new Rect(rectaAux.x, rectaAux.y, rectaAux.width * ((float)getHp() / getHpMax()), rectaAux.height));
            GUI.DrawTexture(new Rect(0f, 0f, rectaAux.width, rectaAux.height), _barraRoja);
            GUI.EndGroup();
            
            GUI.Label(rectaAux, _nombre, estiloNombre);
        }
    }

    public override void EjecutarAccionAI()
    {
        //esto hay que programarlo para cada boss en su clase correspondiente
        //es innecesario hacer el override aca, pero es como un recordatorio de que hay que modificarla para cada boss
    }

    protected override void AjustarNivel()
    {
        
    }

    public void ActivarBoss(bool state)
    {
        _activado = state;
        refGame.bossActivo = this;
    }

}
