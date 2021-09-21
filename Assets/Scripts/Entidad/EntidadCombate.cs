using System;
using UnityEngine;

public abstract class EntidadCombate : Entidad {

	public enum estado {idle, caminando, atacando, muerto, muerto2, miss};   //miss es despues de morir, el plazo en el que todavia no hace respawn

    protected estado _state;
	protected int _hpMax, _hp;
	protected int _dmgMin, _dmgMax;
	protected int _nivel;
	protected float _tiempoInicioAtaque;	//mientras ataca no se puede mover
	protected int _faseSkill;   //esto no lo usan los enemigos comunes, pero lo dejo aca por los bosses que podrian tener skills
    protected int _faseAtkMax, _faseMorirMax;
    protected float _morirTransparencia;
    protected float _dragListaDrop;

    protected float _modificadorDmg;
    public float modificadorDmg
    {
        set
        {
            _modificadorDmg = value;
        }
        get
        {
            return _modificadorDmg;
        }
    }
	protected float _modificadorDef;
    public float modificadorDef
    {
        set
        {
            _modificadorDef = value;
        }
        get
        {
            return _modificadorDef;
        }
    }
    protected bool noDesaparecer = false;   //parche para jefes como ruk y nhar, para que cuando muere uno no desaparezca

    public EntidadCombate()
	{
		//default constructor
	}

	public EntidadCombate(Texture2D spr, int posX, int posY, int nivel, Vector2 mirandoHacia) : base(spr, posX, posY, new Vector2(0, -1))
	{
		this._nivel = nivel;
		_modificadorDmg = 1.0f;	//100%
		_modificadorDef = 1.0f;	//100%
        _fase = 0;
        _faseCaminarMax = 7;
        _faseAtkMax = 7;
        _faseMorirMax = 5;
        _state = estado.idle;
        AjustarNivel();
	}

    public override void Actualizar()
	{
		if (_state == estado.miss)
			return;

		while (_microPos.x < 0) 
		{
			_microPos.x = 100 + _microPos.x;
			_pos.x--;
		}
		
		while (_microPos.x >= 100) 
		{
			_microPos.x -= 100;
			_pos.x++;
		}
		
		while (_microPos.y < 0) 
		{
			_microPos.y = 100 + _microPos.y;
			_pos.y--;
		}
		
		while (_microPos.y >= 100) 
		{
			_microPos.y -= 100;
			_pos.y++;
		}
		
		if (_direccionVect.x == 0 && _direccionVect.y == 0)
			_direccionVect = new Vector2 (0, -1);
		
		Vector2 v1 = new Vector2 (1, 0);
		float ang = Vector2.Angle (v1, _direccionVect);
		Vector3 cross = Vector3.Cross (v1, _direccionVect);
		if (cross.z < 0)
			ang = 360 - ang;
		
		
		if ((ang >= 315) || (ang < 45))
			_dirEnum = direccionEnum.derecha;
		if ((ang >= 45) && (ang < 135))
			_dirEnum = direccionEnum.arriba;
		if ((ang >= 135) && (ang < 225))
			_dirEnum = direccionEnum.izquierda;
		if ((ang >= 225) && (ang < 315))
			_dirEnum = direccionEnum.abajo;
		
		
		if (_state == estado.idle) 
		{
			if (_dirEnum == direccionEnum.abajo)
				_sprActual = _idle[0];
			if (_dirEnum == direccionEnum.izquierda)
				_sprActual = _idle[1];
			if (_dirEnum == direccionEnum.arriba)
				_sprActual = _idle[2];
			if (_dirEnum == direccionEnum.derecha)
				_sprActual = _idle[3];
		}
        int offset = 0;

		if (_state == estado.caminando)	//AJUSTA ANIMACION DE CAMINAR
		{
			if (Game.TiempoTranscurrido - _tiempoUltimaAnim > CONFIG.TiempoAnimCaminar)
			{
				_tiempoUltimaAnim = Game.TiempoTranscurrido;
                switch (_dirEnum)
                {
                    case direccionEnum.abajo:
                        offset = 0;
                        break;
                    case direccionEnum.arriba:
                        offset = (_faseCaminarMax + 1) * 2;
                        break;
                    case direccionEnum.izquierda:
                        offset = (_faseCaminarMax + 1) * 1;
                        break;
                    case direccionEnum.derecha:
                        offset = (_faseCaminarMax + 1) * 3;
                        break;
                }

                _fase++;
                if (_fase > _faseCaminarMax)
                {
                    _fase = 0;
                }
    
                _sprActual = _caminar[_fase + offset];
			}
		}
		if (_state == estado.atacando)
		{
			//ActualizarAnimSkill();
			if (Game.TiempoTranscurrido - _tiempoUltimaAnim > CONFIG.TiempoAnimAtacar)
			{
				_tiempoUltimaAnim = Game.TiempoTranscurrido;

                switch (_dirEnum)
                {
                    case direccionEnum.abajo:
                        offset = (_faseAtkMax + 1) * 2;
                        break;
                    case direccionEnum.arriba:
                        offset = 0;
                        break;
                    case direccionEnum.izquierda:
                        offset = _faseAtkMax + 1;
                        break;
                    case direccionEnum.derecha:
                        offset = (_faseAtkMax + 1) * 3;
                        break;
                }

                _fase++;
                if (_fase > _faseAtkMax)
                {
                    _fase = 0;
                    CambiarEstado(estado.idle);
                }
                try
                {
                    _sprActual = _atacar[_fase + offset];
                }
                catch(IndexOutOfRangeException ex)
                {
                    Debug.Log(ex.Message + " " + _atacar.Length);
                    Debug.Log(_faseAtkMax + " " + _fase + " " + offset);
                }
			}

		}

        if (_state == estado.muerto)
        {
            if (Game.TiempoTranscurrido - _tiempoUltimaAnim > CONFIG.TiempoAnimCaminar)
            {
                _tiempoUltimaAnim = Game.TiempoTranscurrido;

                _fase++;
                if (_fase >= _faseMorirMax)
                {
                    _fase = _faseMorirMax;
                    _morirTransparencia++;
                    if (_morirTransparencia >= 4)
                    {
                        _morirTransparencia = 4;
                    }

                    if (_morirTransparencia >= 4 && (Game.TiempoTranscurrido - _dragListaDrop) >= 2.0f)
                    {
                        _morirTransparencia = 0;
                        _dragListaDrop = 0;
                        if (noDesaparecer)
                            _state = estado.muerto2;
                        else
                            _state = estado.miss;
                    }
                }
                _sprActual = _morir[_fase];
            }
        }
    }

