using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cWaveManager : MonoBehaviour {

	public _eWaveState _state;

	List<cBaseType> _baseList = new List<cBaseType>();

	public List<cStageType> _stageList = new List<cStageType>();

	public List<cPipeline> _cPipe;
	int _pipeidx = 0;

	public Stack<GameObject> _stackObject = new Stack<GameObject>();

	//죽을시 없애자
	public List<GameObject> _Barterias = new List<GameObject>();

	public int _stageIndex = 0;
	public int _monsterLive = 0;
	public int _stackCount = 0;
	public uint _appearCount = 0;
	public uint _totalMonster = 0; 

	public int _killMonster = 0;

	Vector2 _offset = new Vector2(-5, 14);
	Vector2 _size = new Vector2 (10, 3);
	Vector2 _toolSize = new Vector2 (1343, 388);
	public float _time = 0;
	public int _Stagecount;
	float _randomTime = 0f;
	public string _stageName = "1_1";

	//
	List<cPattern> _listPattern;

	void Start()
	{
		_listPattern = new List<cPattern> ();

		cPattern p0 = new cPattern_0 ();
		p0._idletime = 1f;

		_listPattern.Add (p0);

		cPattern p1 = new cPattern_1 ();
		p1._idletime = 1f;

		_listPattern.Add (p1);

		cPattern p2 = new cPattern_2 ();
		p2._idletime = 1f;

		_listPattern.Add (p2);

		cPattern p3 = new cPattern_3 ();
		p3._idletime = 1f;

		_listPattern.Add (p3);

		cPattern p4 = new cPattern_4 ();
		p3._idletime = 1f;

		_listPattern.Add (p4);

		cPattern p5 = new cPattern_5 ();
		p3._idletime = 1f;

		_listPattern.Add (p5);

		cPattern p6 = new cPattern_6 ();
		p3._idletime = 1f;

		_listPattern.Add (p6);

		cPattern p7 = new cPattern_7 ();
		p3._idletime = 3f;

		_listPattern.Add (p7);
		////


		string stagename = "";

		//TODO
		if (cRoot._GetInst () == null) {
			stagename = string.Format ("{0}{1}", "Stage/", _stageName);
		} else {
			
			stagename = string.Format ("{0}{1}", "Stage/", cRoot._GetInst ()._stageName);
		}

		List<Dictionary<string, object>> data = CSVReader.Read(stagename);

		for (var i=0; i<data.Count; i++)
		{
			if (!data [i]["BaseType"].Equals ("-")) {

				string sprit = data [i] ["BaseType"].ToString ();

				string[] sprits = { "/" };

				string[] monster = sprit.Split (sprits, System.StringSplitOptions.RemoveEmptyEntries);

				cBaseType type = new cBaseType ();


				eBarteriaNUMBER _deemy = eBarteriaNUMBER.NULL;

				for (int j = 0; j < (int)eBarteriaNUMBER.END; j++) {
					_deemy = (eBarteriaNUMBER)j;

					if (monster [2].Equals (_deemy.ToString ())) {

						type._monsterNum = _deemy;

						GameObject _base = _GetMonsterObject (_deemy);

						//Debug.Log (_deemy);

						Vector2 toolvector = new Vector2 (float.Parse (monster [0]), float.Parse (monster [1]));
						float toolAvlX = _toolSize.x / toolvector.x;
						float toolAvlY = _toolSize.y / toolvector.y;

						float moveX = _size.x / toolAvlX;
						float moveY = _size.y / toolAvlY;

						Vector2 moveVector = new Vector2 (_offset.x + moveX, _offset.y - moveY);

						type._percent = float.Parse (monster [3]);

						_base.transform.localPosition = moveVector;


						_stackObject.Push (_base.gameObject);

						break;
					}
				}

				_baseList.Add (type);
			}
		}

		_baseList.Sort (delegate(cBaseType x, cBaseType y) {

			if (x._percent > y._percent)
				return 1;
			else if (x._percent < y._percent)
				return -1;
			return 0;

		});

		float cumulative = 0;
		for (int i = 0; i < _baseList.Count; i++) {

			cumulative += _baseList [i]._percent;
			_baseList [i]._cumulative = cumulative;

		}

		for (var i = 0; i < data.Count; i++) {

			if (!data [i] ["Stage"].Equals ("-")) {

				//cGameUIManager._GetInst()._uiProgression._ProGressionWaveIconAdd(i, data.Count);

				string sprit = data [i] ["Stage"].ToString ();

				string[] sprits = { "/" };

				string[] stage = sprit.Split (sprits, System.StringSplitOptions.RemoveEmptyEntries);

				switch (stage [2]) {
				case "cAppear":

					cStageType appear = new cAppear ();
					appear._name = "appear";


					eBarteriaShow _deemyshow = eBarteriaShow.All_7;

					for (int j = 0; j < (int)eBarteriaShow.END; j++) {
						_deemyshow = (eBarteriaShow)j;

						if (stage [3].Equals (_deemyshow.ToString ())) {
							((cAppear)appear)._show = _deemyshow;
							break;
						} 
					}

					((cAppear)appear)._appearCount = uint.Parse (stage [4]);
					_totalMonster += ((cAppear)appear)._appearCount;
					_stageList.Add (appear);

					break;
				case "cIfTime":
					cStageType time = new cIfTime ();
					time._name = "time";
					((cIfTime)time)._iftime = float.Parse (stage [3]);

					_stageList.Add (time);
					break;
				case "cIfRound":
					cStageType round = new cIfNextRound ();
					round._name = "round";
					((cIfNextRound)round)._ifNextRound = uint.Parse (stage [3]);

					_stageList.Add (round);
					break;
				case "cWarning":
					cStageType warning = new cWarning ();
					warning._name = "warning";
					_stageList.Add (warning);

					break;
				case "cFinal":
					cFinal final = new cFinal ();
					final._name = "final";
					_stageList.Add (final);

					break;
				case "cSpecial":
					
					cStageType special = new cSpecial ();
					special._name = "special";
					eBarteriaNUMBER _deemy = eBarteriaNUMBER.NULL;

					for (int j = 0; j < (int)eBarteriaNUMBER.END; j++) {
						_deemy = (eBarteriaNUMBER)j;

						if (stage [3].Equals (_deemy.ToString ())) {

							((cSpecial)special)._number = _deemy;
							break;
						} 
					}

					_totalMonster++;
					_stageList.Add (special);
					break;
				}

			}
		}
		_Stagecount = _stageList.Count;

		//cProgress
		cGameUIManager._GetInst()._uiProgression._ProGressionInit(_totalMonster);
		//방해
		_StageObstacle ();
		//WaveIcon
		_InitWaveIcon(data);
	}

	void Update()
	{
		switch (_state) {
		case _eWaveState.Awake:

			if (cMainGame._GetInst ()._Start) {
				_state = _eWaveState.Start;
			}

			break;
		case _eWaveState.Start:

			//스테이지 시작 스택에 있는거 가져옴
			_stackCount = _stackObject.Count;

			for (int i = 0; i < _stackCount; i++) {
				//강에 대기 한거 반환
				cWasteBasketManager._GetInst ()._ObjectInMonster (_stackObject.Pop ().GetComponent<cBarteriaBase> ());
			}

			if (cMainGame._GetInst ()._Start)
				_WhatStage (_stageList [_stageIndex]);

			break;
		//확률과 수량을 가지고 스택에 넣음
		case _eWaveState.cAppear:

			for (int c = 0; c < ((cAppear)_stageList [_stageIndex])._appearCount; c++) {

				int random = Random.Range (0, 101);

				for (int q = 0; q < _baseList.Count; q++) {

					if (random <= _baseList [q]._cumulative) {

						cBarteriaBase monster = cWasteBasketManager._GetInst ()._ObjectReuseMonster (_baseList [q]._monsterNum);

						GameObject obj = null;

						if (monster != null) {
							obj = monster.gameObject;
						} else {
							obj = _GetMonsterObject (_baseList [q]._monsterNum);
						}
						_HpUp (obj.GetComponent<cBarteriaBase> (), obj.GetComponent<cBarteriaBase> ()._number);
						obj.GetComponent<cBarteriaBase> ()._Init ();

						obj.SetActive (false);
						_stackObject.Push (obj);

						break;
					}
				}
			}
			//현 스택상황
			_stackCount = _stackObject.Count;
			//받아올 몬스터 수량
			_appearCount = ((cAppear)_stageList [_stageIndex])._appearCount;

			_randomTime = Random.Range (0f, 5f);
			_state = _eWaveState.cAppearing;

			//패턴 준비용
			switch (((cAppear)_stageList [_stageIndex])._show) {

			case eBarteriaShow.Random_0:
				_listPattern [(int)eBarteriaShow.Random_0]._Initalization ();
				//Debug.Log ("Random_0");
				break;
			case eBarteriaShow.L_GO_R_1:
				_listPattern [(int)eBarteriaShow.L_GO_R_1]._Initalization();
				//Debug.Log ("L_GO_R_1");
				break;
			case eBarteriaShow.R_GO_L_2:
				_listPattern [(int)eBarteriaShow.R_GO_L_2]._Initalization();
				//Debug.Log ("R_GO_L_2");
				break;
			case eBarteriaShow.L_Stepping_stone_R_3:
				_listPattern [(int)eBarteriaShow.L_Stepping_stone_R_3]._Initalization();
				//Debug.Log ("L_Stepping_stone_R_3");
				break;
			case eBarteriaShow.R_Stepping_stone_L_4:
				_listPattern [(int)eBarteriaShow.R_Stepping_stone_L_4]._Initalization();
				//Debug.Log ("R_Stepping_stone_L_4");
				break;
			case eBarteriaShow.Out_Cross_In_5:
				_listPattern [(int)eBarteriaShow.Out_Cross_In_5]._Initalization();
				//Debug.Log ("Out_Cross_In_5");
				break;
			case eBarteriaShow.In_Cross_Out_6:
				_listPattern [(int)eBarteriaShow.In_Cross_Out_6]._Initalization();
				//Debug.Log ("In_Cross_Out_6");
				break;
			case eBarteriaShow.All_7:
				_listPattern [(int)eBarteriaShow.All_7]._Initalization();
				//Debug.Log ("All_7");
				break;
			default:
				//Debug.Log ("ERREOR _eWaveState.cAppearing switch (((cAppear)_stageList [_stageIndex])._show) {");
				break;
			}

			//cGameUIManager._GetInst ()._uiProgression._ProGression ();

			break;
		//시간에 맞추어서 박테리아 생성
		case _eWaveState.cAppearing:



			//이제 패턴대로 불러와서 진행하게 하자
			if (_stageList [_stageIndex].GetType () == typeof(cAppear)) {


				_time += Time.deltaTime;

				switch (((cAppear)_stageList [_stageIndex])._show) {

				case eBarteriaShow.Random_0:

					if (_time >= _listPattern [(int)eBarteriaShow.Random_0]._idletime) {


						//스택 검사 
						if (_stackObject.Count >= 1) {

							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.Random_0]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.Random_0]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;


					break;

				case eBarteriaShow.L_GO_R_1:

					if (_time >= _listPattern [(int)eBarteriaShow.L_GO_R_1]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.L_GO_R_1]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.L_GO_R_1]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				case eBarteriaShow.R_GO_L_2:

					if (_time >= _listPattern [(int)eBarteriaShow.R_GO_L_2]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.R_GO_L_2]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.R_GO_L_2]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				case eBarteriaShow.L_Stepping_stone_R_3:

					if (_time >= _listPattern [(int)eBarteriaShow.L_Stepping_stone_R_3]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.L_Stepping_stone_R_3]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.L_Stepping_stone_R_3]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

				

					break;
				case eBarteriaShow.R_Stepping_stone_L_4:

					if (_time >= _listPattern [(int)eBarteriaShow.R_Stepping_stone_L_4]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.R_Stepping_stone_L_4]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.R_Stepping_stone_L_4]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				case eBarteriaShow.Out_Cross_In_5:

					if (_time >= _listPattern [(int)eBarteriaShow.Out_Cross_In_5]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.Out_Cross_In_5]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.Out_Cross_In_5]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				case eBarteriaShow.In_Cross_Out_6:
					
					if (_time >= _listPattern [(int)eBarteriaShow.In_Cross_Out_6]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.In_Cross_Out_6]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.In_Cross_Out_6]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				case eBarteriaShow.All_7:

					if (_time >= _listPattern [(int)eBarteriaShow.All_7]._idletime) {

						//스택 검사 
						if (_stackObject.Count >= 1) {
							_Barterias.Add (_stackObject.Peek ());
							//파이프 인덱스 가져오고
							_cPipe [_listPattern [(int)eBarteriaShow.All_7]._GetIndex()]._Input (_stackObject.Pop ());
							//진행하기
							_listPattern [(int)eBarteriaShow.All_7]._Process();
							_appearCount--;
							//몬스터 살아잇는거 ++
							_monsterLive++;
						} 

						//스택 보유 상황 0 작을시
						if (_stackObject.Count <= 0) {
							_stageIndex++;
							//End 기다리는 상황으로 간다
							if (_stageIndex >= _stageList.Count) {
								_state = _eWaveState.End;
							} else {
								_WhatStage (_stageList [_stageIndex]);
							}
						}
						_time = 0;
					}
					_stackCount = _stackObject.Count;

					break;
				default:
					//Debug.Log ("ERREOR _eWaveState.cAppearing switch (((cAppear)_stageList [_stageIndex])._show) {");
					break;
				}
			}

			break;
		//기다리는 시간
		case _eWaveState.cIfTime:

			_time += Time.deltaTime;

			if (_time >= ((cIfTime)_stageList [_stageIndex])._iftime) {
				_stageIndex++;
				_time = 0;

				if (_stageIndex >= _stageList.Count) {
					_state = _eWaveState.End;
				}
				else
					_WhatStage (_stageList [_stageIndex]);
			}

			break;
		//NEXT IF 조건
		case _eWaveState.cIfNextRound:

			if (_monsterLive <= ((cIfNextRound)_stageList [_stageIndex])._ifNextRound) {

				_stageIndex++;
				_time = 0;

				if (_stageIndex >= _stageList.Count) {
					_state = _eWaveState.End;
				}
				else
					_WhatStage (_stageList [_stageIndex]);

			}

			break;
		//알림을 알린다
		case _eWaveState.cWarning:

			_stageIndex++;
			_time = 0;
			if (_stageIndex >= _stageList.Count) {
				_state = _eWaveState.End;
			}
			else
				_WhatStage (_stageList [_stageIndex]);

			cGameUIManager._GetInst ()._WarningUIOpen ();

			break;
		//알림을 알린다
		case _eWaveState.cFinal:

			_stageIndex++;
			_time = 0;
			if (_stageIndex >= _stageList.Count) {
				_state = _eWaveState.End;
			}
			else
				_WhatStage (_stageList [_stageIndex]);

			//cGameUIManager._GetInst ()._WarningUIOpen ();
			cGameUIManager._GetInst ()._FinalUIOpen ();

			break;
		//스페셜 소환
		case _eWaveState.cSpecial:

			//_time += Time.deltaTime;

			//if (_time > Random.Range (0f, 5f)) {

			cBarteriaBase monsterSpecial = cWasteBasketManager._GetInst ()._ObjectReuseMonster (((cSpecial)_stageList [_stageIndex])._number);

			GameObject objSpecial = null;

			if (monsterSpecial != null) {
				objSpecial = monsterSpecial.gameObject;
			} else {
				objSpecial = _GetMonsterObject (((cSpecial)_stageList [_stageIndex])._number);

				monsterSpecial = objSpecial.GetComponent<cBarteriaBase> ();
			}

			_HpUp (objSpecial.GetComponent<cBarteriaBase> (), objSpecial.GetComponent<cBarteriaBase> ()._number);
			objSpecial.GetComponent<cBarteriaBase> ()._Init ();

			if (monsterSpecial.GetType () == typeof(cBarteriaThrow)) {

				objSpecial.SetActive (false);
				_stackObject.Push (objSpecial);

				_Barterias.Add (_stackObject.Peek ());
				_cPipe [0]._Input (_stackObject.Pop ());
			} else {
				objSpecial.SetActive (false);
				_stackObject.Push (objSpecial);

				_pipeidx = UnityEngine.Random.Range (0, 6);

				_Barterias.Add (_stackObject.Peek ());
				_cPipe [_pipeidx]._Input (_stackObject.Pop ());
			}

			_monsterLive++;

				//스택 보유 상황 0 작을시
			if (_stackObject.Count <= 0) {
				_stageIndex++;
				//End 기다리는 상황으로 간다
				if (_stageIndex >= _stageList.Count) {
					_state = _eWaveState.End;
				} else {
					_WhatStage (_stageList [_stageIndex]);
				}

			}
			//}
			break;

		case _eWaveState.End:

			if (_monsterLive <= 0) {
				_state = _eWaveState.Clear;
			}

			break;
		case _eWaveState.Clear:

			StartCoroutine (_IClear ());

			//cGameUIManager._GetInst ()._GameClearUI.SetActive (true);
			cMainGame._GetInst ()._gameTouchManager.gameObject.SetActive (false);
			//this.gameObject.SetActive (false);

			_state = _eWaveState.NULL;

			break;
		case _eWaveState.NULL:
			break;
		}
	}


	IEnumerator _IClear()
	{

		//cMainGame._GetInst ()._ObjClearEffect.SetActive (true);



		cGameUIManager._GetInst ()._ObjClearEffect.SetActive (true);
		////public AudioClip _GameClearBoom;
		//public AudioClip _GameDeath;

		cMainGame._GetInst()._ManagerSound._GetSound().PlayOneShot(cMainGame._GetInst()._ManagerSound._GameClearBoom);

		yield return new WaitForSeconds (3f);

		cGameUIManager._GetInst ()._GameClearUI.SetActive (true);

		this.gameObject.SetActive (false);
	}

	public void _AllDie()
	{

		while (_Barterias.Count != 0) {

			if (_Barterias[0].GetComponent<cBarteriaBase>()._number == eBarteriaNUMBER.throwbody) {
				//((cBarteriaThrow)_Barterias [0].GetComponent<cBarteriaBase> ())._time = 9999;
				((cBarteriaThrow)_Barterias [0].GetComponent<cBarteriaBase> ())._MushroomReCovery();
				cMainGame._GetInst ()._throwList.Clear ();
			}

			cWasteBasketManager._GetInst ()._ObjectInMonster (_Barterias[0].GetComponent<cBarteriaBase>());
		}





		cSkillCoinManager._GetInst ()._AllDie ();

		cMainGame._GetInst ()._waveManager._monsterLive = 0;


		/*
		for(int i=0; i < _Barterias.Count; i++)
		{
			cMainGame._GetInst ()._waveManager._monsterLive--;
			cWasteBasketManager._GetInst ()._ObjectInMonster (_Barterias[i].GetComponent<cBarteriaBase>());
			//Debug.Log (_Barterias [i].name + i + "   "  + _Barterias.Count);
			//cMainGame._GetInst ()._waveManager._monsterLive--;
			//cWasteBasketManager._GetInst ()._ObjectInMonster (_Barterias[i].GetComponent<cBarteriaBase>());
			//_liveBarteria[i].GetComponent<cBarteriaBase>()._
		}

		//foreach (GameObject mushobj in _stackObject) {
		//	Debug.Log (mushobj.name);

		//	cMainGame._GetInst ()._waveManager._monsterLive--;
		//	cWasteBasketManager._GetInst ()._ObjectInMonster (mushobj.GetComponent<cBarteriaBase> ());
		//}
		*/
	}




	void _InitWaveIcon(List<Dictionary<string, object>> data)
	{
		uint appearAndspecial = 0;

		for (var i = 0; i < data.Count; i++) {

			//cGameUIManager._GetInst ()._uiProgression._ProGressionWaveIconAdd ((uint)i, _totalMonster);


			if (!data [i] ["Stage"].Equals ("-")) {

				string sprit = data [i] ["Stage"].ToString ();

				string[] sprits = { "/" };

				string[] stage = sprit.Split (sprits, System.StringSplitOptions.RemoveEmptyEntries);

				switch (stage [2]) {
				case "cAppear":

					cStageType appear = new cAppear ();
					((cAppear)appear)._appearCount = uint.Parse (stage [4]);
					appearAndspecial += ((cAppear)appear)._appearCount;

					/*
					//TODO
					if (stage.Length > 4) {
						((cAppear)appear)._appearCount = uint.Parse (stage [3]);
						appearAndspecial += ((cAppear)appear)._appearCount;

					} else {
						((cAppear)appear)._appearCount = uint.Parse (stage [4]);
						appearAndspecial += ((cAppear)appear)._appearCount;
					}
					*/



					break;
				case "cIfTime":
					break;
				case "cIfRound":
					break;
				case "cWarning":
					cGameUIManager._GetInst ()._uiProgression._ProGressionWaveIconAdd (appearAndspecial, _totalMonster);
					break;
				case "cFinal":
					cGameUIManager._GetInst ()._uiProgression._ProGressionWaveIconAdd (appearAndspecial, _totalMonster);
					break;
				case "cSpecial":
					appearAndspecial++;
					break;
				}

			}
		}
	}

	public void _WhatStage(cStageType state)
	{
		_time = 0;

		if (state.GetType () == typeof(cAppear)) {
			_state = _eWaveState.cAppear;
			//Debug.Log ("_state = _eWaveState.cAppear");
		}else if (state.GetType () == typeof(cIfTime)) {
			_state = _eWaveState.cIfTime;
			//Debug.Log ("_state = _eWaveState.cIfTime");
		}else if (state.GetType () == typeof(cIfNextRound)) {
			_state = _eWaveState.cIfNextRound;
			//Debug.Log ("_state = _eWaveState.cIfNextRound");
		}else if (state.GetType () == typeof(cWarning)) {
			_state = _eWaveState.cWarning;
			//Debug.Log ("_state = _eWaveState.cWarning");
		}else if (state.GetType () == typeof(cFinal)) {
			_state = _eWaveState.cFinal;
			//_eWaveState.End
			//cGameUIManager._GetInst ()._FinalUIOpen ();
			//Debug.Log ("_state = _eWaveState.cWarning");
		}
		else if (state.GetType () == typeof(cSpecial)) {
			
			_state = _eWaveState.cSpecial;
			//Debug.Log ("_state = _eWaveState.cSpecial;");
		}
	}

	//몬스터 생성 가져오기 
	public GameObject _GetMonsterObject(eBarteriaNUMBER number)
	{
		switch (number) {
		case eBarteriaNUMBER.normal_1:
			return Instantiate(Resources.Load("Monster/prefabs/normal_1")) as GameObject;
		case eBarteriaNUMBER.normal_2:
			return Instantiate(Resources.Load("Monster/prefabs/normal_2")) as GameObject;
		case eBarteriaNUMBER.normal_3:
			return Instantiate(Resources.Load("Monster/prefabs/normal_3")) as GameObject;
		case eBarteriaNUMBER.normal_4:
			return Instantiate(Resources.Load("Monster/prefabs/normal_4")) as GameObject;
		case eBarteriaNUMBER.normal_5:
			return Instantiate(Resources.Load("Monster/prefabs/normal_5")) as GameObject;
			//
		case eBarteriaNUMBER.hide_1:
			return Instantiate(Resources.Load("Monster/prefabs/hide_1")) as GameObject;
		case eBarteriaNUMBER.hide_2:
			return Instantiate(Resources.Load("Monster/prefabs/hide_2")) as GameObject;
		case eBarteriaNUMBER.hide_3:
			return Instantiate(Resources.Load("Monster/prefabs/hide_3")) as GameObject;
		case eBarteriaNUMBER.hide_4:
			return Instantiate(Resources.Load("Monster/prefabs/hide_4")) as GameObject;
			//
		case eBarteriaNUMBER.zombie_1:
			return Instantiate(Resources.Load("Monster/prefabs/zombie_1")) as GameObject;
		case eBarteriaNUMBER.zombie_2:
			return Instantiate(Resources.Load("Monster/prefabs/zombie_2")) as GameObject;
		case eBarteriaNUMBER.zombie_3:
			return Instantiate(Resources.Load("Monster/prefabs/zombie_3")) as GameObject;
		case eBarteriaNUMBER.zombie_4:
			return Instantiate(Resources.Load("Monster/prefabs/zombie_4")) as GameObject;
			//
		case eBarteriaNUMBER.speed_1:
			return Instantiate(Resources.Load("Monster/prefabs/speed_1")) as GameObject;
		case eBarteriaNUMBER.speed_2:
			return Instantiate(Resources.Load("Monster/prefabs/speed_2")) as GameObject;
		case eBarteriaNUMBER.speed_3:
			return Instantiate(Resources.Load("Monster/prefabs/speed_3")) as GameObject;
			//
		case eBarteriaNUMBER.medic_1:
			return Instantiate(Resources.Load("Monster/prefabs/medic_1")) as GameObject;
		case eBarteriaNUMBER.medic_2:
			return Instantiate(Resources.Load("Monster/prefabs/medic_2")) as GameObject;
		case eBarteriaNUMBER.medic_3:
			return Instantiate(Resources.Load("Monster/prefabs/medic_3")) as GameObject;
			//
		case eBarteriaNUMBER.rich:
			return Instantiate(Resources.Load("Monster/prefabs/rich")) as GameObject;
			//
		case eBarteriaNUMBER.life:
			return Instantiate(Resources.Load("Monster/prefabs/life")) as GameObject;
			//
		case eBarteriaNUMBER.gift:
			return Instantiate(Resources.Load("Monster/prefabs/gift")) as GameObject;
			//
		case eBarteriaNUMBER.playeronkill:
			return Instantiate(Resources.Load("Monster/prefabs/playeronkill")) as GameObject;
			//
		case eBarteriaNUMBER.arbiter:
			return Instantiate(Resources.Load("Monster/prefabs/arbiter")) as GameObject;
			//
		case eBarteriaNUMBER.allkill:
			return Instantiate(Resources.Load("Monster/prefabs/allkill")) as GameObject;
		//
		case eBarteriaNUMBER.boom:
			return Instantiate(Resources.Load("Monster/prefabs/boom")) as GameObject;
			//
		case eBarteriaNUMBER.splits:
			return Instantiate(Resources.Load("Monster/prefabs/splits")) as GameObject;
			//
		case eBarteriaNUMBER.dawdler:
			return Instantiate(Resources.Load("Monster/prefabs/dawdler")) as GameObject;
		//
		case eBarteriaNUMBER.throwbody:
			return Instantiate(Resources.Load("Monster/prefabs/throwbody")) as GameObject;

			/*
			rich	,
			life	,
			gift	,
			playeronkill,
			arbiter	,
			allkill	,
			boom	,
			splits	,
			dawdler	,
			throwbody,
			*/
			//
			//Air
		case eBarteriaNUMBER.air_1:
			return Instantiate(Resources.Load("Monster/prefabs/air_1")) as GameObject;
		case eBarteriaNUMBER.air_2:
			return Instantiate(Resources.Load("Monster/prefabs/air_2")) as GameObject;
		case eBarteriaNUMBER.air_3:
			return Instantiate(Resources.Load("Monster/prefabs/air_3")) as GameObject;
		}

		return null;
	}

	void _HpUp(cBarteriaBase barteria, eBarteriaNUMBER number)
	{

		if (cRoot._GetInst ()._chapter == 1) {

			if (cRoot._GetInst ()._stage == 1) {
			
				barteria._nMaxHP -= (barteria._nMaxHP * 50) * 0.01f; 

			}
			else if (cRoot._GetInst ()._stage == 2) {
				barteria._nMaxHP -= (barteria._nMaxHP * 40) * 0.01f; 
			}
			else if (cRoot._GetInst ()._stage == 3) {
				barteria._nMaxHP -= (barteria._nMaxHP * 30) * 0.01f; 
			}
			else if (cRoot._GetInst ()._stage == 4) {
				barteria._nMaxHP -= (barteria._nMaxHP * 20) * 0.01f; 
			}
			else if (cRoot._GetInst ()._stage == 5) {
				barteria._nMaxHP -= (barteria._nMaxHP * 10) * 0.01f; 
			}
		}

		int hp = 0;
		switch (number) {

		case eBarteriaNUMBER.normal_5:

			hp = (cRoot._GetInst()._chapter - 3) * 200;
			barteria._nMaxHP += hp;

			return;
		case eBarteriaNUMBER.hide_4:

			hp = (cRoot._GetInst()._chapter - 3) * 160;
			barteria._nMaxHP += hp;

			return;
		case eBarteriaNUMBER.zombie_4:

			hp = (cRoot._GetInst()._chapter - 3) * 180;
			barteria._nMaxHP += hp;

			return;
		case eBarteriaNUMBER.speed_3:

			hp = (cRoot._GetInst()._chapter - 3) * 170;
			barteria._nMaxHP += hp;
			return;

		case eBarteriaNUMBER.medic_3:

			hp = (cRoot._GetInst()._chapter - 3) * 160;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.rich:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.life:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.gift:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
		case eBarteriaNUMBER.boom:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 200;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.playeronkill:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 200;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.arbiter:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 200;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.allkill:
			hp = cRoot._GetInst()._stage * 7 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.splits:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
			//
		case eBarteriaNUMBER.dawdler:
			hp = cRoot._GetInst()._stage * 5 + cRoot._GetInst()._chapter * 150;
			barteria._nMaxHP += hp;

			return;
			//
		
			/*
			rich	,
			life	,
			gift	,
			playeronkill,
			arbiter	,
			allkill	,
			boom	,
			splits	,
			dawdler	,
			throwbody,
			*/
			//
			//Air
		default:
			break;
		}
	}

	//방해물
	void _StageObstacle()
	{
		switch (cRoot._GetInst ()._stageName) {
		case "2_1":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			break;
		case "2_2":
			_CreateMuddyWater (0);
			break;
		case "2_3":
			_CreateMuddyWater (1);
			break;
		case "2_4":
			_CreateMuddyWater (2);
			break;
		case "2_5":
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			break;
		case "2_6":
			_CreateMuddyWater (3);
			break;
		case "2_7":
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			break;
		case "2_8":
			_CreateMuddyWater (6);
			break;
		case "2_9":
			_CreateMuddyWater (2);
			break;
		case "2_10":
			_CreateMuddyWater (0);
			_CreateMuddyWater (1);
			break;
		case "2_11":
			_CreateMuddyWater (2);
			_CreateMuddyWater (4);

			break;
		case "2_12":
			_CreateMuddyWater (6);
			_CreateMuddyWater (3);
			break;
		case "2_13":
			_CreateMuddyWater (2);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "2_14":
			_CreateMuddyWater (2);
			_CreateMuddyWater (5);
			break;
		case "2_15":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (1);
			break;
		case "2_16":
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			break;
		case "2_17":
			_CreateMuddyWater (1);
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			break;
		case "2_18":
			_CreateMuddyWater (2);
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			break;
		case "2_19":
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "2_20":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			break;
		case "2_21":
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			break;
		case "2_22":
			_CreateMuddyWater (1);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "2_23":
			_CreateMuddyWater (0);
			_CreateMuddyWater (5);
			break;
		case "2_24":
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			_CreateMuddyWater (5);
			break;
		case "2_25":
			_CreateMuddyWater (1);
			_CreateMuddyWater (6);
			break;
		case "2_26":
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "2_27":
			_CreateMuddyWater (1);
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			break;
		case "2_28":
			_CreateMuddyWater (5);
			_CreateMuddyWater (6);
			break;
		case "2_29":
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			_CreateMuddyWater (6);
			break;
		case "2_30":
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			break;

		case "3_1":
			_CreateMuddy (0);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			break;
		case "3_2":
			_CreateMuddy (1);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "3_3":
			_CreateMuddy (4);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			break;
		case "3_4":
			_CreateMuddy (3);
			_CreateMuddyWater (3);
			break;
		case "3_5":
			_CreateMuddy (2);
			_CreateMuddyWater (6);
			_CreateMuddyWater (6);
			break;
		case "3_6":
			_CreateMuddy (0);
			_CreateMuddy (0);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			break;
		case "3_7":
			_CreateMuddy (2);
			_CreateMuddy (2);
			_CreateMuddyWater (5);
			break;
		case "3_8":
			_CreateMuddy (5);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			break;
		case "3_9":
			_CreateMuddy (3);
			_CreateMuddy (3);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			break;
		case "3_10":
			_CreateMuddy (0);
			_CreateMuddy (0);
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			break;
		case "3_11":
			_CreateMuddyWater (0);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddy (2);
			_CreateMuddy (2);
			break;
		case "3_12":
			_CreateMuddyWater (2);
			_CreateMuddy (4);
			_CreateMuddy (4);
			_CreateMuddyWater (5);

			break;
		case "3_13":
			_CreateMuddy (1);
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);

			break;
		case "3_14":
			_CreateMuddyWater (0);
			_CreateMuddy (2);
			_CreateMuddy (2);
			_CreateMuddyWater (5);
			break;
		case "3_15":
			_CreateMuddy (3);
			_CreateMuddy (4);
			_CreateMuddy (5);
			break;
		case "3_16":
			_CreateMuddyWater (1);
			_CreateMuddy (4);
			_CreateMuddy (4);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			break;
		case "3_17":
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			_CreateMuddyWater (3);
			_CreateMuddyWater (5);
			_CreateMuddy (6);
			_CreateMuddy (6);
			break;
		case "3_18":
			_CreateMuddy (4);
			_CreateMuddyWater (5);
			_CreateMuddyWater (5);
			_CreateMuddyWater (7);
			_CreateMuddyWater (7);
			break;
		case "3_19":
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddy (3);
			_CreateMuddy (6);
			break;
		case "3_20":
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (1);
			_CreateMuddy (3);
			_CreateMuddy (4);
			break;

		case "3_21":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddy (2);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			break;
		case "3_22":
			_CreateMuddyWater (1);
			_CreateMuddyWater (3);
			_CreateMuddy (4);
			_CreateMuddy (4);
			break;
		case "3_23":
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			_CreateMuddy (4);
			_CreateMuddy (4);
			_CreateMuddyWater (5);
			break;
		case "3_24":
			_CreateMuddyWater (1);
			_CreateMuddyWater (4);
			_CreateMuddyWater (4);
			_CreateMuddy (6);
			break;
		case "3_25":
			_CreateMuddy (0);
			_CreateMuddy (0);
			_CreateMuddy (1);
			_CreateMuddyWater (1);
			_CreateMuddyWater (5);
			break;
		case "3_26":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (1);
			_CreateMuddyWater (4);
			_CreateMuddy (4);
			break;
		case "3_27":
			_CreateMuddyWater (4);
			_CreateMuddy (6);
			_CreateMuddy (7);
			break;
		case "3_28":
			_CreateMuddyWater (0);
			_CreateMuddyWater (0);
			_CreateMuddyWater (2);
			_CreateMuddy (3);
			_CreateMuddy (3);
			_CreateMuddy (3);
			break;
		case "3_29":
			_CreateMuddyWater (2);
			_CreateMuddyWater (2);
			_CreateMuddyWater (4);
			_CreateMuddy (4);
			_CreateMuddyWater (5);
			break;
		case "3_30":
			_CreateMuddyWater (1);
			_CreateMuddy (1);
			_CreateMuddyWater (4);
			_CreateMuddyWater (7);
			_CreateMuddy (7);
			break;
		default:
			//Debug.Log ("Error 방해물 설치 에러" + _stageName);
			break;
		}
	}


	void _CreateMuddyWater(int line)
	{
		
		//라인
		int x = Random.Range (0, 6);
		int y = line;

		int random = (y * 6) + x;

		//맵에 비어 있을시 설치한다
		if (cMainGame._GetInst ()._mapManager._mapList [random]._state == eMAPSTATE.Empty) {


			GameObject obj = Instantiate (Resources.Load ("2.GameScene/Prefabs/Muddy1"), 
				                 cMainGame._GetInst ()._mapManager._mapList [random].transform) as GameObject;

			obj.transform.localPosition = new Vector3 (-0.05f, 0);

			//설치 불가로 만든다
			cMainGame._GetInst ()._mapManager._mapList [random]._state = eMAPSTATE.Impossibility;
			cMainGame._GetInst ()._mapManager._mapList [random]._object = obj;
			return;

		} else {

			_CreateMuddyWater (line);
			
		}
	}

	void _CreateMuddy(int line)
	{

		//for (int i = 0; i < 7; i++) {
		//	cMainGame._GetInst()._mapManager._mapList[i]
		//}

		//라인
		int x = Random.Range (0, 6);
		int y = line;

		int random = (y * 6) + x;

		//맵에 비어 있을시 설치한다
		if (cMainGame._GetInst ()._mapManager._mapList [random]._state == eMAPSTATE.Empty) {

			GameObject obj = null;
			if (cRoot._GetInst ()._chapter == 3) {

				switch (Random.Range (0, 2)) {
				case 0:
					obj = Instantiate (Resources.Load ("2.GameScene/Prefabs/Muddy2"), 
						cMainGame._GetInst ()._mapManager._mapList [random].transform) as GameObject;
					break;
				case 1:
					obj = Instantiate (Resources.Load ("2.GameScene/Prefabs/Muddy3"), 
						cMainGame._GetInst ()._mapManager._mapList [random].transform) as GameObject;

					obj.transform.localPosition = new Vector3 (-0.05f, 0);
					break;

				}
			}
			else
			{
				obj = Instantiate (Resources.Load ("2.GameScene/Prefabs/Muddy2"), 
				                 cMainGame._GetInst ()._mapManager._mapList [random].transform) as GameObject;
			}
			//설치 불가로 만든다
			cMainGame._GetInst ()._mapManager._mapList [random]._state = eMAPSTATE.Impossibility;
			cMainGame._GetInst ()._mapManager._mapList [random]._object = obj;
			return;

		} else {

			_CreateMuddy (line);

		}
	}
}

