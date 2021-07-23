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

### 框架介绍 https://github.com/odinsam/OdinMA

|     | 名称                 | 说明                      |  状态  |
| :-: | :------------------- | :------------------------ | :----: |
|  1  | odinCore             | webApi 演示项目           | 更新中 |
|  2  | OdinWorkers          | 后台服务 演示项目         | 更新中 |
|  3  | OdinOIS              | id4 演示项目              | 未开始 |
|  4  | OdinServerManagerApi | 框架管理                  | 未开始 |
|  5  | ~~OdinHangFire~~     | ~~HangFire 后台框架项目~~ | 已弃用 |

### 封装 package 包

|     | 名称                               | 说明                                                                           |   状态   | version |
| :-: | :--------------------------------- | :----------------------------------------------------------------------------- | :------: | :-----: |
|  1  | OdinPlugs                          | [综合使用封装框架](https://github.com/odinsam/OdinPlugs)                       |  更新中  |  1.0.5  |
|  2  | OdinPlugs.ApiLinkMonitor           | [链路监控框架](https://github.com/odinsam/OdinPlugs.ApiLinkMonitor)            | 基本完成 |  1.0.5  |
|  3  | OdinPlugs.ApiLinkMonitor.Dashboard | [链路监控 - UI](https://github.com/odinsam/OdinPlugs.ApiLinkMonitor.Dashboard) |  未开始  |  1.0.0  |
|  4  | OdinPlugs.OdinHostedService        | [后台服务框架](https://github.com/odinsam/OdinPlugs.OdinHostedService)         | 基本完成 |  1.0.5  |
|  5  | OdinPlugs.OdinInject               | [核心注入框架](https://github.com/odinsam/OdinPlugs.OdinInject)                | 基本完成 |  1.0.5  |
|  5  | OdinPlugs.OdinUtils                | [核心工具扩展框架](https://github.com/odinsam/OdinPlugs.OdinUtils)             | 基本完成 |  1.0.5  |
|  5  | OdinPlugs.SnowFlake                | [雪花 Id 封装](https://github.com/odinsam/OdinPlugs.SnowFlake)                 | 基本完成 |  1.0.5  |

具体内容可以参见对应项目的 readme 和 changelog
