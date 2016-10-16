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
            using ( var session = NHibernateHelper.OpenSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {
                    var userlist = session.QueryOver<TestUser>();
                    transaction.Commit();
                    return userlist.List();
                }
            }
        }

        public IList<TestUser> GetUserByUsername( string username )
        {
            using ( var session = NHibernateHelper.OpenSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {
                    var userlist = session.QueryOver<TestUser>().Where( user => user.Username == username );
                    transaction.Commit();
                    return userlist.List();
                }
            }
        }

        public void SaveUser( TestUser user )
        {
            using ( var session = NHibernateHelper.OpenSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {
                    session.Save( user );
                    transaction.Commit();
                }
            }
        }

        public void DeleteById( int id )
        {
            using ( var session = NHibernateHelper.OpenSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {
                    TestUser user = new TestUser();
                    user.Id = id;
                    session.Delete( user );
                    transaction.Commit();
                }
            }
        }
        public void UpdataUser( TestUser user )
        {
            using ( var session = NHibernateHelper.OpenSession() )
            {
                using ( var transaction = session.BeginTransaction() )
                {
                    session.Update( user );
                    transaction.Commit();
                }
            }
        }

        static void Main( string[] args )
        {
            TestUserManager testUserManager = new TestUserManager();
            IList<TestUser> testusetList = testUserManager.GetAllUser();

            foreach ( TestUser item in testusetList )
            {
                Console.WriteLine( item.Username );
            }

            Console.WriteLine( "save..." );

            TestUser user = new TestUser();
            user.Username = "e";
            user.Password = "3333";
            user.Age = 55;
            testUserManager.SaveUser( user );
            Console.WriteLine( "save end..." );

            Console.WriteLine( "delete..." );

            // testUserManager.DeleteById( 2 );

            Console.WriteLine( "updata..." );
            testusetList[0].Age = 1000;
            testUserManager.UpdataUser( testusetList[0] );

            Console.ReadKey();
        }
    }
}
