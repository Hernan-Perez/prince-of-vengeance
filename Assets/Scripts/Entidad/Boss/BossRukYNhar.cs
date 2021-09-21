using UnityEngine;

class BossRukYNhar : Boss
{
    /*
     * RUK es el guerrero, persigue al jugador asi que necesita comprobacion de colisiones con el entorno, RUK es el principal de los 2
     * Nhar el el arquero, esta quieto todo el tiempo (puede intercambiar la posicion con el RUK), no necesita comprobacion de colisiones 
     * -> Nhar clase Enemigo, dentro de esta clase
     * */

    private Enemigo Nhar;
    private float ultimoTiempoIntercambio = 0f;
    private float contDesaparecer = 0f;
    private bool freneziRukActivado = false;
    private bool freneziNharActivado = false;
    private Vector2 distNharAPlayer;
    private Texture2D _buff;

    public BossRukYNhar(Texture2D spr, int posX1, int posY1, int posX2, int posY2, Texture2D buff, int presetAnim = -1, bool derrotado = false) : base(CONFIG.getTexto(68), spr, posX1, posY1, presetAnim)
    {
        _codigo = 1;
        //setStatsCustom(17, 250, 22, 30, 300, 540);
        _estadoAI = AiState.IDLE;
        _maxDistTarget = 9999f;
        _maxDistAtaque = 1.5f;
        _activado = true;
        refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(74));
        refControl.PlayMusica(10);
        _buff = buff;


        Nhar = new Enemigo(refControl.sprites[39], posX2, posY2, 1, false, ITEMLIST.Instance.getPreset(0));
        Nhar.setBarraVidaVisible(false);
        Nhar.maxDistAtaque = 9999f;
        Nhar.maxDistTarget = 9999f;

        tieneDrop = false;
        Nhar.tieneDrop = false;
        setStatsCustom(20, 15000, 50, 75, 6708, 59065);
        Nhar.setStatsCustom(20, 5000, 150, 250, 0, 0);
        Nhar.intervaloGolpe = 2f;
        intervaloGolpe = 1f;
        _modificadorVelocidad = 0.65f;
        noDesaparecer = true;

        cantItemsDrop = 3;
        _itemGroup = ITEMLIST.Instance.getPreset(1, new Vector4(0f, 0f, 0f, 100f));

        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
            Nhar.Estado = estado.miss;
            refGame.peticionRedimencionarArrayEnemigos = true;
        }

        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 100f, 0f, 0f));
            setStatsCustom(100, 480000, 500, 3500, 75000, 484604);
            Nhar.setStatsCustom(100, 280000, 4000, 6000, 0, 0);
        }
    }

    /*public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        base.Draw(posPlayer, microPosPlayer);
        Nhar.Draw(posPlayer, microPosPlayer);
    }*/

    public override void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
    {
        base.PostDraw(posPlayer, microPosPlayer);
        if (Game.TiempoTranscurrido - ultimoTiempoIntercambio < 2f)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
            if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), _buff);
            }

            x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+Nhar.pos.x - posPlayer.x) * CONFIG.TAM + Nhar.microPosAbsoluta.x - microPosPlayer.x);
            y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(Nhar.pos.y) + posPlayer.y) * CONFIG.TAM - Nhar.microPosAbsoluta.y + microPosPlayer.y);
            if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), _buff);
            }
        }
    }

    public override void EjecutarAccionAI()
    {
        //Nhar.EjecutarAccionAI();

        if (_state == estado.muerto2 && Nhar.Estado == estado.miss && !tieneDrop)
        {
            tieneDrop = true;
            _state = estado.muerto;
            _drop = UTIL.AsignarDrop((int)(_oro), _itemGroup, cantItemsDrop);
            for (int i = 0; i < _drop.Length; i++)
            {
                refGame.player.inventario.AgregarNuevoItem(_drop[i]);
            }

            _dragListaDrop = Game.TiempoTranscurrido;
            refGame.player.Exp += _expDrop;

            contDesaparecer = 2f;
        }

        if (contDesaparecer != 0f)
        {
            contDesaparecer -= Game.elapsed;
            if (contDesaparecer < 0f)
            {
                _activado = false;
                _state = estado.miss;
                refGame.peticionRedimencionarArrayEnemigos = true;
                refGame.setBossDerrotado(_codigo);
            }
        }

        if (_state == estado.miss || _state == estado.muerto2)
        {
            //refGame.peticionRedimencionarArrayEnemigos = true;

            if (!freneziNharActivado)
            {
                freneziNharActivado = true;
                Nhar.setColorLayer(Color.red);
                Nhar.intervaloGolpe /= 2f;
            }
            return;
        }

        if (_state == estado.muerto ||_state == estado.muerto2 || _state == estado.miss || _estadoAI == AiState.DEAD || !_activado)
        {
            return;
        }

        

        distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
        distNharAPlayer = new Vector2(Nhar.pos.x + Nhar.microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, Nhar.pos.y + Nhar.microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
        //distBase = new Vector2(pos.x - _posBase.x, pos.y - _posBase.y);
        if (Nhar.Estado != estado.miss && Nhar.Estado != estado.muerto)
        {
            if (distNharAPlayer.magnitude < 3f && Game.TiempoTranscurrido - ultimoTiempoIntercambio > 10f)
            {
                Vector2 aux = pos;
                pos = Nhar.pos;
                Nhar.pos = aux;

                aux = microPos;
                microPos = Nhar.microPos;
                Nhar.microPos = aux;
                ultimoTiempoIntercambio = Game.TiempoTranscurrido;
            }
        }
        else
        {
            if (!freneziRukActivado)
            {
                freneziRukActivado = true;
                setColorLayer(Color.red);
                _intervaloGolpe /= 2f;
            }
        }

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

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (_state == estado.muerto || _state == estado.muerto2 || _state == estado.miss || _estadoAI == AiState.DEAD || !_activado)
        {
            return 0;
        }
        return base.RecibirDmg(dmg, critico);
    }

    /*public override void Actualizar()
    {
        base.Actualizar();
        Nhar.Actualizar();
    }*/

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 1;
    }

    public override int getHp()
    {
        return Nhar.getHp() + _hp;
    }

    public override int getHpMax()
    {
        return Nhar.getHpMax() + _hpMax;
    }

    public Enemigo getRefANhar()
    {
        return Nhar;
    }
}
