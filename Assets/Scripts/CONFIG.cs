using UnityEngine;
using System.Collections;

public static class CONFIG
{
    public static int idioma = 1;   // 0 = español 1 = ingles

    public static bool MODO_EXTREMO = false;

    //rango de volumen 0f a 1f
    public static float vol_sonido = 1f;
    public static float vol_musica = 0.5f;

	private static float _escala = 1f;	// 1 es lo normal (64)
	private static float _TAM = _escala * 64;

	public static float escala
	{
		get 
		{
			return _escala;
		}
		set 
		{
			_escala = value;
			_TAM = _escala * 64;
		}
	}

	public static float TAM
	{
		get 
		{
			return _TAM;
		}
	}

    public static bool volviendoAMenu = false;

	public static float TiempoAnimCaminar = 0.1f;
	public static float TiempoAnimAtacar = 0.1f;
	public static bool DEBUG = true;
	public static int inventarioMAX = 200;
	public static bool mostrarPanelZoom = true;
    public static int escalaTiempo = 1; //x segundos reales son 1 min del juego
    public static int offsetTiempoMinutos = 1020;    //12 horas

    public static string[,] t = new string [,]
    { 
     {"Jugar", "Play"},//0
     {"Configuración","Configuration"},//1
     {"Ayuda","Help"},//2
     {"Ranura 1: Partida Nueva","Slot 1: New Game"},
     {"Ranura 2: Partida Nueva","Slot 2: New Game"},
     {"Ranura 3: Partida Nueva","Slot 3: New Game"},
     {"Partidas:","Games:"},//6
     {"Volver","Back"},
     {"Iniciar","Start"},//8
     {"Borrar","Delete"},//9
     {"Sonido:","Sound:"},//10
     {"Musica:","Music:"},//11
     {"Idioma:","Language:"},//12
     {"Por Hernán Pérez","By Hernan Perez"},//13
     {"Saltar","Skip"},//14
     {"Siguiente","Next"},//15
     {"Comenzar","Start"},//16
     {"Tienda:","Shop:"},
     {"Comprar","Buy"},//18
     {"Vender","Sell"},
     {"Salir","Exit"},//20
     {"Oro: ","Gold: "},
     {"Apostador: ","Gambler: "}, //22
     {"Precio actual: ","Current price: "},
     {"Has conseguido ","You've got "},//24
     {"Dificultad: Normal","Difficulty: Normal"},
     {"Dificultad: Extremo","Difficulty: Extreme"},//26
     {"Nivel: ","Level: "},
     {"Daño: ","Damage: "},//28
     {"Vida: ","Life: "},
     {"Experiencia: ","Experience: "},//30
     {"Daño base: ","Base damage: "},
     {"Prob. de golpe critico: ","Critic hit chance: "},//32
     {"Reducción de enfriamiento: ","Cooldown reduction: "},
     {"Armadura: ","Armor: "},//34
     {"Daño recibido: ","Damage taken: "},
     {"Prob. de esquivar: ","Dodge chance: "}, //36
     {"Vel. de movimiento: ","Walking speed: "},
     {"Inventario","Backpack"},//38
     {"Habilidades","Skills"},
     {"Menu principal","Main menu"},//40
     {"Equipado","Equipped"},
     {"Casco: ","Helmet: "},//42
     {"Amuleto: ","Necklace: "},
     {"Anillo: ","Ring: "},//44
     {"Arma: ","Weapon: "},
     {"Escudo: ","Shield: "},//46
     {"Desequipar","Unequip"},
     {"Atras","Back"},//48
     {"(Raro)","(Rare)"},
     {"(Épico)","(Epic)"},//50
     {"(Legendario)","(Legendary)"},
     {"Valor: ","Value: "},//52
     {" oro"," gold"},
     {"Nivel ","Level "},//54
     {"(Amuleto)","(Necklace)"},
     {"(Anillo)","(Ring)"},//56
     {"(Armadura)","(Armor)"},
     {"(Casco)","(Helmet)"},//58
     {"(Escudo)","(Shield)"},
     {"(Espada)","(Sword)"},//60
     {"Ant","Back"},
     {"Sig","Next"},//62
     {"Equipar","Equip"},
     {"Aprender","Learn"},//64
     {"Aprendida","Learned"},
     {"¿Esta seguro desea volver al menu principal?","Are you sure you want to go back to Main Menu?"},//66
     {"Guardian Barnak","Guard Barnak"},
     {"Guardianes Ruk & Nhar","Guards Ruk & Nhar"},//68
     {"Hechicera Nalyivia","Sorceress Nalyivia"},
     {"Alquimista Karah","Alchemist Karah"},//70
     {"Nigromante Rhilik","Necromancer Rhilik"},
     {"Asesino Ragh'tul","Assassin Ragh'tul"},//72
     {"Rey Modrean","King Modrean"},//73
     {"No se quien eres pero tenemos ordenes explicitas de matar a cualquiera que intente pasar.","I don't know who you are but we have explicit orders to kill anyone who tries to pass."},//74
     {"¡Has conseguido el Alma de la Sabiduria!","You've got the Soul of Wisdom!"},//75
     {"¡Has conseguido el Alma del Valor!","You've got the Soul of Courage!"},//76
     {"¡Has conseguido el Alma del Poder!","You've got the Soul of Power!"},//77
     {"¿Vienes a interrumpir mis experimentos? Quizá pueda usarte a ti como sujeto de prueba.","Are you coming to interrupt my experiments? Maybe i can use you as a test subject."},//78
     {"Puedes haber derrotado a mis magos, pero no podras derrotarme con una espada. Preparate para arder.","You may have defeated my wizards, but you can not defeat me with a sword. Prepare to burn."},
     {"Has llegado hasta mi, parece que no solo eres valiente, tambien eres estupido, no tienes chances contra mis esqueletos y mis poderes.","You have come to me, it seems you are not only brave, you are also stupid, you have no chance against my skeletons and my powers."},//80
     {"El rey Modrean me ha llamado a mi, su mejor asesino, para encargarme de ti. Tengo que reconocer que has causado demasiados disturbios, pero no te preocupes, te daré una muerte rapida.","King Modrean has called me, his best assassin, to take care of you. I have to admit that you have caused too much trouble, but do not worry, I will give you a quick death."},
     {"Estas cerca de la sala del trono, pero para tu desgracia te estaba esperando. No creas que he llegado a ser general dando ordenes solamente.","You are near the throne room, but for your misfortune I was expecting you. Do not think I have become a general by giving orders only."},//82
     {"Finalmente, has llegado. No puedo creer que hayas matado a mis mejores soldados. ¡Esos inutiles! Tendré que terminar con esto personalmente. Una vez que mueras terminará la revelión en el pueblo y yo seguiré reinando."," Finally, you have arrived. I can not believe you killed my best soldiers. Those useless! I will have to end this personally. Once you die the rebellion in the village will end and I will continue to reign."},
     {"¡Maldito! No pienso morir aqui...","No! I'm not going to die here ..."},//84
     {"¡No pienso rendirme, el calor del magma aumenta mi poder!","I won't give up, the heat of magma increases my power!"},
     {"Zona de sirvientes","Servant's area"},//86
     {"Entrada al castillo","Entrance to the castle"},
     {"Jardines Reales","Royal Gardens"},//88
     {"Biblioteca","Library"},
     {"Comedor central","Central dinning hall"},//90
     {"Mazmorras","Dungeons"},
     {"Patio de armas","Central square"},//92
     {"Puedes pausar el juego en cualquier momento tocando el boton de pausa en el extremo superior izquierdo.","You can pause the game at any time by tapping the pause button in the upper left corner."},
     {"Dentro del menu de pausa puedes seleccionar la opción inventario para gestionar los objetos que tienes equipados y en el inventario.","In the pause menu you can select the backpack option to manage the objects that you have equipped and in your backpack."},
     {"Para atacar a los enemigos selecciona la habilidad con el simbolo de la espada en la barra inferior. Cuando esta habilidad esté seleccionada, podrás atacar en la direccion deseada. Para moverte recuerda volver a cambiar a la habilidad con el simbolo de pies.","To attack the enemies select the skill with the sword symbol in the bottom bar. When this skill is selected, you can attack in the desired direction. To move remember remember to change to the skill with the symbol of feet."},
     {"A veces encontrarás cofres ocultos los cuales otorgan oro y objetos. Debes destruirlos como si fuesen enemigos.","Sometimes you will find hidden chests which give gold and objects. You must destroy them as if they were enemies."},
     {"Algunos enemigos utilizan armas diferentes, intenta buscar la mejor forma de matarlos recibiendo la menor cantidad de daño.","Some enemies use different weapons, try to find the best way to kill them receiving the least amount of damage."},
     {"Para avanzar al siguiente mapa simplemente muevete al borde donde termina el camino.","To advance to the next map simply move to the edge where the road ends."},
     {"Príncipe, soy el espía Damaren, serví a tu padre, el verdadero rey, hace muchos años hasta su fin. Quiero ayudarte a recuperar tu imperio, no tengo habilidades de combate pero puedo escabullirme y conseguir información. Voy a adelantarme e intentaré conseguir información de los sirvientes. Buscame cuando estés adentro del castillo.", "Prince, I am the spy Damaren, I served your father, the true king, many years to the end. I want to help you regain your empire, I have no combat skills but I can sneak and get information. I will go ahead and try to get information from the servants. Seek me when you are inside the castle. "},
     {"No puedo volver...","I can't go back..."},//100
     {"Parece ser un antiguo estandarte destruido del reino de mi padre.","It seems to be a destroyed banner from my father's kingdom."},
     {"Es un estandarte del rey usurpador Modrean. Debo estar cerca de la entrada al castillo...","It is a banner of the usurper king Modrean. I must be near the entrance to the castle ..."},
     {"Parece que te diriges al castillo, no esta lejos de aqui pero ten cuidado, la entrada esta custodiada por los soldados del rey Modrean. Los soldados de Modrean están mejor entrenados que los bandidos que merodean este bosque.","Looks like you're heading for the castle, it's not far from here, but be careful, the entrance is guarded by the soldiers of King Modrean. Modrean's soldiers are better trained than the bandits who roam this forest."},
     {"Vaya, si es el príncipe, nunca deberías haber aparecido por aquí, este castillo ya no te pertenece. Pero bueno, el rey estará feliz de que le lleve tu cabeza. Este es tu fin...","Look who's here, the prince, you shouldn't have come here, this castle does not belong to you anymore, but the king will be happy if i bring him your head, this is your end..."},
     {"Mi compañero vende y compra objetos, toca sobre él para comerciar. Puedes encontrar otros comerciantes en otras partes del castillo pero recuerda que muchos estarán escondiendose de los guardias.","My partner sells and buys objects, touch over him to trade. You can find other merchants in other parts of the castle but remember that many will be hiding from the guards."},
     {"¿Deseas comprar algo?","¿Do you want to trade?"},//106
     {"¡Realmente eres tu! Nos llegó el rumor de un disturbio en la entrada, me alegro de que vengas a recuperar lo que te pertenece. Recuerda hablar con los personajes que tengan un signo encima de su cabeza, te dirán cosas útiles, y hasta algunos podrían comerciar contigo!","It's really you! There was a rumor of a disturbance at the entrance, I'm glad you're coming to retake what belongs to you. Remember to talk to characters with a sign above their head, they could tell you useful things, and some may even trade with you!"},
     {"¡Exelente, has conseguido entrar! He hablado con algunos sirvientes, parece que tienes muchos aliados todavía. Debes seguir avanzando, yo seguiré intentando recolectar información.","Excellent, you got in! I've talked to some servants, you seem to have a lot of allies, you have to keep going, I'll keep trying to collect information."},
     {"Los rumorer corren rápido, los guardias se han enterado de tu presencia y han cerrado esta puerta, debes buscar otro camino.","Rumors spread fast, the guards have heard of your presence and have closed this door, you must look for another way."},
     {"Aqui puedo descansar sin que los guardias me molesten.","Here I can rest without the guards bothering me."},
     {"¿Tu tambien buscas evitar los guardias? Ellos no se acercan mucho a estos depositos.","Do you also try to avoid the guards? They do not get too close to these warehouses."},
     {"No molestes, intento descansar.","Do not bother, I try to rest."},//112
     {"Esta puerta parece que esta controlada por un extraño mecanismo. Intenta tocar los pedestrales para cambiar los simbolos en la pared.","This door seems to be controlled by a strange mechanism. Try to activate the pedestals to change the symbols on the wall."},
     {"Ten cuidado, aquí delante se encuentra la casa de la Alquimista Karah. Si piensas enfrentarte a ella debes tener cuidado lance sus pociones. No conozco todas sus pociones pero cuando lance la pocion azul no debes moverte, cuando lance la roja debes moverte sin parar.","Be careful, here is the house of the Alchemist Karah.If you plan to face her, you must be careful when she throws her potions.I do not know all her potions but when she releases the blue potion you must not move, when she releases the red one you must move without stopping ."},
     {"Eres bastante famoso ahora mismo, los guardias te buscan por todo el castillo. Ten cuidado si vas para la biblioteca, alli se encuentra la Hechicera Nalyvia, la cual es temida incluso entre los mejores magos.","You are quite famous right now, the guards are looking for you all over the castle. Be careful if you go to the library, there is the Sorceress Nalyvia, which is feared even among the best mages."},
     {"Me has salvado. Soy el antiguo Jefe de investigación, serví a tu padre durante su reinado. Gracias a que has derrotado a la hechicera Nalyivia, se ha roto el hechizo que me tenía atrapado. Voy a ayudarte a recuperar el poder, pero necesito recuperarme primero.","You saved me. I'm the old Chief of Investigation, I served your father during his reign. Thanks you have defeated the sorceress Nalyivia, the spell that trapped me has been broken. I'm going to help you regain power, but I need to recover myself first."},
     {"¿Deseas hacer apuestas? Puedo darte un objeto adecuado para tu nivel de cualquier calidad al azar por una suma de dinero.","Do you want to gamble? I can give you a suitable object for your level of any random quality for a sum of money."},
     {"La unica forma de abrir esta puerta, es activando los pedestrales. Estos se activan automaticamente cuando consigas el Alma de la Sabiduria, el Alma del Poder y el Alma del Valor. Tengo entendido que el alma de la Sabiduria esta en alguna parte de los Jardines Imperiales, el Alma de Poder en la Biblioteca y el Alma del Valor es las Mazmorras.", "The only way to open this door is to activate the pedestrals, which are automatically activated when you get the Soul of Wisdom, the Soul of Power and the Soul of Courage. I understand that the Soul of Wisdom is somewhere in the Royal Gardens, the Soul of Power is in the Library and the Soul of Courage is in the Dungeons."},
     {"¡Bien hecho! Cada vez estamos más cerca de llegar a la sala del trono, pero no bajes la guardia.", "Well done! We are getting closer to the throne room, but do not let your guard down."},
     //120
     {"El pedestral esta desactivado. Necesito conseguir el Alma de Sabiduria.","The pedestal is disabled. I need to get the Soul of Wisdom."},
     {"El pedestral brilla fuertemente. Tengo conmigo el Alma de Sabiduria.","The pedestal shines strongly. I have the Soul of Wisdom with me."},
     {"El pedestral esta desactivado. Necesito conseguir el Alma de Poder.","The pedestal is disabled. I need to get the Soul of Power."},
     {"El pedestral brilla fuertemente. Tengo conmigo el Alma de Poder.","The pedestal shines strongly. I have the Soul of Power with me."},
     {"El pedestral esta desactivado. Necesito conseguir el Alma de Valor.","The pedestal is disabled. I need to get the Soul of Courage."},
     {"El pedestral brilla fuertemente. Tengo conmigo el Alma de Valor.","The pedestal shines strongly. I have the Soul of Courage with me."},
     {"Me has salvado. Soy el antiguo comandante del ejercito real, serví a tu padre durante su reinado. Gracias a que has derrotado a ese maldito nigromante he logrado escapar de mi celda. Voy a ayudarte a recuperar el poder, pero necesito recuperarme primero.","You have saved me. I am the old commander of the royal army, I served your father during his reign. Thanks you have defeated that damn necromancer, I have managed to escape from my cell. I will help you regain power, but I need to recover first."},
     {"Este lugar me da escalofrios. Dicen que Rhilik, quien dirige estas mazmorras, es un nigromante que puede controlar a los muertos.","This place gives me chills. They say that Rhilik, who runs these dungeons, is a necromancer who can control the dead."},
     {"Cuenta la leyenda que un tesoro se encuentra detras de esa puerta. Pero el mecanismo parece imposible de decifrar, he intentado más de 100 veces sin éxito, ojalá tu tengas suerte...","The legend says that a treasure is behind that door, but the mechanism seems impossible to decipher, I have tried more than 100 times without success, i hope you are lucky..."},
     {"Finalmente, el salon del trono se encuentra cerca, sin embargo es facil perderse en estas salas.","Finally, the throne room is nearby, yet it is easy to get lost in these rooms."},
     //130
     {"Estas salas pueden parecer confusas, están diseñadas así para evitar que los invasores se infiltren, pero con un poco de memoria puedes recordar el camino.", "These rooms may seem confusing, they are designed to prevent intruders from infiltrating, but with a little memory you can remember the way."},
     {"Finalmente ha llegado la hora de enfrentar tu destino. Modrean te espera al final de la sala.","Finally the time has come to face your destiny. Modrean awaits you at the end of the room."},
     {"Nos hemos encargado de los guardias personales de Modrean, solamente queda él.","We have taken care of the personal guards of Modrean, only he remains."},
     {"¡Rapido, Modrean intenta escaparse por esa cueva!","Quick, Modrean tries to escape through that cave!"},
     {"Modrean no puede estar muy lejos, sin embargo desconozco a donde conduce esta antigua cueva.","Modrean can not be far, yet I do not know where this ancient cave is leading."},
     {"Siento un extraño calor que viene de delante, ten cuidado.","I feel a strange heat coming from the front, be careful."},
     {"Vamos, es el momento de que le des el golpe final, y termines con lo que empezaste.","Come on, it's time for you to give him the final blow, and end with what you started."},
     {"¡Lo has logrado! Modrean finalmente ha muerto. Eres ahora el nuevo REY, darás inicio a una nueva epoca de paz y prosperidad.","You've made it! You are now the new KING, you will start a new era of peace and prosperity."},
     {"Estas parecen ser las tumbas de tus antepasados, los cuales construyeron este castillo. Quien iba a saber que sus tumbas se encontraban en una cueva olvidada detras de la sala del trono.","These seem to be the tombs of your ancestors, who built this castle. Who would know that their tombs were in a forgotten cave behind the throne room?"},
     {"Ahora que Modrean ha caído, los soldados ya no tienen que cumplir ordenes, tienes al ejercito a tus pies.","Now that Modrean has fallen, the soldiers no longer have to fulfill orders, you have the army at your feet."},
     //140
     {"¡Felicidades, has completado el juego!\nAhora puedes iniciar el juego en dificultad Extrema.","Congratulations, you have completed the game!\nNow you can start the game in Extreme difficulty."},
     {"No tienes mas espacio en el inventario, no recibiras objetos nuevos. Ve a un comerciante y vende lo que no necesites.","You do not have free space left in your backpack, you will not receive new items. Go to a merchant and sell what you do not need."},
     {"¡Has muerto!\n\nExperiencia perdida: ","¡You are dead!\n\nLost experience: "},
     {"\n\nOro perdido: ","\n\nLost gold: "},
     {"Has llegado a nivel 5! Ahora puedes aprender una habilidad nueva. Para aprenderla debes pausar el juego e ir al menu de habilidades.","You have now reached level 5. You can now learn a new skill, you must pause the game and go to the skills menu to learn it."},
     {"Armadura","Armor"},
     {"Reducción de enfriamiento","Cooldown reduction"},
     {"Daño","Damage"},
     {"Esquivar","Dodge"},
     {"Golpe critico","Critical hit"},
     //150
     {"Vel. de movimiento","Walking speed"},
     {"Raro","Rare"},
     {"Épico","Epic"},
     {"Legendario","Legendary"},
     {"Ranura ","Slot "},
     {"¿Esta seguro que desea eliminar la partida guardada?","Are you sure you want to delete the saved game?"},
     {"Salas del Rey","King's Rooms"},
     {"esquivado","miss"},
     { "¡Lo has logrado príncipe! Ten cuidado, presiento algo malo delante, tomaré un camino alternativo, te contactaré cuando obtenga información.", "You've made it! Be careful, i sense something wrong in the next room, i'm going to take an alternative path, I will contact you when i get information." },
     {"Bienvenido, para moverte pulsa en la pantalla en el punto donde quieras ir.","Welcome, to move the character touch the place where you want to go."},
     //160
     {"Confirmar","Confirm"},
     {"¿Desea cambiar a dificultad Extrema?\n(Usted comenzara el juego desde el principio\nen la nueva dificultad manteniendo el personaje.)","Do you want to change the difficulty to extreme?\n(You will start the game from the beginning in a harder difficulty\nbut you keep the character.)"},
     {"Ya ganaste el juego en la dificultad maxima.\nPuedes comenzar el juego otra vez en la misma dificultad\nmanteniendo el personaje.","You already won the game in the maximium difficulty.\nYou can start again the game in the same difficulty\nand keeping the character."},
     {"Más","More"},
     {"Texturas diseñadas por Daniel Cook (Lostgarden.com)","Map textures designed by Daniel Cook (Lostgarden.com)"},
     {"Personajes creados utilizando el programa: \"Universal LPC Sprite Sheet\".","Characters created using the \"Universal LPC Sprite Sheet\" program."},
     {"Autores de \"Universal LPC Sprite Sheet\": ","\"Universal LPC Sprite Sheet\" authors: "},
     {"",""},
     {"",""}

    };

    public static string getTexto(int index)
    {
        //Debug.Log(index);
        return t[index, idioma];
    }
}
