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
		public const int SUCCESS = 200;
		public const int SUCCESS_ = 201; // 로그인 성공
		//public const int ERR_NO_API = 100; // 없는거 호출
		//public const int ERR_API_FAIL = 101; // throw 발생


		//
		public int statusCode;
		public string responseMessage;
		// 해석해서 써야하는 명령어
		
		[Serializable]
		public class ResData
		{
			public string id;
			public string pw;
			public int userUid;
		};

		public ResData data;

		
		public bool IsSuccess { get { return statusCode == SUCCESS || statusCode == SUCCESS_; } }
		public bool IsFail { get { return !IsSuccess; } }
		
	}
}

#region 웹통신 WebReq

// Sign In
[Serializable]
public class ReqLogin : ReqBase
{
	public string id;
	public string pw;

	public ReqLogin()
	{
		api = "account/login";
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}

// Sign Up

[Serializable]
public class ReqSignUp : ReqBase
{
	public string id;
	public string pw;

	public ReqSignUp()
	{
		api = "account/signup";
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
		public string id;
		public string pw;
		public long userUid;
	}
}


// Sign Out 
[Serializable]
public class ReqSignOut : ReqBase
{
	public int userUid;

	public ReqSignOut()
	{
		api = "account/logout";
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}

#endregion



