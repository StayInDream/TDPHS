using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace DataBase
{
    /// <summary>
    /// 数据库的映射类
    /// </summary>
    public class NHibernateHelper
    {
        private static ISessionFactory sessionFactory = null;
        private static void InitSessionFactory()
        {
            MySQLConfiguration configuration = MySQLConfiguration.Standard.ConnectionString( db => db.Server("localhost" ).Database( "泰斗" ).Username( "root" ).Password( "root" ) );


            sessionFactory = Fluently.Configure().Database( configuration ).Mappings( x => x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>() ).BuildSessionFactory();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if(sessionFactory == null)
                {
                    InitSessionFactory();
                }
                return sessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
