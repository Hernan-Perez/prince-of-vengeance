using UnityEngine;
using System.Collections.Generic;

public sealed class BossGhaldum : Boss
{
    private float contadorTiempo = 10f; //arranca en 10 para el primer totem y despues va de a intervalos de 15 seg
    private Texture2D _buff;
    private Enemigo add1, add2;
    private bool matarEnemigos = true;
    private EnemigoPasivo tSalud, tDmg, tDef; //referencia a enemigos totems
    private float cdCuracion = 0f;

    public BossGhaldum(Texture2D spr, int posX, int posY, int presetAnim = -1, bool derrotado = false) : base("General Ghaldum", spr, posX, posY, presetAnim)
    {
        _codigo = 6;
        //activado = true;
        _hpMax = _hp = 2000;
        _expDrop = 740;
        _dmgMax = 11;
        _dmgMin = 4;
        _oro = 78;
        _estadoAI = AiState.IDLE;
        _intervaloGolpe = 1.2f;
        _maxDistTarget = 22f;
        _maxDistAtaque = 2f;
        _modificadorVelocidad = 0.75f;

        setStatsCustom(55, 114500, 680, 950, 30591, 245895);
        cantItemsDrop = 2;

        
        _itemGroup = ITEMLIST.Instance.getPreset(5, new Vector4(0f, 0f, 0f, 100f));

        if (derrotado)
        {
            _estadoAI = AiState.DEAD;
            _state = estado.miss;
        }
        if (CONFIG.MODO_EXTREMO)
        {
            cantItemsDrop = 1;
            _itemGroup = ITEMLIST.Instance.getPreset(10, new Vector4(0f, 0f, 0f, 100f));
            setStatsCustom(135, 4000000, 2500, 8000, 117641, 714151);
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
        
        if (Estado != estado.muerto && Estado != estado.miss)
        {
            int x = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+_pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int y = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(_pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);

            GUI.color = new Color(1f, 1f, 1f, 0.5f);
            if (tSalud != null)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[38]);
            }
            if (tDmg != null)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[36]);
            }
            if (tDef != null)
            {
                GUI.DrawTexture(new Rect(x, y, CONFIG.TAM, CONFIG.TAM), refControl.skills[15]);
            }
            GUI.color = Color.white;
        }

        base.PostDraw(posPlayer, microPosPlayer);

    }

    public override void EjecutarAccionAI()
    {
        if ((_state == estado.miss) && matarEnemigos)
        {
            matarEnemigos = false;
            refGame.enemigoArray = new Enemigo[1];
            refGame.enemigoArray[0] = this;
            refGame.peticionRedimencionarArrayEnemigos = true;
            _state = estado.miss;
            _estadoAI = AiState.DEAD;
        }


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
                float multiplicador = 1f;
                if (tDmg != null)
                {
                    multiplicador = 1.5f;   //si el totem dmg esta vivo pega 50% mas
                }
                refGame.player.RecibirDmg((int)Random.Range(_dmgMin * _modificadorDmg * multiplicador, _dmgMax * _modificadorDmg * multiplicador), false); //LOS ENEMIGOS ?PEGAN CRITICO??
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
                refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(82));
            }

            _estadoAI = AiState.TARGET_ATK;
            //CambiarEstado(estado.atacando);
        }

        base.Actualizar();

        if (_state == estado.muerto || _estadoAI == AiState.DEAD || !_activado)
            return;

        

        if (tSalud != null && tSalud.Estado == estado.miss)
        {
            tSalud = null;
        }
        else if (tSalud != null && _hp > 0)
        {
            cdCuracion -= Game.elapsed;
            if (cdCuracion <= 0f)
            {
                cdCuracion = 2f;
                _hp += (int)(_hpMax * 0.05f);
                if (_hp > _hpMax)
                {
                    _hp = _hpMax;
                }
            }
        }
        if (tDmg != null && tDmg.Estado == estado.miss)
        {
            tDmg = null;
        }
        if (tDef != null && tDef.Estado == estado.miss)
        {
            tDef = null;
        }


        contadorTiempo -= Game.elapsed;
        if (contadorTiempo <= 0f)
        {
            contadorTiempo = 20f;
            if (tDmg != null && tDef != null && tSalud != null)
            {}
            else
            {
                List<Enemigo> agregados = new List<Enemigo>();
                Enemigo aux;
                if (!CONFIG.MODO_EXTREMO)
                {
                    aux = new Enemigo(refControl.sprites[6], 23, 45, 50, true, null, 2);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);

                    aux = new Enemigo(refControl.sprites[23], 24, 45, 50, false, null, 1);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);

                    aux = new Enemigo(refControl.sprites[6], 25, 45, 50, true, null, 2);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);
                }
                else
                {
                    aux = new Enemigo(refControl.sprites[6], 23, 45, 50, true, null, 2);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);

                    aux = new Enemigo(refControl.sprites[23], 24, 45, 50, false, null, 1);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);

                    aux = new Enemigo(refControl.sprites[6], 25, 45, 50, true, null, 2);
                    aux.maxDistTarget = 5000f;
                    aux.tieneDrop = false;
                    agregados.Add(aux);
                }
                int op = -1;

                while (op == -1)
                {
                    op = Random.Range(0, 3);

                    switch (op)
                    {
                        case 0: //t vida
                            if (tSalud != null)
                            {
                                op = -1;
                            }
                            else
                            {
                                cdCuracion = 4f;    //la primera vez tarda mas en empezar a curarse
                                if (!CONFIG.MODO_EXTREMO)
                                {
                                    tSalud = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 24, 35, 40, null, false);
                                    tSalud.setStatsCustom(40, 2000, 0, 0, 0, 0);
                                }
                                else
                                {
                                    tSalud = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 24, 35, 130, null, false);
                                    tSalud.setStatsCustom(40, 15000, 0, 0, 0, 0);
                                }
                                tSalud.tieneDrop = false;
                                tSalud.buffAlternativo = refControl.otrasTexturas[38];
                                agregados.Add(tSalud);
                                
                            }
                            break;

                        case 1: //t dmg
                            if (tDmg != null)
                            {
                                op = -1;
                            }
                            else
                            {
                                if (!CONFIG.MODO_EXTREMO)
                                {
                                    tDmg = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 19, 38, 40, null, false);
                                    tDmg.setStatsCustom(40, 2000, 0, 0, 0, 0);
                                }
                                else
                                {
                                    tDmg = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 19, 38, 130, null, false);
                                    tDmg.setStatsCustom(40, 15000, 0, 0, 0, 0);
                                }
                                
                                tDmg.tieneDrop = false;
                                tDmg.buffAlternativo = refControl.otrasTexturas[36];
                                agregados.Add(tDmg);
                            }
                            break;

                        case 2: //t def
                            if (tDef != null)
                            {
                                op = -1;
                            }
                            else
                            {
                                if (!CONFIG.MODO_EXTREMO)
                                {
                                    tDef = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 29, 38, 40, null, false);
                                    tDef.setStatsCustom(40, 2000, 0, 0, 0, 0);
                                }
                                else
                                {
                                    tDef = new EnemigoPasivo(-1, refControl.otrasTexturas[46], 29, 38, 130, null, false);
                                    tDef.setStatsCustom(40, 15000, 0, 0, 0, 0);
                                }
                                
                                tDef.tieneDrop = false;
                                tDef.buffAlternativo = refControl.skills[15];
                                agregados.Add(tDef);
                            }
                            break;

                        default:
                            op = -1;
                            break;
                    }

                }

                agregarEnemigos(agregados);

            }
            
        }
        
    }

    protected override void AjustarNivel()
    {

    }

    public static int getCodBoss()
    {
        return 6;
    }

    public override int RecibirDmg(int dmg, bool critico = false)
    {
        if (!_activado)
            return 0;
        if (tDef != null)
            dmg = (int)(dmg * 0.25f);   //recibe 25% de dmg si el totme def esta activo
        return base.RecibirDmg(dmg, critico);
    }

    private float getPorcentajeHP()
    {
        float h = _hp;
        float hm = _hpMax;
        return (h / hm);
    }

    private void agregarEnemigos(List<Enemigo> enemigos)
    {
        Enemigo[] aux = refGame.enemigoArray;
        refGame.enemigoArray = new Enemigo[aux.Length + enemigos.Count];

        int i = 0, c = 0;
        for (i = 0; i < aux.Length; i++)
        {
            refGame.enemigoArray[c] = aux[i];
            c++;
        }

        for (i = 0; i < enemigos.Count; i++)
        {
            refGame.enemigoArray[c] = enemigos[i];
            c++;
        }
    }
}
