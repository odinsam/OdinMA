using System.Linq;
using Microsoft.EntityFrameworkCore;
using OdinPlugs.OdinSecurity;

namespace OdinOIS.Models.DbModels.IdentityUserStore
{
    public class UserStore
    {
        private readonly OdinIdentityEntities _dbContext;
        public UserStore(OdinIdentityEntities dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 根据SubjectID查询用户信息
        /// </summary>
        /// <param name="subjectId">用户id</param>
        /// <returns></returns>
        public IdUser FindBySubjectId(string subjectId)
        {
            return _dbContext.Set<IdUser>().Where(r => r.SubjectId.Equals(subjectId)).Include(r => r.IdentityUserClaims).SingleOrDefault();
        }
        /// <summary>
        /// 根据用户名查询用户
        /// </summary>
        /// <param name="username">用户</param>
        /// <returns></returns>
        public IdUser FindByUsername(string username)
        {
            return _dbContext.Set<IdUser>().Where(r => r.UserName.Equals(username)).Include(r => r.IdentityUserClaims).SingleOrDefault();
        }
        /// <summary>
        /// 验证登录密码
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidateCredentials(string username, string password)
        {
            password = password.ToMd5Lower();
            var user = _dbContext.Set<IdUser>().Where(r => r.UserName.Equals(username)
            && r.Password.Equals(password)).Include(r => r.IdentityUserClaims).SingleOrDefault();
            return user != null;
        }
    }
}