	protected virtual void ActualizarAnimSkill()
	{
		
	}
	
	protected virtual void AjustarNivel()
	{

	}

	public virtual void CambiarEstado(estado ee)
	{
		_state = ee;
		if (_state == estado.atacando)
		{
			_faseSkill = 0;
			_tiempoInicioAtaque = Game.TiempoTranscurrido;
		}
	}

    protected override void AjustarCoordenadasSheet(int preset)
    {
        _idle = new Vector2[4];  //para cada direccion
        _idle[0] = new Vector2(0, 640);
        _idle[1] = new Vector2(0, 576);
        _idle[2] = new Vector2(0, 512);
        _idle[3] = new Vector2(0, 704);
        _caminar = new Vector2[32];
        _caminar[00] = new Vector2(64, 640); _caminar[01] = new Vector2(64, 640); _caminar[02] = new Vector2(128, 640); _caminar[03] = new Vector2(192, 640); _caminar[04] = new Vector2(256, 640); _caminar[05] = new Vector2(320, 640); _caminar[06] = new Vector2(384, 640); _caminar[07] = new Vector2(448, 640);
        _caminar[08] = new Vector2(64, 576); _caminar[09] = new Vector2(64, 576); _caminar[10] = new Vector2(128, 576); _caminar[11] = new Vector2(192, 576); _caminar[12] = new Vector2(256, 576); _caminar[13] = new Vector2(320, 576); _caminar[14] = new Vector2(384, 576); _caminar[15] = new Vector2(448, 576);
        _caminar[16] = new Vector2(64, 512); _caminar[17] = new Vector2(64, 512); _caminar[18] = new Vector2(128, 512); _caminar[19] = new Vector2(192, 512); _caminar[20] = new Vector2(256, 512); _caminar[21] = new Vector2(320, 512); _caminar[22] = new Vector2(384, 512); _caminar[23] = new Vector2(448, 512);
        _caminar[24] = new Vector2(64, 704); _caminar[25] = new Vector2(64, 704); _caminar[26] = new Vector2(128, 704); _caminar[27] = new Vector2(192, 704); _caminar[28] = new Vector2(256, 704); _caminar[29] = new Vector2(320, 704); _caminar[30] = new Vector2(384, 704); _caminar[31] = new Vector2(448, 704);


        switch (preset)
        {
            case 0: //enemigo con lanza
                _atacar = new Vector2[32];
                _atacar[00] = new Vector2(0, 256); _atacar[01] = new Vector2(64, 256); _atacar[02] = new Vector2(128, 256); _atacar[03] = new Vector2(192, 256); _atacar[04] = new Vector2(256, 256); _atacar[05] = new Vector2(320, 256); _atacar[06] = new Vector2(384, 256); _atacar[07] = new Vector2(448, 256);
                _atacar[08] = new Vector2(0, 320); _atacar[09] = new Vector2(64, 320); _atacar[10] = new Vector2(128, 320); _atacar[11] = new Vector2(192, 320); _atacar[12] = new Vector2(256, 320); _atacar[13] = new Vector2(320, 320); _atacar[14] = new Vector2(384, 320); _atacar[15] = new Vector2(448, 320);
                _atacar[16] = new Vector2(0, 384); _atacar[17] = new Vector2(64, 384); _atacar[18] = new Vector2(128, 384); _atacar[19] = new Vector2(192, 384); _atacar[20] = new Vector2(256, 384); _atacar[21] = new Vector2(320, 384); _atacar[22] = new Vector2(384, 384); _atacar[23] = new Vector2(448, 384);
                _atacar[24] = new Vector2(0, 448); _atacar[25] = new Vector2(64, 448); _atacar[26] = new Vector2(128, 448); _atacar[27] = new Vector2(192, 448); _atacar[28] = new Vector2(256, 448); _atacar[29] = new Vector2(320, 448); _atacar[30] = new Vector2(384, 448); _atacar[31] = new Vector2(448, 448);
                _faseAtkMax = 7;
                break;

            case 1: //enemigo con arco
                _atacar = new Vector2[52];
                _atacar[00] = new Vector2(0, 1024); _atacar[01] = new Vector2(64, 1024); _atacar[02] = new Vector2(128, 1024); _atacar[03] = new Vector2(192, 1024); _atacar[04] = new Vector2(256, 1024); _atacar[05] = new Vector2(320, 1024); _atacar[06] = new Vector2(384, 1024); _atacar[07] = new Vector2(448, 1024); _atacar[08] = new Vector2(512, 1024); _atacar[09] = new Vector2(576, 1024); _atacar[10] = new Vector2(640, 1024); _atacar[11] = new Vector2(704, 1024); _atacar[12] = new Vector2(768, 1024);
                _atacar[13] = new Vector2(0, 1088); _atacar[14] = new Vector2(64, 1088); _atacar[15] = new Vector2(128, 1088); _atacar[16] = new Vector2(192, 1088); _atacar[17] = new Vector2(256, 1088); _atacar[18] = new Vector2(320, 1088); _atacar[19] = new Vector2(384, 1088); _atacar[20] = new Vector2(448, 1088); _atacar[21] = new Vector2(512, 1088); _atacar[22] = new Vector2(576, 1088); _atacar[23] = new Vector2(640, 1088); _atacar[24] = new Vector2(704, 1088); _atacar[25] = new Vector2(768, 1088);
                _atacar[26] = new Vector2(0, 1152); _atacar[27] = new Vector2(64, 1152); _atacar[28] = new Vector2(128, 1152); _atacar[29] = new Vector2(192, 1152); _atacar[30] = new Vector2(256, 1152); _atacar[31] = new Vector2(320, 1152); _atacar[32] = new Vector2(384, 1152); _atacar[33] = new Vector2(448, 1152); _atacar[34] = new Vector2(512, 1152); _atacar[35] = new Vector2(576, 1152); _atacar[36] = new Vector2(640, 1152); _atacar[37] = new Vector2(704, 1152); _atacar[38] = new Vector2(768, 1152);
                _atacar[39] = new Vector2(0, 1216); _atacar[40] = new Vector2(64, 1216); _atacar[41] = new Vector2(128, 1216); _atacar[42] = new Vector2(192, 1216); _atacar[43] = new Vector2(256, 1216); _atacar[44] = new Vector2(320, 1216); _atacar[45] = new Vector2(384, 1216); _atacar[46] = new Vector2(448, 1216); _atacar[47] = new Vector2(512, 1216); _atacar[48] = new Vector2(576, 1216); _atacar[49] = new Vector2(640, 1216); _atacar[50] = new Vector2(704, 1216); _atacar[51] = new Vector2(768, 1216);
                _faseAtkMax = 12;
                break;

            case 2: //enemigo con espada
                _atacar = new Vector2[24];
                _atacar[00] = new Vector2(0, 256 + 512); _atacar[01] = new Vector2(64, 256 + 512); _atacar[02] = new Vector2(128, 256 + 512); _atacar[03] = new Vector2(192, 256 + 512); _atacar[04] = new Vector2(256, 256 + 512); _atacar[05] = new Vector2(320, 256 + 512);
                _atacar[06] = new Vector2(0, 320 + 512); _atacar[07] = new Vector2(64, 320 + 512); _atacar[08] = new Vector2(128, 320 + 512); _atacar[09] = new Vector2(192, 320 + 512); _atacar[10] = new Vector2(256, 320 + 512); _atacar[11] = new Vector2(320, 320 + 512);
                _atacar[12] = new Vector2(0, 384 + 512); _atacar[13] = new Vector2(64, 384 + 512); _atacar[14] = new Vector2(128, 384 + 512); _atacar[15] = new Vector2(192, 384 + 512); _atacar[16] = new Vector2(256, 384 + 512); _atacar[17] = new Vector2(320, 384 + 512);
                _atacar[18] = new Vector2(0, 448 + 512); _atacar[19] = new Vector2(64, 448 + 512); _atacar[20] = new Vector2(128, 448 + 512); _atacar[21] = new Vector2(192, 448 + 512); _atacar[22] = new Vector2(256, 448 + 512); _atacar[23] = new Vector2(320, 448 + 512);
                _faseAtkMax = 5;
                break;
            case 3: //enemigo mago (casteo alternativo)
                _atacar = new Vector2[28];
                _atacar[00] = new Vector2(0, 0); _atacar[01] = new Vector2(64, 0); _atacar[02] = new Vector2(128, 0); _atacar[03] = new Vector2(192, 0); _atacar[04] = new Vector2(256, 0); _atacar[05] = new Vector2(320, 0); _atacar[06] = new Vector2(384, 0);
                _atacar[07] = new Vector2(0, 64); _atacar[08] = new Vector2(64, 64); _atacar[09] = new Vector2(128, 64); _atacar[10] = new Vector2(192, 64); _atacar[11] = new Vector2(256, 64); _atacar[12] = new Vector2(320, 64); _atacar[13] = new Vector2(384, 64);
                _atacar[14] = new Vector2(0, 128); _atacar[15] = new Vector2(64, 128); _atacar[16] = new Vector2(128, 128); _atacar[17] = new Vector2(192, 128); _atacar[18] = new Vector2(256, 128); _atacar[19] = new Vector2(320, 128); _atacar[20] = new Vector2(384, 128);
                _atacar[21] = new Vector2(0, 192); _atacar[22] = new Vector2(64, 192); _atacar[23] = new Vector2(128, 192); _atacar[24] = new Vector2(192, 192); _atacar[25] = new Vector2(256, 192); _atacar[26] = new Vector2(320, 192); _atacar[27] = new Vector2(384, 192);
                _faseAtkMax = 6;
                break;
        }

        _morir = new Vector2[6];
        _morir[00] = new Vector2(0, 1280); _morir[01] = new Vector2(64, 1280); _morir[02] = new Vector2(128, 1280); _morir[03] = new Vector2(192, 1280); _morir[04] = new Vector2(256, 1280); _morir[05] = new Vector2(320, 1280);
    }

