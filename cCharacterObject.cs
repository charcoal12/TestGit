using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class cCharacterObject : MonoBehaviour {

	protected GameObject _installobj = null;
	protected cInstallAniEnd _endani;

	public _eMushRoomState _state;
	public _eMushRoomNUMBER _number;

	protected Animator _animator = null;
	protected AudioSource _sound = null;

	protected float _skillvalue = 1;
	protected float _delay = 1;

	public virtual void _Delete ()
	{

		cCharacterInfoManager._GetInst ()._GetMushRoomInfo ((int)_number)._totalInstall++;

		for(int i=0; i< cMainGame._GetInst ()._gameTouchManager._notslotManager._uiSlotList.Count; i++)
		{

			if (cMainGame._GetInst ()._gameTouchManager._notslotManager._uiSlotList [i].gameObject.activeSelf) {
			//	cCharacterInfoManager._GetInst ()._GetMushRoomInfo ((int)_number)._totalInstall--;
				cMainGame._GetInst ()._gameTouchManager._notslotManager._uiSlotList [i]._Init ();
			}
		}




	}

	public virtual void _Init()
	{
		_animator = this.GetComponent<Animator> ();
		_sound = this.GetComponent<AudioSource> ();
		_skillvalue = cCharacterInfoManager._GetInst ()._GetMushRoomInfo ((int)_number)._GetSkillValue ();
		_delay = cCharacterInfoManager._GetInst ()._GetMushRoomInfo ((int)_number)._GetDelay ();
	}


	public virtual void _VirtualAnimation()
	{
		switch (_state) {

		case _eMushRoomState.InstallSet:

			_state = _eMushRoomState.Install;
			break;

		case _eMushRoomState.Install:

			/*
			_installobj = Instantiate (cCharacterInfoManager._GetInst ()._GetMushRoomInfo ((int)_number)._installObject, this.transform.parent) as GameObject;
			_installobj.transform.localPosition = Vector2.zero;

			_endani = _installobj.GetComponent<cInstallAniEnd> ();
			*/
			_state = _eMushRoomState.Awake;

			break;

		case _eMushRoomState.Awake:

			/*
			if (_endani._AniEnd) {

				DestroyObject (_installobj.gameObject);

				this.transform.localPosition = Vector2.zero;

				_endani = null;
				_state = _eMushRoomState.Idle;

				GameObject obj = cObjectPool.SharedInstance.GetPooledObject ("CreateEffect");

				if (obj != null) {

					obj.transform.parent = this.transform;
					obj.transform.localPosition = new Vector3 (0, -0.35f);
					obj.SetActive (true);
				}
			}
			*/
			this.transform.localPosition = Vector2.zero;

			GameObject obj = cObjectPool.SharedInstance.GetPooledObject ("CreateEffect");

			if (obj != null) {

				obj.transform.parent = this.transform;
				obj.transform.localPosition = new Vector3 (0, -0.35f);
				obj.SetActive (true);
			}



			_state = _eMushRoomState.Idle;

			break;
		}
	}

	public void _SetState(_eMushRoomState state)
	{
		_state = state;
	}
}
