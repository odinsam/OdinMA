# OdinMA

## OdinMA-微服务架构（Microservice Architecture）

### 项目简介:

1. 项目包含 Ocelot 网关、Consul 服务注册发现，结合 IdentityServer4 身份验证

2. 整体项目有后台管理，可以配置对应项目并远程重启

3. 项目采用.net core 5.0 框架

4. 项目部分说明在 [OdinSam's blog](https://www.odinsam.com) 的博客中有文章说明

### 目录介绍:

|     | 名称                 | 说明                         | 类型   | ip           | url                     | port  |
| --- | :------------------- | :--------------------------- | :----- | :----------- | :---------------------- | ----- |
| 1   | OdinOIS              | Ocelot+IdentityServer 服务器 | WebApi | 121.42.15.95 | https://ois.odinsam.com | 25050 |
| 2   | OdinConsul           | Consul 服务器                | WebApi |
| 3   | OdinServerManager    | 整体服务后台管理             | UI     |
| 4   | OdinServerManagerApi | 整体服务后台管理             | WebApi |
| 5   | OdinCoreConfig       | 整体服务配置                 | WebApi |
| 6   | OdinServiceServer    | 业务服务器                   | WebApi |