public class cBaseType
{
	public eBarteriaNUMBER _monsterNum;
	public float _percent;
	public float _cumulative;
};

[System.Serializable]
public class cStageType
{

	public string _name = "";

	public virtual void _Debug()
	{}
};
public class cAppear : cStageType
{
	
	public eBarteriaShow _show;
	public uint _appearCount = 0;

	public override void _Debug ()
	{
	//	Debug.Log ("cAppear " + " _appearCount = " + _appearCount);
	}
};
public class cIfTime : cStageType
{
	public float _iftime = 0;

	public override void _Debug ()
	{
	//	Debug.Log ("cIfTime " + " _iftime = " + _iftime);
	}
};
public class cIfNextRound : cStageType
{
	public uint _ifNextRound = 0;

	public override void _Debug ()
	{
		//Debug.Log ("cIfNextRound " + " _ifNextRound = " + _ifNextRound);
	}
};
public class cWarning : cStageType
{
	public override void _Debug ()
	{
		//Debug.Log ("cWarning");
	}
};
public class cFinal : cStageType
{
	//public eBarteriaNUMBER _number;

	public override void _Debug ()
	{
		//Debug.Log ("cSpecial " + " _number = " + _number);
	}
};
public class cSpecial : cStageType
{
	public eBarteriaNUMBER _number;

