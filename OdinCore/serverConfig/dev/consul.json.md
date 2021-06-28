{
	"ProjectConfigOptions": {
		"Domain": {
			"Protocol": "Http",
			"IpAddress": "ip",
			"Port": port
		},
		"Consul": {
			"Enable": true,
			"Protocol": "Http",
			"ConsulName": "Odin-ConfigCore",
			"ConsulIpAddress": "ip",
			"ConsulPort": port,
			"DataCenter": "dc1",
			"Weight": 100,
			"ConsulCheck": {
				"DeregisterCriticalServiceAfter": 5,
				"HealthApi": "http://127.0.0.1:20101/odinCore/api/health",
				"Interval": 10,
				"Timeout": 5
			}
		}
	}
}
