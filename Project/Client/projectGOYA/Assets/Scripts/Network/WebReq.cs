using Protocols;
using System;
using System.Collections;
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

	string url = "http://43.201.26.129:8888/";

	long user_uid = 0;
	bool sending = false;
	
	public bool IsBusy {  get { return sending; } }


	private void Awake()
	{
		GameObject.DontDestroyOnLoad(gameObject);
		
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
		

		Debug.Log("req " + req.GetType() + " " + req);

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
			Debug.Log("res " + res.GetType() + " " + res);
		}
		else
		{
			Debug.LogWarning("res " + res.GetType() + " " + res);
		}

		PreProc(res);
		func(res); 
	}
	
	// 기본 응답 처리
	void PreProc(ResBase res_base)
	{
		// 패킷별 별도 처리
		var res_type = res_base.GetType();;
		
		if (res_type == typeof(ReqLogin.Res))
		{
			var res = (ReqLogin.Res)res_base;


			if (res.IsSuccess)
			{
				user_uid = res.data.userUid;
				UserData.myData.user_uid = res.data.userUid;
				UserData.myData.user_id = res.data.id;
				UserData.myData.user_pw = res.data.pw;
				if((PlayerPrefs.GetInt(Global.KEY_USER_UID) != UserData.myData.user_uid))
				{
					PlayerPrefs.SetString(Global.KEY_USER_ID,res.data.id);
					PlayerPrefs.SetInt(Global.KEY_USER_UID,res.data.userUid);
					PlayerPrefs.SetString(Global.KEY_USER_PW,res.data.pw);
					PlayerPrefs.Save();
				}
				Debug.Log("login success");
			}
		}
		
		if (res_type == typeof(ReqSignUp.Res))
		{
			var res = (ReqSignUp.Res)res_base;
			user_uid = res.data.userUid;
			UserData.myData.user_uid = res.data.userUid;
			UserData.myData.user_id = res.data.id;

			if (res.IsSuccess)
			{
				Debug.Log("Sign-Up success");
			}
		}
		
		if (res_type == typeof(ReqUserInfo.Res))
		{
			var res = (ReqUserInfo.Res)res_base;

			if (res.IsSuccess)
			{
				UserData.myData.user_name = res.data.nickname;
				UserData.myData.cur_map = res.data.curMap;
				Debug.Log("Set nickname success");
			}
			else
			{
				Debug.Log("Need Set nickname");
			}
		}
		
	}

	// Sign in
	public void Request(ReqLogin req, Action<ReqLogin.Res> func)
	{

		StartCoroutine(Proc< ReqLogin.Res>(req, func));
		
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
	
	public void Request(ReqCreateUserInfo req, Action<ReqCreateUserInfo.Res> func)
	{

		StartCoroutine(Proc< ReqCreateUserInfo.Res>(req, func));
		
	}

	


}

