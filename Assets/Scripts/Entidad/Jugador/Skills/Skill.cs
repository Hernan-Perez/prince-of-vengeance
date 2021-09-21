using UnityEngine;
using System.Collections;

public abstract class Skill
{
	public Texture2D refIcono;
	public Texture2D[] effectSkill;
	protected int currentTexSkill;
	protected float ultTiempoSkill;
	protected float tiempoFase;
	protected float mod1, mod2;		//modificadores de dmg, def o lo que sea, por ejemplo la habilidad pega 50% del daño base. el segundo modificador esta al pedo para la mayoria
	protected int tier;
	public bool enabled;
	public bool persistente;	//si es un buff
	public bool pasiva;
	public float cooldown;
	public float tiempoUltimoActivo;
	protected int codigo;
	protected string _nombre, _descripcion;

	public string Nombre
	{
		get {return _nombre;}
	}

	public string Descripcion
	{
		get {return _descripcion;}
	}

	public bool Comparar(Skill s)
	{
		return (this.codigo == s.codigo);
	}

	public bool enCooldown
	{
		get
		{
			return (Game.TiempoTranscurrido - tiempoUltimoActivo < cooldown);
		}
	}

	public float cooldownRestante
	{
		get
		{
            if (enCooldown)
			    return (cooldown - (Game.TiempoTranscurrido - tiempoUltimoActivo));
            return 0f;
		}
	}

	public Skill()
	{
		pasiva = false;
		currentTexSkill = 0;
		enabled = false;
		ultTiempoSkill = 0;
		persistente = false;
		cooldown = 1.0f;
		tiempoUltimoActivo = -1000;	//COMO DEBUG, PARA QUE NO EMPIEZEN CON COOLDOWN
		codigo = 0;
	}

	public virtual int Accion(int dmgMin, int dmgMax, Game refGame)
	{
		tiempoUltimoActivo = Game.TiempoTranscurrido - cooldown * refGame.player.ReduccionCD/100f;
		return 0;
	}

	public virtual bool Draw(Game refGame)
	{
		return false;
	}

	public void ResetSkillDraw()
	{
		currentTexSkill = 0;
		ultTiempoSkill = 0;
		enabled = false;
	}

	//getters
	public int Tier
	{
		get {return tier;}
	}

    public int getCodigo()
    {
        return codigo;
    }

    public void setCooldownRestante(float cd)
    {
        tiempoUltimoActivo = Game.TiempoTranscurrido - cooldown + cd;
    }
}
