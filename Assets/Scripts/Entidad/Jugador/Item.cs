using UnityEngine;
using System.Collections;

public sealed class Item
{
	public enum ATRIBUTO {DMG, ARMADURA, GOLPE_CRITICO, ESQUIVAR, VEL_MOV, CD_REDUCCION, NADA};
	private int codigoItem;
	private string nombre;
	public enum Calidad {normal, raro, epico, legendario};	//depende del modificador: 0% - 5% normal | 6% - 14% raro | 15% - 24% epico | 25% a 50% legendario
	private Calidad calidad;
	private int valorBase;
	public enum TipoItem {Casco, Amuleto, Armadura, Anillo, Espada, Arco, Baston, Escudo, Oro, Otro};	//el wizard usa libro de hechizos, el monje usa baston
	private TipoItem tipoItem;
	private int valorOroBase;
	private Color colorTexto;
	private ATRIBUTO[] atributos;
	private int[] modificadorAtributosSecundarios;
    public int lvl; //deberia ser privado con getter pero bueno

	public Item()
	{
		//default
	}
	//usar este constructor solo para las definiciones, para instanciar elementos usar el de abajo
	public Item(int codItem, int nivel, string nombre, TipoItem tipo, int valor, int costo, ATRIBUTO atrib0 = ATRIBUTO.NADA, ATRIBUTO atrib1 = ATRIBUTO.NADA, int mod1 = 0, ATRIBUTO atrib2 = ATRIBUTO.NADA, int mod2 = 0, ATRIBUTO atrib3 = ATRIBUTO.NADA, int mod3 = 0)	
	{
		codigoItem = codItem;
		this.nombre = nombre;
		tipoItem = tipo;
		valorBase = valor;
		valorOroBase = costo;
		calidad = Calidad.normal;

        colorTexto = Color.white;

		if (tipoItem == TipoItem.Oro)
		{
			colorTexto = Color.yellow;
		}

		atributos = new ATRIBUTO[4]; //EL PRIMER ATRIBUTO ES EL FIJO; EL VALOR CAMBIA CON EL MODIFICADOR; LOS DEMAS DEPENDEN DE LA CALIDAD DEL OBJETO Y TIENEN VALOR FIJO
		modificadorAtributosSecundarios = new int[3];
		atributos[0] = atrib0;
		atributos[1] = atrib1;	modificadorAtributosSecundarios[0] = mod1;
		atributos[2] = atrib2;	modificadorAtributosSecundarios[1] = mod2;
		atributos[3] = atrib3;	modificadorAtributosSecundarios[2] = mod3;
        lvl = nivel;
	}

	public Item(Item itemBase, Calidad calidad)
	{
		colorTexto = new Color(1.0f, 1.0f, 1.0f);
		codigoItem = itemBase.codigoItem;
		nombre = itemBase.nombre;
		valorBase = itemBase.valorBase;
		tipoItem = itemBase.tipoItem;
		valorOroBase = itemBase.valorOroBase;
        this.calidad = calidad;
		colorTexto = Color.white;


        //ACA TENDRIA QUE PONER PARA QUE LOS ATRIBUTOS SECUNDARIOS SE ELIJAN AL AZAR
        switch(calidad)
        {
            case Calidad.normal:

                break;
            case Calidad.raro:
                colorTexto = Color.green;
                break;
            case Calidad.epico:
                colorTexto = Color.red;
                break;
            case Calidad.legendario:
                colorTexto = new Color(0.5f, 0, 0.5f);
                break;

        }

		if (tipoItem == TipoItem.Oro)
		{
			colorTexto = Color.yellow;
		}

        int[] orden = new int[3];

        orden[0] = -1;
        orden[1] = -1;
        orden[2] = -1;

        for (int i = 0; i < 3; i++)
        {
            int val = -1;
            while (val == -1)
            {
                val = Random.Range(0, 3);

                if (orden[0] == val || orden[1] == val || orden[2] == val)
                {
                    val = -1;
                }
                else
                {
                    orden[i] = val;
                }

            }
            

        }

        modificadorAtributosSecundarios = new int[3];
        atributos = new ATRIBUTO[4];
        atributos[0] = itemBase.atributos[0];
        for (int c = 0; c < 3; c++)
        {
            modificadorAtributosSecundarios[c] = itemBase.modificadorAtributosSecundarios[orden[c]];
            atributos[c + 1] = itemBase.atributos[orden[c] + 1];
        }

		
		/*modificadorAtributosSecundarios = itemBase.modificadorAtributosSecundarios;
		atributos = itemBase.atributos;*/
        lvl = itemBase.lvl;
	}

