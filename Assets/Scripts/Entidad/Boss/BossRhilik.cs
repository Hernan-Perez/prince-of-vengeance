using UnityEngine;

public sealed class BossRhilik : Boss
{
    private bool trigger75 = false;
    private bool trigger40 = false;
    private bool trigger10 = false;
    private float timer75 = 0f, timer40 = 0f;
    private Texture2D _buff;
    private Enemigo add1, add2;
    private bool matarEnemigos = true;
    private bool derr1 = false;

    public BossRhilik(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(71), spr, posX, posY, presetAnim)
    {
        _codigo = 4;
        //activado = true;
        _hpMax = _hp = 3500;
        _expDrop = 740;
        _dmgMax = 11;
        _dmgMin = 4;
        _oro = 78;
        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 2f;
        _maxDistTarget = 22f;
        _maxDistAtaque = 2f;
        _modificadorVelocidad = 0.75f;

        setStatsCustom(35, 70000, 450, 550, 15529, 130598);
        cantItemsDrop = 2;
        _itemGroup = ITEMLIST.Instance.getPreset(3, new Vector4(0f, 0f, 0f, 100f));

        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 100f, 0f));
            setStatsCustom(115, 1800000, 8000, 15000, 92492, 538933);
        }
    }

    /*public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        base.Draw(posPlayer, microPosPlayer);
    }*/

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        if (!_activado)
            return;
        base.PostDraw(posPlayer, microPosPlayer);
    }

    public override void EjecutarAccionAI()
    {
        if ((_state == estado.miss || _state == estado.muerto) && matarEnemigos)
        {
            matarEnemigos = false;
            refGame.enemigoArray = new Enemigo[1];
            refGame.enemigoArray[0] = this;
            refGame.peticionRedimencionarArrayEnemigos = true;
        }


        if (_state == estado.miss && !derr1)
        {
            derr1 = true;
            refGame.almaValor = true;
            refGame.hud.npcActivo = null;
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(76));
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            return;
        }
        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
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
        {
            distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
            if (distPlayer.magnitude < 5f)
            {
                _activado = true;
                ActivarBoss(true);
                refControl.PlayMusica(10);
                refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(80));
            }

            _estadoAI = AiState.TARGET_ATK;
            //CambiarEstado(estado.atacando);
        }


        if (!_activado)
            return;

        base.Actualizar();

        if (add1 != null && add1.Estado == estado.miss)
        {
            add1 = null;
        }
        if (add2 != null && add2.Estado == estado.miss)
        {
            add2 = null;
        }

        if (timer40 != 0f)
        {
            if (add1 == null)
                timer40 = 0f;
            timer40 -= Game.elapsed;
            if (timer40 <= 0f)
            {
                timer40 = 0f;
                if (!(add1 == null || add1.Estado == estado.miss || add1.Estado == estado.muerto))
                {
                    add1.Estado = estado.miss;
                    add1 = null;
                    _hp += (int)(_hpMax * 0.4f);
                }
                if (_hp > _hpMax)
                {
                    _hp = _hpMax;
                }
            }
        }


        if (timer75 != 0f)
        {
            if (add1 == null && add2 == null)
                timer75 = 0f;

            timer75 -= Game.elapsed;
            if (timer75 <= 0f)
            {
                timer75 = 0f;
                if (!(add1 == null || add1.Estado == estado.miss || add1.Estado == estado.muerto))
                {
                    add1.Estado = estado.miss;
                    add1 = null;
                    _hp += (int)(_hpMax * 0.1f);
                }
                if (!(add2 == null || add2.Estado == estado.miss || add2.Estado == estado.muerto))
                {
                    add2.Estado = estado.miss;
                    add2 = null;
                    _hp += (int)(_hpMax * 0.1f);
                }
                if (_hp > _hpMax)
                {
                    _hp = _hpMax;
                }
            }
        }

        if (getPorcentajeHP() < 0.10f && !trigger10)
        {
            trigger10 = true;
            trigger40 = true;
            trigger75 = true;

            refGame.enemigoArray = new Enemigo[7];
            if (!CONFIG.MODO_EXTREMO)
            {
                refGame.enemigoArray[0] = this;
                refGame.enemigoArray[1] = new Enemigo(refControl.sprites[35], 11, 34, 32, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[1].maxDistTarget = 5000f; refGame.enemigoArray[1].tieneDrop = false;
                refGame.enemigoArray[2] = new Enemigo(refControl.sprites[36], 11, 32, 32, false, ITEMLIST.Instance.getPreset(0), 1); refGame.enemigoArray[2].maxDistTarget = 5000f; refGame.enemigoArray[2].tieneDrop = false;
                refGame.enemigoArray[3] = new Enemigo(refControl.sprites[35], 11, 30, 32, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[3].maxDistTarget = 5000f; refGame.enemigoArray[3].tieneDrop = false;
                refGame.enemigoArray[4] = new Enemigo(refControl.sprites[35], 17, 34, 32, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[4].maxDistTarget = 5000f; refGame.enemigoArray[4].tieneDrop = false;
                refGame.enemigoArray[5] = new Enemigo(refControl.sprites[36], 17, 32, 32, false, ITEMLIST.Instance.getPreset(0), 1); refGame.enemigoArray[5].maxDistTarget = 5000f; refGame.enemigoArray[5].tieneDrop = false;
                refGame.enemigoArray[6] = new Enemigo(refControl.sprites[35], 17, 30, 32, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[6].maxDistTarget = 5000f; refGame.enemigoArray[6].tieneDrop = false;
                refGame.peticionRedimencionarArrayEnemigos = true;
            }
            else
            {
                refGame.enemigoArray[0] = this;
                refGame.enemigoArray[1] = new Enemigo(refControl.sprites[35], 11, 34, 150, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[1].maxDistTarget = 5000f; refGame.enemigoArray[1].tieneDrop = false;
                refGame.enemigoArray[2] = new Enemigo(refControl.sprites[36], 11, 32, 150, false, ITEMLIST.Instance.getPreset(0), 1); refGame.enemigoArray[2].maxDistTarget = 5000f; refGame.enemigoArray[2].tieneDrop = false;
                refGame.enemigoArray[3] = new Enemigo(refControl.sprites[35], 11, 30, 150, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[3].maxDistTarget = 5000f; refGame.enemigoArray[3].tieneDrop = false;
                refGame.enemigoArray[4] = new Enemigo(refControl.sprites[35], 17, 34, 150, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[4].maxDistTarget = 5000f; refGame.enemigoArray[4].tieneDrop = false;
                refGame.enemigoArray[5] = new Enemigo(refControl.sprites[36], 17, 32, 150, false, ITEMLIST.Instance.getPreset(0), 1); refGame.enemigoArray[5].maxDistTarget = 5000f; refGame.enemigoArray[5].tieneDrop = false;
                refGame.enemigoArray[6] = new Enemigo(refControl.sprites[35], 17, 30, 150, true, ITEMLIST.Instance.getPreset(0), 2); refGame.enemigoArray[6].maxDistTarget = 5000f; refGame.enemigoArray[6].tieneDrop = false;
                refGame.peticionRedimencionarArrayEnemigos = true;
            }
        }
        else if (getPorcentajeHP() < 0.40f && !trigger40)
        {
            trigger40 = true;
            trigger75 = true;

            timer40 = 8f;
            refGame.enemigoArray = new Enemigo[2];
            refGame.enemigoArray[0] = this;
            add1 = refGame.enemigoArray[1] = new Enemigo(refControl.sprites[37], 14, 30, 40, true, ITEMLIST.Instance.getPreset(0), 2);
            add1.tieneDrop = false;
            add1.maxDistTarget = 5000f;
            add1.setStatsCustom(32, 10000, 200, 300, 0, 0);
            if (CONFIG.MODO_EXTREMO)
            {
                add1.setStatsCustom(150, 300000, 1000, 9000, 0, 0);
            }
            refGame.peticionRedimencionarArrayEnemigos = true;
        }
        else if (getPorcentajeHP() < 0.75f && !trigger75)
        {
            trigger75 = true;
            timer75 = 8f;
            refGame.enemigoArray = new Enemigo[3];
            refGame.enemigoArray[0] = this;
            
            add1 = refGame.enemigoArray[1] = new Enemigo(refControl.sprites[35], 12, 32, 32, true, ITEMLIST.Instance.getPreset(0), 2);
            add2 = refGame.enemigoArray[2] = new Enemigo(refControl.sprites[35], 16, 32, 32, true, ITEMLIST.Instance.getPreset(0), 2);
            add1.maxDistTarget = 5000f;
            add1.setStatsCustom(32, 7000, 200, 300, 0, 0);
            add1.tieneDrop = false;
            add2.maxDistTarget = 5000f;
            add2.tieneDrop = false;
            add2.setStatsCustom(32, 7000, 200, 300, 0, 0);
            if (CONFIG.MODO_EXTREMO)
            {
                add1.setStatsCustom(115, 120000, 1000, 9000, 0, 0);
                add2.setStatsCustom(115, 120000, 1000, 9000, 0, 0);
            }
            refGame.peticionRedimencionarArrayEnemigos = true;
        }

    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 4;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;

        if (add1 != null || add2 != null)
        {
            dmg = (int)(dmg * 0.25f);
        }

        return base.RecibirDmg(dmg, critico);
    }

    private float getPorcentajeHP()
    {
        float h = _hp;
        float hm = _hpMax;
        return (h / hm);
    }
}
