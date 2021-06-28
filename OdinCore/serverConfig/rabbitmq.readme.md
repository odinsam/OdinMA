## 配置文件详细说明

### 1. cnf.json 配置文件详细说明:

```json
{
	"ProjectConfigOptions": {
		"RabbitMQ": {
			"Account": {
				"UserName": "username", // 用户名
				"Password": "password" // 密码
			},
			"HostNames": ["rabbitmq ips"], // 服务器列表
			"Exchanges": [
				{
					"ExchangeName": "", // 交换机 名称
					"ExchangeType": "", // 交换机类型 Direct,Topic,Fanout,Headers
					"Durability": true, // 交换机是否持久化
					"AutoDelete": false, // 交换机是否自动删除
					/*
                    当交换器根据自身类型和路由key找不到一个合适的队列时，交换器就会把消息转交给备份交换器（备份交换器和普通交换器没有特殊之处，它也需要绑定队列）。
                    如果备份交换器根据自身类型和路由key也找不到合适的队列，那么消息就可能会丢失，
                    这里的是“可能"，因为如果在发送消息的时候指定了mandatory参数为true，
                    那么即使备份交换器找不到合适的队列，消息也会再次返回给消息发送者。
                    */
					"Arguments": { "KeyName": "alternate-exchange", "Value": "" }, //
					"Queues": [
						{
							"QueuesName": "Name",
							"Durability": true, // 队列是否持久化
							"RoutingKey": "",
							/*
                            消息是否自动删除
                            重启RabbitMQ服务器(可以通过rabbitmqctl stop_app关闭服务器，rabbitmqctl start_app重启服务器)，
                            可以登录RabbitMQ Management—> Queues中可以看到之前声明的队列还存在
                            */
							"AutoDelete": false,
							// 队列只对首次声明它的连接可见，并且在连接断开时自动删除
							"Exclusive": false,
							"Arguments": [
								// 统一设置队列中的所有消息的过期时间，例如设置10秒，10秒后这个队列的消息清零
								{ "KeyName": "x-message-ttl", "Value": 0 },
								/*
                                当多长时间没有消费者访问该队列的时候，该队列会自动删除，可以设置一个延迟时间，
                                如仅启动一个生产者，10秒之后该队列会删除，或者启动一个生产者，再启动一个消费者，消费者运行结束后10秒，队列也会被删除
                                */
								{ "KeyName": "x-expires", "Value": 0 },
								/*
                                指定队列的长度，如果不指定，可以认为是无限长，例如指定队列的长度是4，当超过4条消息，前面的消息将被删除
                                */
								{ "KeyName": "x-max-length", "Value": 0 },
								/*
                                指定队列存储消息的占用空间大小，当达到最大值是会删除之前的数据
                                */
								{ "KeyName": "x-max-length-bytes", "Value": 0 },
								/*
                                设置消息的优先级，优先级值越大，越被提前消费
                                */
								{ "KeyName": "x-max-priority", "Value": 0 },
								/*
                                当队列消息长度大于最大长度、或者过期的等，将从队列中删除的消息推送到指定的交换机中去而不是丢弃掉
                                */
								{ "KeyName": "x-dead-letter-exchange", "Value": "" },
								// 将删除的消息推送到指定交换机的指定路由键的队列中去
								{ "KeyName": "x-dead-letter-routing-key", "Value": "" },
								/*
                                先将消息保存到磁盘上，不放在内存中，当消费者开始消费的时候才加载到内存中
                                */
								{ "KeyName": "x-queue-mode", "Value": "lazy" },
								// 将队列设置为主位置模式，确定在节点集群上声明时队列主位置所依据的规则。
								{ "KeyName": "x-queue-master-locator", "Value": "" }
							]
						}
					]
				}
			]
		}
	}
}
```
