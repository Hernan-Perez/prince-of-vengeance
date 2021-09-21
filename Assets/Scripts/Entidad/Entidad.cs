using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Entidad
{
    protected static ControlScript refControl;
    protected static Game refGame;
    public enum TIPO { NO_DEF, JUGADOR, ENEMIGO, NPC };
    public enum direccionEnum { abajo, izquierda, arriba, derecha };

    protected TIPO _tipo;
    protected direccionEnum _dirEnum;
    protected Vector2[] _idle, _atacar, _caminar, _morir;
    protected Vector2 _sprActual;
    protected Texture2D _sprite;
    public Texture2D sprite
    {
        get
        {
            return _sprite;
        }
    }
    protected Vector2 _pos;
    public Vector2 pos
    {
        set
        {
            _pos = value;
        }
        get
        {
            return _pos;
        }
    }
    protected Vector2 _microPos;    //ahora va de 0 a 99. ES EN PORCENTAJE, asi no depende del zoom
    public Vector2 microPos
    {
        set
        {
            _microPos = value;
        }
        get
        {
            return _microPos;
        }
    }
    public Vector2 microPosAbsoluta
    {
        get
        {
            return new Vector2((_microPos.x * CONFIG.TAM) / 100, (_microPos.y * CONFIG.TAM) / 100);
        }
    }
    protected Vector2 _direccionVect;
    protected float _tiempoUltimaAnim;
    protected int _fase, _faseCaminarMax;
    protected int _velocidadBase;
    protected float _modificadorVelocidad;

    protected int velocidad
    {
        get
        {
            if (Time.deltaTime > 0.3f)
                return 0;
            return (int)(_velocidadBase * _modificadorVelocidad * 100 * Time.deltaTime);
            /*CONFIG.TAM = 64 seria 100%*/
        }
    }

    /// <summary>
    /// VACIO
    /// </summary>
    public Entidad()
    {
        //constructor default
    }

    public Entidad(Texture2D spr, int posX, int posY, Vector2 mirandoHacia)
    {
        _tipo = TIPO.NO_DEF;
        _modificadorVelocidad = 1.0f;
        _pos = new Vector2((float)posX, (float)posY);
        _microPos = new Vector2();
        _direccionVect = mirandoHacia;
        _sprite = spr;
        AjustarCoordenadasSheet(0);
        _dirEnum = direccionEnum.abajo;
        _sprActual = _idle[0];
        _tiempoUltimaAnim = 0;
        _fase = 0;
        _faseCaminarMax = 7;
    }

    /// <summary>
    /// Esto es para setear la referencia estatica a Game, se tiene que llamar 1 sola vez, en el init de Game
    /// </summary>
    public static void setRefGame(Game reff)
    {
        refGame = reff;
    }

    public static void setRefControl(ControlScript reff)
    {
        refControl = reff;
    }

    public virtual void Draw(Vector2 posPlayer, Vector2 microPosPlayer)
	{
		GUI.BeginGroup (new Rect (Screen.width/2  - CONFIG.TAM/2 + (+ _pos.x - posPlayer.x) * CONFIG.TAM - microPosPlayer.x + microPosAbsoluta.x, Screen.height/2 - CONFIG.TAM/2 + (- _pos.y + posPlayer.y) * CONFIG.TAM + microPosPlayer.y - microPosAbsoluta.y, CONFIG.TAM, CONFIG.TAM));
		GUI.DrawTexture (new Rect (- _sprActual.x * CONFIG.escala, - _sprActual.y * CONFIG.escala, 832 * CONFIG.escala, 1344 * CONFIG.escala), _sprite);
		GUI.EndGroup ();
	}

    /// <summary>
    /// VACIO
    /// </summary>
	public virtual void PostDraw(Vector2 posPlayer, Vector2 microPosPlayer)
	{

	}

    public virtual void EjecutarAccionAI()
    {

    }

    protected virtual bool MoverAPos(Vector2 posTarget, int velocidad)
    {
        float dirX, dirY;
        dirX = dirY = 0;
        bool colisionX = false, colisionY = false;

        if (!(pos.x == posTarget.x && microPos.x == 0) && !(pos.y == posTarget.y && microPos.y == 0))
        {
            velocidad = (int)(velocidad * 0.9f);    //velocidad diagonal
        }

        if (_pos.x < posTarget.x)
        {
            dirX = velocidad;
            if (refGame.DetectarColision(this, new Vector2(1, 0))
                || (_microPos.y > 0 && refGame.DetectarColision(this, new Vector2(1, 1))))
            {
                _microPos.x = 0;
                colisionX = true;
            }
            else
            {
                _microPos.x += velocidad;
                if (_microPos.x > 99)
                {
                    _pos.x++;
                    if (_pos.x == posTarget.x
                        || refGame.DetectarColision(this, new Vector2(1, 0))
                        || (_microPos.y > 0 && refGame.DetectarColision(this, new Vector2(1, 1))))
                    {
                        _microPos.x = 0;
                    }
                    else
                    {
                        _microPos.x -= 100;
                    }
                }
            }
        }
        else if (_pos.x > posTarget.x || (_pos.x == posTarget.x && _microPos.x > 0))   //si la pos es mayor, o igual PERO con la microPos defasada/// la idea es que quede en misma pos con micropos = 0
        {
            _microPos.x -= velocidad;
            dirX = -velocidad;

            if (_microPos.x < 0)
            {
                if (refGame.DetectarColision(this, new Vector2(-1, 0)) || (_pos.x == posTarget.x)
                    || (_microPos.y > 0 && refGame.DetectarColision(this, new Vector2(-1, 1))))
                {
                    colisionX = true;
                    _microPos.x = 0;
                }
                else
                {
                    _microPos.x += 100;
                    _pos.x--;
                }
            }
        }

        if (_pos.y < posTarget.y)
        {
            dirY = velocidad;
            if (refGame.DetectarColision(this, new Vector2(0, 1))
                || (_microPos.x > 0 && refGame.DetectarColision(this, new Vector2(1, 1))))
            {
                _microPos.y = 0;
                colisionY = true;
            }
            else
            {
                _microPos.y += velocidad;
                if (_microPos.y > 99)
                {
                    _pos.y++;
                    if (_pos.y == posTarget.y
                        || refGame.DetectarColision(this, new Vector2(0, 1))
                        || (_microPos.x > 0 && refGame.DetectarColision(this, new Vector2(1, 1))))
                    {
                        _microPos.y = 0;
                    }
                    else
                    {
                        _microPos.y -= 100;
                    }
                }
            }
        }
        else if (_pos.y > posTarget.y || (_pos.y == posTarget.y && _microPos.y > 0))   //si la pos es mayor, o igual PERO con la microPos defasada/// la idea es que quede en misma pos con micropos = 0
        {
            _microPos.y -= velocidad;
            dirY = -velocidad;

            if (_microPos.y < 0)
            {
                if (refGame.DetectarColision(this, new Vector2(0, -1)) || (_pos.y == posTarget.y)
                    || (_microPos.x > 0 && refGame.DetectarColision(this, new Vector2(1, -1))))
                {
                    colisionY = true;
                    _microPos.y = 0;
                }
                else
                {
                    _microPos.y += 100;
                    _pos.y--;
                }
            }
        }

        _direccionVect = new Vector2(dirX, dirY);
        if (_direccionVect != Vector2.zero)
        {
            _direccionVect.Normalize();
        }


        if (((_pos.x == posTarget.x && _microPos.x == 0) || colisionX) && ((_pos.y == posTarget.y && _microPos.y == 0) || colisionY))
        {
            _microPos = Vector2.zero;
            return true;    //llego a destino, o colisiono
        }
        else
        {

            return false; //todavia no llego a posicion target
        }
    }

    /// <summary>
    /// VACIO
    /// </summary>
	public virtual void Actualizar()
	{	

	}
	
	protected virtual void AjustarCoordenadasSheet(int preset)
	{
		_idle = new Vector2[4];	//para cada direccion
		_idle [0] = new Vector2 (0, 640);
		_idle [1] = new Vector2 (0, 576);
		_idle [2] = new Vector2 (0, 512);
		_idle [3] = new Vector2 (0, 704);
		_caminar = new Vector2[32];
		_caminar [00] = new Vector2 (64, 640); _caminar [01] = new Vector2 (64, 640); _caminar [02] = new Vector2 (128, 640); _caminar [03] = new Vector2 (192, 640); _caminar [04] = new Vector2 (256, 640); _caminar [05] = new Vector2 (320, 640); _caminar [06] = new Vector2 (384, 640); _caminar [07] = new Vector2 (448, 640);
		_caminar [08] = new Vector2 (64, 576); _caminar [09] = new Vector2 (64, 576); _caminar [10] = new Vector2 (128, 576); _caminar [11] = new Vector2 (192, 576); _caminar [12] = new Vector2 (256, 576); _caminar [13] = new Vector2 (320, 576); _caminar [14] = new Vector2 (384, 576); _caminar [15] = new Vector2 (448, 576);
		_caminar [16] = new Vector2 (64, 512); _caminar [17] = new Vector2 (64, 512); _caminar [18] = new Vector2 (128, 512); _caminar [19] = new Vector2 (192, 512); _caminar [20] = new Vector2 (256, 512); _caminar [21] = new Vector2 (320, 512); _caminar [22] = new Vector2 (384, 512); _caminar [23] = new Vector2 (448, 512);
		_caminar [24] = new Vector2 (64, 704); _caminar [25] = new Vector2 (64, 704); _caminar [26] = new Vector2 (128, 704); _caminar [27] = new Vector2 (192, 704); _caminar [28] = new Vector2 (256, 704); _caminar [29] = new Vector2 (320, 704); _caminar [30] = new Vector2 (384, 704); _caminar [31] = new Vector2 (448, 704);
		_atacar = new Vector2[24];
		_atacar [00] = new Vector2 (0, 768); _atacar [01] = new Vector2 (64, 768); _atacar [02] = new Vector2 (128, 768); _atacar [03] = new Vector2 (192, 768); _atacar [04] = new Vector2 (256, 768); _atacar [05] = new Vector2 (320, 768);
		_atacar [06] = new Vector2 (0, 832); _atacar [07] = new Vector2 (64, 832); _atacar [08] = new Vector2 (128, 832); _atacar [09] = new Vector2 (192, 832); _atacar [10] = new Vector2 (256, 832); _atacar [11] = new Vector2 (320, 832);
		_atacar [12] = new Vector2 (0, 896); _atacar [13] = new Vector2 (64, 896); _atacar [14] = new Vector2 (128, 896); _atacar [15] = new Vector2 (192, 896); _atacar [16] = new Vector2 (256, 896); _atacar [17] = new Vector2 (320, 896);
		_atacar [18] = new Vector2 (0, 960); _atacar [19] = new Vector2 (64, 960); _atacar [20] = new Vector2 (128, 960); _atacar [21] = new Vector2 (192, 960); _atacar [22] = new Vector2 (256, 960); _atacar [23] = new Vector2 (320, 960);
		_morir = new Vector2[6];
		_morir [00] = new Vector2 (0, 1280); _morir [01] = new Vector2 (64, 1280); _morir [02] = new Vector2 (128, 1280); _morir [03] = new Vector2 (192, 1280); _morir [04] = new Vector2 (256, 1280); _morir [05] = new Vector2 (320, 1280);
	}
	
	public virtual Vector2 getCoordenadasPixeles()	//las coordenadas pixeles son la cantidad de pixeles, dependen de la escala con la que se trabaje
	{
		return (new Vector2(_pos.x * CONFIG.TAM + _microPos.x, _pos.y * CONFIG.TAM + _microPos.y));
	}

	public Vector2 directionVector
	{
		set {this._direccionVect = value;}
		get {return this._direccionVect;}
	}

    public Rect getSuperficie()
    {
        return new Rect(_pos.x, _pos.y, (_microPos.x > 0) ? (2f) : (1f), (_microPos.y > 0) ? (2f) : (1f));
    }

    public TIPO getTipo()
    {
        return _tipo;
    }
}

