using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class Jugador : EntidadCombate
{
	private string _nombre;
    public string nombre
    {
        get
        {
            return _nombre;
        }
    }
	private Inventario _inventario;
    public Inventario inventario
    {
        get
        {
            return _inventario;
        }
    }
	private Skill[] _skills;
	private Skill[] _skillsPasivas;
	private int _exp, _expMax;
    public int Exp
    {
        get { return _exp; }
        set
        {
            _exp = value;
            if (_nivel < 99 && _exp >= _expMax)
            {
                PasarDeNivel();
            }
        }
    }
    public int ExpMax
    {
        get { return _expMax; }
    }

    //private bool _usandoImagenConCasco = false;
	private List<Vector2> _dmgRecibidoMensaje;	
	private SkillCuracionAll _curacionPasiva;
    private bool auraPoderActivada = false;
	private static float _DMG_GC = 1.5f;	//50% mas
    public static float DMG_GC
    {
        get
        {
            return _DMG_GC;
        }
    }
	private float _tiempoUltimoGolpeRecibido;
    public float tiempoUltimoGolpeRecibido
    {
        get
        {
            return _tiempoUltimoGolpeRecibido;
        }
    }
    private bool _animacionPasandoDeNivel;
	private float _timerPasandoDeNivel;
    private bool _flickAnimacionSubirNivel;
    private float _ultimoFlickAnimacionSubirNivel;

    //interaccionan con game y hud
    public int skillElegida;
    public Vector2 posicionObjetivo;
    public Skill[] skillsAuxiliar;
    public Skill skillAux = null;      //se usa para la ventana emergente que muestra la descripcion de la habilidad
    public char estadoSkillDisplay;
    public Color filtro;
    public bool noMorir = false;
    //private SkillPassive[] skillPassive;
    public float modificadorDef2 = 1f;
    private bool muerto = false;
    public Jugador() : base()
	{
		//constructor default
	}

	public Jugador(Texture2D spr, int posX, int posY, int nivel) : base(spr, posX, posY, nivel, new Vector2(0, 1))	//llama al constructor base
	{
        _tipo = TIPO.JUGADOR;
		_nombre = "Metalher";
		_skills = new Skill[5];
		_skillsPasivas = new Skill[2];
        _skillsPasivas[0] = _skillsPasivas[1] = null;
        _skills[0] = _skills[1] = _skills[2] = _skills[3] = _skills[4] = null;
        _microPos = new Vector2();
		_inventario = new Inventario(refGame);
		_velocidadBase = 5;
		_exp = 0;
		this._nivel = nivel;
		AjustarNivel();
		_hp = _hpMax;
		_tiempoUltimoGolpeRecibido = 0;
		_dmgRecibidoMensaje = new List<Vector2>();
		_curacionPasiva = new SkillCuracionAll();
		_animacionPasandoDeNivel = false;
		_timerPasandoDeNivel = 0;
        _flickAnimacionSubirNivel = false;
        _ultimoFlickAnimacionSubirNivel = 0f;
        filtro = Color.white;

        //instanciar skills en un array auxiliar, todavia no estan habilitadas para ser usadas
        skillElegida = 0;
        skillsAuxiliar = new Skill[11];

        SkillT1Slash sT1aux = new SkillT1Slash();
        sT1aux.effectSkill = new Texture2D[4];
        sT1aux.effectSkill[0] = refControl.skills[0];
        sT1aux.effectSkill[1] = refControl.skills[1];
        sT1aux.effectSkill[2] = refControl.skills[2];
        sT1aux.effectSkill[3] = refControl.skills[3];
        sT1aux.refIcono = refControl.otrasTexturas[9];
        NuevaSkill(sT1aux);
        skillsAuxiliar[0] = sT1aux;

            SkillT2Twist sT2aux = new SkillT2Twist();
            sT2aux.effectSkill = new Texture2D[5];
            sT2aux.effectSkill[0] = refControl.skills[4];
            sT2aux.effectSkill[1] = refControl.skills[5];
            sT2aux.effectSkill[2] = refControl.skills[6];
            sT2aux.effectSkill[3] = refControl.skills[7];
            sT2aux.effectSkill[4] = refControl.skills[8];
            sT2aux.refIcono = refControl.otrasTexturas[10];

            SkillT2LongSlash sT2aaux = new SkillT2LongSlash();
            sT2aaux.effectSkill = new Texture2D[5];
            sT2aaux.effectSkill[0] = refControl.skills[10];
            sT2aaux.effectSkill[1] = refControl.skills[11];
            sT2aaux.effectSkill[2] = refControl.skills[12];
            sT2aaux.effectSkill[3] = refControl.skills[13];
            sT2aaux.effectSkill[4] = refControl.skills[14];
            sT2aaux.refIcono = refControl.otrasTexturas[24];

            SkillT3Dmg sT3aux = new SkillT3Dmg();
            sT3aux.effectSkill = new Texture2D[1];
            sT3aux.effectSkill[0] = refControl.skills[9];
            sT3aux.refIcono = refControl.otrasTexturas[14];

            SkillT3Def sT3aaux = new SkillT3Def();
            sT3aaux.effectSkill = new Texture2D[1];
            sT3aaux.effectSkill[0] = refControl.skills[15];
            sT3aaux.refIcono = refControl.otrasTexturas[21];

            SkillT4Reflect sT4aux = new SkillT4Reflect();
            sT4aux.effectSkill = new Texture2D[1];
            sT4aux.effectSkill[0] = refControl.skills[18];
            sT4aux.refIcono = refControl.otrasTexturas[22];

            SkillT4Vamp sT4aaux = new SkillT4Vamp();
            sT4aaux.effectSkill = new Texture2D[1];
            sT4aaux.effectSkill[0] = refControl.skills[19];
            sT4aaux.refIcono = refControl.otrasTexturas[20];

            SkillT5TSwords sT5aux = new SkillT5TSwords();
            sT5aux.effectSkill = new Texture2D[1];
            sT5aux.effectSkill[0] = refControl.skills[17];
            sT5aux.refIcono = refControl.otrasTexturas[23];

            SkillT5Fury sT5aaux = new SkillT5Fury();
            sT5aaux.effectSkill = new Texture2D[1];
            sT5aaux.effectSkill[0] = refControl.skills[16];
            sT5aaux.refIcono = refControl.otrasTexturas[19];

            PasivaT1 PT1aux = new PasivaT1();
            PT1aux.refIcono = refControl.otrasTexturas[18];
            PasivaT2 PT2aux = new PasivaT2();
            PT2aux.refIcono = refControl.otrasTexturas[17];

            skillsAuxiliar[1] = sT2aux;
            skillsAuxiliar[2] = sT2aaux;
            skillsAuxiliar[3] = sT3aux;
            skillsAuxiliar[4] = sT3aaux;
            skillsAuxiliar[5] = sT4aux;
            skillsAuxiliar[6] = sT4aaux;
            skillsAuxiliar[7] = sT5aux;
            skillsAuxiliar[8] = sT5aaux;
            skillsAuxiliar[9] = PT1aux;
            skillsAuxiliar[10] = PT2aux;
    }

	public override void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
	{

        GUI.color = filtro;
		GUI.BeginGroup (new Rect (Screen.width / 2 - CONFIG.TAM/2, Screen.height / 2 - CONFIG.TAM/2, CONFIG.TAM, CONFIG.TAM));
		GUI.DrawTexture (new Rect (- _sprActual.x * CONFIG.escala, - _sprActual.y * CONFIG.escala, 832 * CONFIG.escala, 1344 * CONFIG.escala), _sprite);
		GUI.EndGroup ();

		GUI.color = Color.white;

		for (int i = 0; refGame.hud.state == Hud.estado.normal && i < 5; i++)
		{
			if (_skills[i] != null)
			{
				_skills[i].Draw(refGame);
			}
		}
		
	}

	public override void PostDraw (Vector2 posPlayer, Vector2 microPosPlayer)
	{
        if (_animacionPasandoDeNivel)
        {
            float dt = Game.TiempoTranscurrido - _timerPasandoDeNivel;
            if (dt < 2f)
            {
                if (Game.TiempoTranscurrido - _ultimoFlickAnimacionSubirNivel > 0.2f)
                {
                    _ultimoFlickAnimacionSubirNivel = Game.TiempoTranscurrido;
                    if (_flickAnimacionSubirNivel)
                    {
                        _flickAnimacionSubirNivel = false;
                    }
                    else
                    {
                        _flickAnimacionSubirNivel = true;
                    }
                }
                if (_flickAnimacionSubirNivel)
                {
                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                }
                else
                {
                    GUI.color = new Color(1f, 1f, 1f, 0f);
                }


                //GUI.color = new Color(1f, 1f, 1f - dt*2);
                GUI.DrawTexture(new Rect(Screen.width / 2 - CONFIG.TAM / 2, Screen.height / 2 - CONFIG.TAM / 2, CONFIG.TAM, CONFIG.TAM), refControl.otrasTexturas[38]);
                GUI.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                _animacionPasandoDeNivel = false;
            }

        }


        GUIStyle estiloDmg = new GUIStyle();
		estiloDmg.normal.textColor = Color.white;
		estiloDmg.fontSize = UTIL.TextoProporcion(24);
		estiloDmg.alignment = TextAnchor.UpperCenter;

		for (int i = 0; i < _dmgRecibidoMensaje.Count; i++)
		{
			if ((Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y) > 1.5f)
			{
				_dmgRecibidoMensaje.RemoveAt(i);
				i--;
				continue;
			}
			//cambiar color en cada iteracion (alpha), dist entre elementos, y que se desplazen hacia arriba
			estiloDmg.normal.textColor = new Color(1.0f, 0.0f, 0.0f, 1.0f - 1.0f * (Game.TiempoTranscurrido - _dmgRecibidoMensaje[i].y)/1.5f);
			GUI.Label(new Rect(0, Screen.height/2 - (float)CONFIG.TAM * (((Game.TiempoTranscurrido -  _dmgRecibidoMensaje[i].y)/1.5f) * 1.0f), Screen.width, 50), (_dmgRecibidoMensaje[i].x!=0?"-"+_dmgRecibidoMensaje[i].x:CONFIG.getTexto(157)), estiloDmg);
		}
	}

	public override void Actualizar ()
	{
        if (muerto && _state != estado.muerto && _state != estado.miss)
        {
            _state = estado.muerto;
        }

		base.Actualizar ();

        if (_state == estado.miss)
        {
            int xp, oo;

            if (_nivel == 99)
            {
                xp = 0;
            }
            else
            {
                if (CONFIG.MODO_EXTREMO)
                {
                    xp = (int)(_expMax * 1f);
                    _exp -= xp;
                    if (_exp < 0)
                    {
                        xp += _exp;
                        _exp = 0;
                    }

                }
                else
                {
                    xp = (int)(_expMax * 0.2f);
                    _exp -= xp;
                    if (_exp < 0)
                    {
                        xp += _exp;
                        _exp = 0;
                    }
                }
            }
            
            if (CONFIG.MODO_EXTREMO)
            {
                oo = (int)(inventario.oro * 0.5f);
                inventario.oro -= oo;
                if (inventario.oro < 0)
                {
                    inventario.oro = 0;
                }
            }
            else
            {
                oo = (int)(inventario.oro * 0.1f);
                inventario.oro -= oo;
                if (inventario.oro < 0)
                {
                    inventario.oro = 0;
                }
            }

            
            refGame.Respawn();
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(142) + xp + CONFIG.getTexto(143) + oo, false);
            //perdiste bla bla bla
            _state = estado.idle;
            muerto = false;
            _hp = _hpMax;
        }
		/*if (_inventario.equipadoCasco != null)
		{
			if (!_usandoImagenConCasco)
			{
				_sprite = refGame.refControl.sprites[1];
				_usandoImagenConCasco = true;
			}
		}
		else
		{
			if (_usandoImagenConCasco)
			{
				_sprite = refGame.refControl.sprites[0];
				_usandoImagenConCasco = false;
			}
		}*/

		for (int i = 0; i < 2; i++)
		{
			if (_skillsPasivas[i] != null)
			{
				_skillsPasivas[i].Accion(0, 0, refGame);
			}
		}
		_curacionPasiva.Accion(0, 0, refGame);
	}

	protected override void AjustarNivel ()
	{
		if (_nivel <= 0)
		{
			if (CONFIG.DEBUG) Debug.LogWarning("NIVEL JUGADOR MENOR A 1, CORREGIDO A 1");
			_nivel = 1;
		}
        
        //aux 
        /* double expAux, expDropAux;
         int cant = 0, cantTot = 0;
         for (int ii = 1; ii < 100; ii++)
         {
             expAux = 200.0 + 50.0 * (ii + 1) * (ii+ 1);
             expDropAux = 60.0 * Math.Pow(ii, 1.25);
             cant = (int)(expAux / expDropAux);
             cantTot += cant;
             Debug.Log("L" + ii + " cant parcial: " + cant);
         }
         Debug.Log("cant FINAL: " + cantTot);
         */

        //formula del nivel
        _hpMax = (int)(240.0 + 40.0 * Math.Pow(_nivel, 1.5));
		_hp = _hpMax;	//esto por ahora lo dejo asi, pero repensar
		_dmgMax = (int)(120.0 + 7.0 * Math.Pow(_nivel, 1.5));
		_dmgMin = (int)(70.0 + 5.0 * Math.Pow(_nivel, 1.5));
		if (_nivel == 1)
			_expMax = 300;	//si es nivel 1, sino sigue la formula
		else
			_expMax = 200 + 50 * (_nivel + 1) * (_nivel + 1);
        if (_nivel == 99)
        {
            _expMax = 0;
        }

        //refGame.hud.comprobarHabilidadSinAprender();
    }

	protected override void ActualizarAnimSkill ()
	{
		float tiempoSkill = 0.05f;	//por ahora hardcodeado pero tiene que variar para cada skill
		if (_tiempoInicioAtaque + tiempoSkill * (_faseSkill + 1) <= Game.TiempoTranscurrido)
		{
			_faseSkill++;
			if (_faseSkill > 3)
			{
				_state = estado.idle;
			}
		}
	}

	public bool MoverPersonaje()	//esto se llama en cada iteracion (mientras se mueva) desde game
	{
        if (muerto)
            return false;
        Vector2 aux = new Vector2(_pos.x, _pos.y);
        if (MoverAPos(posicionObjetivo, (int)(velocidad * VelMovimiento / 100f)))
        {
            CambiarEstado(EntidadCombate.estado.idle);
            refControl.StopPasos();
        }
            
        bool cambiandoMapa = false;
        try
        {
            cambiandoMapa = refGame.CambioDeMapa();
        }
        catch(NullReferenceException ex)
        {
            Debug.Log(ex.Message);
            _pos = aux;
        }
        if (cambiandoMapa)
		{
            refControl.StopPasos();
            CambiarEstado(EntidadCombate.estado.idle);
            return true;
		}
		return false;
	}

	public bool SkillAttack()
	{
        if (muerto)
            return false;
        int nroSkill = skillElegida - 1;
        //comprobar que el skill existe
        if (nroSkill > 4 || !existeSkill(nroSkill))
			return false;

		if (_skills[nroSkill].enCooldown)
			return false;

		_skills[nroSkill].Accion(getDmgMin(), getDmgMax(), refGame);
		return true;
	}

	public bool existeSkill(int nro)	//de 0 a 4 OJO
	{
		if (nro < 0 || nro > 4 || _skills[nro] == null)
			return false;
		return true;
	}

	public bool NuevaSkill(Skill s)
	{
		if (s.pasiva == true)
			return false;
		for (int i = 0; i < s.Tier - 1; i++ )	//primero tiene que ver que las habilidades de tier anterior no esten vacias
		{
			if (_skills[i] == null)
				return false;
		}
		//despues tiene que ver que en ese tier no haya ninguna habilidad puesta
		if (_skills[s.Tier - 1] != null)
			return false;

		_skills[s.Tier - 1] = s;
		return true;
	}

	public bool NuevaPasiva(Skill s)
	{
		if (!s.pasiva)
			return false;
		
		_skillsPasivas[s.Tier - 1] = s;

        if (s.Tier == 2)
        {
            auraPoderActivada = true;
        }

		return true;
	}

	//getters

    public Skill getSkill(int nro)
	{
		if (nro > 4)
			return null;
		return _skills[nro];
	}
    public Skill getSkillPorCodigo(int nro)
    {
        for (int i = 0; i < _skills.Length; i++)
        {
            if (_skills[i] != null && _skills[i].getCodigo() == nro)
                return _skills[i];
        }
        return null;
    }


    public Skill getPasiva(int nro)
	{
		if (nro > 2)
			return null;
		return _skillsPasivas[nro];
	}

	private void PasarDeNivel()
	{
        if (_nivel > 98)
            return;
        int lvlAnt = _nivel;
		if (_exp < _expMax)
			return;
        while (_exp >= _expMax)
        {
            _exp -= _expMax;
            _nivel++;
        }

        refControl.PlaySonido(5);
		//int hpAux = hpMax;

		AjustarNivel();

		//hp = (int)(hp * (1f - hpAux/(float)hpMax)) ;
		_timerPasandoDeNivel = Game.TiempoTranscurrido;
		_animacionPasandoDeNivel = true;

        if (lvlAnt < 5 && _nivel > 4)   //si paso a nivel 5 (lo puse asi por si pasa de nivel 4 a 6)
        {
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(144));
        }

        refGame.hud.comprobarHabilidadSinAprender();
	}

	public bool esEquipable(Item it)
	{
        if (it == null || it.Tipo == Item.TipoItem.Otro || it.Tipo == Item.TipoItem.Oro || it.lvl > _nivel)
            return false;
		return true;
	}

	public void ganarVida(int h)
	{
		_hp += h;
		if (_hp > _hpMax)
			_hp = _hpMax;
	}

	public override int getDmgMin()
	{
		float aumento = (_inventario.AumentoDmg()/100.0f) + 1.0f;
		return (int)(_dmgMin * _modificadorDmg * aumento * ((auraPoderActivada == true) ? (1.3f) : (1f)));
	}
	
	public override int getDmgMax()
	{
		float aumento = (_inventario.AumentoDmg()/100.0f) + 1.0f;
		return (int)(_dmgMax * _modificadorDmg * aumento * ((auraPoderActivada == true) ? (1.3f) : (1f)));
	}

	public float VelMovimiento //BASE + ITEMS
	{
		get
		{
			return 100f + _inventario.getTotalAtributoEquipado(Item.ATRIBUTO.VEL_MOV) + ((auraPoderActivada == true) ? (20f) : (0f));
		}
	}

	public int Armadura 
	{
		get
		{
			return (int)(_inventario.getTotalAtributoEquipado(Item.ATRIBUTO.ARMADURA) * _modificadorDef * modificadorDef2 * ((auraPoderActivada == true)?(1.25f):(1f)));
		}
	}

	public float ReduccionDmg 
	{
		get
		{
			return ((Mathf.Log((Armadura + 1)) * 8f) ); //formula de la armadura y reduccion
		}
	}

	public float ReduccionCD //BASE + ITEMS
	{
		get
		{
			return _inventario.getTotalAtributoEquipado(Item.ATRIBUTO.CD_REDUCCION);
		}
	}

	public float GolpeCritico	//BASE + ITEMS
	{
		get
		{
			return 10f + _inventario.getTotalAtributoEquipado(Item.ATRIBUTO.GOLPE_CRITICO);
		}
	}

	public float Esquivar //BASE + ITEMS
	{
		get
		{
			return 10f + _inventario.getTotalAtributoEquipado(Item.ATRIBUTO.ESQUIVAR);
		}
	}

	public bool esGCritico
	{
		get
		{
			return (UnityEngine.Random.Range(0, 101) <= GolpeCritico);
		}
	}

	public bool esquivaGolpe
	{
		get
		{
			return (UnityEngine.Random.Range(0, 101) <= Esquivar);
		}
	}

	public override int RecibirDmg (int dmg, bool critico)
	{
		/*if (CONFIG.DEBUG)
			return 0;*/

		int dmgoutput = 0;

		if (!esquivaGolpe)
		{

            refGame.refControl.PlaySonido(10);
            dmgoutput = (int)(dmg * (1f - ReduccionDmg/100f));
			_hp -= dmgoutput;
			_tiempoUltimoGolpeRecibido = Game.TiempoTranscurrido;

            if (noMorir && _hp < 1)
            {
                _hp = 1;
            }

			if (_hp <= 0)
			{
				dmgoutput -= _hp;
				_hp = 0;
                //muerto
                refControl.StopEfectos();
                refControl.StopPasos();

                //refControl.PlaySonido(6);
                GameObject.Find("Morir").GetComponent<AudioSource>().Play();
                _state = estado.muerto;
                muerto = true;
                for (int i = 0; i < 5; i++)
                {
                    if (existeSkill(i))
                    {
                        _skills[i].tiempoUltimoActivo = 0f;
                        _skills[i].enabled = false;
                    }
                }

                
                
			}
		}
		_dmgRecibidoMensaje.Add(new Vector2(dmgoutput, Game.TiempoTranscurrido));
		return dmgoutput;
	}

    public void setStatsCargar(int lvl, int hp, int xp)
    {
        _nivel = lvl;
        AjustarNivel();
        _hp = hp;
        _exp = xp;
    }

    public override void CambiarEstado(estado ee)
    {
        if (ee != estado.caminando)
        {
            refControl.StopPasos();
        }

        base.CambiarEstado(ee);
    }
}
