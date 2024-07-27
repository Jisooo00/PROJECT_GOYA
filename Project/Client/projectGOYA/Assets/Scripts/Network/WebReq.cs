using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// 웹서버 통신
public class WebReq : MonoBehaviour
{
	static WebReq s_inst;
	public static WebReq Instance
	{
		get
		{
			if(s_inst == null) {
				var go = new GameObject();
				go.name = "--WebReq";
				s_inst = go.AddComponent<WebReq>();
			}
			return s_inst;
		}
	}

	string url = "http://13.125.39.166:8888/";

	long user_uid = 0;
	bool sending = false;
	
	public bool IsBusy {  get { return sending; } }


	private void Awake()
	{
		GameObject.DontDestroyOnLoad(gameObject);
		
	}

	public readonly string GoogleSheetAddress= "https://docs.google.com/spreadsheets/d/1WobN-MMi-Nigpe19jeNTL7J377Ffpy3LPnjHB0qLwu4/export?format=tsv&range={0}&gid={1}";
	public readonly long DialogDataSheetID = 1957841210;
	public readonly string DialogeDataRange = "A3:F";
	public readonly long ScriptDataSheetID = 198592244;
	public readonly string ScriptDatRange = "A2:E";

	public void LoadDialogData()
	{
		StartCoroutine("RequestData");
	}
	IEnumerator RequestData()
	{
		UnityWebRequest www =
			UnityWebRequest.Get(string.Format(GoogleSheetAddress, DialogeDataRange, DialogDataSheetID));
		yield return www.SendWebRequest();
		GameData.InitDialogData(www.downloadHandler.text);
		
		www = UnityWebRequest.Get(string.Format(GoogleSheetAddress, ScriptDatRange, ScriptDataSheetID));
		yield return www.SendWebRequest();
		GameData.InitScriptData(www.downloadHandler.text);

		GameData.myData.bInitDialog = true;

	}
	

	// 재전송 구현
	IEnumerator Proc<T>(ReqBase req, Action<T> func) where T : ResBase, new()
	{
		// 동시 처리 제한, 한번에 1개만
		while(sending)
		{
			yield return null;
		}
		sending = true;

#if UNITY_EDITOR
		Debug.Log("req " + req.GetType() + " " + req);
#endif
		
		var req_text = req.ToJson();
		int try_count = 0;
		int max_count = 5;
		string res_text = "";
		while(true)
		{
			var www = UnityWebRequest.Post(url+req.api, req_text, "application/json");
			yield return www.SendWebRequest();
			//Debug.Log("www.result = " + www.result);

			if (www.result != UnityWebRequest.Result.Success)
			{
				// 통신에러
				try_count++;
				Debug.Log("www.result fail, " + www.result + ", try_count=" + try_count);
				if (try_count < max_count)
				{
					// 다시 전송
					yield return new WaitForSeconds(1);
					continue;
				}

				// 재전송 횟수 초과
				sending = false;

				var res_fail = new T();
				res_fail.responseMessage = www.result.ToString();
				func(res_fail);
				
				yield break;
			}

			// 응답을 받았음
			res_text = www.downloadHandler.text;
			break;
		}

		sending = false;

		var res = JsonUtility.FromJson<T>(res_text);
		if(res.statusCode == ResBase.SUCCESS)
		{
			Global.DebugLogText("res " + res.GetType() + " " + res);
		}
		else
		{
			Global.DebugLogText("res " + res.GetType() + " " + res,-1);
		}

		PreProc(res);
		func(res); 
	}
	
