using UnityEngine;
using System.Collections.Generic;

public sealed class EnemigoPasivo : Enemigo
{
    private bool _solido;
    private int _codigo;

    public EnemigoPasivo() : base()
	{
        //constructor default
    }

    public EnemigoPasivo(int cod, Texture2D spr, int posX, int posY, int nivel, ITEMLIST.ITEM_GROUP iG = null, bool solido = true, int oro = 0) //: base(spr, posX, posY, nivel, true)
	{
        //cod = -1 significa que no es unico, osea que se va a crear cada vez que vuelva al mapa
        if (cod != -1 && refGame.cofresDestruidos[cod] == true) //el cofre ya fue destruido
        {
            _state = estado.miss;
            _estadoAI = AiState.DEAD;
            return;
        }
        _nivel = nivel;
        _modificadorDef = 1f;
        AjustarNivel();
        _codigo = cod;
        _tipo = TIPO.ENEMIGO;
        _barraVidaVisible = false;
        _pos = new Vector2((float)posX, (float)posY);
        _sprite = spr;
        _microPos = new Vector2();
        _drawDrop = true;
        _morirTransparencia = 0.0f;
        _dragListaDrop = 0;
        _dmgRecibidoMensaje = new List<Vector3>();
        _itemGroup = iG;
        _expDrop = 0;
        this._oro = oro;
        _afectadoPorHorario = false;

        estiloDmg = new GUIStyle();
        estiloDmg.normal.textColor = Color.red;
        estiloDmg.fontSize = UTIL.TextoProporcion(24);
        estiloDmg.alignment = TextAnchor.UpperCenter;
        estilo = new GUIStyle();
        estilo.normal.textColor = Color.yellow;
        estilo.fontSize = UTIL.TextoProporcion(28);
        estilo.alignment = TextAnchor.UpperCenter;


        if (solido)
        {
            refGame.currentMapa.mundoObstaculos[(int)(_pos.x + _pos.y * refGame.currentMapa.DIMX)] = true;
        }
        _solido = solido;
        _hpMax = _hp = 50;
    }

    public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (_state == estado.miss)
        {
            return;
        }
        GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f - 0.25f * _morirTransparencia);
        GUI.DrawTexture(new Rect(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM - microPosPlayer.x + microPosAbsoluta.x, Screen.height / 2 - CONFIG.TAM / 2 + (-_pos.y + posPlayer.y) * CONFIG.TAM + microPosPlayer.y - microPosAbsoluta.y, CONFIG.TAM, CONFIG.TAM), _sprite);
        GUI.color = Color.white;
    }
    //postdraw no hace falta hacerle override

    protected override void Morir()
    {
        base.Morir();
        if (_solido)
        {
            refGame.currentMapa.mundoObstaculos[(int)(_pos.x + _pos.y * refGame.currentMapa.DIMX)] = false;
        }

        if (_codigo != -1)
        {
            refGame.cofresDestruidos[_codigo] = true;
        }
    }

    protected override void AjustarCoordenadasSheet(int preset)
    {
       
    }

    public override void Actualizar()
    {
        if (_state == estado.muerto)
        {
            if (Game.TiempoTranscurrido - _tiempoUltimaAnim > CONFIG.TiempoAnimCaminar)
            {
                if (_solido)    //esto es un parche para arreglar el problema de que se vuelven transpasables al cambiar de mapa y volver
                {
                    refGame.currentMapa.mundoObstaculos[(int)(_pos.x + _pos.y * refGame.currentMapa.DIMX)] = true;
                }

                _tiempoUltimaAnim = Game.TiempoTranscurrido;

                _fase++;
                if (_fase >= _faseMorirMax)
                {
                    _fase = _faseMorirMax;
                    _morirTransparencia++;
                    if (_morirTransparencia >= 4)
                    {
                        _morirTransparencia = 4;
                    }

                    if (_morirTransparencia >= 4 && (Game.TiempoTranscurrido - _dragListaDrop) >= 2.0f)
                    {
                        _morirTransparencia = 0;
                        _dragListaDrop = 0;
                        _state = estado.miss;
                    }
                }
            }
        }
    }

    public override void EjecutarAccionAI()
    {
        
    }
}