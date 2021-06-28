{
	"ProjectConfigOptions": {
		"EnvironmentName": "dev",
		"Debug": true,
		"Url": "http://*:20303",
		"FrameworkConfig": {
			"SnowFlake": {
				"DataCenterId": 1,
				"WorkerId": 1
			},
			"ApiSecurity": true,
			"ParamsSign": {
				"Enable": true,
				"signKey": "验签秘钥"
			},
			"ApiLink": false,
			"CheckIps": {
				"Enable": false,
				"AllowIps": "::1,::ffff:192.168.0.,::ffff:127.0.0.1",
				"DisallowIps": ""
			},
			"WebUIConfig": {
				"PageRecord": {
					"PageSize": 20
				}
			}
		},
		"ApiVersion": {
			"MajorVersion": 1,
			"MinorVersion": 0
		},
		"SslCers": [
			{
				"ClientName": "",
				"CerName": "",
				"CerPassword": "",
				"CerPath": ""
			}
		],
		"CrossDomain": {
			"AllowOrigin": {
				"Enable": true,
				"PolicyName": "AllowSpecificOrigin",
				"WithOrigins": "http://127.0.0.1:20101,https://127.0.0.1:20102"
			}
		},
		"Security": {
			"Rsa": {
				"RsaPublicKey": "api内容加密公钥",
				"RsaPrivateKey": "api内容加密私钥"
			}
		},
		"CacheManager": {
			"MaxRetries": 1000,
			"CacheName": "缓存管理名称",
			"RetryTimeout": 100,
			"UpdateMode": "Up",
			"BackPlane": {
				"Key": "redisConnection",
				"KnownType": "Redis",
				"ChannelName": "test"
			},
			"LoggerFactory": {
				"KnownType": "Microsoft"
			},
			"Serializer": {
				"KnownType": "Json"
			},
			"Handles": [
				{
					"KnownType": "SystemRuntime",
					"EnablePerformanceCounters": true,
					"EnableStatistics": true,
					"ExpirationMode": "Sliding",
					"ExpirationTimeout": 60000,
					"IsBackPlaneSource": false,
					"HandleName": "SystemRuntime"
				},
				{
					"HandleName": "Redis",
					"KnownType": "Redis",
					"Key": "Redis",
					"IsBackPlaneSource": true
				}
			]
		},

		"Global": {
			"Url": "http://127.0.0.1:48181",
			"SysApi": {
				"ApiCalledRecord": {
					"ApiName": "/api/v1/orbit/ApiCallRecord"
				},
				"SaveErrorThrow": {
					"ApiName": "/api/v1/orbit/SaveErrorThrow"
				},
				"AopSysErrorRecord": {
					"ApiName": "/api/v1/orbit/SysError"
				},
				"AopErrorNotifyMail": {
					"ApiName": "/api/v1/orbit/ErrorMailNotify"
				},
				"AopSysErrorCodes": {
					"ApiName": "/api/v1/ErrorCodes/GetErrorCodes"
				}
			},
			"EnableAop": true,
			"EnableErrorNotifyMail": true
		},
		"ApiLink": {
			"Enable": false
		},

		"ValidateToken": {
			"Enable": true
		},

		"AccountAuthen": {
			"ExpireTime": 14400,
			"RenewalTime": 1800
		},
		"UpLoad": {
			"MaxRequestBodySize": 209715200
		}
	}
}
