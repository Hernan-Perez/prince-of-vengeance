using UnityEngine;
using System.Collections.Generic;
using System;

public class Enemigo : EntidadCombate
{
	public enum AiState {IDLE, ROAM, TARGET_FOCUS, TARGET_LOST, TARGET_ATK, DEAD};
	protected AiState _estadoAI;
    
    public AiState estadoAI
    {
        get
        {
            return _estadoAI;
        }
    }
	protected static Texture2D _barraRoja, _barraGris;

	//variables de ROAM. Se usan en enemigos y NPC
	protected Vector2 _posBase; //posicion a la que vuelve desp de perseguir al jugador o durante roam
	protected Vector2 _ptoRoam;
	protected bool _buscarNuevoPuntoRoam;
	protected float _tiempoPausaRoam;	//el tiempo que espera es variable
	protected float _tiempoInicioPausaRoam;
    protected int _oro;
    protected bool _drawDrop;
	protected Item[] _drop;
	protected int _expDrop;
	protected List<Vector3> _dmgRecibidoMensaje;		// (x, y) = (dmg, Game.TiempoTranscurrido)
	protected float _intervaloGolpe;
    protected string _clase = "enemigo";
    public bool tieneDrop = true;
    protected int cantItemsDrop = 1;

    public Texture buffAlternativo = null;

    public string clase
    {
        get
        {
            return _clase;
        }
    }

    public float intervaloGolpe
    {
        get
        {
            return _intervaloGolpe;
        }
        set
        {
            _intervaloGolpe = value;
        }
    }
	protected float _ultimoGolpe;
    protected bool _melee;
    public bool esMelee()
    {
        return _melee;
    }
    protected bool _barraVidaVisible;
    protected float _maxDistTarget, _maxDistAtaque;
    public float maxDistTarget
    {
        get
        {
            return _maxDistTarget;
        }
        set
        {
            _maxDistTarget = value;
        }
    }
    public float maxDistAtaque
    {
        get
        {
            return _maxDistAtaque;
        }
        set
        {
            _maxDistAtaque = value;
        }
    }
    protected ITEMLIST.ITEM_GROUP _itemGroup;
    protected bool _afectadoPorHorario;
    protected GUIStyle estiloDmg;
    protected GUIStyle estilo;
    protected Vector2 distPlayer;
    protected Vector2 distBase;
    protected Color _colorLayer;
    protected int _presetAnimacion;
    public int presetAnimacion
    {
        get
        {
            return _presetAnimacion;
        }
    }
    protected bool mago = false;
    protected Texture2D buffMago = null;
    protected Vector2 posBuffMago, microposBuffMago;
    protected float buffMagotiempo = 0f;
    protected bool buffMagoVisible = false;

    public Enemigo() : base()
	{
		//constructor default
	}

    public Enemigo(Texture2D spr, int posX, int posY, int nivel, bool esMelee, ITEMLIST.ITEM_GROUP iG = null, int presetAnim = -1) : base(spr, posX, posY, nivel, new Vector2(0, -1))
    {
        _tipo = TIPO.ENEMIGO;
        _drawDrop = true;
        _estadoAI = AiState.ROAM;
        _posBase = new Vector2(pos.x, pos.y);
        _buscarNuevoPuntoRoam = true;
        _tiempoPausaRoam = _tiempoInicioPausaRoam = 0.0f;
        _velocidadBase = 2;
        _morirTransparencia = 0.0f;
        _dragListaDrop = 0;
        _ultimoGolpe = 0;
        _intervaloGolpe = 2.0f;
        _barraVidaVisible = true;
        _melee = esMelee;
        _itemGroup = iG;
        _afectadoPorHorario = true;

        estiloDmg = new GUIStyle();
        estiloDmg.normal.textColor = Color.red;
        estiloDmg.fontSize = UTIL.TextoProporcion(24);
        estiloDmg.alignment = TextAnchor.UpperCenter;
        estilo = new GUIStyle();
        estilo.normal.textColor = Color.yellow;
        estilo.fontSize = UTIL.TextoProporcion(28);
        estilo.alignment = TextAnchor.UpperCenter;
        _presetAnimacion = presetAnim;
        if (_melee)
        {
            _maxDistTarget = 5.0f;
            _maxDistAtaque = 1.2f;
        }
        else
        {
            _maxDistTarget = 7.0f;
            _maxDistAtaque = 3.0f;
            AjustarCoordenadasSheet(1); //se ejecuta primero con 0 de parametro porque se hereda del constructor de entidad, pero se reemplaza con este
        }

        if (presetAnim >= 0 && presetAnim <= 3)
        {
            AjustarCoordenadasSheet(presetAnim);
        }

		_dmgRecibidoMensaje = new List<Vector3>();
		AjustarNivel();
        _colorLayer = Color.white;
	}

