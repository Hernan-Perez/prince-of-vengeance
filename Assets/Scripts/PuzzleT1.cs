using UnityEngine;
using System.Collections.Generic;

public sealed class PuzzleT1 : Puzzle
{

    private List<Vector2> mRelojes;
    private List<bool> condMRelojes;

    private List<Vector2> mecanismos;

    //PUERTA U OTRO QUE ESTA ACTIVO MIENTRAS EL MECANISMO NO ESTE ACTIVADO, DESPUES DESAPARECE ESTO
    private List<short> texCerrado;
    private List<short> texANTCerrado;
    private List<Vector2> posCerrado;
    private List<bool> obsCerrado;
    private List<bool> obsANTCerrado;   //ESTO ES PARA DEVOLVER LA CONDICION INICIAL, SI UNA PUERTA ESTA CERRADA PARA QUE DESPUES SE PEUDA PASAR

    private bool _desactivado;
    private bool _ext = false;

    //CLAVE EXTENDIDO: 2-3-5-7-8
    public PuzzleT1(int codigo, bool extendido = false)
    {
        cod = codigo;
        _desactivado = refGame.puzzleResuelto[cod];
        mRelojes = new List<Vector2>();
        condMRelojes = new List<bool>();
        mecanismos = new List<Vector2>();

        texCerrado = new List<short>();
        texANTCerrado = new List<short>();
        posCerrado = new List<Vector2>();
        obsANTCerrado = new List<bool>();
        obsCerrado = new List<bool>();
        _ext = extendido;
    }

    public override void Update()
    {
        if (_desactivado)
            return;
    }

    public override void Draw()
    {
        if (_desactivado)
            return;
    }

    public override bool Touch(Vector2 pos)
    {
        if (_desactivado)
            return false;
        
        for (int i = 0; i < mecanismos.Count; i++)
        {
            if (mecanismos[i] == pos)
            {
                refControl.PlaySonido(0);
                refControl.touchLock = true;
                //aca tiene que alterar los relojes segun el numero de mecanismo

                if (!_ext)
                {
                    switch (i)
                    {
                        case 0:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            break;
                        case 1:
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            break;
                        case 2:
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            condMRelojes[3] = intercambiar(condMRelojes[3]);
                            break;
                        case 3:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            //condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            //condMRelojes[4] = intercambiar(condMRelojes[4]);
                            //condMRelojes[5] = intercambiar(condMRelojes[5]);
                            condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 1:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            //condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            condMRelojes[4] = intercambiar(condMRelojes[4]);
                            //condMRelojes[5] = intercambiar(condMRelojes[5]);
                            condMRelojes[6] = intercambiar(condMRelojes[6]);
                            //condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 2:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            //condMRelojes[1] = intercambiar(condMRelojes[1]);
                            //condMRelojes[2] = intercambiar(condMRelojes[2]);
                            condMRelojes[3] = intercambiar(condMRelojes[3]);
                            condMRelojes[4] = intercambiar(condMRelojes[4]);
                            //condMRelojes[5] = intercambiar(condMRelojes[5]);
                            //condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 3:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            condMRelojes[3] = intercambiar(condMRelojes[3]);
                            //condMRelojes[4] = intercambiar(condMRelojes[4]);
                            condMRelojes[5] = intercambiar(condMRelojes[5]);
                            condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 4:
                            //condMRelojes[0] = intercambiar(condMRelojes[0]);
                            //condMRelojes[1] = intercambiar(condMRelojes[1]);
                            //condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            condMRelojes[4] = intercambiar(condMRelojes[4]);
                            condMRelojes[5] = intercambiar(condMRelojes[5]);
                            condMRelojes[6] = intercambiar(condMRelojes[6]);
                            //condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 5:
                            //condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            //condMRelojes[4] = intercambiar(condMRelojes[4]);
                            condMRelojes[5] = intercambiar(condMRelojes[5]);
                            //condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 6:
                            condMRelojes[0] = intercambiar(condMRelojes[0]);
                            //condMRelojes[1] = intercambiar(condMRelojes[1]);
                            //condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            //condMRelojes[4] = intercambiar(condMRelojes[4]);
                            //condMRelojes[5] = intercambiar(condMRelojes[5]);
                            //condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                        case 7:
                            //condMRelojes[0] = intercambiar(condMRelojes[0]);
                            condMRelojes[1] = intercambiar(condMRelojes[1]);
                            //condMRelojes[2] = intercambiar(condMRelojes[2]);
                            //condMRelojes[3] = intercambiar(condMRelojes[3]);
                            //condMRelojes[4] = intercambiar(condMRelojes[4]);
                            //condMRelojes[5] = intercambiar(condMRelojes[5]);
                            condMRelojes[6] = intercambiar(condMRelojes[6]);
                            condMRelojes[7] = intercambiar(condMRelojes[7]);
                            break;
                    }
                }
                
                _actualizar();
                return true;
            }
        }
        return false;
    }

