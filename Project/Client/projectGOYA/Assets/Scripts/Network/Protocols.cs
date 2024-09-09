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
		//public const int SUCCESS_ = 201; // 로그인 성공
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
			public string nickname;
			public string curMap;
		};
		
		[Serializable]
		public class QuestRes
		{
			public string questId;
			public string state;
			public int count;
		}
		

		public ResData data;

		
		public bool IsSuccess { get { return statusCode == SUCCESS; } }
		public bool IsFail { get { return !IsSuccess; } }
		
	}
}

#region 웹통신 WebReq

// Guset Sign Up

[Serializable]
public class ReqGuestSignUp : ReqBase
{
	public ReqGuestSignUp()
	{
		api = "account/guest";
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

// User Info
[Serializable]
public class ReqUserInfo : ReqBase
{
	public int userUid;

	public ReqUserInfo()
	{
		api = "user/info";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}

// Create User Info
[Serializable]
public class ReqCreateUserInfo : ReqBase
{
	public string nickname;
	public int userUid;

	public ReqCreateUserInfo()
	{
		api = "user/create";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}

// Quest Info
[Serializable]
public class ReqQuestInfo : ReqBase
{
	public int userUid;

	public ReqQuestInfo()
	{
		api = "quest/info2";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
		[Serializable]
		public class ResData
		{
			public string questId;
			public string state;
		}
		
		public List<ResData> data = new List<ResData>();
	}
}

// Quest Accept
[Serializable]
public class ReqQuestAccept : ReqBase
{
	public int userUid;
	public string questId;

	public ReqQuestAccept()
	{
		api = "quest/accept2";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
		[Serializable]
		public class ResData
		{
			public string questId;
			public string state;
		}
		
		public List<ResData> data = new List<ResData>();
	}
}

// Quest Clear
[Serializable]
public class ReqQuestClear : ReqBase
{
	public int userUid;
	public string questId;

	public ReqQuestClear()
	{
		api = "quest/clear2";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
		[Serializable]
		public class ResData
		{
			public string questId;
			public string state;
		}
		
		public List<ResData> data = new List<ResData>();
	}
}

// Quest Action
[Serializable]
public class ReqQuestAction : ReqBase
{
	public int userUid;
	public string type;
	public string target;
	public int count;

	public enum eType
	{
		Game,
		Dialog,
		Item,
		Monster
	}

	public ReqQuestAction()
	{
		api = "quest/action";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
		[Serializable]
		public class ResData
		{
			public string questId;
			public string state;
		}
		
		public List<ResData> data = new List<ResData>();
	}
}

// Mep Enter
[Serializable]
public class ReqMapEnter : ReqBase
{
	public int userUid;
	public string mapId;

	public ReqMapEnter()
	{
		api = "map/enter";
		userUid = GameData.myData.user_uid;
		mapId = GameManager.Instance.Scene.GetCurrentSceneID();
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}

// Init User Info
[Serializable]
public class ReqInitUser : ReqBase
{
	public int userUid;

	public ReqInitUser()
	{
		api = "user/init";
		userUid = GameData.myData.user_uid;
	}

	// Response
	[Serializable]
	public class Res : ResBase
	{
	}
}


#endregion



