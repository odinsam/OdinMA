using OdinPlugs.OdinMAF.OdinEF.EFCore.EFExtensions;
using OdinPlugs.OdinMvcCore.OdinInject;
using OdinPlugs.OdinNetCore.OdinAssembly;
using OdinPlugs.OdinNetCore.OdinSnowFlake.SnowFlakeInterface;
using OdinPlugs.OdinSqlSugar.SqlSugarExtends;
using OdinPlugs.OdinSqlSugar.SqlSugarInterface;
using OdinPlugs.OdinSqlSugar.SqlSugarUtils;
using SqlSugar;
using SqlSugar.IOC;

namespace OdinCore.Models.DbModels
{
    public class OdinProjectSugarDbContext
    {
        SqlSugarClient db;
        public OdinProjectSugarDbContext()
        {
            db = DbScoped.Sugar;
        }
        public void CreateTable(string databaseName, bool Backup = false)
        {
            var flag = false;
            try
            {
                db.DbMaintenance.GetDataBaseList(db).Contains(databaseName);
                System.Console.WriteLine($"【 数据库已存在，无需新建 】"); ;
            }
            catch
            {
                System.Console.WriteLine($"【 自动创建数据库 】"); ;
                flag = db.DbMaintenance.CreateDatabase(databaseName);
            }
            finally
            {
                var types = this.GetType().Assembly.GetAllTypes<IDbTable>();
                if (Backup)
                {
                    foreach (var item in types)
                    {
                        if (!OdinSqlSugarUtils.CheckTable(item))
                        {
                            DbScoped.Sugar.CodeFirst.BackupTable().InitTables(item);
                            System.Console.WriteLine($"创建数据表【 {item.ToString()} 】"); ;
                        }

                    }

                }
                else
                {
                    foreach (var item in types)
                    {
                        if (!OdinSqlSugarUtils.CheckTable(item))
                        {
                            DbScoped.Sugar.CodeFirst.InitTables(item);
                            System.Console.WriteLine($"创建数据表【 {item.ToString()} 】");
                        }
                    }
                }
                if (flag)
                {
                    SampleData.Init();
                    System.Console.WriteLine($"数据库【 数据初始化 】");
                }

            }
        }

        public SqlSugarBaseRepository<Aop_ApiInvokerCatch_DbModel> ApiInvokerCatchs { get { return new SqlSugarBaseRepository<Aop_ApiInvokerCatch_DbModel>(db); } }

        public SqlSugarBaseRepository<Aop_ApiInvokerRecord_DbModel> ApiInvokerRecords { get { return new SqlSugarBaseRepository<Aop_ApiInvokerRecord_DbModel>(db); } }

        public SqlSugarBaseRepository<ErrorCode_DbModel> ErrorCodes { get { return new SqlSugarBaseRepository<ErrorCode_DbModel>(db); } }

        public SqlSugarBaseRepository<Aop_ApiInvokerException_DbModel> ApiInvokerExceptions { get { return new SqlSugarBaseRepository<Aop_ApiInvokerException_DbModel>(db); } }
    }
}