    private void _actualizar()  //se llama despues de un cambio en touch. el otro update esta al pedo
    {
        int index;
        Vector2 pos;
        for (int i = 0; i < mRelojes.Count; i++)
        {
            pos = mRelojes[i];
            index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
            refGame.currentMapa._layer0[index] = (short)((condMRelojes[i]) ? (141) : (140));
        }
        if (estanRelojesActivados())
        {
            Desactivar();
        }
    }


    public void AgregarRelojMecanismo(Vector2 pos)
    {
        int index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
        mRelojes.Add(pos);
        refGame.currentMapa._layer0[index] = (short)((_desactivado) ? (141) : (140));    //SOLO FUNCIONA EN TILESET INTERIORES
        condMRelojes.Add(false);
    }

    public void AgregarControlMecanismo(Vector2 pos)
    {
        int index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
        mecanismos.Add(pos);
        refGame.currentMapa._layer0[index] = 139;    //SOLO FUNCIONA EN TILESET INTERIORES
        refGame.currentMapa._mundoObstaculos[index] = true;
    }

    public void AgregarZonaCerrada(short tex, Vector2 pos, bool obs)
    {
        if (_desactivado)
            return;
        int index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
        texCerrado.Add(tex);
        texANTCerrado.Add(refGame.currentMapa._layer2[index]);
        refGame.currentMapa._layer2[index] = tex;
        posCerrado.Add(pos);
        obsANTCerrado.Add(refGame.currentMapa.esPosObstaculo((int)(pos.x + pos.y * refGame.currentMapa.DIMX))); //guarda la anterior condicion obs
        obsCerrado.Add(obs);    //por ahora se guarda al pedo pero puede llegar a servir despues
        refGame.currentMapa._mundoObstaculos[index] = obs;
    }

    private bool intercambiar(bool cond)
    {
        if (cond)
            return false;
        return true;
    }

    private bool estanRelojesActivados()
    {
        for (int i = 0; i < condMRelojes.Count; i++)
        {
            if (condMRelojes[i] == false)
            {
                return false;
            }
        }
        return true;
    }

    private void Desactivar()
    {
        if (_desactivado)
            return;
        _desactivado = true;
        refGame.puzzleResuelto[cod] = true;
        int index = 0;
        Vector2 pos;
        for (int i = 0; i < texCerrado.Count; i++)
        {
            pos = posCerrado[i];
            index = (int)(pos.x + pos.y * refGame.currentMapa.DIMX);
            refGame.currentMapa._layer2[index] = texANTCerrado[i];
            refGame.currentMapa._mundoObstaculos[index] = obsANTCerrado[i];
        }

        if (!_ext)
        {
            refGame.hud.AgregarTextoConversacion(CONFIG.getTexto(158));
            refGame._npcArray = null;
        }


    }
}