	public override void _Debug ()
	{
		//Debug.Log ("cSpecial " + " _number = " + _number);
	}
};

//파이프에서 생성 한 후 진행을 한다 ?

public abstract class cPattern
{
	public float _idletime = 0;

	public abstract void _Initalization ();

	public abstract int _GetIndex ();

	public abstract void _Process ();
};

//랜덤
public class cPattern_0 : cPattern
{
	int _index = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_index = Random.Range (0, 6);
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{
		_index = Random.Range (0, 6);
	}
};

//왼쪽에서 오른쪽으로 가는 패턴 다시 왼쪽으로 가는 패턴
public class cPattern_1 : cPattern
{
	int _index = 0;
	bool _isturn = false;

	//0 ~ 5
	public override void _Initalization ()
	{
		_index = 0;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{
		
		if (_isturn == false) {

			_index++;

			if (_index >= 6) {
				_index = 4;
				_isturn = true;
			}

		}
		else if (_isturn == true) {

			_index--;

			if (_index <= -1) {
				_index = 1;
				_isturn = false;
			}

		}
	}
};

public class cPattern_2 : cPattern
{
	int _index = 5;
	bool _isturn = false;

	//0 ~ 5
	public override void _Initalization ()
	{
		_index = 5;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{

		if (_isturn == false) {

			_index--;

			if (_index <= -1) {
				_index = 1;
				_isturn = true;
			}

		} else if (_isturn == true) {

			_index++;

			if (_index >= 6) {
				_index = 4;
				_isturn = false;
			}
		}
	}
};

public class cPattern_3 : cPattern
{
	int _oldindex = 0;
	int _index = 0;
	bool _isturn = false;
	int _processcount = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_oldindex = 0;
		_index = 0;
		_processcount = 0;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{

		if (_isturn == false) {

			_index += 2;

			if (_index >= 6) {
				_index = _index - 6;
			}

		}
		else if (_isturn == true) {

			_index -= 2;

			if (_index <= -1) {
				_index = 6 + _index;
			}

		}

		_processcount++;
		//1//2
		if (_processcount >= 3) {

			if (_isturn == false) {
				_oldindex++; //1
			} else if (_isturn == true) {
				_oldindex--;
			}

			if (_oldindex >= 6) {
				_oldindex = 4;
				_isturn = true;
			} else if (_oldindex <= -1) {
				_oldindex = 1;
				_isturn = false;
			}


			_index = _oldindex; //1
			_processcount = 0; //0
			_idletime = 1;

		} else {
			_idletime = 0;
		}
	}
};

public class cPattern_4 : cPattern
{
	int _oldindex = 5;
	int _index = 5;
	bool _isturn = false;
	int _processcount = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_oldindex = 5;
		_index = 5;
		_processcount = 0;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{

		if (_isturn == false) {

			_index -= 2;

			if (_index <= -1) {
				_index = 6 + _index;
			}

		} else if (_isturn == true) {

			_index += 2;

			if (_index >= 6) {
				_index = _index - 6;
			}
		}


		_processcount++;
		//1//2
		if (_processcount >= 3) {
			
			if (_isturn == false) {
				_oldindex--; //1
			} else if (_isturn == true) {
				_oldindex++;
			}

			if (_oldindex >= 6) {
				_oldindex = 4;
				_isturn = false;
			} else if (_oldindex <= -1) {
				_oldindex = 1;
				_isturn = true;
			}

			_index = _oldindex; //1
			_processcount = 0; //0
			_idletime = 1;

		} else {
			_idletime = 0;
		}
	}
};

public class cPattern_5 : cPattern
{
	int _oldindex = 0;
	int _index = 0;
	bool _isturn = false;
	int _processcount = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_idletime = 0;