	// 기본 응답 처리
	void PreProc(ResBase res_base)
	{
		// 패킷별 별도 처리
		var res_type = res_base.GetType();;

		if (res_type == typeof(ReqGuestSignUp))
		{
			var res = (ReqGuestSignUp.Res)res_base;
			
			user_uid = res.data.userUid;
			GameData.myData.user_uid = res.data.userUid;
			GameData.myData.user_id = res.data.id;
			GameData.myData.user_pw = res.data.pw;

			PlayerPrefs.SetString(Global.KEY_USER_ID,res.data.id);
			PlayerPrefs.SetInt(Global.KEY_USER_UID,res.data.userUid);
			PlayerPrefs.SetString(Global.KEY_USER_PW,res.data.pw);
			PlayerPrefs.Save();
		}
		
		if (res_type == typeof(ReqLogin.Res))
		{
			var res = (ReqLogin.Res)res_base;
			
			if (res.IsSuccess)
			{
				user_uid = res.data.userUid;
				GameData.myData.user_uid = res.data.userUid;
				GameData.myData.user_id = res.data.id;
				GameData.myData.user_pw = res.data.pw;

				PlayerPrefs.SetString(Global.KEY_USER_ID,res.data.id);
				PlayerPrefs.SetInt(Global.KEY_USER_UID,res.data.userUid);
				PlayerPrefs.SetString(Global.KEY_USER_PW,res.data.pw);
				PlayerPrefs.Save();
				
			}
		}
		
		if (res_type == typeof(ReqSignUp.Res))
		{
			var res = (ReqSignUp.Res)res_base;
			user_uid = res.data.userUid;
			GameData.myData.user_uid = res.data.userUid;
			GameData.myData.user_id = res.data.id;
			GameData.myData.user_pw = res.data.pw;

			if (res.IsSuccess)
			{
				Global.DebugLogText("Sign-Up success");
			}
		}
		
		if (res_type == typeof(ReqUserInfo.Res))
		{
			var res = (ReqUserInfo.Res)res_base;

			if (res.IsSuccess)
			{
				GameData.myData.user_name = res.data.nickname;
				GameData.myData.cur_map = res.data.curMap;
			}
			else
			{
				Global.DebugLogText("Need Set nickname");
			}
		}
		
		if (res_type == typeof(ReqCreateUserInfo.Res))
		{
			var res = (ReqCreateUserInfo.Res)res_base;

			if (res.IsSuccess)
			{
				GameData.myData.user_name = res.data.nickname;
				GameData.myData.cur_map = res.data.curMap;
			}
			else
			{
				Global.DebugLogText("Need Set nickname");
			}
		}

		if (res_type == typeof(ReqInitUser.Res))
		{
			var res = (ReqInitUser.Res) res_base;

			if (res.IsSuccess)
			{
				GameData.myData.user_name = "";
				GameData.myData.cur_map = "";
			}

		}
		
		if (res_type == typeof(ReqQuestInfo.Res))
		{
			var res = (ReqQuestInfo.Res)res_base;

			if (res.IsSuccess)
			{
				if (GameData.QuestDatas == null)
					GameData.QuestDatas = new Dictionary<string, GameData.QuestData>();
				foreach (var quest in res.data)
				{
					if(!GameData.QuestDatas.ContainsKey(quest.questId))
						GameData.QuestDatas.Add(quest.questId,new GameData.QuestData(quest.questId,quest.state));
				}
			}
			else
			{
			}
		}
		
		if (res_type == typeof(ReqQuestAccept.Res))
		{
			var res = (ReqQuestAccept.Res) res_base;

			if (res.IsSuccess)
			{
				foreach (var quest in res.data)
				{
					if (GameData.QuestDatas.ContainsKey(quest.questId) &&
					    GameData.QuestDatas[quest.questId].state != quest.state)
						GameData.QuestDatas[quest.questId].state = quest.state;
				}
			}
			else
			{
			}
		}
		
		if (res_type == typeof(ReqQuestClear.Res))
		{
			var res = (ReqQuestClear.Res) res_base;

			if (res.IsSuccess)
			{
				foreach (var quest in res.data)
				{
					if (GameData.QuestDatas.ContainsKey(quest.questId) &&
					    GameData.QuestDatas[quest.questId].state != quest.state)
						GameData.QuestDatas[quest.questId].state = quest.state;
					
				}
				
			}
			else
			{
			}
		}
		
		if (res_type == typeof(ReqQuestAction.Res))
		{
			var res = (ReqQuestAction.Res) res_base;

			if (res.IsSuccess)
			{
				foreach (var quest in res.data)
				{
					if (GameData.QuestDatas.ContainsKey(quest.questId) &&
					    GameData.QuestDatas[quest.questId].state != quest.state)
						GameData.QuestDatas[quest.questId].state = quest.state;
				}
			}
			else
			{
			}
		}
		
	}
	
	// Guest Sign Up
	
	public void Request(ReqGuestSignUp req, Action<ReqGuestSignUp.Res> func)
	{
		StartCoroutine(Proc< ReqGuestSignUp.Res>(req, func));
	}

	// Sign in
	public void Request(ReqLogin req, Action<ReqLogin.Res> func)
	{
		StartCoroutine(Proc<ReqLogin.Res>(req, func));
	}


	// Sign up
	public void Request(ReqSignUp req, Action<ReqSignUp.Res> func)
	{
		StartCoroutine(Proc< ReqSignUp.Res>(req, func));
	}
	
	// Sign out
	public void Request(ReqSignOut req, Action<ReqSignOut.Res> func)
	{
		StartCoroutine(Proc< ReqSignOut.Res>(req, func));
	}
	
	// User Info
	public void Request(ReqUserInfo req, Action<ReqUserInfo.Res> func)
	{
		StartCoroutine(Proc< ReqUserInfo.Res>(req, func));
	}
	
	// Create User
	public void Request(ReqCreateUserInfo req, Action<ReqCreateUserInfo.Res> func)
	{
		StartCoroutine(Proc< ReqCreateUserInfo.Res>(req, func));
	}
	
	// Init User
	public void Request(ReqInitUser req, Action<ReqInitUser.Res> func)
	{
		StartCoroutine(Proc<ReqInitUser.Res>(req,func));
	}
	
	// Map Enter
	public void Request(ReqMapEnter req, Action<ReqMapEnter.Res> func)
	{
		StartCoroutine(Proc< ReqMapEnter.Res>(req, func));
	}
	
	// Quest Info
	public void Request(ReqQuestInfo req, Action<ReqQuestInfo.Res> func)
	{

		StartCoroutine(Proc< ReqQuestInfo.Res>(req, func));
	}
	
	// Quest Accept
	public void Request(ReqQuestAccept req, Action<ReqQuestAccept.Res> func)
	{

		StartCoroutine(Proc< ReqQuestAccept.Res>(req, func));
	}
	
	// Quest Clear
	public void Request(ReqQuestClear req, Action<ReqQuestClear.Res> func)
	{
		StartCoroutine(Proc< ReqQuestClear.Res>(req, func));
	}
	
	// Quest Action
	public void Request(ReqQuestAction req, Action<ReqQuestAction.Res> func)
	{
		StartCoroutine(Proc< ReqQuestAction.Res>(req, func));
	}

	


}

