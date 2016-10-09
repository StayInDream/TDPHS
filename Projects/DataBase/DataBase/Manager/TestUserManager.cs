using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Model;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace DataBase.Manager
{
    class TestUserManager
    {
        public IList<TestUser> GetAllUser()
        {
            using(var session = NHibernateHelper.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var userlist = session.QueryOver<TestUser>();
                    transaction.Commit();
                    return userlist.List();
                }
            }
        }

        public IList<TestUser> GetUserByUsername( string username )
        {
            using(var session = NHibernateHelper.OpenSession())
            {
                using(var transaction = session.BeginTransaction())
                {
                    var userlist = session.QueryOver<TestUser>().Where( user => user.Username == username );
                    transaction.Commit();
                    return userlist.List();
                }
            }
        }


        static void Main( string[] args )
        {
            TestUserManager testUserManager = new TestUserManager();
            IList<TestUser> testusetList = testUserManager.GetAllUser();

            //foreach(TestUser item in testusetList)
            //{
            //    Console.WriteLine( item.Username );
            //}
            //Console.ReadKey();
        }
    }
}
