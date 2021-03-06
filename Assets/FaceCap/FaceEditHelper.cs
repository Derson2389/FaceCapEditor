﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class FaceEditHelper
{

    public static float PROXIMITY_TOLERANCE = 0.001f;

    public static string[] facial_list =
	{
		"brow_list",
		"eye_list",
		"cheek_list",
		"mouth_list",
		"other_list",
		"kouxing_list"
	};


    public enum BrowCtrl
    {
        //brow list
        r_out_brow_facialControl,
        r_mid_brow_facialControl,
        r_in_brow_facialControl,
        l_out_brow_facialControl,
        l_mid_brow_facialControl,
        l_in_brow_facialControl,
        r_brow_move_facialControl,
        l_brow_move_facialControl,
    }
    public static string[] BrowCtrlName =
    {
        "r_out_brow_facialControl",
        "r_mid_brow_facialControl",
        "r_in_brow_facialControl",
        "l_out_brow_facialControl",
        "l_mid_brow_facialControl",
        "l_in_brow_facialControl",
        "r_brow_move_facialControl",
        "l_brow_move_facialControl",
    };

    public enum EyeListCtrl
    {
        //eye_list
        r_upper_eyelid_facialControl,
        r_lower_eyelid_facialControl,
        l_upper_eyelid_facialControl,
        l_lower_eyelid_facialControl,
        r_eyeBall_facialControl,
        l_eyeBall_facialControl,
        r_pupil_scale_facialControl,
        l_pupil_scale_facialControl,
        r_eyeHightLight_move_facialControl,
        l_eyeHightLight_move_facialControl,
        r_eyeHightLight_scale_facialControl,
        l_eyeHightLight_scale_facialControl,
        r_eyelid_blink_facialControl,
        l_eyelid_blink_facialControl,
    }
    public static string[] EyeListCtrlName =
    {
        //eye_list
		"r_upper_eyelid_facialControl",
        "r_lower_eyelid_facialControl",
        "l_upper_eyelid_facialControl",
        "l_lower_eyelid_facialControl",
        "r_eyeBall_facialControl",
        "l_eyeBall_facialControl",
        "r_pupil_scale_facialControl",
        "l_pupil_scale_facialControl",
        "r_eyeHightLight_move_facialControl",
        "l_eyeHightLight_move_facialControl",
        "r_eyeHightLight_scale_facialControl",
        "l_eyeHightLight_scale_facialControl",
        "r_eyelid_blink_facialControl",
        "l_eyelid_blink_facialControl"

    };

    public enum CheekListCtrl
    {
        //eye_list
        r_cheek_facialControl,
        l_cheek_facialControl,
        nose_facialControl,
        r_nose_facialControl,
        l_nose_facialControl,
    }
    public static string[] CheekListCtrlName =
   {
        //cheek_list
		"r_cheek_facialControl",
        "l_cheek_facialControl",
        "nose_facialControl",
        "r_nose_facialControl",
        "l_nose_facialControl",
   };

    public enum MouthCtrl
    {
        //mouth_list
        tongue_facialControl,
        jaw_facialControl,
        r_corners_facialControl,
        l_corners_facialControl,
        upper_lip_facialControl,
        lower_lip_facialControl,
        r_upper_lip_facialControl,
        r_lower_lip_facialControl,
        l_upper_lip_facialControl,
        l_lower_lip_facialControl,
        r_upper_corners_facialControl,
        r_lower_corners_facialControl,
        l_upper_corners_facialControl,
        l_lower_corners_facialControl,
        mouth_rotate_facialControl,
        mouth_move_facialControl,
    }
    public static string[] MouthCtrlName =
    {
        //mouth_list
        "tongue_facialControl",
        "jaw_facialControl",
        "r_corners_facialControl",
        "l_corners_facialControl",
        "upper_lip_facialControl",
        "lower_lip_facialControl",
        "r_upper_lip_facialControl",
        "r_lower_lip_facialControl",
        "l_upper_lip_facialControl",
        "l_lower_lip_facialControl",
        "r_upper_corners_facialControl",
        "r_lower_corners_facialControl",
        "l_upper_corners_facialControl",
        "l_lower_corners_facialControl",
        "mouth_rotate_facialControl",
        "mouth_move_facialControl",

    };

    public enum MouthShape
    {
        //kouxing_list
        //A
        A_facialControl,
        //E
        E_facialControl,
        //I
        I_facialControl,
        //O
        O_facialControl,
        //U
        U_facialControl,
        //F
        F_facialControl,
        //E
        M_facialControl,
    }
    public static string[] MouthShapeCtrlName =
    {   
		//kouxing_list
		//A
		"A_facialControl",
		//E
		"E_facialControl",
		//I
		"I_facialControl",
		//O
		"O_facialControl",
		//U
		"U_facialControl",
		//F
		"F_facialControl",
		//E
		"M_facialControl",

    };

    public enum OtherCtrl
    {
        // other_list
        upper_teeth_facialControl,
        lower_teeth_facialControl,
        add_facialControl,
        add01_facialControl,
        add02_facialControl,
        add03_facialControl,

    }

    public static string[] OtherCtrlName =
    {
        // other_list
        "upper_teeth_facialControl",
        "lower_teeth_facialControl",
        "add_facialControl",
        "add01_facialControl",
        "add02_facialControl",
        "add03_facialControl",
    };


    public enum ControllerType          
   {
		SyncType_X ,
		SyncType_Y ,
		SyncType_XY,
	}

}

