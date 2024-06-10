using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Security.Cryptography;
//using Unity.VisualScripting;
//using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// 웹서버 통신
public class WebReq : MonoBehaviour
{
	static WebReq s_inst;
	public static WebReq Inst
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

//#if UNITY_EDITOR == false || USE_REAL_SERVER
	// 빌드시 설정
	string domain = "";//"api.sidnft.com:444";
//#else
	// 에디터에서
	//string domain = null; // 서버 없고 c# 에서 응답을 코딩해야함
//#endif

	string url = null;
	[HideInInspector]
	public string url_ws = null;

	long user_uid = 0;
	int seq = 0;
	string key = "";
	bool sending = false;



	public bool IsBusy {  get { return sending; } }

#if UNITY_EDITOR

    public bool IsEmptyUrl {  get { return url == null; } }
#endif

	private void Awake()
	{
		GameObject.DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
		if (domain == null)
		{
			url = null;
			url_ws = null;
		}
		else if (domain.IndexOf(".com") >= 0)
		{
			url = string.Format("https://{0}/api-pw", domain);
			url_ws = string.Format("wss://{0}/", domain);
		}
#else
		url = string.Format("https://{0}/api-pw", domain);
		url_ws = string.Format("wss://{0}/", domain);
#endif
	}
	

	// 기다렸다 호출하기
	public void WaitCall(float time, Action func)
	{
		StartCoroutine(WaitCall_inner(time, func));
	}
	IEnumerator WaitCall_inner(float time, Action func)
	{
		yield return new WaitForSeconds(time);
		func();
	}
	//
#if UNITY_EDITOR
	IEnumerator ProcDummy(ReqBase req, Action func)
	{
		Debug.Log("ProcDummy req " + req.GetType() + " " + req.ToString());
		yield return new WaitForSeconds(1);

		Debug.Log("ProcDummy wait end");
		func();
	}
#endif

	// 재전송 구현
	IEnumerator Proc<T>(ReqBase req, Action<T> func) where T : ResBase, new()
	{
		// 동시 처리 제한, 한번에 1개만
		while(sending)
		{
			yield return null;
		}

		sending = true;

		var is_login = true;
		if (req.GetType() != typeof(ReqLogin))
		{
			is_login = false;

			seq++;

			//Debug.Log("req type= " + req.GetType());
			var req_logined = (ReqBaseWithSession)req;
			req_logined.user_uid = user_uid;
			req_logined.seq = seq;
			req_logined.key = key;
		}

		Debug.Log("req " + req.GetType() + " " + req);

		var req_text = req.ToJson();
		int try_count = 0;
		int max_count = is_login ? 1 : 5;
		string res_text = "";
		while(true)
		{
			var www = UnityWebRequest.Post(url, req_text, "application/json");
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
				//res_fail.code = ResBase.ERR_RETRY_OVER;
				res_fail.msg = www.result.ToString();
				func(res_fail);
				
				yield break;
			}

			// 응답을 받았음
			res_text = www.downloadHandler.text;
			break;
		}

		sending = false;
		//Debug.Log("res_text= " + res_text);

		var res = JsonUtility.FromJson<T>(res_text);
		if(res.code == ResBase.SUCCESS)
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

	void ProcCmd(string cmd)
	{
		if (cmd == null) return;

		Debug.Log("ProcCmd " + cmd);
		var arr = cmd.Split(',');
		var api = arr[0];
		Debug.LogError("unknown res.cmds, api=" + api);
		
	}

	// 기본 응답 처리
	void PreProc(ResBase res_base)
	{
		// cmds 먼저
		if(res_base.cmds != null)
		{
			foreach (var cmd in res_base.cmds)
			{
				try
				{
					ProcCmd(cmd);
				}
				catch(Exception e)
				{
					Debug.LogError("PreProc cmd=" + cmd + ", " + e.StackTrace);
				}
            }
		}

		// 패킷별 별도 처리
		var res_type = res_base.GetType();;
		if (res_type == typeof(ReqLogin.Res))
		{
			var res = (ReqLogin.Res)res_base;
			user_uid = res.user_uid;
			//Debug.Log("ResLogin seq=" + seq);

			if (res.IsSuccess)
			{
				/*
				var me = new UserData();
				me.user_uid = res.user_uid;
				me.user_name = res.user_name;
				me.profile_url = res.profile_url;
				me.country = res.country;
				me.email = res.email;

				me.items = new List<UserItem>();
				me.UpdateItem(res.items);

				UserData.me = me;
				*/
			}
		}
		
	}

	// Login
	public void Request(ReqLogin req, Action<ReqLogin.Res> func)
	{
		// uuid 뽑기
		var client_key = PlayerPrefs.GetString("client_key", "");
		var uuid = PlayerPrefs.GetString("uuid", "");
		//Debug.LogError("read client_key=" + client_key);

		if (client_key.Length < 10 || uuid.Length < 10)
		{
			Request_get_uuid(delegate (string server_uuid)
			{
				Request_get_uuid(delegate (string server_cliekt_key)
				{
					if (server_uuid == null || server_cliekt_key == null)
					{
						var res = new ReqLogin.Res();
						res.code = ResBase.ERR_API_FAIL;
						func(res);
						return;
					}

					//client_key = Guid.NewGuid().ToString();
					client_key = server_cliekt_key;
					uuid = server_uuid;
					PlayerPrefs.SetString("client_key", client_key);
					PlayerPrefs.SetString("uuid", uuid);

					req.client_key = client_key;
					req.udid = uuid;
					Request_login(req, func);
				});
			});
		}
		else
		{
			req.client_key = client_key;
			req.udid = uuid;
			Request_login(req, func);
		}
	}

	// 테스트 용도
	public void Request_get_uuid(Action<string> func)
	{
#if UNITY_EDITOR
		if(IsEmptyUrl)
		{
			WaitCall(1, delegate
			{
				func("editor-empty-url");
			});
			return;
		}
#endif

		StartCoroutine(Request_get_uuid_2(func));
	}

	IEnumerator Request_get_uuid_2(Action<string> func)
	{
		var url2 = url.Replace("api-pw", "get_uuid");
		Debug.Log("url2=" + url2);

		var www = UnityWebRequest.Get(url2);
		yield return www.SendWebRequest();

		if (www.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("Request_get_uuid fail, result=" + www.result);
			func(null);
			yield break;
		}

		func(www.downloadHandler.text);
	}

	void Request_login(ReqLogin req, Action<ReqLogin.Res> func)
	{
		
		user_uid = 0;
		seq = 0;
		key = "";

#if UNITY_EDITOR
		if (url == null)
		{
			StartCoroutine(ProcDummy(req, delegate
			{
				Debug.Log("Dummy ResLogin");

				var res = new ReqLogin.Res();
				res.code = Protocols.ResBase.SUCCESS;
				res.msg = "dummy res";
				res.user_uid = 1;
				res.user_name = "";

				res.items = new Item[] { };

				PreProc(res);
				func(res);
			}));
			return;
		}
#endif

		StartCoroutine(Proc< ReqLogin.Res>(req, func));
	}



}

