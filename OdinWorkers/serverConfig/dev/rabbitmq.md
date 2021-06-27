{
	"ProjectConfigOptions": {
		"RabbitMQ": {
			"Account": {
				"UserName": "UserName",
				"Password": "Password~"
			},
			"HostNames": ["ip"],
			"Exchanges": [
				{
					"ExchangeName": "canal-exchange",
					"ExchangeType": "topic",
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
							"RoutingKey": "canal.#",
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
				},
				{
					"ExchangeName": "aop_ApiInvoker_Exchange",
					"ExchangeType": "topic",
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
							"QueuesName": "aop_ApiInvoker_Queues",
							"Durability": true,
							"RoutingKey": "aop.ApiInvoker.#",
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
