using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class Inventario 
{

	public int oro;
    private int maxEspacio = 30;
	public Item equipadoArma;
	public Item equipadoEscudo;
	public Item equipadoCasco;
	public Item equipadoArmadura;
	public Item equipadoAmuleto;
	public Item equipadoAnillo;
	private List<Item> listaItems;	//si esta equipado NO esta en esta lista
    private Game _refGame;
    private bool mensajeSinEspacio = true;

	public Inventario(Game refGame)
	{
		oro = 10;
        _refGame = refGame;
		ITEMLIST refItemList = ITEMLIST.Instance;	//llamo a la instance para que se initialice
		if (refItemList.dummy)
		{
            refItemList.Reload();
            /*equipadoArma =   new Item(ITEMLIST.Espada[10], Item.Calidad.legendario);   //para que el warning no rompa las pelotas, para que tampoco aparezca un error (que es un bug) de unity
            equipadoEscudo = new Item(ITEMLIST.Escudo[10], Item.Calidad.legendario);
            equipadoArmadura = new Item(ITEMLIST.Armadura[10], Item.Calidad.legendario);
            equipadoCasco = new Item(ITEMLIST.Casco[10], Item.Calidad.legendario);
            equipadoAmuleto = new Item(ITEMLIST.Amuleto[10], Item.Calidad.legendario);
            equipadoAnillo = new Item(ITEMLIST.Anillo[10], Item.Calidad.legendario);*/

            equipadoArma =   new Item(ITEMLIST.Espada[0], Item.Calidad.normal);
            equipadoEscudo = new Item(ITEMLIST.Escudo[0], Item.Calidad.normal);
            equipadoArmadura = new Item(ITEMLIST.Armadura[0], Item.Calidad.normal);
            equipadoCasco = new Item(ITEMLIST.Casco[0], Item.Calidad.normal);
            equipadoAmuleto = new Item(ITEMLIST.Amuleto[0], Item.Calidad.normal);
            equipadoAnillo = new Item(ITEMLIST.Anillo[0], Item.Calidad.normal);
        }
        /*
		equipadoArma = new Item(ITEMLIST.Espada[0], Item.Calidad.legendario);
		equipadoEscudo = new Item(ITEMLIST.Escudo[0], Item.Calidad.epico);
		//equipadoCasco = new Item(ITEMLIST.Casco0, 0);
		equipadoArmadura = new Item(ITEMLIST.Armadura[0], 0);
		equipadoAmuleto = new Item(ITEMLIST.Amuleto[0], Item.Calidad.raro);
		//equipadoAnillo = new Item(ITEMLIST.Anillo0, 0);*/

        listaItems = new List<Item>();
        
        /*for (int i = 0; i < 30; i++)
        {
            listaItems.Add(new Item(ITEMLIST.Armadura[0], Item.Calidad.epico));
        }*/

    }

	public bool AgregarNuevoItem(Item it)
	{
        if (it == null)
            return false;

		if (it.Tipo == Item.TipoItem.Oro)
		{
			oro += it.ValorTotal;
			return true;
		}
        if (listaItems.Count >= maxEspacio)
        {
            if (mensajeSinEspacio)
            {
                _refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(141)); 
                mensajeSinEspacio = false;
            }
            return false;
        }

        mensajeSinEspacio = true;
		listaItems.Add(it);
		return true;
	}

	public Color getColorItemEquipado(string slot)
	{
		slot.ToLower();

		Color col = Color.white;

		if (slot == "arma")
		{
			if (equipadoArma != null)
			{
				col = equipadoArma.ColorTexto;
			}
		}
		
		if (slot == "escudo")
		{
			if (equipadoEscudo != null)
			{
				col = equipadoEscudo.ColorTexto;
			}
		}
		
		if (slot == "armadura")
		{
			if (equipadoArmadura != null)
			{
				col = equipadoArmadura.ColorTexto;
			}	
		}
		
		if (slot == "casco")
		{
			if (equipadoCasco != null)
			{
				col = equipadoCasco.ColorTexto;
			}
		}
		
		if (slot == "amuleto")
		{
			if (equipadoAmuleto != null)
			{
				col = equipadoAmuleto.ColorTexto;
			}
		}
		
		if (slot == "anillo")
		{
			if (equipadoAnillo != null)
			{
				col = equipadoAnillo.ColorTexto;
			}
		}
		
		return col;
	}


	public string getNameEquipado(string slot, int largo = 0)
	{
		slot.ToLower();

		if (largo < 0)
			largo = 0;

		if (slot == "arma")
		{
			if (equipadoArma != null)
			{
				if (largo == 0)
					return equipadoArma.Nombre;
				else
					return equipadoArma.getNombre(largo);
			}
			return "<vacio>";
		}

		if (slot == "escudo")
		{
			if (equipadoEscudo != null)
			{
				if (largo == 0)
					return equipadoEscudo.Nombre;
				else
					return equipadoEscudo.getNombre(largo);
			}
			return "<vacio>";
		}

		if (slot == "armadura")
		{
			if (equipadoArmadura != null)
			{
				if (largo == 0)
					return equipadoArmadura.Nombre;
				else
					return equipadoArmadura.getNombre(largo);
			}	
			return "<vacio>";
		}

		if (slot == "casco")
		{
			if (equipadoCasco != null)
			{
				if (largo == 0)
					return equipadoCasco.Nombre;
				else
					return equipadoCasco.getNombre(largo);
			}
			return "<vacio>";
		}

		if (slot == "amuleto")
		{
			if (equipadoAmuleto != null)
			{
				if (largo == 0)
					return equipadoAmuleto.Nombre;
				else
					return equipadoAmuleto.getNombre(largo);
			}
			return "<vacio>";
		}

		if (slot == "anillo")
		{
			if (equipadoAnillo != null)
			{
				if (largo == 0)
					return equipadoAnillo.Nombre;
				else
					return equipadoAnillo.getNombre(largo);
			}
			return "<vacio>";
		}
			
		return "ERROR"; 
	}
	public Item getEquipadoSegunTipo(Item.TipoItem tipo)
	{
		Item aux = null;

		if (equipadoArma != null && equipadoArma.Tipo == tipo)
			aux = equipadoArma;
		else if (equipadoArmadura != null && equipadoArmadura.Tipo == tipo)
			aux = equipadoArmadura;
		else if (equipadoEscudo != null && equipadoEscudo.Tipo == tipo)
			aux = equipadoEscudo;
		else if (equipadoCasco != null && equipadoCasco.Tipo == tipo)
			aux = equipadoCasco;
		else if (equipadoAmuleto != null && equipadoAmuleto.Tipo == tipo)
			aux = equipadoAmuleto;
		else if (equipadoAnillo != null && equipadoAnillo.Tipo == tipo)
			aux = equipadoAnillo;


		return aux;
	}

	public Item sacarItemInventarioByIndex(int i)
	{
		if (i >= listaItems.Count)
			return null;
        mensajeSinEspacio = true;
		Item aux = listaItems[i];
		listaItems.RemoveAt(i);
		return aux;
	}

	public int getTamInventario()
	{
		return listaItems.Count;
	}

    public int getTamInventarioMax()
    {
        return maxEspacio;
    }

    public List<Item> getListaItems()
    {
        return listaItems;
    }

	public int AumentoDmg()
	{
		return (int)getTotalAtributoEquipado(Item.ATRIBUTO.DMG);
	}

	public Item getItemByIndex(int i)
	{
		if (i >= listaItems.Count)
			return null;
		return listaItems[i];
	}

	public float getTotalAtributoEquipado(Item.ATRIBUTO a)
	{
		float valor = 0;
		Item aux = null;
		int i = 0;

		while (i < 6)
		{

			switch (i)
			{
			case 0:
				aux = equipadoArma;
				break;
			case 1:
				aux = equipadoArmadura;
				break;
			case 2:
				aux = equipadoEscudo;
				break;
			case 3:
				aux = equipadoAmuleto;
				break;
			case 4:
				aux = equipadoAnillo;
				break;
			case 5:
				aux = equipadoCasco;
				break;
			}
			i++;

			if (aux != null)
			{
				if (aux.getTipoAtributo(0) == a)
				{
					valor += aux.getValorAtributo(0);
				}
				if (aux.getCalidad != Item.Calidad.normal)	//es por lo menos raro
				{
					if (aux.getTipoAtributo(1) == a)
					{
						valor += aux.getValorAtributo(1);
					}
					if (aux.getCalidad == Item.Calidad.epico || aux.getCalidad == Item.Calidad.legendario)
					{
						if (aux.getTipoAtributo(2) == a)
						{
							valor += aux.getValorAtributo(2);
						}

						if (aux.getCalidad == Item.Calidad.legendario)
						{
							if (aux.getTipoAtributo(3) == a)
							{
								valor += aux.getValorAtributo(3);
							}
						}
					}
				}
			}
		}

		return valor;
	}

    public bool cambiarCantidadOro(int o)
    {
        if (o < 0 && oro + o < 0)   //si esta perdiendo oro, y no alcanza la cantidad de oro que tiene devuelve falso
            return false;

        oro += o;

        return true;
    }

    public bool getInventarioLleno()
    {
        return (getTamInventario() == getTamInventarioMax());
    }
}
