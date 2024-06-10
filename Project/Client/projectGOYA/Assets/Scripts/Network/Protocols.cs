using JetBrains.Annotations;
using Protocols;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Protocols
{
	// 패킷 데이터 기본 클래스
	// json <-> class 변환이 가능해함
	[Serializable]
	public abstract class JsonBase
	{
		public string ToJson()
		{
			return JsonUtility.ToJson(this, false);
		}

		public override string ToString()
		{
			return JsonUtility.ToJson(this, true);
		}
	}
	
	// 보내기 베이스
	[Serializable]
	public abstract class ReqBase : JsonBase
	{
		public string api;
	}

	// 받을때 베이스
	[Serializable]
	public abstract class ResBase : JsonBase
	{
		public const int SUCCESS = 200; // 성공
		public const int ERR_NO_API = 100; // 없는거 호출
		public const int ERR_API_FAIL = 101; // throw 발생


		//
		public int code;
		public string msg;
		public string[] cmds;   // 해석해서 써야하는 명령어

		public bool IsSuccess { get { return code == SUCCESS; } }
		public bool IsFail { get { return !IsSuccess; } }

		public abstract int No { get; }
	}
	
	// 보내기 + 세션획득후
	[Serializable]
	public abstract class ReqBaseWithSession : ReqBase
	{
		public int seq;
		public string key;
		public long user_uid;
	}
}

public enum ITEM_ID
{
	//GOLD = 1,       // 골드
	//CASH = 2,       // 다이아

}

// 실패 리턴용
[Serializable]
public class ResFail : Protocols.ResBase
{
	public override int No { get { return 0; } }
}

// 패킷 공룡 데이터
[Serializable]
public class Item : Protocols.JsonBase
{
	public long uid;
	public int id;
	public int count;
	public string attr;
}

#region 웹통신 WebReq
// Login
[Serializable]
public class ReqLogin : Protocols.ReqBase
{
	public string udid;
	public string client_key;

	public ReqLogin()
	{
		api = GetType().ToString();
	}

	// Response
	[Serializable]
	public class Res : Protocols.ResBase
	{
		public override int No { get { return 1; } }
		public long user_uid;
		public string user_name;

		//]
		public Item[] items = new Item[] { };
	}
}



#endregion



