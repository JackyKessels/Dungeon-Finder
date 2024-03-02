using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardHandler
{
    // --- General --- //
    public static bool Escape()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
    public static bool ProgressWindow()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }
    public static bool CompareTooltip()
    {
        return Input.GetKeyDown(KeyCode.LeftControl);
    }
    public static bool StopCompareTooltip()
    {
        return Input.GetKeyUp(KeyCode.LeftControl);
    }

    // --- UI --- //
    // Menus //
    public static bool PreviousPage()
    {
        return Input.GetKeyDown(KeyCode.A);
    }

    public static bool NextPage()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public static bool OpenHeroInformationGeneral()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public static bool OpenHeroInformationPaths()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public static bool OpenMap()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public static bool OpenAbilityShop()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public static bool OpenGlyphShop()
    {
        return Input.GetKeyDown(KeyCode.T);
    }

    public static bool OpenCodex()
    {
        return Input.GetKeyDown(KeyCode.C);
    }

    //public static bool OpenItemShop()
    //{
    //    return Input.GetKeyDown(KeyCode.T);
    //}




    // --- Battle --- //
    // Abilities //
    public static bool CastAbility1()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public static bool CastAbility2()
    {
        return Input.GetKeyDown(KeyCode.W);
    }

    public static bool CastAbility3()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public static bool CastAbility4()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    // Items //

    public static bool UseItem1()
    {
        return Input.GetKeyDown(KeyCode.Alpha1);
    }

    public static bool UseItem2()
    {
        return Input.GetKeyDown(KeyCode.Alpha2);
    }

    public static bool UseItem3()
    {
        return Input.GetKeyDown(KeyCode.Alpha3);
    }

    public static bool UseItem4()
    {
        return Input.GetKeyDown(KeyCode.Alpha4);
    }

    public static bool UseItem5()
    {
        return Input.GetKeyDown(KeyCode.Alpha5);
    }

    public static bool UseItem6()
    {
        return Input.GetKeyDown(KeyCode.Alpha6);
    }

    public static bool UseItem7()
    {
        return Input.GetKeyDown(KeyCode.Alpha7);
    }

    public static bool UseItem8()
    {
        return Input.GetKeyDown(KeyCode.Alpha8);
    }
    
    // Other //

    public static bool UseFlask()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    public static bool PassTurn()
    {
        return Input.GetKeyDown(KeyCode.X);
    }

    // --- Features --- //

}