    public static void setTexBarraVida(Texture2D barra_Roja, Texture2D barra_Gris)
    {
        _barraGris = barra_Gris;
        _barraRoja = barra_Roja;
    }

	public override void Draw (Vector2 posPlayer, Vector2 microPosPlayer)
	{
		if (_state == estado.miss || _state == estado.muerto2)	//esto podria ser mejor mandarlo a entidad combativa, ver mejor
		{
			return;
		}
		GUI.BeginGroup (new Rect (Screen.width/2  - CONFIG.TAM/2 + (+ pos.x - posPlayer.x) * CONFIG.TAM - microPosPlayer.x + microPosAbsoluta.x, Screen.height/2 - CONFIG.TAM/2 + (- pos.y + posPlayer.y) * CONFIG.TAM + microPosPlayer.y - microPosAbsoluta.y, CONFIG.TAM, CONFIG.TAM));
        if (_state == estado.muerto)
            GUI.color = new Color(_colorLayer.r, _colorLayer.g, _colorLayer.b, 1.0f - 0.25f * _morirTransparencia);
        else
            GUI.color = _colorLayer;
        GUI.DrawTexture (new Rect (- _sprActual.x * CONFIG.escala, - _sprActual.y * CONFIG.escala, 832 * CONFIG.escala, 1344 * CONFIG.escala), _sprite);
		GUI.color = Color.white;
		GUI.EndGroup ();
	}

