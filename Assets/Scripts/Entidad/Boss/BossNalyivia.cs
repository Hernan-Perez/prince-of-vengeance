using UnityEngine;

public sealed class BossNalyivia : Boss
{
    private float contadorTiempo;
    private bool teleportEfecto = false;
    private float countVisualTP = 0f;
    private Texture2D _buff;
    private bool derr1 = false;
    private Rect areaValida;
    private float cdTeleport = 10f;
    private float cdWave = 5f;

    //private Rect hitboxHor, hitboxVer;
    private Vector2[] misil1x, misil2x, misil1y, misil2y;   //posiciones BASE
    private bool gMisil1x = false, gMisil2x = false, gMisil1y = false, gMisil2y = false;    //si esta vivo el grupo
    private float count1x, count2x, count1y, count2y;
    private bool[] cMisil1x, cMisil2x, cMisil1y, cMisil2y;  //estan vivos
    private float offsetM1x, offsetM2x, offsetM1y, offsetM2y;

    private int dmg;

    public BossNalyivia(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(69), spr, posX, posY, presetAnim)
    {
        _codigo = 3;
        _activado = true;
        ActivarBoss(true);
        refControl.PlayMusica(10);
        areaValida = new Rect(8, 8, 21, 24);

        misil1x = new Vector2[10];
        misil1x[0] = new Vector2(0, 0);
        misil1x[1] = new Vector2(0, 4);
        misil1x[2] = new Vector2(0, 8);
        misil1x[3] = new Vector2(0, 12);
        misil1x[4] = new Vector2(0, 16);
        misil1x[5] = new Vector2(0, 20);
        misil1x[6] = new Vector2(0, 24);
        misil1x[7] = new Vector2(0, 28);
        misil1x[8] = new Vector2(0, 32);
        misil1x[9] = new Vector2(0, 36);
        cMisil1x = new bool[10];
        for (int i = 0; i < cMisil1x.Length; i++)
        {
            cMisil1x[i] = true;
        }
        misil2x = new Vector2[9];
        misil2x[0] = new Vector2(40, 2);
        misil2x[1] = new Vector2(40, 6);
        misil2x[2] = new Vector2(40, 10);
        misil2x[3] = new Vector2(40, 14);
        misil2x[4] = new Vector2(40, 18);
        misil2x[5] = new Vector2(40, 22);
        misil2x[6] = new Vector2(40, 26);
        misil2x[7] = new Vector2(40, 30);
        misil2x[8] = new Vector2(40, 34);
        cMisil2x = new bool[9];
        for (int i = 0; i < cMisil2x.Length; i++)
        {
            cMisil2x[i] = true;
        }

        misil1y = new Vector2[10];
        misil1y[0] = new Vector2(0, 0);
        misil1y[1] = new Vector2(4, 0);
        misil1y[2] = new Vector2(8, 0);
        misil1y[3] = new Vector2(12, 0);
        misil1y[4] = new Vector2(16, 0);
        misil1y[5] = new Vector2(20, 0);
        misil1y[6] = new Vector2(24, 0);
        misil1y[7] = new Vector2(28, 0);
        misil1y[8] = new Vector2(32, 0);
        misil1y[9] = new Vector2(36, 0);
        cMisil1y = new bool[10];
        for (int i = 0; i < cMisil1y.Length; i++)
        {
            cMisil1y[i] = true;
        }
        misil2y = new Vector2[9];
        misil2y[0] = new Vector2(2, 40);
        misil2y[1] = new Vector2(6, 40);
        misil2y[2] = new Vector2(10, 40);
        misil2y[3] = new Vector2(14, 40);
        misil2y[4] = new Vector2(18, 40);
        misil2y[5] = new Vector2(22, 40);
        misil2y[6] = new Vector2(26, 40);
        misil2y[7] = new Vector2(30, 40);
        misil2y[8] = new Vector2(34, 40);
        cMisil2y = new bool[9];
        for (int i = 0; i < cMisil2y.Length; i++)
        {
            cMisil2y[i] = true;
        }

        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 2.5f;
        contadorTiempo = 0f;
        _maxDistTarget = 2000f;
        _maxDistAtaque = 2000f;
        _modificadorVelocidad = 0.75f;

        setStatsCustom(25, 35000, 400, 700, 9375, 81537);
        cantItemsDrop = 2;
        _itemGroup = ITEMLIST.Instance.getPreset(2, new Vector4(0f, 0f, 0f, 100f));

        _buff = refControl.otrasTexturas[37];
        setMago(refControl.otrasTexturas[50]);

        refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(79));


