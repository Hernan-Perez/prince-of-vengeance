using UnityEngine;

public sealed class BossBarnak : Boss
{
    private float contadorTiempo;
    private bool berserk = false;
    private Texture2D _buff;
    public BossBarnak(Texture2D spr, int posX, int posY, Texture2D buff, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(67), spr, posX, posY, presetAnim)
    {
        _codigo = 0;
        //activado = true;
        _hpMax = _hp = 350;
        _expDrop = 740;
        _dmgMax = 11;
        _dmgMin = 4;
        _oro = 78;
        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 1.2f;
        contadorTiempo = 0f;
        _maxDistTarget = 22f;
        _maxDistAtaque = 2f;
        _modificadorVelocidad = 0.75f;
        cantItemsDrop = 3;

        _itemGroup = ITEMLIST.Instance.getPreset(0, new Vector4(0f, 0f, 0f, 100f));
        _buff = buff;
        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        //6750 xp , si jugador es l5 necesita 2000 para pasar de lvl
        //EXP Y ORO ES x15 ENEMIGOS DEL MISMO LVL
        setStatsCustom(5, 4500, 20, 25, 838, 8566);

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(8, new Vector4(0f, 0f, 0f, 100f));
            setStatsCustom(85, 650000, 1000, 1500, 58790, 385232);
        }
    }

    /*public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        base.Draw(posPlayer, microPosPlayer);
    }*/

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (!_activado && estadoAI != AiState.DEAD)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
            if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.skills[15]);
            }
        }

        if (!_activado)
            return;
        refControl.PlayMusica(10);
        base.PostDraw(posPlayer, microPosPlayer);
        if (berserk && estadoAI != AiState.DEAD)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
            if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), _buff);
            }
        }
    }

    public override void EjecutarAccionAI()
    {
        if (_state == estado.miss)
        {
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            return;
        }
        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        distPlayer = new Vector2(pos.x + microPos.x/100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
        //distBase = new Vector2(pos.x - _posBase.x, pos.y - _posBase.y);

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
                refGame.player.RecibirDmg((int)Random.Range(_dmgMin * _modificadorDmg, _dmgMax * _modificadorDmg), false); //LOS ENEMIGOS ?PEGAN CRITICO??
                CambiarEstado(estado.atacando);
            }
        }
    }

    public override void Actualizar()
    {
        if (!_activado)
            return;

        base.Actualizar();
        contadorTiempo += Game.elapsed;
        if (contadorTiempo > 10f)
        {
            contadorTiempo -= 10f;
        }

        if (contadorTiempo < 5f)
        {
            _modificadorDmg = 0.75f;
            berserk = false;
        }
        else
        {
            _modificadorDmg = 2.5f;
            berserk = true;
        }
           
    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 0;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;
        return base.RecibirDmg(dmg, critico);
    }
}
