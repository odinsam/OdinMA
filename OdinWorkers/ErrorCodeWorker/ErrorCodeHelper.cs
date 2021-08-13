using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using OdinPlugs.OdinInject.InjectPlugs.OdinCacheManagerInject;
using OdinPlugs.OdinInject.InjectPlugs.OdinCanalInject;
using OdinPlugs.OdinInject.Models.CanalModels;
using OdinPlugs.OdinUtils.OdinExtensions.BasicExtensions.OdinObject;
using OdinPlugs.OdinWebApi.OdinCore.Models.ErrorCode;

namespace OdinWorkers.ErrorCodeWork
{
    public class ErrorCodeWorker
    {
        public static void ErrorCodeCanalHandler(IOdinCanal canalHelper, IOdinCacheManager cacheManager, OdinCanalModel canalModel)
        {
            var obj = canalModel;
            var type = obj.type;
            switch (type.ToLower())
            {
                case "insert":
                    {
                        var models = ConvertCanalDataToErrorCodeModel(obj);
                        foreach (var model in models)
                        {
                            var flag = true;
                            if (cacheManager.Exists(model.ErrorCode))
                            {
                                flag = cacheManager.Cover(model.ErrorCode, model);
                                if (flag)
                                    System.Console.WriteLine($"cacheManager add-Cover success:\r\n{model.ToJson(enumStringFormat.Json)}\r\n");
                                else
                                {
                                    System.Console.WriteLine($"cacheManager add-Cover fail:\r\n{model.ToJson(enumStringFormat.Json)}");
                                }
                            }
                            else
                            {
                                flag = cacheManager.Add(model.ErrorCode, model);
                                if (flag)
                                    System.Console.WriteLine($"cacheManager add success:\r\n{model.ToJson(enumStringFormat.Json)}\r\n");
                                else
                                {
                                    System.Console.WriteLine($"cacheManager add fail:\r\n{model.ToJson(enumStringFormat.Json)}");
                                }
                            }
                        }
                    }
                    break;
                case "update":
                    {
                        var models = ConvertCanalDataToErrorCodeModel(obj);
                        foreach (var model in models)
                        {
                            var flag = cacheManager.Cover<ErrorCode_Model>(model.ErrorCode, model);
                            if (flag)
                                System.Console.WriteLine($"cacheManager Cover success:\r\n{model.ToJson(enumStringFormat.Json)}\r\n");
                            else
                            {
                                System.Console.WriteLine($"cacheManager Cover fail:\r\n{model.ToJson(enumStringFormat.Json)}");
                            }
                        }
                    }
                    break;
                case "delete":
                    {
                        var errorCodes = GetErrorCode(obj);
                        foreach (var errorCode in errorCodes)
                        {
                            bool flag = cacheManager.Delete(errorCode);
                            if (flag)
                                System.Console.WriteLine($"cacheManager delete success:\r\n{errorCode}\r\n");
                            else
                                System.Console.WriteLine($"cacheManager delete fail:\r\n{errorCode}");
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        private static List<string> GetErrorCode(OdinCanalModel obj, int index = 0)
        {
            List<string> errorCodes = new List<string>();
            for (int i = 0; i < obj.data.Length; i++)
            {
                errorCodes.Add(obj.data[i].GetValue("ErrorCode").ToString());
            }
            return errorCodes;
        }
        private static List<ErrorCode_Model> ConvertCanalDataToErrorCodeModel(OdinCanalModel obj)
        {
            var models = new List<ErrorCode_Model>();
            for (int i = 0; i < obj.data.Length; i++)
            {
                var id = Convert.ToInt64(obj.data[i].GetValue("Id"));
                var errorCode = obj.data[i].GetValue("ErrorCode").ToString();
                var codeShowMessage = obj.data[i].GetValue("CodeShowMessage").ToString();
                var codeErrorMessage = obj.data[i].GetValue("CodeErrorMessage").ToString();
                ErrorCode_Model model = new ErrorCode_Model()
                {
                    Id = id.ToString(),
                    ErrorCode = errorCode,
                    ErrorMessage = codeErrorMessage,
                    ShowMessage = codeShowMessage,
                };
                models.Add(model);
            }
            return models;
        }
    }
}