### web config 模板说明

1. 配置文件在 dev.json 或者 pro.json 文件中都可以重新配置

2. 修改 cnf_config.json 为 cnf.json

3. dev.json 或者 pro.json 文件中的配置都会覆盖 cnf.json 的配置

4. 如果需要自定义配置 可以添加自定义配置文件 \*.json 文件

   > 3.1 文件根节点格式必须如下

   ```json
   {
   	"ProjectConfigOptions": {
   		// 自定义配置
   	}
   }
   ```

5. 创建 ProjectExtendsOptions.cs 代码如下:

```csharp
public class ProjectExtendsOptions : ConfigOptions
{
    // 自定义配置文件对应的属性
}
```

---

## 配置文件详细说明

### 1. cnf.json 配置文件详细说明:

```json
{
	"ProjectConfigOptions": {
		"EnvironmentName": "dev", // 当先项目运行环境  dev 或者 pro 固定值，对应envConfig中的文件名
		"Debug": true, // 当前项目运行模式是否是调试模式
		"Url": "http://*:80,https://*:443", // 当前项目启动监听的uri
		//  当前项目框架配置
		"FrameworkConfig": {
			//  是否启用autofac
			"Autofac": {
				"Enable": true
			},
			//  是否启用 aspectCore 注入框架
			"AspectCore": {
				"Enable": true
			}
		},
		//当前项目版本
		"ApiVersion": {
			"MajorVersion": 1,
			"MinorVersion": 0
		},
		// 项目包含的cer证书 数组
		"SslCer": [
			{
				// 证书名
				"CerName": "",
				// 证书名 秘钥
				"CerPassword": "",
				// 证书路径
				"CerPath": ""
			}
		],
		// 项目域配置
		"CrossDomain": {
			//  项目跨域配置
			"AllowOrigin": {
				"Enable": true,
				// 允许跨域的别名
				"PolicyName": "AllowSpecificOrigin",
				// 允许跨域的 uri
				"WithOrigins": "http://127.0.0.1:80,https://127.0.0.1:443"
			},
			// ip检测
			"CheckIps": {
				// 是否启用ip检测  true 启用白名单验证(AllowIps)   false 启用黑名单验证(DisallowIps)
				"Enable": false,
				"AllowIps": "::1,::ffff:192.168.0.,::ffff:127.0.0.1",
				"DisallowIps": ""
			}
		},
		// 项目安全配置 结合 Security 节点一起配置
		"ApiSecurity": {
			"Enable": true
		},
		// rsa 的秘钥对
		"Security": {
			"Rsa": {
				"RsaPublicKey": "api内容加密公钥",
				"RsaPrivateKey": "api内容加密私钥"
			}
		},
		// 项目url验签
		"ParamsSign": {
			"Enable": true,
			"signKey": "参数验签秘钥"
		},
		// 项目ui界面配置
		"WebUIConfig": {
			"PageRecord": {
				"PageSize": 20
			}
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
```

### 2. dev.json 配置文件详细说明(pro.json 相同):

    2.1 dev.json 与 pro.json 配置唯一的区别只在于开发环境的 ip 与生产环境的 ip 区别。
    2.2 文件内的配置内容都可以在 cnf.json 配置，只是 dev.json 和 pro.json 配置在运行时会覆盖 cnf.json 的配置所以一般都在 dev.json 和 pro.json 配置

```json
{
	"ProjectConfigOptions": {
		// 数据库配置
		"DbEntity": {
			// 数据库连接字符串
			"ConnectionString": "Server=mysqlIp;database=dbName;uid=userName;pwd=password",
			// 是否自动初始化数据库
			"InitDb": true
		},
		// mongodb 配置
		"MongoDb": {
			// 是否启用mongodb
			"Enable": true,
			// mongodb 连接字符串
			"MongoConnection": "mongodb://username:password@mongoip:mongoPort/?authSource=databasename",
			//mongodb 集合名称
			"Database": "collectionName"
		},
		// redis 配置
		"RedisConfig": {
			// redis 连接字符串
			"Connection": "redisIp,abortConnect=false,password=redisPassword,defaultdatabase=默认启用的数据库",
			// redis 前缀
			"InstanceName": "redisPrefix"
		}
	}
}
```
