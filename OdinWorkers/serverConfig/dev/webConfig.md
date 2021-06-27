{
    "ProjectConfigOptions": {
        "ApiSecurity": {
            "Enable": true
        },
        "DbEntity": {
            "ConnectionString": "Server=ip;database=dbname;uid=uid;pwd=pwd",
            "InitDb": true
        },
        "MongoDb": {
            "Enable": true,
            "MongoConnection": "mongodb://uname:pwd@ip:port/?authSource=collectionname",
            "Database": "dbname"
        },
        "Redis": {
            "Enable": true,
            "RedisIp": "ip",
            "RedisPort": port,
            "RedisPassword": "pwd",
            "DefaultDatabase": 0,
            "Connection": "ip,abortConnect=false,password=pwd,defaultdatabase=1",
            "InstanceName": "prefix"
        }
    }
}
    