		_oldindex = 0;
		_index = 0;
		_processcount = 0;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{
		
		_index = 5 - _oldindex;

		_processcount++;

		if (_processcount >= 2) {

			if (_isturn == false) {
				_oldindex++;
			} else {
				_oldindex--;
			}

			if (_oldindex <= -1) {
				_isturn = false;
				_oldindex = 1;
			} else if (_oldindex >= 3) {
				_isturn = true;
				_oldindex = 1;
			}

			_index = _oldindex;
			_processcount = 0;
			_idletime = 1;

		} else {
			_idletime = 0;
		}

	}
};

public class cPattern_6 : cPattern
{
	int _oldindex = 2;
	int _index = 2;
	bool _isturn = false;
	int _processcount = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_idletime = 0;

		_oldindex = 2;
		_index = 2;
		_processcount = 0;
		_isturn = false;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{

		_index = 5 - _oldindex;

		_processcount++;

		if (_processcount >= 2) {

			if (_isturn == false) {
				_oldindex--;
			} else {
				_oldindex++;
			}

			if (_oldindex <= -1) {
				_isturn = true;
				_oldindex = 1;
			} else if (_oldindex >= 3) {
				_isturn = false;
				_oldindex = 1;
			}

			_index = _oldindex;
			_processcount = 0;
			_idletime = 1;

		} else {
			_idletime = 0;
		}
	}
};


public class cPattern_7 : cPattern
{
	int _index = 0;

