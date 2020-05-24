using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMaster
{
    public static readonly string DOG_MOVE_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Dog/ControllerBackGround";
    public static readonly string CAT_MOVE_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Cat/ControllerBackGround";
    public static readonly string ATTACK_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Dog/AttackButton/AttackController";
    public static readonly string SWEET_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Dog/SweetButton/SweetController";
    public static readonly string SENSOR_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Dog/SensorButton/SensorController";
    public static readonly string SKILL_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Dog/SkillButton/SkillController";
    public static readonly string JUMP_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Cat/JumpButton/JumpController";
    public static readonly string DASH_SKILL_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Cat/DashSkillButton/DashSkillController";
    public static readonly string DECOY_SKILL_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Cat/DecoySkillButton/DecoySkillController";
    public static readonly string SMOKE_SKILL_CONTROLLER = "Canvas/MainController/TouchControllers/PlayerMoveController_Cat/SmokeSkillButton/SmokeSkillController";
    public static readonly string PLAYER_PREFAB_ROOT = "prefabs/Player/";

    #region collection
    public static readonly string ITEM_ROOT = "Design/Home/Other/Items/";

    #endregion

    #region character
    public static readonly string CHARACTER_ICON_ROOT = "Design/Home/Other/Character/";

    #endregion
}
