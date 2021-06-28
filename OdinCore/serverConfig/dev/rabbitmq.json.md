{
	"ProjectConfigOptions": {
		"RabbitMQ": {
			"Account": {
				"UserName": "user name",
				"Password": "user password~"
			},
            "VirtualHost": "/",
			"Port": 5672,
			"HostNames": ["ip"],
			"Exchanges": [
				{
					"ExchangeName": "canal-exchange",
					"ExchangeType": "direct",
					"Durability": true,
					"AutoDelete": false,
					"Arguments": [
						{
							"KeyName": "alternate-exchange",
							"Value": ""
						}
					],
					"Queues": [
						{
							"QueuesName": "canal-queues",
							"Durability": true,
							"RoutingKey": "canal-routingkey",
							"AutoDelete": false,
							"Exclusive": false,
							"Arguments": [
								{
									"KeyName": "x-message-ttl",
									"Value": 0
								},
								{
									"KeyName": "x-expires",
									"Value": 0
								},
								{
									"KeyName": "x-max-length",
									"Value": 0
								},
								{
									"KeyName": "x-max-length-bytes",
									"Value": 0
								},
								{
									"KeyName": "x-max-priority",
									"Value": 0
								},
								{
									"KeyName": "x-dead-letter-exchange",
									"Value": ""
								},
								{
									"KeyName": "x-dead-letter-routing-key",
									"Value": ""
								},
								{
									"KeyName": "x-queue-mode",
									"Value": ""
								},
								{
									"KeyName": "x-queue-master-locator",
									"Value": ""
								}
							]
						}
					]
				}
			]
		}
	}
}
