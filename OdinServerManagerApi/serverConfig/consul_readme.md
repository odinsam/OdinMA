## 配置文件详细说明

### 1. cnf.json 配置文件详细说明:

```json
{
	"ProjectConfigOptions": {
		"Domain": {
			"Protocol": "Http",
			"IpAddress": "121.42.15.95",
			"Port": 20101
		},
		"Consul": {
			"Enable": true,
			"Protocol": "Http",
			"ConsulName": "Odin-ConfigCore",
			"ConsulIpAddress": "121.42.15.95",
			"ConsulPort": 8500,
			"DataCenter": "dc1",
			"Weight": 100, // 服务权重
			"ConsulCheck": {
				"DeregisterCriticalServiceAfter": 5, // 多久后注销服务
				"HealthApi": "http://127.0.0.1:20101/odinCore/api/health", // 健康检查api
				"Interval": 10, // 心跳时间
				"Timeout": 5 // 超时时间
			}
		}
	}
}
```
