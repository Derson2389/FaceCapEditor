using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class FaceEditHelper
{
	public enum FaceController
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

		//cheek_list
		r_cheek_facialControl,
		l_cheek_facialControl,
		nose_facialControl,
		r_nose_facialControl,

		//mouth_list
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

		// other_list
		tongue_facialControl,
		jaw_facialControl,
		upper_teeth_facialControl,
		lower_teeth_facialControl,
		add_facialControl,

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


	public static string[] FaceControllerNames =
	{
		//brow list
		"r_out_brow_facialControl",
		"r_mid_brow_facialControl",
		"r_in_brow_facialControl",
		"l_out_brow_facialControl",
		"l_mid_brow_facialControl",
		"l_in_brow_facialControl",
		"r_brow_move_facialControl",
		"l_brow_move_facialControl",

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

		//cheek_list
		"r_cheek_facialControl",
		"l_cheek_facialControl",
		"nose_facialControl",
		"r_nose_facialControl",

		//mouth_list
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

		// other_list
		"tongue_facialControl",
		"jaw_facialControl",
		"upper_teeth_facialControl",
		"lower_teeth_facialControl",
		"add_facialControl",

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

	public static string[] facial_list =
	{
		"brow_list",
		"eye_list",
		"cheek_list",
		"mouth_list",
		"other_list",
		"kouxing_list"
	};

	public enum SyncType          
   {
		SyncType_X ,
		SyncType_Y ,
		SyncType_XY,
	}

	public class FaceControllerMod
	{
		public string controllerName = string.Empty;
		public List<string> blenderShaps = new List<string>();
		public SyncType controllerType = SyncType.SyncType_X;
	}




}