	//0 ~ 5
	public override void _Initalization ()
	{
		_idletime = 0;

		_index = 0;
	}

	public override int _GetIndex ()
	{
		return _index;
	}

	public override void _Process ()
	{
		_index++;
		_idletime = 0;

		if (_index >= 6) {
			_index = 0;

			_idletime = 3;
		}
	}
};


public enum _eWaveState
{
	Awake		,
	Start		,
	cAppear		,
	cAppearing	,
	cIfTime		,
	cIfNextRound,
	cWarning	,
	cFinal		,
	cSpecial	,
	End			,
	Clear		,
	NULL		,
};

public enum eBarteriaShow
{
	Random_0					,

	L_GO_R_1					,
	R_GO_L_2					,

	L_Stepping_stone_R_3		,	
	R_Stepping_stone_L_4		,	

	Out_Cross_In_5				,
	In_Cross_Out_6				,

	All_7						,

	END							,

}

/*
					//1번은 1, 2, 3, 4, 5, 6, 5, 4, 3, 2, 1, .. 순
_time += Time.deltaTime;

if (_time >= _randomTime) {

	_randomTime = Random.Range (0f, 2f);

	//초과할시
	if (_pipeidx >= 6) {
		_pipeidx = 5;
		_isturn = true;

	} else if (_pipeidx <= -1) {
		_pipeidx = 1;
		_isturn = false;

	}

	//스택 검사 
	if (_stackObject.Count >= 1) {
		//_cPipe [_pipeidx]._baseMonster = _stackObject.Pop ().GetComponent<cBarteriaBase> ();
		//_cPipe [_pipeidx]._InPipe ();

		_cPipe [_pipeidx]._Input (_stackObject.Pop ());

		if (_isturn == true) {
			_pipeidx--;
		} else {
			_pipeidx++;
		}

		//cMainGame._GetInst ()._waveManager._cPipe [pipe]._Input(ball);
		_appearCount--;
		//몬스터 살아잇는거 ++
		_monsterLive++;
	} 

	//스택 보유 상황 0 작을시
	if (_stackObject.Count <= 0) {
		_stageIndex++;
		//End 기다리는 상황으로 간다
		if (_stageIndex >= _stageList.Count) {
			_state = _eWaveState.End;
		} else {
			_WhatStage (_stageList [_stageIndex]);
		}

	}
	_time = 0;
	//}
}
_stackCount = _stackObject.Count;

*/