        dmg = 1000;

        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 100f, 0f));
            setStatsCustom(105, 800000, 8500, 12500, 80694, 502502);
            dmg = 25000;
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
                int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (39 + offsetM2x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
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
                int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-39 - offsetM2y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[54]);
            }



            if (mago && buffMagoVisible)
            {
                float transparencia = Game.TiempoTranscurrido - buffMagotiempo;
                if (transparencia > 1f)
                {
                    buffMagoVisible = false;
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, 1f - transparencia);
                    int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+posBuffMago.x - posPlayer.x) * CONFIG.TAM + microposBuffMago.x - microPosPlayer.x);
                    int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(posBuffMago.y) + posPlayer.y) * CONFIG.TAM - microposBuffMago.y + microPosPlayer.y);
                    GUI.DrawTexture(new Rect(xx, yy, CONFIG.TAM, CONFIG.TAM), buffMago);

                    GUI.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }
        

        if (teleportEfecto && estadoAI != AiState.DEAD)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
            if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), _buff);
            }
        }

        base.PostDraw(posPlayer, microPosPlayer);
    }

    public override void EjecutarAccionAI()
    {
        if (_state == estado.miss && !derr1)
        {
            derr1 = true;
            countVisualTP = 0f;
            teleportEfecto = false;
            cdTeleport = 100f;
            refGame.almaPoder = true;
            refGame.hud.npcActivo = null;
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(77));

            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            refGame._npcArray = new NPC[1];
            Portal aux = new Portal(false);
            refGame._npcArray[0] = aux;
            return;
        }
        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        if (countVisualTP != 0f)
        {
            countVisualTP -= Game.elapsed;
            if (countVisualTP < 0f)
            {
                countVisualTP = 0f;
                teleportEfecto = false;
            }
            
        }

        if (cdTeleport != 0f)
        {
            cdTeleport -= Game.elapsed;
            if (cdTeleport < 0f)
            {
                cdTeleport = 10f;
                Teleport();
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

                if (mago)
                {
                    buffMagoVisible = true;
                    buffMagotiempo = Game.TiempoTranscurrido;
                    posBuffMago = refGame.player.pos;
                    microposBuffMago = refGame.player.microPosAbsoluta;
                }

            }
        }
    }

    public override void Actualizar()
    {
        if (!_activado)
            return;

        base.Actualizar();
        contadorTiempo += Game.elapsed;

    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 3;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;
        return base.RecibirDmg(dmg, critico);
    }

    private void Teleport()
    {
        Vector2 nPos = new Vector2();
        do
        {
            nPos = new Vector2(
                Random.Range((int)areaValida.x, (int)areaValida.xMax), 
                Random.Range((int)areaValida.y, (int)areaValida.yMax));
        }
        while (!puntoValido(nPos));

        microPos = new Vector2();
        pos = nPos;
        teleportEfecto = true;
        countVisualTP = 1f;
    }

    private bool puntoValido (Vector2 p)
    {
        return !refGame.currentMapa.esPosObstaculo((int)(p.x + p.y * refGame.currentMapa.DIMX));
    }

    private void CrearMisil()
    {
        int n = -1;
        if (gMisil1x && gMisil2x && gMisil1y && gMisil2y)
            return;

        while (n == -1)
        {
            n = Random.Range(0, 4);

            switch(n)
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
                    int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (39 + offsetM2x - refGame.player.pos.x) * CONFIG.TAM - refGame.player.microPosAbsoluta.x);
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
                    int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (- 39 - offsetM2y + refGame.player.pos.y) * CONFIG.TAM + refGame.player.microPosAbsoluta.y);

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
