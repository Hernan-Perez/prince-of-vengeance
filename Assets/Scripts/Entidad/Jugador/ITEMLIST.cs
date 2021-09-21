using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class ITEMLIST
{
    public class ITEM_GROUP
    {
        private List<Item> item;
        private float _probNormal, _probRaro, _probEpico, _probLegendario;

        public ITEM_GROUP(float probNormal = 5f, float probRaro = 3f, float probEpico = 1.5f, float probLegendario = 0.5f)
        {
            item = new List<Item>();
            _probNormal = probNormal;
            _probRaro = probRaro;
            _probEpico = probEpico;
            _probLegendario = probLegendario;
        }

        public void AgregarItem(Item i)
        {
            item.Add(i);
        }

        public int getItemCant()
        {
            return item.Count;
        }

        public Item getItemAtIndex(int index)
        {
            return item[index];
        }
        public float getProbabilidad(Item.Calidad calidad)
        {
            switch (calidad)
            {
                case Item.Calidad.normal:
                    return _probNormal;

                case Item.Calidad.raro:
                    return _probRaro;

                case Item.Calidad.epico:
                    return _probEpico;

                case Item.Calidad.legendario:
                    return _probLegendario;

                default:
                    return 0;
            }
        }
    }


	private static ITEMLIST instance;

	public static Item[] Espada;
	public static Item[] Escudo;
	public static Item[] Armadura;
	public static Item[] Casco;
	public static Item[] Amuleto;
	public static Item[] Anillo;
	public bool dummy = true;

	private ITEMLIST() 
	{
		Espada = new Item[11];
		Escudo = new Item[11];
		Armadura = new Item[11];
		Casco = new Item[11];
		Amuleto = new Item[11];
		Anillo = new Item[11];

        //item lvls 1, 10, 20, 30 , 40, 50, 60 ,70 ,80, 90 ,95 |||| 11 tiers

        /*
         * TIERS:
         * 
         * NORMAL
         * T0: exclusivo zona 1 - hay vendedor
         * T1: exclusivo zona 2 - hay vendedor
         * T2: zonas 4,5 (bosque y biblioteca) - hay vendedor (biblioteca)
         * T3: zona 6 - hay vendedor en comedor
         * T4: zona 7 - hay vendedor (escondido al final)
         * T5: zona 8 - hay vendedor
         * T6: unicamente boss final
         * 
         * EXTREMO:
         * T6: zona 1 - sin vendedor
         * T7: zona 2 - vend
         * T8: zona 4,5 - vend
         * T9: zona 6 - vend comedor
         * T10: zona 7, 8 sin vend
         * 
         * */
        Reload();

	}

    public void Reload()
    {

        if (CONFIG.idioma == 0)
        {
            Espada[0] = new Item(50, 1, "Espada de madera", Item.TipoItem.Espada, 12, 00120, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 3, Item.ATRIBUTO.CD_REDUCCION, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[1] = new Item(51, 10, "Espada de hierro", Item.TipoItem.Espada, 20, 01500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 4, Item.ATRIBUTO.CD_REDUCCION, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[2] = new Item(52, 20, "Espada de acero", Item.TipoItem.Espada, 35, 04400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 5, Item.ATRIBUTO.CD_REDUCCION, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[3] = new Item(53, 30, "Espada de acero encantado", Item.TipoItem.Espada, 50, 08500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 7, Item.ATRIBUTO.CD_REDUCCION, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[4] = new Item(54, 40, "Espada de los antiguos", Item.TipoItem.Espada, 60, 12500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 9, Item.ATRIBUTO.CD_REDUCCION, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[5] = new Item(55, 50, "Espada de hueso de gigante", Item.TipoItem.Espada, 75, 22400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 12, Item.ATRIBUTO.CD_REDUCCION, 11, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[6] = new Item(56, 60, "Espada antigua de oro", Item.TipoItem.Espada, 85, 28500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 15, Item.ATRIBUTO.CD_REDUCCION, 14, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[7] = new Item(57, 70, "Gran espada de adamanto", Item.TipoItem.Espada, 100, 35200, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 18, Item.ATRIBUTO.CD_REDUCCION, 18, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[8] = new Item(58, 80, "Espada de cristal de fuego de dragon", Item.TipoItem.Espada, 110, 43500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 24, Item.ATRIBUTO.CD_REDUCCION, 21, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[9] = new Item(59, 90, "Espada gigante del cazador de demonios", Item.TipoItem.Espada, 120, 62400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 30, Item.ATRIBUTO.CD_REDUCCION, 25, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[10] = new Item(60, 95, "Gran espada de escamas de dragon", Item.TipoItem.Espada, 150, 97200, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 35, Item.ATRIBUTO.CD_REDUCCION, 30, Item.ATRIBUTO.VEL_MOV, 5);

            Escudo[0] = new Item(61, 1, "Escudo de madera", Item.TipoItem.Escudo, 30, 100, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[1] = new Item(62, 10, "Escudo de hierro", Item.TipoItem.Escudo, 70, 1450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[2] = new Item(63, 20, "Escudo de acero", Item.TipoItem.Escudo, 140, 4300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[3] = new Item(64, 30, "Escudo de acero encantado", Item.TipoItem.Escudo, 220, 8450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[4] = new Item(65, 40, "Escudo de los antiguos", Item.TipoItem.Escudo, 430, 12300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[5] = new Item(66, 50, "Escudo de hueso de gigante", Item.TipoItem.Escudo, 540, 22200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[6] = new Item(67, 60, "Escudo antiguo de oro", Item.TipoItem.Escudo, 720, 28000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[7] = new Item(68, 70, "Gran escudo de adamanto", Item.TipoItem.Escudo, 980, 35000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[8] = new Item(69, 80, "Escudo de cristal de fuego de dragon", Item.TipoItem.Escudo, 1050, 43200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[9] = new Item(70, 90, "Escudo gigante de cazador de demonios", Item.TipoItem.Escudo, 1215, 62250, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[10] = new Item(71, 95, "Gran Escudo de escamas de dragon", Item.TipoItem.Escudo, 1540, 96850, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);

            Armadura[0] = new Item(72, 1, "Armadura de cuero degastado", Item.TipoItem.Armadura, 50, 100, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[1] = new Item(73, 10, "Armadura de cuero", Item.TipoItem.Armadura, 110, 1450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[2] = new Item(74, 20, "Armadura de cuero reforzado", Item.TipoItem.Armadura, 175, 4300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[3] = new Item(75, 30, "Armadura de acero", Item.TipoItem.Armadura, 235, 8450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[4] = new Item(76, 40, "Armadura de los antiguos", Item.TipoItem.Armadura, 470, 12300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[5] = new Item(77, 50, "Armadura de hueso de gigante", Item.TipoItem.Armadura, 590, 22200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 7, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[6] = new Item(78, 60, "Armadura de adamanto", Item.TipoItem.Armadura, 785, 28000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[7] = new Item(79, 70, "Armadura de adamanto reforzado", Item.TipoItem.Armadura, 1040, 35000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 10, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[8] = new Item(80, 80, "Armadura de cristal de fuego de dragon", Item.TipoItem.Armadura, 1560, 63200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 11, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[9] = new Item(81, 90, "Armadura de cazador de demonios", Item.TipoItem.Armadura, 2045, 82250, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 13, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[10] = new Item(82, 95, "Armadura de escamas de dragon", Item.TipoItem.Armadura, 2300, 126850, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 15, Item.ATRIBUTO.DMG, 12, Item.ATRIBUTO.VEL_MOV, 5);

            Casco[0] = new Item(83, 1, "Casco de cuero degastado", Item.TipoItem.Casco, 20, 80, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[1] = new Item(84, 10, "Casco de cuero", Item.TipoItem.Casco, 65, 1300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[2] = new Item(85, 20, "Casco de cuero reforzado", Item.TipoItem.Casco, 125, 4150, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[3] = new Item(86, 30, "Casco de acero", Item.TipoItem.Casco, 180, 8000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[4] = new Item(87, 40, "Casco de los antiguos", Item.TipoItem.Casco, 375, 11200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 6, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[5] = new Item(88, 50, "Casco de hueso de gigante", Item.TipoItem.Casco, 480, 18500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 7, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[6] = new Item(89, 60, "Casco de adamanto", Item.TipoItem.Casco, 695, 25500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[7] = new Item(90, 70, "Casco de adamanto reforzado", Item.TipoItem.Casco, 910, 34000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[8] = new Item(91, 80, "Casco de cristal de fuego de dragon", Item.TipoItem.Casco, 1040, 42000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 9, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[9] = new Item(92, 90, "Casco de cazador de demonios", Item.TipoItem.Casco, 1170, 59500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 9, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[10] = new Item(93, 95, "Casco de escamas de dragon", Item.TipoItem.Casco, 1480, 87500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 10, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);

            Amuleto[0] = new Item(39, 1, "Amuleto de hierro", Item.TipoItem.Amuleto, 1, 40, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[1] = new Item(40, 10, "Amuleto de safiro", Item.TipoItem.Amuleto, 2, 800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[2] = new Item(41, 20, "Amuleto de safiro encantado", Item.TipoItem.Amuleto, 3, 2200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[3] = new Item(42, 30, "Amuleto de rubi", Item.TipoItem.Amuleto, 4, 6400, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[4] = new Item(43, 40, "Amuleto de rubi encantado", Item.TipoItem.Amuleto, 5, 8200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[5] = new Item(44, 50, "Amuleto de onyx", Item.TipoItem.Amuleto, 6, 12500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[6] = new Item(45, 60, "Amuleto de onyx encantado", Item.TipoItem.Amuleto, 7, 16000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[7] = new Item(46, 70, "Amuleto de los antiguos", Item.TipoItem.Amuleto, 8, 22500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[8] = new Item(47, 80, "Amuleto de la destruccion", Item.TipoItem.Amuleto, 9, 37500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[9] = new Item(48, 90, "Amuleto de cazador de demonios", Item.TipoItem.Amuleto, 10, 47000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 12, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[10] = new Item(49, 95, "Amuleto de alma de dragon", Item.TipoItem.Amuleto, 12, 69500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 15, Item.ATRIBUTO.VEL_MOV, 5);

            Anillo[0] = new Item(28, 1, "Anillo de hierro", Item.TipoItem.Anillo, 1, 40, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[1] = new Item(29, 10, "Anillo de safiro", Item.TipoItem.Anillo, 2, 750, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[2] = new Item(30, 20, "Anillo de safiro encantado", Item.TipoItem.Anillo, 3, 2150, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[3] = new Item(31, 30, "Anillo de rubi", Item.TipoItem.Anillo, 4, 6200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[4] = new Item(32, 40, "Anillo de rubi encantado", Item.TipoItem.Anillo, 5, 8000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[5] = new Item(33, 50, "Anillo de onyx", Item.TipoItem.Anillo, 6, 11800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[6] = new Item(34, 60, "Anillo de onyx encantado", Item.TipoItem.Anillo, 7, 15800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[7] = new Item(35, 70, "Anillo de los antiguos", Item.TipoItem.Anillo, 8, 21200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[8] = new Item(36, 80, "Anillo de la destruccion", Item.TipoItem.Anillo, 9, 36000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[9] = new Item(37, 90, "Anillo de cazador de demonios", Item.TipoItem.Anillo, 10, 45500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[10] = new Item(38, 95, "Anillo de alma de dragon", Item.TipoItem.Anillo, 12, 68000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
        }
        else
        {
            Espada[0] = new Item(50, 1, "Wooden sword", Item.TipoItem.Espada, 12, 00120, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 3, Item.ATRIBUTO.CD_REDUCCION, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[1] = new Item(51, 10, "Iron sword", Item.TipoItem.Espada, 20, 01500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 4, Item.ATRIBUTO.CD_REDUCCION, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[2] = new Item(52, 20, "Steel sword", Item.TipoItem.Espada, 35, 04400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 5, Item.ATRIBUTO.CD_REDUCCION, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[3] = new Item(53, 30, "Enchanted steel sword", Item.TipoItem.Espada, 50, 08500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 7, Item.ATRIBUTO.CD_REDUCCION, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[4] = new Item(54, 40, "Sword of the ancients", Item.TipoItem.Espada, 60, 12500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 9, Item.ATRIBUTO.CD_REDUCCION, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[5] = new Item(55, 50, "Sword of giant's bone", Item.TipoItem.Espada, 75, 22400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 12, Item.ATRIBUTO.CD_REDUCCION, 11, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[6] = new Item(56, 60, "Ancient golden sword", Item.TipoItem.Espada, 85, 28500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 15, Item.ATRIBUTO.CD_REDUCCION, 14, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[7] = new Item(57, 70, "Great adamant sword", Item.TipoItem.Espada, 100, 35200, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 18, Item.ATRIBUTO.CD_REDUCCION, 18, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[8] = new Item(58, 80, "Dragon fire sword", Item.TipoItem.Espada, 110, 43500, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 24, Item.ATRIBUTO.CD_REDUCCION, 21, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[9] = new Item(59, 90, "Giant sword of the demon hunter", Item.TipoItem.Espada, 120, 62400, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 30, Item.ATRIBUTO.CD_REDUCCION, 25, Item.ATRIBUTO.VEL_MOV, 5);
            Espada[10] = new Item(60, 95, "Giant sword of dragon scales", Item.TipoItem.Espada, 150, 97200, Item.ATRIBUTO.DMG, Item.ATRIBUTO.GOLPE_CRITICO, 35, Item.ATRIBUTO.CD_REDUCCION, 30, Item.ATRIBUTO.VEL_MOV, 5);

            Escudo[0] = new Item(61, 1, "Wooden shield", Item.TipoItem.Escudo, 30, 100, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[1] = new Item(62, 10, "Iron shield", Item.TipoItem.Escudo, 70, 1450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[2] = new Item(63, 20, "Steel shield", Item.TipoItem.Escudo, 140, 4300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[3] = new Item(64, 30, "Enchanted steel shield", Item.TipoItem.Escudo, 220, 8450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[4] = new Item(65, 40, "Shield of the ancients", Item.TipoItem.Escudo, 430, 12300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[5] = new Item(66, 50, "Shield of giant's bone", Item.TipoItem.Escudo, 540, 22200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[6] = new Item(67, 60, "Ancient golden shield", Item.TipoItem.Escudo, 720, 28000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[7] = new Item(68, 70, "Great adamant shield", Item.TipoItem.Escudo, 980, 35000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[8] = new Item(69, 80, "Dragon fire shield", Item.TipoItem.Escudo, 1050, 43200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[9] = new Item(70, 90, "Giant shield of the demon hunter", Item.TipoItem.Escudo, 1215, 62250, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Escudo[10] = new Item(71, 95, "Giant shield of dragon scales", Item.TipoItem.Escudo, 1540, 96850, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);

            Armadura[0] = new Item(72, 1, "Worn leather armor", Item.TipoItem.Armadura, 50, 100, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[1] = new Item(73, 10, "Leather armor", Item.TipoItem.Armadura, 110, 1450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[2] = new Item(74, 20, "Reinforced leather armor", Item.TipoItem.Armadura, 175, 4300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[3] = new Item(75, 30, "Steel armor", Item.TipoItem.Armadura, 235, 8450, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[4] = new Item(76, 40, "Armor of the ancient", Item.TipoItem.Armadura, 470, 12300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[5] = new Item(77, 50, "Armor of giant's bone", Item.TipoItem.Armadura, 590, 22200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 7, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[6] = new Item(78, 60, "Adamant armor", Item.TipoItem.Armadura, 785, 28000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[7] = new Item(79, 70, "Reinforced adamant armor", Item.TipoItem.Armadura, 1040, 35000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 10, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[8] = new Item(80, 80, "Dragon fire armor", Item.TipoItem.Armadura, 1560, 63200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 11, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[9] = new Item(81, 90, "Great armor of the demon hunter", Item.TipoItem.Armadura, 2045, 82250, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 13, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
            Armadura[10] = new Item(82, 95, "Great armor of dragon scales", Item.TipoItem.Armadura, 2300, 126850, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 15, Item.ATRIBUTO.DMG, 12, Item.ATRIBUTO.VEL_MOV, 5);

            Casco[0] = new Item(83, 1, "Worn leather helmet", Item.TipoItem.Casco, 20, 80, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[1] = new Item(84, 10, "Leather helmet", Item.TipoItem.Casco, 65, 1300, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[2] = new Item(85, 20, "Reinforced leather helmet", Item.TipoItem.Casco, 125, 4150, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[3] = new Item(86, 30, "Steel helmet", Item.TipoItem.Casco, 180, 8000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[4] = new Item(87, 40, "Helmet of the ancient", Item.TipoItem.Casco, 375, 11200, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 6, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[5] = new Item(88, 50, "Helmet of giant's bone", Item.TipoItem.Casco, 480, 18500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 7, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[6] = new Item(89, 60, "Adamant helmet", Item.TipoItem.Casco, 695, 25500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[7] = new Item(90, 70, "Reinforced adamant helmet", Item.TipoItem.Casco, 910, 34000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 8, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[8] = new Item(91, 80, "Dragon fire helmet", Item.TipoItem.Casco, 1040, 42000, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 9, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[9] = new Item(92, 90, "Great helmet of the demon hunter", Item.TipoItem.Casco, 1170, 59500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 9, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Casco[10] = new Item(93, 95, "Great helmet of dragon scales", Item.TipoItem.Casco, 1480, 87500, Item.ATRIBUTO.ARMADURA, Item.ATRIBUTO.ESQUIVAR, 10, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);

            Amuleto[0] = new Item(39, 1, "Iron necklace", Item.TipoItem.Amuleto, 1, 40, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[1] = new Item(40, 10, "Sapphire necklace", Item.TipoItem.Amuleto, 2, 800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[2] = new Item(41, 20, "Enchanted sapphire necklace", Item.TipoItem.Amuleto, 3, 2200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[3] = new Item(42, 30, "Ruby necklace", Item.TipoItem.Amuleto, 4, 6400, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[4] = new Item(43, 40, "Enchanted ruby necklace", Item.TipoItem.Amuleto, 5, 8200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[5] = new Item(44, 50, "Onyx necklace", Item.TipoItem.Amuleto, 6, 12500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[6] = new Item(45, 60, "Enchanted onyx necklace", Item.TipoItem.Amuleto, 7, 16000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[7] = new Item(46, 70, "Necklace of the ancients", Item.TipoItem.Amuleto, 8, 22500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[8] = new Item(47, 80, "Necklace of the destruction", Item.TipoItem.Amuleto, 9, 37500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[9] = new Item(48, 90, "Necklace of the demon hunter", Item.TipoItem.Amuleto, 10, 47000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 12, Item.ATRIBUTO.VEL_MOV, 5);
            Amuleto[10] = new Item(49, 95, "Dragon soul necklace", Item.TipoItem.Amuleto, 12, 69500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 15, Item.ATRIBUTO.VEL_MOV, 5);

            Anillo[0] = new Item(28, 1, "Iron ring", Item.TipoItem.Anillo, 1, 40, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 1, Item.ATRIBUTO.DMG, 1, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[1] = new Item(29, 10, "Sapphire ring", Item.TipoItem.Anillo, 2, 750, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 2, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[2] = new Item(30, 20, "Enchanted sapphire ring", Item.TipoItem.Anillo, 3, 2150, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 3, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[3] = new Item(31, 30, "Ruby ring", Item.TipoItem.Anillo, 4, 6200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 2, Item.ATRIBUTO.DMG, 4, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[4] = new Item(32, 40, "Enchanted ruby ring", Item.TipoItem.Anillo, 5, 8000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 5, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[5] = new Item(33, 50, "Onyx ring", Item.TipoItem.Anillo, 6, 11800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 6, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[6] = new Item(34, 60, "Enchanted onyx ring", Item.TipoItem.Anillo, 7, 15800, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 3, Item.ATRIBUTO.DMG, 7, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[7] = new Item(35, 70, "Ring of the ancient", Item.TipoItem.Anillo, 8, 21200, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 8, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[8] = new Item(36, 80, "Ring of the destruction", Item.TipoItem.Anillo, 9, 36000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[9] = new Item(37, 90, "Ring of the demon hunter", Item.TipoItem.Anillo, 10, 45500, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 4, Item.ATRIBUTO.DMG, 9, Item.ATRIBUTO.VEL_MOV, 5);
            Anillo[10] = new Item(38, 95, "Dragon soul ring", Item.TipoItem.Anillo, 12, 68000, Item.ATRIBUTO.CD_REDUCCION, Item.ATRIBUTO.ESQUIVAR, 5, Item.ATRIBUTO.DMG, 10, Item.ATRIBUTO.VEL_MOV, 5);
        }

    }

    public static ITEMLIST Instance
	{
		get 
		{
			if (instance == null)
			{
				instance = new ITEMLIST();
			}
			return instance;
		}
	}

    public ITEM_GROUP getPreset(int index, Vector4 probabilidades = new Vector4()) //probabilidades = zero para usar las estandar
    {
        ITEM_GROUP aux;
        if (probabilidades.x == 0f && probabilidades.y == 0f && probabilidades.z == 0f && probabilidades.w == 0f)
        {
            aux = new ITEM_GROUP();
        }
        else
        {
            aux = new ITEM_GROUP(probabilidades.x, probabilidades.y, probabilidades.z, probabilidades.w);
        }
        

        aux.AgregarItem(Espada[index]);
        aux.AgregarItem(Armadura[index]);
        aux.AgregarItem(Casco[index]);
        aux.AgregarItem(Escudo[index]);
        aux.AgregarItem(Amuleto[index]);
        aux.AgregarItem(Anillo[index]);


        return aux;
    }

    public Item getItemPorCodigo(int codigo)
    {
        Item aux = null;
        int i = 0;

        for (i = 0; aux == null && i < 11; i++)
        {
            if (codigo == Armadura[i].CodigoItem)
            {
                aux = Armadura[i];
                break;
            }
            if (codigo == Casco[i].CodigoItem)
            {
                aux = Casco[i];
                break;
            }
            if (codigo == Espada[i].CodigoItem)
            {
                aux = Espada[i];
                break;
            }
            if (codigo == Escudo[i].CodigoItem)
            {
                aux = Escudo[i];
                break;
            }
            if (codigo == Anillo[i].CodigoItem)
            {
                aux = Anillo[i];
                break;
            }
            if (codigo == Amuleto[i].CodigoItem)
            {
                aux = Amuleto[i];
                break;
            }
        }
        
        return aux;
    }
}