### web config 模板说明

1. 配置文件在 dev.json 或者 pro.json 文件中都可以重新配置

2. dev.json 或者 pro.json 文件中的配置都会覆盖 cnf.json 的配置

3. 如果需要自定义配置 可以添加自定义配置文件 \*.json 文件

   > 3.1 文件根节点格式必须如下

   ```json
   {
   	"ProjectConfigOptions": {
   		// 自定义配置
   	}
   }
   ```

4. 创建 ProjectExtendsOptions.cs 代码如下:

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
				"RsaPublicKey": "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAwjMngZz86VCgPydKjuYL+Y6h9qyuO92dIwVONWwxtkCt+190+MOl7lz+EeguCxnlxu1Hqv5B3kmLv6+Fclp3HN3y9qHrf+zsEkF9YRWeOAXKEY8l44qUAoKxFg8eH2Ywjf98A7yjjyXKmXcns9aqOLq7QNpv1lukfqKe8igzjnhZQwRBL7HzMwL4X95SDZnznnlb1pRyy9w3k0dvYRU+O5oL9gYOp8Ffjg2hznaLKdzSXML8nZHgd3CuBa6wT54YWkiTIcMz19IWP61jdydJ4e/ZCULwHnZ4edXSXnFriH571PPovtKeimjhZ8QEnOkjCDndgvAtAIMcfPQzHLoteQIDAQAB",
				"RsaPrivateKey": "MIIEwAIBADANBgkqhkiG9w0BAQEFAASCBKowggSmAgEAAoIBAQDCMyeBnPzpUKA/J0qO5gv5jqH2rK473Z0jBU41bDG2QK37X3T4w6XuXP4R6C4LGeXG7Ueq/kHeSYu/r4VyWncc3fL2oet/7OwSQX1hFZ44BcoRjyXjipQCgrEWDx4fZjCN/3wDvKOPJcqZdyez1qo4urtA2m/WW6R+op7yKDOOeFlDBEEvsfMzAvhf3lINmfOeeVvWlHLL3DeTR29hFT47mgv2Bg6nwV+ODaHOdosp3NJcwvydkeB3cK4FrrBPnhhaSJMhwzPX0hY/rWN3J0nh79kJQvAednh51dJecWuIfnvU8+i+0p6KaOFnxASc6SMIOd2C8C0Agxx89DMcui15AgMBAAECggEBAIBnnHaaG6wVcG9xhl3oBAD1a2gUE3xn8w+F4Yl3SYNtTtyH6GrXym5KNIAYmeEjNYgLujh4t4rH7ExJPR44bMu8gXrL41AJkqobfVBlH+GBCnQDx5SlO2pRic/BHTS7t90cEZ7S6v6qiURhwfGqPZEW/ttJ02rT23cUBa6uIDDRgaUDIGnBvhH5wZPIzVpLy/sZkSq9Wvi4/vnZG+1bw3pty8xdvtS93/pMTTJC9w7cKsS8usW9097Y0scN6Xz1CqvPqXmXJ1gj81SXqpgbYwIQw96UXOHTQ1JXdNKjqy7Ru5X60g0kr23qsJUb6+xqV8M52p9bn3LhNdr6+Z+uRgECgYEA4tKGHY0U8LJSUoHRnEheBGqJ1l9S/cAVe2jei4IFpOKjGVufgu5OdNiCReLc22oQryx2UGapW5RSoNrF3CCKlv+GKp5llXy5JT5EVi4NE133kL1j3hTVKBUtOGqtveZJJD3B4ZmWMVN/3qXBPiAApIIMAqyzIKJm6586owH4lzkCgYEA2y5W2CWWQhzr0ozhiRVvynaYZftQbdsXQNw7bO2XAuBVOliCI8HVbhWbww3k0DaYQ4i6v6Lncjh8av66M6dZRBwhJ6UKwEnErretB/hNaKJTPQKz4dXUigaILD60/6LURQxEQ9FxNvvbEkvgcf5T0a2XhwXFSzACFg5AhATyCEECgYEAgjZnZdmTNua4GcOLCNQGnTRoMtgAdcnqyzEV6TLYeKLfDPSraufRxIRyrRoivhdywo2c0mVNPlS05sERK2DHDw5cAGV0Xc//HeFUK8E+Imskb0Q6a8i74cnebu/XFBh6zsJhZljcfy63TqMLf9WxaL97k6F/J/Hzcbq0V9YO1wkCgYEAzjz9RKehipYyJE2iDt6X8VfEsPlYRfaJG+PfGvE5TmCOcbdExaanUM2OYmyZaH5mqe1nY9lClQynrfoAdYg7i8Y4QFihFCyVoJ1+eGhhVlqfnmIDpzYQpsvcqF1c7MpnmI0hUvsp38X+7mltSXY5oqwIT+nTKpuC6wQAGvmYUMECgYEAj7Kl6LFi/LBcqf1VrJuJMUev8l5qhLc/5/41lzikUAMgy52e4Pu6g5zNKhRsIAR+xTtqkLoHQzoD9/uV2sI00oy1JHNoAX5FyQsEGpAJVJsK58C//GrNrRUIJBTvYzDg2Bz2sJFGdzMhMS+jvxZB4tK6Ij1GXdCfPqVu5rkndx4="
			}
		},
		// 项目url验签
		"ParamsSign": {
			"Enable": true,
			"signKey": "dd8404ec0e4b085798a5477c0640ca55"
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
