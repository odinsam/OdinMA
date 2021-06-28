{
	"ProjectConfigOptions": {
		"FrameworkConfig": {
			"ApiSecurity": true
		},
		"DbEntity": {
			"ConnectionString": "Server=ip;database=dbname;uid=username;pwd=password",
			"InitDb": true
		},
		"MongoDb": {
			"Enable": true,
			"MongoConnection": "mongodb://username:password@ip:port/?authSource=collectionname",
			"Database": "dbname"
		},
		"Redis": {
			"Enable": true,
			"RedisIp": "ip",
			"RedisPort": port,
			"RedisPassword": "password",
			"DefaultDatabase": 0,
			"Connection": "ip,abortConnect=false,password=password,defaultdatabase=1",
			"InstanceName": "prefix"
		}
	}
}
