using UnityEngine;

public sealed class BossModrean2 : Boss
{
    private float contadorTiempo = 1f;
    private float offset = 0f;
    private Texture2D _buff;
    public BossModrean2(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(73), spr, posX, posY, presetAnim)
    {
        _codigo = 8;
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
        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        cantItemsDrop = 4;


        _itemGroup = ITEMLIST.Instance.getPreset(7, new Vector4(0f, 0f, 0f, 100f));
        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }

        setStatsCustom(70, 400000, 1000, 1250, 43924, 344651);
        

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 5;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 0f, 100f));
            setStatsCustom(150, 6000000, 2000, 10000, 150000, 1000000);
        }

        _hp = (int)(_hpMax * 0.2f);
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
            GUI.color = Color.red;
            Matrix4x4 matrixBackup = GUI.matrix;

            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - 6f - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y + 6f) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);

            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);

            GUIUtility.RotateAroundPivot(30.0f * offset, new Vector2(xx + CONFIG.TAM / 2f, yy + CONFIG.TAM / 2f));

            GUI.DrawTexture(new Rect(x + CONFIG.TAM / 2f, y + CONFIG.TAM / 2f, CONFIG.TAM * 12f, CONFIG.TAM * 12f), refControl.skills[17], ScaleMode.ScaleAndCrop);
            GUI.matrix = matrixBackup;
            GUI.color = Color.white;

        }


        //GUI.DrawTexture(new Rect(Screen.width / 2 - CONFIG.TAM * 4.5f, Screen.height / 2 - CONFIG.TAM * 4.5f, CONFIG.TAM * 9f, CONFIG.TAM * 9f), refControl.skills[17], ScaleMode.ScaleAndCrop);

        base.PostDraw(posPlayer, microPosPlayer);
    }

    public override void EjecutarAccionAI()
    {
        if (_state == estado.miss)
        {
            refGame.peticionRedimencionarArrayEnemigos = true;
            refGame.setBossDerrotado(_codigo);
            refGame._pNuevaPos = new Vector2(49, 58);
            refGame.player.microPos = new Vector2();
            refGame.IniciarTransicionMapa("mapas/Z8-13");
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
                //refControl.PlayMusica(11);
                refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(85));
            }

            _estadoAI = AiState.TARGET_ATK;
            //CambiarEstado(estado.atacando);
        }

        if (!_activado)
            return;

        base.Actualizar();
        offset += Game.elapsed;

        contadorTiempo -= Game.elapsed;
        if (contadorTiempo < 0f)
        {
            contadorTiempo = 1f;
            refGame.player.RecibirDmg((int)(getDmgMax()/2f), false);
        }

    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 8;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;
        if (!CONFIG.MODO_EXTREMO)
        {
            dmg = (int)(dmg * 0.4f);
        }
        else
        {
            dmg = (int)(dmg * 0.85f);
        }
        
        return base.RecibirDmg(dmg, critico);
    }
}
