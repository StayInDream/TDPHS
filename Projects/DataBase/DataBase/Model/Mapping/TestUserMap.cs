using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;


namespace DataBase.Model.Mapping
{
    /// <summary>
    /// TestUser的一个对象
    /// </summary>
    class TestUserMap : ClassMap<TestUser>
    {

        public TestUserMap()
        {
            Id( x => x.Id ).Column( "id" );//设置id属性为主键
            Map( x => x.Username ).Column( "username" );
            Map( x => x.Password ).Column( "password" );
            Map( x => x.Age ).Column( "age" );
            Table("testuser"); //映射的库
        }
    }
}
