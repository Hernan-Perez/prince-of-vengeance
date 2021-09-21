using UnityEngine;

public sealed class BossModrean1 : Boss
{
    private float contadorTiempoEscudo;
    private float cdWave = 5f;
    private bool ultimoEscudoFueLuz = true;
    private bool escudoLuz = false, escudoSom = false;
    private float valorTotalEscudo = 0f;
    private float cdCuracion = 0f;
    private Enemigo[] adds;

    private Vector2[] misil1x, misil2x, misil1y, misil2y;   //posiciones BASE
    private bool gMisil1x = false, gMisil2x = false, gMisil1y = false, gMisil2y = false;    //si esta vivo el grupo
    private float count1x, count2x, count1y, count2y;
    private bool[] cMisil1x, cMisil2x, cMisil1y, cMisil2y;  //estan vivos
    private float offsetM1x, offsetM2x, offsetM1y, offsetM2y;
    private int dmg;
    public BossModrean1(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(73), spr, posX, posY, presetAnim)
    {
        _codigo = 7;

        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 3f;
        contadorTiempoEscudo = 15f;
        _maxDistTarget = 22f;
        _maxDistAtaque = 2f;
        _modificadorVelocidad = 0.75f;
        cantItemsDrop = 2;


        _itemGroup = ITEMLIST.Instance.getPreset(7, new Vector4(0f, 0f, 0f, 100f));
        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        setStatsCustom(70, 400000, 800, 1200, 0, 0);

        misil1x = new Vector2[3];
        misil1x[0] = new Vector2(0, 40);
        misil1x[1] = new Vector2(0, 46);
        misil1x[2] = new Vector2(0, 52);
        cMisil1x = new bool[3];
        for (int i = 0; i < cMisil1x.Length; i++)
        {
            cMisil1x[i] = true;
        }
        misil2x = new Vector2[3];
        misil2x[0] = new Vector2(40, 42);
        misil2x[1] = new Vector2(40, 48);
        misil2x[2] = new Vector2(40, 54);
        cMisil2x = new bool[3];
        for (int i = 0; i < cMisil2x.Length; i++)
        {
            cMisil2x[i] = true;
        }

        misil1y = new Vector2[5];
        misil1y[0] = new Vector2(0, 0);
        misil1y[1] = new Vector2(6, 0);
        misil1y[2] = new Vector2(12, 0);
        misil1y[3] = new Vector2(18, 0);
        misil1y[4] = new Vector2(24, 0);
        cMisil1y = new bool[5];
        for (int i = 0; i < cMisil1y.Length; i++)
        {
            cMisil1y[i] = true;
        }
        misil2y = new Vector2[4];
        misil2y[0] = new Vector2(2, 40);
        misil2y[1] = new Vector2(10, 40);
        misil2y[2] = new Vector2(16, 40);
        misil2y[3] = new Vector2(20, 40);
        cMisil2y = new bool[4];
        for (int i = 0; i < cMisil2y.Length; i++)
        {
            cMisil2y[i] = true;
        }

        dmg = 10000;

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 0f, 100f));
            setStatsCustom(150, 6000000, 2000, 10000, 0, 0);
            dmg = 30000;
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

        if (Estado != estado.miss && Estado != estado.muerto)
        {
            for (int i = 0; gMisil1x && i < misil1x.Length; i++)
            {
                if (!cMisil1x[i])
                    continue;
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+offsetM1x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-misil1x[i].y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[52]);
            }
            for (int i = 0; gMisil2x && i < misil2x.Length; i++)
            {
                if (!cMisil2x[i])
                    continue;
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (23 + offsetM2x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-misil2x[i].y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[51]);
            }
            for (int i = 0; gMisil1y && i < misil1y.Length; i++)
            {
                if (!cMisil1y[i])
                    continue;
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+misil1y[i].x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-offsetM1y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[53]);
            }
            for (int i = 0; gMisil2y && i < misil2y.Length; i++)
            {
                if (!cMisil2y[i])
                    continue;
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+misil2y[i].x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-59 - offsetM2y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[54]);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            if (escudoLuz && estadoAI != AiState.DEAD)
            {
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
                if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
                {
                    GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[58]);
                }
                GUI.color = new Color(1f, 1f, 1f, valorTotalEscudo);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), refControl.otrasTexturas[25]);
            }

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            if (escudoSom && estadoAI != AiState.DEAD)
            {
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
                if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
                {
                    GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[57]);
                }
                GUI.color = new Color(1f, 1f, 1f, valorTotalEscudo);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), refControl.otrasTexturas[32]);
            }
            GUI.color = Color.white;
        }

        base.PostDraw(posPlayer, microPosPlayer);
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
                ActivarBoss(true);
                _activado = true;
                //refControl.PlayMusica(11);
                refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(83));
            }

            _estadoAI = AiState.TARGET_ATK;
            //CambiarEstado(estado.atacando);
        }


        if (!_activado)
            return;

        base.Actualizar();

        if ((float)_hp / (float)_hpMax <= 0.2f)
        {
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(84));
            refGame._pNuevaPos = new Vector2(12, 50);
            refGame.player.microPos = new Vector2();
            refGame.IniciarTransicionMapa("mapas/Z8-12");
        }


        if (escudoLuz && _hp > 0)
        {
            cdCuracion -= Game.elapsed;
            if (cdCuracion <= 0f)
            {
                cdCuracion = 5f;
                _hp += (int)(_hpMax * 0.01f);
                if (_hp > _hpMax)
                {
                    _hp = _hpMax;
                }
            }
        }


        if (escudoLuz || escudoSom)
        {
            valorTotalEscudo = 0f;
            for (int i = 0; i < adds.Length; i++)
            {
                if (adds[i] == null || adds[i].Estado == estado.miss)
                {
                    adds[i] = null;
                    refGame.peticionRedimencionarArrayEnemigos = true;
                }
                else
                {
                    valorTotalEscudo += 0.15f;
                }
            }

            if (valorTotalEscudo == 0f)
            {
                escudoLuz = escudoSom = false;
            }

        }


        if (!escudoLuz && !escudoSom)
        {
            contadorTiempoEscudo -= Game.elapsed;
            if (contadorTiempoEscudo < 0f)
            {
                contadorTiempoEscudo = 25f;
                //ACTIVAR ESCUDO

                if (!CONFIG.MODO_EXTREMO)
                {
                    if (ultimoEscudoFueLuz)
                    {
                        ultimoEscudoFueLuz = false;
                        escudoSom = true;
                        refGame.enemigoArray = new Enemigo[5];
                        refGame.enemigoArray[0] = this;
                        refGame.enemigoArray[1] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 8, 50, 32, null, false);
                        refGame.enemigoArray[1].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[1].tieneDrop = false;
                        refGame.enemigoArray[2] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 8, 47, 32, null, false);
                        refGame.enemigoArray[2].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[2].tieneDrop = false;
                        refGame.enemigoArray[3] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 16, 50, 32, null, false);
                        refGame.enemigoArray[3].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[3].tieneDrop = false;
                        refGame.enemigoArray[4] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 16, 47, 32, null, false);
                        refGame.enemigoArray[4].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[4].tieneDrop = false;
                        refGame.peticionRedimencionarArrayEnemigos = true;
                    }
                    else
                    {
                        ultimoEscudoFueLuz = true;
                        escudoLuz = true;
                        refGame.enemigoArray = new Enemigo[5];
                        refGame.enemigoArray[0] = this;
                        refGame.enemigoArray[1] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 8, 50, 32, null, false);
                        refGame.enemigoArray[1].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[1].tieneDrop = false;
                        refGame.enemigoArray[2] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 8, 47, 32, null, false);
                        refGame.enemigoArray[2].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[2].tieneDrop = false;
                        refGame.enemigoArray[3] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 16, 50, 32, null, false);
                        refGame.enemigoArray[3].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[3].tieneDrop = false;
                        refGame.enemigoArray[4] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 16, 47, 32, null, false);
                        refGame.enemigoArray[4].setStatsCustom(1, 10000, 1, 1, 1, 1);
                        refGame.enemigoArray[4].tieneDrop = false;
                        refGame.peticionRedimencionarArrayEnemigos = true;
                    }
                }
                else
                {
                    if (ultimoEscudoFueLuz)
                    {
                        ultimoEscudoFueLuz = false;
                        escudoSom = true;
                        refGame.enemigoArray = new Enemigo[5];
                        refGame.enemigoArray[0] = this;
                        refGame.enemigoArray[1] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 8, 50, 32, null, false);
                        refGame.enemigoArray[1].setStatsCustom(1, 50000, 1, 1, 1, 1);
                        refGame.enemigoArray[1].tieneDrop = false;
                        refGame.enemigoArray[2] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 8, 47, 32, null, false);
                        refGame.enemigoArray[2].setStatsCustom(1, 50000, 1, 1, 1, 1);
                        refGame.enemigoArray[2].tieneDrop = false;
                        refGame.enemigoArray[3] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 16, 50, 32, null, false);
                        refGame.enemigoArray[3].setStatsCustom(1, 50000, 1, 1, 1, 1);
                        refGame.enemigoArray[3].tieneDrop = false;
                        refGame.enemigoArray[4] = new EnemigoPasivo(-1, refControl.otrasTexturas[57], 16, 47, 32, null, false);
                        refGame.enemigoArray[4].setStatsCustom(1, 50000, 1, 1, 1, 1);
                        refGame.enemigoArray[4].tieneDrop = false;
                        refGame.peticionRedimencionarArrayEnemigos = true;
                    }
                    else
                    {
                        ultimoEscudoFueLuz = true;
                        escudoLuz = true;
                        refGame.enemigoArray = new Enemigo[5];
                        refGame.enemigoArray[0] = this;
                        refGame.enemigoArray[1] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 8, 50, 32, null, false);
                        refGame.enemigoArray[1].setStatsCustom(1, 500000, 1, 1, 1, 1);
                        refGame.enemigoArray[1].tieneDrop = false;
                        refGame.enemigoArray[2] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 8, 47, 32, null, false);
                        refGame.enemigoArray[2].setStatsCustom(1, 500000, 1, 1, 1, 1);
                        refGame.enemigoArray[2].tieneDrop = false;
                        refGame.enemigoArray[3] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 16, 50, 32, null, false);
                        refGame.enemigoArray[3].setStatsCustom(1, 500000, 1, 1, 1, 1);
                        refGame.enemigoArray[3].tieneDrop = false;
                        refGame.enemigoArray[4] = new EnemigoPasivo(-1, refControl.otrasTexturas[58], 16, 47, 32, null, false);
                        refGame.enemigoArray[4].setStatsCustom(1, 500000, 1, 1, 1, 1);
                        refGame.enemigoArray[4].tieneDrop = false;
                        refGame.peticionRedimencionarArrayEnemigos = true;
                    }
                }
                adds = new Enemigo[4];
                adds[0] = refGame.enemigoArray[1];
                adds[1] = refGame.enemigoArray[2];
                adds[2] = refGame.enemigoArray[3];
                adds[3] = refGame.enemigoArray[4];
            }
        }

        cdWave -= Game.elapsed;
        if (cdWave < 0f)
        {
            cdWave = 5f;
            CrearMisil();
        }

        if (gMisil1x)   //si esta vivo el grupo
        {
            count1x -= Game.elapsed;
            offsetM1x += Game.elapsed * 10f;
            if (count1x < 0f)
            {
                count1x = 0f;
                gMisil1x = false;

            }
            CheckMisil(0);
        }

        if (gMisil2x)   //si esta vivo el grupo
        {
            count2x -= Game.elapsed;
            offsetM2x -= Game.elapsed * 10f;
            if (count2x < 0f)
            {
                count2x = 0f;
                gMisil2x = false;

            }
            CheckMisil(1);
        }

        if (gMisil1y)   //si esta vivo el grupo
        {
            count1y -= Game.elapsed;
            offsetM1y += Game.elapsed * 10f;
            if (count1y < 0f)
            {
                count1y = 0f;
                gMisil1y = false;

            }
            CheckMisil(2);
        }

        if (gMisil2y)   //si esta vivo el grupo
        {
            count2y -= Game.elapsed;
            offsetM2y -= Game.elapsed * 10f;
            if (count2y < 0f)
            {
                count2y = 0f;
                gMisil2y = false;

            }
            CheckMisil(3);
        }
    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 7;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado || escudoSom || escudoLuz)
            return 0;
        return base.RecibirDmg(dmg, critico);
    }

    private void CrearMisil()
    {
        int n = -1;
        if (gMisil1x && gMisil2x && gMisil1y && gMisil2y)
            return;

        while (n == -1)
        {
            n = Random.Range(0, 4);

            switch (n)
            {
                case 0:
                    if (gMisil1x)
                        n = -1;
                    else
                    {
                        gMisil1x = true;
                        offsetM1x = 0f;
                        count1x = 10f;
                        for (int i = 0; i < cMisil1x.Length; i++)
                        {
                            cMisil1x[i] = true;
                        }
                    }
                    break;
                case 1:
                    if (gMisil2x)
                        n = -1;
                    else
                    {
                        gMisil2x = true;
                        offsetM2x = 0f;
                        count2x = 10f;
                        for (int i = 0; i < cMisil2x.Length; i++)
                        {
                            cMisil2x[i] = true;
                        }
                    }
                    break;
                case 2:
                    if (gMisil1y)
                        n = -1;
                    else
                    {
                        gMisil1y = true;
                        offsetM1y = 0f;
                        count1y = 10f;
                        for (int i = 0; i < cMisil1y.Length; i++)
                        {
                            cMisil1y[i] = true;
                        }
                    }
                    break;
                case 3:
                    if (gMisil2y)
                        n = -1;
                    else
                    {
                        gMisil2y = true;
                        offsetM2y = 0f;
                        count2y = 10f;
                        for (int i = 0; i < cMisil2y.Length; i++)
                        {
                            cMisil2y[i] = true;
                        }
                    }
                    break;
            }
        }
    }

    private void CheckMisil(int index)
    {
        
        if (index < 0 && index > 3)
            index = 0;

        switch (index)
        {
            case 0:
                for (int i = 0; i < misil1x.Length; i++)
                {
                    if (!cMisil1x[i])
                        continue;
                    int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+offsetM1x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                    int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-misil1x[i].y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                    if (x > (Screen.width / 2 - CONFIG.TAM) && x < (Screen.width / 2 + CONFIG.TAM / 2.0f) && y > (Screen.height / 2 - CONFIG.TAM) && y < (Screen.height / 2 + CONFIG.TAM / 2.0f))
                    {
                        cMisil1x[i] = false;
                        refGame.player.RecibirDmg(dmg, false);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < misil2x.Length; i++)
                {
                    if (!cMisil2x[i])
                        continue;
                    int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (23 + offsetM2x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                    int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-misil2x[i].y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                    if (x > (Screen.width / 2 - CONFIG.TAM) && x < (Screen.width / 2 + CONFIG.TAM / 2.0f) && y > (Screen.height / 2 - CONFIG.TAM) && y < (Screen.height / 2 + CONFIG.TAM / 2.0f))
                    {
                        cMisil2x[i] = false;
                        refGame.player.RecibirDmg(dmg, false);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < misil1y.Length; i++)
                {
                    if (!cMisil1y[i])
                        continue;
                    int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+misil1y[i].x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                    int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-offsetM1y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                    if (x > (Screen.width / 2 - CONFIG.TAM) && x < (Screen.width / 2 + CONFIG.TAM / 2.0f) && y > (Screen.height / 2 - CONFIG.TAM) && y < (Screen.height / 2 + CONFIG.TAM / 2.0f))
                    {
                        cMisil1y[i] = false;
                        refGame.player.RecibirDmg(dmg, false);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < misil2y.Length; i++)
                {
                    if (!cMisil2y[i])
                        continue;
                    int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+misil2y[i].x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
                    int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-59 - offsetM2y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);

                    if (x > (Screen.width / 2 - CONFIG.TAM) && x < (Screen.width / 2 + CONFIG.TAM / 2.0f) && y > (Screen.height / 2 - CONFIG.TAM) && y < (Screen.height / 2 + CONFIG.TAM / 2.0f))
                    {
                        cMisil2y[i] = false;
                        refGame.player.RecibirDmg(dmg, false);
                    }
                }
                break;
        }




    }
}