	public override void PostDraw (Vector2 posPlayer, Vector2 microPosPlayer)
	{
        //AGREGAR ACA VARIABLE PARA BUFF ALTERNATIVO Y FUNCION PARA CARGARLO
        //DESPUES AGREGAR FUNCION PARA DESACTIVAR ROAM Y QUE SE QUEDE COMO MUERTO O FIJARSE SI CON PONER AI = MUERTO ALCANZA
        //Y DESPUES ESTARIA BUENO PODER DESACTIVAR EL DROP, NO TIENE SENTIDO QUE LOS TOTEMS DEN XP Y ORO

        if (buffAlternativo != null && Estado != estado.miss)
        {
            int xx = (int)(Screen.width / 2 - CONFIG.TAM / 2 + (+pos.x - posPlayer.x) * CONFIG.TAM + microPosAbsoluta.x - microPosPlayer.x);
            int yy = (int)(Screen.height / 2 - CONFIG.TAM / 2 + (-(pos.y) + posPlayer.y) * CONFIG.TAM - microPosAbsoluta.y + microPosPlayer.y);
            GUI.color = new Color(1f, 1f, 1f, 0.75f);
            GUI.DrawTexture(new Rect(xx, yy, CONFIG.TAM, CONFIG.TAM), buffAlternativo);
            GUI.color = Color.white;
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


		if (_state == estado.miss)
		{
			return;
		}

		

		int x = (int)(Screen.width/2 - CONFIG.TAM/2 + (+ pos.x - posPlayer.x) * CONFIG.TAM - microPosPlayer.x + microPosAbsoluta.x);
		int y = (int)(Screen.height/2 - CONFIG.TAM/2 + (- pos.y + posPlayer.y) * CONFIG.TAM + microPosPlayer.y - microPosAbsoluta.y);
		if (x >= -CONFIG.TAM && x <= Screen.width && y >= -CONFIG.TAM && y <= Screen.height)
		{
			//los mensajes de dmg se muestran aunque este muerto, PERO EL DROP POR ENSIMA DE ESTO

			if (_state == estado.muerto) //dibujar texto de drop
			{
				for (int i = 0; i < _dmgRecibidoMensaje.Count; i++)
				{
					if ((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) > 1.5f)
					{
						_dmgRecibidoMensaje.RemoveAt(i);
						i--;
						continue;
					}
					//cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
					estiloDmg.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y)/1.5f);
					GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido -  _dmgRecibidoMensaje[i].y)/1.5f) * 1.0f + i * 0.3f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString(), estiloDmg);
				}
                estilo.normal.textColor = Color.yellow;
                int c = 0;
				for (c = 0; _drop != null && c < _drop.GetLength(0); c++)
				{
                    if (_drop[c] == null)
                        continue;
					//cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
					estilo.normal.textColor = new Color(_drop[c].ColorTexto.r, _drop[c].ColorTexto.g, _drop[c].ColorTexto.b, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dragListaDrop)/2.0f);
					GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dragListaDrop)/2.0f) * 1.0f + c * 0.3f), CONFIG.TAM * 3, 5), "+" + _drop[c].Nombre, estilo);
				}
                estilo.normal.textColor = Color.white;
                if (_expDrop != 0)
				    GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido - _dragListaDrop)/2.0f) * 1.0f + c * 0.3f), CONFIG.TAM * 3, 5), "+" + _expDrop + " exp", estilo);
			}
			else
			{
                if (_barraVidaVisible)
                {
                    GUI.DrawTexture(new Rect(x, y + CONFIG.TAM * (-1.0f + 0.70f), CONFIG.TAM, CONFIG.TAM * 0.25f), _barraGris);
                    GUI.DrawTexture(new Rect(x, y + CONFIG.TAM * (-1.0f + 0.70f), CONFIG.TAM * ((float)_hp / _hpMax), CONFIG.TAM * 0.25f), _barraRoja);
                }
				
				for (int i = 0; i < _dmgRecibidoMensaje.Count; i++)
				{
					if ((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) > 1.5f)
					{
						_dmgRecibidoMensaje.RemoveAt(i);
						i--;
						continue;
					}
					//cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
					estiloDmg.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y)/1.5f);
					if (_dmgRecibidoMensaje[i].z == 1)
					{
						estiloDmg.fontSize = UTIL.TextoProporcion(30);
						estiloDmg.normal.textColor = new Color(1f, 0.5f, 0f);
						GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido -  _dmgRecibidoMensaje[i].y)/1.5f) * 1.0f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString() + "!", estiloDmg);
						estiloDmg.fontSize = UTIL.TextoProporcion(24);
						estiloDmg.normal.textColor = Color.red;
					}
					else
						GUI.Label(new Rect(x - CONFIG.TAM * 1, y - (float)CONFIG.TAM * (((Game.TiempoTranscurrido -  _dmgRecibidoMensaje[i].y)/1.5f) * 1.0f), CONFIG.TAM * 3, 5), _dmgRecibidoMensaje[i].x.ToString(), estiloDmg);
				}
			}

		}
	}

	public override void EjecutarAccionAI()
	{

        if (_state == estado.miss)
        {
            refGame.peticionRedimencionarArrayEnemigos = true;
            return;
        }
        if (_state == estado.muerto || _estadoAI == AiState.DEAD || _estadoAI == AiState.IDLE)
		{
			return;
		}
        /*LENTA*/
        distPlayer = new Vector2(pos.x + microPos.x / 100f - refGame.player.pos.x - refGame.player.microPos.x / 100f, pos.y + microPos.y / 100f - refGame.player.pos.y - refGame.player.microPos.y / 100f);
        //RAPIDA //distPlayer = new Vector2(pos.x - refGame.player.pos.x, pos.y - refGame.player.pos.y);
        distBase = new Vector2(pos.x - _posBase.x, pos.y - _posBase.y);

		if (_estadoAI != AiState.TARGET_FOCUS && _estadoAI != AiState.TARGET_ATK && distPlayer.magnitude <= _maxDistTarget && distBase.magnitude <= 6.0f)//comprueba si el jugador esta cerca del enemigo
		{
			CambiarEstado(estado.caminando);
			_estadoAI = AiState.TARGET_FOCUS;
		}

		if (_estadoAI == AiState.TARGET_ATK && distPlayer.magnitude > _maxDistAtaque)	// si se alejo del rango de ataque lo vuelve a perseguir
		{
			CambiarEstado(estado.caminando);
			_estadoAI = AiState.TARGET_FOCUS;
		}

		if (_estadoAI == AiState.TARGET_FOCUS)
		{
			if (distPlayer.magnitude > _maxDistTarget /*|| distBase.magnitude > 10f*/)  //HAGO ESTE CAMBIO PARA PROBAR, SOLO VUELVE SI EL JUGADOR SE ALEJA MUCHO
			{
				CambiarEstado(estado.caminando);
				_estadoAI = AiState.TARGET_LOST;
			}
			else
			{
				if (distPlayer.magnitude <= _maxDistAtaque)	//comprueba si esta en rango de ataque
				{
                    //CambiarEstado(estado.atacando);
                    CambiarEstado(estado.idle);
					_estadoAI = AiState.TARGET_ATK;
				}
				else	//si todavia no esta en rango de ataque se acerca
				{
					MoverAPos(refGame.player.pos, velocidad);
				}
			}
		}

		if (_estadoAI == AiState.TARGET_LOST)
		{
			if (MoverAPos(_posBase, velocidad))	//aca se puede producir un bug, seria mejor si se moviera ignorando los obstaculos
			{
				_estadoAI = AiState.ROAM;
				return;	//como ya se movio, se deberia mover en roam en la proxima iteracion
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
				refGame.player.RecibirDmg((int)UnityEngine.Random.Range(_dmgMin, _dmgMax + 1), false); //LOS ENEMIGOS ?PEGAN CRITICO??
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

		if (_estadoAI == AiState.ROAM)	//Deambular a partir del punto base. OJO LAS COLISIONES
		{
			if (_tiempoInicioPausaRoam != 0.0f)	//si es 0 significa que no esta en pausa
			{
				if ((_tiempoInicioPausaRoam + _tiempoPausaRoam) <= Game.TiempoTranscurrido)	//fin de pausa
				{
					_tiempoInicioPausaRoam = 0.0f;
					_buscarNuevoPuntoRoam = true;
				}
				else  //sigue en pausa
				{
					return;
				}
			}

			if (_buscarNuevoPuntoRoam)	//calcular punto random para desplazarse
			{
				int auxX, auxY;
				auxX = (int)UnityEngine.Random.Range(_posBase.x - 3, _posBase.x + 3);
				auxY = (int)UnityEngine.Random.Range(_posBase.y - 3, _posBase.y + 3);
				_ptoRoam = new Vector2(auxX, auxY);
				_buscarNuevoPuntoRoam = false;
				CambiarEstado(estado.caminando);
			}
			//moverse hacia el nuevo pto
			if (MoverAPos(_ptoRoam, velocidad))
			{
				_tiempoInicioPausaRoam = Game.TiempoTranscurrido;
				_tiempoPausaRoam = UnityEngine.Random.Range(1.5f, 3.0f);
				CambiarEstado(EntidadCombate.estado.idle);
			}
			return;
		}
	}

	protected override void AjustarNivel ()
	{
		if (_nivel < 1)
		{
			Debug.LogWarning("Nivel enemigo menor a 1. Corregido a 1");
			_nivel = 1;
		}

        //_hpMax = _hp = (int)(140.0 + 40.0 * Math.Pow(_nivel, 1.25));
        _hpMax = _hp = (int)(140.0 + 40.0 * Math.Pow(_nivel, 1.5));
        _hp = _hpMax;   //esto por ahora lo dejo asi, pero repensar
        _dmgMax = (int)(12.0 + 5.0 * Math.Pow(_nivel, 1.25));
        _dmgMin = (int)(8.0 + 4.0 * Math.Pow(_nivel, 1.25));

        //_hpMax = _hp = 100 + 42 * _nivel;
		_expDrop = (int)(60.0 * Math.Pow(_nivel, 1.4));
		//_dmgMax = 10 + 5 * _nivel;
		//_dmgMin = 8 + 4 * _nivel;
        _oro = (int)(2.5f * Math.Pow(_nivel, 1.5f));

        if (CONFIG.MODO_EXTREMO)
        {
            _expDrop = (int)(_expDrop * 1.5f);
            if (refGame._bossDerrotado[5] == false)
            {
                _hp = _hpMax = (int)(_hpMax * 2f);
                _dmgMin = _dmgMin * 2;
                _dmgMax = _dmgMax * 2;
            }
            else
            {
                _hp = _hpMax = (int)(_hpMax * 10f);
                _dmgMin = _dmgMin * 5;
                _dmgMax = _dmgMax * 5;
            }

        }
    }

	public override int RecibirDmg (int dmg, bool critico = false)
	{
		if (_state == estado.miss || _state == estado.muerto)	//sacar lo de estado muerto para debuggear drop, (mientras esta muerto se le puede seguir pegando y consiguiendo mas drop)
			return 0;

        dmg = (int)(dmg * _modificadorDef);
		_hp -= dmg;
		int outputDmg = dmg;
		if (_hp < 0)
		{
			outputDmg += _hp;
			_hp = 0;
            Morir();
		}

		_dmgRecibidoMensaje.Add(new Vector3(outputDmg, Game.TiempoTranscurrido, (critico?1:0)));

		return outputDmg;
	}

    protected override void Morir()
    {
        _state = estado.muerto;
        _estadoAI = AiState.DEAD;
        if (!tieneDrop)
            return;

        _drop = UTIL.AsignarDrop((int)(_oro), _itemGroup, cantItemsDrop);
        for (int i = 0; i < _drop.Length; i++)
        {
            refGame.player.inventario.AgregarNuevoItem(_drop[i]);
        }

        _dragListaDrop = Game.TiempoTranscurrido;
        refGame.player.Exp += _expDrop;
    }

    public void setBarraVidaVisible(bool cond)
    {
        _barraVidaVisible = cond;
    }

    public void setColorLayer(Color col)
    {
        _colorLayer = col;
    }

    public void setStatsCustom(int lvl, int hp, int dmgmin, int dmgmax, int oro, int xp)
    {
        _nivel = lvl;
        _hpMax = _hp = hp;
        _dmgMin = dmgmin;
        _dmgMax = dmgmax;
        _oro = oro;
        _expDrop = xp;
    }

    public void setMago(Texture2D buff)
    {
        mago = true;
        buffMago = buff;
        _intervaloGolpe = 5.0f;
    }

    public bool esMago()
    {
        return mago;
    }

    public Texture2D getTexMago()
    {
        return buffMago;
    }
}
