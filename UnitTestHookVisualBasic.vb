' API requirement to function

#Const EXT_IUTIL = 1
#Const EXT_ICINIFILE = 1
#Const EXT_ICOMMAND = 1

' Future API support

'#Const EXT_HKEXTERNAL = 1          // TBD

#If DO_NOT_INCLUDE_THIS Then
addon_info EXTPluginInfo = { "UnitTest Hook Visual Basic", "1.0.0.0",
                            "DZS|All-In-One, founder of DZS",
                            "Used for verifying each hook API are working right in VB language under C99 standard.",
                            "UnitTestHook",
                            "unit_test",
                            "test_unit",
                            "unit test",
                            "[unit]test",
                            "test[unit]"};
#End If

Imports System.Text

Imports RGiesecke.DllExport
Imports System.Runtime.InteropServices

Namespace UnitTestHookCSharp
    Public Class Addon
        Public Shared EAOhashID As UInteger
#If EXT_ICINIFILE Then
        Public Shared iniFileStr As String = "HookTestVisualBasic.ini"
        Public Shared pICIniFile As Addon_API.ICIniFileClass
        Public Shared pIUtil As Addon_API.IUtil
        Public Shared pICommand As Addon_API.ICommand
#End If
        <DllExport("EXTOnEAOLoad", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnEAOLoad(uniqueHash As UInteger) As Addon_API.EAO_RETURN
            EAOhashID = uniqueHash
            pICIniFile = Addon_API.[Interface].getICIniFile(EAOhashID)
            pIUtil = Addon_API.[Interface].getIUtil(EAOhashID)
            pICommand = Addon_API.[Interface].getICommand(EAOhashID)

            If Not pICIniFile.isNotNull() Then
                GoTo initFail
            End If
            If pICIniFile.m_open_file(iniFileStr) Then
                If Not pICIniFile.m_delete_file(iniFileStr) Then
                    GoTo initFail
                End If
                If pICIniFile.m_open_file(iniFileStr) Then
                    GoTo initFail
                End If
            End If
            If Not pICIniFile.m_create_file(iniFileStr) Then
                GoTo initFail
            End If
            If Not pICIniFile.m_open_file(iniFileStr) Then
                GoTo initFail
            End If

            ' This is needed in order to preserve function pointer address
            eao_unittesthook_savePtr = AddressOf eao_unittesthook_save
            GC.KeepAlive(eao_unittesthook_savePtr)

            If Not pICommand.m_add(EAOhashID, eao_unittesthook_saveStr, eao_unittesthook_savePtr, "unit_test", 1, 1,
                False, HEXT.modeAll) Then
                GoTo initFail
            End If

            Return Addon_API.EAO_RETURN.OVERRIDE
initFail:
            If pICIniFile.isNotNull() Then
                pICIniFile.m_release()
            End If
            If pICommand.isNotNull() Then
                pICommand.m_delete(EAOhashID, eao_unittesthook_savePtr, eao_unittesthook_saveStr)
            End If
            GC.Collect()
            Return Addon_API.EAO_RETURN.FAIL
        End Function

        <DllExport("EXTOnEAOUnload", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnEAOUnload()
            If pICIniFile.isNotNull() Then
                pICIniFile.m_save()
                pICIniFile.m_close()
                pICIniFile.m_release()
            End If
            If pICommand.isNotNull() Then
                pICommand.m_delete(EAOhashID, eao_unittesthook_savePtr, eao_unittesthook_saveStr)
            End If
            GC.Collect()
        End Sub
        Public Shared eao_unittesthook_saveStr As String = "eao_unittesthook_save_visualbasic"
        'This is needed in order to preserve function pointer address
        Public Shared eao_unittesthook_savePtr As Addon_API.CmdFunc
        Public Shared Function eao_unittesthook_save(<[In]> plI As Addon_API.PlayerInfo, <[In], Out> ByRef arg As Addon_API.ArgContainerVars, <[In]> protocolMsg As Addon_API.MSG_PROTOCOL, <[In]> idTimer As UInteger, <[In], Out> showChat As boolOption) As Addon_API.CMD_RETURN
            If pICIniFile.isNotNull() Then
                pICIniFile.m_save()
            End If
            Return Addon_API.CMD_RETURN.SUCC
        End Function

        Public Shared HookNames As String() = {
            "EXTOnPlayerJoin",
            "EXTOnPlayerQuit",
            "EXTOnPlayerDeath",
            "EXTOnPlayerChangeTeamAttempt",
            "EXTOnPlayerJoinDefault",
            "EXTOnNewGame",
            "EXTOnEndGame",
            "EXTOnObjectInteraction",
            "EXTOnWeaponAssignmentDefault",
            "EXTOnWeaponAssignmentCustom",
            "EXTOnServerIdle",
            "EXTOnPlayerScoreCTF",
            "EXTOnWeaponDropAttemptMustBeReadied",
            "EXTOnPlayerSpawn",
            "EXTOnPlayerVehicleEntry",
            "EXTOnPlayerVehicleEject",
            "EXTOnPlayerSpawnColor",
            "EXTOnPlayerValidateConnect",
            "EXTOnWeaponReload",
            "EXTOnObjectCreate",
            "EXTOnKillMultiplier",
            "EXTOnVehicleRespawnProcess",
            "EXTOnObjectDeleteAttempt",
            "EXTOnObjectDamageLookupProcess",
            "EXTOnObjectDamageApplyProcess",
            "EXTOnMapLoad",
            "EXTOnAIVehicleEntry",
            "EXTOnWeaponDropCurrent",
            "EXTOnServerStatus",
            "EXTOnPlayerUpdate",
            "EXTOnMapReset",
            "EXTOnObjectCreateAttempt",
            "EXTOnGameSpyValidationCheck",
            "EXTOnWeaponExchangeAttempt",
            "EXTOnObjectDelete"}

        Public Structure checkList
            Public EXTOnPlayerJoin As Integer
            Public EXTOnPlayerQuit As Integer
            Public EXTOnPlayerDeath As Integer
            Public EXTOnPlayerChangeTeamAttempt As Integer
            Public EXTOnPlayerJoinDefault As Integer
            Public EXTOnNewGame As Integer
            Public EXTOnEndGame As Integer
            Public EXTOnObjectInteraction As Integer
            Public EXTOnWeaponAssignmentDefault As Integer
            Public EXTOnWeaponAssignmentCustom As Integer
            Public EXTOnServerIdle As Integer
            Public EXTOnPlayerScoreCTF As Integer
            Public EXTOnWeaponDropAttemptMustBeReadied As Integer
            Public EXTOnPlayerSpawn As Integer
            Public EXTOnPlayerVehicleEntry As Integer
            Public EXTOnPlayerVehicleEject As Integer
            Public EXTOnPlayerSpawnColor As Integer
            Public EXTOnPlayerValidateConnect As Integer
            Public EXTOnWeaponReload As Integer
            Public EXTOnObjectCreate As Integer
            Public EXTOnKillMultiplier As Integer
            Public EXTOnVehicleRespawnProcess As Integer
            Public EXTOnObjectDeleteAttempt As Integer
            Public EXTOnObjectDamageLookupProcess As Integer
            Public EXTOnObjectDamageApplyProcess As Integer
            ' Featured in 0.5.1 and newer
            Public EXTOnMapLoad As Integer
            Public EXTOnAIVehicleEntry As Integer
            Public EXTOnWeaponDropCurrent As Integer
            Public EXTOnServerStatus As Integer
            ' Featured in 0.5.2.3 and newer
            Public EXTOnPlayerUpdate As Integer
            Public EXTOnMapReset As Integer
            ' Featured in 0.5.3.0 and newer
            Public EXTOnObjectCreateAttempt As Integer
            ' Featured in 0.5.3.2 and newer
            Public EXTOnGameSpyValidationCheck As Integer
            ' Featured in 0.5.3.3 and newer
            Public EXTOnWeaponExchangeAttempt As Integer
            ' Featured in 0.5.3.4 and newer
            Public EXTOnObjectDelete As Integer
        End Structure
        Public Shared checkHooks As New checkList()
        Public Const MAX_HOOK_COUNTER As Integer = 5
        Public Const nullStr As String = "NULL"

        ' List of all available hooks

        <DllExport("EXTOnPlayerJoin", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnPlayerJoin(plI As Addon_API.PlayerInfo)
            If checkHooks.EXTOnPlayerJoin < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerJoin += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = plI.plS.Name
                vars(1) = plI.plEx.adminLvl
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, Admin: {1:hd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(0), checkHooks.EXTOnPlayerJoin.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnPlayerQuit", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnPlayerQuit(plI As Addon_API.PlayerInfo)
            If checkHooks.EXTOnPlayerQuit < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerQuit += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = plI.plS.Name
                vars(1) = plI.plEx.adminLvl
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, Admin: {1:hd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(1), checkHooks.EXTOnPlayerQuit.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnPlayerDeath", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnPlayerDeath(<[In]> killer As Addon_API.PlayerInfo, <[In]> victim As Addon_API.PlayerInfo, <[In]> mode As Integer, <[In], Out> show_message As boolOption)
            If checkHooks.EXTOnPlayerDeath < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerDeath += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(3) {}
                vars(0) = If(killer.cplS <> IntPtr.Zero, killer.plS.Name, nullStr)
                vars(1) = If(victim.cplS <> IntPtr.Zero, victim.plS.Name, nullStr)
                vars(2) = mode
                vars(3) = show_message.[boolean]
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Killer: {0:s}, Victim: {1:s}, Mode: {2:d}, showMessage: {3:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(2), checkHooks.EXTOnPlayerDeath.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnPlayerChangeTeamAttempt", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerChangeTeamAttempt(<[In]> plI As Addon_API.PlayerInfo, <[In]> team As e_color_team_index, <[In], MarshalAs(UnmanagedType.I1)> forceChange As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnPlayerChangeTeamAttempt < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerChangeTeamAttempt += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                vars(0) = plI.plS.Name
                vars(1) = team
                vars(2) = forceChange
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, Team: {1:d}, forceChange: {2:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(3), checkHooks.EXTOnPlayerChangeTeamAttempt.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will prevent change team. Unless forceChange is true, this value is ignored.
        End Function

        <DllExport("EXTOnPlayerJoinDefault", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerJoinDefault(<[In]> mS As s_machine_slot_ptr, <[In]> cur_team As e_color_team_index) As e_color_team_index
            If checkHooks.EXTOnPlayerJoinDefault < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerJoinDefault += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                Dim mS_managed As New s_machine_slot_managed(mS)
                vars(0) = mS_managed.data.machineIndex
                vars(1) = mS_managed.data.SessionKey
                vars(2) = mS_managed.data.isAvailable
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "mIndex: {0:hd}, SessionKey: {1:s}, isAvailable: {2:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(4), checkHooks.EXTOnPlayerJoinDefault.ToString(), output.ToString())
            End If
            Return e_color_team_index.TEAM_NONE 'If set to 0 will force set to red team. Or set to 1 will force set to blue team. -1 is default team.
        End Function

        <Obsolete("Do not use EXTOnNewGame hook, use EXTOnMapLoad hook instead.")>
        <DllExport("EXTOnNewGame", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnNewGame(<[In], MarshalAs(UnmanagedType.LPWStr)> map As String)
            If checkHooks.EXTOnNewGame < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnNewGame += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(0) {}
                vars(0) = map
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Map: {0:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(5), checkHooks.EXTOnNewGame.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnEndGame", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnEndGame(<[In]> mode As Integer)
            If checkHooks.EXTOnEndGame < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnEndGame += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(0) {}
                vars(0) = mode
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Mode: {0:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(6), checkHooks.EXTOnEndGame.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnObjectInteraction", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnObjectInteraction(<[In]> plI As Addon_API.PlayerInfo, <[In]> obj_id As s_ident, <[In]> objectStruct As s_object_ptr, <[In]> hTag As Addon_API.hTagHeaderPtr) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnObjectInteraction < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectInteraction += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                Dim objectStructManaged As New s_object_managed(objectStruct)
                Dim hTagManaged As New Addon_API.hTagHeader_managed(hTag)
                vars(0) = plI.plS.Name
                vars(1) = obj_id.Tag
                vars(2) = objectStructManaged.s_object_n.ModelTag.Tag
                vars(3) = objectStructManaged.s_object_n.GameObject
                vars(4) = hTagManaged.hTagHeader_n.tag_name
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, ObjectID: {1:08X}, ModelTag: {2:08X}, GameObject: {3:hd}, tagName: {4:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(7), checkHooks.EXTOnObjectInteraction.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will deny interaction to an object.
        End Function

        <DllExport("EXTOnWeaponAssignmentDefault", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnWeaponAssignmentDefault(<[In]> plI As Addon_API.PlayerInfo, <[In]> owner_obj_id As s_ident, <[In]> cur_weap_id As s_tag_reference_ptr, <[In]> order As UInteger, <[In], Out> new_weap_id As s_ident_ptr)
            If checkHooks.EXTOnWeaponAssignmentDefault < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponAssignmentDefault += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                Dim cur_weap_id_managed As New s_tag_reference_managed(cur_weap_id)
                Dim new_weap_id_managed As New s_ident_managed(new_weap_id)
                vars(0) = plI.plS.Name
                vars(1) = owner_obj_id.Tag
                vars(2) = cur_weap_id_managed.data.name
                vars(3) = order
                vars(4) = new_weap_id_managed.data.Tag
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, OwnerObjectID: {1:08X}, Weapon Name: {2:s}, Order: {3:d}, newWeapID: {4:08X}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(8), checkHooks.EXTOnWeaponAssignmentDefault.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnWeaponAssignmentCustom", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnWeaponAssignmentCustom(<[In]> plI As Addon_API.PlayerInfo, <[In]> owner_obj_id As s_ident, <[In]> cur_weap_id As s_ident, <[In]> order As UInteger, <[In], Out> new_weap_id As s_ident_ptr)
            If checkHooks.EXTOnWeaponAssignmentCustom < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponAssignmentCustom += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                Dim new_weap_id_managed As New s_ident_managed(new_weap_id)
                vars(0) = plI.plS.Name
                vars(1) = owner_obj_id.Tag
                vars(2) = cur_weap_id.Tag
                vars(3) = order
                vars(4) = new_weap_id_managed.data.Tag
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, OwnerObjectID: {1:08X}, curWeaponID: {2:08X}, Order: {3:d}, newWeapID: {4:08X}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(9), checkHooks.EXTOnWeaponAssignmentCustom.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnServerIdle", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnServerIdle()
            If checkHooks.EXTOnServerIdle < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnServerIdle += 1
                pICIniFile.m_value_set(HookNames(10), checkHooks.EXTOnServerIdle.ToString(), "Idle")
            End If
        End Sub

        <DllExport("EXTOnPlayerScoreCTF", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerScoreCTF(<[In]> plI As Addon_API.PlayerInfo, <[In]> cur_weap_id As s_ident, <[In]> team As UInteger, <[In], MarshalAs(UnmanagedType.I1)> isGameObject As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnPlayerScoreCTF < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerScoreCTF += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(3) {}
                vars(0) = plI.plS.Name
                vars(1) = cur_weap_id.Tag
                vars(2) = team
                vars(3) = isGameObject
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, curWeaponID: {1:08X}, Team: {2:d}, isGameObject: {3:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(11), checkHooks.EXTOnPlayerScoreCTF.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will prevent player to score a flag.
        End Function

        <DllExport("EXTOnWeaponDropAttemptMustBeReadied", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnWeaponDropAttemptMustBeReadied(<[In]> plI As Addon_API.PlayerInfo, <[In]> owner_obj_id As s_ident, <[In]> pl_Biped As s_object_ptr) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnWeaponDropAttemptMustBeReadied < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponDropAttemptMustBeReadied += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                Dim pl_Biped_managed As New s_biped_managed(pl_Biped)
                vars(0) = plI.plS.Name
                vars(1) = owner_obj_id.Tag
                vars(2) = pl_Biped_managed.s_object_n.sObject.ModelTag.Tag
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, ownerObjID: {1:08X}, pl_Biped: {2:08X}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(12), checkHooks.EXTOnWeaponDropAttemptMustBeReadied.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will prevent player to drop an object.
        End Function

        <DllExport("EXTOnPlayerSpawn", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnPlayerSpawn(<[In]> plI As Addon_API.PlayerInfo, <[In]> obj_id As s_ident, <[In]> pl_Biped As s_object_ptr)
            If checkHooks.EXTOnPlayerSpawn < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerSpawn += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                vars(0) = plI.plS.Name
                vars(1) = obj_id.Tag
                vars(2) = pl_Biped.ptr
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, obj_id: {1:08X}, pl_Biped: {2:08X}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(13), checkHooks.EXTOnPlayerSpawn.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnPlayerVehicleEntry", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerVehicleEntry(<[In]> plI As Addon_API.PlayerInfo, <[In], MarshalAs(UnmanagedType.I1)> forceEntry As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnPlayerVehicleEntry < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerVehicleEntry += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = plI.plS.Name
                vars(1) = forceEntry
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, forceEntry: {1:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(14), checkHooks.EXTOnPlayerVehicleEntry.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will prevent player enter a vehicle.
        End Function

        <DllExport("EXTOnPlayerVehicleEject", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerVehicleEject(<[In]> plI As Addon_API.PlayerInfo, <[In], MarshalAs(UnmanagedType.I1)> forceEject As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnPlayerVehicleEject < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerVehicleEject += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = plI.plS.Name
                vars(1) = forceEject
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, forceEject: {1:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(15), checkHooks.EXTOnPlayerVehicleEject.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will prevent player leave a vehicle.
        End Function

        <DllExport("EXTOnPlayerSpawnColor", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerSpawnColor(<[In]> plI As Addon_API.PlayerInfo, <[In], MarshalAs(UnmanagedType.I1)> isTeamPlay As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnPlayerSpawnColor < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerSpawnColor += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                vars(0) = plI.plS.Name
                vars(1) = plI.plR.ColorIndex
                vars(2) = isTeamPlay
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, Color Index: {1:hd}, isTeamPlay: {2:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(16), checkHooks.EXTOnPlayerSpawnColor.ToString(), output.ToString())
            End If
            Return True 'If set to false, it will use custom color instead of team color.
        End Function

        <DllExport("EXTOnPlayerValidateConnect", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnPlayerValidateConnect(<[In]> plEx As Addon_API.PlayerExtended, <[In]> mS As s_machine_slot, <[In]> banCheckPlayer As s_ban_check, <[In], MarshalAs(UnmanagedType.I1)> isBanned As Boolean, <[In]> svPass As Byte, <[In]> isForceReject As HEXT.PLAYER_VALIDATE) As HEXT.PLAYER_VALIDATE
            If checkHooks.EXTOnPlayerValidateConnect < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerValidateConnect += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(5) {}
                Dim cdKeyHash As String = Encoding.ASCII.GetString(banCheckPlayer.cdKeyHash.str)
                vars(0) = plEx.CDHashW
                vars(1) = cdKeyHash
                vars(2) = banCheckPlayer.requestPlayerName
                vars(3) = isBanned
                vars(4) = svPass
                vars(5) = isForceReject
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "plEx.CDHashW: {0:s}, banCheckPlayer.cdKeyHash: {1:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(17), checkHooks.EXTOnPlayerValidateConnect.ToString() + ".1", output.ToString())
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Request Name: {2:s}, isBanned: {3:d}, svPass: {4:hhd} isForceReject: {5:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(17), checkHooks.EXTOnPlayerValidateConnect.ToString() + ".2", output.ToString())
            End If
            Return HEXT.PLAYER_VALIDATE.[DEFAULT] 'Look in PLAYER_VALIDATE enum for available options to return.
        End Function

        <DllExport("EXTOnWeaponReload", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnWeaponReload(<[In]> obj_Struct As s_object_ptr, <[In], MarshalAs(UnmanagedType.I1)> allowReload As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnWeaponReload < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponReload += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                Dim obj_Struct_managed As New s_object_managed(obj_Struct)
                vars(0) = obj_Struct_managed.s_object_n.ModelTag.Tag
                vars(1) = allowReload
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Object Model Tag: {0:08X}, allowReload: {1:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(18), checkHooks.EXTOnWeaponReload.ToString(), output.ToString())
            End If
            Return True 'If set to false, weapon will not reload. Unless allowReload is false, then this value is ignored.
        End Function

        <DllExport("EXTOnObjectCreate", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnObjectCreate(<[In]> obj_id As s_ident, <[In]> obj_Struct As s_object_ptr, <[In]> header As Addon_API.hTagHeaderPtr)
            If checkHooks.EXTOnObjectCreate < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectCreate += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                Dim obj_Struct_managed As New s_object_managed(obj_Struct)
                Dim header_managed As New Addon_API.hTagHeader_managed(header)
                vars(0) = obj_Struct_managed.s_object_n.ModelTag.Tag
                vars(1) = header_managed.hTagHeader_n.tag_name
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Object Model Tag: {0:08X}, tag_name: {1:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(19), checkHooks.EXTOnObjectCreate.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnKillMultiplier", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnKillMultiplier(<[In]> killer As Addon_API.PlayerInfo, <[In]> multiplier As UInteger)
            If checkHooks.EXTOnKillMultiplier < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnKillMultiplier += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = killer.plS.Name
                vars(1) = multiplier
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, Multiplier: {1:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(20), checkHooks.EXTOnKillMultiplier.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnVehicleRespawnProcess", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnVehicleRespawnProcess(<[In]> obj_id As s_ident, <[In]> cur_object As s_object_ptr, <[In]> managedObj As Addon_API.objManagedPtr, <[In], MarshalAs(UnmanagedType.I1)> isManaged As Boolean) As HEXT.VEHICLE_RESPAWN
            If checkHooks.EXTOnVehicleRespawnProcess < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnVehicleRespawnProcess += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                Dim cur_object_managed As New s_object_managed(cur_object)
                Dim objManaged_managed As New Addon_API.objManaged_managed(managedObj)
                vars(0) = cur_object_managed.s_object_n.ModelTag.Tag
                vars(1) = objManaged_managed.objManaged_n.world.x
                vars(2) = objManaged_managed.objManaged_n.world.y
                vars(3) = objManaged_managed.objManaged_n.world.z
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "ModelTag: {0:08X}, World X: {1:f}, Y: {2:f}, Z: {3:f}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(21), checkHooks.EXTOnVehicleRespawnProcess.ToString(), output.ToString())
            End If
            Return HEXT.VEHICLE_RESPAWN.DEFAULT 'Look in VEHICLE_RESPAWN enum for available options to return.
        End Function

        <DllExport("EXTOnObjectDeleteAttempt", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnObjectDeleteAttempt(<[In]> obj_id As s_ident, <[In]> cur_object As s_object_ptr, <[In]> curTicks As Integer) As HEXT.OBJECT_ATTEMPT
            If checkHooks.EXTOnObjectDeleteAttempt < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectDeleteAttempt += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                Dim cur_object_managed As New s_object_managed(cur_object)
                vars(0) = cur_object_managed.s_object_n.ModelTag.Tag
                vars(1) = curTicks
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "ModelTag: {0:08X}, Current Ticks: {1:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(22), checkHooks.EXTOnObjectDeleteAttempt.ToString(), output.ToString())
            End If
            Return HEXT.OBJECT_ATTEMPT.DEFAULT 'Look in OBJECT_ATTEMPT enum for available options to return.
        End Function

        <DllExport("EXTOnObjectDamageLookupProcess", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnObjectDamageLookupProcess(<[In]> damageInfo As Addon_API.objDamageInfoPtr, <[In]> obj_recv As s_ident_ptr, <[In]> allowDamage As boolOption, <[In], MarshalAs(UnmanagedType.I1)> isManaged As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnObjectDamageLookupProcess < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectDamageLookupProcess += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                Dim cur_object_managed As New Addon_API.objDamageInfo_managed(damageInfo)
                Dim obj_recv_managed As New s_ident_managed(obj_recv)
                vars(0) = cur_object_managed.objDamageInfo_n.causer.Tag
                vars(1) = cur_object_managed.objDamageInfo_n.player_causer.Tag
                vars(2) = If(obj_recv_managed.isNotNull(), obj_recv_managed.data.Tag, 0)
                vars(3) = allowDamage.[boolean]
                vars(4) = isManaged
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Causer: {0:08X}, PlayerCauser: {1:08X}, obj_recv: {2:08X}, allowDamage: {3:d}, isManaged: {4:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(23), checkHooks.EXTOnObjectDamageLookupProcess.ToString(), output.ToString())
            End If
            Return True 'If set to false, it is managed by you. True for default.
        End Function

        <DllExport("EXTOnObjectDamageApplyProcess", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnObjectDamageApplyProcess(<[In]> damageInfo As Addon_API.objDamageInfoPtr, <[In]> obj_recv As s_ident_ptr, <[In]> hitInfo As Addon_API.objHitInfoPtr, <[In], MarshalAs(UnmanagedType.I1)> isBacktap As Boolean, <[In]> allowDamage As boolOption, <[In], MarshalAs(UnmanagedType.I1)> isManaged As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnObjectDamageApplyProcess < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectDamageApplyProcess += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(5) {}
                Dim cur_object_managed As New Addon_API.objDamageInfo_managed(damageInfo)
                Dim obj_recv_managed As New s_ident_managed(obj_recv)
                vars(0) = cur_object_managed.objDamageInfo_n.causer.Tag
                vars(1) = cur_object_managed.objDamageInfo_n.player_causer.Tag
                vars(2) = If(obj_recv_managed.isNotNull(), obj_recv_managed.data.Tag, 0)
                vars(3) = isBacktap
                vars(4) = allowDamage.[boolean]
                vars(5) = isManaged
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Causer: {0:08X}, PlayerCauser: {1:08X}, obj_recv: {2:08X}, isBacktap: {3:d}, allowDamage: {4:d}, isManaged: {5:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(24), checkHooks.EXTOnObjectDamageApplyProcess.ToString(), output.ToString())
            End If
            Return True 'If set to false, it is managed by you. True for default.
        End Function

        ' Featured in 0.5.1 and newer
        ' Changed in 0.5.3.3
        <DllExport("EXTOnMapLoad", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnMapLoad(<[In]> map_tag As s_ident, <[In], MarshalAs(UnmanagedType.LPWStr)> map_name As String, <[In]> game_mode As HEXT.GAME_MODE)
            If checkHooks.EXTOnMapLoad < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnMapLoad += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                vars(0) = map_tag.Tag
                vars(1) = map_name
                vars(2) = game_mode
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Map Tag: {0:08X}, Map: {1:s}, Game Mode: {2:hd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(25), checkHooks.EXTOnMapLoad.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnAIVehicleEntry", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnAIVehicleEntry(<[In]> bipedTag As s_ident, <[In]> vehicleTag As s_ident, <[In]> seatNum As UShort, <[In]> isManaged As SByte) As SByte
            If checkHooks.EXTOnAIVehicleEntry < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnAIVehicleEntry += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(3) {}
                vars(0) = bipedTag.Tag
                vars(1) = vehicleTag.Tag
                vars(2) = seatNum
                vars(3) = isManaged
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "bipedTag: {0:08X}, vehicleTag: {1:08X}, seatNum: {2:hd}, isManaged: {3:hhd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(26), checkHooks.EXTOnAIVehicleEntry.ToString(), output.ToString())
            End If
            Return -1 '-1 = default, 0 = prevent entry, 1 = force entry
        End Function
        ' Changed in 0.5.3.3
        <DllExport("EXTOnWeaponDropCurrent", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnWeaponDropCurrent(<[In]> plI As Addon_API.PlayerInfo, <[In]> bipedTag As s_ident, <[In]> biped As s_object_ptr, <[In]> weaponTag As s_ident, <[In]> weapon As s_object_ptr)
            If checkHooks.EXTOnWeaponDropCurrent < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponDropCurrent += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = bipedTag.Tag
                vars(1) = weaponTag.Tag
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "bipedTag: {0:08X}, weaponTag: {1:08X}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(27), checkHooks.EXTOnWeaponDropCurrent.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnServerStatus", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnServerStatus(<[In]> execId As Integer, <[In]> isManaged As SByte) As SByte
            If checkHooks.EXTOnServerStatus < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnServerStatus += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = execId
                vars(1) = isManaged
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "execId: {0:d}, isManaged: {1:hhd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(28), checkHooks.EXTOnServerStatus.ToString(), output.ToString())
            End If
            Return -1 '-1 = default, 0 = hide message, 1 = show message
        End Function

        ' Featured in 0.5.2.3 and newer
        <DllExport("EXTOnPlayerUpdate", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnPlayerUpdate(<[In]> plI As Addon_API.PlayerInfo)
            If checkHooks.EXTOnPlayerUpdate < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnPlayerUpdate += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                vars(0) = plI.plS.Name
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(29), checkHooks.EXTOnPlayerUpdate.ToString(), output.ToString())
            End If
        End Sub

        <DllExport("EXTOnMapReset", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnMapReset()
            If checkHooks.EXTOnMapReset < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnMapReset += 1
                pICIniFile.m_value_set(HookNames(30), checkHooks.EXTOnMapReset.ToString(), "Reset")
            End If
        End Sub

        ' Featured in 0.5.3.0 and newer
        <DllExport("EXTOnObjectCreateAttempt", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnObjectCreateAttempt(<[In]> plOwner As Addon_API.PlayerInfo, <[In]> object_creation As Addon_API.objCreationInfo, <[In], Out> change_object As Addon_API.objCreationInfoPtr, <[In], MarshalAs(UnmanagedType.I1)> isOverride As Boolean) As HEXT.OBJECT_ATTEMPT
            If checkHooks.EXTOnObjectCreateAttempt < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectCreateAttempt += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                vars(0) = object_creation.map_id.Tag
                vars(1) = object_creation.parent_id.Tag
                vars(2) = object_creation.pos.x
                vars(3) = object_creation.pos.y
                vars(4) = object_creation.pos.z
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "map_id: {0:08X}, parent_id: {1:08X}, pos.x: {2:f}, pos.y: {3:f}, pos.z: {4:f}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(31), checkHooks.EXTOnObjectCreateAttempt.ToString(), output.ToString())
            End If
            Return HEXT.OBJECT_ATTEMPT.DEFAULT 'Look in OBJECT_ATTEMPT enum for available options to return.
        End Function

        'Featured in 0.5.3.2 and newer
        <DllExport("EXTOnGameSpyValidationCheck", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnGameSpyValidationCheck(<[In]> UniqueID As UInteger, <[In], MarshalAs(UnmanagedType.I1)> isValid As Boolean, <[In], MarshalAs(UnmanagedType.I1)> forceBypass As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnGameSpyValidationCheck < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnGameSpyValidationCheck += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(2) {}
                vars(0) = UniqueID
                vars(1) = isValid
                vars(2) = forceBypass
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "UniqueID: {0:d}, isValid: {1:d}, forceBypass: {2:d}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(32), checkHooks.EXTOnGameSpyValidationCheck.ToString(), output.ToString())
            End If
            Return True 'Set to false will force bypass. True for default. Use isOverride check.
        End Function

        'Featured in 0.5.3.3 and newer
        <DllExport("EXTOnWeaponExchangeAttempt", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function EXTOnWeaponExchangeAttempt(<[In]> plOwner As Addon_API.PlayerInfo, <[In]> bipedTag As s_ident, <[In]> biped As s_object_ptr, <[In]> slot_index As Integer, weaponTag As s_ident, <[In]> weapon As s_object_ptr,
            <[In], MarshalAs(UnmanagedType.I1)> allowExchange As Boolean) As <MarshalAs(UnmanagedType.I1)> Boolean
            If checkHooks.EXTOnWeaponExchangeAttempt < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnWeaponExchangeAttempt += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(4) {}
                vars(0) = If(plOwner.cplS = IntPtr.Zero, nullStr, plOwner.plS.Name)
                vars(1) = bipedTag.Tag
                vars(2) = slot_index
                vars(3) = weaponTag.Tag
                vars(4) = allowExchange
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Player: {0:s}, bipedTag: {1:08X}, index: {2:d}, weaponTag: {3:08X}, allowExchange: {4:hhd}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(33), checkHooks.EXTOnWeaponExchangeAttempt.ToString(), output.ToString())
            End If
            Return True 'If false, then will deny exchange weapon. True by default. Use allowExchange check, if already false. Don't do anything.
        End Function

        'Featured in 0.5.3.4 and newer
        <DllExport("EXTOnObjectDelete", CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Sub EXTOnObjectDelete(<[In]> obj_id As s_ident, <[In]> obj_Struct As s_object_ptr, <[In]> header As Addon_API.hTagHeaderPtr)
            If checkHooks.EXTOnObjectDelete < MAX_HOOK_COUNTER Then
                checkHooks.EXTOnObjectDelete += 1
                Dim output As New StringBuilder(Addon_API.ICIniFileClass.INIFILEVALUEMAX)
                Dim vars As Object() = New Object(1) {}
                Dim obj_Struct_managed As New s_object_managed(obj_Struct)
                Dim header_managed As New Addon_API.hTagHeader_managed(header)
                vars(0) = obj_Struct_managed.s_object_n.ModelTag.Tag
                vars(1) = header_managed.hTagHeader_n.tag_name
                pIUtil.m_formatVariantW(output, CUInt(output.Capacity), "Object Model Tag: {0:08X}, tag_name: {1:s}", CUInt(vars.Length), vars)
                pICIniFile.m_value_set(HookNames(34), checkHooks.EXTOnObjectDelete.ToString(), output.ToString())
            End If
        End Sub

    End Class
End Namespace
