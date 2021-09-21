using UnityEngine;
using System.Collections.Generic;

class BossRaghtul : Boss
{
    //Ragh'tul
    private Enemigo clon;
    private bool clonVisible = false;
    private float ultimoTiempoIntercambio = 0f;
    private float ultimoTiempoVisible = 0f;
    private float factorInvisibilidad = 1f;
    private List<Vector2> posicionesTP;
    private Texture2D _buff;

    public BossRaghtul(Texture2D spr, int posX1, int posY1, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(72), spr, posX1, posY1, presetAnim)
    {
        //init 10, 6
        _codigo = 5;
        setStatsCustom(15, 250, 22, 30, 300, 540);
        _estadoAI = AiState.IDLE;
        _maxDistTarget = 9999f;
        _maxDistAtaque = 1.5f;
        _activado = true;
        ActivarBoss(true);
        refControl.PlayMusica(10);
        refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(81));

        posicionesTP = new List<Vector2>();
        posicionesTP.Add(new Vector2(5, 4));
        posicionesTP.Add(new Vector2(5, 7));
        posicionesTP.Add(new Vector2(15, 4));
        posicionesTP.Add(new Vector2(15, 7));

        clon = new Enemigo(spr, posX1, posY1, 1, false, ITEMLIST.Instance.getPreset(0), 2);
        clon.setBarraVidaVisible(false);
        clon.maxDistAtaque = 2f;
        clon.maxDistTarget = 9999f;

        clon.setStatsCustom(15, 150, 450, 820, 0, 0);
        clon.intervaloGolpe = 2f;

        intervaloGolpe = 1f;
        _modificadorVelocidad = 0.65f;

        setStatsCustom(40, 75000, 450, 820, 18973, 157444);
        cantItemsDrop = 2;
        _itemGroup = ITEMLIST.Instance.getPreset(4, new Vector4(0f, 0f, 0f, 100f));


        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
            clon.Estado = estado.miss;
            refGame.peticionRedimencionarArrayEnemigos = true;
        }

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 100f, 0f));
            setStatsCustom(120, 2250000, 2250, 8000, 98590, 602502);
            clon.setStatsCustom(120, 2500000, 2250, 8000, 0, 0);
        }

        ultimoTiempoIntercambio = Game.TiempoTranscurrido;
    }

    public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        _colorLayer = new Color(1f, 1f, 1f, factorInvisibilidad);
        base.Draw(posPlayer, microPosPlayer);
        GUI.color = new Color(1f, 1f, 1f, 1f);

        if (clonVisible)
        {
            clon.Draw(posPlayer, microPosPlayer);
        }
    }

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (!_activado)
            return;

        if (Estado != estado.miss && estadoAI != AiState.DEAD)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), refControl.otrasTexturas[32]);
            GUI.color = new Color(1f, 1f, 1f, 1f);
        }
        

        base.PostDraw(posPlayer, microPosPlayer);
    }

    public override void EjecutarAccionAI()
    {
        if (!_activado)
            return;

        if (_state == estado.miss)
        {
            _activado = false;
            _state = estado.miss;
            clon.Estado = estado.miss;
            clonVisible = false;
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            
        }

        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        if (clonVisible)
        {
            clon.EjecutarAccionAI();
        }

        if (factorInvisibilidad != 1f)
        {
            float f = Game.TiempoTranscurrido - ultimoTiempoVisible;   //ultimo tiempo visible coincide con el tiempo de invisibilidad
            f = f / 2f;
            if (f > 1f)
            {
                f = 1f;
            }
            factorInvisibilidad = f;
        }

        if (Game.TiempoTranscurrido - ultimoTiempoIntercambio > 10f)
        {
            ultimoTiempoIntercambio = Game.TiempoTranscurrido;
            teleport();
        }

        if (clonVisible && Game.TiempoTranscurrido - ultimoTiempoVisible > 5f)
        {
            clonVisible = false;
        }

        distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);

        if (_estadoAI != AiState.TARGET_FOCUS && _estadoAI != AiState.TARGET_ATK && distPlayer.magnitude <= _maxDistTarget/* && distBase.magnitude <= 12.0f*/)//comprueba si el jugador esta cerca del enemigo
        {
            CambiarEstado(estado.caminando);
            _estadoAI = AiState.TARGET_FOCUS;
        }

        if (_estadoAI == AiState.TARGET_ATK && distPlayer.magnitude > _maxDistAtaque)   // si se alejo del rango de ataque lo vuelve a perseguir
        {
            CambiarEstado(estado.caminando);
            _estadoAI = AiState.TARGET_FOCUS;
        }

        if (_estadoAI == AiState.TARGET_FOCUS)
        {
            if (distPlayer.magnitude > _maxDistTarget/* || distBase.magnitude > 10f*/)
            {
                CambiarEstado(estado.idle);
                _estadoAI = AiState.IDLE;
            }
            else
            {
                if (distPlayer.magnitude <= _maxDistAtaque) //comprueba si esta en rango de ataque
                {
                    CambiarEstado(estado.idle);
                    _estadoAI = AiState.TARGET_ATK;
                }
                else    //si todavia no esta en rango de ataque se acerca
                {
                    MoverAPos(refGame.player.pos, velocidad);
                }
            }
        }

        if (_estadoAI == AiState.TARGET_ATK)
        {
            _direccionVect = new Vector2(refGame.player.pos.x - pos.x, refGame.player.pos.y - pos.y);
            _direccionVect.Normalize();
            //atacar
            if (Game.TiempoTranscurrido - _ultimoGolpe >= _intervaloGolpe)
            {
                _ultimoGolpe = Game.TiempoTranscurrido;
                refGame.player.RecibirDmg((int)Random.Range(_dmgMin, _dmgMax + 1), false); //LOS ENEMIGOS ?PEGAN CRITICO??
                CambiarEstado(estado.atacando);
            }
        }
    }

    public override void Actualizar()
    {
        if (!_activado)
            return;

        base.Actualizar();
        if (clonVisible)
        {
            clon.Actualizar();
        }
    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 5;
    }

    public override int getHp()
    {
        return _hp;
    }

    public override int getHpMax()
    {
        return _hpMax;
    }

    private void teleport()
    {
        if (Estado == estado.muerto || Estado == estado.miss)
            return;
        int n = Random.Range(0, posicionesTP.Count);

        clonVisible = true;
        ultimoTiempoVisible = Game.TiempoTranscurrido;
        clon.pos = pos;
        clon.microPos = microPos;

        pos = new Vector2(posicionesTP[n].x, posicionesTP[n].y);
        microPos = new Vector2();
        factorInvisibilidad = 0f;
    }
}