    public virtual int RecibirDmg(int dmgg, bool critico = false)
    {
        if (_state == estado.miss)
            return 0;
        int dmg = (int)(dmgg * _modificadorDef);
        _hp -= dmg;
        int outputDmg = dmg;
        if (_hp < 0)
        {
            outputDmg += _hp;
            _hp = 0;
            Morir();
        }
        return outputDmg;   //devuelve el dmg que realmente pego. porque si tiene 0 de vida no tiene sentido el daño que pega, para las estadisticas y otras cosas
    }

    protected virtual void Morir()
    {
        _state = estado.muerto;
    }

    //GETTERS
    public virtual int getHp()
	{
		return _hp;
	}   //esta  getHpMax las hago virtuales para los bosses que son multiples, asi la vida total es la de la suma de los enemigos

	public virtual int getHpMax()
	{
		return _hpMax;
	}
	
	public int getNivel()
	{
		return _nivel;
	}
	
	public virtual int getDmgMin()
	{
		return (int)(_dmgMin * _modificadorDmg);
	}
	
	public virtual int getDmgMax()
	{
		return (int)(_dmgMax * _modificadorDmg);
	}

	public virtual int getDmgMinBase()
	{
		return _dmgMin;
	}
	
	public virtual int getDmgMaxBase()
	{
		return _dmgMax;
	}

	public estado Estado
	{
        set
        {
            _state = value;
        }
        get
        {
            return _state;
        }
	}

}