    //usar esta para cargar partida
    public Item(Item itemBase, Calidad calidad, ATRIBUTO atrib1, ATRIBUTO atrib2, ATRIBUTO atrib3)
    {
        colorTexto = new Color(1.0f, 1.0f, 1.0f);
        codigoItem = itemBase.codigoItem;
        nombre = itemBase.nombre;
        valorBase = itemBase.valorBase;
        tipoItem = itemBase.tipoItem;
        valorOroBase = itemBase.valorOroBase;
        this.calidad = calidad;
        colorTexto = Color.white;


        //ACA TENDRIA QUE PONER PARA QUE LOS ATRIBUTOS SECUNDARIOS SE ELIJAN AL AZAR
        switch (calidad)
        {
            case Calidad.normal:
                colorTexto = Color.white;
                break;
            case Calidad.raro:
                colorTexto = Color.green;
                break;
            case Calidad.epico:
                colorTexto = Color.red;
                break;
            case Calidad.legendario:
                colorTexto = new Color(0.5f, 0, 0.5f);
                break;

        }

        if (tipoItem == TipoItem.Oro)
        {
            colorTexto = Color.yellow;
        }

        int[] orden = new int[3];

        orden[0] = -1;
        orden[1] = -1;
        orden[2] = -1;

        for (int i = 0; i < 3; i++)
        {
            if (atrib1 == itemBase.atributos[i + 1])
            {
                orden[0] = i;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (atrib2 == itemBase.atributos[i + 1])
            {
                orden[1] = i;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            if (atrib3 == itemBase.atributos[i + 1])
            {
                orden[2] = i;
            }
        }

        modificadorAtributosSecundarios = new int[3];
        atributos = new ATRIBUTO[4];
        atributos[0] = itemBase.atributos[0];
        for (int c = 0; c < 3; c++)
        {
            modificadorAtributosSecundarios[c] = itemBase.modificadorAtributosSecundarios[orden[c]];
            atributos[c + 1] = itemBase.atributos[orden[c] + 1];
        }


        /*modificadorAtributosSecundarios = itemBase.modificadorAtributosSecundarios;
		atributos = itemBase.atributos;*/
        lvl = itemBase.lvl;
    }

    private float getValorCalidad()
    {
        float aux = 0f;
        switch (calidad)
        {
            case Calidad.normal:
                aux = 0f;
                break;
            case Calidad.raro:
                aux = 15f;
                break;
            case Calidad.epico:
                aux = 30f;
                break;
            case Calidad.legendario:
                aux = 50f;
                break;
        }

        return aux;
    }


	//getters

	public int CodigoItem
	{
		get {return codigoItem;}
	}
	public string Nombre
	{
		get {return nombre;}
	}
	public Calidad getCalidad
	{
		get {return calidad;}
	}

	public int ValorBase
	{
		get {return valorBase;}
	}


	public int ValorTotal		//valor base + modificador
	{
		get {return valorBase + (int)((float)valorBase * getValorCalidad() / 100.0f);}
	}

	public TipoItem Tipo
	{
		get {return tipoItem;}
	}
	public int ValorOro			//valor de oro base + modificador
	{
		get {return valorOroBase + (int)((float)valorOroBase * getValorCalidad() / 100.0f);}
	}

	public string getNombre(int cantChar)
	{
		if (cantChar >= nombre.Length)
			return nombre;
		return (nombre.Substring(0, cantChar) + "...");
	}

	public Color ColorTexto
	{
		get {return colorTexto;}
	}

	public ATRIBUTO getTipoAtributo(int index)
	{
		if (index < 0 || index > 3)
			return ATRIBUTO.NADA;
		return atributos[index];
	}

	public int getValorAtributo(int index)
	{
		if (index < 0 || index > 3)
			return 0;
		if (index == 0)
			return ValorTotal;
		return modificadorAtributosSecundarios[index -1];

	}

	public static string tipoAtributoToString(ATRIBUTO a)
	{
        if (a == ATRIBUTO.ARMADURA)
			return CONFIG.getTexto(145);
		if (a == ATRIBUTO.CD_REDUCCION)
			return CONFIG.getTexto(146);
		if (a == ATRIBUTO.DMG)
			return CONFIG.getTexto(147);
		if (a == ATRIBUTO.ESQUIVAR)
			return CONFIG.getTexto(148);
		if (a == ATRIBUTO.GOLPE_CRITICO)
			return CONFIG.getTexto(149);
		if (a == ATRIBUTO.VEL_MOV)
			return CONFIG.getTexto(150);
		return "";

	}

    public string getNombre()
    {
        return nombre;
    }

    public string getCalidadTexto()
    {
        switch (calidad)
        {
            case Calidad.normal:
                return "Normal";
                //break;
            case Calidad.raro:
                return CONFIG.getTexto(151);
               // break;
            case Calidad.epico:
                return CONFIG.getTexto(152);
                //break;
            case Calidad.legendario:
                return CONFIG.getTexto(153);
                //break;
            default:
                return "";
                //break;
        }
    }
}
