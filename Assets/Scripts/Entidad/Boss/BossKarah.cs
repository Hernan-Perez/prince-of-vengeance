using UnityEngine;

class BossKarah : Boss
{
    private Texture2D _buff, _buff2;
    private Vector2 puntoTeleport;  //punto fijo al que teletransporta al jugador
    private float cd = 0f;  //variable para intervalos en los que no tira ningun humo
    private bool enCD = true;
    private int fase = 0;   //0 = humo azul 1 = humo verde 2 = humo rojo
    private float flick = 0f;
    private float cdDmg = 0;
    private Vector2 monitorPosPJ;
    private Color c1, c2;
    private bool derr1 = false;
    private float multiplicador = 1f;

    public BossKarah(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(70), spr, posX, posY, presetAnim)
    {
        _codigo = 2;
        //activado = true;
        _hpMax = _hp = 350;
        _expDrop = 740;
        _dmgMax = 11;
        _dmgMin = 4;
        _oro = 78;
        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 4f;
        _maxDistTarget = 30f;
        _maxDistAtaque = 30f;
        _modificadorVelocidad = 0f;
        _buff = refControl.otrasTexturas[40];
        _buff2 = refControl.otrasTexturas[41];
        puntoTeleport = new Vector2(52, 68);


        setStatsCustom(25, 37000, 200, 400, 9375, 81537);
        cantItemsDrop = 2;
        _itemGroup = ITEMLIST.Instance.getPreset(2, new Vector4(0f, 0f, 0f, 100f));

        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }
        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 100f, 0f));
            setStatsCustom(105, 800000, 6750, 10000, 80694, 502502);
            multiplicador = 10f;
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
        
        if (!enCD && estadoAI != AiState.DEAD)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - 8 - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y + 8) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);

            GUI.color = c1;
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM * 16, CONFIG.TAM * 16), _buff);
            GUI.color = c2;
            GUI.DrawTexture(new Rect(x, y, CONFIG.TAM * 16, CONFIG.TAM * 16), _buff2);
            GUI.color = Color.white;
           
        }

        base.PostDraw(posPlayer, microPosPlayer);
    }

    public override void EjecutarAccionAI()
    {

        if (_state == estado.miss && !derr1)
        {
            derr1 = true;
            refGame.almaSabiduria = true;
            refGame.hud.npcActivo = null;
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(75));
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            return;
        }

        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        if (_estadoAI == AiState.IDLE)
        {
            CambiarEstado(estado.idle);
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
        if (_activado)
            base.Actualizar();

        if (estadoAI == AiState.DEAD)
            return;

        if (!_activado)
        {
            distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
            if (distPlayer.magnitude < 5f)
            {
                ActivarBoss(true);
                refControl.PlayMusica(10);
                refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(78));
            }
                
            _estadoAI = AiState.TARGET_ATK;
            CambiarEstado(estado.atacando);
            cd = Game.TiempoTranscurrido;
        }

        
        if (enCD == true)
        {
            if (Game.TiempoTranscurrido - cd > 10f)
            {
                cd = Game.TiempoTranscurrido;
                flick = Game.TiempoTranscurrido;
                enCD = false;
                _estadoAI = AiState.IDLE;
                monitorPosPJ = new Vector2(refGame.player.pos.x, refGame.player.pos.y); //por las dudas para que no pase referencia
                refGame.player.pos = new Vector2(puntoTeleport.x, puntoTeleport.y);
                switch (fase)
                {
                    case 0:
                        c1 = new Color(0f, 0f, 1f, 0.5f);
                        c2 = new Color(0f, 0f, 1f, 0f);
                        break;
                    case 1:
                        c1 = new Color(0.5f, 1f, 0.5f, 0.5f);
                        c2 = new Color(0.5f, 1f, 0.5f, 0f);
                        break;
                    case 2:
                        c1 = new Color(1f, 0f, 0f, 0.5f);
                        c2 = new Color(1f, 0f, 0f, 0f);
                        break;
                }
            }
            
        }
        else
        {
            float aaa = Game.TiempoTranscurrido - flick;
            if (aaa > 4f)
            {
                flick = Game.TiempoTranscurrido;
                aaa = 0f;
            }
            float bbb = aaa - 2f;
            if (bbb < 0)
                bbb *= -1;

            //actualizar animaciones y efectos
            switch (fase)
            {
                case 0: //azul
                    c1 = new Color(0f, 0f, 1f, 1);
                    c2 = new Color(0f, 0f, 1f, 1);
                    if (monitorPosPJ.x != refGame.player.pos.x || monitorPosPJ.y != refGame.player.pos.y)
                    {
                        monitorPosPJ = new Vector2(refGame.player.pos.x, refGame.player.pos.y);
                        if (Game.TiempoTranscurrido - cdDmg > 1f)
                        {
                            cdDmg = Game.TiempoTranscurrido;
                            refGame.player.RecibirDmg((int)(1500 * multiplicador), true);
                        }
                        
                    }

                        break;
                case 1: //verde
                    c1 = new Color(0.5f, 1f, 0.5f, 1);
                    c2 = new Color(0.5f, 1f, 0.5f, 1);
                    break;
                case 2: //rojo
                    c1 = new Color(1f, 0f, 0f, 1);
                    c2 = new Color(1f, 0f, 0f, 1);


                    if (Game.TiempoTranscurrido - cdDmg > 1f)
                    {
                        if (monitorPosPJ.x == refGame.player.pos.x && monitorPosPJ.y == refGame.player.pos.y
                            && refGame.player.microPos.x == 0 && refGame.player.microPos.y == 0)
                        {
                            cdDmg = Game.TiempoTranscurrido;
                            refGame.player.RecibirDmg((int)(1000 * multiplicador), true);
                        }
                        else
                        {
                            monitorPosPJ = new Vector2(refGame.player.pos.x, refGame.player.pos.y);
                        }
                    }


                    break;
            }

            //c1.a = 1f - (0.5f * bbb / 2f);
            c1.a = bbb / 2f;    //secuencia 1 0 1
            c2.a = 1 - c1.a;    //secuencia 0 1 0
            if (Game.TiempoTranscurrido - cd > 10f)
            {
                cd = Game.TiempoTranscurrido;
                enCD = true;
                _estadoAI = AiState.TARGET_ATK;
                CambiarEstado(estado.atacando);
                fase++;
                cdDmg = Game.TiempoTranscurrido;
                
                if (fase == 3)
                {
                    fase = 0;
                }
            }
        }
    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 2;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;
        if (fase == 1 && !enCD)  //fase verde
        {
            _hp += dmg;
            if (_hp > _hpMax)
            {
                _hp = _hpMax;
            }
            return base.RecibirDmg(0, critico);
        }
        else
        {
            return base.RecibirDmg(dmg, critico);
        }
        
    }
  }
