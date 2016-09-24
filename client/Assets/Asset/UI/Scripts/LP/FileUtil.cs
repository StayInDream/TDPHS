using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace LP
{
    /// <summary>
    /// 文件管理
    /// </summary>
    public static class FileUtil
    {

        public static void EnsureFile( string file )
        {
            if(!File.Exists( file ))
            {
                string folder = Path.GetDirectoryName( file );
                EnsureFolder( folder );
                File.Create( file ).Close();
            }
        }

        public static void EnsureFileParent( string file )
        {
            string folder = Path.GetDirectoryName( file );
            EnsureFolder( folder );
        }

        /// <summary>
        /// 在目标文件夹下创建某个类型的文件
        /// </summary>
        /// <param name="floderParent">目标文件夹</param>
        /// <param name="flie">文件，要带后缀</param>
        public static void EnsureFileParent( string floderParentPath, string flie )
        {
            EnsureFolder( floderParentPath );
            string p = Path.Combine( floderParentPath, flie );
            if(!File.Exists( p ))
            {
                File.Create( p ).Close();;
            }
        }
        /// <summary>
        /// 确保文件夹存在，若没有则创建
        /// </summary>
        /// <param name="folder"></param>
        public static void EnsureFolder( string folder )
        {
            if(!Directory.Exists( folder ))
                Directory.CreateDirectory( folder );
        }

        /// <summary>
        /// 删除文件夹及其文件夹内的所有东西
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="deleteSelf">目录文件夹是否要删除 </param>
        public static void DeleteFolder( string folder, bool deleteSelf )
        {
            string[] files = Directory.GetFiles( folder );
            for(int i = 0; i < files.Length; i++)
            {
                File.Delete( files[i] );
            }
            string[] dirs = Directory.GetDirectories( folder );
            for(int i = 0; i < dirs.Length; i++)
            {
                DeleteFolder( dirs[i], true );
            }
            if(deleteSelf)
                Directory.Delete( folder );
        }
    }

    /// <summary>
    /// 路径的斜杠处理
    /// </summary>
    public sealed class UrlUtil
    {
        public static string Combine( string url1, string url2 )
        {
            if(!url1.EndsWith( "/" ))
                url1 = url1 + "/";
            if(url2.StartsWith( "/" ))
                url2 = url2.Substring( 1 );
            return url1 + url2;
        }
    }
}

